using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace Project1
{
	class HorLineObject : FixedObject
    {
        private readonly float _x;
        private readonly float _y;
        private readonly float _width;
        private readonly float _kr = 1.0f / 100.0f;
        private readonly bool _top_side;

        public HorLineObject(float x, float y, float width, bool top_side)
		{
            _x = x;
            _y = y;
            _width = width;
            _top_side = top_side;
		}

        public override void Draw()
        {
            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0f, 0.1f, 0.1f);
            GL.Vertex2(_x, _y);
            GL.Color3(1.0f, 0.1f, 0.1f);
            GL.Vertex2(_x + _width, _y);
            GL.End();
        }

        public override bool HasCollision(Particle p)
        {
            float distance = p.Position.Y - _y;

            if ((_top_side && distance < 0) || (!_top_side && distance > 0))
                return true;
            else
            return false;
        }

        public override void SolveCollision(Particle p)
        {
            float v_t = p.Velocity.Y;
            float v_n = p.Velocity.X;

            p.Position = new HyperPoint<float>(p.Position.X, _y);
            p.Velocity = new HyperPoint<float>(-v_n, v_t * _kr);
        }
	}
}
