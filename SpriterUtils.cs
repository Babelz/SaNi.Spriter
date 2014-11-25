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
        public static CurveType GetType(string name)
        {
            if (name.Equals("instant"))
            {
                return CurveType.Instant;
            }
            else if (name.Equals("quadratic"))
            {
                return CurveType.Quadratic;
            }
            else if (name.Equals("cubic"))
            {
                return CurveType.Cubic;
            }
            else if (name.Equals("quartic"))
            {
                return CurveType.Quartic;
            }
            else if (name.Equals("quintic"))
            {
                return CurveType.Quintic;
            }
            else if (name.Equals("bezier"))
            {
                return CurveType.Bezier;
            }
            else
            {
                return CurveType.Linear;
            }
        }

        public static Entity.ObjectType GetObjectInfoFor(string name)
        {
            if (name == "bone") return Entity.ObjectType.Bone;
            if (name == "skin") return Entity.ObjectType.Skin;
            if (name == "box") return Entity.ObjectType.Box;
            if (name == "point") return Entity.ObjectType.Point;
            return Entity.ObjectType.Sprite;
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float x = v.X;
            float y = v.Y;
            if (x != 0f || y != 0f)
            {
                float cos = Calculator.CosDeg(degrees);
                float sin = Calculator.SinDeg(degrees);

                float xx = x*cos - y*sin;
                float yy = x*sin + y*cos;
                v.X = xx;
                v.Y = yy;

            }
            return v;
        }
    }
}
