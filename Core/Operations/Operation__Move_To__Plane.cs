
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace COE5_Map_Editor.Core.Operations;

public class Operation__Move_To__Plane : Operation_Core
{
    public Operation__Move_To__Plane()
    : base(Operation_Types.Move_To__Plane)
    { 
        Is_GPU_Operation = false;
    }

    protected internal override void Operate
    (
        COE5_Editor_State state, 
        Input_Binding e
    )
    {
        int number = 
            (int)((Keys)e.Key_Binding!)
            -
            (int)Keys.D0
            ;

        state.Move_To__Plane((COE5_Plane_Type)number, out _);
    }
}
