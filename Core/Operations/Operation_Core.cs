
namespace COE5_Map_Editor.Core.Operations;

public class Operation_Core : Operation
{
    public Operation_Core(Operation_Types operation_type)
    : base(new Operation_Info((int)operation_type, operation_type.ToString(), "", false))
    { }
}
