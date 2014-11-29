using System;
using System.Collections;
using System.Collections.Generic;
using SaNi.Spriter;
using SaNi.Spriter.Data;

namespace SaNi.Spriter
{
    using Object = SpriterObject;
    public delegate void AnimationFinishedEventHandler(SpriterAnimation animation);
    public delegate void AnimationChangedEventHandler(SpriterAnimation old, SpriterAnimation newAnim);
    public delegate void PlayerProcessEventHandler(SpriterAnimationPlayer player);
    public delegate void MainlineKeyChangedEventHandler(Mainline.Key previous, Mainline.Key newKey);
    
	public class SpriterAnimationPlayer
	{
		protected internal Entity Entity;
		internal SpriterAnimation Animation;
		internal int Time;
		public int Speed;
		internal Timeline.Key[] TweenedKeys, UnmappedTweenedKeys;
		private Timeline.Key[] tempTweenedKeys, tempUnmappedTweenedKeys;

        #region Events

         public event AnimationFinishedEventHandler OnAnimationFinished;
         public event AnimationChangedEventHandler OnAnimationChanged;
         /// <summary>
         /// Kutsutaan ennen kuin päivitetään animaatiota
         /// </summary>
         public event PlayerProcessEventHandler OnPreProcess;
         /// <summary>
         /// Kutsutaan animaation päivittämisen jälkeen
         /// </summary>
         public event PlayerProcessEventHandler OnPostProcess;
         /// <summary>
         /// Kun mainlinekey vaihtuu
         /// </summary>
         public event MainlineKeyChangedEventHandler OnMainlineKeyChanged;

        #endregion

        public readonly IList<Attachment> Attachments = new List<Attachment>();
		internal Bone Root = new Bone(new Point(0,0));
		private readonly Point position = new Point(0,0), pivot = new Point(0,0);
		private readonly Dictionary<Object, Timeline.Key> objToTimeline = new Dictionary<Object, Timeline.Key>();
		private float angle;
		private bool dirty = true;
		public Entity.CharacterMap[] CharacterMaps;
		private readonly Rectangle rect;
		public readonly Box PrevBBox;
		private readonly BoneIterator boneIterator;
		private readonly ObjectIterator objectIterator;
		private Mainline.Key currentKey, prevKey;
		public bool copyObjects = true;
		/// <summary>
		/// Creates a <seealso cref="SpriterAnimationPlayer"/> instance with the given entity. </summary>
		/// <param name="entity"> the entity this player will animate </param>
		public SpriterAnimationPlayer(Entity entity)
		{
			this.boneIterator = new BoneIterator(this);
			this.objectIterator = new ObjectIterator(this);
			this.Speed = 15;
			this.rect = new Rectangle(0,0,0,0);
			this.PrevBBox = new Box();
			
			SetEntity(entity);
		}

		/// <summary>
		/// Updates this player.
		/// This means the current time gets increased by <seealso cref="Speed"/> and is applied to the current animation.
		/// </summary>
		public virtual void Update()
		{
		    if (OnPreProcess != null)
		    {
		        OnPreProcess(this);
		    }
			if (dirty)
			{
				this.UpdateRoot();
			}
			this.Animation.Update(Time, Root);
			this.currentKey = this.Animation.CurrentKey;
			if (prevKey != currentKey)
			{
			    if (OnMainlineKeyChanged != null)
			    {
			        OnMainlineKeyChanged(prevKey, currentKey);
			    }
				prevKey = currentKey;
			}
			if (copyObjects)
			{
				TweenedKeys = tempTweenedKeys;
				UnmappedTweenedKeys = tempUnmappedTweenedKeys;
				this.CopyObjects();
			}
			else
			{
				TweenedKeys = Animation.TweenedKeys;
				UnmappedTweenedKeys = Animation.UnmappedTweenedKeys;
			}

			foreach (Attachment attach in Attachments)
			{
				attach.Update();
			}

		    if (OnPostProcess != null)
		    {
		        OnPostProcess(this);
		    }
			this.IncreaseTime();
		}

		private void CopyObjects()
		{
			for (int i = 0; i < Animation.TweenedKeys.Length; i++)
			{
				this.TweenedKeys[i].Active = Animation.TweenedKeys[i].Active;
				this.UnmappedTweenedKeys[i].Active = Animation.UnmappedTweenedKeys[i].Active;
				this.TweenedKeys[i].Object.Set(Animation.TweenedKeys[i].Object);
				this.UnmappedTweenedKeys[i].Object.Set(Animation.UnmappedTweenedKeys[i].Object);
			}
		}

		private void IncreaseTime()
		{
			Time += Speed;
			if (Time > Animation.Length)
			{
				Time = Time - Animation.Length;
			    if (OnAnimationFinished != null)
			    {
			        OnAnimationFinished(Animation);
			    }
			}
			if (Time < 0)
			{
                if (OnAnimationFinished != null)
                {
                    OnAnimationFinished(Animation);
                }
				Time += Animation.Length;
			}
		}

		private void UpdateRoot()
		{
			this.Root.Angle = angle;
			this.Root.Position.Set(pivot);
			this.Root.Position.Rotate(angle);
			this.Root.Position.Translate(position);
			dirty = false;
		}

