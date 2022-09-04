namespace COE5_Map_Editor.Core;

public class COE5_Textures : IDisposable
{
    private readonly Dictionary<COE5_Ground_Type, Texture> _ground_textures =
        new Dictionary<COE5_Ground_Type, Texture>();
    internal Texture Get_Ground_Texture(COE5_Tile tile)
        => _ground_textures[tile.Ground];
    private readonly Dictionary<COE5_Logic_Type, Texture> _logic_textures =
        new Dictionary<COE5_Logic_Type, Texture>();

    internal COE5_Textures(COE5_Data_Directory coe5_directory)
    {
        Load_Texture_Dictionary(_ground_textures, coe5_directory.Ground_Textures);
    }

    internal void Unload()
    {
        Unload_Dictionary(_ground_textures);
        Unload_Dictionary(_logic_textures);
    }

    public void Dispose()
    {
        Unload();
    }

    private static void Unload_Dictionary<TEnum>
    (
        Dictionary<TEnum, Texture> dict
    )
    where TEnum : Enum
    {
        foreach (KeyValuePair<TEnum, Texture> pair in dict.ToArray())
        {
            dict.Remove(pair.Key);
            pair.Value.Dispose();
        }
    }

    private void Load_Texture_Dictionary<TEnum>
    (
        Dictionary<TEnum, Texture> dictionary,
        Dictionary<string, TEnum> texture_files
    )
    where TEnum : Enum
    {
        int texture_count = texture_files.Count;
        //TODO: throw error.
        if (texture_count <= 0) return;
        foreach(KeyValuePair<string, TEnum> pair in texture_files)
        {
            TEnum @enum = pair.Value;

            Texture loaded_texture =
                Texture.Load(pair.Key);

            dictionary.Add(@enum, loaded_texture);
        }
    }
}
