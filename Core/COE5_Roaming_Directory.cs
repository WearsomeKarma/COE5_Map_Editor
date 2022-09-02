namespace COE5_Map_Editor.Core;

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

        string coe5_roaming;

        if (System.OperatingSystem.IsWindows())
        {
            coe5_roaming =
                Path.Combine
                (
                    Environment
                    .GetFolderPath
                    (
                        Environment
                        .SpecialFolder
                        .ApplicationData
                    ),
                    "coe5"
                );
        }
        else if (System.OperatingSystem.IsLinux())
        {
            coe5_roaming =
                Path.Combine
                (
                    Environment
                    .GetFolderPath
                    (
                        Environment
                        .SpecialFolder
                        .Personal
                    ),
                    ".coe5"
                );
        }
        else
        {
            throw new NotImplementedException("Cannot determine Roaming Directory on this OS.");
        }

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

    public static implicit operator string(COE5_Roaming_Directory roaming_directory)
        => roaming_directory.Roaming_Directory;
}
