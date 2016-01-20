using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmarterDashboard_Programmers
{

    
    /// <summary>
    /// Represents a vector in the 3d space (X, Y, Z)
    /// </summary>
    class Vector3D
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double GetLength()
        {
            return Math.Sqrt(GetLengthSquared());
        }

        public double GetLengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        public double GetHorizontalAngle()
        {
            return Math.Atan2(X, Y);
        }

        public double GetVerticalAngle()
        {
            return Math.Atan2(Z, Math.Sqrt(X * X + Y * Y));
        }

        public Vector3D Rotate(double vertical, double horizontal)
        {
            double length = GetLength();
            double horCur = GetHorizontalAngle();
            double verCur = GetVerticalAngle();
            double newVer = verCur + vertical;
            double newHor = horCur + horizontal;
            double newZ = Math.Sin(newVer) * length;
            double newXY = Math.Cos(newVer) * length;
            double newX = Math.Sin(newHor) * newXY;
            double newY = Math.Cos(newHor) * newXY;
            return new Vector3D(newX, newY, newZ);
        }


    }
}
