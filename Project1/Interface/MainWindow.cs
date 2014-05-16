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

        int N;
        float dt;
        float d;

        public MainWindow(string[] args)
        {
            InitializeComponent();

            if (args.Length == 0)
            {
                N = 64;
                dt = 0.001f;                d = 5.0f;

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
                }
            }
            catch (Exception exception)
            {
                ErrorReporting.Instance.ReportFatalT(this, "Erro", exception);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl control = (TabControl)sender;

            if (control.SelectedIndex == 0)
                gameCloth.Pause();
            else if (control.SelectedIndex == 1)
                gameParticle.Pause();
        }
    }
}
