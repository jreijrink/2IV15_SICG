using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Project1.Interface;

namespace Project1
{
	class Program
	{
		public static void Main(string[] args)
		{
            micfort.GHL.GHLWindowsInit.Init();
            startWindow(args);
		}

        public static void startWindow(string[] args)
        {
            MainWindow window = new MainWindow(args);
            window.ShowDialog();
        }
	}
}
