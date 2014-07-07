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
        private HyperPoint<float> x;
        private HyperPoint<float> x_orig;
        protected HyperPoint<float> v;

        protected float orientation;
        protected float rotv;
        protected float torque;
        protected HyperPoint<float> force;

        protected List<Particle> vertices = new List<Particle>();
        protected List<HyperPoint<float>> localVertices = new List<HyperPoint<float>>();

        protected Particle selectedParticle;
        protected SpringForce Spring;

        private bool _canRotate;

        public RigidBody(HyperPoint<float> x, float mass, bool canRotate = true)
        {
            this.mass = mass;
            this.X = x;
            this.v = new HyperPoint<float>(0, 0);
            this.x_orig = x;
            this._canRotate = canRotate;
            this.force = new HyperPoint<float>(0, 0);
        }

        protected HyperPoint<float> X
        {
            get { return x; }
            set { x = value; }
        }

        public void addForce(HyperPoint<float> force, HyperPoint<float> location)
        {
            this.force += force;
            this.torque += cross(location - x, force) / inertia;
        }

        private float cross(HyperPoint<float> a, HyperPoint<float> b)
        {
            float i = a.X * b.Y;
            float j = a.Y * b.X;

            return i - j;
        }

        public void update(float dt, int N, float[] d, float[] u, float[] v, float[] o, float mx, float my, BoundryConditions[] conditions)
        {
            foreach (Particle particle in vertices)
            {
                PressureForce pf = new PressureForce(particle, N, d, u, v, conditions);
                DragForce df = new DragForce(particle);

                pf.Calculate();
                df.Calculate();

                if (particle.isSelected)
                {
                    Particle mouseParticle = new Particle(0, new HyperPoint<float>(mx, my));
                    Spring = new SpringForce(mouseParticle, particle, 1 / (float)N, 100, 100);
                    Spring.Calculate();
                }

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

                if (i > 0 && i < N && j > 0 && j < N)
                {
                    if ((i <= 1 && this.v.X < 0) || (i >= N - 1 && this.v.X > 0))
                    {
                        this.v.X = old_x * -1;
                        break;
                    }
                    if ((j <= 1 && this.v.Y < 0) || (j >= N - 1 && this.v.Y > 0))
                    {
                        this.v.Y = old_y * -1;
                        break;
                    }
                }
            }
            this.X += this.v * dt;

            float drag = 0.25f;
            if (_canRotate)
            {
                torque -= rotv * drag;
                rotv += torque * dt;
            }

            orientation = (float)((orientation + (rotv * dt)));

            torque = 0;
            force = new HyperPoint<float>(0, 0);

            Matrix<float> rot = getRotationMatrix(orientation);
            for (int i = 0; i < vertices.Count; i++)
            {
                HyperPoint<float> rotatedPos = new HyperPoint<float>((rot * localVertices[i]).m);
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
            Matrix<float> rot = new Matrix<float>(2, 2);
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

            if (Spring != null)
            {
                Spring.Draw();
            }
        }

        public void reset()
        {
            this.X = x_orig;
            this.v = new HyperPoint<float>(0, 0);
            orientation = 0;
            rotv = 0;

            for (int i = 0; i < vertices.Count; i++)
            {
                Particle p = vertices[i];
                p.Position = localVertices[i] + this.x;
                p.isSelected = false;
            }

            Spring = null;
        }

        public bool pointInPolygon(HyperPoint<float> point)
        {
            int i, j = vertices.Count - 1;
            bool oddNodes = false;

            for (i = 0; i < vertices.Count; i++)
            {
                float xi = vertices[i].Position.X;
                float yi = vertices[i].Position.Y;
                float xj = vertices[j].Position.X;
                float yj = vertices[j].Position.Y;

                if ((yi < point.Y && yj >= point.Y || yj < point.Y && yi >= point.Y) && (xi <= point.X || xj <= point.X))
                {
                    if (xi + (point.Y - yi) / (yj - yi) * (xj - xi) < point.X)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }

        public bool IsInPolygon(HyperPoint<float> p)
        {
            int nvert = vertices.Count;

            int i, j;
            bool c = false;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                HyperPoint<float> verti = vertices[i].Position;
                HyperPoint<float> vertj = vertices[j].Position;

                if (((verti.Y > p.Y) != (vertj.Y > p.Y)) &&
                    (p.X < (vertj.X - verti.X) * (p.Y - verti.Y) / (vertj.Y - verti.Y) + verti.X))
                    c = !c;
            }
            return c;
        }

        public List<Particle> getGlobalVertices()
        {
            return vertices;
        }

        public HyperPoint<float> getVelocity()
        {
            return v;
        }

        public void selectParticle(Particle p)
        {
            selectedParticle = p;
            p.isSelected = true;
        }

        public void deselectParticle()
        {
            selectedParticle.isSelected = false;
            selectedParticle = null;
            Spring = null;
        }
    }
}
