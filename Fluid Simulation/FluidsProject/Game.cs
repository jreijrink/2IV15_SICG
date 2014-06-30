using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluidsProject.Objects;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using micfort.GHL.Math2;
using FluidsProject.Particles;

namespace FluidsProject
{
    public class Game
    {
        private static int N;
        private float dt, diff, visc;
        private float force, source;
        private bool dvel, show_v;
        private MovingObject on_movingObject;
        private int current_stage = 1;

        float d;
        private Cloth cloth;

        private float[] u, v, u_prev, v_prev, o;
        private List<MovingObject> objects;
        private float[] dens, dens_prev;

        private int win_x, win_y;
        private bool[] mouse_down = { false, false, false };
        private int omx, omy, mx, my;
        private float gravity = -9.81f / 100.0f;

        private List<RigidBody> rigids = new List<RigidBody>();

        public void init(int width, int height, string[] args)
        {
            win_x = width;
            win_y = height;

            initConfiguration(args);
            allocate_data();

            d = 5.0f;
            cloth = new Cloth(N, dt, d, win_x, win_y);

            PreDisplay();
        }   

        private void create_bodies()
        {
            float h = 1.0f / N;
            float x0 = (N/2.0f) * h;
            float y0 = (N/2.0f) * h;

            Box box = new Box(new HyperPoint<float>(x0, y0), 100, 10.0f * h, 10.0f * h, N);
            rigids.Add(box);
        }

        private void initConfiguration(string[] args)
        {
            if (args.Length == 0)
            {
                N = 64;
                dt = 0.05f;
                diff = 0.0f;
                visc = 0.001f;
                force = 5.0f;
                source = 100.0f;
                Console.WriteLine("Using defaults : N={0} dt={1} diff={2} visc={3} force = {4} source={5}",
                    N, dt, diff, visc, force, source);
            }
            else
            {
                N = int.Parse(args[0]);
                dt = float.Parse(args[1]);
                diff = float.Parse(args[2]);
                visc = float.Parse(args[3]);
                force = float.Parse(args[4]);
                source = float.Parse(args[5]);
                Console.WriteLine("Using defaults : N={0} dt={1} diff={2} visc={3} force = {4} source={5}",
                    N, dt, diff, visc, force, source);
            }   
        }

        private void allocate_data()
        {
            int size = (N + 2) * (N + 2);

            u = new float[size];
            v = new float[size];
            u_prev = new float[size];
            v_prev = new float[size];
            dens = new float[size];
            dens_prev = new float[size];
            o = new float[size];
            objects = new List<MovingObject>();
        }

        private void apply_grafity()
        {
            int i, j;
            for (i = 0; i <= N + 1; i++)
            {
                for (j = 0; j <= N + 1; j++)
                {
                    v[IX(i, j)] = gravity;
                }
            }
        }

        private void create_solid_object()
        {
            SquareObject square = new SquareObject(N / 4, N / 4, N / 4, N / 4, N);
            objects.Add(square);
            square.SetVelocity(0,1f);
        }

        public void OnUpdateFrame()
        {
            
            foreach (MovingObject ob in objects)
            {
                ob.UpdatePosition();
            }
            get_from_UI(dens_prev, u_prev, v_prev);
            Solver.initialize_boundaries(N);
            //Solver.add_rigid_velocity(rigids, u_prev, v_prev, N);

            Solver.setBoundaryConditionsSolidObjects2(N, 1, u, objects);
            Solver.setBoundaryConditionsSolidObjects2(N, 2, v, objects);
            Solver.setBoundaryConditionsSolidObjects2(N, 0, dens, objects);

            //Solver.setBoundaryConditionsSolidObjects(objects, N);
            Solver.setBoundaryConditionsRigidBodies(rigids, N);

            Solver.vel_step(N, u, v, u_prev, v_prev, o, visc, dt);
            Solver.dens_step(N, dens, dens_prev, u, v, o, diff, dt);

            foreach (RigidBody body in rigids)
            {
                body.update(dt, N, dens, u, v, o);
            }
            
            cloth.OnUpdateFrame();
        }

        private void loadStage()
        {
            switch (current_stage)
            {
                case 1:
                    loadStage1();
                    break;
                case 2:
                    loadStage2();
                    break;
                case 3:
                    loadStage3();
                    break;
                case 4:
                    loadStage4();
                    break;
                case 5:
                    loadStage5();
                    break;
            }
        }

        private void loadStage1()
        {
            //Nothing
            clear_data();
            current_stage = 1;
        }

        private void loadStage2()
        {
            //Fixed objects
            clear_data();
            for (int i = 0 / 4; i < N / 2; i++)
            {
                o[IX(i + N / 4, N / 4)] = 1;
                o[IX(i + N / 4, N - N / 4)] = 1;

                o[IX(N / 4, i + N / 4)] = 1;
                o[IX(N - N / 4, i + N / 4)] = 1;
            }
            current_stage = 2;
        }

