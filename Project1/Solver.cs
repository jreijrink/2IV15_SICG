using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;

namespace Project1
{
	static class Solver
	{
		public const float Damp = 0.98f;
		private static Random rand = new Random();
		public static float GetRandom()
		{
			float result = Convert.ToSingle(((rand.Next()%2000.0f)/1000.0f) - 1.0f);
			return result;
		}

		public static void SimulationStep(List<Particle> particles, List<Force> forces, List<Constraint> constraints, float dt)
        {
            foreach (Particle particle in particles)
            {
                particle.Force = new HyperPoint<float>(0, 0);
            }

            forces.ForEach(f => f.Calculate());

            constraints.ForEach(f => f.Calculate());

            foreach(Particle particle in particles)
			{
                HyperPoint<float> acceleration = particle.Force / particle.Mass;
                particle.Velocity += acceleration * dt;
                particle.Position += particle.Velocity * dt;
			}
		}
	}
}
