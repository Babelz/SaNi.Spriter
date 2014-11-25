using System;

namespace SaNi.Spriter.Data
{

	/// <summary>
	/// Represents a 2D rectangle with left, top, right and bottom bounds.
	/// A rectangle is responsible for calculating its own size and checking if a point is inside it or if it is intersecting with another rectangle.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class Rectangle
	{

		/// <summary>
		/// Belongs to the bounds of this rectangle.
		/// </summary>
		public float Left, Top, Right, Bottom;
		/// <summary>
		/// The size of this rectangle.
		/// </summary>
		public readonly Dimension Size;

		/// <summary>
		/// Creates a rectangle with the given bounds. </summary>
		/// <param name="left"> left bounding </param>
		/// <param name="top"> top bounding </param>
		/// <param name="right"> right bounding </param>
		/// <param name="bottom"> bottom bounding </param>
		public Rectangle(float left, float top, float right, float bottom)
		{
			this.Set(left, top, right, bottom);
			this.Size = new Dimension(0, 0);
			this.CalculateSize();
		}

		/// <summary>
		/// Creates a rectangle with the bounds of the given rectangle. </summary>
		/// <param name="rect"> rectangle containing the bounds. </param>
		public Rectangle(Rectangle rect) : this(rect.Left, rect.Top, rect.Right, rect.Bottom)
		{
		}

		/// <summary>
		/// Returns whether the given point (x,y) is inside this rectangle. </summary>
		/// <param name="x"> the x coordinate </param>
		/// <param name="y"> the y coordinate </param>
		/// <returns> <code>true</code> if (x,y) is inside </returns>
		public virtual bool IsInside(float x, float y)
		{
			return x >= this.Left && x <= this.Right && y <= this.Top && y >= this.Bottom;
		}

		/// <summary>
		/// Returns whether the given point is inside this rectangle. </summary>
		/// <param name="point"> the point </param>
		/// <returns> <code>true</code> if the point is inside </returns>
		public virtual bool IsInside(Point point)
		{
			return IsInside(point.X, point.Y);
		}

		/// <summary>
		/// Calculates the size of this rectangle.
		/// </summary>
		public virtual void CalculateSize()
		{
			this.Size.Set(Right - Left, Top - Bottom);
		}

		/// <summary>
		/// Sets the bounds of this rectangle to the bounds of the given rectangle. </summary>
		/// <param name="rect"> rectangle containing the bounds. </param>
		public virtual void Set(Rectangle rect)
		{
			if (rect == null)
			{
				return;
			}
			this.Bottom = rect.Bottom;
			this.Left = rect.Left;
			this.Right = rect.Right;
			this.Top = rect.Top;
			this.CalculateSize();
		}

		/// <summary>
		/// Sets the bounds of this rectangle to the given bounds. </summary>
		/// <param name="left"> left bounding </param>
		/// <param name="top"> top bounding </param>
		/// <param name="right"> right bounding </param>
		/// <param name="bottom"> bottom bounding </param>
		public virtual void Set(float left, float top, float right, float bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}

		/// <summary>
		/// Returns whether the given two rectangles are intersecting. </summary>
		/// <param name="rect1"> the first rectangle </param>
		/// <param name="rect2"> the second rectangle </param>
		/// <returns> <code>true</code> if the rectangles are intersecting </returns>
		public static bool AreIntersecting(Rectangle rect1, Rectangle rect2)
		{
			return rect1.IsInside(rect2.Left, rect2.Top) || rect1.IsInside(rect2.Right, rect2.Top) || rect1.IsInside(rect2.Left, rect2.Bottom) || rect1.IsInside(rect2.Right, rect2.Bottom);
		}

		/// <summary>
		/// Creates a bigger rectangle of the given two and saves it in the target. </summary>
		/// <param name="rect1"> the first rectangle </param>
		/// <param name="rect2"> the second rectangle </param>
		/// <param name="target"> the target to save the new bounds. </param>
		public static void SetBiggerRectangle(Rectangle rect1, Rectangle rect2, Rectangle target)
		{
			target.Left = Math.Min(rect1.Left, rect2.Left);
			target.Bottom = Math.Min(rect1.Bottom, rect2.Bottom);
			target.Right = Math.Max(rect1.Right, rect2.Right);
			target.Top = Math.Max(rect1.Top, rect2.Top);
		}

	}

}