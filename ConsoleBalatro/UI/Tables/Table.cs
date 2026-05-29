using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.Tables
{
    public class Table
    {
        public required string Name { get; set; }
        public int Width;
        public List<TableRow> Rows { get; set; } = [];
        public List<TableColumn> Columns { get; set; } = [];

        private void Validate()
        {

        }
    }
}
