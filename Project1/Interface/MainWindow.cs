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
        private Game game;

        int N;
        float dt;
        float d;

        public MainWindow(string[] args)
        {
            InitializeComponent();

            if (args.Length == 0)
            {
                N = 64;
                dt = 0.001f;
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
                    Game game = new Game(N, dt, d);
                    customGLControl1.init(game);
                    settingsControl1.init(game);
                }
            }
            catch (Exception exception)
            {
                ErrorReporting.Instance.ReportFatalT(this, "Erro", exception);
            }
        }
    }
}
