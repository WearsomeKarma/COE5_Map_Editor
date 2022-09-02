
using Gwen.Net.Control;

namespace COE5_Map_Editor;

public static class Gwen_Helper
{
    public static void Disable_Recursively(ControlBase control, int depth=int.MaxValue)
    {
        if (depth <= 0) return;
        control.Disable();
        foreach(ControlBase child in control.Children)
            Disable_Recursively(child, depth-1);
    }

    public static void Enable_Recursively(ControlBase control, int depth=int.MaxValue)
    {
        if (depth <= 0) return;
        control.Enable();
        foreach(ControlBase child in control.Children)
            Enable_Recursively(child, depth-1);
    }

    public static void Operate_Recursively
    (
        ControlBase control, 
        Action<ControlBase> callback, 
        int depth=int.MaxValue
    )
    {
        if (depth <= 0) return;
        callback(control);
        foreach(ControlBase child in control.Children)
            Operate_Recursively(child, callback, depth-1);
    }

    public static void Operate_Recursively_On<TTarget>
    (
        ControlBase control, 
        Action<ControlBase> callback, 
        int depth=int.MaxValue
    )
    where TTarget : ControlBase
    {
        if (depth <= 0) return;
        if (control is TTarget)
        {
            callback(control);
        }
        foreach(ControlBase child in control.Children)
            Operate_Recursively_On<TTarget>(child, callback, depth-1);
    }
}
