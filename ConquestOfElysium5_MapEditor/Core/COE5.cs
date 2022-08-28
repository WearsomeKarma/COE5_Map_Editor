using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConquestOfElysium5_MapEditor.Core
{
    public static class COE5
    {
        public const int GROUND_TYPE_COUNT = 28;
        public const int LOGIC_TYPE_COUNT = 353;

        public static COE5_Data_Directory Data_Directory { get; internal set; }
        public static COE5_Roaming_Directory Roaming_Directory { get; internal set; }

        public static IEnumerable<COE5_Ground_Type> Cast
        (
            string sequence, 
            char seperator, 
            Action<int> failure_callback = null,
            bool stop_on_failure = false
        )
        {
            string[] split = sequence.Split(seperator);

            bool failure;
            // -1 because .coem has a redundant ',' at the end of each comma seperated sequence.
            for(int i=0;i<split.Length-1;i++)
            {
                COE5_Ground_Type ground_type =
                    Cast(split[i], out failure);

                if (failure)
                    failure_callback?
                    .Invoke(i);

                if (failure && stop_on_failure)
                    yield break;

                yield return ground_type;
            }
        }

        public static COE5_Ground_Type Cast(string s)
            => Cast(s, out bool _);
        public static COE5_Ground_Type Cast(string s, out bool failure)
        {
            int i;
            bool success =
                int.TryParse(s, out i)
                ;

            failure = !success;

            return
                (success)
                ? Cast(i, out failure)
                : COE5_Ground_Type.Arid
                ;
        }


        public static COE5_Ground_Type Cast(int i)
            => Cast(i, out bool _);
        public static COE5_Ground_Type Cast(int i, out bool failure)
        {
            bool success =
                (i >= 0 && i < GROUND_TYPE_COUNT)
                ;

            failure = !success;

            return
                (success)
                ? (COE5_Ground_Type)i
                : COE5_Ground_Type.Arid
                ;
        }
    }
}
