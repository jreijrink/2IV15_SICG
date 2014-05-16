using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace Project1
{
    class RodConstraint : Constraint
	{
		private readonly Particle _p1;
		private readonly Particle _p2;
		private readonly float _dist;

		public RodConstraint(Particle p1, Particle p2, float dist)
		{
			_p1 = p1;
			_p2 = p2;
			_dist = dist;
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

        /*
        public override void Calculate()
        {
            float distance = (float)Math.Pow((_p1.Position.X - _p2.Position.X), 2) + (float)Math.Pow((_p1.Position.Y - _p2.Position.Y), 2) - (float)Math.Pow(_dist, 2);

            float force = -10 * (_p1.Position.X - _p2.Position.X) - 1 * (_p1.Velocity.Y - _p2.Velocity.Y);
            _p1.Force += new HyperPoint<float>(force, 0);
            _p2.Force -= new HyperPoint<float>(force, 0);
        }
        */

        public override float GetC()
        {
            return ((_p1.Position.X - _p2.Position.X) * (_p1.Position.X - _p2.Position.X)) +
                    ((_p1.Position.Y - _p2.Position.Y) * (_p1.Position.Y - _p2.Position.Y)) -
                    (_dist * _dist);
        }

        public override float GetCdot()
        {
            List<Particle> derivatives = GetDerivative();

            Matrix<float> velocity1Transpose = new Matrix<float>(1, 2);
            velocity1Transpose[0, 0] = _p1.Velocity.X;
            velocity1Transpose[0, 1] = _p1.Velocity.Y;
            Matrix<float> cdot1 = velocity1Transpose * derivatives[0].Velocity;

            Matrix<float> velocity2Transpose = new Matrix<float>(1, 2);
            velocity2Transpose[0, 0] = _p2.Velocity.X;
            velocity2Transpose[0, 1] = _p2.Velocity.Y;
            Matrix<float> cdot2 = velocity2Transpose * derivatives[1].Velocity;
            
            return cdot1[0, 0] + cdot2[0, 0];
        }

        public override List<Particle> GetDerivative()
        {
            List<Particle> derivatives = new List<Particle>();

            HyperPoint<float> derivativeX1 = new HyperPoint<float>(
                2 * (_p1.Position.X - _p2.Position.X),
                2 * (_p1.Position.Y - _p2.Position.Y)
            );
            Particle p1 = new Particle(_p1.Index);
            p1.Velocity = derivativeX1;
            derivatives.Add(p1);

            HyperPoint<float> derivativeX2 = new HyperPoint<float>(
                -2 * (_p1.Position.X - _p2.Position.X),
                -2 * (_p1.Position.Y - _p2.Position.Y)
            );
            Particle p2 = new Particle(_p2.Index);
            p2.Velocity = derivativeX2;
            derivatives.Add(p2);

            return derivatives;
        }

        public override List<Particle> GetTimeDerivative()
        {
            List<Particle> timeDerivatives = new List<Particle>();
            HyperPoint<float> timeDerivativeX1 = new HyperPoint<float>(
                2 * (_p1.Velocity.X - _p2.Velocity.X),
                2 * (_p1.Velocity.Y - _p2.Velocity.Y)
            );
            Particle p1 = new Particle(_p1.Index);
            p1.Velocity = timeDerivativeX1;
            timeDerivatives.Add(p1);

            HyperPoint<float> timeDerivativeX2 = new HyperPoint<float>(
                -2 * (_p1.Velocity.X - _p2.Velocity.X),
                -2 * (_p1.Velocity.Y - _p2.Velocity.Y)
            );
            Particle p2 = new Particle(_p2.Index);
            p2.Velocity = timeDerivativeX2;
            timeDerivatives.Add(p2);

            return timeDerivatives;
        }
	}
}
