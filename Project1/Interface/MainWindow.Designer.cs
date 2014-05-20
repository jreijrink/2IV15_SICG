namespace Project1.Interface
{
    partial class MainWindow
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.settingsControl1 = new Project1.Interface.SettingsControl();
            this.customGLControl1 = new Project1.Interface.CustomGLControl();
            this.settingsControl2 = new Project1.Interface.SettingsControl();
            this.customGLControl2 = new Project1.Interface.CustomGLControl();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1017, 595);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(1009, 569);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Particles";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.settingsControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.customGLControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1003, 563);
            this.splitContainer1.SplitterDistance = 333;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(1009, 569);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Cloth";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.settingsControl2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.customGLControl2);
            this.splitContainer2.Size = new System.Drawing.Size(1003, 563);
            this.splitContainer2.SplitterDistance = 333;
            this.splitContainer2.SplitterWidth = 3;
            this.splitContainer2.TabIndex = 0;
            // 
            // settingsControl1
            // 
            this.settingsControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.settingsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsControl1.Location = new System.Drawing.Point(0, 0);
            this.settingsControl1.Margin = new System.Windows.Forms.Padding(4);
            this.settingsControl1.Name = "settingsControl1";
            this.settingsControl1.Size = new System.Drawing.Size(333, 563);
            this.settingsControl1.TabIndex = 0;
            // 
            // customGLControl1
            // 
            this.customGLControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.customGLControl1.BackColor = System.Drawing.Color.Black;
            this.customGLControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customGLControl1.Location = new System.Drawing.Point(0, 0);
            this.customGLControl1.Margin = new System.Windows.Forms.Padding(4);
            this.customGLControl1.Name = "customGLControl1";
            this.customGLControl1.Size = new System.Drawing.Size(666, 563);
            this.customGLControl1.TabIndex = 0;
            this.customGLControl1.VSync = false;
            // 
            // settingsControl2
            // 
            this.settingsControl2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.settingsControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsControl2.Location = new System.Drawing.Point(0, 0);
            this.settingsControl2.Margin = new System.Windows.Forms.Padding(4);
            this.settingsControl2.Name = "settingsControl2";
            this.settingsControl2.Size = new System.Drawing.Size(333, 563);
            this.settingsControl2.TabIndex = 0;
            // 
            // customGLControl2
            // 
            this.customGLControl2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.customGLControl2.BackColor = System.Drawing.Color.Black;
            this.customGLControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customGLControl2.Location = new System.Drawing.Point(0, 0);
            this.customGLControl2.Margin = new System.Windows.Forms.Padding(4);
            this.customGLControl2.Name = "customGLControl2";
            this.customGLControl2.Size = new System.Drawing.Size(667, 563);
            this.customGLControl2.TabIndex = 0;
            this.customGLControl2.VSync = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 595);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainWindow";
            this.Text = "TinkerToy - Euler Integration";
            this.Load += new System.EventHandler(this.MainWindow_Load_1);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabPage tabPage2;
        private SettingsControl settingsControl1;
        private CustomGLControl customGLControl1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private SettingsControl settingsControl2;
        private CustomGLControl customGLControl2;
    }
}