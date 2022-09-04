
namespace COE5_Map_Editor.Core.Processing;

/// <summary>
/// Responsible for rendering a binded
/// Plane_Snapshot within the present
/// GL.Viewport in a manner which can be
/// shown to the end user.
/// </summary>
public class Plane_Render
{
    private const string SOURCE__VERTEX =
@"
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec2 TexCoord;

void main()
{
    gl_Position = projection * view * model * aPosition;
    TexCoord = aTexCoord;
}
";
    private const string SOURCE__FRAGMENT =
@"
out vec4 FragColor;
in vec2 TexCoord;

uniform sampler2D texture_atlas;

uniform sampler2D map__terrain;
uniform sampler2D map__logical;

uniform float tile_size;

uniform int map__width;
uniform int map__height;

void main()
{
    float tex_index   = TexCoord/tile_size;
    float terrain_val = texture(map__terrain, tex_index);
    float logical_val = texture(map__logical, tex_index);

    // convert TexCoord to position on texture_atlas -> AtlasCoord_Terrain
    //      for this, we would use terrain_val to offset via X
    //      we can then even do the fancy texture-wrapping
    // again for AtlasCoord_Logical

    // do a blend func on the two -> FragColor
}
";
}
