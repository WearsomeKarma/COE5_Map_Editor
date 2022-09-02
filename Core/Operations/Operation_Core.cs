
namespace COE5_Map_Editor.Core.Operations;

public class Operation_Core : Operation
{
    public Operation_Core(Operation_Core__Types operation_type)
    : base(new Operation_Info((int)operation_type, operation_type.ToString(), "", false))
    { }

    public override bool Handle__Permit_Binding(Input_Binding new_binding)
        => true;

    public override void Operate(Input_Binding event_signature)
    {
        Console.WriteLine($"{GetType().FullName} - not Implemented");
    }
}
