using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidsProject
{
    class Solver
    {
        public static int IX(int i, int j)
        {
            int index = Game.IX(i, j);
            return index;
        }

        public static void add_source(int N, float[] x, float[] s, float dt)
        {
            int size = (N + 2) * (N + 2);
            for (int i = 0; i < size; i++)
                x[i] += dt * s[i];
        }

        private static void lin_solve(int N, int b, float[] x, float[] x0, float[] o, List<MovingObject> objects, float a, float c)
        {
            for (int k = 0; k < 20; k++)
            {
                for (int i = 1; i <= N; i++)
                {
                    for (int j = 1; j <= N; j++)
                    {
                        x[IX(i, j)] = (x0[IX(i, j)] + a * (x[IX(i - 1, j)] + x[IX(i + 1, j)] + x[IX(i, j - 1)] + x[IX(i, j + 1)])) / c;
                    }
                }

                set_bnd(N, b, x, o, objects);
            }
        }

        private static void diffuse(int N, int b, float[] x, float[] x0, float[] o, List<MovingObject> objects, float diff, float dt)
        {
            float a = dt * diff * N * N;
            lin_solve(N, b, x, x0, o, objects, a, 1 + 4 * a);
        }

        private static void advect(int N, int b, float[] d, float[] d0, float[] u, float[] v, float[] o, List<MovingObject> objects, float dt)
        {
            int i0, j0, i1, j1;
            float x, y, s0, t0, s1, t1, dt0;

            dt0 = dt * N;
            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= N; j++)
                {
                    x = i - dt0 * u[IX(i, j)];
                    y = j - dt0 * v[IX(i, j)];
                    if (x < 0.5f) x = 0.5f;
                    if (x > N + 0.5f) x = N + 0.5f;
                    i0 = (int)x;
                    i1 = i0 + 1;
                    if (y < 0.5f) y = 0.5f;
                    if (y > N + 0.5f) y = N + 0.5f;
                    j0 = (int)y;
                    j1 = j0 + 1;
                    s1 = x - i0;
                    s0 = 1 - s1;
                    t1 = y - j0;
                    t0 = 1 - t1;
                    d[IX(i, j)] = s0 * (t0 * d0[IX(i0, j0)] + t1 * d0[IX(i0, j1)]) +
                                  s1 * (t0 * d0[IX(i1, j0)] + t1 * d0[IX(i1, j1)]);
                }
            }
            set_bnd(N, b, d, o, objects);
        }

        private static void project(int N, float[] u, float[] v, float[] p, float[] div, float[] o, List<MovingObject> objects)
        {

            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= N; j++)
                {
                    div[IX(i, j)] = -0.5f * (u[IX(i + 1, j)] - u[IX(i - 1, j)] + v[IX(i, j + 1)] - v[IX(i, j - 1)]) / N;
                    p[IX(i, j)] = 0;
                }
            }
            set_bnd(N, 0, div, o, objects); set_bnd(N, 0, p, o, objects);

            lin_solve(N, 0, p, div, o, objects, 1, 4);

            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= N; j++)
                {
                    u[IX(i, j)] -= 0.5f * N * (p[IX(i + 1, j)] - p[IX(i - 1, j)]);
                    v[IX(i, j)] -= 0.5f * N * (p[IX(i, j + 1)] - p[IX(i, j - 1)]);
                }
            }
            set_bnd(N, 1, u, o, objects); set_bnd(N, 2, v, o, objects);
        }

        public static void dens_step(int N, float[] x, float[] x0, float[] u, float[] v, float[] o, List<MovingObject> objects, float diff, float dt)
        {
            add_source(N, x, x0, dt);
            SWAP(ref x0, ref x); diffuse(N, 0, x, x0, o, objects, diff, dt);
            SWAP(ref x0, ref x); advect(N, 0, x, x0, u, v, o, objects, dt);
        }

        public static void vel_step(int N, float[] u, float[] v, float[] u0, float[] v0, float[] o, List<MovingObject> objects, float visc, float dt)
        {   
            add_source ( N, u, u0, dt ); add_source ( N, v, v0, dt ); 
            SWAP(ref u0, ref u); diffuse(N, 1, u, u0, o, objects, visc, dt);
            SWAP(ref v0, ref v); diffuse(N, 2, v, v0, o, objects, visc, dt);
            project(N, u, v, u0, v0, o, objects);
            SWAP(ref u0, ref u); SWAP(ref v0, ref v);
            advect(N, 1, u, u0, u0, v0, o, objects, dt); advect(N, 2, v, v0, u0, v0, o, objects, dt);
            project(N, u, v, u0, v0, o, objects);
        }

        private static void SWAP(ref float[] x0, ref float[] p1)
        {
            float[] temp = x0;
            x0 = p1;
            p1 = temp;
        }

        private static void set_bnd(int N, int b, float[] x, float[] o, List<MovingObject> objects)
        {
            int i, j;

            for (i = 1; i <= N; i++)
            {
                x[IX(0, i)] = b == 1 ? -x[IX(1, i)] : x[IX(1, i)];
                x[IX(N + 1, i)] = b == 1 ? -x[IX(N, i)] : x[IX(N, i)];
                x[IX(i, 0)] = b == 2 ? -x[IX(i, 1)] : x[IX(i, 1)];
                x[IX(i, N + 1)] = b == 2 ? -x[IX(i, N)] : x[IX(i, N)];
            }

            for (i = 1; i <= N; i++)
            {
                for (j = 1; j <= N; j++)
                {
                    if (o[IX(i, j)] == 1)
                    {
                        //Center
                        x[IX(i, j)] = 0;
                        //Left, invert x
                        x[IX(i - 1, j)] = b == 1 ? -x[IX(i - 2, j)] : x[IX(i - 2, j)];
                        //Right, invert x
                        x[IX(i + 1, j)] = b == 1 ? -x[IX(i + 2, j)] : x[IX(i + 2, j)];

                        //Bottom, invert y
                        x[IX(i, j - 1)] = b == 2 ? -x[IX(i, j - 2)] : x[IX(i, j - 2)];
                        //Top, invert y
                        x[IX(i, j + 1)] = b == 2 ? -x[IX(i, j + 2)] : x[IX(i, j + 2)];
                    }
                }
            }

            foreach (MovingObject movingObject in objects)
            {
                for (i = 2; i <= N - 1; i++)
                {
                    for (j = 2; j <= N - 1; j++)
                    {
                        if (movingObject.IsObjectCell(i, j))
                        {
                            /*
                            float u = movingObject.GetVelocityX();
                            float v = movingObject.GetVelocityY();
                            */

                            x[IX(i, j)] = b == 1 ? movingObject.GetVelocityX(i, j, x) : (b == 2) ? movingObject.GetVelocityY(i, j, x) : x[IX(i, j)];

                            /*
                            //Center
                            x[IX(i, j)] = 0;

                            //Left, invert x
                            x[IX(i - 1, j)] = b == 1 ? -x[IX(i - 2, j)] : x[IX(i - 2, j)];
                            //Right, invert x
                            x[IX(i + 1, j)] = b == 1 ? -x[IX(i + 2, j)] : x[IX(i + 2, j)];

                            //Bottom, invert y
                            x[IX(i, j - 1)] = b == 2 ? -x[IX(i, j - 2)] + v : x[IX(i, j - 2)];
                            //Top, invert y
                            x[IX(i, j + 1)] = b == 2 ? -x[IX(i, j + 2)] + v : x[IX(i, j + 2)];
                            */
                        }
                    }
                }
            }

            x[IX(0, 0)] = 0.5f * (x[IX(1, 0)] + x[IX(0, 1)]);
            x[IX(0, N + 1)] = 0.5f * (x[IX(1, N + 1)] + x[IX(0, N)]);
            x[IX(N + 1, 0)] = 0.5f * (x[IX(N, 0)] + x[IX(N + 1, 1)]);
            x[IX(N + 1, N + 1)] = 0.5f * (x[IX(N, N + 1)] + x[IX(N + 1, N)]);
        }
    }
}
