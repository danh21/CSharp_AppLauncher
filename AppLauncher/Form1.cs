using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;



namespace AppLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.listView1.AllowDrop = true;
        }



        private void LaunchFile(string filePath)
        {
            Process p;

            if (File.Exists(filePath))
            {
                p = new Process();
                p.StartInfo.FileName = filePath;
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }
            else if (Directory.Exists(filePath))
                MessageBox.Show("This path is a directory!", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(filePath + " doesn't exist!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        
        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            Array filePaths;

            if (e.Data != null)
            {
                filePaths = (Array)e.Data.GetData(DataFormats.FileDrop);

                foreach (string filePath in filePaths)
                    listView1.Items.Add(filePath);
            }
        }



        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
        }



        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string filePath = listView1.FocusedItem.Text;
            LaunchFile(filePath);
        }
    }
}