        private void loadStage3()
        {
            //Moving solid object
            clear_data();
            create_solid_object();
            current_stage = 3;
        }

        private void loadStage4()
        {
            //Particles
            clear_data();
            cloth.InitCurtainSystem(dens, u, v);
            current_stage = 4;
        }

        private void loadStage5()
        {
            //Rigid body
            clear_data();
            create_bodies();
            current_stage = 5;
        }

        private void get_from_UI(float[] d, float[] u, float[] v)
        {
            int i, j, size = (N + 2) * (N + 2);

            for (i = 0; i < size; i++)
            {
                u[i] = v[i] = d[i] = 0.0f;
            }

            if (!mouse_down[0] && !mouse_down[2] && !mouse_down[1]) return;

            i = (int)((mx / (float)win_x) * N + 1);
            j = (int)(((win_y - my) / (float)win_y) * N + 1);

            if (i < 1 || i > N || j < 1 || j > N) return;

            if (mouse_down[0])
            {
                u[IX(i, j)] = force * (mx - omx);
                v[IX(i, j)] = force * (omy - my);
            }

            if (on_movingObject != null)
            {
                on_movingObject.SetPosition(i, j);
                //on_movingObject.SetVelocity((mx - omx), (omy - my));
            }

            if (mouse_down[1] && (j >= 2 && j < N) && (i >= 2 && i < N))
            {
                o[IX(i, j)] = 1;
            }

            if (mouse_down[2])
            {
                int index = IX(i, j);
                d[index] = source;
            }

            omx = mx;
            omy = my;
        }

        public void OnRenderFrame()
        {
            PreDisplay();
            if (dvel)
            {
                draw_velocities();
            }
            else
            {
                draw_density();
                draw_object();
                drawBoundry();
                drawBodies();
                if (show_v)
                    draw_velocities();

               cloth.OnRenderFrame();
            }
        }

        private void drawBodies()
        {
            foreach (RigidBody body in rigids)
            {
                body.draw();
            }
        }

        private void PreDisplay()
        {
            GL.Viewport(0, 0, win_x, win_y);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0, 1.0, 0.0, 1.0, -1, 1);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        private void draw_velocities()
        {
            int i, j;
            float x, y, h;

            h = 1.0f / N;

            GL.Color3(1.0f, 0.2f, 0.2f);
            GL.LineWidth(1.0f);

            GL.Begin(BeginMode.Lines);

            for (i = 1; i <= N; i++)
            {
                x = (i - 0.5f) * h;
                for (j = 1; j <= N; j++)
                {
                    y = (j - 0.5f) * h;

                    GL.Vertex2(x, y);
                    GL.Vertex2(x + 0.5f * u[IX(i, j)], y + 0.5f * v[IX(i, j)]);
                }
            }

            GL.End();
        }

        private void drawBoundry()
        {
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.LineWidth(1.0f);

            GL.Begin(BeginMode.Lines);
            
            drawLineForIJ(1, N, N, N);
            drawLineForIJ(N, N, N, 1);
            drawLineForIJ(N, 1, 1, 1);
            drawLineForIJ(1, 1, 1, N);

            GL.End();
        }

        private void drawLineForIJ(int i0, int j0, int i1, int j1)
        {
            float h = h = 1.0f / N;
            float x0 = (i0 - 0.5f) * h;
            float y0 = (j0 - 0.5f) * h;

            float x1 = (i1 - 0.5f) * h;
            float y1 = (j1 - 0.5f) * h;

            GL.Vertex2(x0, y0);
            GL.Vertex2(x1, y1);
        }

        private void draw_object()
        {
            int i, j;
            float x1, x2, y1, y2, h;

            h = 1.0f / N;

            GL.Color3(1.0f, 1.0f, 0.0f);
            GL.LineWidth(1.0f);

            GL.Begin(BeginMode.Quads);

            for (j = 1; j <= N; j++)
            {
                for (i = 1; i <= N; i++)
                {
                    x1 = (i - 0.5f) * h;
                    x2 = (i + 0.5f) * h;
                    y1 = (j - 0.5f) * h;
                    y2 = (j + 0.5f) * h;

                    if (o[IX(i, j)] == 1)
                    {

                        /*
                        GL.Color3(d00, d00, d00); GL.Vertex2(x, y);
                        GL.Color3(d10, d10, d10); GL.Vertex2(x + h, y);
                        GL.Color3(d11, d11, d11); GL.Vertex2(x + h, y + h);
                        GL.Color3(d01, d01, d01); GL.Vertex2(x, y + h);
                        */

                        GL.Color3(0, 1, 0.6f); GL.Vertex2(x1 - 1 * h, y1 - 1 * h);
                        GL.Color3(0, 1, 0.1f); GL.Vertex2(x1 + 1 * h, y1 - 1 * h);
                        GL.Color3(0, 1, 0.1f); GL.Vertex2(x1 + 1 * h, y1 + 1 * h);
                        GL.Color3(0, 1, 0.6f); GL.Vertex2(x1 - 1 * h, y1 + 1 * h);

                        //GL.Color3(1.0f, 1.0f, 0.0f);
                    }
                }
            }
            GL.End();

            foreach (MovingObject movingObject in objects)
            {
                movingObject.Draw();
            }
        }

