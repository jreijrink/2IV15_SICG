using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidsProject
{
    abstract class MovingObject
    {
        abstract public void Draw();

        abstract public void UpdatePosition();

        abstract public bool IsObjectCell(int x, int y);

        abstract public float GetVelocityX(int x, int y, float[] u);

        abstract public float GetVelocityY(int x, int y, float[] v);
    }
}
