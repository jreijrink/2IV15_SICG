using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluidsProject.Particles;
using micfort.GHL.Math2;

namespace FluidsProject.Objects
{
    class Disk : RigidBody
    {
        private float radius;

        public Disk(HyperPoint<float> x, float radius, float mass, bool canRotate = true) : base(x, mass, canRotate)
        {
            this.radius = radius;

            constructDisc();
            calculateInertia();
        }

        private void constructDisc()
        {
            int amount = 20;
            float angleStep = (float) (( 2*Math.PI) / amount);
            for (int i = 0; i < amount; i++)
            {
                Particle p = new Particle(i);
                float x = (float)Math.Cos(i * angleStep) * radius;
                float y = (float)Math.Sin(i * angleStep) * radius;

                HyperPoint<float> pos = new HyperPoint<float>(x, y);
                p.Position = pos + this.X;

                vertices.Add(p);
                localVertices.Add(pos);
            }
        }

        public override void calculateInertia()
        {
            this.inertia = 0.5f*mass*radius*radius;
        }
    }
}
