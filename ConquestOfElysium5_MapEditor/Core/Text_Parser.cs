using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquestOfElysium5_MapEditor.Core
{
    public class Text_Parser
    {
        private readonly StreamReader _stream;
        public string Current_Line { get; private set; }
        public int Current_Line_Number { get; private set; }
        public bool Can_Continue
            => !_stream.EndOfStream;

        public Text_Parser(StreamReader stream)
        {
            _stream = stream;
            Current_Line_Number = 0;
        }

        public bool Next_Line()
        {
            if (_stream.EndOfStream)
                return false;
            Current_Line = _stream.ReadLine();
            Current_Line_Number++;

            return true;
        }

        public Text_Parser Next_Line(Action failure_callback = null)
        {
            if (!Next_Line())
                failure_callback?.Invoke();
            return this;
        }

        public Text_Parser Move_To(string pattern, Action failure_callback = null)
        {
            bool failure;
            Move_To(pattern, out failure);

            if (failure)
                failure_callback?.Invoke();

            return this;
        }

        public Text_Parser Move_To(string pattern, out bool failure)
        {
            while (!(failure = !Next_Line()))
                if (Current_Line.Contains(pattern))
                    break;

            return this;
        }

        public Text_Parser Get_Word(int index, out string val, Action failure_callback = null)
        {
            bool failure;
            Get_Word(index, out val, out failure);

            if (failure)
                failure_callback?.Invoke();

            return this;
        }

        public Text_Parser Get_Word(int index, out string val, out bool failure)
        {
            string[] split = Current_Line.Split(' ');

            val = null;
            if (failure = index < 0 || index >= split.Length)
                return this;

            val = split[index];

            return this;
        }

        public Text_Parser Get_Integer(int index, out int val, Action failure_callback = null)
        {
            bool failure;
            Get_Integer(index, out val, out failure);

            if (failure)
                failure_callback?.Invoke();

            return this;
        }

        public Text_Parser Get_Integer(int index, out int val, out bool failure)
        {
            string word;
            Get_Word(index, out word, out failure);
            val = 0;
            if (failure || (failure = !int.TryParse(word, out val)))
                return this;

            return this;
        }
    }
}
