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
        private HyperPoint<float> v1, v2, v3, v4;

        public Box(HyperPoint<float> x, float mass, float height, float width)
            : base(x, mass)
        {
            this.width = width;
            this.height = height;

            v1 = new HyperPoint<float>(x.X - width / 2.0f, x.Y - height / 2.0f);
            v2 = new HyperPoint<float>(x.X + width / 2.0f, x.Y - height / 2.0f);
            v3 = new HyperPoint<float>(x.X + width / 2.0f, x.Y + height / 2.0f);
            v4 = new HyperPoint<float>(x.X - width / 2.0f, x.Y + height / 2.0f);
        }

        public override void calculateInertia()
        {
            this.inertia = (1/12.0f)*width*height*(width*width + height*height);
        }

        public override void draw()
        {
            GL.Color3(0.25f, 0.4f, 0.89f);
            GL.Begin(BeginMode.Quads);

            GL.Vertex2(v1.X, v1.Y);
            GL.Vertex2(v2.X, v2.Y);
            GL.Vertex2(v3.X, v3.Y);
            GL.Vertex2(v4.X, v4.Y);

            GL.End();
        }
    }
}
