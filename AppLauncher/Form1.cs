using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;



namespace AppLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.flowLayoutPanel1.AllowDrop = true;
        }



        private void launch(string path)
        {
            Process p;

            if (File.Exists(path) || Directory.Exists(path))
            {
                p = new Process();
                p.StartInfo.FileName = path;
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }
            else if (Directory.Exists(path))
                MessageBox.Show("This path is a directory!", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(path + " doesn't exist!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }



        private void btn_Click(object sender, EventArgs e)
        {
            launch(((Button)sender).Tag.ToString());
        }



        private void AddButton(string path)
        {
            Button btn;
            Icon icon;

            btn = new Button();
            btn.Size = new Size(70, 70);
            btn.UseVisualStyleBackColor = true;
            btn.Tag = path;
            btn.Text = Path.GetFileNameWithoutExtension(path);
            btn.Click += new EventHandler(btn_Click);

            // set alignment
            btn.TextAlign = ContentAlignment.BottomCenter;
            btn.ImageAlign = ContentAlignment.TopCenter;

            // add icon image
            icon = Icon.ExtractAssociatedIcon(path);
            btn.Image = icon.ToBitmap();
           
            flowLayoutPanel1.Controls.Add(btn);
        }



        private void flowLayoutPanel1_DragDrop(object sender, DragEventArgs e)
        {
            Array paths;

            if (e.Data != null)
            {
                paths = (Array)e.Data.GetData(DataFormats.FileDrop);

                foreach (string path in paths)
                    AddButton(path);
            }
        }



        private void flowLayoutPanel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
        }
    }
}
