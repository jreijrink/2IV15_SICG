using micfort.GHL.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Project1.Interface
{
    public partial class MainWindow : Form
    {
        private Game gameParticle;
        private Game gameCloth;
        private Game gameHair;

        int N;
        float dt;
        float d;

        public MainWindow(string[] args)
        {
            InitializeComponent();

            if (args.Length == 0)
            {
                N = 64;
                dt = 0.01f;                
                d = 5.0f;

            }
            else
            {
                N = int.Parse(args[0]);
                dt = int.Parse(args[1]);
                d = int.Parse(args[2]);
            }
        }

        private void MainWindow_Load_1(object sender, EventArgs e)
        {
            try
            {
                ErrorReporting.Instance.ReportInfo(this, "starting game");
                if (Site == null || !Site.DesignMode)
                {
                    gameParticle = new Game(N, dt, d);
                    customGLControl1.init(gameParticle, GameType.Particle);
                    settingsControl1.init(gameParticle);

                    gameCloth = new Game(N, dt, d);
                    customGLControl2.init(gameCloth, GameType.Cloth);
                    settingsControl2.init(gameCloth);

                    gameHair = new Game(N, dt, d);
                    customGLControl3.init(gameHair, GameType.Hair);
                    settingsControl3.init(gameHair);

                    customGLControl1.setActive(true);
                    customGLControl2.setActive(false);
                    customGLControl3.setActive(false);

                }
            }
            catch (Exception exception)
            {
                ErrorReporting.Instance.ReportFatalT(this, "Erro", exception);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl control = (TabControl) sender;

            if (control.SelectedIndex == 0)
            {
                gameCloth.Pause();
                customGLControl1.setActive(true);
                customGLControl2.setActive(false);
                customGLControl3.setActive(false);
            }
            else if (control.SelectedIndex == 1)
            {
                gameParticle.Pause();
                customGLControl1.setActive(false);
                customGLControl2.setActive(true);
                customGLControl3.setActive(false);
            }
            else if (control.SelectedIndex == 2)
            {
                gameHair.Pause();
                customGLControl1.setActive(false);
                customGLControl2.setActive(false);
                customGLControl3.setActive(true);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.I)
            {
                int intMode;
                if(tabControl1.SelectedIndex == 0)
                {
                    intMode = gameParticle.integrationMode;
                }
                else
                {
                    intMode = gameCloth.integrationMode;
                }

                string windowText = "TinkerToy: ";
                if(intMode == 0)
                {
                    this.Text = windowText + " Euler";
                }
                else if (intMode == 1)
                {
                    this.Text = windowText + " MidPoint";
                }
                else if (intMode == 2)
                {
                    this.Text = windowText + " RungaKutta";
                }
            }
                 
        }
    }
}
