using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        protected HyperPoint<float> x = new HyperPoint<float>(0, 0);
        protected HyperPoint<float> v = new HyperPoint<float>(0, 0);

        protected float orientation;
        protected float rotv;
        protected float torque;
        protected HyperPoint<float> force = new HyperPoint<float>(0, 0);

        protected List<HyperPoint<float>> vertices = new List<HyperPoint<float>>();
        protected List<HyperPoint<float>> localVertices = new List<HyperPoint<float>>();
 
        public RigidBody(HyperPoint<float> x, float mass)
        {
            this.mass = mass;
            this.x = x;
        }

        public void addForce(HyperPoint<float> force, HyperPoint<float> location)
        {
            location = vertices[0];
            this.force += force;
            this.torque +=  cross(location - x, force) / inertia;
        }

        private float cross(HyperPoint<float> a, HyperPoint<float> b)
        {
            float i =  a.X*b.Y;
            float j = a.Y*b.X;

            return i - j;
        }

        public void update(float dt)
        {
            v += ((force/mass)*dt);
            x += v*dt;

            rotv += torque*dt;
            orientation = (float)((orientation + (rotv * dt)));

            torque = 0;
            force = new HyperPoint<float>(0, 0);

            Matrix<float> rot = getRotationMatrix(orientation);
            for (int i = 0; i < vertices.Count; i++)
            {
                HyperPoint<float> rotatedPos = new HyperPoint<float>((rot*localVertices[i]).m);
                vertices[i] = rotatedPos + x;
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
                HyperPoint<float> x0 = vertices[i];

                GL.Vertex2(x0.X, x0.Y);
            }
            GL.End();
        }
    }
}
