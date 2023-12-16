using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Collections.Generic;



namespace AppLauncher
{
    public partial class Form1 : Form
    {
        string currentCfg;
        string recentFiles;



        public Form1()
        {
            InitializeComponent();
            recentFiles = Path.GetDirectoryName(Application.StartupPath + "\\recordRecentFiles.txt\\"); ;
        }



        // open files in config
        private void launch(string path)
        {
            Process p;

            if (File.Exists(path))
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



        // add files into config as button-based
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



        private void saveCfg(string path)
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                foreach (Control c in flowLayoutPanel1.Controls)
                    outputFile.WriteLine(c.Tag);
            }

            addToRecentFilesMenu(path);
        }



        private void saveAsCfg()
        {
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.StartupPath + "\\configs\\");
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveCfg(saveFileDialog1.FileName);
                currentCfg = saveFileDialog1.FileName;
            }
        }



        // handler for load button in Files menu
        private void loadCfg(string pathConfig)
        {
            string pathFile;
            currentCfg = pathConfig;

            flowLayoutPanel1.Controls.Clear();

            using (StreamReader inputFile = new StreamReader(pathConfig))
            {
                while ((pathFile = inputFile.ReadLine()) != null)
                    AddButton(pathFile);
            }
        }



        void addToRecentFilesMenu(string path)
        {
            if (path != null)
            {
                // limit 4 files
                if (recentFilesMenu.DropDownItems.Count > 3)
                    recentFilesMenu.DropDownItems.RemoveAt(0);
                recentFilesMenu.DropDownItems.Add(new ToolStripMenuItem(path, null, new EventHandler(cfg_load)));
            }
        }



        void saveRecentFiles(string path)
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                for (int i = 0; i < recentFilesMenu.DropDownItems.Count; i++)
                    outputFile.WriteLine(recentFilesMenu.DropDownItems[i].Text);
            }
        }



        void loadRecentFiles(string path)
        {
            string cfg;

            using (StreamReader inputFile = new StreamReader(path))
            {
                while ((cfg = inputFile.ReadLine()) != null)
                    addToRecentFilesMenu(cfg);
            }
        }



        // event handler for files in config
        private void btn_Click(object sender, EventArgs e)
        {
            launch(((Button)sender).Tag.ToString());
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



        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            currentCfg = null;
        }



        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentCfg != null)
                saveCfg(currentCfg);
            else
                saveAsCfg();
        }



        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAsCfg();
        }



        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.StartupPath + "\\configs\\");
            openFileDialog1.Filter = "Text files (*.txt)|*.txt";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadCfg(openFileDialog1.FileName);
                addToRecentFilesMenu(openFileDialog1.FileName);
            }
        }



        // event handler for recent files in menu
        private void cfg_load(object sender, EventArgs e)
        {
            string cfgPath = ((ToolStripMenuItem)sender).Text;
            FileInfo fi = new FileInfo(cfgPath);

            if (fi.Exists)
            {
                loadCfg(cfgPath);
                currentCfg = cfgPath;
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(recentFiles))
                loadRecentFiles(recentFiles);
        }



        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            saveRecentFiles(recentFiles);
        }
    }
}
