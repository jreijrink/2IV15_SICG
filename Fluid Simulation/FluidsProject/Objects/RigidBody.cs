using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluidsProject.Particles;
using OpenTK.Graphics.OpenGL;
using micfort.GHL.Math2;

namespace FluidsProject.Objects
{
    abstract class RigidBody
    {
        // Constant quantities
        protected float mass;
        protected float inertia;
        
        // State Variables
        protected HyperPoint<float> x;
        private HyperPoint<float> x_orig;
        protected HyperPoint<float> v = new HyperPoint<float>(0, 0);

        protected float orientation;
        protected float rotv;
        protected float torque;
        protected HyperPoint<float> force = new HyperPoint<float>(0, 0);

        protected List<Particle> vertices = new List<Particle>();
        protected List<HyperPoint<float>> localVertices = new List<HyperPoint<float>>();
        
        public RigidBody(HyperPoint<float> x, float mass)
        {
            this.mass = mass;
            this.x = x;
            this.x_orig = x;
        }

        public void addForce(HyperPoint<float> force, HyperPoint<float> location)
        {
            this.force += force;
            this.torque +=  cross(location - x, force) / inertia;
        }

        private float cross(HyperPoint<float> a, HyperPoint<float> b)
        {
            float i =  a.X*b.Y;
            float j = a.Y*b.X;

            return i - j;
        }

        public void update(float dt, int N, float[] d, float[] u, float[] v)
        {
            foreach (Particle particle in vertices)
            {
                PressureForce pf = new PressureForce(particle, dt, N, d, u, v);
                DragForce df = new DragForce(particle);

                pf.Calculate();
                df.Calculate();

                addForce(particle.Force, particle.Position);
                particle.Force = new HyperPoint<float>(0, 0);
            }

            this.v += ((force / mass) * dt);
            float old_y = this.v.Y;
            float old_x = this.v.X;
            foreach (Particle particle in vertices)
            {
                int i = (int)(particle.Position.X * N);
                int j = (int)(particle.Position.Y * N);

                if (i >= 0 && i <= N && j >= 0 && j <= N)
                {
                    //v[Game.IX(i, j)] = d[Game.IX(i, j)] * this.v.X;
                    //u[Game.IX(i, j)] = d[Game.IX(i, j)] * this.v.Y;
                    //u[Game.IX(i, j)] = d[Game.IX(i, j)] * (10 * this.v.X);//-u[Game.IX(i + 1, j)] + ;
                    //v[Game.IX(i, j)] = d[Game.IX(i, j)]*(10 * this.v.Y);//-v[Game.IX(i, j + 1)] + ;
                }
                if (i <= 1 || i >= N - 1)
                {
                    this.v.X = old_x * -1;
                }
                if (j <= 1 || j >= N - 1)
                {
                    this.v.Y = old_y * -1;
                }
            }
            this.x += this.v * dt;
            
            rotv += torque*dt;
            orientation = (float)((orientation + (rotv * dt)));

            torque = 0;
            force = new HyperPoint<float>(0, 0);

            Matrix<float> rot = getRotationMatrix(orientation);
            for (int i = 0; i < vertices.Count; i++)
            {
                HyperPoint<float> rotatedPos = new HyperPoint<float>((rot*localVertices[i]).m);
                vertices[i].Position = rotatedPos + x;
                vertices[i].Velocity = this.v;
            }
        }
        
        public HyperPoint<float> getPosition()
        {
            return x;
        }

        abstract public void calculateInertia();

        public Matrix<float> getRotationMatrix(float angle)
        {
            Matrix<float> rot = new Matrix<float>(2,2);
            rot[0, 0] = (float)Math.Cos(angle);
            rot[0, 1] = (float)-Math.Sin(angle);
            rot[1, 0] = (float)Math.Sin(angle);
            rot[1, 1] = (float)Math.Cos(angle);

            return rot;
        }

        public void draw()
        {
            GL.Color3(0.2, 0.6, 0.8);
            GL.Begin(BeginMode.Polygon);
            for (int i = 0; i < vertices.Count; i++)
            {
                HyperPoint<float> x0 = vertices[i].Position;
                GL.Vertex2(x0.X, x0.Y);
            }
            GL.End();

            foreach (Particle particle in vertices)
            {
                particle.draw();
            }
        }

        public void reset()
        {
            this.x = x_orig;
            this.v = new HyperPoint<float>(0, 0);
            orientation = 0;
            rotv = 0;

            for (int i = 0; i < vertices.Count; i++)
            {
                Particle p = vertices[i];
                p.Position = localVertices[i] + this.x;
            }
        }
    }
}
