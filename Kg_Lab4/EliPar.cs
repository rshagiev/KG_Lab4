using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kg_Lab4;

namespace Kg_Lab4
{
    class EliPar
    {
        double a = 1, b = 1;
        public double A
        {
            set
            {
                if (value != a) a = value;
            }
            get
            {
                return a;
            }
        }
        public double B
        {
            set
            {
                if (value != b) b = value;
            }
            get
            {
                return b;
            }
        }

        public double ElipPar(double x, double y)
        {
            return Math.Pow(x, 2) / (A * A) + Math.Pow(y, 2) / (B * B);
        }
        public double F(double x, double y, double z)
        {
            //double r = Math.Sqrt(x * x + y * y);
            double f = ElipPar(x, y);
            return f - z;
        }

        public Form1.Vertex CalculateVertex(double x, double y)
        {
            Form1.Vertex resault = new Form1.Vertex();

            //double r = Math.Sqrt(x * x + y * y);
            double z = ElipPar(x, y);

            double delta = 1e-6;

            float f = (float)F(x, y, z);

            double dfdx = -(F(x + delta, y, z) - f) / delta;
            double dfdy = -(F(x, y + delta, z) - f) / delta;
            double dfdz = 1;

            double invLen = 1 / Math.Sqrt(dfdx * dfdx + dfdy * dfdy + dfdz * dfdz);

            resault.x = (float)x;
            resault.y = (float)y;
            resault.z = (float)z;

            resault.nx = (float)(dfdx * invLen);
            resault.ny = (float)(dfdy * invLen);
            resault.nz = (float)(dfdz * invLen);

            return resault;
        }
    }
}
