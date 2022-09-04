
using Gwen.Net;
using Gwen.Net.Control;

namespace COE5_Map_Editor;

public class Loading_Screen : ControlBase
{
    public Loading_Screen(ControlBase root)
    : base(root)
    {
        Dock = Dock.Fill;

        Label welcome_title = new Label(this)
        {
            Text = "COE5 Map Editor",
            Font = new Font(Skin.Renderer, "Arial", 20)
        };

        welcome_title.Position = 
            new Point(Width/2 - welcome_title.Width/2, Height/2 - welcome_title.Height/2);

        Label progress = new Label(this) { Text = "Loading..." };

        progress.Dock = Dock.Bottom;
    }
}
