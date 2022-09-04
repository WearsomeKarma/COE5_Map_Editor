
namespace COE5_Map_Editor.Core;

/// <summary>
/// These are operations limited to controlling
/// or operating on the map view.
/// </summary>
public enum Operation_Types
{
    SUB_OPERATION = -1, // this is for sub_operations.
    NOTHING = 0,
    Tool__Use = 1,
    Move_To__Plane = 2,
    View__Pan = 3,
    View__Reset = 4
}
