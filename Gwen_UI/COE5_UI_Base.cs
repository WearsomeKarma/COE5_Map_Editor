
using Gwen.Net;
using Gwen.Net.CommonDialog;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;
using Gwen.Net.Xml;

namespace COE5_Map_Editor;

public class COE5_UI_Base : ControlBase
{
    private readonly ControlBase m_COE5_Render_Viewport_Container;
    private readonly COE5_Map_View m_COE5_Render_Viewport;
    public int Render_View__X
        => m_COE5_Render_Viewport_Container.ActualPosition.X;
    public int Render_View__Y
        => m_COE5_Render_Viewport_Container.ActualPosition.Y;
    public int Render_View__Width
        => m_COE5_Render_Viewport.ActualSize.Width;
    public int Render_View__Height
        => m_COE5_Render_Viewport.ActualSize.Height;

    public event EventHandler? Rendering_Map;
    public event Handle_UI_Event? Plane_Changed;

    public event EventHandler? Mouse_Focused_Viewport;
    public event EventHandler? Mouse_Defocused_Viewport;

    public delegate void Handle_UI_Event
    (
        string path, 
        Action<string> error_callback
    );

    public event Action? UI_Busy;
    public event Action? UI_Free;

    public event Handle_UI_Event? Map_Loaded;
    public event Handle_UI_Event? Map_Saved;
    public event Handle_UI_Event? Data_Directory_Set;
    public event Handle_UI_Event? Roaming_Directory_Set;
    public event Handle_UI_Event? Active_Plane_Changed;

    public event Func<IEnumerable<KeyValuePair<Input_Binding, Operation_Info>>>?
        Requesting_Bindings;

    private string? m_Map_Directory;

    private readonly Dialog_Bindings.Validate__Binding_Request 
        m_Validate__Binding_Request;
    private readonly Dialog_Bindings.Validate__Binding_Transition 
        m_Validate__Binding_Transition;

    private IEnumerable<KeyValuePair<Input_Binding, Operation_Info>> m_Bindings;

    private DockBase m_Dock;
    private MenuStrip m_Menu_Strip;

    public COE5_UI_Base
    (
        ControlBase parent, 
        Dialog_Bindings.Validate__Binding_Request validate__binding_request,
        Dialog_Bindings.Validate__Binding_Transition validate__binding_transition,
        IEnumerable<KeyValuePair<Input_Binding, Operation_Info>> bindings,
        string? map_directory = null
    )
    : base(parent)
    {
        m_Validate__Binding_Request = 
            validate__binding_request;
        m_Validate__Binding_Transition =
            validate__binding_transition;

        m_Bindings = bindings;

        // TODO: move everything below to Xml

        m_Map_Directory = map_directory;
        Dock = Dock.Fill;

        m_Dock = new DockBase(this);
        m_Dock.Dock = Dock.Fill;

        m_Menu_Strip = new MenuStrip(this);
        m_Menu_Strip.Dock = Dock.Top;
        MenuItem menu_item__file = new MenuItem(m_Menu_Strip) { Text = "File" };
        menu_item__file.Menu.AddItem("Load Map").Clicked += Handle__Load_Map;
        menu_item__file.Menu.AddItem("Save Map").Clicked += Handle__Save_Map;
        menu_item__file.Menu.AddItem("Select Data Directory").Clicked += Handle__Set_Data_Directory;
        menu_item__file.Menu.AddItem("Select Map Directory").Clicked += Handle__Set_Map_Directory;
        m_Menu_Strip.AddItem(menu_item__file);
        MenuItem menu_item__edit = new MenuItem(m_Menu_Strip) { Text = "Edit" };
        menu_item__edit.Menu.AddItem("Edit Keybindings");

        StatusBar status_bar = new StatusBar(this);

        m_Dock.LeftDock.TabControl.AddPage("Tools");

        m_COE5_Render_Viewport_Container = new DockLayout(m_Dock);
        m_COE5_Render_Viewport_Container.Dock = Dock.Fill;

        m_COE5_Render_Viewport = new COE5_Map_View(m_COE5_Render_Viewport_Container);
        m_COE5_Render_Viewport.Rendering_Map +=
            (s, e) => Rendering_Map?.Invoke(s,e);
        m_COE5_Render_Viewport.Plane_Changed +=
            (s, plane) => Plane_Changed?.Invoke(plane, Handle__Display_Error);
        m_COE5_Render_Viewport.Mouse_Focused +=
            (s, e) => Mouse_Focused_Viewport?.Invoke(s, e);
        m_COE5_Render_Viewport.Mouse_Defocused +=
            (s, e) => Mouse_Defocused_Viewport?.Invoke(s, e);

        // Even this stuff I think. (to move to Xml)
    }

