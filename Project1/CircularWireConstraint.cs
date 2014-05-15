using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace Project1
{
    class CircularWireConstraint : Constraint
    {
        private readonly Particle _p;
		private readonly HyperPoint<float> _center;
		private readonly float _radius;

		public CircularWireConstraint(Particle p, HyperPoint<float> center, float radius)
		{
			_p = p;
			_center = center;
			_radius = radius;
		}

        public override void Draw()
		{
			GL.Begin(BeginMode.LineLoop);
			GL.Color3(0f, 1f, 0f);
			for (int i = 0; i < 360; i = i + 18)
			{
				float degInRad = i * Convert.ToSingle(Math.PI) / 180;
				GL.Vertex2(_center[0] + Math.Cos(degInRad) * _radius, _center[1] + Math.Sin(degInRad) * _radius);
			}
			GL.End();
		}
       
        /*
        public override void Calculate()
        {
            HyperPoint<float> normal = _p.Position - _center;
            float labda = (-_p.Force.DotProduct(normal) - _p.Mass * _p.Velocity.DotProduct(_p.Velocity)) / (normal.DotProduct(normal));
            HyperPoint<float> force_c = normal * labda;
            _p.Force += force_c;


        }
        */


        public override float GetC()
        {
            return (_p.Position.X - _center.X) * (_p.Position.X - _center.X) +
                    (_p.Position.Y - _center.Y) * (_p.Position.Y - _center.Y) -
                    _radius * _radius;
        }

        public override float GetCdot()
        {
            List<Particle> derivatives = GetDerivative();

            Matrix<float> velocity1Transpose = new Matrix<float>(1, 2);
            velocity1Transpose[0, 0] = _p.Velocity.X;
            velocity1Transpose[0, 1] = _p.Velocity.Y;
            Matrix<float> cdot1 = velocity1Transpose * derivatives[0].Velocity;

            return cdot1[0, 0];
        }

        public override List<Particle> GetDerivative()
        {
            List<Particle> derivatives = new List<Particle>();

            HyperPoint<float> derivativeX1 = new HyperPoint<float>(
                2 * (_p.Position.X - _center.X),
                2 * (_p.Position.Y - _center.Y)
            );
            Particle p1 = new Particle(_p.Index);
            p1.Velocity = derivativeX1;
            derivatives.Add(p1);
            
            return derivatives;
        }

        public override List<Particle> GetTimeDerivative()
        {
            List<Particle> timeDerivatives = new List<Particle>();
            HyperPoint<float> timeDerivativeX1 = new HyperPoint<float>(
                2 * (_p.Velocity.X),
                2 * (_p.Velocity.Y)
            );
            Particle p1 = new Particle(_p.Index);
            p1.Velocity = timeDerivativeX1;
            timeDerivatives.Add(p1);
            
            return timeDerivatives;
        }
	}
}
