using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SaNi.Spriter.Data
{
    public enum CurveType
    {
        Instant,
        Linear,
        Quadratic,
        Cubic,
        Quartic,
        Quintic,
        Bezier
    }

    public class Curve
    {
        public CurveType Type
        {
            get;
            set;
        }
        public Curve SubCurve;

        public readonly Constraints Constraints = new Constraints(0, 0, 0, 0);

        private float lastCubicSolution = 0f;

        public static CurveType GetType(string name)
        {
            if (name == ("instant")) return CurveType.Instant;
            else if (name == ("quadratic")) return CurveType.Quadratic;
            else if (name == ("cubic")) return CurveType.Cubic;
            else if (name == ("quartic")) return CurveType.Quartic;
            else if (name == ("quintic")) return CurveType.Quintic;
            else if (name == ("bezier")) return CurveType.Bezier;
            else return CurveType.Linear;
        }


        public Curve(CurveType type = CurveType.Linear, Curve subCurve = null)
        {
            Type = type;
            SubCurve = subCurve;
        }

        public float tween(float a, float b, float t)
        {
            t = tweenSub(0f, 1f, t);
            switch (Type)
            {
                case CurveType.Instant: return a;
                case CurveType.Linear: return Interpolator.Linear(a, b, t);
                case CurveType.Quadratic: return Interpolator.Quadratic(a, Interpolator.Linear(a, b, Constraints.C1), b, t);
                case CurveType.Cubic: return Interpolator.Cubic(a, Interpolator.Linear(a, b, Constraints.C1), Interpolator.Linear(a, b, Constraints.C2), b, t);
                case CurveType.Quartic: return Interpolator.Quartic(a, Interpolator.Linear(a, b, Constraints.C1), Interpolator.Linear(a, b, Constraints.C2), Interpolator.Linear(a, b, Constraints.C3), b, t);
                case CurveType.Quintic: return Interpolator.Quintic(a, Interpolator.Linear(a, b, Constraints.C1), Interpolator.Linear(a, b, Constraints.C2), Interpolator.Linear(a, b, Constraints.C3), Interpolator.Linear(a, b, Constraints.C4), b, t);
                case CurveType.Bezier: float? cubicSolution = Calculator.SolveCubic(3f * (Constraints.C1 - Constraints.C3) + 1f, 3f * (Constraints.C3 - 2f * Constraints.C1), 3f * Constraints.C1, -t);
                    if (cubicSolution == null) cubicSolution = lastCubicSolution;
                    else lastCubicSolution = cubicSolution.Value;
                    return Interpolator.Linear(a, b, Interpolator.Bezier(cubicSolution.Value, 0f, Constraints.C2, Constraints.C4, 1f));
                default: return Interpolator.Linear(a, b, t);
            }
        }

        public void tweenPoint(Vector2 a, Vector2 b, float t, ref Vector2 target)
        {
            target.X = this.tween(a.X, b.X, t);
            target.Y = this.tween(a.Y, b.Y, t);
        }
        private float tweenSub(float a, float b, float t)
        {
            if (this.SubCurve != null) return SubCurve.tween(a, b, t);
            else return t;
        }

        public float tweenAngle(float a, float b, float t, int spin)
        {
            if (spin > 0)
            {
                if (b - a < 0)
                    b += 360;
            }
            else if (spin < 0)
            {
                if (b - a > 0)
                    b -= 360;
            }
            else return a;
            return tween(a, b, t);
        }

        public float tweenAngle(float a, float b, float t)
        {
            t = tweenSub(0f, 1f, t);
            switch (Type)
            {
                case CurveType.Instant: return a;
                case CurveType.Linear: return Interpolator.LinearAngle(a, b, t);
                case CurveType.Quadratic: return Interpolator.QuadraticAngle(a, Interpolator.LinearAngle(a, b, Constraints.C1), b, t);
                case CurveType.Cubic: return Interpolator.CubicAngle(a, Interpolator.LinearAngle(a, b, Constraints.C1), Interpolator.LinearAngle(a, b, Constraints.C2), b, t);
                case CurveType.Quartic: return Interpolator.QuarticAngle(a, Interpolator.LinearAngle(a, b, Constraints.C1), Interpolator.LinearAngle(a, b, Constraints.C2), Interpolator.LinearAngle(a, b, Constraints.C3), b, t);
                case CurveType.Quintic: return Interpolator.QuinticAngle(a, Interpolator.LinearAngle(a, b, Constraints.C1), Interpolator.LinearAngle(a, b, Constraints.C2), Interpolator.LinearAngle(a, b, Constraints.C3), Interpolator.LinearAngle(a, b, Constraints.C4), b, t);
                case CurveType.Bezier: float? cubicSolution = Calculator.SolveCubic(3f * (Constraints.C1 - Constraints.C3) + 1f, 3f * (Constraints.C3 - 2f * Constraints.C1), 3f * Constraints.C1, -t);
                    if (cubicSolution == null) cubicSolution = lastCubicSolution;
                    else lastCubicSolution = cubicSolution.Value;
                    return Interpolator.LinearAngle(a, b, Interpolator.Bezier(cubicSolution.Value, 0f, Constraints.C2, Constraints.C4, 1f));
                default: return Interpolator.LinearAngle(a, b, t);
            }
        }
    }

    public class Constraints
    {
        public float C1, C2, C3, C4;
        public Constraints(float c1, float c2, float c3, float c4)
        {
            this.Set(c1, c2, c3, c4);
        }
        public void Set(float c1, float c2, float c3, float c4)
        {
            this.C1 = c1;
            this.C2 = c2;
            this.C3 = c3;
            this.C4 = c4;
        }
    }
}
