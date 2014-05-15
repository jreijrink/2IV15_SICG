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

	        Matrix<double> J = new Matrix<double>(nConstraint, particles.Count * 2);

	        for(int i = 0; i < nConstraint; i++)
            {
                List<Particle> c = constraints[i].GetDerivative();
                for (int j = 0; j < c.Count; j++)
                {
                    J[i, c[j].Index * 2] = c[j].Velocity.X;
                    J[i, c[j].Index * 2 + 1] = c[j].Velocity.X;
                }
	        }
	        Matrix<double> JT = J.Transpose();

	        Matrix<double> M = new Matrix<double>(particles.Count * 2, particles.Count * 2);

            for (int i = 0; i < M.Rows;  i++)
            {
                for (int j = 0; j < M.Columns; j++)
                    M[i, j] = 0.0;
            }

            for (int i = 0; i < M.Rows; i++)
            {
                int index = (int)Math.Floor((double)i / 3);
		        M[i, i] = particles[index].Mass;
	        }
            Matrix<double> W = M.Inverse();
            
	        Matrix<double> Jdot = new Matrix<double>(nConstraint, particles.Count * 2);
	        for (int i = 0; i < nConstraint; i++)
            {
	          List<Particle> c = constraints[i].GetTimeDerivative();

	          for (int j = 0; j < c.Count; j++)
              {
		          Jdot[i, c[j].Index * 2] = c[j].Velocity.X;
		          Jdot[i, c[j].Index * 2 + 1] = c[j].Velocity.Y;
	          }
	        }

	        Matrix<double> qdot = new Matrix<double>(1, particles.Count * 2);
	        for (int i = 0; i < particles.Count; i++)
            {
			        qdot[0, 2*i] = particles[i].Velocity.X;
			        qdot[0, 2*i+1] = particles[i].Velocity.Y;
	        }

	        Matrix<double> Q = new Matrix<double>(1, particles.Count * 2);
	        for (int i = 0; i < particles.Count; i++)
            {
			        Q[0, 2*i] = particles[i].Force.X;
			        Q[0, 2*i+1] = particles[i].Force.Y;
	        }

            Matrix<double> step1 = J * W;
            Matrix<double> step2 = J.Transpose();
            Matrix<double> step3 = Jdot * -1;
            Matrix<double> step4 = qdot.Transpose();
            Matrix<double> step5 = Q.Transpose();
            Matrix<double> step6 = step1 * step5;
            Matrix<double> step7 = step1 * step2;
            Matrix<double> step8 = step3 * step4;
            Matrix<double> step9 = step6 * -1;
            Matrix<double> step10 = step8.Add(step9).Transpose();
            Matrix<double> step11 = step7.Inverse();

            Matrix<double> labda = step11 * step10;
            Matrix<double> force = labda * J;
            
            /*
	        //--------------------JW.Q-------------------------------------------
	        Matrix<double> JWQ;
	        JWQ = Matrix::multiply(JW, Q);

	        //--------------------Correction damping: -------------------------------------------
	        Matrix<double> C(nConstraint, vector<double>(1));
	        for(int i=0; i<nConstraint; i++) {
	          C[i][0] = allConstraints[i]->getC();
	        }

	        Matrix<double> Cdot(nConstraint, vector<double>(1));
	        for(int i=0; i<nConstraint; i++) {
	          Cdot[i][0] = allConstraints[i]->getCdot();
	        };

	        //--------------------b=(-Jdotqdot-JWQ-ksC-kdCdot)-------------------------------------------
	        Matrix<double> b = Matrix::subtract(Matrix::mulconst(Jdotqdot,-1), JWQ);
	        b = Matrix::subtract(b, Matrix::mulconst(C, ks));
	        b = Matrix::subtract(b, Matrix::mulconst(Cdot, kd));

	        //--------------------Solve: Ax=b -------------------------------------------
	        implicitMatrix *M = new implicitMatrix(A);
	        double t1[nConstraint], t2[nConstraint];

	        //There should be an elegant way to convert vector to array? :)
	        for (unsigned int i=0; i<b.size(); i++) t2[i] = b[i][0];

	        int steps = 100;
	        ConjGrad(nConstraint, M, t1, t2, 1.0e-5, &steps);
	        delete M;

	        //-------------------- JT.lamda -------------------------------------------
	        //Compute correction matrix:
	        Matrix<double> lambda(nConstraint, vector<double>(1));
	        for (int i=0; i<nConstraint; i++) lambda[i][0] = t1[i];

	        Matrix<double> corrc = Matrix::multiply(JT,lambda);


	        //--------------------------------------------------------------------------
	        //Apply correction forces to the force accumulator:
	        for (unsigned int i=0; i<pVector.size(); i++) {
		        pVector[i]->setForce(pVector[i]->getForce()+Vec2f(corrc[2*i][0], corrc[2*i+1][0]));
	        }
            */
        }
    }
}