    private void Disable__Base_UI(bool make_busy)
    {
        Console.WriteLine("disable");
        Gwen_Helper.Operate_Recursively_On<Button>
        (
            this,
            control => control.MouseInputEnabled = false
        );
        // For some reason, I can't tell why
        // TabStrip's parent get's silently changed
        // so I have to operate on it explicitly here.
        Gwen_Helper.Operate_Recursively_On<Button>
        (
            m_COE5_Render_Viewport.TabStrip,
            control => control.MouseInputEnabled = false
        );
        if (make_busy)
            UI_Busy?.Invoke();
    }

    private void Enable__Base_UI(bool make_free)
    {
        Gwen_Helper.Operate_Recursively_On<Button>
        (
            this,
            control => control.MouseInputEnabled = true
        );
        Gwen_Helper.Operate_Recursively_On<Button>
        (
            m_COE5_Render_Viewport.TabStrip,
            control => control.MouseInputEnabled = true
        );
        if (make_free)
            UI_Free?.Invoke();
    }

    public void Update_Planes(IEnumerable<string> planes)
        => m_COE5_Render_Viewport.Set_Planes(planes);

    private void Handle__Edit_Keybindings(ControlBase sender, ClickedEventArgs e)
    {

    }

    private void Handle__Load_Map(ControlBase sender, ClickedEventArgs e)
    {
        void callback(string map_path)
            => Map_Loaded?.Invoke(map_path, Handle__Display_Error);

        Handle__Dialog<OpenFileDialog>(sender, "Load a COE5 Map", callback, m_Map_Directory);
    }

    private void Handle__Save_Map(ControlBase sender, ClickedEventArgs e)
    {
        void callback(string map_path)
            => Map_Saved?.Invoke(map_path, Handle__Display_Error);

        Handle__Dialog<SaveFileDialog>(sender, "Save COE5 Map", callback, m_Map_Directory);
    }

    private void Handle__Set_Data_Directory(ControlBase sender, ClickedEventArgs e)
    {
        void callback(string data_directory)
            => Data_Directory_Set?.Invoke(data_directory, Handle__Display_Error);

        Handle__Dialog<FolderBrowserDialog>(sender, "Locate COE5, Data Directory", callback);
    }

    private void Handle__Set_Map_Directory(ControlBase sender, ClickedEventArgs e)
    {
        void callback(string roaming_directory)
            => Roaming_Directory_Set?.Invoke(roaming_directory, Handle__Display_Error);

        Handle__Dialog<FolderBrowserDialog>(sender, "Locate COE5 Roaming Directory", callback);
    }

    private void Handle__Dialog<TDialog>
    (
        ControlBase sender, 
        string title, 
        Action<string> callback,
        string? initial_folder = null
    )
    where TDialog : FileDialog
    {
        Disable__Base_UI(true);
        FileDialog dialog = Component.Create<TDialog>(sender);
        dialog.InitialFolder =
            initial_folder
            ??
            Environment.CurrentDirectory;

        dialog.Title = title;
        dialog.Callback += path =>
        {
            if (path != null) 
                callback(path);
            Enable__Base_UI(true);
        };
    }

    private void Handle__Display_Error(string message)
    {
        MessageBox error_box = new MessageBox(this, message, "Error");
    }
}
