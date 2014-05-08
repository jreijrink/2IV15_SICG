using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace Project1
{
	class SpringForce : Force
	{
		private readonly Particle _p1;
		private readonly Particle _p2;
        private readonly float _dist;
        private readonly float _ks;
        private readonly float _kd;

        public SpringForce(Particle p1, Particle p2, float dist, float ks, float kd)
		{
			_p1 = p1;
			_p2 = p2;
			_dist = dist;
			_ks = ks;
			_kd = kd;
		}

        public override void Draw()
        {
            GL.Begin(BeginMode.Lines);
            GL.Color3(0.8f, 0.7f, 0.6f);
            GL.Vertex2(_p1.Position[0], _p1.Position[1]);
            GL.Color3(0.8f, 0.7f, 0.6f);
            GL.Vertex2(_p2.Position[0], _p2.Position[1]);
            GL.End();
        }

        public override void Calculate()
        {
            HyperPoint<float> pos_diff = _p1.Position - _p2.Position;
            HyperPoint<float> vel_diff = _p1.Velocity - _p2.Velocity;
            float pos_diff_length = pos_diff.GetLength();

            float x = _ks * (pos_diff_length - _dist);
            float y = _kd * (vel_diff.DotProduct(pos_diff) / pos_diff_length);
            float result = x + y;

            HyperPoint<float> force = (pos_diff / pos_diff_length) * result;

            _p1.Force -= force;
            _p2.Force += force;
        }
	}
}
