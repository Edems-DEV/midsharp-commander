using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp7.Components
{
    public class Table : IComponent
    {
        public event Action<int> RowSelected; 


        private List<string> headers;

        private List<Row> rows = new List<Row>();

        private int offset = 0;

        public int Selected { get; set; } = 0;

        public int Count { get; set; } = 10;

        public Table(string[] headers)
        {
            this.headers = new List<string>(headers);
        }

        public void Add(string[] data)
        {
            if (data.Length != headers.Count)
                throw new ArgumentException("Invalid columns count");

            rows.Add(new Row(data));
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.UpArrow)
            {
                if (Selected <= 0)
                    return;

                Selected--;

                if (Selected == offset - 1)
                    offset--;
            }
            else if (info.Key == ConsoleKey.DownArrow)
            {
                if (Selected >= rows.Count - 1)
                    return;

                Selected++;

                if (Selected == offset + Math.Min(Count, this.rows.Count))
                    offset++;
            }
            else if (info.Key == ConsoleKey.Enter)
            {
                this.RowSelected(this.Selected);
            }
        }

        public void Draw()
        {
            List<int> widths = Widths();

            DrawData(null, widths, '+', '-');
            DrawData(headers, widths, '|', ' ');
            DrawData(null, widths, '+', '=');

            for (int i = offset; i < offset + Math.Min(Count, this.rows.Count); i++)
            {
                if (i == Selected)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }

                DrawData(rows[i].Data, widths, '|', ' ');

                Console.ResetColor();
            }

            DrawData(null, widths, '+', '-');
        }

        private void DrawData(List<string>? data, List<int> widths, char sep, char pad)
        {
            int i = 0;
            foreach (int width in widths)
            {
                string value = data != null ? data[i] : "";

                Console.Write(sep);
                Console.Write(pad);
                Console.Write(value.PadRight(widths[i] + 1, pad));

                i++;
            }

            Console.WriteLine(sep);
        }

        private List<int> Widths()
        {
            List<int> widths = new List<int>();

            for (int i = 0; i < headers.Count; i++)
            {
                int width = headers[i].Length;

                foreach (Row item in rows)
                {
                    if (item.Data[i].Length > width)
                        width = item.Data[i].Length;
                }

                widths.Add(width);
            }

            return widths;
        }
    }
}
