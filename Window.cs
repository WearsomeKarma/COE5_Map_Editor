using COE5_Map_Editor.Core;
using System.Diagnostics.CodeAnalysis;
using Gwen.Net.OpenTk;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using COE5_Map_Editor.Core.Operations;
using COE5_Map_Editor.Core.Input;

namespace COE5_Map_Editor;

public class Window : GameWindow
{
    [AllowNull]
    COE5_UI_Base m_COE5_UI_Base;
    IGwenGui m_Editor_Gui;
    IGwenGui m_Loading_Gui;
    Loading_Screen m_Loading_Screen;

    Matrix4 m_Projection;
    Matrix4 m_View;
    COE5_Textures m_COE5_Textures;
    COE5_Plane_Render m_COE5_Plane_Renderer;

    COE5_Editor_State? m_Editor_State;
    private bool Is__Editor_Initalized
        => m_Editor_State != null;

    private bool Is__Loading = true;
    private bool Is__Performing_IO { get; set; }
    private bool Is__Awaiting_UI   { get; set; }
    private bool Is__Possessive_Of__Mouse_Focus { get; set; }
    private bool Is__Present__Active_Plane
        => m_Editor_State?.Plane__Active__Type != null;
    private bool Is__Accepting_Input
        => 
        Is__Possessive_Of__Mouse_Focus
        &&
        Is__Present__Active_Plane
        &&
        !Is__Awaiting_UI
        ;

    private Viewport m_Base_Viewport;

    public Window() 
    : base
    (
        GameWindowSettings.Default,
        new NativeWindowSettings()
        {
            WindowBorder = OpenTK.Windowing.Common.WindowBorder.Hidden,
            Size = new Vector2i(250, 250)
        }
    )
    {
        m_Editor_Gui = 
            GwenGuiFactory
            .CreateFromGame
            (
                this, 
                GwenGuiSettings
                    .Default
                    .From
                    (
                        settings => 
                        settings.SkinFile =
                        new System.IO.FileInfo("DefaultSkin2.png")
                    )
            );

        m_Loading_Gui =
            GwenGuiFactory
            .CreateFromGame
            (
                this,
                GwenGuiSettings
                    .Default
                    .From
                    (
                        settings => 
                        settings.SkinFile =
                        new System.IO.FileInfo("DefaultSkin2.png")
                    )
            );

        m_Base_Viewport =
            new Viewport(Size);
        GLHelper.Initalize(m_Base_Viewport);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        //TODO: route mouse hold???

        if(Is__Accepting_Input)
            m_Editor_State?
            .Operation_Controller
            .Process(m_Editor_State, null, e, KeyboardState, MouseState);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);
        //TODO: key press/hold

