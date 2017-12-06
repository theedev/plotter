namespace Plotter
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colourListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imagePreparationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ditherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateColourMapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generatePatternMapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generatePatternSequencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showColourMapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewEdgeAndFillingMapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printOutlinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualControlModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToPlotterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DiameterMain = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.DiameterDecimal = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.halpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DiameterMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiameterDecimal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.imagePreparationToolStripMenuItem,
            this.previewToolStripMenuItem,
            this.printingToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(370, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openImageToolStripMenuItem,
            this.saveImageToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openImageToolStripMenuItem
            // 
            this.openImageToolStripMenuItem.Name = "openImageToolStripMenuItem";
            this.openImageToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.openImageToolStripMenuItem.Text = "Open Image";
            this.openImageToolStripMenuItem.Click += new System.EventHandler(this.openImageToolStripMenuItem_Click);
            // 
            // saveImageToolStripMenuItem
            // 
            this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
            this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.saveImageToolStripMenuItem.Text = "Save Image";
            this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colourListToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // colourListToolStripMenuItem
            // 
            this.colourListToolStripMenuItem.Name = "colourListToolStripMenuItem";
            this.colourListToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.colourListToolStripMenuItem.Text = "Colour List";
            this.colourListToolStripMenuItem.Click += new System.EventHandler(this.colourListToolStripMenuItem_Click);
            // 
            // imagePreparationToolStripMenuItem
            // 
            this.imagePreparationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ditherToolStripMenuItem,
            this.generateColourMapsToolStripMenuItem,
            this.generatePatternMapsToolStripMenuItem,
            this.generatePatternSequencesToolStripMenuItem});
            this.imagePreparationToolStripMenuItem.Name = "imagePreparationToolStripMenuItem";
            this.imagePreparationToolStripMenuItem.Size = new System.Drawing.Size(116, 20);
            this.imagePreparationToolStripMenuItem.Text = "Image Preparation";
            // 
            // ditherToolStripMenuItem
            // 
            this.ditherToolStripMenuItem.Name = "ditherToolStripMenuItem";
            this.ditherToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.ditherToolStripMenuItem.Text = "Dither";
            this.ditherToolStripMenuItem.Click += new System.EventHandler(this.ditherToolStripMenuItem_Click);
            // 
            // generateColourMapsToolStripMenuItem
            // 
            this.generateColourMapsToolStripMenuItem.Name = "generateColourMapsToolStripMenuItem";
            this.generateColourMapsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.generateColourMapsToolStripMenuItem.Text = "Generate Colour Maps";
            this.generateColourMapsToolStripMenuItem.Click += new System.EventHandler(this.generateColourMapsToolStripMenuItem_Click);
            // 
            // generatePatternMapsToolStripMenuItem
            // 
            this.generatePatternMapsToolStripMenuItem.Name = "generatePatternMapsToolStripMenuItem";
            this.generatePatternMapsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.generatePatternMapsToolStripMenuItem.Text = "Generate Pattern Maps";
            this.generatePatternMapsToolStripMenuItem.Click += new System.EventHandler(this.generatePatternMapsToolStripMenuItem_Click);
            // 
            // generatePatternSequencesToolStripMenuItem
            // 
            this.generatePatternSequencesToolStripMenuItem.Name = "generatePatternSequencesToolStripMenuItem";
            this.generatePatternSequencesToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.generatePatternSequencesToolStripMenuItem.Text = "Generate Pattern Sequences";
            this.generatePatternSequencesToolStripMenuItem.Click += new System.EventHandler(this.generatePatternSequencesToolStripMenuItem_Click);
            // 
            // previewToolStripMenuItem
            // 
            this.previewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showColourMapsToolStripMenuItem,
            this.previewEdgeAndFillingMapsToolStripMenuItem});
            this.previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            this.previewToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.previewToolStripMenuItem.Text = "Preview";
            // 
            // showColourMapsToolStripMenuItem
            // 
            this.showColourMapsToolStripMenuItem.Name = "showColourMapsToolStripMenuItem";
            this.showColourMapsToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.showColourMapsToolStripMenuItem.Text = "Preview Colour Maps";
            this.showColourMapsToolStripMenuItem.Click += new System.EventHandler(this.showColourMapsToolStripMenuItem_Click_1);
            // 
            // previewEdgeAndFillingMapsToolStripMenuItem
            // 
            this.previewEdgeAndFillingMapsToolStripMenuItem.Name = "previewEdgeAndFillingMapsToolStripMenuItem";
            this.previewEdgeAndFillingMapsToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.previewEdgeAndFillingMapsToolStripMenuItem.Text = "Preview Edge and Filling Maps";
            this.previewEdgeAndFillingMapsToolStripMenuItem.Click += new System.EventHandler(this.previewEdgeAndFillingMapsToolStripMenuItem_Click_1);
            // 
            // printingToolStripMenuItem
            // 
            this.printingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printOutlinesToolStripMenuItem,
            this.manualControlModeToolStripMenuItem,
            this.connectToPlotterToolStripMenuItem});
            this.printingToolStripMenuItem.Name = "printingToolStripMenuItem";
            this.printingToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.printingToolStripMenuItem.Text = "Printing";
            // 
            // printOutlinesToolStripMenuItem
            // 
            this.printOutlinesToolStripMenuItem.Name = "printOutlinesToolStripMenuItem";
            this.printOutlinesToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.printOutlinesToolStripMenuItem.Text = "Print Image";
            this.printOutlinesToolStripMenuItem.Click += new System.EventHandler(this.printOutlinesToolStripMenuItem_Click);
            // 
            // manualControlModeToolStripMenuItem
            // 
            this.manualControlModeToolStripMenuItem.Name = "manualControlModeToolStripMenuItem";
            this.manualControlModeToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.manualControlModeToolStripMenuItem.Text = "Manual Control Mode";
            this.manualControlModeToolStripMenuItem.Click += new System.EventHandler(this.manualControlModeToolStripMenuItem_Click_1);
            // 
            // connectToPlotterToolStripMenuItem
            // 
            this.connectToPlotterToolStripMenuItem.Name = "connectToPlotterToolStripMenuItem";
            this.connectToPlotterToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.connectToPlotterToolStripMenuItem.Text = "Connect To Plotter";
            this.connectToPlotterToolStripMenuItem.Click += new System.EventHandler(this.connectToPlotterToolStripMenuItem_Click_1);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.halpToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // DiameterMain
            // 
            this.DiameterMain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DiameterMain.Location = new System.Drawing.Point(95, 295);
            this.DiameterMain.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.DiameterMain.Name = "DiameterMain";
            this.DiameterMain.Size = new System.Drawing.Size(28, 20);
            this.DiameterMain.TabIndex = 5;
            this.DiameterMain.ValueChanged += new System.EventHandler(this.DiameterMain_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(117, 288);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 29);
            this.label1.TabIndex = 6;
            this.label1.Text = ".";
            // 
            // DiameterDecimal
            // 
            this.DiameterDecimal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DiameterDecimal.Location = new System.Drawing.Point(129, 295);
            this.DiameterDecimal.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.DiameterDecimal.Name = "DiameterDecimal";
            this.DiameterDecimal.Size = new System.Drawing.Size(32, 20);
            this.DiameterDecimal.TabIndex = 7;
            this.DiameterDecimal.ValueChanged += new System.EventHandler(this.DiameterDecimal_ValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 297);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Ink dot diameter:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(160, 297);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "mm";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(6, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(352, 262);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragDrop);
            this.pictureBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragEnter);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(249, 297);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Plotter not connected";
            // 
            // timer1
            // 
            this.timer1.Interval = 40;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(205, 295);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(38, 20);
            this.textBox1.TabIndex = 15;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // halpToolStripMenuItem
            // 
            this.halpToolStripMenuItem.Name = "halpToolStripMenuItem";
            this.halpToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.halpToolStripMenuItem.Text = "Halp";
            this.halpToolStripMenuItem.Click += new System.EventHandler(this.halpToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(370, 317);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.DiameterDecimal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DiameterMain);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.SystemColors.MenuText;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(386, 356);
            this.Name = "Form1";
            this.Text = "Plotter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DiameterMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiameterDecimal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colourListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem imagePreparationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ditherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateColourMapsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generatePatternMapsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generatePatternSequencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printOutlinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showColourMapsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previewEdgeAndFillingMapsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualControlModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToPlotterToolStripMenuItem;
        internal System.Windows.Forms.NumericUpDown DiameterMain;
        internal System.Windows.Forms.NumericUpDown DiameterDecimal;
        internal System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem halpToolStripMenuItem;
    }
}

