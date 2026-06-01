using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.Tables
{
    public class TableDisplayEntity : PanelDisplayEntity
    {
        public Table MyTable;

        public TableDisplayEntity() : base(1, 1)
        {
            MyTable = new();
        }

        public void BuildTable(List<(string contents, int width)> headers, List<List<string>> rows, int tableWidth)
        {
            MyTable = new();
            foreach (var h in headers)
            {
                MyTable.Columns.Add(new TableColumn { Header = h.contents, Width = h.width });
            }
            foreach (var r in rows)
            {
                MyTable.Rows.Add(new TableRow(r));
            }
            MyTable.Width = tableWidth;
        }

        public void SimpleTable(string headers, List<string> rows, string delimiter = ",", int minColWidth = 10)
        {
            MyTable = new Table
            {
                Columns = headers.Split(delimiter).Select(x => new TableColumn { Header = x.Trim(), Width = minColWidth }).ToList(),
                Rows = rows.Select(x => new TableRow(x, delimiter)).ToList()
            };
        }

        public override void PreDisplaySetup()
        {
            SetupSprite();
            base.PreDisplaySetup();
            var lines = MyTable.BuildDisplayLines();
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                for (int j = 0; j < line.Length && j < Width - 2; j++)
                {
                    Sprite[i + 1, j + 1] = line[j].ToString();
                }
            }
        }

        private void SetupSprite()
        {
            //+2 for the outline, +1 for the header
            Sprite = new string[Math.Max(MyTable.GetContentsHeight() + 2, Height), MyTable.Width + 2];
        }
    }
}
