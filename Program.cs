using OpenTK.Windowing.Desktop;

namespace COE5_Map_Editor;

public class Program
{
    public static void Main(string[] args)
    {
        new Window(GameWindowSettings.Default, NativeWindowSettings.Default).Run();
    }
}
