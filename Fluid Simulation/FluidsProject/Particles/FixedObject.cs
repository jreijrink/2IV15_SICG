using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace FluidsProject.Particles
{
    abstract class FixedObject
    {
        abstract public void Draw();

        abstract public bool HasCollision(Particle p);

        abstract public void SolveCollision(Particle p);
	}
}
