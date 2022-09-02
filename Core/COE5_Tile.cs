namespace COE5_Map_Editor.Core;

public struct COE5_Tile
{
    /// <summary>
    /// This code corresponds to the numerical code
    /// for the ground of the tile. Set this to
    /// [MAGMA_VALUE] if you want a village on magma.
    /// </summary>
    public COE5_Ground_Type Ground;

    /// <summary>
    /// This code corresponds to the tile's logic.
    /// This determines if the tile is a Village 
    /// Nexus, or Mountains.
    /// </summary>
    public COE5_Logic_Type Structure;

    /// <summary>
    /// This is the temperature value for the tile.
    /// </summary>
    public int Temperature;
}
