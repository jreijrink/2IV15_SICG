using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using System.Threading;

namespace Project1.Interface
{
    public partial class CustomGLControl : GLControl
    {
        private Game game;
        private bool isActive = false;
        private int fps = 200;
        private long elapsedTime;
        private System.Threading.Timer timer;

        public CustomGLControl()
        {
            InitializeComponent();
        }

        public void init(Game game, GameType type)
        {
            this.VSync = true;
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

            if(type == GameType.Particle)
                game.InitParticleSystem(this.ClientRectangle);
            else if (type == GameType.Cloth)
                game.InitClothSystem(this.ClientRectangle);
            else if (type == GameType.Hair)
                game.InitHairSystem(this.ClientRectangle);

            timer = new System.Threading.Timer(frameElapsed);

        }

        private void frameElapsed(object state)
        {
            throw new NotImplementedException();
        }

        public void setActive(bool active)
        {
            isActive = active;
            if (active)
            {
                MakeCurrent();
                SetWindowText();
                //sw.Start();
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
            int intMode = game.integrationMode;

            string windowText = "TinkerToy: ";
            if (intMode == 0)
            {
                windowText = windowText + " Euler";
            }
            else if (intMode == 1)
            {
                windowText = windowText + " MidPoint";
            }
            else if (intMode == 2)
            {
                windowText = windowText + " RungaKutta";
            }

            windowText += ", TimeStep : " + game.dt;
            windowText += ", SpeedUp : " + game.numSteps;

            this.TopLevelControl.Text = windowText;
        }
    }
}
