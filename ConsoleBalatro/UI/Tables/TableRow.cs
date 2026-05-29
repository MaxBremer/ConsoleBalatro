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
            RowValues = vals.ToArray();
        }
        public TableRow(string fullRow, string delimiter)
        {
            RowValues = fullRow.Split(delimiter);
        }
        public string[] RowValues;
    }
}
