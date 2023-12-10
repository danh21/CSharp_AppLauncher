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
            textBox1.AllowDrop = true;
        }



        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }



        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            Array fileNames;
            string filePath;

            fileNames = (Array)e.Data.GetData(DataFormats.FileDrop);
            filePath = fileNames.GetValue(0).ToString();
            textBox1.Text = filePath;
        }



        private void openBtn_Click(object sender, EventArgs e)
        {
            string filePath = textBox1.Text;
            Process p;

            if (File.Exists(filePath))
            {
                p = new Process();
                p.StartInfo.FileName = filePath;
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }
            else if (Directory.Exists(filePath))
            {
                MessageBox.Show("This path is a directory!", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (filePath == "")
            {
                MessageBox.Show("Please fill the valid path!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show(filePath + " doesn't exist!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
