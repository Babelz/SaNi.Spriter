namespace SaNi.Spriter.Data
{
    public enum CurveType
    {
        Instant, Linear, Quadratic, Cubic, Quartic, Quintic, Bezier
    }
	/// <summary>
	/// Represents a curve in a Spriter SCML file.
	/// An instance of this class is responsible for tweening given data.
	/// The most important method of this class is <seealso cref="#tween(float, float, float)"/>.
	/// Curves can be changed with sub curves <seealso cref="Curve#subCurve"/>.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class Curve
	{
		/// <summary>
		/// The sub curve of this curve, which can be <code>null</code>.
		/// </summary>
		public Curve SubCurve;
		/// <summary>
		/// The constraints of a curve which will affect a curve of the types different from <seealso cref="Type#Linear"/> and <seealso cref="Type#Instant"/>.
		/// </summary>
		public readonly Constraints Constraints = new Constraints(0, 0, 0, 0);

		/// <summary>
		/// Creates a new linear curve.
		/// </summary>
        public Curve()
            : this(CurveType.Linear)
		{
		}

		/// <summary>
		/// Creates a new curve with the given type. </summary>
		/// <param name="type"> the curve type </param>
        public Curve(CurveType type)
            : this(type, null)
		{
		}

		/// <summary>
		/// Creates a new curve with the given type and sub cuve. </summary>
		/// <param name="type"> the curve type </param>
		/// <param name="subCurve"> the sub curve. Can be <code>null</code> </param>
        public Curve(CurveType type, Curve subCurve)
		{
			this.Type = (type);
			this.SubCurve = subCurve;
		}

	    public CurveType Type
	    {
	        get;
	        set;
	    }


		private float lastCubicSolution = 0f;
		/// <summary>
		/// Returns a new value based on the given values.
		/// Tweens the weight with the set sub curve. </summary>
		/// <param name="a"> the start value </param>
		/// <param name="b"> the end value </param>
		/// <param name="t"> the weight which lies between 0.0 and 1.0 </param>
		/// <returns> tweened value </returns>
		public virtual float Tween(float a, float b, float t)
		{
			t = TweenSub(0f,1f,t);
			switch (Type)
			{
                case CurveType.Instant:
				return a;
                case CurveType.Linear:
				return Interpolator.Linear(a, b, t);
                case CurveType.Quadratic:
                return Interpolator.Quadratic(a, Interpolator.Linear(a, b, Constraints.C1), b, t);
                case CurveType.Cubic:
                return Interpolator.Cubic(a, Interpolator.Linear(a, b, Constraints.C1), Interpolator.Linear(a, b, Constraints.C2), b, t);
                case CurveType.Quartic:
                return Interpolator.Quartic(a, Interpolator.Linear(a, b, Constraints.C1), Interpolator.Linear(a, b, Constraints.C2), Interpolator.Linear(a, b, Constraints.C3), b, t);
                case CurveType.Quintic:
                return Interpolator.Quintic(a, Interpolator.Linear(a, b, Constraints.C1), Interpolator.Linear(a, b, Constraints.C2), Interpolator.Linear(a, b, Constraints.C3), Interpolator.Linear(a, b, Constraints.C4), b, t);
                case CurveType.Bezier:
                float? cubicSolution = Calculator.SolveCubic(3f * (Constraints.C1 - Constraints.C3) + 1f, 3f * (Constraints.C3 - 2f * Constraints.C1), 3f * Constraints.C1, -t);
						 if (cubicSolution == null)
						 {
							 cubicSolution = lastCubicSolution;
						 }
						 else
						 {
							 lastCubicSolution = cubicSolution.Value;
						 }
                         return Interpolator.Linear(a, b, Interpolator.Bezier(cubicSolution.Value, 0f, Constraints.C2, Constraints.C4, 1f));
			default:
                         return Interpolator.Linear(a, b, t);
			}
		}

		/// <summary>
		/// Interpolates the given two points with the given weight and saves the result in the target point. </summary>
		/// <param name="a"> the start point </param>
		/// <param name="b"> the end point </param>
		/// <param name="t"> the weight which lies between 0.0 and 1.0 </param>
		/// <param name="target"> the target point to save the result in </param>
		public virtual void TweenPoint(Point a, Point b, float t, Point target)
		{
			target.Set(this.Tween(a.X, b.X, t), this.Tween(a.Y, b.Y, t));
		}

		private float TweenSub(float a, float b, float t)
		{
			if (this.SubCurve != null)
			{
				return SubCurve.Tween(a, b, t);
			}
			else
			{
				return t;
			}
		}

		/// <summary>
		/// Returns a tweened angle based on the given angles, weight and the spin. </summary>
		/// <param name="a"> the start angle </param>
		/// <param name="b"> the end angle </param>
		/// <param name="t"> the weight which lies between 0.0 and 1.0 </param>
		/// <param name="spin"> the spin, which is either 0, 1 or -1 </param>
		/// <returns> tweened angle </returns>
		public virtual float TweenAngle(float a, float b, float t, int spin)
		{
			if (spin > 0)
			{
				if (b - a < 0)
				{
					b += 360;
				}
			}
			else if (spin < 0)
			{
				if (b - a > 0)
				{
					b -= 360;
				}
			}
			else
			{
				return a;
			}

			return Tween(a, b, t);
		}

		/// <seealso cref= <seealso cref="#tween(float, float, float)"/> </seealso>
		public virtual float TweenAngle(float a, float b, float t)
		{
			t = TweenSub(0f,1f,t);
			switch (Type)
			{
                case CurveType.Instant:
				return a;
                case CurveType.Linear:
                return Interpolator.LinearAngle(a, b, t);
                case CurveType.Quadratic:
                return Interpolator.QuadraticAngle(a, Interpolator.LinearAngle(a, b, Constraints.C1), b, t);
                case CurveType.Cubic:
                return Interpolator.CubicAngle(a, Interpolator.LinearAngle(a, b, Constraints.C1), Interpolator.LinearAngle(a, b, Constraints.C2), b, t);
                case CurveType.Quartic:
                return Interpolator.QuarticAngle(a, Interpolator.LinearAngle(a, b, Constraints.C1), Interpolator.LinearAngle(a, b, Constraints.C2), Interpolator.LinearAngle(a, b, Constraints.C3), b, t);
                case CurveType.Quintic:
                return Interpolator.QuinticAngle(a, Interpolator.LinearAngle(a, b, Constraints.C1), Interpolator.LinearAngle(a, b, Constraints.C2), Interpolator.LinearAngle(a, b, Constraints.C3), Interpolator.LinearAngle(a, b, Constraints.C4), b, t);
                case CurveType.Bezier:
				float? cubicSolution = Calculator.SolveCubic(3f * (Constraints.C1 - Constraints.C3) + 1f, 3f * (Constraints.C3 - 2f * Constraints.C1), 3f * Constraints.C1, -t);
						 if (cubicSolution == null)
						 {
							 cubicSolution = lastCubicSolution;
						 }
						 else
						 {
							 lastCubicSolution = cubicSolution.Value;
						 }
                         return Interpolator.LinearAngle(a, b, Interpolator.Bezier(cubicSolution.Value, 0f, Constraints.C2, Constraints.C4, 1f));
			default:
                         return Interpolator.LinearAngle(a, b, t);
			}
		}

		public override string ToString()
		{
			return GetType().Name + "|[" + Type + ":" + Constraints + ", subCurve: " + SubCurve + "]";
		}

	}

    /// <summary>
    /// Represents constraints for a curve.
    /// Constraints are important for curves which have a order higher than 1.
    /// @author Trixt0r
    /// 
    /// </summary>
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

        public override string ToString()
        {
            return this.GetType().Name + "| [c1:" + C1 + ", c2:" + C2 + ", c3:" + C3 + ", c4:" + C4 + "]";
        }
    }

}