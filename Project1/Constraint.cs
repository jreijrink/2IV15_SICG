﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Project1
{
    abstract class Constraint
    {
        abstract public void Calculate();

        abstract public void Draw();
	}
}
