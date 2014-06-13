using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluidsProject;
using OpenTK;
using System.Threading;

namespace FluidsProject.Interface
{
    public partial class CustomGLControl : GLControl
    {
        private Game game;
        private bool isActive = true;

        public CustomGLControl()
        {
            InitializeComponent();
        }

        public void init(Game game, string[] args)
        {
            //this.VSync = true;
            this.game = game;
            //this.settings = settings;

            Application.Idle += GameControl_Idle;
            this.Resize += game.OnResize;
            this.HandleDestroyed += GameControl_unload;
            this.Paint += GameControl_render;
            this.KeyDown += game.OnKeyDown;
            this.KeyUp += game.OnKeyUp;
            this.MouseDown += game.OnMouseDown;
            this.MouseUp += game.OnMouseUp;
            this.MouseMove += game.OnMouseMove;

            game.init(this.Width, this.Height, args);
        }

        public void setActive(bool active)
        {
            isActive = active;
            if (active)
            {
                MakeCurrent();
                SetWindowText();
            }
        }

        private void GameControl_render(object sender, PaintEventArgs e)
        {
            game.OnRenderFrame();
            this.SwapBuffers();
        }

        private void GameControl_unload(object sender, EventArgs e)
        {
            
        }

        private void GameControl_Idle(object sender, EventArgs e)
        {
            if(!isActive)
                return;

            if (isActive)
            {
                game.OnUpdateFrame();
                this.Invalidate();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Q)
                Application.Exit();

            base.OnKeyDown(e);
 
            if (!isActive)
            {
                return;
            }

            SetWindowText();
        }

        private void SetWindowText()
        {
            
        }
    }
}
