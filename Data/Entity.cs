using System.Collections.Generic;

namespace SaNi.Spriter.Data
{


	/// <summary>
	/// Represents an entity of a Spriter SCML file.
	/// An entity holds <seealso cref="SpriterAnimation"/>s, an <seealso cref="#id"/>, a <seealso cref="#name"/>.
	/// <seealso cref="#characterMaps"/> and <seealso cref="#objectInfos"/> may be empty.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class Entity
	{

		public readonly int Id;
		public readonly string Name;
		private readonly SpriterAnimation[] spriterAnimations;
		private int animationPointer = 0;
		private readonly Dictionary<string, SpriterAnimation> namedAnimations;
		private readonly CharacterMap[] characterMaps;
		private int charMapPointer = 0;
		private readonly ObjectInfo[] objectInfos;
		private int objInfoPointer = 0;

		internal Entity(int id, string name, int animations, int characterMaps, int objectInfos)
		{
			this.Id = id;
			this.Name = name;
			this.spriterAnimations = new SpriterAnimation[animations];
			this.characterMaps = new CharacterMap[characterMaps];
			this.objectInfos = new ObjectInfo[objectInfos];
			this.namedAnimations = new Dictionary<string, SpriterAnimation>();
		}

		internal virtual void AddAnimation(SpriterAnimation anim)
		{
			this.spriterAnimations[animationPointer++] = anim;
			this.namedAnimations[anim.Name] = anim;
		}

		/// <summary>
		/// Returns an <seealso cref="SpriterAnimation"/> with the given index. </summary>
		/// <param name="index"> the index of the animation </param>
		/// <returns> the animation with the given index </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the index is out of range </exception>
		public virtual SpriterAnimation GetAnimation(int index)
		{
			return this.spriterAnimations[index];
		}

		/// <summary>
		/// Returns an <seealso cref="SpriterAnimation"/> with the given name. </summary>
		/// <param name="name"> the name of the animation </param>
		/// <returns> the animation with the given name or null if no animation exists with the given name </returns>
		public virtual SpriterAnimation GetAnimation(string name)
		{
			return this.namedAnimations[name];
		}

		/// <summary>
		/// Returns the number of animations this entity holds. </summary>
		/// <returns> the number of animations </returns>
		public virtual int Animations()
		{
			return this.spriterAnimations.Length;
		}

		/// <summary>
		/// Returns whether this entity contains the given animation. </summary>
		/// <param name="anim"> the animation to check </param>
		/// <returns> true if the given animation is in this entity, false otherwise. </returns>
		public virtual bool ContainsAnimation(SpriterAnimation anim)
		{
			foreach (SpriterAnimation a in this.spriterAnimations)
			{
				if (a == anim)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns the animation with the most number of time lines in this entity. </summary>
		/// <returns> animation with the maximum amount of time lines. </returns>
		public virtual SpriterAnimation SpriterAnimationWithMostTimelines
		{
			get
			{
				SpriterAnimation maxAnim = GetAnimation(0);
				foreach (SpriterAnimation anim in this.spriterAnimations)
				{
					if (maxAnim.Timelines() < anim.Timelines())
					{
						maxAnim = anim;
					}
				}
				return maxAnim;
			}
		}

		/// <summary>
		/// Returns a <seealso cref="CharacterMap"/> with the given name. </summary>
		/// <param name="name"> name of the character map </param>
		/// <returns> the character map or null if no character map exists with the given name </returns>
		public virtual CharacterMap GetCharacterMap(string name)
		{
			foreach (CharacterMap map in this.characterMaps)
			{
				if (map.Name.Equals(name))
				{
					return map;
				}
			}
			return null;
		}

		internal virtual void AddCharacterMap(CharacterMap map)
		{
			this.characterMaps[charMapPointer++] = map;
		}

		internal virtual void AddInfo(ObjectInfo info)
		{
			this.objectInfos[objInfoPointer++] = info;
		}

		/// <summary>
		/// Returns an <seealso cref="ObjectInfo"/> with the given index. </summary>
		/// <param name="index"> the index of the object info </param>
		/// <returns> the object info </returns>
		/// <exception cref="IndexOutOfBoundsException"> if index is out of range </exception>
		public virtual ObjectInfo GetInfo(int index)
		{
			return this.objectInfos[index];
		}

		/// <summary>
		/// Returns an <seealso cref="ObjectInfo"/> with the given name. </summary>
		/// <param name="name"> name of the object info </param>
		/// <returns> object info or null if no object info exists with the given name </returns>
		public virtual ObjectInfo GetInfo(string name)
		{
			foreach (ObjectInfo info in this.objectInfos)
			{
				if (info.Name.Equals(name))
				{
					return info;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns an <seealso cref="ObjectInfo"/> with the given name and the given <seealso cref="ObjectType"/> type. </summary>
		/// <param name="name"> the name of the object info </param>
		/// <param name="type"> the type if the object info </param>
		/// <returns> the object info or null if no object info exists with the given name and type </returns>
		public virtual ObjectInfo GetInfo(string name, ObjectType type)
		{
			ObjectInfo info = this.GetInfo(name);
			if (info != null && info.Type == type)
			{
				return info;
			}
			else
			{
				return null;
			}
		}


        public enum ObjectType
        {
            Sprite, Bone, Box, Point, Skin
        }
		

		/// <summary>
		/// Represents the object info in a Spriter SCML file.
		/// An object info holds a <seealso cref="#type"/> and a <seealso cref="#name"/>.
		/// If the type is a Sprite it holds a list of frames. Otherwise it has a <seealso cref="#size"/> for debug drawing purposes.
		/// @author Trixt0r
		/// 
		/// </summary>
		public class ObjectInfo
		{
			public readonly ObjectType Type;
			public readonly IList<FileReference> Frames;
			public readonly string Name;
			public readonly Dimension Size;

			internal ObjectInfo(string name, ObjectType type, Dimension size, IList<FileReference> frames)
			{
				this.Type = type;
				this.Frames = frames;
				this.Name = name;
				this.Size = size;
			}

			internal ObjectInfo(string name, ObjectType type, Dimension size) : this(name, type, size, new List<FileReference>())
			{
			}

			internal ObjectInfo(string name, ObjectType type, IList<FileReference> frames) : this(name, type, new Dimension(0,0), frames)
			{
			}

			public override string ToString()
			{
				return Name + ": " + Type + ", size: " + Size + "|frames:\n" + Frames;
			}
		}

		/// <summary>
		/// Represents a Spriter SCML character map.
		/// A character map maps <seealso cref="FileReference"/>s to <seealso cref="FileReference"/>s.
		/// It holds an <seealso cref="CharacterMap#id"/> and a <seealso cref="CharacterMap#name"/>.
		/// @author Trixt0r
		/// 
		/// </summary>
		public class CharacterMap : Dictionary<FileReference, FileReference>
		{
			internal const long SerialVersionUID = 6062776450159802283L;

			public readonly int Id;
			public readonly string Name;

			public CharacterMap(int id, string name)
			{
				this.Id = id;
				this.Name = name;
			}

			/// <summary>
			/// Returns the mapped reference for the given key. </summary>
			/// <param name="key"> the key of the reference </param>
			/// <returns> The mapped reference if the key is in this map, otherwise the given key itself is returned. </returns>
			public virtual FileReference Get(FileReference key)
			{
				if (!base.ContainsKey(key))
				{
					return key;
				}
				else
				{
					return base[key];
				}
			}
		}

		public override string ToString()
		{
			string toReturn = this.GetType().Name + "|[id: " + Id + ", name: " + Name + "]";
			toReturn += "Object infos:\n";
			foreach (ObjectInfo info in this.objectInfos)
			{
				toReturn += "\n" + info;
			}
			toReturn += "Character maps:\n";
			foreach (CharacterMap map in this.characterMaps)
			{
				toReturn += "\n" + map;
			}
			toReturn += "Animations:\n";
			foreach (SpriterAnimation animaton in this.spriterAnimations)
			{
				toReturn += "\n" + animaton;
			}
			toReturn += "]";
			return toReturn;
		}

	}

}