
namespace COE5_Map_Editor.Core.Operations;

public class Operation__View__Reset : Operation_Core
{
    public Operation__View__Reset()
    : base(Operation_Core__Types.View__Reset)
    { 
        Is_GPU_Operation = false;
    }
}
