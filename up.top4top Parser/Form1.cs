using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace up.top4top_Parser
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            consoleLogs.Add(richTextBox1, "Coded with love ^_^ By Spawner.", Color.LightSeaGreen);
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            consoleLogs.Add(richTextBox1, "Searching ...", Color.Black);

            metroButton1.Enabled = false;

            metroProgressBar1.Value = 0;
            listView1.Items.Clear();

            browseWeb web = new browseWeb
            (
                ProcessFinished,
                richTextBox1,
                listView1,
                new Random().Next(2) == 1 ? "https" : "http",
                textBox1.Text, textBox2.Text,
                metroProgressBar1,
                "Random"
            );

            web.NavigateSite();

        }
        public void ProcessFinished()
        {

            MessageBox.Show("Items Found : {browseWeb.webElement.Count}", "Search Completed.",
                MessageBoxButtons.OK,
                browseWeb.webElement.Count > 0 ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            consoleLogs.Add(richTextBox1, "Done : The scan has finished.", Color.Green);
            consoleLogs.Add(richTextBox1, "Items Found : {browseWeb.webElement.Count}", Color.Red);

            metroButton1.Enabled = true;
        }

        #region Control Form move

        /*  API stuff to move the form    #Copied  */
        private const int WM_NCLBUTTONDOWN = 0xA1;

        private const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();
        /*  End it's declaration      */

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion
        #region Navigate to the URL with the Selected Item
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                Process.Start(selectedItem.SubItems[0].Text);
            }
        }
        #endregion

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}