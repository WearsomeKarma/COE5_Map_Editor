
namespace COE5_Map_Editor.Core.Operations;

public class Operation__Move_To__Plane : Operation_Core
{
    public Operation__Move_To__Plane()
    : base(Operation_Core__Types.Move_To__Plane)
    { 
        Is_GPU_Operation = false;
    }
}
