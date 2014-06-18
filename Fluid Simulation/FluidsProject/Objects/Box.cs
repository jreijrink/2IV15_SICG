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
    class Box : RigidBody
    {
        private float width, height;

        public Box(HyperPoint<float> x, float mass, float height, float width, int N)
            : base(x, mass)
        {
            this.width = width;
            this.height = height;


            HyperPoint<float> v1 = new HyperPoint<float>(x.X - width / 2.0f, x.Y - height / 2.0f);
            HyperPoint<float> v2 = new HyperPoint<float>(x.X + width / 2.0f, x.Y - height / 2.0f);
            HyperPoint<float> v3 = new HyperPoint<float>(x.X + width / 2.0f, x.Y + height / 2.0f);
            HyperPoint<float> v4 = new HyperPoint<float>(x.X - width / 2.0f, x.Y + height / 2.0f);

            constructEdge(v1, v2, N);
            constructEdge(v2, v3, N);
            constructEdge(v3, v4, N);
            constructEdge(v4, v1, N);
            
            calculateInertia();
        }
        
        public void constructEdge(HyperPoint<float> x1, HyperPoint<float> x2, int N)
        {
            int amount = (int) ((x1 - x2).GetLength() * N) + 1;
            HyperPoint<float> dir = (x2 - x1) / amount;

            for (int i = 0; i < amount; i++)
            {
                HyperPoint<float> px = x1 + dir*i;
                vertices.Add(new Particle(0, px, 1));
                localVertices.Add(px - this.x);
            }
            
        }

        public override void calculateInertia()
        {
            this.inertia = mass * (width*width + height*height);
        }
    }
}
