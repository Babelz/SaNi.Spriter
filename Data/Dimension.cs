namespace SaNi.Spriter.Data
{

	/// <summary>
	/// Represents a dimension in a 2D space.
	/// A dimension has a width and a height.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class Dimension
	{

		public float X, Y;

		/// <summary>
		/// Creates a new dimension with the given size. </summary>
		/// <param name="width"> the width of the dimension </param>
		/// <param name="height"> the height of the dimension </param>
		public Dimension(float width, float height)
		{
			this.Set(width, height);
		}

		/// <summary>
		/// Creates a new dimension with the given size. </summary>
		/// <param name="size"> the size </param>
		public Dimension(Dimension size)
		{
			this.Set(size);
		}

		/// <summary>
		/// Sets the size of this dimension to the given size. </summary>
		/// <param name="width"> the width of the dimension </param>
		/// <param name="height"> the height of the dimension </param>
		public virtual void Set(float width, float height)
		{
			this.X = width;
			this.Y = height;
		}

		/// <summary>
		/// Sets the size of this dimension to the given size. </summary>
		/// <param name="size"> the size </param>
		public virtual void Set(Dimension size)
		{
			this.Set(size.X, size.Y);
		}

		public override string ToString()
		{
			return "[" + X + "x" + Y + "]";
		}

	}

}