		/// <summary>
		/// Returns a time line bone at the given index. </summary>
		/// <param name="index"> the index of the bone </param>
		/// <returns> the bone with the given index. </returns>
		public virtual Bone GetBone(int index)
		{
			return this.UnmappedTweenedKeys[CurrentKey.GetBoneRef(index).Timeline].Object;
		}

		/// <summary>
		/// Returns a time line object at the given index. </summary>
		/// <param name="index"> the index of the object </param>
		/// <returns> the object with the given index. </returns>
		public virtual Object GetObject(int index)
		{
			return this.UnmappedTweenedKeys[CurrentKey.GetObjectRef(index).Timeline].Object;
		}

		/// <summary>
		/// Returns the index of a time line bone with the given name. </summary>
		/// <param name="name"> the name of the bone </param>
		/// <returns> the index of the bone or -1 if no bone exists with the given name </returns>
		public virtual int GetBoneIndex(string name)
		{
			foreach (Mainline.Key.BoneRef @ref in CurrentKey.BoneRefs)
			{
				if (Animation.GetTimeline(@ref.Timeline).Name.Equals(name))
				{
					return @ref.Id;
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns a time line bone with the given name. </summary>
		/// <param name="name"> the name of the bone </param>
		/// <returns> the bone with the given name </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if no bone exists with the given name </exception>
		/// <exception cref="NullPointerException"> if no bone exists with the given name </exception>
		public virtual Bone GetBone(string name)
		{
		    return this.UnmappedTweenedKeys[Animation.GetTimeline(name).Id].Object;
		}

		/// <summary>
		/// Returns a bone reference for the given time line bone. </summary>
		/// <param name="bone"> the time line bone </param>
		/// <returns> the bone reference for the given bone </returns>
		/// <exception cref="NullPointerException"> if no reference for the given bone was found </exception>
		public virtual Mainline.Key.BoneRef GetBoneRef(Bone bone)
		{
			return this.CurrentKey.GetBoneRefTimeline(this.objToTimeline[bone as Object].Id);
		}

		/// <summary>
		/// Returns the index of a time line object with the given name. </summary>
		/// <param name="name"> the name of the object </param>
		/// <returns> the index of the object or -1 if no object exists with the given name </returns>
		public virtual int GetObjectIndex(string name)
		{
			foreach (Mainline.Key.ObjectRef @ref in CurrentKey.ObjectRefs)
			{
				if (Animation.GetTimeline(@ref.Timeline).Name.Equals(name))
				{
					return @ref.Id;
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns a time line object with the given name. </summary>
		/// <param name="name"> the name of the object </param>
		/// <returns> the object with the given name </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if no object exists with the given name </exception>
		/// <exception cref="NullPointerException"> if no object exists with the given name </exception>
		public virtual Object GetObject(string name)
		{
			return UnmappedTweenedKeys[Animation.GetTimeline(name).Id].Object;
		}

		/// <summary>
		/// Returns a object reference for the given time line bone. </summary>
		/// <param name="object"> the time line object </param>
		/// <returns> the object reference for the given bone </returns>
		/// <exception cref="NullPointerException"> if no reference for the given object was found </exception>
		public virtual Mainline.Key.ObjectRef GetObjectRef(Object @object)
		{
			return this.CurrentKey.GetObjectRefTimeline(this.objToTimeline[@object].Id);
		}

		/// <summary>
		/// Returns the name for the given bone or object. </summary>
		/// <param name="boneOrObject"> the bone or object </param>
		/// <returns> the name of the bone or object </returns>
		/// <exception cref="NullPointerException"> if no name for the given bone or bject was found </exception>
		public virtual string GetNameFor(Bone boneOrObject)
		{
			return this.Animation.GetTimeline(objToTimeline[boneOrObject as Object].Id).Name;
		}

		/// <summary>
		/// Returns the object info for the given bone or object. </summary>
		/// <param name="boneOrObject"> the bone or object </param>
		/// <returns> the object info of the bone or object </returns>
		/// <exception cref="NullPointerException"> if no object info for the given bone or bject was found </exception>
		public virtual Entity.ObjectInfo GetObjectInfoFor(Bone boneOrObject)
		{
			return this.Animation.GetTimeline(objToTimeline[boneOrObject as Object].Id).ObjectInfo;
		}

		/// <summary>
		/// Returns the time line key for the given bone or object </summary>
		/// <param name="boneOrObject"> the bone or object </param>
		/// <returns> the time line key of the bone or object, or null if no time line key was found </returns>
		public virtual Timeline.Key GetKeyFor(Bone boneOrObject)
		{
			return objToTimeline[boneOrObject as Object];
		}

		/// <summary>
		/// Calculates and returns a <seealso cref="Box"/> for the given bone or object. </summary>
		/// <param name="boneOrObject"> the bone or object to calculate the bounding box for </param>
		/// <returns> the box for the given bone or object </returns>
		/// <exception cref="NullPointerException"> if no object info for the given bone or object exists </exception>
		public virtual Box GetBox(Bone boneOrObject)
		{
			Entity.ObjectInfo info = GetObjectInfoFor(boneOrObject);
			this.PrevBBox.CalcFor(boneOrObject, info);
			return this.PrevBBox;
		}

		/// <summary>
		/// Returns whether the given point at x,y lies inside the box of the given bone or object. </summary>
		/// <param name="boneOrObject"> the bone or object </param>
		/// <param name="x"> the x value of the point </param>
		/// <param name="y"> the y value of the point </param>
		/// <returns> <code>true</code> if x,y lies inside the box of the given bone or object </returns>
		/// <exception cref="NullPointerException"> if no object info for the given bone or object exists </exception>
		public virtual bool CollidesFor(Bone boneOrObject, float x, float y)
		{
			Entity.ObjectInfo info = GetObjectInfoFor(boneOrObject);
			this.PrevBBox.CalcFor(boneOrObject, info);
			return this.PrevBBox.Collides(boneOrObject, info, x, y);
		}

		/// <summary>
		/// Returns whether the given point lies inside the box of the given bone or object. </summary>
		/// <param name="bone"> the bone or object </param>
		/// <param name="point"> the point </param>
		/// <returns> <code>true</code> if the point lies inside the box of the given bone or object </returns>
		/// <exception cref="NullPointerException"> if no object info for the given bone or object exists </exception>
		public virtual bool CollidesFor(Bone boneOrObject, Point point)
		{
			return this.CollidesFor(boneOrObject, point.X, point.Y);
		}

		/// <summary>
		/// Returns whether the given area collides with the box of the given bone or object. </summary>
		/// <param name="boneOrObject"> the bone or object </param>
		/// <param name="area"> the rectangular area </param>
		/// <returns> <code>true</code> if the area collides with the bone or object </returns>
		public virtual bool CollidesFor(Bone boneOrObject, Rectangle area)
		{
			Entity.ObjectInfo info = GetObjectInfoFor(boneOrObject);
			this.PrevBBox.CalcFor(boneOrObject, info);
			return this.PrevBBox.IsInside(area);
		}

		/// <summary>
		/// Sets the given values of the bone with the given name. </summary>
		/// <param name="name"> the name of the bone </param>
		/// <param name="x"> the new x value of the bone </param>
		/// <param name="y"> the new y value of the bone </param>
		/// <param name="angle"> the new angle of the bone </param>
		/// <param name="scaleX"> the new scale in x direction of the bone </param>
		/// <param name="scaleY"> the new scale in y direction of the bone </param>
		/// <exception cref="SpriterException"> if no bone exists of the given name </exception>
		public virtual void SetBone(string name, float x, float y, float angle, float scaleX, float scaleY)
		{
			int index = GetBoneIndex(name);
			if (index == -1)
			{
				throw new SpriterException("No bone found of name \"" + name + "\"");
			}
			Mainline.Key.BoneRef @ref = CurrentKey.GetBoneRef(index);
			Bone bone = GetBone(index);
			bone.Set(x, y, angle, scaleX, scaleY, 0f, 0f);
			UnmapObjects(@ref);
		}

		/// <summary>
		/// Sets the given values of the bone with the given name. </summary>
		/// <param name="name"> the name of the bone </param>
		/// <param name="position"> the new position of the bone </param>
		/// <param name="angle"> the new angle of the bone </param>
		/// <param name="scale"> the new scale of the bone </param>
		/// <exception cref="SpriterException"> if no bone exists of the given name </exception>
		public virtual void SetBone(string name, Point position, float angle, Point scale)
		{
			this.SetBone(name, position.X, position.Y, angle, scale.X, scale.Y);
		}

		/// <summary>
		/// Sets the given values of the bone with the given name. </summary>
		/// <param name="name"> the name of the bone </param>
		/// <param name="x"> the new x value of the bone </param>
		/// <param name="y"> the new y value of the bone </param>
		/// <param name="angle"> the new angle of the bone </param>
		/// <exception cref="SpriterException"> if no bone exists of the given name </exception>
		public virtual void SetBone(string name, float x, float y, float angle)
		{
			Bone b = GetBone(name);
			SetBone(name, x, y, angle, b.Scale.X, b.Scale.Y);
		}

		/// <summary>
		/// Sets the given values of the bone with the given name. </summary>
		/// <param name="name"> the name of the bone </param>
		/// <param name="position"> the new position of the bone </param>
		/// <param name="angle"> the new angle of the bone </param>
		/// <exception cref="SpriterException"> if no bone exists of the given name </exception>
		public virtual void SetBone(string name, Point position, float angle)
		{
			Bone b = GetBone(name);
			SetBone(name, position.X, position.Y, angle, b.Scale.X, b.Scale.Y);
		}

		/// <summary>
		/// Sets the position of the bone with the given name. </summary>
		/// <param name="name"> the name of the bone </param>
		/// <param name="x"> the new x value of the bone </param>
		/// <param name="y"> the new y value of the bone </param>
		/// <exception cref="SpriterException"> if no bone exists of the given name </exception>
		public virtual void SetBone(string name, float x, float y)
		{
			Bone b = GetBone(name);
			SetBone(name, x, y, b.Angle);
		}

		/// <summary>
		/// Sets the position of the bone with the given name. </summary>
		/// <param name="name"> the name of the bone </param>
		/// <param name="position"> the new position of the bone </param>
		/// <exception cref="SpriterException"> if no bone exists of the given name </exception>
		public virtual void SetBone(string name, Point position)
		{
			SetBone(name, position.X, position.Y);
		}

		/// <summary>
		/// Sets the angle of the bone with the given name </summary>
		/// <param name="name"> the name of the bone </param>
		/// <param name="angle"> the new angle of the bone </param>
		/// <exception cref="SpriterException"> if no bone exists of the given name </exception>
		public virtual void SetBone(string name, float angle)
		{
			Bone b = GetBone(name);
			SetBone(name, b.Position.X, b.Position.Y, angle);
		}

		/// <summary>
		/// Sets the values of the bone with the given name to the values of the given bone </summary>
		/// <param name="name"> the name of the bone </param>
		/// <param name="bone"> the bone with the new values </param>
		/// <exception cref="SpriterException"> if no bone exists of the given name </exception>
		public virtual void SetBone(string name, Bone bone)
		{
			SetBone(name, bone.Position, bone.Angle, bone.Scale);
		}

		/// <summary>
		/// Sets the given values of the object with the given name. </summary>
		/// <param name="name"> the name of the object </param>
		/// <param name="x"> the new position in x direction of the object </param>
		/// <param name="y"> the new position in y direction of the object </param>
		/// <param name="angle"> the new angle of the object </param>
		/// <param name="scaleX"> the new scale in x direction of the object </param>
		/// <param name="scaleY"> the new scale in y direction of the object </param>
		/// <param name="pivotX"> the new pivot in x direction of the object </param>
		/// <param name="pivotY"> the new pivot in y direction of the object </param>
		/// <param name="alpha"> the new alpha value of the object </param>
		/// <param name="folder"> the new folder index of the object </param>
		/// <param name="file"> the new file index of the object </param>
		/// <exception cref="SpriterException"> if no object exists of the given name </exception>
		public virtual void SetObject(string name, float x, float y, float angle, float scaleX, float scaleY, float pivotX, float pivotY, float alpha, int folder, int file)
		{
			int index = GetObjectIndex(name);
			if (index == -1)
			{
				throw new SpriterException("No object found for name \"" + name + "\"");
			}
			Mainline.Key.ObjectRef @ref = CurrentKey.GetObjectRef(index);
			Object @object = GetObject(index);
			@object.Set(x, y, angle, scaleX, scaleY, pivotX, pivotY, alpha, folder, file);
			UnmapObjects(@ref);
		}

		/// <summary>
		/// Sets the given values of the object with the given name. </summary>
		/// <param name="name"> the name of the object </param>
		/// <param name="position"> the new position of the object </param>
		/// <param name="angle"> the new angle of the object </param>
		/// <param name="scale"> the new scale of the object </param>
		/// <param name="pivot"> the new pivot of the object </param>
		/// <param name="alpha"> the new alpha value of the object </param>
		/// <param name="ref"> the new file reference of the object </param>
		/// <exception cref="SpriterException"> if no object exists of the given name </exception>
		public virtual void SetObject(string name, Point position, float angle, Point scale, Point pivot, float alpha, FileReference @ref)
		{
			this.SetObject(name, position.X, position.Y, angle, scale.X, scale.Y, pivot.X, pivot.Y, alpha, @ref.Folder, @ref.File);
		}

		/// <summary>
		/// Sets the given values of the object with the given name. </summary>
		/// <param name="name"> the name of the object </param>
		/// <param name="x"> the new position in x direction of the object </param>
		/// <param name="y"> the new position in y direction of the object </param>
		/// <param name="angle"> the new angle of the object </param>
		/// <param name="scaleX"> the new scale in x direction of the object </param>
		/// <param name="scaleY"> the new scale in y direction of the object </param>
		/// <exception cref="SpriterException"> if no object exists of the given name </exception>
		public virtual void SetObject(string name, float x, float y, float angle, float scaleX, float scaleY)
		{
			Object b = GetObject(name);
			SetObject(name, x, y, angle, scaleX, scaleY, b.Pivot.X, b.Pivot.Y, b.Alpha, b.@ref.Folder, b.@ref.File);
		}

		/// <summary>
		/// Sets the given values of the object with the given name. </summary>
		/// <param name="name"> the name of the object </param>
		/// <param name="x"> the new position in x direction of the object </param>
		/// <param name="y"> the new position in y direction of the object </param>
		/// <param name="angle"> the new angle of the object </param>
		/// <exception cref="SpriterException"> if no object exists of the given name </exception>
		public virtual void SetObject(string name, float x, float y, float angle)
		{
			Object b = GetObject(name);
			SetObject(name, x, y, angle, b.Scale.X, b.Scale.Y);
		}

		/// <summary>
		/// Sets the given values of the object with the given name. </summary>
		/// <param name="name"> the name of the object </param>
		/// <param name="position"> the new position of the object </param>
		/// <param name="angle"> the new angle of the object </param>
		/// <exception cref="SpriterException"> if no object exists of the given name </exception>
		public virtual void SetObject(string name, Point position, float angle)
		{
			Object b = GetObject(name);
			SetObject(name, position.X, position.Y, angle, b.Scale.X, b.Scale.Y);
		}

		/// <summary>
		/// Sets the position of the object with the given name. </summary>
		/// <param name="name"> the name of the object </param>
		/// <param name="x"> the new position in x direction of the object </param>
		/// <param name="y"> the new position in y direction of the object </param>
		/// <exception cref="SpriterException"> if no object exists of the given name </exception>
		public virtual void SetObject(string name, float x, float y)
		{
			Object b = GetObject(name);
			SetObject(name, x, y, b.Angle);
		}

		/// <summary>
		/// Sets the position of the object with the given name. </summary>
		/// <param name="name"> the name of the object </param>
		/// <param name="position"> the new position of the object </param>
		/// <exception cref="SpriterException"> if no object exists of the given name </exception>
		public virtual void SetObject(string name, Point position)
		{
			SetObject(name, position.X, position.Y);
		}

		/// <summary>
		/// Sets the position of the object with the given name. </summary>
		/// <param name="name"> the name of the object </param>
		/// <param name="angle"> the new angle of the object </param>
		/// <exception cref="SpriterException"> if no object exists of the given name </exception>
		public virtual void SetObject(string name, float angle)
		{
			Object b = GetObject(name);
			SetObject(name, b.Position.X, b.Position.Y, angle);
		}

		/// <summary>
		/// Sets the position of the object with the given name. </summary>
		/// <param name="name"> the name of the object </param>
		/// <param name="alpha"> the new alpha value of the object </param>
		/// <param name="folder"> the new folder index of the object </param>
		/// <param name="file"> the new file index of the object </param>
		/// <exception cref="SpriterException"> if no object exists of the given name </exception>
		public virtual void SetObject(string name, float alpha, int folder, int file)
		{
			Object b = GetObject(name);
			SetObject(name, b.Position.X, b.Position.Y, b.Angle, b.Scale.X, b.Scale.Y, b.Pivot.X, b.Pivot.Y, alpha, folder, file);
		}

		/// <summary>
		/// Sets the values of the object with the given name to the values of the given object. </summary>
		/// <param name="name"> the name of the object </param>
		/// <param name="object"> the object with the new values </param>
		/// <exception cref="SpriterException"> if no object exists of the given name </exception>
		public virtual void SetObject(string name, Object @object)
		{
			SetObject(name, @object.Position, @object.Angle, @object.Scale, @object.Pivot, @object.Alpha, @object.@ref);
		}

	    /// <summary>
	    /// Maps all object from the parent's coordinate system to the global coordinate system. </summary>
	    /// <param name="base"> the root bone to start at. Set it to <code>null</code> to traverse the whole bone hierarchy. </param>
	    public virtual void UnmapObjects(Mainline.Key.BoneRef @base)
		{
			int start = @base == null ? - 1 : @base.Id - 1;
			for (int i = start + 1; i < CurrentKey.BoneRefs.Length; i++)
			{
				Mainline.Key.BoneRef @ref = CurrentKey.GetBoneRef(i);
				if (@ref.Parent != @base && @base != null)
				{
					continue;
				}
				Bone parent = @ref.Parent == null ? this.Root : this.UnmappedTweenedKeys[@ref.Parent.Timeline].Object;
				UnmappedTweenedKeys[@ref.Timeline].Object.Set(TweenedKeys[@ref.Timeline].Object);
				UnmappedTweenedKeys[@ref.Timeline].Object.Unmap(parent);
				UnmapObjects(@ref);
			}
			foreach (Mainline.Key.ObjectRef @ref in CurrentKey.ObjectRefs)
			{
				if (@ref.Parent != @base && @base != null)
				{
					continue;
				}
				Bone parent = @ref.Parent == null ? this.Root : this.UnmappedTweenedKeys[@ref.Parent.Timeline].Object;
                UnmappedTweenedKeys[@ref.Timeline].Object.Set(TweenedKeys[@ref.Timeline].Object);
                UnmappedTweenedKeys[@ref.Timeline].Object.Unmap(parent);
			}
		}

	    /// <summary>
	    /// Sets the entity for this player instance.
	    /// The animation will be switched to the first one of the new entity. </summary>
	    /// <param name="value"> the new entity </param>
	    /// <exception cref="SpriterException"> if the entity is <code>null</code> </exception>
	    public void SetEntity(Entity value)
	    {
	        if (value == null)
	        {
	            throw new SpriterException("entity can not be null!");
	        }
	        this.Entity = value;
	        int maxAnims = value.SpriterAnimationWithMostTimelines.Timelines();
	        TweenedKeys = new Timeline.Key[maxAnims];
	        UnmappedTweenedKeys = new Timeline.Key[maxAnims];
	        for (int i = 0; i < maxAnims; i++)
	        {
	            Timeline.Key key = new Timeline.Key(i);
	            Timeline.Key keyU = new Timeline.Key(i);
	            key.Object = new Object(new Point(0, 0));
	            keyU.Object = new Object(new Point(0, 0));
	            TweenedKeys[i] = key;
	            UnmappedTweenedKeys[i] = keyU;
	            this.objToTimeline[keyU.Object] = keyU;
	        }
	        this.tempTweenedKeys = TweenedKeys;
	        this.tempUnmappedTweenedKeys = UnmappedTweenedKeys;
	        this.Animation = value.GetAnimation(0);

	    }

	    /// <summary>
	    /// Sets the animation of this player. </summary>
	    /// <param name="animation"> the new animation </param>
	    /// <exception cref="SpriterException"> if the animation is <code>null</code> or the current animation is not a member of the current set entity </exception>
	    public void SetAnimation(SpriterAnimation value)
	    {

	        SpriterAnimation prevAnim = this.Animation;
	        if (value == this.Animation)
	        {
	            return;
	        }
	        if (value == null)
	        {
	            throw new SpriterException("animation can not be null!");
	        }
	        if (!this.Entity.ContainsAnimation(value) && value.Id != -1)
	        {
	            throw new SpriterException("animation has to be in the same entity as the current set one!");
	        }
	        if (value != this.Animation)
	        {
	            Time = 0;
	        }
	        this.Animation = value;
	        int tempTime = this.Time;
	        this.Time = 0;
	        this.Update();
	        this.Time = tempTime;
	        if (OnAnimationChanged != null)
	        {
	           OnAnimationChanged(prevAnim, value);
	        }

	    }

	    /// <summary>
		/// Sets the animation of this player to the one with the given name. </summary>
		/// <param name="name"> the name of the animation </param>
		/// <exception cref="SpriterException"> if no animation exists with the given name </exception>
		public void SetAnimation(string name)
		{
				this.Animation = Entity.GetAnimation(name);
		}

		/// <summary>
		/// Sets the animation of this player to the one with the given index. </summary>
		/// <param name="index"> the index of the animation </param>
		/// <exception cref="IndexOutOfBoundsException"> if the index is out of range </exception>
		public void SetAnimation(int value)
		{
            this.Animation = Entity.GetAnimation(value);
		}


		/// <summary>
		/// Returns a bounding box for this player.
		/// The bounding box is calculated for all bones and object starting from the given root. </summary>
		/// <param name="root"> the starting root. Set it to null to calculate the bounding box for the whole player </param>
		/// <returns> the bounding box </returns>
		public virtual Rectangle GetBoundingRectangle(Mainline.Key.BoneRef root)
		{
			Bone boneRoot = root == null ? this.Root : this.UnmappedTweenedKeys[root.Timeline].Object;
			this.rect.Set(boneRoot.Position.X, boneRoot.Position.Y, boneRoot.Position.X, boneRoot.Position.Y);
			this.CalcBoundingRectangle(root);
			this.rect.CalculateSize();
			return this.rect;
		}

		/// <summary>
		/// Returns a bounding box for this player.
		/// The bounding box is calculated for all bones and object starting from the given root. </summary>
		/// <param name="root"> the starting root. Set it to null to calculate the bounding box for the whole player </param>
		/// <returns> the bounding box </returns>
		public virtual Rectangle GetBoudingRectangle(Bone root)
		{
			return this.GetBoundingRectangle(root == null ? null: GetBoneRef(root));
		}

		private void CalcBoundingRectangle(Mainline.Key.BoneRef root)
		{
			foreach (Mainline.Key.BoneRef @ref in CurrentKey.BoneRefs)
			{
				if (@ref.Parent != root && root != null)
				{
					continue;
				}
				Bone bone = this.UnmappedTweenedKeys[@ref.Timeline].Object;
				this.PrevBBox.CalcFor(bone, Animation.GetTimeline(@ref.Timeline).ObjectInfo);
				Rectangle.SetBiggerRectangle(rect, this.PrevBBox.BoundingRect, rect);
				this.CalcBoundingRectangle(@ref);
			}
			foreach (Mainline.Key.ObjectRef @ref in CurrentKey.ObjectRefs)
			{
				if (@ref.Parent != root)
				{
					continue;
				}
				Bone bone = this.UnmappedTweenedKeys[@ref.Timeline].Object;
				this.PrevBBox.CalcFor(bone, Animation.GetTimeline(@ref.Timeline).ObjectInfo);
				Rectangle.SetBiggerRectangle(rect, this.PrevBBox.BoundingRect, rect);
			}
		}

		/// <summary>
		/// Returns the current main line key based on the current <seealso cref="#time"/>. </summary>
		/// <returns> the current main line key </returns>
		public virtual Mainline.Key CurrentKey
		{
			get
			{
				return this.currentKey;
			}
		}

		/// <summary>
		/// Sets the time for the current time.
		/// The player will make sure that the new time will not exceed the time bounds of the current animation. </summary>
		/// <param name="time"> the new time </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer SetTime(int time)
		{
			Time = time;
			int prevSpeed = this.Speed;
			this.Speed = 0;
			this.IncreaseTime();
			this.Speed = prevSpeed;
			return this;
		}

		/// <summary>
		/// Sets the scale of this player to the given one.
		/// Only uniform scaling is supported. </summary>
		/// <param name="scale"> the new scale. 1f means 100% scale. </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer SetScale(float scale)
		{
			this.Root.Scale.Set(scale * FlippedX(), scale * FlippedY());
			return this;
		}

		/// <summary>
		/// Scales this player based on the current set scale. </summary>
		/// <param name="scale"> the scaling factor. 1f means no scale. </param>
		/// <returns> this player to enable chained operations </returns>
		public SpriterAnimationPlayer Scale(float scale)
		{
			this.Root.Scale.Scale(scale, scale);
			return this;
		}

		/// <summary>
		/// Returns the current scale. </summary>
		/// <returns> the current scale </returns>
		public float ScaleX
		{
			get
			{
				return Root.Scale.X;
			}
		}

		/// <summary>
		/// Flips this player around the x and y axis. </summary>
		/// <param name="x"> whether to flip the player around the x axis </param>
		/// <param name="y"> whether to flip the player around the y axis </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer Flip(bool x, bool y)
		{
			if (x)
			{
				this.FlipX();
			}
			if (y)
			{
				this.FlipY();
			}
			return this;
		}

		/// <summary>
		/// Flips the player around the x axis. </summary>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer FlipX()
		{
			this.Root.Scale.X *= -1;
			return this;
		}

		/// <summary>
		/// Flips the player around the y axis. </summary>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer FlipY()
		{
			this.Root.Scale.Y *= -1;
			return this;
		}

		/// <summary>
		/// Returns whether this player is flipped around the x axis. </summary>
		/// <returns> 1 if this player is not flipped, -1 if it is flipped </returns>
		public virtual int FlippedX()
		{
			return (int) Math.Sign(Root.Scale.X);
		}

		/// <summary>
		/// Returns whether this player is flipped around the y axis. </summary>
		/// <returns> 1 if this player is not flipped, -1 if it is flipped </returns>
		public virtual int FlippedY()
		{
			return (int) Math.Sign(Root.Scale.Y);
		}

		/// <summary>
		/// Sets the position of this player to the given coordinates. </summary>
		/// <param name="x"> the new position in x direction </param>
		/// <param name="y"> the new position in y direction </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer SetPosition(float x, float y)
		{
			this.dirty = true;
			this.position.Set(x,y);
			return this;
		}

		/// <summary>
		/// Sets the position of the player to the given one. </summary>
		/// <param name="position"> the new position </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer SetPosition(Point position)
		{
			return this.SetPosition(position.X, position.Y);
		}

		/// <summary>
		/// Adds the given coordinates to the current position of this player. </summary>
		/// <param name="x"> the amount in x direction </param>
		/// <param name="y"> the amount in y direction </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer TranslatePosition(float x, float y)
		{
			return this.SetPosition(position.X + x, position.Y + y);
		}

		/// <summary>
		/// Adds the given amount to the current position of this player. </summary>
		/// <param name="amount"> the amount to add </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer Translate(Point amount)
		{
			return this.TranslatePosition(amount.X, amount.Y);
		}

		/// <summary>
		/// Returns the current position in x direction. </summary>
		/// <returns> the current position in x direction </returns>
		public virtual float X
		{
			get
			{
				return position.X;
			}
		}

		/// <summary>
		/// Returns the current position in y direction. </summary>
		/// <returns> the current position in y direction </returns>
		public virtual float Y
		{
			get
			{
				return position.Y;
			}
		}

		/// <summary>
		/// Sets the angle of this player to the given angle. </summary>
		/// <param name="angle"> the angle in degrees </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer SetAngle(float angle)
		{
			this.dirty = true;
			this.angle = angle;
			return this;
		}

		/// <summary>
		/// Rotates this player by the given angle. </summary>
		/// <param name="angle"> the angle in degrees </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer Rotate(float angle)
		{
			return this.SetAngle(angle + this.angle);
		}

		/// <summary>
		/// Returns the current set angle. </summary>
		/// <returns> the current angle </returns>
		public virtual float Angle
		{
			get
			{
				return this.angle;
			}
		}

		/// <summary>
		/// Sets the pivot, i.e. origin, of this player.
		/// A pivot at (0,0) means that the origin of the played animation will have the same one as in Spriter. </summary>
		/// <param name="x"> the new pivot in x direction </param>
		/// <param name="y"> the new pivot in y direction </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer SetPivot(float x, float y)
		{
			this.dirty = true;
			this.pivot.Set(x, y);
			return this;
		}

		/// <summary>
		/// Sets the pivot, i.e. origin, of this player.
		/// A pivot at (0,0) means that the origin of the played animation will have the same one as in Spriter. </summary>
		/// <param name="pivot"> the new pivot </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer SetPivot(Point pivot)
		{
			return this.SetPivot(pivot.X, pivot.Y);
		}

		/// <summary>
		/// Translates the current set pivot position by the given amount. </summary>
		/// <param name="x"> the amount in x direction </param>
		/// <param name="y"> the amount in y direction </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer TranslatePivot(float x, float y)
		{
			return this.SetPivot(pivot.X + x, pivot.Y + y);
		}

		/// <summary>
		/// Adds the given amount to the current set pivot position. </summary>
		/// <param name="amount"> the amount to add </param>
		/// <returns> this player to enable chained operations </returns>
		public virtual SpriterAnimationPlayer TranslatePivot(Point amount)
		{
			return this.TranslatePivot(amount.X, amount.Y);
		}

		/// <summary>
		/// Returns the current set pivot in x direction. </summary>
		/// <returns> the pivot in x direction </returns>
		public virtual float PivotX
		{
			get
			{
				return pivot.X;
			}
		}

		/// <summary>
		/// Returns the current set pivot in y direction. </summary>
		/// <returns> the pivot in y direction </returns>
		public virtual float PivotY
		{
			get
			{
				return pivot.Y;
			}
		}

		/// <summary>
		/// Returns an iterator to iterate over all time line bones in the current animation. </summary>
		/// <returns> the bone iterator </returns>
		public virtual IEnumerator<Bone> BoneIterator()
		{
			return this.BoneIterator(this.CurrentKey.BoneRefs[0]);
		}

		/// <summary>
		/// Returns an iterator to iterate over all time line bones in the current animation starting at a given root. </summary>
		/// <param name="start"> the bone reference to start at </param>
		/// <returns> the bone iterator </returns>
		public virtual IEnumerator<Bone> BoneIterator(Mainline.Key.BoneRef start)
		{
			this.boneIterator.Index = start.Id;
			return this.boneIterator;
		}

		/// <summary>
		/// Returns an iterator to iterate over all time line objects in the current animation. </summary>
		/// <returns> the object iterator </returns>
		public virtual IEnumerator<Object> ObjectIterator()
		{
			return this.ObjectIterator(this.CurrentKey.ObjectRefs[0]);
		}

		/// <summary>
		/// Returns an iterator to iterate over all time line objects in the current animation starting at a given root. </summary>
		/// <param name="start"> the object reference to start at </param>
		/// <returns> the object iterator </returns>
		public virtual IEnumerator<Object> ObjectIterator(Mainline.Key.ObjectRef start)
		{
			this.objectIterator.Index = start.Id;
			return this.objectIterator;
		}

		/// <summary>
		/// An attachment is an abstract object which can be attached to a <seealso cref="SpriterAnimationPlayer"/> object.
		/// An attachment extends a <seealso cref="Bone"/> which means that <seealso cref="Bone#position"/>, <seealso cref="Bone#scale"/> and <seealso cref="Bone#angle"/> can be set to change the relative position to its <seealso cref="Attachment#parent"/>
		/// The <seealso cref="SpriterAnimationPlayer"/> object will make sure that the attachment will be transformed relative to its <seealso cref="Attachment#parent"/>.
		/// @author Trixt0r
		/// 
		/// </summary>
		public abstract class Attachment : Bone
		{

			internal Bone Parent_Renamed;
			internal readonly Point PositionTemp, ScaleTemp;
			internal float AngleTemp;

			public Attachment(Bone parent)
			{
				this.PositionTemp = new Point();
				this.ScaleTemp = new Point();
				this.Parent = parent;
			}

			public virtual Bone Parent
			{
				set
				{
					if (value == null)
					{
						throw new SpriterException("The parent cannot be null!");
					}
					this.Parent_Renamed = value;
				}
                get
                {
                    return this.Parent_Renamed;
                }
			}

		    internal void Update()
		    {
		        this.PositionTemp.Set(base.Position);
		        this.ScaleTemp.Set(base.Scale);
		        this.AngleTemp = base.Angle;
		        base.Unmap(Parent_Renamed);
		        this.SetPosition(base.Position.X, base.Position.Y);
		        this.SetScale(base.Scale.X, base.Scale.Y);
		        this.SetAngle(base.Angle);
		        base.Position.Set(this.PositionTemp);
		        base.Scale.Set(this.ScaleTemp);
		        base.Angle = this.AngleTemp;
		    }

		    protected internal abstract void SetPosition(float x, float y);
			protected internal abstract void SetScale(float xscale, float yscale);
		    protected internal abstract float SetAngle(float angle);
		}
	}

    public class BoneIterator : IEnumerator<Bone>
    {
        private SpriterAnimationPlayer SpriterAnimationPlayer;
        internal int Index = 0;
        public BoneIterator(SpriterAnimationPlayer SpriterAnimationPlayer)
        {
            this.SpriterAnimationPlayer = SpriterAnimationPlayer;
        }
        public bool MoveNext()
        {
            if (Index < SpriterAnimationPlayer.CurrentKey.BoneRefs.Length)
            {
                Current = SpriterAnimationPlayer.UnmappedTweenedKeys[SpriterAnimationPlayer.CurrentKey.BoneRefs[Index++].Timeline].Object;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            Index = 0;
        }

        public Bone Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
            // TODO mitä?
        }
    }

    public class ObjectIterator : IEnumerator<Object>
    {
        private SpriterAnimationPlayer SpriterAnimationPlayer;
        internal int Index = 0;
        public ObjectIterator(SpriterAnimationPlayer SpriterAnimationPlayer)
        {
            this.SpriterAnimationPlayer = SpriterAnimationPlayer;
        }

        public void Dispose()
        {
            // TODO mitä?
        }

        public bool MoveNext()
        {
            if (Index < SpriterAnimationPlayer.CurrentKey.ObjectRefs.Length)
            {
                Current = SpriterAnimationPlayer.UnmappedTweenedKeys[SpriterAnimationPlayer.CurrentKey.ObjectRefs[Index++].Timeline].Object;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            Index = 0;
        }

        public Object Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}