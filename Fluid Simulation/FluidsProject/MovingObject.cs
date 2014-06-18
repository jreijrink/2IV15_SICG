using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using micfort.GHL.Math2;

namespace FluidsProject
{
    abstract class MovingObject
    {
        abstract public void Draw();

        abstract public void UpdatePosition();

        abstract public bool IsObjectCell(int x, int y);

        abstract public float GetVelocityX(int x, int y, float[] u);
        
        abstract public void SetVelocity(float u, float v);

        abstract public void SetPosition(float x, float y);

        abstract public float GetVelocityY(int x, int y, float[] v);

        abstract public float GetVelocityDensity(int x, int y, float[] d);

        abstract public List<HyperPoint<float>> GetVertices();

        abstract public bool pointInObject(HyperPoint<float> point);
    }
}
