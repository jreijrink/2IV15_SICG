using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace FluidsProject.Particles
{
    abstract class Constraint
    {
        abstract public void Draw();

        abstract public float GetC();

        abstract public float GetCdot();

        abstract public List<Particle> GetDerivative();

        abstract public List<Particle> GetTimeDerivative();
	}
}