        private void draw_density()
        {
            int i, j;
            float x, y, h, d00, d01, d10, d11;

            h = 1.0f / N;

            GL.Begin(BeginMode.Quads);

            for (i = 1; i < N; i++)
            {
                x = (i - 0.5f) * h;
                for (j = 1; j < N; j++)
                {
                    y = (j - 0.5f) * h;

                    d00 = dens[IX(i, j)];
                    d01 = dens[IX(i, j + 1)];
                    d10 = dens[IX(i + 1, j)];
                    d11 = dens[IX(i + 1, j + 1)];

                    if(d00 > 0 || d01 > 0 || d10 > 0 || d11 > 1)
                    {
                        d00 = d00;
                    }
                    /*
                    GL.Color3(d00, d00, d00); GL.Vertex2(x, y);
                    GL.Color3(d10, d10, d10); GL.Vertex2(x + h, y);
                    GL.Color3(d11, d11, d11); GL.Vertex2(x + h, y + h);
                    GL.Color3(d01, d01, d01); GL.Vertex2(x, y + h);
                    */
                    //0, .5, 1
                    GL.Color3(d00 * .8, d00 * .8, d00); GL.Vertex2(x, y);
                    GL.Color3(d10 * .8, d10 * .8, d10); GL.Vertex2(x + h, y);
                    GL.Color3(d11 * .8, d11 * .8, d11); GL.Vertex2(x + h, y + h);
                    GL.Color3(d01 * .8, d01 * .8, d01); GL.Vertex2(x, y + h);
                }
            }

            GL.End();
        }

        private void clear_data()
        {
            int size = (N + 2) * (N + 2);

            for (int i = 0; i < size; i++)
            {
                u[i] = v[i] = u_prev[i] = v_prev[i] = dens[i] = dens_prev[i] = o[i] = 0.0f;
            }
            objects = new List<MovingObject>();
            cloth.ClearData();
            rigids = new List<RigidBody>();
        }

        public static int IX(int i, int j)
        {
            return ((i) + (N + 2) * (j));
        }

        public void OnResize(object sender, EventArgs e)
        {
            GLControl control = (GLControl)sender;
            win_x = control.ClientRectangle.Width;
            win_y = control.ClientRectangle.Height;
            
            cloth.OnResize(win_x, win_y);

            GL.Viewport(0, 0, win_x, win_y);
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                    case Keys.C:
                        clear_data();
                        loadStage();
                        break;
                    case Keys.V:
                        dvel = !dvel;
                        break;
                    case Keys.S:
                        show_v = !show_v;
                        break;
                    case Keys.D1:
                        loadStage1();
                        break;
                    case Keys.D2:
                        loadStage2();
                        break;
                    case Keys.D3:
                        loadStage3();
                        break;
                    case Keys.D4:
                        loadStage4();
                        break;
                    case Keys.D5:
                        loadStage5();
                        break;
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
        }

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (!cloth.OnMouseDown(e))
            {
                omx = mx = e.Location.X;
                omy = my = e.Location.Y;

                if (e.Button == MouseButtons.Left)
                    mouse_down[0] = true;
                if (e.Button == MouseButtons.Right)
                    mouse_down[2] = true;
                if (e.Button == MouseButtons.Middle)
                    mouse_down[1] = true;

                int i, j, size = (N + 2) * (N + 2);

                i = (int)((mx / (float)win_x) * N + 1);
                j = (int)(((win_y - my) / (float)win_y) * N + 1);

                foreach (MovingObject movingObject in objects)
                {
                    if (movingObject.IsObjectCell(i, j))
                    {
                        //movingObject.SetVelocity(0, 0);
                        //movingObject.SetPosition(i, j);
                        //on_movingObject = movingObject;
                    }
                }
            }
        }

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            omx = mx = e.Location.X;
            omx = my = e.Location.X;

            if (e.Button == MouseButtons.Left)
                mouse_down[0] = false;
            if (e.Button == MouseButtons.Right)
                mouse_down[2] = false;
            if (e.Button == MouseButtons.Middle)
                mouse_down[1] = false;
            on_movingObject = null;

            cloth.OnMouseUp(e);
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            mx = e.X;
            my = e.Y;

            cloth.OnMouseMove(e);
        }
    }
}
