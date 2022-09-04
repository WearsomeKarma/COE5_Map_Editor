
using OpenTK.Graphics.OpenGL4;

namespace COE5_Map_Editor.Core.Processing;

/// <summary>
/// Represents a COE5 Map editor process
/// to occur on the GPU. At the very base implementation
/// this includes a Shader-Program to utilize.
///
/// Implement this to allow for the declaration of
/// uniforms.
/// </summary>
public class GPU_Process
{
    public readonly int Shader_Handle;

    public GPU_Process(int shader_handle)
    {
        Shader_Handle = shader_handle;
    }

    public virtual void Use()
    {
        GL.UseProgram(Shader_Handle);
    }
}
