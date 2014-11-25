using System;

namespace SaNi.Spriter.Data
{
    /// <summary>
	/// Represents a time line in a Spriter SCML file.
	/// A time line holds an <seealso cref="#id"/>, a <seealso cref="#name"/> and at least one <seealso cref="Key"/>.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class Timeline
	{

		public readonly Key[] Keys;
		private int KeyPointer = 0;
		public readonly int Id;
		public readonly string Name;
		public readonly Entity.ObjectInfo ObjectInfo;

		internal Timeline(int id, string name, Entity.ObjectInfo objectInfo, int keys)
		{
			this.Id = id;
			this.Name = name;
			this.ObjectInfo = objectInfo;
			this.Keys = new Key[keys];
		}

		internal virtual void AddKey(Key key)
		{
			this.Keys[KeyPointer++] = key;
		}

		/// <summary>
		/// Returns a <seealso cref="Key"/> at the given index </summary>
		/// <param name="index"> the index of the key. </param>
		/// <returns> the key with the given index. </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the index is out of range </exception>
		public virtual Key GetKey(int index)
		{
			return this.Keys[index];
		}

		public override string ToString()
		{
			string toReturn = this.GetType().Name + "|[id:" + Id + ", name: " + Name + ", object_info: " + ObjectInfo;
			foreach (Key key in Keys)
			{
				toReturn += "\n" + key;
			}
			toReturn += "]";
			return toReturn;
		}

		/// <summary>
		/// Represents a time line key in a Spriter SCML file.
		/// A key holds an <seealso cref="#id"/>, a <seealso cref="#time"/>, a <seealso cref="#spin"/>, an <seealso cref="#object()"/> and a <seealso cref="#curve"/>.
		/// @author Trixt0r
		/// 
		/// </summary>
		public class Key
		{

			public readonly int Id, Spin;
			public int Time;
			public readonly Curve Curve;
			public bool Active;
			internal SpriterObject Object;

			public Key(int id, int time, int spin, Curve curve)
			{
				this.Id = id;
				this.Time = time;
				this.Spin = spin;
				this.Curve = curve;
			}

			public Key(int id, int time, int spin) : this(id, time, 1, new Curve())
			{
			}

			public Key(int id, int time) : this(id, time, 1)
			{
			}

			public Key(int id) : this(id, 0)
			{
			}

			public virtual void SetObject(SpriterObject @object)
			{
                if (Object == null)
				{
					throw new System.ArgumentException("object can not be null!");
				}
                this.Object = @object;
			}

			public override string ToString()
			{
				return this.GetType().Name + "|[id: " + Id + ", time: " + Time + ", spin: " + Spin + "\ncurve: " + Curve + "\nobject:" + Object + "]";
			}


		}

	}

    /// <summary>
    /// Represents a bone in a Spriter SCML file.
    /// A bone holds a <seealso cref="#position"/>, <seealso cref="#scale"/>, an <seealso cref="#angle"/> and a <seealso cref="#pivot"/>.
    /// Bones are the only objects which can be used as a parent for other tweenable objects.
    /// @author Trixt0r
    /// 
    /// </summary>
    public class Bone
    {
        public readonly Point Position, Scale, Pivot;
        public float Angle;

        public Bone(Point position, Point scale, Point pivot, float angle)
        {
            this.Position = new Point(position);
            this.Scale = new Point(scale);
            this.Angle = angle;
            this.Pivot = new Point(pivot);
        }

        public Bone(Bone bone)
            : this(bone.Position, bone.Scale, bone.Pivot, bone.Angle)
        {
        }

        public Bone(Point position)
            : this(position, new Point(1f, 1f), new Point(0f, 1f), 0f)
        {
        }

        public Bone()
            : this(new Point())
        {
        }

        /// <summary>
        /// Returns whether this instance is a Spriter object or a bone. </summary>
        /// <returns> true if this instance is a Spriter bone </returns>
        public virtual bool IsBone()
        {
            return !(this is SpriterObject);
        }

        /// <summary>
        /// Sets the values of this bone to the values of the given bone </summary>
        /// <param name="bone"> the bone </param>
        public virtual void Set(Bone bone)
        {
            this.Set(bone.Position, bone.Angle, bone.Scale, bone.Pivot);
        }

        /// <summary>
        /// Sets the given values for this bone. </summary>
        /// <param name="x"> the new position in x direction </param>
        /// <param name="y"> the new position in y direction </param>
        /// <param name="angle"> the new angle </param>
        /// <param name="scaleX"> the new scale in x direction </param>
        /// <param name="scaleY"> the new scale in y direction </param>
        /// <param name="pivotX"> the new pivot in x direction </param>
        /// <param name="pivotY"> the new pivot in y direction </param>
        public virtual void Set(float x, float y, float angle, float scaleX, float scaleY, float pivotX, float pivotY)
        {
            this.Angle = angle;
            this.Position.Set(x, y);
            this.Scale.Set(scaleX, scaleY);
            this.Pivot.Set(pivotX, pivotY);
        }

