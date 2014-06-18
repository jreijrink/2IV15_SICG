using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace FluidsProject.Particles
{
	abstract class Force
    {
        abstract public void Calculate();

        abstract public void Draw();
	}
}
