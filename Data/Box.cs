using System;

namespace SaNi.Spriter.Data
{

	/// <summary>
	/// Represents a box, which consists of four points: top-left, top-right, bottom-left and bottom-right.
	/// A box is responsible for checking collisions and calculating a bounding box for a <seealso cref="Bone"/>.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class Box
	{
		public readonly Point[] Points;
		private readonly Rectangle rect;

		/// <summary>
		/// Creates a new box with no witdh and height.
		/// </summary>
		public Box()
		{
			this.Points = new Point[4];
			//this.Temp = new Point[4];
			for (int i = 0; i < 4; i++)
			{
				this.Points[i] = new Point(0,0);
				//this.Temp[i] = new Point(0,0);
			}
			this.rect = new Rectangle(0,0,0,0);
		}

	    /// <summary>
	    /// Calculates its four points for the given bone or object with the given info. </summary>
	    /// <param name="boneOrObject"> the bone or object </param>
	    /// <param name="info"> the info </param>
	    /// <exception cref="NullPointerException"> if info or boneOrObject is <code>null</code> </exception>
	    public virtual void CalcFor(Bone boneOrObject, Entity.ObjectInfo info)
		{
			float width = info.Size.X * boneOrObject.Scale.X;
			float height = info.Size.Y * boneOrObject.Scale.Y;

			float pivotX = width * boneOrObject.Pivot.X;
			float pivotY = height * boneOrObject.Pivot.Y;

			this.Points[0].Set(-pivotX,-pivotY);
			this.Points[1].Set(width - pivotX, -pivotY);
			this.Points[2].Set(-pivotX,height - pivotY);
			this.Points[3].Set(width - pivotX,height - pivotY);

			for (int i = 0; i < 4; i++)
			{
				this.Points[i].Rotate(boneOrObject.Angle);
			}
			for (int i = 0; i < 4; i++)
			{
				this.Points[i].Translate(boneOrObject.Position);
			}
		}

	    /// <summary>
	    /// Returns whether the given coordinates lie inside the box of the given bone or object. </summary>
	    /// <param name="boneOrObject"> the bone or object </param>
	    /// <param name="info"> the object info of the given bone or object </param>
	    /// <param name="x"> the x coordinate </param>
	    /// <param name="y"> the y coordinate </param>
	    /// <returns> <code>true</code> if the given point lies in the box </returns>
	    /// <exception cref="NullPointerException"> if info or boneOrObject is <code>null</code> </exception>
	    public virtual bool Collides(Bone boneOrObject, Entity.ObjectInfo info, float x, float y)
		{
			float width = info.Size.X * boneOrObject.Scale.X;
			float height = info.Size.Y * boneOrObject.Scale.Y;

			float pivotX = width * boneOrObject.Pivot.X;
			float pivotY = height * boneOrObject.Pivot.Y;

			Point point = new Point(x - boneOrObject.Position.X,y - boneOrObject.Position.Y);
			point.Rotate(-boneOrObject.Angle);

			return point.X >= -pivotX && point.X <= width - pivotX && point.Y >= -pivotY && point.Y <= height - pivotY;
		}

		/// <summary>
		/// Returns whether this box is inside the given rectangle. </summary>
		/// <param name="rect"> the rectangle </param>
		/// <returns>  <code>true</code> if one of the four points is inside the rectangle </returns>
		public virtual bool IsInside(Rectangle rect)
		{
			bool inside = false;
			foreach (Point p in Points)
			{
				inside |= rect.IsInside(p);
			}
			return inside;
		}

		/// <summary>
		/// Returns a bounding box for this box. </summary>
		/// <returns> the bounding box </returns>
		public virtual Rectangle BoundingRect
		{
			get
			{
				this.rect.Set(Points[0].X,Points[0].Y,Points[0].X,Points[0].Y);
				this.rect.Left = Math.Min(Math.Min(Math.Min(Math.Min(Points[0].X, Points[1].X),Points[2].X),Points[3].X), this.rect.Left);
				this.rect.Right = Math.Max(Math.Max(Math.Max(Math.Max(Points[0].X, Points[1].X),Points[2].X),Points[3].X), this.rect.Right);
				this.rect.Top = Math.Max(Math.Max(Math.Max(Math.Max(Points[0].Y, Points[1].Y),Points[2].Y),Points[3].Y), this.rect.Top);
				this.rect.Bottom = Math.Min(Math.Min(Math.Min(Math.Min(Points[0].Y, Points[1].Y),Points[2].Y),Points[3].Y), this.rect.Bottom);
				return this.rect;
			}
		}

	}

}