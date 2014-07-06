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
        private float factor = 30.0f;
        private BoundryConditions[] _settings;

        private float damp = 0.8f;

        public PressureForce(Particle p1, int N, float[] d, float[] u, float[] v, BoundryConditions[] settings = null)
        {
            _p1 = p1;
            _N = N;
            _d = d;
            _u = u;
            _v = v;
            _settings = settings;
        }

        public override void Draw()
        {
        }

        public override void Calculate()
        {
            int i0, j0, i1, j1;
            float s0, t0, s1, t1;

            float x = _p1.Position.X * _N;
            float y = _p1.Position.Y * _N;

            if (x < 0.5f) x = 0.5f;
            if (x > _N + 0.5f) x = _N + 0.5f;
            if (y < 0.5f) y = 0.5f;
            if (y > _N + 0.5f) y = _N + 0.5f;

            i0 = (int)x;
            j0 = (int)y;

            Source source;
            if (_settings != null)
            {
                source = _settings[Game.IX(i0, j0)].source;
            }
            else
            {
                source = Source.None;
            }

            if (source == Source.up && j0 != 0)
            {

                i1 = i0 + 1;
                j0 = j0 - 2;
                j1 = j0 + 1;
            }
            else if(source == Source.down && j0 != _N)
            {
                i1 = i0 + 1;
                j0 = j0 + 1;
                j1 = j0 + 1;
            }
            else
            {
                i1 = i0 + 1;
                j1 = j0 + 1;
            }

            s1 = x - 1 - i0;
            s0 = 1 - s1;
            t1 = y - 1 - j0;
            t0 = 1 - t1;

            if ((float)Math.Max(i0, Math.Max(i1, Math.Max(j0, j1))) <= _N && (float)Math.Min(i0, Math.Min(i1, Math.Min(j0, j1))) >= 0)
            {
                float density = s0 * (t0 * _d[Game.IX(i0, j0)] + t1 * _d[Game.IX(i0, j1)]) + s1 * (t0 * _d[Game.IX(i1, j0)] + t1 * _d[Game.IX(i1, j1)]);
                float v_x = s0 * (t0 * _u[Game.IX(i0, j0)] + t1 * _u[Game.IX(i0, j1)]) + s1 * (t0 * _u[Game.IX(i1, j0)] + t1 * _u[Game.IX(i1, j1)]);
                float v_y = s0 * (t0 * _v[Game.IX(i0, j0)] + t1 * _v[Game.IX(i0, j1)]) + s1 * (t0 * _v[Game.IX(i1, j0)] + t1 * _v[Game.IX(i1, j1)]);
                _p1.Force += new HyperPoint<float>(v_x * factor, v_y * factor) * Math.Min(1, density) * damp;
            }
        }
    }
}
