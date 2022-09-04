
namespace COE5_Map_Editor.Core.Operations;

public class Operation__View__Pan : Operation_Core
{
    public Operation__View__Pan()
    : base(Operation_Types.View__Pan)
    { 
        Is_GPU_Operation = false;
    }

    /// <summary>
    /// Override binding to require being
    /// held.
    /// </summary>
    protected internal override Input_Binding? Handle__Permit_Binding
    (
        Input_Binding new_binding
    )
    {
        Input_Binding binding =
            new Input_Binding
            (
                new_binding,
                modifier_type: 
                    new_binding.Modifier_Type 
                    | 
                    Input_Modifier_Types.Held
            );

        return binding;
    }

    DateTime m_Last_Time;
    int m_Last__Mouse_X, m_Last__Mouse_Y;
    protected internal override void Operate
    (
        COE5_Editor_State state, 
        Input_Binding e
    )
    {
        if 
        (
            state.Mouse__X == m_Last__Mouse_X
            &&
            state.Mouse__Y == m_Last__Mouse_Y
        )
            return;

        TimeSpan delta_time =
            DateTime.Now.Subtract(m_Last_Time);

        float normalized_x =
            (state.Mouse__X / (float)state.Viewport__Width);
        float normalized_y =
            (state.Mouse__Y / (float)state.Viewport__Height);

        state.Set__Camera
        (
        );
    }
}
