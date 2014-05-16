using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace Project1.Interface
{
    public partial class CustomGLControl : GLControl
    {
        private Game game;

        public CustomGLControl()
        {
            InitializeComponent();
        }

        public void init(Game game, GameType type)
        {
            this.game = game;
            //this.settings = settings;

            Application.Idle += GameControl_Idle;
            this.Resize += game.OnResize;
            this.HandleDestroyed += GameControl_unload;
            this.Paint += GameControl_render;
            this.KeyDown += game.OnKeyDown;
            this.KeyUp += game.OnKeyUp;
            this.MouseDown += game.OnMouseDown;
            this.MouseUp += game.OnMouseDown;
            this.MouseMove += game.OnMouseMove;

            if(type == GameType.Particle)
                game.InitParticleSystem(this.ClientRectangle);
            else if (type == GameType.Cloth)
                game.InitClothSystem(this.ClientRectangle);
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
            game.OnUpdateFrame();
            this.Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Q)
                Application.Exit();

            base.OnKeyDown(e);
        }
        
    }
}
