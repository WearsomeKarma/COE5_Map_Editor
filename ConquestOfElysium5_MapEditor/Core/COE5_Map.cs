using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquestOfElysium5_MapEditor.Core
{
    public class COE5_Map : IEnumerable<COE5_Plane>
    {
        private readonly List<COE5_Plane> _planes = new List<COE5_Plane>();

        public int COE5_Version { get; }
        public string Description { get; set; }

        public IEnumerator<COE5_Plane> GetEnumerator()
        {
            return _planes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal COE5_Map
        (
            List<COE5_Plane> planes,
            int version,
            string description
        )
        {
            _planes = planes;
            COE5_Version = version;
            Description = description;
        }
    }
}
