using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;

namespace Project1
{
    static class Solver
    {
        private static float constraint_ks = 1000;
        private static float constraint_kd = 1;

        public const float Damp = 0.98f;
        private static Random rand = new Random();
        public static float GetRandom()
        {
            float result = Convert.ToSingle(((rand.Next() % 2000.0f) / 1000.0f) - 1.0f);
            return result;
        }

        public static void SimulationStep(List<Particle> particles, List<Force> forces, List<Constraint> constraints, float dt, int mode)
        {
            //Euler
            if(mode == 0)
            {
                eulerStep(particles, forces, constraints, dt);
            }
            // MidPoint
            else if(mode == 1)
            {
                midPointStep(particles, forces, constraints, dt);
            }
            // Runga kutta
            else
            {
                rungaKuttaStep(particles, forces, constraints, dt);
            }
        }

        

        private static void eulerStep(List<Particle> particles, List<Force> forces, List<Constraint> constraints, float dt)
        {
            clearForces(particles);
            forces.ForEach(f => f.Calculate());
            constraintForce(particles, constraints, constraint_ks, constraint_kd);

            resolveForces(particles, dt);
        }

        private static void midPointStep(List<Particle> particles, List<Force> forces, List<Constraint> constraints, float dt)
        {
            List<Particle> backupParticles = particles.ConvertAll(p => new Particle(p.Index, p.Position, p.Mass) {Velocity = p.Velocity, Force = new HyperPoint<float>(0,0)});
            eulerStep(particles, forces, constraints, dt / 2);

            clearForces(particles);
            forces.ForEach(f => f.Calculate());
            constraintForce(particles, constraints, constraint_ks, constraint_kd);

            foreach (Particle p in backupParticles)
            {
                particles[p.Index].Position = p.Position;
            }

            resolveForces(particles, dt);
        }

        private static void rungaKuttaStep(List<Particle> particles, List<Force> forces, List<Constraint> constraints, float dt)
        {
            List<Particle> backupParticles = particles.ConvertAll(p => new Particle(p.Index, p.Position, p.Mass) { Velocity = p.Velocity, Force = new HyperPoint<float>(0, 0) });
            //List<Particle> finalParticles = particles.ConvertAll(p => new Particle(p.Index, p.Position, p.Mass) { Velocity = p.Velocity, Force = new HyperPoint<float>(0, 0) });
            List<Particle> stepParticles = particles.ConvertAll(p => new Particle(p.Index, p.Position, p.Mass) { Velocity = p.Velocity, Force = new HyperPoint<float>(0, 0) });

            // k1
            clearForces(particles);
            forces.ForEach(f => f.Calculate());
            constraintForce(particles, constraints, constraint_ks, constraint_kd);

            foreach (Particle p in backupParticles)
            {
                p.Force += particles[p.Index].Force * (1 / 6f);
            }
            resolveForces(particles, dt/2);

            // k2
            clearForces(particles);
            forces.ForEach(f => f.Calculate());
            constraintForce(particles, constraints, constraint_ks, constraint_kd);

            foreach (Particle p in backupParticles)
            {
                p.Force += particles[p.Index].Force * (1 / 3f);
            }

            resolveForces(particles, dt/2);

            // k3
            clearForces(particles);
            forces.ForEach(f => f.Calculate());
            constraintForce(particles, constraints, constraint_ks, constraint_kd);

            foreach (Particle p in backupParticles)
            {
                p.Force += particles[p.Index].Force * (1 / 3f);
            }

            resolveForces(particles, dt);

            // k4
            clearForces(particles);
            forces.ForEach(f => f.Calculate());
            constraintForce(particles, constraints, constraint_ks, constraint_kd);

            foreach (Particle p in backupParticles)
            {
                p.Force += particles[p.Index].Force * (1 / 6f);
                particles[p.Index].Position = p.Position;
            }

            resolveForces(particles, dt);
        }

        static void clearForces(List<Particle> particles )
        {
            particles.ForEach(p => p.Force = new HyperPoint<float>(0, 0));
        }

        public static void resolveForces(List<Particle> particles, float dt)
        {
            foreach (Particle particle in particles)
            {
                HyperPoint<float> vel = new HyperPoint<float>(particle.Velocity);

                HyperPoint<float> acceleration = particle.Force / particle.Mass;
                particle.Velocity += acceleration * dt;
                particle.Position += particle.Velocity * dt;
            }
        }

        static void constraintForce(List<Particle> particles, List<Constraint> constraints, float ks, float kd)
        {
	        int nConstraint = constraints.Count;

	        Matrix<float> J = new Matrix<float>(nConstraint, particles.Count * 2);

	        for(int i = 0; i < nConstraint; i++)
            {
                List<Particle> c = constraints[i].GetDerivative();
                for (int j = 0; j < c.Count; j++)
                {
                    J[i, c[j].Index * 2] = c[j].Velocity.X;
                    J[i, c[j].Index * 2 + 1] = c[j].Velocity.Y;
                }
	        }

	        Matrix<float> JT = J.Transpose();

            Matrix<float> W = new Matrix<float>(particles.Count * 2, particles.Count * 2);

            for (int i = 0; i < W.Rows;  i++)
            {
                for (int j = 0; j < W.Columns; j++)
                    W[i, j] = 0.0f;
            }

            for (int i = 0; i < W.Rows; i++)
            {
                int index = (int)Math.Floor((float)i / 2);
		        W[i, i] = 1 / particles[index].Mass;
	        }
            
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
			        qdot[0, 2 * i] = particles[i].Velocity.X;
                    qdot[0, (i * 2) + 1] = particles[i].Velocity.Y;
	        }

	        Matrix<float> Q = new Matrix<float>(1, particles.Count * 2);
	        for (int i = 0; i < particles.Count; i++)
            {
			        Q[0, 2 * i] = particles[i].Force.X;
                    Q[0, (i * 2) + 1] = particles[i].Force.Y;
	        }

            Matrix<float> C = new Matrix<float>(nConstraint, 1);
	        for(int i=0; i<nConstraint; i++) {
	            C[i, 0] = constraints[i].GetC();
	        }

	        Matrix<float>  Cdot = new Matrix<float>(nConstraint, 1);
	        for(int i=0; i<nConstraint; i++) {
	            Cdot[i, 0] = constraints[i].GetCdot();
	        };
            
            //Solve Ax = b
            Matrix<float> A = J * W * J.Transpose();
            HyperPoint<float> lambda = new HyperPoint<float>(0, 0);
            Matrix<float> B = ((Jdot * -1) * (qdot.Transpose())).Add((J * W * Q.Transpose()) * -1);
            
	        B = B.Add(C * ks * -1);
	        B = B.Add(Cdot * kd * -1);
            
            HyperPoint<float> Bvec = new HyperPoint<float>(B.m);
            int steps = 100;
            LinearSolver.ConjGrad(nConstraint, A, Bvec, 1.0f / 10000.0f, ref steps, out lambda);
            Matrix<float> lambdaM = new Matrix<float>(lambda.p.Count(), 1, lambda.p);
            Matrix<float> force = lambdaM.Transpose() * J;

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Force += new HyperPoint<float>(force[0, i * 2], force[0, (i * 2) + 1]);
            }
        }
    }
}
