namespace PrepareImageFrm
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSelectesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detectShapesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearResultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePreparedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repeatDirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.histToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cLACHEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filtersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gaussianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bynaryzationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paramsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detectPaeamsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tvResults = new System.Windows.Forms.TreeView();
            this.drawTrajectoriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Location = new System.Drawing.Point(3, 326);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(348, 196);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(357, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1548, 317);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.processToolStripMenuItem,
            this.filtersToolStripMenuItem,
            this.paramsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1908, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openSelectesToolStripMenuItem,
            this.detectShapesToolStripMenuItem,
            this.clearResultToolStripMenuItem,
            this.savePreparedToolStripMenuItem,
            this.saveLogToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveDetailToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // openSelectesToolStripMenuItem
            // 
            this.openSelectesToolStripMenuItem.Name = "openSelectesToolStripMenuItem";
            this.openSelectesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
            this.openSelectesToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.openSelectesToolStripMenuItem.Text = "Open Selectes";
            this.openSelectesToolStripMenuItem.Click += new System.EventHandler(this.OpenSelectedToolStripMenuItem_Click);
            // 
            // detectShapesToolStripMenuItem
            // 
            this.detectShapesToolStripMenuItem.Name = "detectShapesToolStripMenuItem";
            this.detectShapesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.detectShapesToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.detectShapesToolStripMenuItem.Text = "Detect Shapes";
            this.detectShapesToolStripMenuItem.Click += new System.EventHandler(this.DetectShapesToolStripMenuItem_Click);
            // 
            // clearResultToolStripMenuItem
            // 
            this.clearResultToolStripMenuItem.Name = "clearResultToolStripMenuItem";
            this.clearResultToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.clearResultToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.clearResultToolStripMenuItem.Text = "Clear result";
            this.clearResultToolStripMenuItem.Click += new System.EventHandler(this.ClearResultToolStripMenuItem_Click);
            // 
            // savePreparedToolStripMenuItem
            // 
            this.savePreparedToolStripMenuItem.Name = "savePreparedToolStripMenuItem";
            this.savePreparedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.savePreparedToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.savePreparedToolStripMenuItem.Text = "Save Prepared";
            this.savePreparedToolStripMenuItem.Click += new System.EventHandler(this.SavePreparedToolStripMenuItem_Click);
            // 
            // saveLogToolStripMenuItem
            // 
            this.saveLogToolStripMenuItem.Name = "saveLogToolStripMenuItem";
            this.saveLogToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.saveLogToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.saveLogToolStripMenuItem.Text = "Save Log";
            this.saveLogToolStripMenuItem.Click += new System.EventHandler(this.SaveLogToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(187, 6);
            // 
            // saveDetailToolStripMenuItem
            // 
            this.saveDetailToolStripMenuItem.Name = "saveDetailToolStripMenuItem";
            this.saveDetailToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.saveDetailToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.saveDetailToolStripMenuItem.Text = "Save Detail";
            this.saveDetailToolStripMenuItem.Click += new System.EventHandler(this.SaveDetailToolStripMenuItem_Click);
            // 
            // processToolStripMenuItem
            // 
            this.processToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dirToolStripMenuItem,
            this.repeatDirToolStripMenuItem,
            this.histToolStripMenuItem,
            this.cLACHEToolStripMenuItem,
            this.drawTrajectoriesToolStripMenuItem});
            this.processToolStripMenuItem.Name = "processToolStripMenuItem";
            this.processToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this.processToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.processToolStripMenuItem.Text = "Process";
            // 
            // dirToolStripMenuItem
            // 
            this.dirToolStripMenuItem.Name = "dirToolStripMenuItem";
            this.dirToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.dirToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.dirToolStripMenuItem.Text = "Dir...";
            this.dirToolStripMenuItem.Click += new System.EventHandler(this.DirToolStripMenuItem_Click);
            // 
            // repeatDirToolStripMenuItem
            // 
            this.repeatDirToolStripMenuItem.Name = "repeatDirToolStripMenuItem";
            this.repeatDirToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.repeatDirToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.repeatDirToolStripMenuItem.Text = "Repeat dir";
            this.repeatDirToolStripMenuItem.Click += new System.EventHandler(this.RepeatDirToolStripMenuItem_Click);
            // 
            // histToolStripMenuItem
            // 
            this.histToolStripMenuItem.Name = "histToolStripMenuItem";
            this.histToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.histToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.histToolStripMenuItem.Text = "Hist";
            this.histToolStripMenuItem.Click += new System.EventHandler(this.HistToolStripMenuItem_Click);
            // 
            // cLACHEToolStripMenuItem
            // 
            this.cLACHEToolStripMenuItem.Name = "cLACHEToolStripMenuItem";
            this.cLACHEToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.cLACHEToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.cLACHEToolStripMenuItem.Text = "CLACHE";
            this.cLACHEToolStripMenuItem.Click += new System.EventHandler(this.ClaSheToolStripMenuItem_Click);
            // 
            // filtersToolStripMenuItem
            // 
            this.filtersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gaussianToolStripMenuItem,
            this.bynaryzationToolStripMenuItem});
            this.filtersToolStripMenuItem.Name = "filtersToolStripMenuItem";
            this.filtersToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.filtersToolStripMenuItem.Text = "Filters";
            // 
            // gaussianToolStripMenuItem
            // 
            this.gaussianToolStripMenuItem.Name = "gaussianToolStripMenuItem";
            this.gaussianToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.gaussianToolStripMenuItem.Text = "Gaussian";
            this.gaussianToolStripMenuItem.Click += new System.EventHandler(this.GaussianToolStripMenuItem_Click);
            // 
            // bynaryzationToolStripMenuItem
            // 
            this.bynaryzationToolStripMenuItem.Name = "bynaryzationToolStripMenuItem";
            this.bynaryzationToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.bynaryzationToolStripMenuItem.Text = "Bynaryzation";
            this.bynaryzationToolStripMenuItem.Click += new System.EventHandler(this.BinarizationToolStripMenuItem_Click);
            // 
            // paramsToolStripMenuItem
            // 
            this.paramsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detectPaeamsToolStripMenuItem});
            this.paramsToolStripMenuItem.Name = "paramsToolStripMenuItem";
            this.paramsToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.paramsToolStripMenuItem.Text = "Params";
            // 
            // detectPaeamsToolStripMenuItem
            // 
            this.detectPaeamsToolStripMenuItem.Name = "detectPaeamsToolStripMenuItem";
            this.detectPaeamsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.detectPaeamsToolStripMenuItem.Text = "DetectPaeams";
            this.detectPaeamsToolStripMenuItem.Click += new System.EventHandler(this.DetectParamsToolStripMenuItem_Click);
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(357, 326);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1548, 196);
            this.listBox1.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.56677F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 81.43323F));
            this.tableLayoutPanel1.Controls.Add(this.listBox1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tvResults, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 61.53846F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.46154F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1908, 525);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // tvResults
            // 
            this.tvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvResults.Location = new System.Drawing.Point(3, 3);
            this.tvResults.Name = "tvResults";
            this.tvResults.Size = new System.Drawing.Size(348, 317);
            this.tvResults.TabIndex = 4;
            // 
            // drawTrajectoriesToolStripMenuItem
            // 
            this.drawTrajectoriesToolStripMenuItem.Name = "drawTrajectoriesToolStripMenuItem";
            this.drawTrajectoriesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this.drawTrajectoriesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.drawTrajectoriesToolStripMenuItem.Text = "Draw Trajectories";
            this.drawTrajectoriesToolStripMenuItem.Click += new System.EventHandler(this.DrawTrajectoriesToolStripMenuItem_ClickAsync);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1908, 549);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ToolStripMenuItem detectShapesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePreparedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem processToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem histToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cLACHEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filtersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gaussianToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView tvResults;
        private System.Windows.Forms.ToolStripMenuItem bynaryzationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paramsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detectPaeamsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem repeatDirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSelectesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearResultToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveDetailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawTrajectoriesToolStripMenuItem;
    }
}

