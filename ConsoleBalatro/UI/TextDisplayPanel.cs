using ConsoleBalatro.UI.EngineUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI
{
    public class TextDisplayPanel : PanelDisplayEntity
    {
        protected List<string> _textLines = new();
        protected int _minWidth = -1;
        protected int _minHeight = -1;
        public TextDisplayPanel(List<string> textLines) : base(0, 0)
        {
            _textLines.AddRange(textLines);
            Sprite = new string[_textLines.Count + 4, GetWidthByLines()];
        }

        public TextDisplayPanel(List<string> textLines, int minWidth, int minHeight) : base(0, 0)
        {
            _textLines.AddRange(textLines);
            _minWidth = minWidth;
            _minHeight = minHeight;
            minDimSetup();
        }

        public TextDisplayPanel(string wholeString, string divider) : base(0, 0)
        {
            SetLines(wholeString, divider);
        }

        public TextDisplayPanel(string textLine, int textWrapWidth = -1) : base(0, 0)
        {
            _textLines.Add(textLine);
            TextWrapWidth = textWrapWidth;
            Sprite = new string[_textLines.Count + 4, GetWidthByLines()];
            if (TextWrapWidth != -1)
                AdjustLinesByWrapWidth();
        }

        public List<string> Lines => _textLines;
        public bool NoBorder = false;
        public bool Display { get; set; } = true;
        public int TextWrapWidth { get; set; } = -1;

        public void AdjustLinesByWrapWidth(int passedWidth = -1)
        {
            if (passedWidth != -1)
            {
                TextWrapWidth = passedWidth;
            }
            var newList = new List<string>();
            foreach (var line in _textLines)
            {
                if (line.Length <= TextWrapWidth)
                {
                    newList.Add(line);
                }
                else
                {
                    int start = 0;
                    while (start < line.Length)
                    {
                        int lengthToTake = Math.Min(TextWrapWidth, line.Length - start);

                        // If the segment fits, just add it.
                        if (lengthToTake == line.Length - start)
                        {
                            newList.Add(line.Substring(start, lengthToTake));
                            break;
                        }

                        // Try to find the last space within the allowed width
                        int lastSpace = line.LastIndexOf(' ', start + lengthToTake - 1, lengthToTake);
                        if (lastSpace > start)
                        {
                            // Break at the last space
                            newList.Add(line.Substring(start, lastSpace - start));
                            start = lastSpace + 1; // Move past the space
                        }
                        else
                        {
                            // No space found, force break at max width
                            newList.Add(line.Substring(start, lengthToTake));
                            start += lengthToTake;
                        }
                    }
                }
            }
            _textLines = newList;
        }

        public void SetLines(string wholeString, string divider)
        {
            _textLines.Clear();
            _textLines.AddRange(wholeString.Split(divider));
            Sprite = new string[_textLines.Count + 4, GetWidthByLines()];
        }

        public void SetLinesMaxWidth(string wholeString, int maxWidth)
        {
            _textLines.Clear();
            _textLines.Add(new string('x', maxWidth));//XTREME JANK. But this way I don't have to write another func :)
            //the longer this project goes on, the greater the jank I will accept.
            EngineDisplayGlobals.AddButDontExpand(_textLines, wholeString);
            _textLines.RemoveAt(0);
            Sprite = new string[_textLines.Count + 4, GetWidthByLines()];
        }

        private void minDimSetup()
        {
            int wNum = Math.Max(_minWidth, GetWidthByLines());
            int hNum = Math.Max(_minHeight, _textLines.Count + 4);
            Sprite = new string[hNum, wNum];
        }

        private int GetWidthByLines()
        {
            int max = 0;
            foreach (var line in _textLines)
            {
                max = Math.Max(max, line.Length + 4);
            }
            return max + 4;
        }

        public override void PreDisplaySetup()
        {
            if (!Display)
            {
                Sprite = new string[0, 0];
                return;
            }
            if (_minWidth >= 0)
            {
                minDimSetup();
            }
            else
            {
                Sprite = new string[_textLines.Count + 4, GetWidthByLines()];
            }
            base.PreDisplaySetup();

            int lnCount = 0;
            foreach (var line in _textLines)
            {
                int xPos = 2;
                int yPos = 2 + lnCount;
                InsertOtherStringDirect(xPos, yPos, line);

                lnCount++;
            }
        }
    }
}
