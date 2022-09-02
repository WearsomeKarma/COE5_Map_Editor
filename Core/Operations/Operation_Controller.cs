
using COE5_Map_Editor.Core.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace COE5_Map_Editor.Core.Operations;

/// <summary>
/// Encapsulates an Operation_Switch
/// It then checks on every update frame
/// for input and routes it to operations.
/// For every render frame, it processes
/// enqueued render operations.
/// </summary>
public class Operation_Controller
{
    internal readonly Operation_Switch m_Operation_Switch;

    private struct Operation_Message
    {
        public Operation operation;
        public Input_Binding event_signature;
    }
    private readonly Queue<Operation_Message> m_GPU_Operation_Queue =
        new Queue<Operation_Message>();

    private readonly Dictionary<int, Operation> m_Operations =
        new Dictionary<int, Operation>();

    public Operation_Controller
    (
        Input_Scheme input_scheme,
        IEnumerable<Operation> operations
    )
    {
        m_Operation_Switch = 
            new Operation_Switch(input_scheme, operations);

        foreach(Operation operation in operations)
            m_Operations.Add(operation.Information.Type_Code, operation);
    }

    protected internal virtual void Process
    (
        KeyboardKeyEventArgs? e_key,
        MouseButtonEventArgs? e_mouse,
        KeyboardState keyboard,
        MouseState mouse
    )
    {
        Operation_Info? operation_info;
        Input_Binding event_signature =
            new Input_Binding
            (
                e_key,
                e_mouse,
                false //TODO: handle holding
            );

        Console.WriteLine($"key?: {e_key?.Key}");
        Console.WriteLine($"mouse?: {e_mouse?.Button}");

        Console.WriteLine($"--- signing: \t\t-- {Convert.ToString(event_signature.GetHashCode(),2).PadLeft(17, '0')} ---");
        m_Operation_Switch.Switch(keyboard, mouse, ref event_signature, out operation_info);

        if (operation_info == null) return;

        Operation operation =
            m_Operations[(int)operation_info?.Type_Code!];

        if (operation.Is_GPU_Operation)
        {
            m_GPU_Operation_Queue
                .Enqueue
                (
                    new Operation_Message 
                    { 
                        operation = operation, 
                        event_signature = event_signature 
                    }
                );
            
            return;
        }

        operation.Operate(event_signature);
    }

    protected internal virtual void Handle__On_Update()
    {

    }

    protected internal virtual void Handle__On_Render()
    {
        Operation_Message msg;

        while(m_GPU_Operation_Queue.TryDequeue(out msg))
        {
            Console.WriteLine($"process k_msg: {msg.operation}");
            msg.operation.Operate(msg.event_signature);
        }
    }
}
