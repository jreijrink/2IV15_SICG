using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using micfort.GHL.Math2;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Project1
{
    public class Game
	{
		private int N;
		private float dt, d;
		private bool dsim;
		private bool dump_frames;
		private int frame_number;

		// static Particle *pList;
		private List<Particle> particles;

		private int win_id;
		private int[] mouse_down;
		private int[] mouse_release;
		private int[] mouse_shiftclick;
		private int omx, omy, mx, my;
		private int hmx, hmy;
        private double viewWidth;
        private double viewHeight;
        private List<Force> forces;
        private List<Constraint> contrains;

        private Rectangle drawWindow = new Rectangle(0,0,400,400);
        private double minParticleDistance = 0.02;
        private Particle currentSelectedParticle;
        private Particle mouseParticle;
        private SpringForce mouseSpringForce;


        /*
		----------------------------------------------------------------------
		free/clear/allocate simulation data
		----------------------------------------------------------------------
		*/

		private void ClearData()
		{
			particles.ForEach(x => x.reset());
		}

        public void InitSystem(Rectangle drawWindow)
        {
            this.viewWidth = 4.0;
            this.viewHeight = 4.0;

            this.drawWindow = drawWindow;
			float dist = 0.5f;
            HyperPoint<float> center = new HyperPoint<float>(0.0f, 0.0f);
            HyperPoint<float> offset = new HyperPoint<float>(dist, 0.0f);

			particles = new List<Particle>();

			particles.Add(new Particle(center + offset * 1, 1.0f));
            particles.Add(new Particle(center + offset * 2, 1.0f));
            particles.Add(new Particle(center + offset * 3, 1.0f));

            forces = new List<Force>();
            forces.Add(new SpringForce(particles[0], particles[1], dist * 2, 1.0f, 1.0f));
            forces.Add(new SpringForce(particles[1], particles[2], dist * 2, 1.0f, 1.0f));
            forces.Add(new SpringForce(particles[2], particles[0], dist * 3, 1.0f, 1.0f));
            forces.Add(new GravityForce(particles[0]));
            forces.Add(new GravityForce(particles[1]));
            forces.Add(new GravityForce(particles[2]));

            contrains = new List<Constraint>();
            contrains.Add(new RodConstraint(particles[0], particles[1], dist));
            contrains.Add(new CircularWireConstraint(particles[0], center, dist));
            contrains.Add(new CircularWireConstraint(particles[1], center, dist * 2));
            contrains.Add(new CircularWireConstraint(particles[2], center, dist * 3));
		}

		/*
		----------------------------------------------------------------------
		OpenGL specific drawing routines
		----------------------------------------------------------------------
		*/

		private void PreDisplay()
		{
			GL.Viewport(0, 0, drawWindow.Width, drawWindow.Height);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
            GL.Ortho(-viewWidth / 2, viewWidth / 2, -viewHeight / 2, viewHeight / 2, -1, 1);
			GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit);
		}

		private void PostDisplay()
		{
			// Write frames if necessary.
			if (dump_frames)
			{
				const int FrameInterval = 4;
				if((frame_number % FrameInterval) == 0)
				{
					using(Bitmap bmp = new Bitmap(drawWindow.Width, drawWindow.Height))
					{
						System.Drawing.Imaging.BitmapData data =
							bmp.LockBits(this.drawWindow, System.Drawing.Imaging.ImageLockMode.WriteOnly,
										 System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        GL.ReadPixels(0, 0, drawWindow.Width, drawWindow.Height, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
						bmp.UnlockBits(data);

						bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

						if (!Directory.Exists("snapshots"))
							Directory.CreateDirectory("snapshots");

						string filename = string.Format("snapshots/img{0}.png", Convert.ToSingle(frame_number)/FrameInterval);
						bmp.Save(filename);
						Console.Out.WriteLine("Output snapshot: {0}", Convert.ToSingle(frame_number) / FrameInterval);
					}
				}
			}
			frame_number++;
		}

		private void DrawParticles()
		{
			particles.ForEach(x => x.draw());
		}

		private void DrawForces()
        {
            forces.ForEach(f => f.Draw());
		}

		private void DrawConstraints()
        {
            contrains.ForEach(c => c.Draw());
		}
		
		/*
		----------------------------------------------------------------------
		relates mouse movements to tinker toy construction
		----------------------------------------------------------------------
		*/
        
		/*
		----------------------------------------------------------------------
		callback routines
		----------------------------------------------------------------------
		*/

        public void OnLoad(object sender, EventArgs eventArgs)
		{
			// setup settings, load textures, sounds
			//VSync = VSyncMode.On;

			GL.Enable(EnableCap.LineSmooth); 
			GL.Enable(EnableCap.PolygonSmooth);
		}

		public void OnResize(object sender, EventArgs eventArgs)
		{
            GLControl control = (GLControl)sender;
		    drawWindow = control.ClientRectangle;

            GL.Viewport(0, 0, drawWindow.Width, drawWindow.Height);
		}

        public void OnRenderFrame()
		{
			PreDisplay();

			DrawForces();
			DrawConstraints();
			DrawParticles();
			
			PostDisplay();

			// render graphics
			//GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			//GL.MatrixMode(MatrixMode.Projection);
			//GL.LoadIdentity();
			//GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

			//GL.Begin(PrimitiveType.Triangles);

			//GL.Color3(Color.MidnightBlue);
			//GL.Vertex2(-1.0f, 1.0f);
			//GL.Color3(Color.SpringGreen);
			//GL.Vertex2(0.0f, -1.0f);
			//GL.Color3(Color.Ivory);
			//GL.Vertex2(1.0f, 1.0f);

			//GL.End();

			//SwapBuffers();
		}

        public void OnUpdateFrame()
		{
			if(dsim)
			{
				Solver.SimulationStep(particles, forces, contrains, dt);
			}
			else
			{
				//todo reset
			}
		}

        public void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
		{
		}

        public void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
		{
            switch (keyEventArgs.KeyCode)
			{
				case Keys.C:
					ClearData();
					break;

				case Keys.D:
					dump_frames = !dump_frames;
					break;

				case Keys.Q:
					break;

				case Keys.Space:
					dsim = !dsim;
					break;
			}
		}
        
		public Game(int n, float dt, float d)
		{
			this.N = n;
			this.dt = dt;
			this.d = d;

			dsim = false;
			dump_frames = false;
		    frame_number = 0;

//			this.Load += OnLoad;
//			this.Resize += OnResize;
//			this.UpdateFrame += OnUpdateFrame;
//			this.RenderFrame += OnRenderFrame;
//			this.KeyDown += OnKeyDown;
//			this.KeyUp += OnKeyUp;
		}

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (!dsim || this.currentSelectedParticle != null || mouseSpringForce != null)
                return;

            if(e.Button == MouseButtons.Left)
            {
                float mouseX = (float) (((e.Location.X / (float)drawWindow.Width) -  0.5) * viewWidth);
                float mouseY = (float) (((e.Location.Y / (float)drawWindow.Height) - 0.5) * -viewHeight);

                HyperPoint<float> mouseLoc = new HyperPoint<float>(mouseX, mouseY);

                double minDistance = double.PositiveInfinity;
                Particle selectedParticle = null;
                foreach (Particle particle in particles)
                {
                    double distance = (particle.Position - mouseLoc).GetLengthSquared();
                    if (distance < minDistance && distance < minParticleDistance)
                    {
                        minDistance = distance;
                        selectedParticle = particle;
                    }
                }

                if (selectedParticle != null)
                {
                    this.mouseParticle = new Particle(mouseLoc, 1f);
                    this.currentSelectedParticle = selectedParticle;
                    this.currentSelectedParticle.isSelected = true;
                    this.mouseSpringForce = new SpringForce(selectedParticle, mouseParticle, 0.01f, 1f, 1f);
                    forces.Add(mouseSpringForce);
                }
            }
        }

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            if(!dsim)
                return;

            if (e.Button == MouseButtons.Left)
            {
                if (currentSelectedParticle != null)
                {
                    forces.Remove(mouseSpringForce);
                    this.currentSelectedParticle.isSelected = false;
                    this.mouseParticle = null;
                    this.currentSelectedParticle = null;
                    this.mouseSpringForce = null;
                }
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            float mouseX = (float)(((e.Location.X / (float)drawWindow.Width) - 0.5) * viewWidth);
            float mouseY = (float)(((e.Location.Y / (float)drawWindow.Height) - 0.5) * -viewHeight);
            HyperPoint<float> mouseLoc = new HyperPoint<float>(mouseX, mouseY);

            if(this.mouseParticle != null)
            {
                mouseParticle.Position = mouseLoc;
            }
        }
	}
}