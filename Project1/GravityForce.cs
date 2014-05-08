using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace Project1
{
	class GravityForce : Force
	{
		private readonly Particle _p1;
        private const float _gravity = -9.81f;

        public GravityForce(Particle p1)
		{
			_p1 = p1;
		}

        public override void Draw()
        {

		}

        public override void Calculate()
        {
            _p1.Force += new HyperPoint<float>(0, _p1.Mass * _gravity);
        }
	}
}
