using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluidsProject.Objects;
using FluidsProject.Particles;
using micfort.GHL.Math2;

namespace FluidsProject
{
    class Solver
    {
        static float gravity = -9.81f / 100.0f;
        private static BoundryConditions[] boundaries;

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

        private static void lin_solve(int N, int b, float[] x, float[] x0, float[] o, List<MovingObject> objects, float a, float c, List<RigidBody> bodies)
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

                set_bnd(N, b, x, o, objects, bodies);
            }
        }

        private static void diffuse(int N, int b, float[] x, float[] x0, float[] o, List<MovingObject> objects, float diff, float dt, List<RigidBody> bodies)
        {
            float a = dt * diff * N * N;
            lin_solve(N, b, x, x0, o, objects, a, 1 + 4 * a, bodies);
        }

        private static void advect(int N, int b, float[] d, float[] d0, float[] u, float[] v, float[] o, List<MovingObject> objects, float dt, List<RigidBody> bodies)
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
                    d[IX(i, j)] = s0 * (t0 * d0[IX(i0, j0)] + t1 * d0[IX(i0, j1)]) + s1 * (t0 * d0[IX(i1, j0)] + t1 * d0[IX(i1, j1)]);
                }
            }
            set_bnd(N, b, d, o, objects, bodies);
        }

        private static void project(int N, float[] u, float[] v, float[] p, float[] div, float[] o, List<MovingObject> objects, List<RigidBody> bodies)
        {

            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= N; j++)
                {
                    div[IX(i, j)] = -0.5f * (u[IX(i + 1, j)] - u[IX(i - 1, j)] + v[IX(i, j + 1)] - v[IX(i, j - 1)]) / N;
                    p[IX(i, j)] = 0;
                }
            }
            set_bnd(N, 0, div, o, objects, bodies); set_bnd(N, 0, p, o, objects, bodies);

            lin_solve(N, 0, p, div, o, objects, 1, 4, bodies);

            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= N; j++)
                {
                    u[IX(i, j)] -= 0.5f * N * (p[IX(i + 1, j)] - p[IX(i - 1, j)]);
                    v[IX(i, j)] -= 0.5f * N * (p[IX(i, j + 1)] - p[IX(i, j - 1)]);
                }
            }
            set_bnd(N, 1, u, o, objects, bodies); set_bnd(N, 2, v, o, objects, bodies);
        }

        public static void dens_step(int N, float[] x, float[] x0, float[] u, float[] v, float[] o, List<MovingObject> objects, float diff, float dt, List<RigidBody> bodies)
        {
            add_source(N, x, x0, dt);
            SWAP(ref x0, ref x); diffuse(N, 0, x, x0, o, objects, diff, dt, bodies);
            SWAP(ref x0, ref x); advect(N, 0, x, x0, u, v, o, objects, dt, bodies);
        }

        public static void vel_step(int N, float[] u, float[] v, float[] u0, float[] v0, float[] o, List<MovingObject> objects, float visc, float dt, List<RigidBody> bodies)
        {
            //float[] g = grafity(N);
            //add_source(N, v, g, dt);
            add_source(N, u, u0, dt); add_source(N, v, v0, dt);
            SWAP(ref u0, ref u); diffuse(N, 1, u, u0, o, objects, visc, dt, bodies);
            SWAP(ref v0, ref v); diffuse(N, 2, v, v0, o, objects, visc, dt, bodies);
            project(N, u, v, u0, v0, o, objects, bodies);
            SWAP(ref u0, ref u); SWAP(ref v0, ref v);
            advect(N, 1, u, u0, u0, v0, o, objects, dt, bodies); advect(N, 2, v, v0, u0, v0, o, objects, dt, bodies);
            project(N, u, v, u0, v0, o, objects, bodies);
        }

        private static void SWAP(ref float[] x0, ref float[] p1)
        {
            float[] temp = x0;
            x0 = p1;
            p1 = temp;
        }

        public static void initialize_boundaries(int N)
        {
            boundaries = new BoundryConditions[(N + 2) * (N + 2)];
            for (int i = 0; i < boundaries.Count(); i++)
            {
                boundaries[i] = new BoundryConditions();
            }
        }

        private static void set_bnd(int N, int b, float[] x, float[] o, List<MovingObject> objects, List<RigidBody> bodies)
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

                    BoundrySettings bs = b == 1
                                                 ? boundaries[IX(i, j)].u
                                                 : b == 2 ? boundaries[IX(i, j)].v : boundaries[IX(i, j)].d;
                    Source source = boundaries[IX(i, j)].source;
                    if (bs == BoundrySettings.Copy)
                    {
                        if (source == Source.left) x[IX(i, j)] = x[IX(i - 1, j)];
                        if (source == Source.up) x[IX(i, j)] = x[IX(i, j + 1)];
                        if (source == Source.right) x[IX(i, j)] = x[IX(i + 1, j)];
                        if (source == Source.down) x[IX(i, j)] = x[IX(i, j - 1)];
                    }
                    else if (bs == BoundrySettings.Invert)
                    {
                        if (source == Source.left) x[IX(i, j)] = -x[IX(i - 1, j)];
                        if (source == Source.up) x[IX(i, j)] = -x[IX(i, j + 1)];
                        if (source == Source.right) x[IX(i, j)] = -x[IX(i + 1, j)];
                        if (source == Source.down) x[IX(i, j)] = -x[IX(i, j - 1)];
                    }
                    else if (bs == BoundrySettings.Zero)
                    {
                        if (source == Source.left) x[IX(i, j)] = 0;
                        if (source == Source.up) x[IX(i, j)] = 0;
                        if (source == Source.right) x[IX(i, j)] = 0;
                        if (source == Source.down) x[IX(i, j)] = 0;
                    }
                }
            }

            x[IX(0, 0)] = 0.5f * (x[IX(1, 0)] + x[IX(0, 1)]);
            x[IX(0, N + 1)] = 0.5f * (x[IX(1, N + 1)] + x[IX(0, N)]);
            x[IX(N + 1, 0)] = 0.5f * (x[IX(N, 0)] + x[IX(N + 1, 1)]);
            x[IX(N + 1, N + 1)] = 0.5f * (x[IX(N, N + 1)] + x[IX(N + 1, N)]);


        }

        public static void setBoundaryConditionsRigidBodies(List<RigidBody> bodies, int N)
        {
            foreach (RigidBody body in bodies)
            {
                int[] minMaxIJ = getMinMaxIJ(bodies.ConvertAll(b => b.getPosition()), N);

                int minI = minMaxIJ[0];
                int maxI = minMaxIJ[1];

                int minJ = minMaxIJ[2];
                int maxJ = minMaxIJ[3];

                for (int i = minI; i < maxI; i++)
                {
                    for (int j = minJ; j < maxJ; j++)
                    {
                        float xi = i / N;
                        float yj = i / N;

                        bool ijIn = body.pointInPolygon(new HyperPoint<float>(xi, yj));
                        if (ijIn)
                        {
                            float xLeft = (i - 1) / N;
                            float xRight = (i + 1) / N;
                            float yUp = (j + 1) / N;
                            float yDown = (j - 1) / N;

                            bool leftIn = body.pointInPolygon(new HyperPoint<float>(xLeft, yj));
                            bool rightIn = body.pointInPolygon(new HyperPoint<float>(xRight, yj));
                            bool upIn = body.pointInPolygon(new HyperPoint<float>(xi, yUp));
                            bool downIn = body.pointInPolygon(new HyperPoint<float>(xi, yDown));

                            if (leftIn && rightIn && downIn && upIn)
                            {
                                boundaries[IX(i, j)].u = BoundrySettings.Zero;
                                boundaries[IX(i, j)].v = BoundrySettings.Zero;
                                boundaries[IX(i, j)].d = BoundrySettings.Zero;
                                boundaries[IX(i, j)].source = Source.None;

                            }
                            if (leftIn && !rightIn)
                            {
                                boundaries[IX(i, j)].u = BoundrySettings.Invert;
                                boundaries[IX(i, j)].v = BoundrySettings.Copy;
                                boundaries[IX(i, j)].d = BoundrySettings.Copy;
                                boundaries[IX(i, j)].source = Source.right;
                            }
                            if (!leftIn && rightIn)
                            {
                                boundaries[IX(i, j)].u = BoundrySettings.Invert;
                                boundaries[IX(i, j)].v = BoundrySettings.Copy;
                                boundaries[IX(i, j)].d = BoundrySettings.Copy;
                                boundaries[IX(i, j)].source = Source.left;
                            }
                            if (!upIn && downIn)
                            {
                                boundaries[IX(i, j)].u = BoundrySettings.Copy;
                                boundaries[IX(i, j)].v = BoundrySettings.Invert;
                                boundaries[IX(i, j)].d = BoundrySettings.Copy;
                                boundaries[IX(i, j)].source = Source.up;
                            }
                            if (!upIn && downIn)
                            {
                                boundaries[IX(i, j)].u = BoundrySettings.Copy;
                                boundaries[IX(i, j)].v = BoundrySettings.Invert;
                                boundaries[IX(i, j)].d = BoundrySettings.Copy;
                                boundaries[IX(i, j)].source = Source.down;
                            }
                        }
                    }
                }

            }
        }

        private static int[] getMinMaxIJ(List<HyperPoint<float>> points, int N)
        {
            int minI = int.MaxValue;
            int maxI = int.MinValue;

            int minJ = int.MaxValue;
            int maxJ = int.MinValue;

            foreach (HyperPoint<float> p in points)
            {
                int i = (int)(p.X * N);
                int j = (int)(p.Y * N);

                if (i < minI)
                {
                    minI = i;
                }
                if (i > maxI)
                {
                    maxI = i;
                }
                if (j < minJ)
                {
                    minJ = j;
                }
                if (j > maxJ)
                {
                    maxJ = j;
                }
            }

            return new[] {minI,maxI,minJ,maxJ};
        }

        public static void setBoundaryConditionsSolidObjects(List<MovingObject> objects, int N)
        {
            foreach (MovingObject movingObject in objects)
            {
                int[] minMaxIJ = getMinMaxIJ(movingObject.GetVertices(), N);

                int minI = minMaxIJ[0];
                int maxI = minMaxIJ[1];

                int minJ = minMaxIJ[2];
                int maxJ = minMaxIJ[3];

                for (int i = minI; i < maxI; i++)
                {
                    for (int j = minJ; j < maxJ; j++)
                    {
                        float xi = i / N;
                        float yj = i / N;

                        bool ijIn = movingObject.pointInObject(new HyperPoint<float>(xi, yj));
                        if (ijIn)
                        {
                            float xLeft = (i - 1) / N;
                            float xRight = (i + 1) / N;
                            float yUp = (j + 1) / N;
                            float yDown = (j - 1) / N;

                            bool leftIn = movingObject.pointInObject(new HyperPoint<float>(xLeft, yj));
                            bool rightIn = movingObject.pointInObject(new HyperPoint<float>(xRight, yj));
                            bool upIn = movingObject.pointInObject(new HyperPoint<float>(xi, yUp));
                            bool downIn = movingObject.pointInObject(new HyperPoint<float>(xi, yDown));

                            if (leftIn && rightIn && downIn && upIn)
                            {
                                boundaries[IX(i, j)].u = BoundrySettings.Zero;
                                boundaries[IX(i, j)].v = BoundrySettings.Zero;
                                boundaries[IX(i, j)].d = BoundrySettings.Zero;
                                boundaries[IX(i, j)].source = Source.None;

                            }
                            if (leftIn && !rightIn)
                            {
                                boundaries[IX(i, j)].u = BoundrySettings.Invert;
                                boundaries[IX(i, j)].v = BoundrySettings.Copy;
                                boundaries[IX(i, j)].d = BoundrySettings.Copy;
                                boundaries[IX(i, j)].source = Source.right;
                            }
                            if (!leftIn && rightIn)
                            {
                                boundaries[IX(i, j)].u = BoundrySettings.Invert;
                                boundaries[IX(i, j)].v = BoundrySettings.Copy;
                                boundaries[IX(i, j)].d = BoundrySettings.Copy;
                                boundaries[IX(i, j)].source = Source.left;
                            }
                            if (!upIn && downIn)
                            {
                                boundaries[IX(i, j)].u = BoundrySettings.Copy;
                                boundaries[IX(i, j)].v = BoundrySettings.Invert;
                                boundaries[IX(i, j)].d = BoundrySettings.Copy;
                                boundaries[IX(i, j)].source = Source.up;
                            }
                            if (!upIn && downIn)
                            {
                                boundaries[IX(i, j)].u = BoundrySettings.Copy;
                                boundaries[IX(i, j)].v = BoundrySettings.Invert;
                                boundaries[IX(i, j)].d = BoundrySettings.Copy;
                                boundaries[IX(i, j)].source = Source.down;
                            }
                        }
                    }
                }
            }
        }

        private static void setBoundaryConditionsSolidObjects2(int N, int b, float[] x, List<MovingObject> objects)
        {
            int i, j;
            foreach (MovingObject movingObject in objects)
            {
                for (i = 2; i <= N - 1; i++)
                {
                    for (j = 2; j <= N - 1; j++)
                    {
                        if (movingObject.IsObjectCell(i, j))
                        {
                            x[IX(i, j)] = b == 1
                                              ? movingObject.GetVelocityX(i, j, x)
                                              : (b == 2)
                                                    ? movingObject.GetVelocityY(i, j, x)
                                                    : movingObject.GetVelocityDensity(i, j, x);
                        }
                    }
                }
            }
        }

        private static float[] grafity(int N)
        {
            int size = (N + 2) * (N + 2);
            float[] g = new float[size];

            int i, j;
            for (i = 2; i <= N - 1; i++)
            {
                for (j = 2; j <= N - 1; j++)
                {
                    g[IX(i, j)] = gravity;
                }
            }

            return g;
        }
    }

    class BoundryConditions
    {
        public BoundrySettings u = BoundrySettings.None;
        public BoundrySettings v = BoundrySettings.None;
        public BoundrySettings d = BoundrySettings.None;

        public Source source;
    }

    internal enum BoundrySettings
    {
        Zero,
        Copy,
        Invert,
        None
    };

    internal enum Source
    {
        left,
        right,
        up,
        down,
        None
    };
}
