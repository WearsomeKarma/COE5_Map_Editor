
using COE5_Map_Editor.Core.Processing;

namespace COE5_Map_Editor.Core.Operations;

public class Operation
{
    public readonly Operation_Info Information;

    public int Type_Code
        => Information.Type_Code;
    public string Type_Name
        => Information.Type_Name;

    public bool Is_GPU_Operation { get; protected set; } 
        = true;

    public Operation
    (
        Operation_Info alias
    )
    {
        Information = alias;
    }

    /// <summary>
    /// Returns a binding as either new_binding or
    /// that binding with some modifications.
    /// Otherwise returns null if the binding is 
    /// not allowed.
    ///
    /// Modifications typically result in
    /// changing the Input_Modifier_Type to
    /// include the Held flag.
    /// </summary>
    protected internal virtual Input_Binding? Handle__Permit_Binding(Input_Binding new_binding)
        => new_binding;

    protected internal virtual void Operate
    (
        COE5_Editor_State state,
        Input_Binding e
    )
    {
        Console.WriteLine($"{GetType().FullName} - not Implemented");
    }

    protected internal virtual GPU_Process? Operate__GPU
    (
        COE5_Editor_State state,
        Input_Binding e
    )
    {
        return null;
    }
}
