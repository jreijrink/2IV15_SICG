using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using micfort.GHL.Math2;

namespace FluidsProject
{
    class SquareObject : MovingObject
    {
        private float _x;
        private float _y;
        private int _height_half;
        private int _width_half;
        private float _u;
        private float _v;
        private int _N;

        public SquareObject(float x, float y, int height, int width, int N)
        {
            _x = x;
            _y = y;
            _height_half = height / 2;
            _width_half = width / 2;
            _N = N;

            //_u = -0.4f;
            _v = -0.5f;
        }

        public override void Draw()
        {
            GL.LineWidth(1.0f);

            GL.Begin(BeginMode.Quads);

            float h = 1.0f / _N;
            float x1 = (_x - _width_half - 0.5f) * h;
            float x2 = (_x + _width_half + 0.5f) * h;
            float y1 = (_y - _height_half - 0.5f) * h;
            float y2 = (_y + _height_half + 0.5f) * h;

            GL.Color3(1, 1, 0.6f); GL.Vertex2(x1, y1);
            GL.Color3(1, 1, 0.1f); GL.Vertex2(x2, y1);
            GL.Color3(1, 1, 0.1f); GL.Vertex2(x2, y2);
            GL.Color3(1, 1, 0.6f); GL.Vertex2(x1, y2);

            GL.End();
        }

        public override void UpdatePosition()
        {
            _x += _u;
            _y += _v;

            float h = 1.0f / _N;
            float x1 = (_x - _width_half - 0.5f) * h;
            float x2 = (_x + _width_half + 0.5f) * h;
            float y1 = (_y - _height_half - 0.5f) * h;
            float y2 = (_y + _height_half + 0.5f) * h;

            if (x2 > 1)
            {
                _x = _N - _width_half - 0.5f;
                _u *= -1;
            }
            if (x1 < 0)
            {
                _x = _width_half + 0.5f;
                _u *= -1;
            }

            if (y2 > 1)
            {
                _y = _N - _height_half - 0.5f;
                _v *= -1;
            }
            if (y1 < 0)
            {
                _y = _height_half + 0.5f;
                _v *= -1;
            }
        }

        public override bool IsObjectCell(int x, int y)
        {
            /*
            if (x == _x + _width_half || x == _x - _width_half ||
                y == _y + _height_half || y == _y - _height_half)
                return true;
            */

            if (x < _x + _width_half &&
                x > _x - _width_half &&
                y < _y + _height_half &&
                y > _y - _height_half)
                return true;

            return false;
        }

        public override void SetVelocity(float u, float v)
        {
            _u = u * 0.01f;
            _v = v * 0.01f;
        }
        public override void SetPosition(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public override float GetVelocityX(int x, int y, float[] u)
        {
            if (x == Math.Floor(_x) + _width_half)
            {
                return -u[Solver.IX(x + 1, y)] + (_u > 0 ? _u : 0);
            }

            if (x == Math.Ceiling(_x) - _width_half)
            {
                return -u[Solver.IX(x - 1, y)] + (_u > 0 ? 0 : _u);
            }

            return 0;
        }

        public override float GetVelocityY(int x, int y, float[] v)
        {
            if (y == Math.Floor(_y) + _height_half)
            {
                return -v[Solver.IX(x, y + 1)] + (_v > 0 ? _v : 0);
            }

            if (y == Math.Ceiling(_y) - _height_half)
            {
                return -v[Solver.IX(x, y - 1)] + (_v > 0 ? 0 : _v);
            }

            return 0;
        }

        public override float GetVelocityDensity(int x, int y, float[] d)
        {
            if (x == Math.Floor(_x) + _width_half)
            {
                return d[Solver.IX(x + 1, y)];
            }

            if (x == Math.Ceiling(_x) - _width_half)
            {
                return d[Solver.IX(x - 1, y)];
            }

            if (y == Math.Floor(_y) + _height_half)
            {
                return d[Solver.IX(x, y + 1)];
            }

            if (y == Math.Ceiling(_y) - _height_half)
            {
                return d[Solver.IX(x, y - 1)];
            }

            return 0;// d[Solver.IX(x, y)];
        }

        public override bool pointInObject(HyperPoint<float> point)
        {
            float h = 1.0f / _N;
            float x1 = (_x - _width_half - 0.5f) * h;
            float x2 = (_x + _width_half + 0.5f) * h;
            float y1 = (_y - _height_half - 0.5f) * h;
            float y2 = (_y + _height_half + 0.5f) * h;

            if(point.X >= x1 && point.X <= x2 && point.Y >= y1 && point.Y <= y2)
            {
                return true;
            }
            return false;
        }

        public override List<HyperPoint<float>> GetVertices()
        {
            float h = 1.0f / _N;
            float x1 = (_x - _width_half - 0.5f) * h;
            float x2 = (_x + _width_half + 0.5f) * h;
            float y1 = (_y - _height_half - 0.5f) * h;
            float y2 = (_y + _height_half + 0.5f) * h;

            return new List<HyperPoint<float>>(new[] {new HyperPoint<float>(x1, y1),
                                           new HyperPoint<float>(x2, y1),
                                           new HyperPoint<float>(x2, y2),
                                           new HyperPoint<float>(x1, y2)
                                          });
        }
    }
}
