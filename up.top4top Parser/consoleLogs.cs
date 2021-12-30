using System;
using System.Drawing;
using System.Windows.Forms;

namespace up.top4top_Parser
{
    class consoleLogs
    {

        public static void Add(RichTextBox box, string text, Color color)
        {
            string temp = "[" + DateTime.Now.ToShortTimeString() + "] " +
                          " " + text + Environment.NewLine;

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;

            box.AppendText(temp);
            box.SelectionColor = box.ForeColor;
        }
    }
}
