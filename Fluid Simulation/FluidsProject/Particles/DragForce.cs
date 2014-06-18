using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace FluidsProject.Particles
{
    class DragForce : Force
	{
		private readonly Particle _p1;
        private float drag = 0.5f;

        public DragForce(Particle p1)
		{
			_p1 = p1;
		}

        public override void Draw()
        {
		}

        public override void Calculate()
        {
            _p1.Force -= drag * _p1.Velocity;
        }
	}
}
