
using OpenTK.Graphics.OpenGL4;

namespace COE5_Map_Editor.Core.Processing;

/// <summary>
/// Represents 3 base FBOs which output sampler2Ds
/// to be operated on, and the outputs of which
/// is stored in 3 final FBOs (also here.)
/// </summary>
public class Plane_Snapshot
{
    public int Sampler2D_Map__Base__Terrain_Texture;
    // 32 bits, 16 ground, 16 portal? or 24 ground, 8 portal...
    public int Sampler2D_Map__Base__Logical_Texture;
    public int Sampler2D_Map__Base__Temperature_Texture;

    public int Sampler2D_Map__Final__Terrain_Texture;
    public int Sampler2D_Map__Final__Logical_Texture;
    public int Sampler2D_Map__Final__Temperature_Texture;

    public readonly List<GPU_Process> Process_Queue =
        new List<GPU_Process>();

    public Plane_Snapshot
    (
        Plane_Snapshot previous,
        int width, int height
    )
    {
        Sampler2D_Map__Base__Terrain_Texture =
            previous.Sampler2D_Map__Final__Terrain_Texture;
        Sampler2D_Map__Base__Logical_Texture =
            previous.Sampler2D_Map__Final__Logical_Texture;
        Sampler2D_Map__Base__Temperature_Texture =
            previous.Sampler2D_Map__Final__Temperature_Texture;

        Sampler2D_Map__Final__Terrain_Texture =
            GL.GenTexture();
        Sampler2D_Map__Final__Logical_Texture =
            GL.GenTexture();
        Sampler2D_Map__Final__Temperature_Texture =
            GL.GenTexture();

        Initalize__Textures
        (
            Sampler2D_Map__Final__Terrain_Texture,
            Sampler2D_Map__Final__Logical_Texture,
            Sampler2D_Map__Final__Temperature_Texture,
            width, height
        );
    }

    public Plane_Snapshot(int width, int height)
    {
        Sampler2D_Map__Base__Terrain_Texture =
            GL.GenTexture();
        Sampler2D_Map__Base__Logical_Texture =
            GL.GenTexture();
        Sampler2D_Map__Base__Temperature_Texture =
            GL.GenTexture();

        Sampler2D_Map__Final__Terrain_Texture =
            GL.GenTexture();
        Sampler2D_Map__Final__Logical_Texture =
            GL.GenTexture();
        Sampler2D_Map__Final__Temperature_Texture =
            GL.GenTexture();

        Initalize__Textures
        (
            Sampler2D_Map__Base__Terrain_Texture,
            Sampler2D_Map__Base__Logical_Texture,
            Sampler2D_Map__Base__Temperature_Texture,
            width, height
        );

        Initalize__Textures
        (
            Sampler2D_Map__Final__Terrain_Texture,
            Sampler2D_Map__Final__Logical_Texture,
            Sampler2D_Map__Final__Temperature_Texture,
            width, height
        );
    }

    private void Initalize__Textures
    (
        int terrain, int logical, int temperature,
        int width, int height
    )
    {
        void init(ref int texture)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D
            (
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Alpha,
                width, height,
                0,
                PixelFormat.Alpha,
                PixelType.Int,
                IntPtr.Zero
            );
        }

        init(ref terrain);
        init(ref logical);
        init(ref temperature);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    internal void Bind()
    {
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Sampler2D_Map__Base__Terrain_Texture);
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, Sampler2D_Map__Base__Logical_Texture);
        GL.ActiveTexture(TextureUnit.Texture2);
        GL.BindTexture(TextureTarget.Texture2D, Sampler2D_Map__Base__Temperature_Texture);
    }

    internal void Unbind()
    {
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.ActiveTexture(TextureUnit.Texture2);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        GL.ActiveTexture(TextureUnit.Texture0);
    }
}
