using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SaNi.Spriter.Data;

namespace SaNi.Spriter
{
    public static class SpriterUtils
    {
        public static ObjectType GetObjectInfoFor(string name)
        {
            if (name == "bone") return ObjectType.Bone;
            if (name == "skin") return ObjectType.Skin;
            if (name == "box") return ObjectType.Box;
            if (name == "point") return ObjectType.Point;
            return ObjectType.Sprite;
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float x = v.X;
            float y = v.Y;
            if (x != 0f || y != 0f)
            {
                float cos = Calculator.cosDeg(degrees);
                float sin = Calculator.sinDeg(degrees);

                float xx = x*cos - y*sin;
                float yy = x*sin + y*cos;
                v.X = xx;
                v.Y = yy;

            }
            return v;
        }
    }

    public class Calculator
    {
        public static readonly float Pi = (float) Math.PI;

        public static float AngleDifference(float a, float b)
        {
            return ((((a - b)%360) + 540)%360) - 180;
        }

        public static float AngleBetween(float x1, float y1, float x2, float y2)
        {
            return MathHelper.ToDegrees((float) Math.Atan2(y2 - y1, x2 - x1));
        }

        public static float DistanceBetween(float x1, float y1, float x2, float y2)
        {
            float xDiff = x2 - x1;
            float yDiff = y2 - y1;
            return (float) Sqrt(xDiff*xDiff + yDiff*yDiff);
        }

        public static float? SolveCubic(float a, float b, float c, float d)
        {
            if (a == 0f) return SolveQuadratic(b, c, d);
            if (d == 0f) return 0f;
            b /= a;
            c /= a;
            d /= a;
            float squaredB = Squared(b);
            float q = (3f*c - squaredB)/9f;
            float r = (-27f*d + b*(9f*c - 2f*squaredB))/54f;
            float disc = Cubed(q) + Squared(r);
            float term1 = b/3f;
            if (disc > 0)
            {
                float s = r + Sqrt(disc);
                s = (s < 0) ? -CubicRoot(-s) : CubicRoot(s);
                float t = r - Sqrt(disc);
                t = (t < 0) ? -CubicRoot(-t) : CubicRoot(t);
                float result = -term1 + s + t;
                if (result >= 0 && result <= 1) return result;
            }
            else if (disc == 0)
            {
                float r13 = (r < 0) ? -CubicRoot(-r) : CubicRoot(r);
                float result = -term1 + 2f*r13;
                if (result >= 0 && result <= 1) return result;
                result = -(r13 + term1);
                if (result >= 0 && result <= 1) return result;
            }
            else
            {
                q = -q;
                float dum1 = q*q*q;
                dum1 = Acos(r/Sqrt(dum1));
                float r13 = 2f*Sqrt(q);
                float result = -term1 + r13*cos(dum1/3f);
                if (result >= 0 && result <= 1) return result;
                result = -term1 + r13*cos((dum1 + 2f*Pi)/3f);
                if (result >= 0 && result <= 1) return result;
                result = -term1 + r13*cos((dum1 + 4f*Pi)/3f);
                if (result >= 0 && result <= 1) return result;
            }
            return null;
        }

        public static float? SolveQuadratic(float a, float b, float c)
        {
            float squaredB = Squared(b);
            float twoA = 2*a;
            float fourAC = 4*a*c;
            float result = (-b + Sqrt(squaredB - fourAC))/twoA;
            if (result >= 0 && result <= 1) return result;
            result = (-b - Sqrt(squaredB - fourAC))/twoA;
            if (result >= 0 && result <= 1) return result;
            return null;
        }

        public static float Squared(float f)
        {
            return f*f;
        }

        public static float Cubed(float f)
        {
            return f*f*f;
        }

        public static float CubicRoot(float f)
        {
            return (float) Math.Pow(f, 1f/3f);
        }

        public static float Sqrt(float x)
        {
            return (float) Math.Sqrt(x);
        }

        public static float Acos(float x)
        {
            return (float) Math.Acos(x);
        }

        private const int SinBits = 14; // 16KB. Adjust for accuracy.
        private const int SinMask = ~(-1 << SinBits);
        private const int SinCount = SinMask + 1;
        private static readonly float radFull = Pi*2;
        private const float degFull = 360;
        private static readonly float radToIndex = SinCount/radFull;
        private const float degToIndex = SinCount/degFull;
        public static readonly float RadiansToDegrees = 180f/Pi;
        public static readonly float RadDeg = RadiansToDegrees;

        public static readonly float DegreesToRadians = Pi/180;
        public static readonly float DegRad = DegreesToRadians;

        private static class Sin
        {
            public static readonly float[] table = new float[SinCount];

            static Sin()
            {
                for (int i = 0; i < SinCount; i++)
                    table[i] = (float) Math.Sin((i + 0.5f)/SinCount*radFull);
                for (int i = 0; i < 360; i += 90)
                    table[(int) (i*degToIndex) & SinMask] = (float) Math.Sin(i*DegreesToRadians);
            }
        }

        public static float sin(float radians)
        {
            return Sin.table[(int) (radians*radToIndex) & SinMask];
        }

        public static float cos(float radians)
        {
            return Sin.table[(int) ((radians + Pi/2)*radToIndex) & SinMask];
        }

        public static float sinDeg(float degrees)
        {
            return Sin.table[(int) (degrees*degToIndex) & SinMask];
        }

        public static float cosDeg(float degrees)
        {
            return Sin.table[(int) ((degrees + 90)*degToIndex) & SinMask];
        }

    }
}
