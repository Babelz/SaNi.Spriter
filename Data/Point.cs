namespace SaNi.Spriter.Data
{

	/// <summary>
	/// A utility class to keep the code short.
	/// A point is essentially that what you would expect if you think about a point in a 2D space.
	/// It holds an x and y value. You can <seealso cref="#translate(Point)"/>, <seealso cref="#scale(Point)"/>, <seealso cref="#rotate(float)"/> and <seealso cref="#set(Point)"/> a point.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class Point
	{

		/// <summary>
		/// The x coordinates of this point.
		/// </summary>
		public float X;
		/// <summary>
		/// The y coordinates of this point.
		/// </summary>
		public float Y;

		/// <summary>
		/// Creates a point at (0,0).
		/// </summary>
		public Point() : this(0,0)
		{
		}

		/// <summary>
		/// Creates a point at the position of the given point. </summary>
		/// <param name="point"> the point to set this point at </param>
		public Point(Point point) : this(point.X, point.Y)
		{
		}

		/// <summary>
		/// Creates a point at (x, y). </summary>
		/// <param name="x"> the x coordinate </param>
		/// <param name="y"> the y coordinate </param>
		public Point(float x, float y)
		{
			this.Set(x, y);
		}

		/// <summary>
		/// Sets this point to the given coordinates. </summary>
		/// <param name="x"> the x coordinate </param>
		/// <param name="y"> the y coordinate </param>
		/// <returns> this point for chained operations </returns>
		public virtual Point Set(float x, float y)
		{
			this.X = x;
			this.Y = y;
			return this;
		}

		/// <summary>
		/// Adds the given amount to this point. </summary>
		/// <param name="x"> the amount in x direction to add </param>
		/// <param name="y"> the amount in y direction to add </param>
		/// <returns> this point for chained operations </returns>
		public virtual Point Translate(float x, float y)
		{
			return this.Set(this.X + x, this.Y + y);
		}

		/// <summary>
		/// Scales this point by the given amount. </summary>
		/// <param name="x"> the scale amount in x direction </param>
		/// <param name="y"> the scale amount in y direction </param>
		/// <returns> this point for chained operations </returns>
		public virtual Point Scale(float x, float y)
		{
			return this.Set(this.X * x, this.Y * y);
		}

		/// <summary>
		/// Sets this point to the given point. </summary>
		/// <param name="point"> the new coordinates </param>
		/// <returns> this point for chained operations </returns>
		public virtual Point Set(Point point)
		{
			return this.Set(point.X, point.Y);
		}

		/// <summary>
		/// Adds the given amount to this point. </summary>
		/// <param name="amount"> the amount to add </param>
		/// <returns> this point for chained operations </returns>
		public virtual Point Translate(Point amount)
		{
			return this.Translate(amount.X, amount.Y);
		}

		/// <summary>
		/// Scales this point by the given amount. </summary>
		/// <param name="amount"> the amount to scale </param>
		/// <returns> this point for chained operations </returns>
		public virtual Point Scale(Point amount)
		{
			return this.Scale(amount.X, amount.Y);
		}

		/// <summary>
		/// Rotates this point around (0,0) by the given amount of degrees. </summary>
		/// <param name="degrees"> the angle to rotate this point </param>
		/// <returns> this point for chained operations </returns>
		public virtual Point Rotate(float degrees)
		{
			if (X != 0 || Y != 0)
			{
				float cos = Calculator.CosDeg(degrees);
				float sin = Calculator.SinDeg(degrees);

				float xx = X * cos - Y * sin;
				float yy = X * sin + Y * cos;

				this.X = xx;
				this.Y = yy;
			}
			return this;
		}

		/// <summary>
		/// Returns a copy of this point with the current set values. </summary>
		/// <returns> a copy of this point </returns>
		public virtual Point Copy()
		{
			return new Point(X,Y);
		}

		public override string ToString()
		{
			return "[" + X + "," + Y + "]";
		}

	}

}