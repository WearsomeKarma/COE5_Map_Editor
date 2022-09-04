using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace COE5_Map_Editor.Core;

public class COE5_Plane_Render
{
    public const string Shader_Source__VERTEX = @"
#version 330

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;

uniform mat4 model;
uniform mat4 projection;
uniform mat4 view;

out vec2 TexCoord;

void main()
{
    //gl_Position = vec4(aPosition, 1.0); //projection * 
    //gl_Position = projection * view * model * vec4(aPosition, 1.0);
    gl_Position = projection * model * vec4(aPosition, 1.0);
    TexCoord = aTexCoord;
}
";
    public const string Shader_Source__FRAGMENT = @"
#version 330

out vec4 FragColor;

in vec2 TexCoord;

uniform sampler2D TileTexture;

void main()
{
    FragColor = texture(TileTexture, TexCoord);
}
";
    private int _shader_handle;

    private readonly float[] _tile_vert_data =
        {
            // formating and yadda yadda copied from LearnOpenTK github
            // Position         Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };

    public readonly uint[] _tile_index_data =
        {
            // formating and yadda yadda copied from LearnOpenTK github
            0, 1, 3, // The first triangle will be the top-right half of the triangle
            1, 2, 3  // Then the second will be the bottom-left half of the triangle
        };

    private int _tile_vbo;
    private int _tile_ebo;
    private int _tile_vao;

    private int _uniform_model;
    private int _uniform_projection;
    private int _uniform_view;

    private readonly COE5_Textures COE5_Textures;

    internal COE5_Plane_Render
    (
        COE5_Textures coe5_textures    
    )
    {
        COE5_Textures = coe5_textures;

        int vert_handle =
            GL.CreateShader(ShaderType.VertexShader);
        int frag_handle =
            GL.CreateShader(ShaderType.FragmentShader);

        GL.ShaderSource(vert_handle, Shader_Source__VERTEX);
        GL.ShaderSource(frag_handle, Shader_Source__FRAGMENT);

        GL.CompileShader(vert_handle);
        GL.CompileShader(frag_handle);

        _shader_handle =
            GL.CreateProgram();
        GL.AttachShader(_shader_handle, vert_handle);
        GL.AttachShader(_shader_handle, frag_handle);
        GL.LinkProgram(_shader_handle);

        _uniform_model =
            GL.GetUniformLocation(_shader_handle, "model");
        _uniform_projection =
            GL.GetUniformLocation(_shader_handle, "projection");
        _uniform_view =
            GL.GetUniformLocation(_shader_handle, "view");

        GL.DeleteShader(vert_handle);
        GL.DeleteShader(frag_handle);
        
        _tile_vbo =
            GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _tile_vbo);

        GL.BufferData
        (
             BufferTarget.ArrayBuffer,
             _tile_vert_data.Length * sizeof(float),
             _tile_vert_data,
             BufferUsageHint.StaticDraw
        );

        _tile_vao =
            GL.GenVertexArray();
        GL.BindVertexArray(_tile_vao);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        _tile_ebo =
            GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _tile_ebo);
        GL.BufferData
        (
            BufferTarget.ElementArrayBuffer, 
            _tile_index_data.Length * sizeof(uint), 
            _tile_index_data, 
            BufferUsageHint.StaticDraw
        );
    }

    internal void Render_Plane
    (
        COE5_Plane plane,
        ref Matrix4 projection,
        ref Matrix4 view
    )
    {
        //TODO: this should render now!!!
        //Just one issue where PERFORMANCE IS BAD.
        //move Plane data to a texture on GPU.
        //TODO: prior to this execution, constant Enum_Error in OpenGL

        for (int x = 0; x < plane.Width; x++)
        {
            for (int y = 0; y < plane.Height; y++)
            {
                Texture ground = 
                    COE5_Textures
                    .Get_Ground_Texture(plane[x, y]);

                GL.UseProgram(_shader_handle);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, ground.Handle);

                Matrix4 mat = Matrix4.CreateTranslation(new Vector3(x - (plane.Width/2), y - (plane.Height/2), 0));
                GL.UniformMatrix4(_uniform_model, false, ref mat);
                GL.UniformMatrix4(_uniform_projection, false, ref projection);
                GL.UniformMatrix4(_uniform_view, false, ref view);

                GL.BindVertexArray(_tile_vao);
                GL.DrawElements(PrimitiveType.Triangles, _tile_index_data.Length, DrawElementsType.UnsignedInt, 0);
                GL.UseProgram(0);
            }
        }
    }
}
