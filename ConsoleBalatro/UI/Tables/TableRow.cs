using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.Tables
{
    public class TableRow
    {
        public TableRow(List<string> vals)
        {
            TrueRowValues = vals.ToArray();
        }
        public TableRow(string fullRow, string delimiter)
        {
            TrueRowValues = fullRow.Split(delimiter);
        }

        public void ValidateDisplayValues(List<TableColumn> columns)
        {
            DisplayRowValues = new string[TrueRowValues.Length];
            for (int i = 0; i < TrueRowValues.Length; i++)
            {
                if (i >= columns.Count)
                {
                    DisplayRowValues[i] = TrueRowValues[i];
                    continue;
                }
                var column = columns[i];
                var value = TrueRowValues[i];
                if (value.Length > column.Width)
                {
                    value = value.Substring(0, column.Width - 3) + "...";
                }
                DisplayRowValues[i] = value.PadRight(column.Width);
            }
        }

        public string[] TrueRowValues;
        public string[] DisplayRowValues;
    }
}
