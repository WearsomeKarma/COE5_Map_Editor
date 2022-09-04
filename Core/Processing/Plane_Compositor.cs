
using COE5_Map_Editor.Core.Operations;
using OpenTK.Graphics.OpenGL4;

namespace COE5_Map_Editor.Core.Processing;

/// <summary>
/// Handles plane compositing. Done per plane.
/// </summary>
public class Plane_Compositor
{
    private const int MAX_OPERATION__PER_SNAPSHOT = 64;

    private readonly List<Plane_Snapshot> m_Map_Snapshots =
        new List<Plane_Snapshot>();

    private Plane_Snapshot Latest
        => m_Map_Snapshots[m_Map_Snapshots.Count-1];

    private readonly int Width, Height;

    private readonly float[] m_Verticies =
        {
            // formating and yadda yadda copied from LearnOpenTK github
            // Position         
            1,  0, 0,  // top right
            1, -1, 0,  // bottom right
            0, -1, 0,  // bottom left
            0,  0, 0   // top left
        };

    private readonly int[] m_Indicies =
        new int[] 
        {
            // formating and yadda yadda copied from LearnOpenTK github
            0, 1, 3, // The first triangle will be the top-right half of the triangle
            1, 2, 3  // Then the second will be the bottom-left half of the triangle
        };

    private int 
        m_VBO,
        m_EBO,
        m_VAO
        ;

    public Plane_Compositor(int width, int height)
    {
        Width = width;
        Height = height;
        Create__Next_Snapshot();

        m_VBO = 
            GL.GenBuffer();
        m_EBO =
            GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, m_VBO);
        GL.BufferData
        (
            BufferTarget.ArrayBuffer, 
            sizeof(float) * m_Verticies.Length, 
            m_Verticies,
            BufferUsageHint.StaticDraw
        );

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_EBO);
        GL.BufferData
        (
            BufferTarget.ElementArrayBuffer,
            sizeof(int) * m_Indicies.Length,
            m_Indicies,
             BufferUsageHint.StaticDraw
        );

        m_VAO =
            GL.GenVertexArray();
        GL.BindVertexArray(m_VAO);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }

    internal void Bind()
    {
        Plane_Snapshot snapshot = Latest;

        snapshot.Bind();
    }

    internal void Unbind()
    {
        Plane_Snapshot snapshot = Latest;

        snapshot.Unbind();
    }

    internal void Composite(Operation_Controller operation_controller)
    {
        Plane_Snapshot snapshot = Latest;

        snapshot.Bind();

        foreach(GPU_Process process in snapshot.Process_Queue)
        {
            GL.BindVertexArray(m_VAO);

            process.Use();

            GL.DrawElements
            (
                PrimitiveType.Triangles, 
                m_Indicies.Length, 
                DrawElementsType.UnsignedInt, 
                0
            );
        }

        snapshot.Unbind();

        // NOTE: from here we are done
        //       the rest is handled by
        //       the map render.
    }

    internal void Enqueue__Process(GPU_Process process)
    {
        //push op onto latest Snap_Shot.

        Plane_Snapshot snapshot = Latest;

        if (snapshot.Process_Queue.Count >= MAX_OPERATION__PER_SNAPSHOT)
        {
            Create__Next_Snapshot();
            snapshot = Latest;
        }

        snapshot.Process_Queue.Add(process);
    }

    private void Create__Next_Snapshot()
    {
        if (m_Map_Snapshots.Count == 0)
        {
            m_Map_Snapshots.Add(new Plane_Snapshot(Width, Height));
            return;
        }

        m_Map_Snapshots.Add(new Plane_Snapshot(Latest, Width, Height));
    }
}
