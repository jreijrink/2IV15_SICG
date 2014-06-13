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
    public enum GameType { Particle, Cloth, Hair, Curtain }

    public class Game
	{
		private int N;
		public float dt, d;
		private bool dsim;
		private bool dump_frames;
		private int frame_number;

        private int win_id;
        private int[] mouse_down;
        private int[] mouse_release;
        private int[] mouse_shiftclick;
        private int omx, omy, mx, my;
        private int hmx, hmy;
        private double viewWidth;
        private double viewHeight;
        private List<Particle> particles;
        private List<Force> forces;
        private List<Constraint> constrains;
        private List<FixedObject> objects;

        private Rectangle drawWindow = new Rectangle(0, 0, 400, 400);
        private double minParticleDistance = 0.02;
        private double minHorDistance = 0.2;
        private Particle currentSelectedParticle;
        private Particle mouseParticle;
        private bool hor_force_applied;
        private SpringForce mouseSpringForce;

        public int integrationMode = 0;
        public int numSteps = 1;
        private float particle_size;

        /*
		----------------------------------------------------------------------
		free/clear/allocate simulation data
		----------------------------------------------------------------------
		*/

        private void ClearData()
        {
            particles.ForEach(x => x.reset());
        }

        public void InitParticleSystem(Rectangle drawWindow)
        {
            this.viewWidth = 4.0;
            this.viewHeight = 4.0;
            this.particle_size = 0.001f;
            this.drawWindow = drawWindow;
            float dist = 0.2f;
            HyperPoint<float> center = new HyperPoint<float>(0.0f, 0.0f);
            HyperPoint<float> offset = new HyperPoint<float>(dist, 0.0f);
            HyperPoint<float> offset2 = new HyperPoint<float>(0.0f, dist);

            float apha = MathHelper.PiOver4;
            HyperPoint<float> offset3 = new HyperPoint<float>((float) -Math.Cos(apha)*dist, (float) Math.Sin(apha)*dist);
            
            particles = new List<Particle>();

            particles.Add(new Particle(0, offset2 * -1, 5.0f));
            particles.Add(new Particle(1, offset * 3, 5.0f));
            particles.Add(new Particle(2, offset3 * 6, 5.0f));
            particles.Add(new Particle(3, particles[2].Position+new HyperPoint<float>(-0.5f, 0.2f), 10.0f));

            particles.Add(new Particle(4, new HyperPoint<float>(0, 1.0f), 5.0f));
            particles.Add(new Particle(5, new HyperPoint<float>(0.2f, 1.2f), 5.0f));

            forces = new List<Force>();
            forces.Add(new SpringForce(particles[0], particles[1], dist * 1, 5.0f, 5.0f));
            forces.Add(new SpringForce(particles[1], particles[2], dist * 5, 10.0f, 5.0f));
            forces.Add(new SpringForce(particles[2], particles[0], dist * 8, 5.0f, 5.0f));
            forces.Add(new SpringForce(particles[2], particles[5], (particles[2].Position-particles[5].Position).GetLength()*1.5f, 5.0f, 5.0f));

            forces.Add(new GravityForce(particles[0]));
            forces.Add(new GravityForce(particles[1]));
            forces.Add(new GravityForce(particles[2]));
            forces.Add(new GravityForce(particles[3]));
            forces.Add(new GravityForce(particles[4]));
            forces.Add(new GravityForce(particles[5]));

            constrains = new List<Constraint>();
            constrains.Add(new CircularWireConstraint(particles[0], center, dist * 1));
            constrains.Add(new CircularWireConstraint(particles[1], center, dist * 3));
            constrains.Add(new CircularWireConstraint(particles[2], center, dist * 6));

            constrains.Add(new RodConstraint(particles[2], particles[3], dist * 2));
            constrains.Add(new RodConstraint(particles[4], particles[5], dist * 2));
            constrains.Add(new LineConstraint(particles[4], new HyperPoint<float>(-2, 1), new HyperPoint<float>(2, 1)));

            objects = new List<FixedObject>();
            objects.Add(new VerLineObject(-1.95f, -2, 4, true));
            objects.Add(new VerLineObject(1.95f, -2, 4, false));

            objects.Add(new HorLineObject(-2f, -1.95f, 4, true));
        }
        
        public void InitFlagSystem(Rectangle drawWindow)
        {
            this.viewWidth = 4.0;
            this.viewHeight = 4.0;
            this.particle_size = 0.015f;
            this.drawWindow = drawWindow;

            int size = 7;
            float dist = 0.2f;
            HyperPoint<float> start = new HyperPoint<float>(-0.8f, 0.0f);
            HyperPoint<float> offset = new HyperPoint<float>(0.0f, 0.0f);

            particles = new List<Particle>();
            forces = new List<Force>();
            constrains = new List<Constraint>();
            objects = new List<FixedObject>();

            createFlag(particles, forces, constrains, objects, start, offset, size, dist);
            //createCurtain(particles, forces, constrains, objects, start, offset, size, dist);
        }

        public void InitCurtainSystem(Rectangle drawWindow)
        {
            this.viewWidth = 4.0;
            this.viewHeight = 4.0;
            this.particle_size = 0.013f;
            this.drawWindow = drawWindow;

            HyperPoint<int> size = new HyperPoint<int>(5,7);
            float dist = 0.2f;
            HyperPoint<float> start = new HyperPoint<float>(-1.5f, 0.0f);
            HyperPoint<float> offset = new HyperPoint<float>(0.0f, 0.0f);

            particles = new List<Particle>();
            forces = new List<Force>();
            constrains = new List<Constraint>();
            objects = new List<FixedObject>();

            createCurtain(particles, forces, constrains, objects, start, offset, size, dist);
        }

        public void InitHairSystem(Rectangle drawWindow)
        {
            this.viewWidth = 4.0;
            this.viewHeight = 4.0;
            this.particle_size = 0.001f;
            this.drawWindow = drawWindow;

            float length = 0.2f;
            float ks = 10f;
            float kd = 20f;

            particles = new List<Particle>();
            forces = new List<Force>();
            constrains = new List<Constraint>();
            objects = new List<FixedObject>();

            int index = 0;
            for (float i = -0.3f; i <= 0.3f; i += 0.3f)
            {
                HyperPoint<float> start = new HyperPoint<float>(i, 1.8f);
                createHair(particles, forces, constrains, index, start, length, ks, kd);
                index += 10;
            }
        }

        private void createCurtain(List<Particle> particels, List<Force> forces, List<Constraint> constraints, List<FixedObject> objects,
            HyperPoint<float> start, HyperPoint<float> offset, HyperPoint<int> size, float dist)
        {
            int index = 0;
            
            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                    start = start + new HyperPoint<float>(size.X*dist*i, 0) + new HyperPoint<float>(0.9f, 0);
                for (int y = 0; y < size.Y; y++)
                {
                    for (int x = 0; x < size.X; x++)
                    {
                        offset = new HyperPoint<float>(dist*x, dist*y);
                        Particle particle = new Particle(index, start + offset, 5.0f);
                        particles.Add(particle);
                        forces.Add(new GravityForce(particle));

                        if (x != 0)
                        {
                            //Stiff spring to left particle
                            forces.Add(createStiffSpringForce(particle, particles[index - 1], dist));
                            //constrains.Add(new RodConstraint(particle, particles[index - 1], dist));
                        }
                        if (x > 1)
                        {
                            //Long spring to particle 2 to left
                            forces.Add(createSpringForce(particle, particles[index - 2], dist*2));
                        }

                        if (y != 0)
                        {
                            //Stiff spring to above particle
                            forces.Add(createStiffSpringForce(particle, particles[index - size.X], dist));
                            //constrains.Add(new RodConstraint(particle, particles[index - size], dist));

                            //Stiff spring to cross particles
                            if (x != 0)
                            {
                                forces.Add(createStiffSpringForce(particle, particles[index - size.X - 1],
                                                                  (float) Math.Sqrt(2*dist*dist)));
                                //constrains.Add(new RodConstraint(particle, particles[index - size - 1], (float)Math.Sqrt(2 * dist * dist)));
                            }
                            if (x < (size.X - 1))
                            {
                                forces.Add(createStiffSpringForce(particle, particles[index - size.X + 1],
                                                                  (float) Math.Sqrt(2*dist*dist)));
                                //constrains.Add(new RodConstraint(particle, particles[index - size + 1], (float)Math.Sqrt(2 * dist * dist)));
                            }
                        }
                        if (y > 1)
                        {
                            //Long spring to particle 2 to above
                            forces.Add(createSpringForce(particle, particles[index - (size.X*2)], dist*2));
                        }

                        if (y == size.Y - 1)
                        {
                            //constrains.Add(new FixedConstraint(particle, particle.Position));
                            constrains.Add(new LineConstraint(particle, particle.Position + new HyperPoint<float>(-2, 0),
                                                              particle.Position + new HyperPoint<float>(2, 0)));
                        }
                        index++;
                    }
                }
            }

            objects.Add(new VerLineObject(-1.95f, -2, 4, true));
            objects.Add(new VerLineObject(1.95f, -2, 4, false));
        }

        private void createFlag(List<Particle> particels, List<Force> forces, List<Constraint> constraints, List<FixedObject> objects,
            HyperPoint<float> start, HyperPoint<float> offset, int size, float dist)
        {
            int index = 0;
            offset = new HyperPoint<float>(0.0f, dist * (size - 1));
            Particle fixedparticle1 = new Particle(0, start + offset, 5.0f);
            offset = new HyperPoint<float>(dist * (size - 1), dist * (size - 1));
            Particle fixedparticle2 = new Particle(0, start + offset, 5.0f);

            VerLineObject pole = new VerLineObject(start.X, -2, 3.2f, true);
            objects.Add(pole);

            Color color = Color.Blue;
            for (int y = 0; y < size; y++)
            {
                if (y >= 3)
                    color = Color.White;
                if (y >= 5)
                    color = Color.Red;

                for (int x = 0; x < size; x++)
                {
                    offset = new HyperPoint<float>(dist * x, dist * y);
                    Particle particle = new Particle(index, start + offset, 5.0f);
                    particles.Add(particle);
                    forces.Add(new GravityForce(particle));

                    if (x != 0)
                    {
                        //Stiff spring to left particle
                        forces.Add(createStiffSpringForce(particle, particles[index - 1], dist, color));
                    }
                    if (x > 1)
                    {
                        //Long spring to particle 2 to left
                        forces.Add(createSpringForce(particle, particles[index - 2], dist * 2, color));
                    }

                    if (y != 0)
                    {
                        //Stiff spring to above particle
                        forces.Add(createStiffSpringForce(particle, particles[index - size], dist, color));

                        //Stiff spring to cross particles
                        if (x != 0)
                        {
                            forces.Add(createStiffSpringForce(particle, particles[index - size - 1], (float)Math.Sqrt(2 * dist * dist), color));
                        }
                        if (x < (size - 1))
                        {
                            forces.Add(createStiffSpringForce(particle, particles[index - size + 1], (float)Math.Sqrt(2 * dist * dist), color));
                        }
                    }
                    if (y > 1)
                    {
                        //Long spring to particle 2 to above
                        forces.Add(createSpringForce(particle, particles[index - (size * 2)], dist * 2, color));
                    }

                    if (x == 0)
                    {
                        //Create the pole
                        constrains.Add(new LineConstraint(particle, new HyperPoint<float>(particle.Position.X, 1.2f), new HyperPoint<float>(particle.Position.X, -2)));
                        if (y == (size - 1))
                        {
                            //Fixd point in left top
                            constrains.Add(new FixedConstraint(particle, particle.Position));
                        }
                    }
                    index++;
                }
            }
        }

        private void createHair(List<Particle> particels, List<Force> forces, List<Constraint> constraints,
            int start_index, HyperPoint<float> start, float length, float ks, float kd)
        {

            particles.Add(new Particle(start_index, start));
            particles.Add(new Particle(start_index + 1, start - new HyperPoint<float>(length, length)));
            particles.Add(new Particle(start_index + 2, start - new HyperPoint<float>(0, length * 2)));
            particles.Add(new Particle(start_index + 3, start - new HyperPoint<float>(length, length * 3)));
            particles.Add(new Particle(start_index + 4, start - new HyperPoint<float>(0, length * 4)));
            particles.Add(new Particle(start_index + 5, start - new HyperPoint<float>(length, length * 5)));
            particles.Add(new Particle(start_index + 6, start - new HyperPoint<float>(0, length * 6)));
            particles.Add(new Particle(start_index + 7, start - new HyperPoint<float>(length, length * 7)));
            particles.Add(new Particle(start_index + 8, start - new HyperPoint<float>(0, length * 8)));
            particles.Add(new Particle(start_index + 9, start - new HyperPoint<float>(length, length * 9)));

            forces.Add(new GravityForce(particles[start_index]));
            forces.Add(new GravityForce(particles[start_index + 1]));
            forces.Add(new GravityForce(particles[start_index + 2]));
            forces.Add(new GravityForce(particles[start_index + 3]));
            forces.Add(new GravityForce(particles[start_index + 4]));
            forces.Add(new GravityForce(particles[start_index + 5]));
            forces.Add(new GravityForce(particles[start_index + 6]));
            forces.Add(new GravityForce(particles[start_index + 7]));
            forces.Add(new GravityForce(particles[start_index + 8]));
            forces.Add(new GravityForce(particles[start_index + 9]));

            float hair_lenght = new HyperPoint<float>(length, length).GetLength();
            float spring_lenght = 2 * hair_lenght * (float)Math.Sin(MathHelper.PiOver4);
            forces.Add(new SpringForce(particles[start_index + 0], particles[start_index + 2], spring_lenght, ks, kd));
            forces.Add(new SpringForce(particles[start_index + 1], particles[start_index + 3], spring_lenght, ks, kd));
            forces.Add(new SpringForce(particles[start_index + 2], particles[start_index + 4], spring_lenght, ks, kd));
            forces.Add(new SpringForce(particles[start_index + 3], particles[start_index + 5], spring_lenght, ks, kd));
            forces.Add(new SpringForce(particles[start_index + 4], particles[start_index + 6], spring_lenght, ks, kd));
            forces.Add(new SpringForce(particles[start_index + 5], particles[start_index + 7], spring_lenght, ks, kd));
            forces.Add(new SpringForce(particles[start_index + 6], particles[start_index + 8], spring_lenght, ks, kd));
            forces.Add(new SpringForce(particles[start_index + 7], particles[start_index + 9], spring_lenght, ks, kd));

            

            constrains.Add(new FixedConstraint(particles[start_index + 0], particles[start_index + 0].Position));
            constrains.Add(new RodConstraint(particles[start_index + 0], particles[start_index + 1], hair_lenght));
            constrains.Add(new RodConstraint(particles[start_index + 1], particles[start_index + 2], hair_lenght));
            constrains.Add(new RodConstraint(particles[start_index + 2], particles[start_index + 3], hair_lenght));
            constrains.Add(new RodConstraint(particles[start_index + 3], particles[start_index + 4], hair_lenght));
            constrains.Add(new RodConstraint(particles[start_index + 4], particles[start_index + 5], hair_lenght));
            constrains.Add(new RodConstraint(particles[start_index + 5], particles[start_index + 6], hair_lenght));
            constrains.Add(new RodConstraint(particles[start_index + 6], particles[start_index + 7], hair_lenght));
            constrains.Add(new RodConstraint(particles[start_index + 7], particles[start_index + 8], hair_lenght));
            constrains.Add(new RodConstraint(particles[start_index + 8], particles[start_index + 9], hair_lenght)); 
        }

        private SpringForce createSpringForce(Particle p1, Particle p2, float dist)
        {
            return new SpringForce(p1, p2, dist, 1.0f, 1.0f, Color.Empty);
        }

        private SpringForce createSpringForce(Particle p1, Particle p2, float dist, Color color)
        {
            return new SpringForce(p1, p2, dist, 1.0f, 1.0f, color);
        }

        private SpringForce createStiffSpringForce(Particle p1, Particle p2, float dist)
        {
            return createStiffSpringForce(p1, p2, dist, Color.Empty);
        }

        private SpringForce createStiffSpringForce(Particle p1, Particle p2, float dist, Color color)
        {
            return new SpringForce(p1, p2, dist, 50.0f, 10.0f, color);
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
                if ((frame_number % FrameInterval) == 0)
                {
                    using (Bitmap bmp = new Bitmap(drawWindow.Width, drawWindow.Height))
                    {
                        System.Drawing.Imaging.BitmapData data =
                            bmp.LockBits(this.drawWindow, System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                         System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        GL.ReadPixels(0, 0, drawWindow.Width, drawWindow.Height, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                        bmp.UnlockBits(data);

                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                        if (!Directory.Exists("snapshots"))
                            Directory.CreateDirectory("snapshots");

                        string filename = string.Format("snapshots/img{0}.png", Convert.ToSingle(frame_number) / FrameInterval);
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
            constrains.ForEach(c => c.Draw());
        }

        private void DrawObjects()
        {
            objects.ForEach(o => o.Draw());
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
            DrawObjects();

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
            if (dsim)
            {
                for (int i = 0; i < numSteps; i++ )
                    Solver.SimulationStep(particles, forces, constrains, objects, particle_size, dt, integrationMode);
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
                case Keys.NumPad6:
			        numSteps += 1;
			        break;
                case Keys.NumPad4:
			        numSteps -=1;
                    numSteps = (numSteps > 1) ? numSteps : 1;
                    break;
                case Keys.NumPad8:
                    dt += 0.001f;
                    break;
                case Keys.NumPad2:
                    dt -= 0.001f;
			        dt = (dt > 0) ? dt : 0.001f;
                    break;
				case Keys.C:
					ClearData();
					break;

                case Keys.D:
                    dump_frames = !dump_frames;
                    break;

                case Keys.Q:
                    break;
                case Keys.I:
			        integrationMode = (integrationMode + 1)%3;
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

        public void Pause()
        {
            dsim = false;
        }

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (!dsim)
                return;

            float mouseX = (float)(((e.Location.X / (float)drawWindow.Width) - 0.5) * viewWidth);
            float mouseY = (float)(((e.Location.Y / (float)drawWindow.Height) - 0.5) * -viewHeight);

            if (e.Button == MouseButtons.Left)
            {
                if (this.currentSelectedParticle != null || mouseSpringForce != null)
                    return;

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
                    this.mouseParticle = new Particle(0, mouseLoc, 1f);
                    this.currentSelectedParticle = selectedParticle;
                    this.currentSelectedParticle.isSelected = true;
                    this.mouseSpringForce = new SpringForce(selectedParticle, mouseParticle, 0.01f, 10.0f, 1f);
                    forces.Add(mouseSpringForce);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                foreach (Particle particle in particles)
                {
                    double hor_distance = particle.Position.Y - mouseY;
                    if (hor_distance < minHorDistance && hor_distance > -minHorDistance)
                    {
                        bool direction = particle.Position.X > mouseX;
                        forces.Add(new HorizontalForce(particle, direction));
                    }
                }
                hor_force_applied = true;
            }
        }

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (!dsim)
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
            else if (e.Button == MouseButtons.Right)
            {
                forces.RemoveAll(F => F.GetType() == typeof(HorizontalForce));
                hor_force_applied = false;
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            float mouseX = (float)(((e.Location.X / (float)drawWindow.Width) - 0.5) * viewWidth);
            float mouseY = (float)(((e.Location.Y / (float)drawWindow.Height) - 0.5) * -viewHeight);

            HyperPoint<float> mouseLoc = new HyperPoint<float>(mouseX, mouseY);

            if (this.mouseParticle != null)
            {
                mouseParticle.Position = mouseLoc;
            }
            
            if (hor_force_applied)
            {
                forces.RemoveAll(F => F.GetType() == typeof(HorizontalForce));

                foreach (Particle particle in particles)
                {
                    double hor_distance = particle.Position.Y - mouseY;
                    if (hor_distance < minHorDistance && hor_distance > -minHorDistance)
                    {
                        bool direction = particle.Position.X > mouseX;
                        forces.Add(new HorizontalForce(particle, direction));
                    }
                }
            }
        }
    }
}