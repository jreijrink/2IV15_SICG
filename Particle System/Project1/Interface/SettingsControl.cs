using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Project1.Interface
{
    public partial class SettingsControl : UserControl
    {
        private Game game;

        public SettingsControl()
        {
            InitializeComponent();
        }

        public void init(Game game)
        {
            this.game = game;
        }
    }
}
