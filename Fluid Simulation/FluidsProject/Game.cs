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

namespace FluidsProject
{
    public class Game
    {
        private static int N;
        private float dt, diff, visc;
        private float force, source;
        private bool dvel;

        private float[] u, v, u_prev, v_prev, o;
        private List<MovingObject> objects;
        private float[] dens, dens_prev;

        private int win_id;
        private int win_x, win_y;
        private bool[] mouse_down = { false, false, false };
        private int omx, omy, mx, my;
        private Rectangle drawWindow;
        private float gravity = -0.01f;//-9.81f / 100.0f;

        private List<RigidBody> rigids = new List<RigidBody>();
 
        public void init(int width, int height, string[] args)
        {
            win_x = width;
            win_y = height;

            initConfiguration(args);
            allocate_data();
            create_solid_object();

            create_bodies();
            PreDisplay();
        }

        private void create_bodies()
        {
            Box box = new Box(new HyperPoint<float>(0,0), 100, 10.0f/N, 10.0f/N);
            rigids.Add(box);
        }

        private void initConfiguration(string[] args)
        {
            if (args.Length == 0)
            {
                N = 64;
                dt = 0.1f;
                diff = 0.0f;
                visc = 0.0f;
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
            SquareObject square = new SquareObject(N / 2, N / 2, N / 4, N / 4, N);
            objects.Add(square);
        }

        public void OnUpdateFrame()
        {
            get_from_UI(dens_prev, u_prev, v_prev);
            //apply_grafity();

            foreach (MovingObject movingObject in objects)
            {
                //movingObject.UpdatePosition();
            }

            Solver.vel_step(N, u, v, u_prev, v_prev, o, objects, visc, dt);
            Solver.dens_step(N, dens, dens_prev, u, v, o, objects, diff, dt);
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
                //drawBodies();
            }
        }

        private void drawBodies()
        {
            foreach (RigidBody body in rigids)
            {
                //body.draw();
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

            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.LineWidth(1.0f);

            GL.Begin(BeginMode.Lines);

            for (i = 1; i <= N; i++)
            {
                x = (i - 0.5f) * h;
                for (j = 1; j <= N; j++)
                {
                    y = (j - 0.5f) * h;

                    GL.Vertex2(x, y);
                    GL.Vertex2(x + u[IX(i, j)], y + v[IX(i, j)]);
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
            GL.Color3(0, 0, 0);
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

            for (i = 1; i <= N; i++)
            {
                x1 = (i - 0.5f) * h;
                x2 = (i + 0.5f) * h;
                for (j = 1; j <= N; j++)
                {
                    y1 = (j - 0.5f) * h;
                    y2 = (j + 0.5f) * h;

                    if (o[IX(i, j)] == 1)
                    {
                        GL.Color3(1.0f, 1.0f, 0.0f);

                        GL.Vertex2(x1 - 1 * h, y1 - 1 * h);
                        GL.Vertex2(x2 + 1 * h, y1 - 1 * h);
                        GL.Vertex2(x2 + 1 * h, y2 + 1 * h);
                        GL.Vertex2(x1 - 1 * h, y2 + 1 * h);
                    }
                }
            }

            foreach (MovingObject movingObject in objects)
            {
                movingObject.Draw();
            }

            GL.End();
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
                    GL.Color3(0, d00 * .3, d00); GL.Vertex2(x, y);
                    GL.Color3(0, d10 * .3, d10); GL.Vertex2(x + h, y);
                    GL.Color3(0, d11 * .3, d11); GL.Vertex2(x + h, y + h);
                    GL.Color3(0, d01 * .3, d01); GL.Vertex2(x, y + h);
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
            create_solid_object();
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

            GL.Viewport(0, 0, win_x, win_y);
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                    case Keys.C:
                        clear_data();
                        break;
                    case Keys.V:
                        dvel = !dvel;
                        break;
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
        }

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            omx = mx = e.Location.X;
            omy = my = e.Location.Y;

            if (e.Button == MouseButtons.Left)
                mouse_down[0] = true;
            if (e.Button == MouseButtons.Right)
                mouse_down[2] = true;
            if (e.Button == MouseButtons.Middle)
                mouse_down[1] = true;
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
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            mx = e.X;
            my = e.Y;
        }
    }
}
