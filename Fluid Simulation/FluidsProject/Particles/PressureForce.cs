using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace FluidsProject.Particles
{
	class PressureForce : Force
	{
		private readonly Particle _p1;
        private int _N;
        private float[] _d, _u, _v;
        public float _dt;
        private float factor = 5.0f;

        public PressureForce(Particle p1, float dt, int N, float[] d, float[] u, float[] v)
		{
			_p1 = p1;
            _N = N;
            _d = d;
            _u = u;
            _v = v;
            _dt = dt;
		}

        public override void Draw()
        {
		}

        public override void Calculate()
        {
            int i0, j0, i1, j1;
            float s0, t0, s1, t1, dt0;

            dt0 = _dt * _N;

            float x = (float)(_p1.Position.X * _N);
            float y = (float)(_p1.Position.Y * _N);

            if (x < 0.5f) x = 0.5f;
            if (x > _N + 0.5f) x = _N + 0.5f;
            i0 = (int)x;
            i1 = i0 + 1;
            if (y < 0.5f) y = 0.5f;
            if (y > _N + 0.5f) y = _N + 0.5f;
            j0 = (int)y;
            j1 = j0 + 1;
            s1 = x - i0;
            s0 = 1 - s1;
            t1 = y - j0;
            t0 = 1 - t1;

            float density = s0 * (t0 * _d[Game.IX(i0, j0)] + t1 * _d[Game.IX(i0, j1)]) + s1 * (t0 * _d[Game.IX(i1, j0)] + t1 * _d[Game.IX(i1, j1)]);
            float v_x = s0 * (t0 * _u[Game.IX(i0, j0)] + t1 * _u[Game.IX(i0, j1)]) + s1 * (t0 * _u[Game.IX(i1, j0)] + t1 * _u[Game.IX(i1, j1)]);
            float v_y = s0 * (t0 * _v[Game.IX(i0, j0)] + t1 * _v[Game.IX(i0, j1)]) + s1 * (t0 * _v[Game.IX(i1, j0)] + t1 * _v[Game.IX(i1, j1)]);

            _p1.Force += new HyperPoint<float>(v_x * factor, v_y * factor) * density;
        }
	}
}
