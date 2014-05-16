using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Project1
{
	class Particle
	{
        private int _index;
		private HyperPoint<float> _constructPos;
        private HyperPoint<float> _position;
        private HyperPoint<float> _velocity;
        private HyperPoint<float> _force;
        private HyperPoint<float> _acceleration;
        private float _mass;

	    public bool isSelected = false;

	    public int Index
        {
            get { return _index; }
        }

		public HyperPoint<float> Position
		{
			get { return _position; }
			set { _position = value; }
		}

        public HyperPoint<float> Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public HyperPoint<float> Force
        {
            get { return _force; }
            set { _force = value; }
        }

        public HyperPoint<float> Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }

        public float Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }

        public Particle(int index)
            : this(index, new HyperPoint<float>(0, 0), 1.0f) { }
        
        public Particle(int index, HyperPoint<float> constructPos)
            : this(index, constructPos, 1.0f) { }

        public Particle(int index, HyperPoint<float> constructPos, float mass)
        {
            _index = index;
            _constructPos = constructPos;
            _position = constructPos;
            _mass = mass;
            _velocity = new HyperPoint<float>(0, 0);
            _force = new HyperPoint<float>(0, 0);
            _acceleration = new HyperPoint<float>(0, 0);
            _mass = mass;
        }

		public void reset()
		{
			_position = _constructPos;
			_velocity = new HyperPoint<float>(0f, 0f);
		}

		public void draw()
        {
			const double h = 0.03;
            if(isSelected)
                GL.Color3(1f, 0f, 0f);
            else
			    GL.Color3(1f, 1f, 1f);

			GL.Begin(BeginMode.Quads);
			GL.Vertex2(_position[0] - h / 2.0, _position[1] - h / 2.0);
			GL.Vertex2(_position[0] + h / 2.0, _position[1] - h / 2.0);
			GL.Vertex2(_position[0] + h / 2.0, _position[1] + h / 2.0);
			GL.Vertex2(_position[0] - h / 2.0, _position[1] + h / 2.0);
			GL.End();
		}
	}
}
