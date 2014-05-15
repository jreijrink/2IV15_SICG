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
            float result = Convert.ToSingle(((rand.Next() % 2000.0f) / 1000.0f) - 1.0f);
            return result;
        }

        public static void SimulationStep(List<Particle> particles, List<Force> forces, List<Constraint> constraints, float dt)
        {
            foreach (Particle particle in particles)
            {
                particle.Force = new HyperPoint<float>(0, 0);
            }

            forces.ForEach(f => f.Calculate());
            
            //constraints.ForEach(f => f.GetCdot());
            constraintForce(particles, constraints);

            foreach (Particle particle in particles)
            {
                HyperPoint<float> acceleration = particle.Force / particle.Mass;
                particle.Velocity += acceleration * dt;
                particle.Position += particle.Velocity * dt;
            }
        }
        
        static void constraintForce(List<Particle> particles, List<Constraint> constraints)
        {
	        int nConstraint = constraints.Count;

	        Matrix<float> J = new Matrix<float>(nConstraint, particles.Count * 2);

	        for(int i = 0; i < nConstraint; i++)
            {
                List<Particle> c = constraints[i].GetDerivative();
                for (int j = 0; j < c.Count; j++)
                {
                    J[i, c[j].Index * 2] = c[j].Velocity.X;
                    J[i, c[j].Index * 2 + 1] = c[j].Velocity.X;
                }
	        }
	        Matrix<float> JT = J.Transpose();

	        Matrix<float> M = new Matrix<float>(particles.Count * 2, particles.Count * 2);

            for (int i = 0; i < M.Rows;  i++)
            {
                for (int j = 0; j < M.Columns; j++)
                    M[i, j] = 0.0f;
            }

            for (int i = 0; i < M.Rows; i++)
            {
                int index = (int)Math.Floor((float)i / 3);
		        M[i, i] = particles[index].Mass;
	        }
            Matrix<float> W = M.Inverse();
            
	        Matrix<float> Jdot = new Matrix<float>(nConstraint, particles.Count * 2);
	        for (int i = 0; i < nConstraint; i++)
            {
	          List<Particle> c = constraints[i].GetTimeDerivative();

	          for (int j = 0; j < c.Count; j++)
              {
		          Jdot[i, c[j].Index * 2] = c[j].Velocity.X;
		          Jdot[i, c[j].Index * 2 + 1] = c[j].Velocity.Y;
	          }
	        }

	        Matrix<float> qdot = new Matrix<float>(1, particles.Count * 2);
	        for (int i = 0; i < particles.Count; i++)
            {
			        qdot[0, 2*i] = particles[i].Velocity.X;
			        qdot[0, 2*i+1] = particles[i].Velocity.Y;
	        }

	        Matrix<float> Q = new Matrix<float>(1, particles.Count * 2);
	        for (int i = 0; i < particles.Count; i++)
            {
			        Q[0, 2*i] = particles[i].Force.X;
			        Q[0, 2*i+1] = particles[i].Force.Y;
	        }

            /*
            Matrix<float> step1 = J * W;
            Matrix<float> step2 = J.Transpose();
            Matrix<float> step3 = Jdot * -1;
            Matrix<float> step4 = qdot.Transpose();
            Matrix<float> step5 = Q.Transpose();
            Matrix<float> step6 = step1 * step5;
            Matrix<float> step7 = step1 * step2;
            Matrix<float> step8 = step3 * step4;
            Matrix<float> step9 = step6 * -1;
            Matrix<float> step11 = step7.Inverse();

            Matrix<float> labda = step11 * step10;
            Matrix<float> force = labda.Transpose() * J;
            */
            
            //Solve Ax = b
            Matrix<float> A = J * W * J.Transpose();
            HyperPoint<float> labda = new HyperPoint<float>(0, 0);
            Matrix<float> B = ((Jdot * -1) * (qdot.Transpose())).Add((J * W * Q.Transpose()) * -1);
            HyperPoint<float> Bvec = new HyperPoint<float>(B.m);
            int steps = 100;
            double test = LinearSolver.ConjGrad(nConstraint, A, Bvec, 1.0f / 10000.0f, ref steps, out labda);
            Matrix<float> labdaM = new Matrix<float>(labda.p.Count(), 1, labda.p);
            Matrix<float> force = labdaM.Transpose() * J;

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Force += new HyperPoint<float>(force[0, i * 2], force[0, i * 2 + 1]);
            }
        }
    }
}
