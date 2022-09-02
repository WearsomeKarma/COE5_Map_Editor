
using System.Collections;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace COE5_Map_Editor.Core.Operations;

public class Operation_Switch_Tree : IEnumerable<KeyValuePair<Input_Binding, Operation_Info>>
{
    // This controls the routing to go left and right
    // based on total comparison.
    internal readonly Input_Binding m_Base_Binding;

    internal readonly Dictionary<Input_Binding, Operation_Info> m_Operations =
        new Dictionary<Input_Binding, Operation_Info>();

    internal Operation_Switch_Tree? m_Left;
    internal Operation_Switch_Tree? m_Right;

    internal Operation_Switch_Tree(Input_Binding binding)
    {
        m_Base_Binding = binding;
    }

    public bool Is__Event_Not_Routable
    (
        KeyboardState keyboard, 
        MouseState mouse,
        ref Input_Binding event_signature
    )
    {
        bool case__any =
            (m_Base_Binding.Key_Binding == Keys.Unknown && keyboard.IsAnyKeyDown);
        bool? case__key =
            (case__any)
            ? null
            :
            (
                m_Base_Binding.Key_Binding != null && m_Base_Binding.Key_Binding != Keys.Unknown
                ? !keyboard[(Keys)m_Base_Binding.Key_Binding]
                : false 
            );
        bool case__mouse =
            (
                m_Base_Binding.Mouse_Binding != null 
                ? !mouse[(MouseButton)m_Base_Binding.Mouse_Binding]
                : false 
            );
        bool case__held =
            (m_Base_Binding.Requires__Held && event_signature.Requires__Held)
            ;
        bool case__alt =
            (m_Base_Binding.Requires__Alt && (keyboard[Keys.LeftAlt] || keyboard[Keys.RightAlt]))
            ;
        bool case__ctrl =
            (m_Base_Binding.Requires__Ctrl && (keyboard[Keys.LeftControl] || keyboard[Keys.RightControl]))
            ;
        bool case_shift =
            (m_Base_Binding.Requires__Shift && (keyboard[Keys.LeftShift] || keyboard[Keys.RightShift]))
            ;

        Console.WriteLine
($"any-{case__any}\tkey-{case__key?.ToString() ?? "skip"}\nmouse-{case__mouse}\theld-{case__held}\nalt-{case__alt}\tctrl-{case__ctrl}\nshift-{case_shift}");

        return 
            !
            (
                (m_Base_Binding.Key_Binding == Keys.Unknown && keyboard.IsAnyKeyDown)
                ||
                (
                    m_Base_Binding.Key_Binding != null && m_Base_Binding.Key_Binding != Keys.Unknown
                    ? keyboard[(Keys)m_Base_Binding.Key_Binding]
                    : false
                )
                ||
                (
                    m_Base_Binding.Mouse_Binding != null 
                    ? mouse[(MouseButton)m_Base_Binding.Mouse_Binding]
                    : false 
                )
                ||
                (m_Base_Binding.Requires__Held && event_signature.Requires__Held)
                ||
                (m_Base_Binding.Requires__Alt && (keyboard[Keys.LeftAlt] || keyboard[Keys.RightAlt]))
                ||
                (m_Base_Binding.Requires__Ctrl && (keyboard[Keys.LeftControl] || keyboard[Keys.RightControl]))
                ||
                (m_Base_Binding.Requires__Shift && (keyboard[Keys.LeftShift] || keyboard[Keys.RightShift]))
            )
            ;
    }

    public bool Route__Binding_Right_Or_Left(ref Input_Binding binding)
        => (binding & m_Base_Binding) >= m_Base_Binding.GetHashCode();

    internal void Route
    (
        KeyboardState keyboard, 
        MouseState mouse,
        ref Input_Binding event_signature,
        out Operation_Info? binded_operation
    )
    {
        Console.WriteLine($"routing {event_signature.Key_Binding?.ToString() ?? event_signature.Mouse_Binding!.ToString()}");
        Console.WriteLine($"--- EVENT: \t\t-- {Convert.ToString(event_signature.GetHashCode(),2).PadLeft(17, '0')} ---");

        if (Is__Event_Not_Routable(keyboard, mouse, ref event_signature))
        {
            if (m_Right != null)
            {
                m_Right.Route(keyboard, mouse, ref event_signature, out binded_operation);
                if (binded_operation != null)
                    return;
            }

            foreach(Input_Binding binding in m_Operations.Keys)
                Console.WriteLine($"{m_Operations[binding].Type_Name} \t\t-- {Convert.ToString(binding.GetHashCode(), 2).PadLeft(17, '0')}");

            binded_operation = m_Operations[event_signature];
        }

        if (m_Left != null)
        {
            m_Left.Route(keyboard, mouse, ref event_signature, out binded_operation);
            if (binded_operation != null)
                return;
        }

        foreach(Input_Binding binding in m_Operations.Keys)
            Console.WriteLine($"{m_Operations[binding].Type_Name} \t\t-- {Convert.ToString(binding.GetHashCode(), 2).PadLeft(17, '0')}");
        binded_operation = m_Operations[event_signature];
    }

    public Operation_Switch_Tree Set_Left
    (
        Input_Binding binding
    )
    {
        return
        (Operation_Switch_Tree)
        (
            m_Left = new Operation_Switch_Tree(binding)
        );
    }

    public Operation_Switch_Tree Set_Right
    (
        Input_Binding binding
    )
    {
        return
        (Operation_Switch_Tree)
        (
            m_Right = new Operation_Switch_Tree(binding)
        );
    }

    internal bool Add_Binding(Input_Binding binding, Operation_Info operation_info)
    {
        Console.WriteLine($"adding {binding.Key_Binding?.ToString() ?? binding.Mouse_Binding!.ToString()}");

        if (Route__Binding_Right_Or_Left(ref binding))
        {
            bool right_set =
                m_Right?.Add_Binding(binding, operation_info) ?? false;

            if (right_set)
            {
                Console.WriteLine("right");
                return true;
            }

            Console.WriteLine("here");
            m_Operations.Add(binding, operation_info);

            return true;
        }

        bool left_set =
            m_Left?.Add_Binding(binding, operation_info) ?? false;

        if (left_set) 
        {
            Console.WriteLine("left");
            return true;
        }

        Console.WriteLine("fail");
        return false;
    }

    internal bool Remove_Control(Input_Binding control)
    {
        if (Route__Binding_Right_Or_Left(ref control))
        {
            bool right_set =
                m_Right?.Remove_Control(control) ?? false;

            if (right_set)
            {
                return true;
            }

            m_Operations.Remove(control);

            return true;
        }

        bool left_set =
            m_Left?.Remove_Control(control) ?? false;

        if (left_set)
        {
            return true;
        }

        return false;
    }

    public IEnumerator<KeyValuePair<Input_Binding, Operation_Info>> GetEnumerator()
    {
        // In order descent. Left first.

        if (m_Left != null)
            foreach(KeyValuePair<Input_Binding, Operation_Info> record in m_Left)
                yield return record;

        // here.

        foreach(KeyValuePair<Input_Binding, Operation_Info> record in m_Operations)
            yield return record;

        // Right

        if (m_Right != null)
            foreach(KeyValuePair<Input_Binding, Operation_Info> record in m_Right)
                yield return record;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
