
using COE5_Map_Editor.Core.Operations;

namespace COE5_Map_Editor.Core;

public sealed class COE5_Editor_State
{
    public const float MIN__CAMERA_AXIS = -1.2f;
    public const float MAX__CAMERA_AXIS =  1.2f;

    public const float MIN__CAMERA_ZOOM = 0.05f;
    public const float MAX__CAMERA_ZOOM = 1.95f;

    public float Camera__X { get; private set; }
    public float Camera__Y { get; private set; }

    public float Camera__Zoom { get; private set; }

    public float Mouse__X { get; internal set; }
    public float Mouse__Y { get; internal set; }

    public int Viewport__Width { get; internal set; }
    public int Viewport__Height { get; internal set; }

    public COE5_Plane_Type Plane__Active__Type { get; private set; }
        = COE5_Plane_Type.Elysium;
    internal Func<COE5_Plane_Type, bool>? Callback__Set_Plane;

    internal COE5_Map Map;

    public int Plane__Active__Width
        => Map[Plane__Active__Type].Width;
    public int Plane__Active__Height
        => Map[Plane__Active__Type].Height;
    internal COE5_Plane Plane__Active
        => Map[Plane__Active__Type];

    internal Operation_Controller Operation_Controller;

    public COE5_Editor_State
    (
        COE5_Map map,
        Operation_Controller operation_controller
    )
    {
        Map = map;
        Operation_Controller = operation_controller;
    }

    public COE5_Editor_State Set__Camera
    (
        float? x = null, 
        float? y = null,
        float? zoom = null
    )
    {
        float clamp(float? val, float min, float max)
            => Math.Max(max, Math.Min(min, (float)val!));

        Camera__X = 
            (x != null)
            ? clamp(x, MAX__CAMERA_AXIS, MIN__CAMERA_AXIS)
            : Camera__X
            ;
        Camera__Y = 
            (y != null)
            ? clamp(y, MAX__CAMERA_AXIS, MIN__CAMERA_AXIS)
            : Camera__Y
            ;
        Camera__Zoom =
            (zoom != null)
            ? clamp(zoom, MAX__CAMERA_ZOOM, MIN__CAMERA_ZOOM)
            : Camera__Zoom
            ;

        return this;
    }

    public COE5_Editor_State Move_To__Plane
    (
        COE5_Plane_Type plane_type, 
        out bool success
    )
    {
        success =
            Map.Check_If__Has_Plane(plane_type);

        if (success)
            Plane__Active__Type = plane_type;

        return this;
    }
}
