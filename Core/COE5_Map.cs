using System.Collections;

namespace COE5_Map_Editor.Core;

public class COE5_Map : IEnumerable<COE5_Plane>
{
    private readonly Dictionary<COE5_Plane_Type, COE5_Plane> _planes =
        new Dictionary<COE5_Plane_Type, COE5_Plane>();

    public bool Check_If__Has_Plane(COE5_Plane_Type plane_type)
        => _planes.ContainsKey(plane_type);

    public COE5_Plane this[COE5_Plane_Type plane_type]
        => _planes[plane_type];

    public int COE5_Version { get; }
    public string Description { get; set; }

    public IEnumerator<COE5_Plane> GetEnumerator()
    {
        return _planes.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private COE5_Map
    (
        List<COE5_Plane> planes,
        int version,
        string description
    )
    {
        foreach(COE5_Plane plane in planes)
            _planes.Add(plane.Plane_Type, plane);
        COE5_Version = version;
        Description = description;
    }

    public static COE5_Map Load_Map
    (
        string map_path
    )
    {
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