        if(Is__Accepting_Input)
            m_Editor_State?
            .Operation_Controller
            .Process(m_Editor_State, e, null, KeyboardState, MouseState);
    }

    protected override void OnLoad()
    {
        Console.WriteLine(">>>> LOADING <<<<");
        GL.ClearColor(0.4f, 0.4f, 0, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        m_Loading_Gui.Load();
        m_Loading_Gui.Resize(Size);

        m_Loading_Screen = new Loading_Screen(m_Loading_Gui.Root);

        m_Loading_Gui.Render();

        SwapBuffers();

        m_Editor_Gui.Load();

        COE5.Initalize();

        m_COE5_UI_Base = new COE5_UI_Base(m_Editor_Gui.Root, null, null, null, COE5.Roaming_Directory.Maps_Directory);

        m_COE5_Textures = new COE5_Textures(COE5.Data_Directory);
        m_COE5_Plane_Renderer = new COE5_Plane_Render(m_COE5_Textures);

        m_COE5_UI_Base.UI_Busy += () => Is__Awaiting_UI = true;
        m_COE5_UI_Base.UI_Free += () => Is__Awaiting_UI = false;
        m_COE5_UI_Base.Data_Directory_Set +=
            (directory, error_callback) =>
            {
                Is__Performing_IO = true;
                try
                {
                    COE5.Data_Directory =
                        COE5_Data_Directory
                        .Get__COE5_Data_Directory(directory);
                    m_COE5_Textures =
                        new COE5_Textures(COE5.Data_Directory);
                    m_COE5_Plane_Renderer =
                        new COE5_Plane_Render(m_COE5_Textures);
                }
                catch(Exception e)
                {
                    error_callback(e.Message);
                }
                Is__Performing_IO = false;
            };
        m_COE5_UI_Base.Roaming_Directory_Set +=
            (directory, error_callback) =>
            {
                Is__Performing_IO = true;
                try
                {
                    COE5.Roaming_Directory =
                        COE5_Roaming_Directory
                        .Get__COE5_Roaming_Directory(directory);
                }
                catch(Exception e)
                {
                    error_callback(e.Message);
                }
                Is__Performing_IO = false;
            };
        m_COE5_UI_Base.Map_Loaded +=
            Handle__Load_Map;
        m_COE5_UI_Base.Map_Saved +=
            Handle__Save_Map;
        m_COE5_UI_Base.Rendering_Map +=
            Handle__Render_Map;
        m_COE5_UI_Base.Plane_Changed +=
            Handle__Active_Plane_Changed;
        m_COE5_UI_Base.Mouse_Focused_Viewport +=
            (s, e) => Is__Possessive_Of__Mouse_Focus = true;
        m_COE5_UI_Base.Mouse_Defocused_Viewport +=
            (s, e) => Is__Possessive_Of__Mouse_Focus = false;

        Size = new Vector2i(800, 600);
        m_Base_Viewport.Resize(Size);
        m_Editor_Gui.Resize(Size);
        WindowBorder = OpenTK.Windowing.Common.WindowBorder.Resizable;
        CenterWindow();
    }

    private void Initialize__Editor
    (
        COE5_Map map
    )
    {
        if (m_Editor_State != null) return;

        m_Editor_State = 
            new COE5_Editor_State
            (
                map,
                new Operation_Controller
                (
                    Input_Scheme.Default,
                    new Operation[] 
                    {
                        new Operation__Move_To__Plane(),
                        new Operation__Tool__Use(),
                        new Operation__View__Pan(),
                        new Operation__View__Reset()
                    }
                )
            );
    }

    private void Handle__Load_Map(string map_path, Action<string> error_callback)
    {
        Is__Performing_IO = true;
        try 
        {
            COE5_Map map = COE5_Map.Load_Map(map_path);

            if (Is__Editor_Initalized)
                m_Editor_State!
                    .Map =
                    COE5_Map.Load_Map(map_path);
            else
                Initialize__Editor(map);

            m_COE5_UI_Base
                .Update_Planes
                (
                    m_Editor_State!
                    .Map
                    .Select(plane => plane.Plane_Type.ToString())
                );
        }
        catch(Exception e)
        {
            error_callback(e.Message);
        }
        Is__Performing_IO = false;
    }

    private void Handle__Save_Map(string map_path, Action<string> error_callback)
    {
        error_callback("Not Implemented Yet.");
    }

    private void Handle__Active_Plane_Changed(string _plane_type, Action<string> error_callback)
    {
        COE5_Plane_Type plane_type;
        if (!Enum.TryParse(_plane_type, out plane_type))
        {
            error_callback.Invoke($"Failure to load plane: {_plane_type}");
            return;
        }

        bool success;
        m_Editor_State!.Move_To__Plane(plane_type, out success);
        if (!success)
            error_callback($"Plane: {plane_type} is not available on this map.");
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        this.Size = e.Size;
        m_Editor_Gui.Resize(e.Size);
        m_Base_Viewport.Resize(e.Size);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        m_Editor_Gui.Render();

        m_Editor_State?
            .Operation_Controller
            .Handle__On_Render(m_Editor_State);

        SwapBuffers();
    }

    protected void Handle__Render_Map(object? sender, EventArgs e)
    {
        if (!Is__Performing_IO && Is__Present__Active_Plane && m_COE5_Plane_Renderer != null)
        {
            COE5_Plane plane = m_Editor_State!.Plane__Active;

            m_Projection =
                Matrix4.CreateOrthographicOffCenter
                (
                    -plane.Width/2,
                     plane.Width/2,
                     plane.Height/2,
                    -plane.Height/2,
                    1, -1
                );
            m_View =
                Matrix4.Identity;
            m_COE5_Plane_Renderer
            .Render_Plane(plane, ref m_Projection, ref m_View);
        }
        Context.MakeCurrent();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        //base.OnUpdateFrame(args);
    }
}
