using System;

namespace SaNi.Spriter.Data
{

	/// <summary>
	/// Represents a mainline in a Spriter SCML file.
	/// A mainline holds only keys and occurs only once in an animation.
	/// The mainline is responsible for telling which draw order the sprites have
	/// and how the objects are related to each other, i.e. which bone is the root and which objects are the children.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class Mainline
	{

		internal readonly Key[] Keys;
		private int KeyPointer = 0;

		public Mainline(int keys)
		{
			this.Keys = new Key[keys];
		}

		public override string ToString()
		{
			string toReturn = this.GetType().Name + "|";
			foreach (Key key in Keys)
			{
				toReturn += "\n" + key;
			}
			toReturn += "]";
			return toReturn;
		}

		public virtual void AddKey(Key key)
		{
			this.Keys[KeyPointer++] = key;
		}

		/// <summary>
		/// Returns a <seealso cref="Key"/> at the given index. </summary>
		/// <param name="index"> the index of the key </param>
		/// <returns> the key with the given index </returns>
		/// <exception cref="IndexOutOfBoundsException"> if index is out of range </exception>
		public virtual Key GetKey(int index)
		{
			return this.Keys[index];
		}

		/// <summary>
		/// Returns a <seealso cref="Key"/> before the given time. </summary>
		/// <param name="time"> the time a key has to be before </param>
		/// <returns> a key which has a time value before the given one.
		/// The first key is returned if no key was found. </returns>
		public virtual Key GetKeyBeforeTime(int time)
		{
			Key found = this.Keys[0];
			foreach (Key key in this.Keys)
			{
				if (key.Time <= time)
				{
					found = key;
				}
				else
				{
					break;
				}
			}
			return found;
		}

		/// <summary>
		/// Represents a mainline key in a Spriter SCML file.
		/// A mainline key holds an <seealso cref="#id"/>, a <seealso cref="#time"/>, a <seealso cref="#curve"/>
		/// and lists of bone and object references which build a tree hierarchy.
		/// @author Trixt0r
		/// 
		/// </summary>
		public class Key
		{

			public readonly int Id, Time;
			internal readonly BoneRef[] BoneRefs;
			internal readonly ObjectRef[] ObjectRefs;
			internal int BonePointer = 0, ObjectPointer = 0;
			public readonly Curve Curve;

			public Key(int id, int time, Curve curve, int boneRefs, int objectRefs)
			{
				this.Id = id;
				this.Time = time;
				this.Curve = curve;
				this.BoneRefs = new BoneRef[boneRefs];
				this.ObjectRefs = new ObjectRef[objectRefs];
			}

			/// <summary>
			/// Adds a bone reference to this key. </summary>
			/// <param name="ref"> the reference to add </param>
			public virtual void AddBoneRef(BoneRef @ref)
			{
				this.BoneRefs[BonePointer++] = @ref;
			}

			/// <summary>
			/// Adds a object reference to this key. </summary>
			/// <param name="ref"> the reference to add </param>
			public virtual void AddObjectRef(ObjectRef @ref)
			{
				this.ObjectRefs[ObjectPointer++] = @ref;
			}

			/// <summary>
			/// Returns a <seealso cref="BoneRef"/> with the given index. </summary>
			/// <param name="index"> the index of the bone reference </param>
			/// <returns> the bone reference or null if no reference exists with the given index </returns>
			public virtual BoneRef GetBoneRef(int index)
			{
				if (index < 0 || index >= this.BoneRefs.Length)
				{
					return null;
				}
				else
				{
					return this.BoneRefs[index];
				}
			}

			/// <summary>
			/// Returns a <seealso cref="ObjectRef"/> with the given index. </summary>
			/// <param name="index"> the index of the object reference </param>
			/// <returns> the object reference or null if no reference exists with the given index </returns>
			public virtual ObjectRef GetObjectRef(int index)
			{
				if (index < 0 || index >= this.ObjectRefs.Length)
				{
					return null;
				}
				else
				{
					return this.ObjectRefs[index];
				}
			}

			/// <summary>
			/// Returns a <seealso cref="BoneRef"/> for the given reference. </summary>
			/// <param name="ref"> the reference to the reference in this key </param>
			/// <returns> a bone reference with the same time line as the given one </returns>
			public virtual BoneRef GetBoneRef(BoneRef @ref)
			{
				return GetBoneRefTimeline(@ref.Timeline);
			}

			/// <summary>
			/// Returns a <seealso cref="BoneRef"/> with the given time line index. </summary>
			/// <param name="timeline"> the time line index </param>
			/// <returns> the bone reference with the given time line index or null if no reference exists with the given time line index </returns>
			public virtual BoneRef GetBoneRefTimeline(int timeline)
			{
				foreach (BoneRef boneRef in this.BoneRefs)
				{
					if (boneRef.Timeline == timeline)
					{
						return boneRef;
					}
				}
				return null;
			}

			/// <summary>
			/// Returns an <seealso cref="ObjectRef"/> for the given reference. </summary>
			/// <param name="ref"> the reference to the reference in this key </param>
			/// <returns> an object reference with the same time line as the given one </returns>
			public virtual ObjectRef GetObjectRef(ObjectRef @ref)
			{
				return GetObjectRefTimeline(@ref.Timeline);
			}

			/// <summary>
			/// Returns a <seealso cref="ObjectRef"/> with the given time line index. </summary>
			/// <param name="timeline"> the time line index </param>
			/// <returns> the object reference with the given time line index or null if no reference exists with the given time line index </returns>
			public virtual ObjectRef GetObjectRefTimeline(int timeline)
			{
				foreach (ObjectRef objRef in this.ObjectRefs)
				{
					if (objRef.Timeline == timeline)
					{
						return objRef;
					}
				}
				return null;
			}

			public override string ToString()
			{
				string toReturn = this.GetType().Name + "|[id:" + Id + ", time: " + Time + ", curve: [" + Curve + "]";
				foreach (BoneRef @ref in BoneRefs)
				{
					toReturn += "\n" + @ref;
				}
				foreach (ObjectRef @ref in ObjectRefs)
				{
					toReturn += "\n" + @ref;
				}
				toReturn += "]";
				return toReturn;
			}

			/// <summary>
			/// Represents a bone reference in a Spriter SCML file.
			/// A bone reference holds an <seealso cref="#id"/>, a <seealso cref="#timeline"/> and a <seealso cref="#key"/>.
			/// A bone reference may have a parent reference.
			/// @author Trixt0r
			/// 
			/// </summary>
			public class BoneRef
			{
				public readonly int Id, Key, Timeline;
				public readonly BoneRef Parent;

				public BoneRef(int id, int timeline, int key, BoneRef parent)
				{
					this.Id = id;
					this.Timeline = timeline;
					this.Key = key;
					this.Parent = parent;
				}

				public override string ToString()
				{
					int parentId = (Parent != null) ? Parent.Id:-1;
					return this.GetType().Name + "|id: " + Id + ", parent:" + parentId + ", timeline: " + Timeline + ", key: " + Key;
				}
			}

			/// <summary>
			/// Represents an object reference in a Spriter SCML file.
			/// An object reference extends a <seealso cref="BoneRef"/> with a <seealso cref="#zIndex"/>,
			/// which indicates when the object has to be drawn.
			/// @author Trixt0r
			/// 
			/// </summary>
			public class ObjectRef : BoneRef, IComparable<ObjectRef>
			{
				public readonly int ZIndex;

				public ObjectRef(int id, int timeline, int key, BoneRef parent, int zIndex) : base(id, timeline, key, parent)
				{
					this.ZIndex = zIndex;
				}

				public override string ToString()
				{
					return base.ToString() + ", z_index: " + ZIndex;
				}

				public virtual int CompareTo(ObjectRef o)
				{
					return (int)Math.Sign(ZIndex - o.ZIndex);
				}
			}
		}

	}

}