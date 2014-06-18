using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using micfort.GHL.Math2;
using OpenTK;

namespace FluidsProject.Particles
{
    class Cloth
    {
        private int N;
        public float dt, d;
        private int windowWidth, windowHeigt;

        private List<Particle> particles;
        private List<Force> forces;
        private List<Constraint> constrains;
        private List<FixedObject> objects;

        private double minParticleDistance = 0.0005f;
        private double minHorDistance = 0.2;
        private Particle currentSelectedParticle;
        private Particle mouseParticle;
        //private bool hor_force_applied;
        private SpringForce mouseSpringForce;

        public int integrationMode = 2;
        public int numSteps = 1;
        private float particle_size;

        public Cloth(int n, float dt, float d, int windowWidth, int windowHeigt)
        {
            this.N = n;
            this.dt = dt;
            this.d = d;
            this.windowWidth = windowWidth;
            this.windowHeigt = windowHeigt;

            this.particle_size = 0.01f;
            particles = new List<Particle>();
            forces = new List<Force>();
            constrains = new List<Constraint>();
            objects = new List<FixedObject>();
        }

        public void ClearData()
        {
            particles.ForEach(x => x.reset());
        }

        public void InitFlagSystem(float[] d, float[] u, float[] v)
        {
            this.particle_size = 0.015f;

            int size = 6;
            float dist = 0.1f;
            HyperPoint<float> start = new HyperPoint<float>(0.2f, 0.4f);
            HyperPoint<float> offset = new HyperPoint<float>(0.0f, 0.0f);

            createFlag(particles, forces, constrains, objects, start, offset, size, dist, d, u, v);
            //createCurtain(particles, forces, constrains, objects, start, offset, size, dist);
        }

        public void InitCurtainSystem(float[] d, float[] u, float[] v)
        {
            this.particle_size = 0.01f;

            HyperPoint<int> size = new HyperPoint<int>(5, 7);
            float dist = 0.05f;
            HyperPoint<float> start = new HyperPoint<float>(0.1f, 0.5f);
            HyperPoint<float> offset = new HyperPoint<float>(0.0f, 0.0f);

            createCurtain(particles, forces, constrains, objects, start, offset, size, dist, d, u, v);

            /*
            Particle particle = new Particle(0, new HyperPoint<float>(0.5f, 0.8f));
            particles.Add(particle);

            PressureForce pressureForce = new PressureForce(particle, dt, N, d, u, v);
            forces.Add(pressureForce);
            DragForce dragForce = new DragForce(particle);
            forces.Add(dragForce);
            */
        }

        private void createCurtain(List<Particle> particels, List<Force> forces, List<Constraint> constraints, List<FixedObject> objects,
            HyperPoint<float> start, HyperPoint<float> offset, HyperPoint<int> size, float dist, float[] d, float[] u, float[] v)
        {
            int index = 0;

            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                    start = start + new HyperPoint<float>(size.X * dist * i, 0) + new HyperPoint<float>(0.35f, 0);
                for (int y = 0; y < size.Y; y++)
                {
                    for (int x = 0; x < size.X; x++)
                    {
                        offset = new HyperPoint<float>(dist * x, dist * y);
                        Particle particle = new Particle(index, start + offset, 2.0f);
                        particles.Add(particle);

                        PressureForce pressureForce = new PressureForce(particle, dt, N, d, u, v);
                        forces.Add(pressureForce);
                        DragForce dragForce = new DragForce(particle);

                        forces.Add(dragForce);
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
                            forces.Add(createSpringForce(particle, particles[index - 2], dist * 2));
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
                                                                  (float)Math.Sqrt(2 * dist * dist)));
                                //constrains.Add(new RodConstraint(particle, particles[index - size - 1], (float)Math.Sqrt(2 * dist * dist)));
                            }
                            if (x < (size.X - 1))
                            {
                                forces.Add(createStiffSpringForce(particle, particles[index - size.X + 1],
                                                                  (float)Math.Sqrt(2 * dist * dist)));
                                //constrains.Add(new RodConstraint(particle, particles[index - size + 1], (float)Math.Sqrt(2 * dist * dist)));
                            }
                        }
                        if (y > 1)
                        {
                            //Long spring to particle 2 to above
                            forces.Add(createSpringForce(particle, particles[index - (size.X * 2)], dist * 2));
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

            objects.Add(new VerLineObject((0.5f)/N, 0, 1, true));
            objects.Add(new VerLineObject(1 - (0.5f) / N, 0, 1, false));
        }

        private void createFlag(List<Particle> particels, List<Force> forces, List<Constraint> constraints, List<FixedObject> objects,
            HyperPoint<float> start, HyperPoint<float> offset, int size, float dist, float[] d, float[] u, float[] v)
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

                    PressureForce pressureForce = new PressureForce(particle, dt, N, d, u, v);
                    forces.Add(pressureForce);
                    DragForce dragForce = new DragForce(particle);

                    forces.Add(dragForce);
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

        public void OnRenderFrame()
        {
            DrawForces();
            DrawConstraints();
            DrawParticles();
            DrawObjects();
        }

        public void OnUpdateFrame()
        {
            for (int i = 0; i < numSteps; i++)
                ParticleSolver.SimulationStep(particles, forces, constrains, objects, particle_size, dt, integrationMode);
        }

        public void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.KeyCode)
            {
                case Keys.NumPad6:
                    numSteps += 1;
                    break;
                case Keys.NumPad4:
                    numSteps -= 1;
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

                case Keys.Q:
                    break;
                case Keys.I:
                    integrationMode = (integrationMode + 1) % 3;
                    break;
            }
        }

        public bool OnMouseDown(MouseEventArgs e)
        {
            float mouseX = (float)(e.Location.X / (float)windowWidth);
            float mouseY = 1 - (float)(e.Location.Y / (float)windowHeigt);

            if (e.Button == MouseButtons.Left)
            {
                if (this.currentSelectedParticle != null || mouseSpringForce != null)
                    return false;

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
                    return true;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                /*
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
                */
            }

            return false;
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            float mouseX = (float)(e.Location.X / (float)windowWidth);
            float mouseY = 1 - (float)(e.Location.Y / (float)windowHeigt);

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
                /*
                forces.RemoveAll(F => F.GetType() == typeof(HorizontalForce));
                hor_force_applied = false;
                */
            }
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            float mouseX = (float)(e.Location.X / (float)windowWidth);
            float mouseY = 1 - (float)(e.Location.Y / (float)windowHeigt);
            HyperPoint<float> mouseLoc = new HyperPoint<float>(mouseX, mouseY);

            if (this.mouseParticle != null)
            {
                mouseParticle.Position = mouseLoc;
            }

            /*
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
            */
        }

        public void OnResize(int windowWidth, int windowHeigt)
        {
            this.windowWidth = windowWidth;
            this.windowHeigt = windowHeigt;
        }
    }
}