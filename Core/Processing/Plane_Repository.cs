
using COE5_Map_Editor.Core.Operations;

namespace COE5_Map_Editor.Core.Processing;

public class Plane_Repository
{
    private COE5_Plane_Type m_Focused_Plane_Key;
    private readonly
        Dictionary
        <
            COE5_Plane_Type, 
            Plane_Compositor
        > m_Plane_Compositors =
        new Dictionary<COE5_Plane_Type, Plane_Compositor>();

    private Plane_Compositor Active_Compositor
        => m_Plane_Compositors[m_Focused_Plane_Key];

    internal void Focus(COE5_Plane_Type plane)
    {
        m_Focused_Plane_Key = plane;
    }

    internal void Enqueue__GPU_Process(GPU_Process process)
    {
        Active_Compositor
            .Enqueue__Process(process);
    }

    internal void Composite(Operation_Controller operation_controller)
    {
        Active_Compositor
            .Composite(operation_controller);
    }

    internal void Render(/* plane shader render */)
    {
        Active_Compositor
            .Bind();

        // use shader
        // draw arrays

        Active_Compositor
            .Unbind();
    }
}
