namespace SaNi.Spriter.Data
{

	/// <summary>
	/// Represents a file in a Spriter SCML file.
	/// A file has an <seealso cref="#id"/>, a <seealso cref="#name"/>.
	/// A <seealso cref="#size"/> and a <seealso cref="#pivot"/> point, i.e. origin of an image do not have to be set since a file can be a sound file.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class File
	{

		public readonly int Id;
		public readonly string Name;
		public readonly Dimension Size;
		public readonly Point Pivot;

		internal File(int id, string name, Dimension size, Point pivot)
		{
			this.Id = id;
			this.Name = name;
			this.Size = size;
			this.Pivot = pivot;
		}

		/// <summary>
		/// Returns whether this file is a sprite, i.e. an image which is going to be animated, or not. </summary>
		/// <returns> whether this file is a sprite or not. </returns>
		public virtual bool Sprite
		{
			get
			{
				return Pivot != null && Size != null;
			}
		}

		public override string ToString()
		{
			return this.GetType().Name + "|[id: " + Id + ", name: " + Name + ", size: " + Size + ", pivot: " + Pivot;
		}

	}

}