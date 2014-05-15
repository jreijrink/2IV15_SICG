using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace Project1
{
    public static class MatrixExtensions
    {
        public static Matrix<double> Inverse(this Matrix<double> matrix)
        {
            int n = matrix.Rows;
            //e will represent each column in the identity matrix
            double[] e;
            //x will hold the inverse matrix to be returned
            Matrix<double> x = new Matrix<double>(matrix.Rows, matrix.Columns);

            /*
            * solve will contain the vector solution for the LUP decomposition as we solve
            * for each vector of x.  We will combine the solutions into the double[][] array x.
            * */
            double[] solve;

            //Get the LU matrix and P matrix (as an array)
            Tuple<Matrix<double>, int[]> results = LUPDecomposition(matrix);

            Matrix<double> LU = results.Item1;
            int[] P = results.Item2;

            /*
            * Solve AX = e for each column ei of the identity matrix using LUP decomposition
            * */
            for (int i = 0; i < n; i++)
            {
                e = new double[matrix.Columns];
                e[i] = 1;
                solve = LUPSolve(LU, P, e);
                for (int j = 0; j < solve.Length; j++)
                {
                    x[j, i] = solve[j];
                }
            }
            return x;
        }

        public static Matrix<double> Transpose(this Matrix<double> matrix)
        {
            Matrix<double> result = new Matrix<double>(matrix.Columns, matrix.Rows);

            for (int n = 0; n < matrix.Columns; n++)
            {
                for (int m = 0; m < matrix.Rows; m++)
                {
                    result[n, m] = matrix[m, n];
                }
            }

            return result;
        }

        public static Matrix<double> Add( this Matrix<double> m1, Matrix<double> m2)
        {
            Matrix<double> result = new Matrix<double>(m1.Rows, m1.Columns);

            for (int i = 0; i < m1.Rows; i++)
            {
                for (int j = 0; j < m1.Columns; j++)
                {
                    result[i, j] = m1[i, j] + m2[i, j];
                }
            }

            return result;
        }

        private static Tuple<Matrix<double>, int[]> LUPDecomposition(Matrix<double> A)
        {
            int n = A.Rows - 1;
            /*
            * pi represents the permutation matrix.  We implement it as an array
            * whose value indicates which column the 1 would appear.  We use it to avoid 
            * dividing by zero or small numbers.
            * */
            int[] pi = new int[n + 1];
            double p = 0;
            int kp = 0;
            int pik = 0;
            int pikp = 0;
            double aki = 0;
            double akpi = 0;

            //Initialize the permutation matrix, will be the identity matrix
            for (int j = 0; j <= n; j++)
            {
                pi[j] = j;
            }

            for (int k = 0; k <= n; k++)
            {
                /*
                * In finding the permutation matrix p that avoids dividing by zero
                * we take a slightly different approach.  For numerical stability
                * We find the element with the largest 
                * absolute value of those in the current first column (column k).  If all elements in
                * the current first column are zero then the matrix is singluar and throw an
                * error.
                * */
                p = 0;
                for (int i = k; i <= n; i++)
                {
                    if (Math.Abs(A[i, k]) > p)
                    {
                        p = Math.Abs(A[i, k]);
                        kp = i;
                    }
                }
                if (p == 0)
                {
                    throw new Exception("singular matrix");
                }
                /*
                * These lines update the pivot array (which represents the pivot matrix)
                * by exchanging pi[k] and pi[kp].
                * */
                pik = pi[k];
                pikp = pi[kp];
                pi[k] = pikp;
                pi[kp] = pik;

                /*
                * Exchange rows k and kpi as determined by the pivot
                * */
                for (int i = 0; i <= n; i++)
                {
                    aki = A[k, i];
                    akpi = A[kp, i];
                    A[k, i] = akpi;
                    A[kp, i] = aki;
                }

                /*
                    * Compute the Schur complement
                    * */
                for (int i = k + 1; i <= n; i++)
                {
                    A[i, k] = A[i, k] / A[k, k];
                    for (int j = k + 1; j <= n; j++)
                    {
                        A[i, j] = A[i, j] - (A[i, k] * A[k, j]);
                    }
                }
            }
            return Tuple.Create(A, pi);
        }

        private static double[] LUPSolve(Matrix<double> LU, int[] pi, double[] b)
        {
            int n = LU.Rows - 1;
            double[] x = new double[n + 1];
            double[] y = new double[n + 1];
            double suml = 0;
            double sumu = 0;
            double lij = 0;

            /*
            * Solve for y using formward substitution
            * */
            for (int i = 0; i <= n; i++)
            {
                suml = 0;
                for (int j = 0; j <= i - 1; j++)
                {
                    /*
                    * Since we've taken L and U as a singular matrix as an input
                    * the value for L at index i and j will be 1 when i equals j, not LU[i][j], since
                    * the diagonal values are all 1 for L.
                    * */
                    if (i == j)
                    {
                        lij = 1;
                    }
                    else
                    {
                        lij = LU[i, j];
                    }
                    suml = suml + (lij * y[j]);
                }
                y[i] = b[pi[i]] - suml;
            }
            //Solve for x by using back substitution
            for (int i = n; i >= 0; i--)
            {
                sumu = 0;
                for (int j = i + 1; j <= n; j++)
                {
                    sumu = sumu + (LU[i, j] * x[j]);
                }
                x[i] = (y[i] - sumu) / LU[i, i];
            }
            return x;
        }
    }   
}
