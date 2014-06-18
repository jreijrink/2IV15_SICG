using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using OpenTK.Graphics.OpenGL;

namespace FluidsProject.Particles
{
    public static class MatrixExtensions
    {
        public static Matrix<float> Transpose(this Matrix<float> matrix)
        {
            Matrix<float> result = new Matrix<float>(matrix.Columns, matrix.Rows);

            for (int n = 0; n < matrix.Columns; n++)
            {
                for (int m = 0; m < matrix.Rows; m++)
                {
                    result[n, m] = matrix[m, n];
                }
            }

            return result;
        }

        public static Matrix<float> Add( this Matrix<float> m1, Matrix<float> m2)
        {
            Matrix<float> result = new Matrix<float>(m1.Rows, m1.Columns);

            for (int i = 0; i < m1.Rows; i++)
            {
                for (int j = 0; j < m1.Columns; j++)
                {
                    result[i, j] = m1[i, j] + m2[i, j];
                }
            }

            return result;
        }
    }   
}
