using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquestOfElysium5_MapEditor.Core
{
    /// <summary>
    /// Represents a COE5 Plane of existance.
    /// This could be Elysium, or any other plane.
    /// </summary>
    public class COE5_Plane
    {
        public int Width { get; }
        public int Height { get; }

        public COE5_Tile[,] Tile_Map { get; }

        public COE5_Plane_Type Plane_Type { get;  }

        internal COE5_Plane
        (
            int width,
            int height,
            COE5_Plane_Type plane_type
        )
        {
            Width = width;
            Height = height;

            Tile_Map =
                new COE5_Tile[Width, Height];

            Plane_Type = plane_type;
        }

        public COE5_Tile this[int x, int y]
        {
            get
                => Tile_Map[x, y];
            set
                => Tile_Map[x, y] = value;
        }

        public void Set_Terrain(int x, int y, COE5_Ground_Type terrain)
            => Tile_Map[x, y].Ground = terrain;
    }
}
