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
        private Game gameCurtain;
        private int activeGameIndex;

        private List<Game> games;
        private List<CustomGLControl> gameTabs;
 
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

            games = new List<Game>();
            gameTabs = new List<CustomGLControl>();
        }

        private void MainWindow_Load_1(object sender, EventArgs e)
        {
            ErrorReporting.Instance.ReportInfo(this, "starting game");
            if (Site == null || !Site.DesignMode)
            {
                gameParticle = new Game(N, dt, d);
                customGLControl1.init(gameParticle, GameType.Particle);
                settingsControl1.init(gameParticle);
                games.Add(gameParticle);
                gameTabs.Add(customGLControl1);

                gameCloth = new Game(N, dt, d);
                customGLControl2.init(gameCloth, GameType.Cloth);
                settingsControl2.init(gameCloth);
                games.Add(gameCloth);
                gameTabs.Add(customGLControl2);

                gameHair = new Game(N, dt, d);
                customGLControl3.init(gameHair, GameType.Hair);
                settingsControl3.init(gameHair);
                games.Add(gameHair);
                gameTabs.Add(customGLControl3);

                gameCurtain = new Game(N, dt, d);
                customGLControl4.init(gameCurtain, GameType.Curtain);
                settingsControl4.init(gameHair);
                games.Add(gameCurtain);
                gameTabs.Add(customGLControl4);

                customGLControl1.setActive(true);
                activeGameIndex = 0;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl control = (TabControl) sender;
            games[activeGameIndex].Pause();

            for (int i = 0; i < games.Count; i++)
            {
                Game game = games[i];
                CustomGLControl glControl = gameTabs[i];
                
                if(i == control.SelectedIndex)
                {
                    glControl.setActive(true);
                    
                }
                else
                {
                    glControl.setActive(false);
                }
            }
            activeGameIndex = control.SelectedIndex;
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

        private void settingsControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