        /// <summary>
        /// Sets the given values for this bone. </summary>
        /// <param name="position"> the new position </param>
        /// <param name="angle"> the new angle </param>
        /// <param name="scale"> the new scale </param>
        /// <param name="pivot"> the new pivot </param>
        public virtual void Set(Point position, float angle, Point scale, Point pivot)
        {
            this.Set(position.X, position.Y, angle, scale.X, scale.Y, pivot.X, pivot.Y);
        }

        /// <summary>
        /// Maps this bone from it's parent's coordinate system to a global one. </summary>
        /// <param name="parent"> the parent bone of this bone </param>
        public virtual void Unmap(Bone parent)
        {
            this.Angle *= Math.Sign(parent.Scale.X) * Math.Sign(parent.Scale.Y);
            this.Angle += parent.Angle;
            this.Scale.Scale(parent.Scale);
            this.Position.Scale(parent.Scale);
            this.Position.Rotate(parent.Angle);
            this.Position.Translate(parent.Position);
        }

        /// <summary>
        /// Maps this from it's global coordinate system to the parent's one. </summary>
        /// <param name="parent"> the parent bone of this bone </param>
        public virtual void Map(Bone parent)
        {
            this.Position.Translate(-parent.Position.X, -parent.Position.Y);
            this.Position.Rotate(-parent.Angle);
            this.Position.Scale(1f / parent.Scale.X, 1f / parent.Scale.Y);
            this.Scale.Scale(1f / parent.Scale.X, 1f / parent.Scale.Y);
            this.Angle -= parent.Angle;
            this.Angle *= Math.Sign(parent.Scale.X) * Math.Sign(parent.Scale.Y);
        }

        public override string ToString()
        {
            return this.GetType().Name + "|position: " + Position + ", scale: " + Scale + ", angle: " + Angle;
        }
    }


    /// <summary>
    /// Represents an object in a Spriter SCML file.
    /// A file has the same properties as a bone with an alpha and file extension.
    /// @author Trixt0r
    /// 
    /// </summary>
    public class SpriterObject : Bone
    {

        public float Alpha;
        public readonly FileReference @ref;

        public SpriterObject(Point position, Point scale, Point pivot, float angle, float alpha, FileReference @ref)
            : base(position, scale, pivot, angle)
        {
            this.Alpha = alpha;
            this.@ref = @ref;
        }

        public SpriterObject(Point position)
            : this(position, new Point(1f, 1f), new Point(0f, 1f), 0f, 1f, new FileReference(-1, -1))
        {
        }

        public SpriterObject(SpriterObject @object)
            : this(@object.Position.Copy(), @object.Scale.Copy(), @object.Pivot.Copy(), @object.Angle, @object.Alpha, @object.@ref)
        {
        }

        public SpriterObject()
            : this(new Point())
        {
        }

        /// <summary>
        /// Sets the values of this object to the values of the given object. </summary>
        /// <param name="object"> the object </param>
        public virtual void Set(SpriterObject @object)
        {
            this.Set(@object.Position, @object.Angle, @object.Scale, @object.Pivot, @object.Alpha, @object.@ref);
        }

        /// <summary>
        /// Sets the given values for this object. </summary>
        /// <param name="x"> the new position in x direction </param>
        /// <param name="y"> the new position in y direction </param>
        /// <param name="angle"> the new angle </param>
        /// <param name="scaleX"> the new scale in x direction </param>
        /// <param name="scaleY"> the new scale in y direction </param>
        /// <param name="pivotX"> the new pivot in x direction </param>
        /// <param name="pivotY"> the new pivot in y direction </param>
        /// <param name="alpha"> the new alpha value </param>
        /// <param name="folder"> the new folder index </param>
        /// <param name="file"> the new file index </param>
        public virtual void Set(float x, float y, float angle, float scaleX, float scaleY, float pivotX, float pivotY, float alpha, int folder, int file)
        {
            base.Set(x, y, angle, scaleX, scaleY, pivotX, pivotY);
            this.Alpha = alpha;
            this.@ref.Folder = folder;
            this.@ref.File = file;
        }

        /// <summary>
        /// Sets the given values for this object. </summary>
        /// <param name="position"> the new position </param>
        /// <param name="angle"> the new angle </param>
        /// <param name="scale"> the new scale </param>
        /// <param name="pivot"> the new pivot </param>
        /// <param name="alpha"> the new alpha value </param>
        /// <param name="fileRef"> the new file reference </param>
        public virtual void Set(Point position, float angle, Point scale, Point pivot, float alpha, FileReference fileRef)
        {
            this.Set(position.X, position.Y, angle, scale.X, scale.Y, pivot.X, pivot.Y, alpha, fileRef.Folder, fileRef.File);
        }

        public override string ToString()
        {
            return base.ToString() + ", pivot: " + Pivot + ", alpha: " + Alpha + ", reference: " + @ref;
        }

    }
}