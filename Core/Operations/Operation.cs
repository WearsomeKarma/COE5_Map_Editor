
namespace COE5_Map_Editor.Core.Operations;

public abstract class Operation
{
    public readonly Operation_Info Information;

    public int Type_Code
        => Information.Type_Code;
    public string Type_Name
        => Information.Type_Name;

    public bool Is_GPU_Operation { get; protected set; } = true;

    public Operation(Operation_Info alias)
    {
        Information = alias;
    }

    public abstract bool Handle__Permit_Binding(Input_Binding new_binding);

    public abstract void Operate(Input_Binding e);
}
