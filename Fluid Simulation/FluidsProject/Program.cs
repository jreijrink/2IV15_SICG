using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using FluidsProject.Interface;

namespace FluidsProject
{
	class Program
	{
		public static void Main(string[] args)
		{
            MainWindow window = new MainWindow(args);
            window.ShowDialog();
		}
	}
}
