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
        #region global vars
        string currentCfg;
        string recentFiles;
        bool dragging;
        int x_Org;
        int y_Org;
        #endregion



        public Form1()
        {
            InitializeComponent();
            recentFiles = Path.GetDirectoryName(Application.StartupPath + "\\recordRecentFiles.txt\\");
            dragging = false;
        }



        #region init files in panel
        private void mainPanel_DragDrop(object sender, DragEventArgs e)
        {
            Array paths;

            if (e.Data != null)
            {
                paths = (Array)e.Data.GetData(DataFormats.FileDrop);

                foreach (string path in paths)
                    AddButton(path);
            }
        }



        private void mainPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
        }



        // add files into config as button-based
        private void AddButton(string path)
        {
            Button btn;
            Icon icon;

            if (File.Exists(path) || Directory.Exists(path))
            {
                icon = getIcon(path);
                btn = CreateButton(path, icon);
                autoSetPos(btn);
                mainPanel.Controls.Add(btn);
            }
        }



        private Icon getIcon(string path)
        {
            Icon icon = new Icon(SystemIcons.Exclamation, 40, 40);

            if (File.Exists(path))
                icon = Icon.ExtractAssociatedIcon(path);
            else if (Directory.Exists(path))
                icon = GetExplorerIcon();

            return icon;
        }



        private Icon GetExplorerIcon()
        {
            string winfolder;
            Icon icon;

            winfolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            icon = Icon.ExtractAssociatedIcon(winfolder + "\\explorer.exe");
            return icon;
        }



        private Button CreateButton(string path, Icon icon)
        {
            Button btn;

            btn = new Button();
            btn.Size = new Size(70, 70);
            btn.UseVisualStyleBackColor = true;
            btn.Tag = path;
            btn.Text = Path.GetFileNameWithoutExtension(path);

            // add event-handlers
            btn.Click += new EventHandler(btn_Click);
            btn.MouseDown += new MouseEventHandler(MouseDown);
            btn.MouseMove += new MouseEventHandler(MouseMove);
            btn.MouseUp += new MouseEventHandler(MouseUp);

            // set alignment
            btn.TextAlign = ContentAlignment.BottomCenter;
            btn.ImageAlign = ContentAlignment.TopCenter;

            // add icon image            
            btn.Image = icon.ToBitmap();

            // Add Context Menu
            btn.ContextMenuStrip = ctrlItemMenu;

            return btn;
        }



        private void autoSetPos(Button b)
        {
            int numctrls = mainPanel.Controls.Count;

            if (numctrls > 0)
            {
                Button lastBtn = (Button)mainPanel.Controls[numctrls - 1];
                int lastX = lastBtn.Location.X;
                b.Location = new Point(lastX + lastBtn.Width + 10, lastBtn.Location.Y);
            }
            else
            {
                b.Location = new Point(10, 10);
            }
        }
        #endregion



        #region handle files (buttons) in panel
        private void btn_Click(object sender, EventArgs e)
        {
            if (!dragging)
                launch(((Button)sender).Tag.ToString());
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
                Process.Start("explorer.exe", path);
            else
                MessageBox.Show(path + " doesn't exist!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }



        private void ctrlItemMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Button btn = (Button)ctrlItemMenu.SourceControl;

            if (e.ClickedItem == delItemMenu)
                deleteBtn(btn);
            else if (e.ClickedItem == moveLeftItemMenu)
                moveLeftBtn(btn);
            else if (e.ClickedItem == moveRightItemMenu)
                moveRightBtn(btn);
        }



        private void deleteBtn(Button btn)
        {
            if (mainPanel.Controls.Contains(btn))
            {
                if (MessageBox.Show("Do you want to delete " + btn.Text + "?", "Remove Button", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    mainPanel.Controls.Remove(btn);
            }
        }



        private void moveLeftBtn(Button btn)
        {
            int index = mainPanel.Controls.GetChildIndex(btn);

            if (index > 0)
                mainPanel.Controls.SetChildIndex(btn, index - 1);

            ReOrderButtons();
        }



        private void moveRightBtn(Button btn)
        {
            int index = mainPanel.Controls.GetChildIndex(btn);

            if (index < mainPanel.Controls.Count - 1)
                mainPanel.Controls.SetChildIndex(btn, index + 1);

            ReOrderButtons();
        }



        private void ReOrderButtons()
        {
            string[] paths = new string[mainPanel.Controls.Count];
            Button b;
            string s = "";


            for (int i = 0; i < mainPanel.Controls.Count; i++)
            {
                b = (Button)mainPanel.Controls[i];
                paths[i] = (string)b.Tag;
            }

            mainPanel.Controls.Clear();

            this.SuspendLayout();

            for (int i = 0; i < paths.Length; i++)
                AddButton(paths[i]);

            this.ResumeLayout();
        }



        private void MouseDown(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                dragging = true;
                x_Org = e.Location.X;
                y_Org = e.Location.Y;
            }
        }



        private void MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }



        private void MouseMove(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            int x_movement;
            int y_movement;
            int x_new;
            int y_new;

            if (dragging)
            {
                x_movement = e.Location.X - x_Org;
                y_movement = e.Location.Y - y_Org;

                x_new = b.Location.X + x_movement;
                y_new = b.Location.Y + y_movement;

                if ((x_new <= 0) || (x_new + b.Width >= this.ClientSize.Width))
                    x_new = b.Location.X;

                if ((y_new <= 0) || (y_new + b.Height >= this.ClientSize.Height))
                    y_new = b.Location.Y;

                b.Location = new Point(x_new, y_new);
            }
        }
        #endregion



        #region handle File menu
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



        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainPanel.Controls.Clear();
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



        private void loadCfg(string pathConfig)
        {
            string pathFile;
            currentCfg = pathConfig;

            mainPanel.Controls.Clear();

            using (StreamReader inputFile = new StreamReader(pathConfig))
            {
                while ((pathFile = inputFile.ReadLine()) != null)
                    AddButton(pathFile);
            }
        }



        private void saveCfg(string path)
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                foreach (Control c in mainPanel.Controls)
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
        #endregion



        #region handle recent files menu
        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(recentFiles))
                loadRecentFiles(recentFiles);
        }



        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            saveRecentFiles(recentFiles);
        }



        private void addToRecentFilesMenu(string path)
        {
            if (path != null)
            {
                // limit 4 files
                if (recentFilesMenu.DropDownItems.Count > 3)
                    recentFilesMenu.DropDownItems.RemoveAt(0);

                if ((recentFilesMenu.DropDownItems.Count == 0) || (recentFilesMenu.DropDownItems[recentFilesMenu.DropDownItems.Count-1].Text != path))
                    recentFilesMenu.DropDownItems.Add(new ToolStripMenuItem(path, null, new EventHandler(cfg_load)));
            }
        }



        private void saveRecentFiles(string path)
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                for (int i = 0; i < recentFilesMenu.DropDownItems.Count; i++)
                    outputFile.WriteLine(recentFilesMenu.DropDownItems[i].Text);
            }
        }



        private void loadRecentFiles(string path)
        {
            string cfg;

            using (StreamReader inputFile = new StreamReader(path))
            {
                while ((cfg = inputFile.ReadLine()) != null)
                    addToRecentFilesMenu(cfg);
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
        #endregion
    }
}
