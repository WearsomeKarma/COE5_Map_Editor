
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace COE5_Map_Editor.Core.Input;

public class Input_Scheme_Factory
{
    public readonly Input_Scheme Input_Scheme;

    internal Input_Scheme_Factory()
    {
        Input_Scheme = new Input_Scheme();
    }

    internal Input_Scheme_Factory(Input_Scheme copy)
    {
        Input_Scheme = copy.Clone();
    }

    public Input_Scheme_Factory Bind_Key
    (
        Keys key_control, 
        Operation_Core__Types operation_type,
        Input_Modifier_Types modifier_type = Input_Modifier_Types.NONE
    )
    {
        Input_Binding binding = new Input_Binding(key_control, (int)operation_type, modifier_type);

        Record_Binding(ref binding);

        return this;
    }

    public Input_Scheme_Factory Bind_Mouse
    (
        MouseButton mouse_control,
        Operation_Core__Types operation_type,
        Input_Modifier_Types modifier_type = Input_Modifier_Types.NONE
    )
    {
        Input_Binding binding = new Input_Binding(mouse_control, (int)operation_type, modifier_type);

        Record_Binding(ref binding);

        return this;
    }

    private void Record_Binding(ref Input_Binding binding)
    {
        for(int i=0;i<Input_Scheme.Scheme.Count;i++)
        {
            if (Input_Scheme.Scheme[i].Matches_To(binding))
            {
                Input_Scheme.Scheme.RemoveAt(i);
                break;
            }
        }

        Input_Scheme.Scheme.Add(binding);
    }
}
