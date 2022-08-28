using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ConquestOfElysium5_MapEditor.Core
{
    /// <summary>
    /// I can't find the original assets for COE5.
    /// I want to say its the .coem files, but I am
    /// unsure as to how to extract its data.
    /// 
    /// I can look into it later but for now its time
    /// to just make some tile colors.
    /// </summary>
    public static class COE5_Tile_Colors
    {
        public static Vector4[] Ground_Colors =
            new Vector4[]
            {
                Red,
                Green,
                Blue
            };

        public static Vector4[] Logic_Colors =
            new Vector4[]
            {
                Red,
                Green,
                Blue
            };

        public static readonly Vector4 Red =
            new Vector4(1, 0, 0, 1);
        public static readonly Vector4 Green =
            new Vector4(0, 1, 0, 1);
        public static readonly Vector4 Blue =
            new Vector4(0, 0, 1, 1);
    }
}
