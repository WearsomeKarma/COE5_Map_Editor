using System.Collections;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace COE5_Map_Editor.Core.Input;

public delegate bool Updated_Control_Handler(Input_Binding old_control, Input_Binding new_control);

public class Input_Scheme : IEnumerable<Input_Binding>
{
    public event Updated_Control_Handler? Control__Updated;

    internal readonly List<Input_Binding> Scheme =
        new List<Input_Binding>();

    internal Input_Scheme(){}
    internal Input_Scheme(IEnumerable<Input_Binding> cloned_list)
    {
        Scheme = new List<Input_Binding>(cloned_list);
    }

    internal Input_Scheme Clone()
        => new Input_Scheme(Scheme);

    public static Input_Scheme_Factory Create()
        => new Input_Scheme_Factory();

    public IEnumerator<Input_Binding> GetEnumerator()
        => Scheme.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public static Input_Scheme Default
    {
        get =>
            new Input_Scheme_Factory()
            .Bind_Key(Keys.D0, Operation_Types.Move_To__Plane)
            .Bind_Key(Keys.D1, Operation_Types.Move_To__Plane)
            .Bind_Key(Keys.D2, Operation_Types.Move_To__Plane)
            .Bind_Key(Keys.D3, Operation_Types.Move_To__Plane)
            .Bind_Key(Keys.D4, Operation_Types.Move_To__Plane)
            .Bind_Key(Keys.D5, Operation_Types.Move_To__Plane)
            .Bind_Key(Keys.D6, Operation_Types.Move_To__Plane)
            .Bind_Key(Keys.D7, Operation_Types.Move_To__Plane)
            .Bind_Key(Keys.D8, Operation_Types.Move_To__Plane)
            .Bind_Key(Keys.D9, Operation_Types.Move_To__Plane)
            .Bind_Mouse(MouseButton.Left, Operation_Types.Tool__Use)
            .Bind_Mouse(MouseButton.Right, Operation_Types.View__Pan)
            .Bind_Key(Keys.R, Operation_Types.View__Reset)
            .Input_Scheme
            ;
    }
}
