using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace Project1
{
	class HorizontalForce : Force
	{
		private readonly Particle _p1;
        private readonly bool _right;
        
        private const float _force = -0.2f;

        public HorizontalForce(Particle p1, bool right)
		{
			_p1 = p1;
            _right = right;
		}

        public override void Draw()
        {
            GL.Begin(BeginMode.Lines);
            GL.Color3(0.1f, 0.1f, 1.0f);
            GL.Vertex2(_p1.Position[0], _p1.Position[1]);
            GL.Color3(0.1f, 0.1f, 1.0f);
            if(_right)
                GL.Vertex2(_p1.Position[0] - _force, _p1.Position[1]);
            else
                GL.Vertex2(_p1.Position[0] + _force, _p1.Position[1]);
            GL.End();
		}

        public override void Calculate()
        {
            if (_right)
                _p1.Force -= new HyperPoint<float>(_p1.Mass * _force, 0);
            else
                _p1.Force += new HyperPoint<float>(_p1.Mass * _force, 0);
        }
	}
}
