using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace Project1
{
	class VerLineObject : FixedObject
    {
        private readonly float _x;
        private readonly float _y;
        private readonly float _height;
        private readonly float _kr = 1.0f / 100.0f;
        private readonly bool _left_side;

        public VerLineObject(float x, float y, float height, bool left_side)
		{
            _x = x;
            _y = y;
            _height = height;
            _left_side = left_side;
		}

        public override void Draw()
        {
            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0f, 0.1f, 0.1f);
            GL.Vertex2(_x, _y);
            GL.Color3(1.0f, 0.1f, 0.1f);
            GL.Vertex2(_x, _y + _height);
            GL.End();
        }

        public override bool HasCollision(Particle p)
        {
            float distance = p.Position.X - _x;

            if ((_left_side && distance < 0) || (!_left_side  && distance > 0))
                return true;
            else
            return false;
        }

        public override void SolveCollision(Particle p)
        {
            float v_t = p.Velocity.Y;
            float v_n = p.Velocity.X;

            p.Position = new HyperPoint<float>(_x, p.Position.Y);
            p.Velocity = new HyperPoint<float>(-v_n * _kr, v_t);
        }
	}
}
