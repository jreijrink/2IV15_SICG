using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            int amountX = (int)Math.Abs(v1.X * N - (int)(v2.X * N)) + 1;
            int amountY = (int) Math.Abs(v1.Y*N - (int) (v2.Y*N)) + 1;

            float distX = width/amountX;
            float distY = height/amountY;

            for (int i = 0; i < amountX; i++)
            {
                
            }


            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);

            localVertices.Add(v1 - x);
            localVertices.Add(v2 - x);
            localVertices.Add(v3 - x);
            localVertices.Add(v4 - x);

            calculateInertia();
        }

        public override void calculateInertia()
        {
            this.inertia = mass * (width*width + height*height);
        }
    }
}
