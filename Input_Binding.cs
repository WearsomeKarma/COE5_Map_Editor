using COE5_Map_Editor.Core;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace COE5_Map_Editor;

public struct Input_Binding
{
    public Keys? Key_Binding { get; private set; }
    public MouseButton? Mouse_Binding { get; private set; }

    public int Operation_Code { get; set; }
    public Operation_Core__Types Operation__As_Core_Op
        => (Operation_Core__Types)Operation_Code;

    public bool Is__Keyboard_Or_Mouse__Controlled
        => Mouse_Binding == null;
    public bool Is__Enabled
        => Mouse_Binding != null || Key_Binding != null;

    public Input_Modifier_Types Modifier_Type { get; private set; }

    public bool Requires__Held
        => (Modifier_Type & Input_Modifier_Types.Held) == Input_Modifier_Types.Held;
    public bool Requires__Shift
        => (Modifier_Type & Input_Modifier_Types.Shift) == Input_Modifier_Types.Shift;
    public bool Requires__Ctrl
        => (Modifier_Type & Input_Modifier_Types.Ctrl) == Input_Modifier_Types.Ctrl;
    public bool Requires__Alt
        => (Modifier_Type & Input_Modifier_Types.Alt) == Input_Modifier_Types.Alt;

    internal Input_Binding
    (
        Keys? key_control = null, 
        MouseButton? mouse_control = null, 
        int operation_type = 0, 
        Input_Modifier_Types modifier_type = Input_Modifier_Types.NONE
    )
    {
        Key_Binding = key_control ?? null;
        Mouse_Binding = mouse_control ?? null;
        if (Key_Binding == null && Mouse_Binding == null)
            Key_Binding = Keys.Unknown;
        Operation_Code = operation_type;
        Modifier_Type = modifier_type;
    }

    internal Input_Binding
    (
        Keys key_control, 
        int operation_type, 
        Input_Modifier_Types modifier_type
    )
    {
        Key_Binding = key_control;
        Mouse_Binding = null;
        Operation_Code = operation_type;
        Modifier_Type = modifier_type;
    }

    internal Input_Binding
    (
        MouseButton mouse_control, 
        int operation_type, 
        Input_Modifier_Types modifier_type
    )
    {
        Key_Binding = null;
        Mouse_Binding = mouse_control;
        Operation_Code = operation_type;
        Modifier_Type = modifier_type;
    }

    internal Input_Binding
    (
        KeyboardKeyEventArgs? e_key,
        MouseButtonEventArgs? e_mouse,
        bool is_held
    )
    {
        // complete constructing an input_binding from params
        Modifier_Type = Input_Modifier_Types.NONE;
        Operation_Code = 0;
        if (e_key != null)
        {
            if (e_key?.Alt ?? false)
                Modifier_Type = Input_Modifier_Types.Alt;
            if (e_key?.Control ?? false)
                Modifier_Type += (int)Input_Modifier_Types.Ctrl;
            if (e_key?.Shift ?? false)
                Modifier_Type += (int)Input_Modifier_Types.Shift;
            if (is_held)
                Modifier_Type = Modifier_Type & Input_Modifier_Types.Held;

            Key_Binding = (Keys)e_key?.Key!;
            Mouse_Binding = null;
            return;
        }

        Key_Binding = null;
        Mouse_Binding = (MouseButton)e_mouse?.Button!;
    }

    public override int GetHashCode()
    {
        int ret = 0;
        if (Key_Binding != null)
            ret = ((int)Key_Binding) << 8;
        if (Mouse_Binding != null)
            ret += ((int)Mouse_Binding + 1) << 4;
        ret += (int)Modifier_Type;

        return ret;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Input_Binding)
            return Schemes_Match(this, (Input_Binding)obj);
        return base.Equals(obj);
    }

    public bool Matches_To(Input_Binding c)
        => Schemes_Match(this, c);

    public static bool Schemes_Match(Input_Binding c1, Input_Binding c2)
        =>
        c1.Modifier_Type == c2.Modifier_Type
        &&
        (
            c1.Key_Binding == c2.Key_Binding
            &&
            c1.Mouse_Binding == c2.Mouse_Binding
        )
        ;

    public static int operator &(Input_Binding b1, Input_Binding b2)
        => b1.GetHashCode() & b2.GetHashCode();
}
