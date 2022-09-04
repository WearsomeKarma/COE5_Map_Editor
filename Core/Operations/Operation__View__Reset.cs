
namespace COE5_Map_Editor.Core.Operations;

public class Operation__View__Reset : Operation_Core
{
    public Operation__View__Reset()
    : base(Operation_Types.View__Reset)
    { 
        Is_GPU_Operation = false;
    }

    protected internal override void Operate
    (
        COE5_Editor_State state, 
        Input_Binding e
    )
    {
        state.Set__Camera(x: 0, y: 0, zoom: 1.05f);
    }
}
