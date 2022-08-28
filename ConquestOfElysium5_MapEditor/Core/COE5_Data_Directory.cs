using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquestOfElysium5_MapEditor.Core
{
    public class COE5_Data_Directory
    {
        public readonly string COE5_Directory;

        internal readonly Dictionary<string, COE5_Ground_Type> Ground_Textures =
            new Dictionary<string, COE5_Ground_Type>();
        internal readonly string[] Structure_Textures;

        internal COE5_Data_Directory
        (
            string coe5_directory
        )
        {
            IEnumerable<string> ground_textures =
                Directory.EnumerateFiles(Path.Combine(coe5_directory, "data"), "map*.tga");

            string config =
                Path.Combine
                (
                    Environment
                    .CurrentDirectory,
                    "coe5_editor_config.txt"
                );

            using (StreamReader stream = File.OpenText(config))
            {
                Text_Parser parser = new Text_Parser(stream);

                while (parser.Next_Line())
                {
                    string file;
                    parser.Get_Word(0, out file);
                    string enum_string;
                    parser.Get_Word(1, out enum_string);

                    COE5_Ground_Type ground_type;
                    if (!Enum.TryParse<COE5_Ground_Type>(enum_string, out ground_type))
                        continue;

                    string file_path =
                        ground_textures.Where(ground => ground.Contains(file)).First();

                    Ground_Textures
                        .Add(file_path, ground_type);
                }
            }

            COE5_Directory = coe5_directory;
            Structure_Textures = new string[0];
        }

        internal static COE5_Data_Directory Get__COE5_Data_Directory
        (
            string path = null
        )
        {
            if (path != null)
            {
                Validate_Installation(path);
                return new COE5_Data_Directory(path);
            }

            string directory_progx86 =
                Environment
                .GetFolderPath
                (
                    Environment
                    .SpecialFolder
                    .ProgramFilesX86
                );

            string coe5_data =
                Path.Combine
                (
                    directory_progx86,
                    "Steam",
                    "steamapps",
                    "common",
                    "ConquestOfElysium5"
                );

            Validate_Installation(coe5_data);
            return new COE5_Data_Directory(coe5_data);
        }

        public static void 
        Validate_Installation
        (
            string directory
        )
        {
            if (!Directory.Exists(directory))
                throw new InvalidOperationException("Not a valid directory");

            bool has_exe =
                File.Exists(Path.Combine(directory, "CoE5.exe"));

            if (!has_exe)
                throw new InvalidOperationException("Exe is missing.");

            IEnumerable<string> sub_directories =
                Directory.EnumerateDirectories(directory);

            string data_directory = null;
            if (!sub_directories.ToList().Exists((path) => (data_directory = path).Contains("data")))
                throw new InvalidOperationException("Data directory is missing.");
        }
    }
}
