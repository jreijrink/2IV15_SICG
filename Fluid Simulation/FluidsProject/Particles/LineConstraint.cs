using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace FluidsProject.Particles
{
    class LineConstraint : Constraint
    {
        private readonly Particle _p;
        private readonly HyperPoint<float> _p1, _p2;
        private readonly bool isHorizontal;

        public LineConstraint(Particle p, HyperPoint<float> p1, HyperPoint<float> p2)
		{
            if (p1.Y == p2.Y)
                isHorizontal = true;
            else if (p1.X == p2.X)
                isHorizontal = false;
            else
                throw new ArgumentException("Line is not straight");

            _p = p;
            _p1 = p1;
            _p2 = p2;
		}

        public override void Draw()
		{
            GL.Begin(BeginMode.LineLoop);
            GL.Color3(0f, 1f, 0f);
			GL.Vertex2(_p1.X, _p1.Y);
            GL.Vertex2(_p2.X, _p2.Y);
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
            return (isHorizontal) ? _p.Position.Y - _p1.Y : _p.Position.X - _p1.X;
        }

        public override float GetCdot()
        {
            return (isHorizontal) ? _p.Velocity.Y : _p.Velocity.X;
        }

        public override List<Particle> GetDerivative()
        {
            List<Particle> derivatives = new List<Particle>();

            HyperPoint<float> derivative;
            if(isHorizontal)
                derivative = new HyperPoint<float>(0, 1);
            else
                derivative = new HyperPoint<float>(1, 0);
            
            Particle p1 = new Particle(_p.Index);
            p1.Velocity = derivative;
            derivatives.Add(p1);
            
            return derivatives;
        }

        public override List<Particle> GetTimeDerivative()
        {
            List<Particle> timeDerivatives = new List<Particle>();
            HyperPoint<float> derivative;
            if (isHorizontal)
                derivative = new HyperPoint<float>(0, 0);
            else
                derivative = new HyperPoint<float>(0, 0);

            Particle p1 = new Particle(_p.Index);
            p1.Velocity = derivative;
            timeDerivatives.Add(p1);
            
            return timeDerivatives;
        }
	}
}
