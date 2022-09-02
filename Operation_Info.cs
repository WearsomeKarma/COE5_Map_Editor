
namespace COE5_Map_Editor;

public struct Operation_Info
{
    public readonly int Type_Code;
    public readonly string Type_Name;
    public readonly string Description;
    public readonly bool Allows_Multiple_Bindings;

    public Operation_Info
    (
        int type_code, 
        string type_name,
        string description,
        bool allows_mutliple_bindings
    )
    {
        Type_Code = type_code;
        Type_Name = type_name;
        Allows_Multiple_Bindings = 
            allows_mutliple_bindings;
        Description =
            description;
    }

    public override string ToString()
        => Type_Name;

    public bool Matching(ref Operation_Info op)
        => Matching(ref this, ref op);

    public static bool Matching(ref Operation_Info op1, ref Operation_Info op2)
        =>
        op1.Type_Name == op2.Type_Name
        &&
        op1.Type_Code == op2.Type_Code
        ;
}
