using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FluidsProject.Interface
{
    public partial class MainWindow : Form
    {
        private Game fluidSimulation;
        private String[] args;

        public MainWindow(string[] args)
        {
            this.args = args;
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            if (Site == null || !Site.DesignMode)
            {
                fluidSimulation = new Game();
                customGLControl1.init(fluidSimulation, args);
            }
        }


    }
}
