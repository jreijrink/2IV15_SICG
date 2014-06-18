using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace FluidsProject.Particles
{
    class FixedConstraint : Constraint
    {
        private readonly Particle _p;
        private readonly HyperPoint<float> _fixedPosition;

        public FixedConstraint(Particle p, HyperPoint<float> fixedPosition)
		{
			_p = p;
            _fixedPosition = fixedPosition;
		}

        public override void Draw()
		{
		}
       
        public override float GetC()
        {
            return (_p.Position.X - _fixedPosition.X) * (_p.Position.X - _fixedPosition.X) +
                    (_p.Position.Y - _fixedPosition.Y) * (_p.Position.Y - _fixedPosition.Y);
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
                2 * (_p.Position.X - _fixedPosition.X),
                2 * (_p.Position.Y - _fixedPosition.Y)
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
