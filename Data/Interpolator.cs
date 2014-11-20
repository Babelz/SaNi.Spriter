using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaNi.Spriter.Data
{
    public static class Interpolator
    {
        public static float Linear(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
        public static float LinearAngle(float a, float b, float t)
        {
            return a + Calculator.AngleDifference(b, a) * t;
        }
        public static float Quadratic(float a, float b, float c, float t)
        {
            return Linear(Linear(a, b, t), Linear(b, c, t), t);
        }
        public static float QuadraticAngle(float a, float b, float c, float t)
        {
            return LinearAngle(LinearAngle(a, b, t), LinearAngle(b, c, t), t);
        }
        public static float Cubic(float a, float b, float c, float d, float t)
        {
            return Linear(Quadratic(a, b, c, t), Quadratic(b, c, d, t), t);
        }
        public static float CubicAngle(float a, float b, float c, float d, float t)
        {
            return LinearAngle(QuadraticAngle(a, b, c, t), QuadraticAngle(b, c, d, t), t);
        }
        public static float Quartic(float a, float b, float c, float d, float e, float t)
        {
            return Linear(Cubic(a, b, c, d, t), Cubic(b, c, d, e, t), t);
        }
        public static float QuarticAngle(float a, float b, float c, float d, float e, float t)
        {
            return LinearAngle(CubicAngle(a, b, c, d, t), CubicAngle(b, c, d, e, t), t);
        }
        public static float Quintic(float a, float b, float c, float d, float e, float f, float t)
        {
            return Linear(Quartic(a, b, c, d, e, t), Quartic(b, c, d, e, f, t), t);
        }
        public static float QuinticAngle(float a, float b, float c, float d, float e, float f, float t)
        {
            return LinearAngle(QuarticAngle(a, b, c, d, e, t), QuarticAngle(b, c, d, e, f, t), t);
        }
        public static float Bezier(float t, float x1, float x2, float x3, float x4)
        {
            return Bezier0(t) * x1 + Bezier1(t) * x2 + Bezier2(t) * x3 + Bezier3(t) * x4;
        }
        private static float Bezier0(float t)
        {
            float temp = t * t;
            return -temp * t + 3 * temp - 3 * t + 1;
        }
        private static float Bezier1(float t)
        {
            float temp = t * t;
            return 3 * t * temp - 6 * temp + 3 * t;
        }
        private static float Bezier2(float t)
        {
            float temp = t * t;
            return -3 * temp * t + 3 * temp;
        }
        private static float Bezier3(float t)
        {
            return t * t * t;
        }
    }
}
