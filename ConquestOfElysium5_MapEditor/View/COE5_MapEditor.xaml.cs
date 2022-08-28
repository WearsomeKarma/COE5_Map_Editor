using ConquestOfElysium5_MapEditor.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Threading;

namespace ConquestOfElysium5_MapEditor.View
{
    /// <summary>
    /// Interaction logic for COE5_MapEditor.xaml
    /// </summary>
    public partial class COE5_MapEditor : UserControl
    {
        private readonly GLControl GLControl;
        private COE5_Textures COE5_Textures;
        private COE5_Plane_Render COE5_Plane_Render;

        private readonly DispatcherTimer GL_Timer = new DispatcherTimer();

        private readonly Dictionary<TabItem, COE5_Plane> Plane_Table =
            new Dictionary<TabItem, COE5_Plane>();

        private COE5_Plane _Active_Plane;
        private Matrix4 _Projection;
        private Matrix4 _View;

        private float _Pos_X, _Pos_Y;

        // ratio of map size.
        private const float MOVE_RATE = 0.0005f;

        // ratios of map size.
        private const float MIN_POS = -4f;
        private const float MAX_POS =  4f;

        private const float MAX_ZOOM = 3f;
        private const float MIN_ZOOM = 0.2f;

        private const float ZOOM_RATE = 0.05f;

        private float _Camera_X = 0;
        private float _Camera_Y = 0;
        private float _Camera_Zoom = 1;

        public COE5_MapEditor()
        {
            InitializeComponent();

            GLControl = new GLControl();

            GLControl.Load += GlControl_Load;
            GLControl.Paint += GlControl_Paint;
            GLControl.MouseWheel += GLControl_MouseWheel; ;
            GLControl.Dock = System.Windows.Forms.DockStyle.Fill;

            GL_Timer.Interval = TimeSpan.FromMilliseconds(1);
            GL_Timer.Tick += GL_Timer_Tick;
            GL_Timer.Start();

            Tab_Planes.SelectionChanged += Tab_Planes_SelectionChanged;
        }

        private void GLControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            float zoom = _Camera_Zoom;

            zoom +=
                (e.Delta < 0)
                ? ZOOM_RATE
                : -ZOOM_RATE
                ;

            zoom =
                (zoom < MIN_ZOOM)
                ? MIN_ZOOM
                : (zoom > MAX_ZOOM)
                    ? MAX_ZOOM
                    : zoom
                    ;

            _Camera_Zoom = zoom;
        }

        private void Tab_Planes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem plane_tab =
                e.AddedItems[0] as TabItem;

            if (plane_tab == null) return;

            _Active_Plane = Plane_Table[plane_tab];
            WindowsFormsHost host =
                plane_tab.Content as WindowsFormsHost;
            host.Child = GLControl;
            plane_tab.Focus();
        }

        public void Load_Map(COE5_Map map)
        {
            foreach (COE5_Plane plane in map)
            {
                if (_Active_Plane == null)
                    _Active_Plane = plane;
                Add_Plane(plane);
            }
        }

        private void Add_Plane(COE5_Plane plane)
        {
            WindowsFormsHost host = new WindowsFormsHost();
            host.VerticalAlignment = VerticalAlignment.Stretch;
            host.HorizontalAlignment = HorizontalAlignment.Stretch;
            
            TabItem plane_tab_item =
                new TabItem()
                {
                    Header =
                    new TextBlock() { Text = plane.Plane_Type.ToString() }
                };

            Tab_Planes.Items.Add(plane_tab_item);
            plane_tab_item.Content = host;
            host.Child = GLControl;

            Plane_Table
                .Add(plane_tab_item, plane);

            plane_tab_item.Focus();
        }

        private void GL_Timer_Tick(object sender, EventArgs e)
        {
            GLControl.Invalidate();
        }

        private DateTime _Previous_Frame_Time = DateTime.Now;
        private DateTime _FPS_Measure = DateTime.Now;
        private int _Frames;
        private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            TimeSpan delta_time = DateTime.Now.Subtract(_Previous_Frame_Time);
            _Previous_Frame_Time = DateTime.Now;
            if (DateTime.Now.Subtract(_FPS_Measure) > TimeSpan.FromSeconds(1))
            {
                MainWindow.Post_Message($"fps: {_Frames}");
                _Frames = 0;
                _FPS_Measure = DateTime.Now;
            }

            Handle__Camera_Movement((float)delta_time.TotalSeconds);

            GLControl.MakeCurrent();

            _Projection = 
                Matrix4.CreateOrthographic
                (
                    _Active_Plane.Width * (1/_Camera_Zoom), 
                    -_Active_Plane.Height * (1/_Camera_Zoom), 
                    1000, 
                    -1000
                );
            _View =
                Matrix4.CreateTranslation
                (
                    new Vector3(_Pos_X, _Pos_Y, 0)
                );

            GL.Viewport(GLControl.Size);

            GL.ClearColor(0.05f, 0.05f, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (_Active_Plane != null)
                COE5_Plane_Render.Render_Plane(_Active_Plane, ref _Projection, ref _View);

            GLControl.SwapBuffers();
            _Frames++;
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            COE5_Textures = new COE5_Textures(COE5.Data_Directory);
            COE5_Plane_Render = new COE5_Plane_Render(COE5_Textures);
        }

        private void Handle__Camera_Movement(float delta_time)
        {
            System.Drawing.Point mouse_pos =
                GLControl.MousePosition;
            System.Windows.Forms.MouseButtons mouse_buttons =
                GLControl.MouseButtons;

            int largest_size =
                (_Active_Plane.Width < _Active_Plane.Height)
                ? _Active_Plane.Height
                : _Active_Plane.Width
                ;

            float x = _Pos_X, y = _Pos_Y;

            int delta_x =
                mouse_pos.X
                -
                GLControl.Width / 2
                ;
            int delta_y =
                mouse_pos.Y
                -
                GLControl.Height / 2
                ;

            switch (mouse_buttons)
            {
                case System.Windows.Forms.MouseButtons.Right:
                    x +=
                        largest_size
                        *
                        MOVE_RATE
                        *
                        Math.Sign(delta_x)
                        /
                        delta_time
                        ;
                    y +=
                        largest_size
                        *
                        MOVE_RATE
                        *
                        Math.Sign(delta_y)
                        /
                        delta_time
                        ;
                    break;
            }

            float min_bounds =
                MIN_POS
                *
                largest_size
                ;
            float max_bounds =
                MAX_POS
                *
                largest_size
                ;
            void clamp(ref float val)
            {
                val =
                    (val < MIN_POS)
                    ? MIN_POS
                    : (val > MAX_POS)
                        ? MAX_POS
                        : val
                        ;
            }

            clamp(ref x);
            clamp(ref y);

            _Pos_X = x;
            _Pos_Y = y;
        }

        private void Handle__Camera_Scroll()
        {

        }
    }
}
