
namespace COE5_Map_Editor.Core.Operations;

public class Operation__Tool__Use : Operation_Core
{
    // injected
    private readonly Dictionary<int, Operation> m_Sub_Operations =
        new Dictionary<int, Operation>()
        {
            {0, new Operation__Tool__Stencil()}
        };

    private int m_Selected_Operation_Index;

    public Operation__Tool__Use() 
    : base(Operation_Types.Tool__Use)
    {
    }

    protected internal override void Operate
    (
        COE5_Editor_State state,
        Input_Binding event_signature
    )
    {
        Operation selected_operation =
            m_Sub_Operations[m_Selected_Operation_Index];

        selected_operation.Operate(state, event_signature);
    }
}
