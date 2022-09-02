using COE5_Map_Editor.Core.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace COE5_Map_Editor.Core.Operations;

public class Operation_Switch
{
    private readonly Operation_Switch_Tree m_Operation_Tree =
        new Operation_Switch_Tree(new Input_Binding(modifier_type: Input_Modifier_Types.Held));

    internal Input_Scheme m_Scheme;

    public Operation_Switch
    (
        Input_Scheme scheme,
        IEnumerable<Operation> operations
    )
    {
        m_Scheme = scheme;

        foreach(Input_Binding binding in scheme)
        {
            //TODO resolve case where operations overlap.
            Operation? operation =
                operations
                .Where(op => op.Type_Code == binding.Operation_Code)
                .FirstOrDefault()
                ;
            Console.WriteLine($"op: {binding.Operation__As_Core_Op} -> {operation?.ToString()}");

            //TODO: give error
            if (operation == null) continue;

            Record(binding, operation.Information);
        }
    }

    public IEnumerable<KeyValuePair<Input_Binding, Operation_Info>> Get__Binding_Records()
    {
        foreach (KeyValuePair<Input_Binding, Operation_Info> record in m_Operation_Tree)
            yield return record;
    }

    public void Record(Input_Binding binding, Operation_Info operation_info)
    {
        m_Operation_Tree.Add_Binding(binding, operation_info);
    }

    public void Switch
    (
        KeyboardState keyboard,
        MouseState mouse,
        ref Input_Binding event_signature,
        out Operation_Info? operation_info
    )
    {
        m_Operation_Tree
            .Route
            (
                keyboard,
                mouse,
                ref event_signature,
                out operation_info
            );
    }
}
