using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
 
        public RigidBody(HyperPoint<float> x, float mass)
        {
            this.mass = mass;
            this.x = x;

            calculateInertia();
        }

        public void addForce(HyperPoint<float> force, HyperPoint<float> location)
        {
            this.force += force;
            this.torque += (((location - x)).Cross2D(force) / inertia);
        }

        public void update(float dt)
        {
            v += (force/mass)*dt;
            x += v*dt;

            rotv += torque*dt;
            orientation = (float)((orientation + (rotv * dt)) % (2 * Math.PI));

            torque = 0;
            force = new HyperPoint<float>(0, 0);
        }

        abstract public void calculateInertia();
        public abstract void draw();

    }
}
