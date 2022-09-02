
using Gwen.Net;
using Gwen.Net.Control;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;
using Gwen.Net.Skin;

namespace COE5_Map_Editor;

public class COE5_Map_View : TabControl 
{
    public event EventHandler? Rendering_Map;
    public event EventHandler<string>? Plane_Changed;

    public event EventHandler? Mouse_Focused;
    public event EventHandler? Mouse_Defocused;

    public COE5_Map_View(ControlBase parent)
    : base(parent)
    {
        MouseInputEnabled = true;
    }

    public void Set_Planes(IEnumerable<string> plane_names)
    {
        TabStrip.DeleteAllChildren();

        foreach(ControlBase child in Children.ToList())
        {
            if (child is DockLayout)
            {
                RemoveChild(child, true);
            }
        }

        foreach(string plane_name in plane_names)
        {
            TabButton tab = AddPage(plane_name);
            tab.Text = plane_name;
            tab.Clicked += (s, e) => Plane_Changed?.Invoke(null, tab.Text);
        }
    }

    protected override void OnMouseEntered()
    {
        base.OnMouseEntered();
        Mouse_Focused?.Invoke(null, new EventArgs());
    }

    protected override void OnMouseLeft()
    {
        base.OnMouseLeft();
        Mouse_Defocused?.Invoke(null, new EventArgs());
    }

    protected override void RenderRecursive(SkinBase skin, Rectangle clipRect)
    {
        base.RenderRecursive(skin, clipRect);

        if (Parent == null) return;

        // Interrupt Gwen render
        skin.Renderer.End();

        GLHelper.Push_Viewport
        (
            Parent.ActualPosition.X
            +
            m_InnerPanel.ActualPosition.X
            ,
            Parent.ActualPosition.Y
            +
            m_InnerPanel.ActualPosition.Y
            ,
            m_InnerPanel.ActualSize.Width,
            m_InnerPanel.ActualSize.Height
        );

        // Perform Map Render.
        Rendering_Map?.Invoke(this, new EventArgs());

        GLHelper.Pop_Viewport();

        // Continue Gwen Render.
        skin.Renderer.Begin();
    }
}
