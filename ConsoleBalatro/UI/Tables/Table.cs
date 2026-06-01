using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.Tables
{
    public class Table
    {
        public string Name { get; set; }
        public int Width;
        public bool HeaderBottomBorder = true;
        public TableStretchMode StretchMode = TableStretchMode.KeepProportions;
        public List<TableRow> Rows { get; set; } = [];
        public List<TableColumn> Columns { get; set; } = [];

        public List<string> BuildDisplayLines()
        {
            Validate();
            var lines = new List<string>();
            var headerLine = string.Join("", Columns.Select(x => x.Header.PadRight(x.Width)));
            lines.Add(headerLine);

            if (HeaderBottomBorder)
            {
                var headerBorderLine = new string('-', Width);
                lines.Add(headerBorderLine);
            }

            foreach (var row in Rows)
            {
                var line = string.Join("", row.DisplayRowValues);
                lines.Add(line);
            }
            return lines;
        }

        public int GetContentsHeight()
        {
            if(HeaderBottomBorder)
                return Rows.Count + 2;//Header and header bar
            return Rows.Count + 1;//just header
        }

        private void Validate()
        {
            // If the total width of the columns exceeds the table width, we need to reduce the width of the columns proportionally
            while (Columns.Sum(x => x.Width) > Width)
            {
                switch (StretchMode)
                {
                    case TableStretchMode.EvenColumns:
                        var column = Columns.OrderByDescending(x => x.Width).First();
                        column.Width--;
                        break;
                    case TableStretchMode.KeepProportions:
                        foreach (var c in Columns)
                        {
                            c.Width--;
                        }
                        break;
                }
                
            }

            foreach (var row in Rows)
            {
                row.ValidateDisplayValues(Columns);
            }
        }
    }

    public enum TableStretchMode
    {
        EvenColumns,
        KeepProportions,
    }
}
