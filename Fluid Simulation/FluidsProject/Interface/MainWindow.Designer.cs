namespace FluidsProject.Interface
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
            this.customGLControl1 = new FluidsProject.Interface.CustomGLControl();
            this.SuspendLayout();
            // 
            // customGLControl1
            // 
            this.customGLControl1.BackColor = System.Drawing.Color.Black;
            this.customGLControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customGLControl1.Location = new System.Drawing.Point(0, 0);
            this.customGLControl1.Name = "customGLControl1";
            this.customGLControl1.Size = new System.Drawing.Size(875, 548);
            this.customGLControl1.TabIndex = 0;
            this.customGLControl1.VSync = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 548);
            this.Controls.Add(this.customGLControl1);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CustomGLControl customGLControl1;
    }
}