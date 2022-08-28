using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConquestOfElysium5_MapEditor.Core
{
    public class COE5_Roaming_Directory
    {
        public readonly string Roaming_Directory;
        public string Maps_Directory
            => Path.Combine(Roaming_Directory, "maps");

        internal COE5_Roaming_Directory
        (
            string directory
        )
        {
            Roaming_Directory = directory;
        }

        internal static COE5_Roaming_Directory Get__COE5_Roaming_Directory
        (
            string path = null
        )
        {
            if (path != null)
            {
                Validate_Directory(path);
                return new COE5_Roaming_Directory(path);
            }

            string directory_appdata =
                Environment
                .GetFolderPath
                (
                    Environment
                    .SpecialFolder
                    .ApplicationData
                );

            string coe5_roaming =
                Path.Combine
                (
                    directory_appdata,
                    "coe5"
                );

            Validate_Directory(coe5_roaming);

            return new COE5_Roaming_Directory(coe5_roaming);
        }

        public IEnumerable<string> Get_Maps()
        {
            string maps_dir =
                Path.Combine(Roaming_Directory, "maps");

            return Directory.EnumerateFiles(maps_dir, "*.coem");
        }

        public static void 
        Validate_Directory(string coe5_roaming)
        {
            if (!Directory.Exists(coe5_roaming))
                throw new InvalidOperationException("Invalid Roaming directory. Directory does not exist.");

            if (!Directory.EnumerateFiles(coe5_roaming).ToList().Exists(file => file.Contains("coe5config")))
                throw new InvalidOperationException("Invalid Roaming directory");

            if (!Directory.EnumerateDirectories(coe5_roaming).ToList().Exists(directory => directory.Contains("maps")))
            {
                Directory.CreateDirectory(Path.Combine(coe5_roaming, "maps"));
            }
        }

        public COE5_Map Load_Map
        (
            string map_name
        )
        {
            string map_path =
                Get_Maps().Where(file => file.Contains(map_name)).First();

            StreamReader fs = File.OpenText(map_path);
            Text_Parser parser = new Text_Parser(fs);

            void abort() => Abort(parser);

            int version;
            string description;

            parser
                .Move_To("version", abort)
                .Get_Integer(1, out version, abort)
                .Move_To("mapdescr", abort)
                .Get_Word(1, out description, abort)
                ;

            List<COE5_Plane> planes =
                Parse_Planes(parser);

            return new COE5_Map(planes, version, description);
        }

        private static List<COE5_Plane> Parse_Planes(Text_Parser parser)
        {
            List<COE5_Plane> planes = new List<COE5_Plane>();

            void abort() => Abort(parser);
            bool end_of_file;

            do
            {
                parser.Move_To("plane", out end_of_file);
                if (end_of_file) break;

                int plane_type;
                parser.Get_Integer(1, out plane_type, abort);

                int width, height;
                parser.Move_To("mapsize");
                parser
                    .Get_Integer(1, out width, abort)
                    .Get_Integer(2, out height, abort);

                COE5_Plane plane = new COE5_Plane(width, height, (COE5_Plane_Type)plane_type);

                for (int i = 0; i < height; i++)
                {
                    parser.Move_To("groundrow", abort);
                    string terrainrow;
                    parser.Get_Word(2, out terrainrow, abort);

                    IEnumerable<COE5_Ground_Type> ground_codes =
                        COE5
                        .Cast
                        (
                            terrainrow,
                            ',',
                            index => Abort(parser, $"At terrainrow entry: {index}"),
                            true
                        );

                    List<COE5_Ground_Type> code_list =
                        new List<COE5_Ground_Type>(ground_codes);

                    if (code_list.Count != width)
                        Abort(parser, "terrainrow does not match plane width.");

                    for (int x = 0; x < width; x++)
                        plane.Set_Terrain(x, i, code_list[x]);
                }

                //TODO: add more functionality here!

                planes.Add(plane);
            }
            while (parser.Can_Continue);

            return planes;
        }

        private static void Abort(Text_Parser parser, string message = null)
            => throw new InvalidOperationException($"File corrupted -- Line: {parser.Current_Line_Number} {message}");
    }
}
