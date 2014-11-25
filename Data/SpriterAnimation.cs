using System.Collections.Generic;

namespace SaNi.Spriter.Data
{
    /// <summary>
	/// Represents an animation of a Spriter SCML file.
	/// An animation holds <seealso cref="Timeline"/>s and a <seealso cref="Mainline"/> to animate objects.
	/// Furthermore it holds an <seealso cref="#id"/>, a <seealso cref="#length"/>, a <seealso cref="#name"/> and whether it is <seealso cref="#looping"/> or not.
	/// @author Trixt0r
	/// 
	/// </summary>
	public class SpriterAnimation
	{

		public readonly Mainline Mainline;
		private readonly Timeline[] timelines;
		private int timelinePointer = 0;
		private readonly Dictionary<string, Timeline> nameToTimeline;
		public readonly int Id, Length;
		public readonly string Name;
		public readonly bool Looping;
		internal Mainline.Key CurrentKey;
		internal Timeline.Key[] TweenedKeys, UnmappedTweenedKeys;
		private bool prepared;

		public SpriterAnimation(Mainline mainline, int id, string name, int length, bool looping, int timelines)
		{
			this.Mainline = mainline;
			this.Id = id;
			this.Name = name;
			this.Length = length;
			this.Looping = looping;
			this.timelines = new Timeline[timelines];
			this.prepared = false;
			this.nameToTimeline = new Dictionary<string, Timeline>();
			//this.currentKey = mainline.getKey(0);
		}

		/// <summary>
		/// Returns a <seealso cref="Timeline"/> with the given index. </summary>
		/// <param name="index"> the index of the timeline </param>
		/// <returns> the timeline with the given index </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the index is out of range </exception>
		public virtual Timeline GetTimeline(int index)
		{
			return this.timelines[index];
		}

		/// <summary>
		/// Returns a <seealso cref="Timeline"/> with the given name. </summary>
		/// <param name="name"> the name of the time line </param>
		/// <returns> the time line with the given name or null if no time line exists with the given name. </returns>
		public virtual Timeline GetTimeline(string name)
		{
			return this.nameToTimeline[name];
		}

		internal virtual void AddTimeline(Timeline timeline)
		{
			this.timelines[timelinePointer++] = timeline;
			this.nameToTimeline[timeline.Name] = timeline;
		}

		/// <summary>
		/// Returns the number of time lines this animation holds. </summary>
		/// <returns> the number of time lines </returns>
		public virtual int Timelines()
		{
			return timelines.Length;
		}

		public override string ToString()
		{
			string toReturn = this.GetType().Name + "|[id: " + Id + ", " + Name + ", duration: " + Length + ", is looping: " + Looping;
			toReturn += "Mainline:\n";
			toReturn += Mainline;
			toReturn += "Timelines\n";
			foreach (Timeline timeline in this.timelines)
			{
				toReturn += timeline;
			}
			toReturn += "]";
			return toReturn;
		}

	    /// <summary>
	    /// Updates the bone and object structure with the given time to the given root bone. </summary>
	    /// <param name="time"> The time which has to be between 0 and <seealso cref="#length"/> to work properly. </param>
	    /// <param name="root"> The root bone which is not allowed to be null. The whole animation runs relative to the root bone. </param>
	    public virtual void Update(int time, Bone root)
		{
			if (!this.prepared)
			{
				throw new SpriterException("This animation is not ready yet to animate itself. Please call prepare()!");
			}
			if (root == null)
			{
				throw new SpriterException("The root can not be null! Set a root bone to apply this animation relative to the root bone.");
			}
			this.CurrentKey = Mainline.GetKeyBeforeTime(time);

			foreach (Timeline.Key timelineKey in this.UnmappedTweenedKeys)
			{
				timelineKey.Active = false;
			}
			foreach (Mainline.Key.BoneRef @ref in CurrentKey.BoneRefs)
			{
				this.Update(@ref, root, time);
			}
			foreach (Mainline.Key.ObjectRef @ref in CurrentKey.ObjectRefs)
			{
				this.Update(@ref, root, time);
			}
		}

		protected internal virtual void Update(Mainline.Key.BoneRef @ref, Bone root, int time)
		{
			bool isObject = @ref is Mainline.Key.ObjectRef;
			//Get the timelines, the refs pointing to
			Timeline timeline = GetTimeline(@ref.Timeline);
			Timeline.Key key = timeline.GetKey(@ref.Key);
			Timeline.Key nextKey = timeline.GetKey((@ref.Key + 1) % timeline.Keys.Length);
			int currentTime = key.Time;
			int nextTime = nextKey.Time;
			if (nextTime < currentTime)
			{
				if (!Looping)
				{
					nextKey = key;
				}
				else
				{
					nextTime = Length;
				}
			}
			//Normalize the time
			float t = (float)(time - currentTime) / (float)(nextTime - currentTime);
			if (float.IsNaN(t) || float.IsInfinity(t))
			{
				t = 1f;
			}
			if (CurrentKey.Time > currentTime)
			{
				float tMid = (float)(CurrentKey.Time - currentTime) / (float)(nextTime - currentTime);
				if (float.IsNaN(tMid) || float.IsInfinity(tMid))
				{
					tMid = 0f;
				}
				t = (float)(time - CurrentKey.Time) / (float)(nextTime - CurrentKey.Time);
				if (float.IsNaN(t) || float.IsInfinity(t))
				{
					t = 1f;
				}
				t = CurrentKey.Curve.Tween(tMid, 1f, t);
			}
			else
			{
				t = CurrentKey.Curve.Tween(0f, 1f, t);
			}
			//Tween bone/object
			Bone bone1 = key.Object;
			Bone bone2 = nextKey.Object;
			Bone tweenTarget = this.TweenedKeys[@ref.Timeline].Object;
			if (isObject)
			{
                this.TweenObject((SpriterObject)bone1, (SpriterObject)bone2, (SpriterObject)tweenTarget, t, key.Curve, key.Spin);
			}
			else
			{
				this.TweenBone(bone1, bone2, tweenTarget, t, key.Curve, key.Spin);
			}
			this.UnmappedTweenedKeys[@ref.Timeline].Active = true;
			this.UnmapTimelineObject(@ref.Timeline, isObject,(@ref.Parent != null) ? this.UnmappedTweenedKeys[@ref.Parent.Timeline].Object: root);
		}

		internal virtual void UnmapTimelineObject(int timeline, bool isObject, Bone root)
		{
			Bone tweenTarget = this.TweenedKeys[timeline].Object;
			Bone mapTarget = this.UnmappedTweenedKeys[timeline].Object;
			if (isObject)
			{
                ((SpriterObject)mapTarget).Set((SpriterObject)tweenTarget);
			}
			else
			{
				mapTarget.Set(tweenTarget);
			}
			mapTarget.Unmap(root);
		}

		protected internal virtual void TweenBone(Bone bone1, Bone bone2, Bone target, float t, Curve curve, int spin)
		{
			target.Angle = curve.TweenAngle(bone1.Angle, bone2.Angle, t, spin);
			curve.TweenPoint(bone1.Position, bone2.Position, t, target.Position);
			curve.TweenPoint(bone1.Scale, bone2.Scale, t, target.Scale);
			curve.TweenPoint(bone1.Pivot, bone2.Pivot, t, target.Pivot);
		}

        protected internal virtual void TweenObject(SpriterObject object1, SpriterObject object2, SpriterObject target, float t, Curve curve, int spin)
		{
			this.TweenBone(object1, object2, target, t, curve, spin);
			target.Alpha = curve.TweenAngle(object1.Alpha, object2.Alpha, t);
			target.@ref.Set(object1.@ref);
		}

		internal virtual Timeline GetSimilarTimeline(Timeline t)
		{
			Timeline found = GetTimeline(t.Name);
			if (found == null && t.Id < this.Timelines())
			{
				found = this.GetTimeline(t.Id);
			}
			return found;
		}

		/*Timeline getSimilarTimeline(BoneRef ref, Collection<Timeline> coveredTimelines){
			if(ref.Parent == null) return null;
			for(BoneRef boneRef: this.currentKey.objectRefs){
				Timeline t = this.getTimeline(boneRef.Timeline);
				if(boneRef.Parent != null && boneRef.Parent.id == ref.Parent.id && !coveredTimelines.contains(t))
					return t;
			}
			return null;
		}
		
		Timeline getSimilarTimeline(ObjectRef ref, Collection<Timeline> coveredTimelines){
			if(ref.Parent == null) return null;
			for(ObjectRef objRef: this.currentKey.objectRefs){
				Timeline t = this.getTimeline(objRef.Timeline);
				if(objRef.Parent != null && objRef.Parent.id == ref.Parent.id && !coveredTimelines.contains(t))
					return t;
			}
			return null;
		}*/

		/// <summary>
		/// Prepares this animation to set this animation in any time state.
		/// This method has to be called before <seealso cref="#update(int, Bone)"/>.
		/// </summary>
		public virtual void Prepare()
		{
			if (this.prepared)
			{
				return;
			}
			this.TweenedKeys = new Timeline.Key[timelines.Length];
			this.UnmappedTweenedKeys = new Timeline.Key[timelines.Length];

			for (int i = 0; i < this.TweenedKeys.Length; i++)
			{
				this.TweenedKeys[i] = new Timeline.Key(i);
				this.UnmappedTweenedKeys[i] = new Timeline.Key(i);
				this.TweenedKeys[i].Object = new SpriterObject(new Point(0,0));
				this.UnmappedTweenedKeys[i].Object = new SpriterObject(new Point(0,0));
			}
			if (Mainline.Keys.Length > 0)
			{
				CurrentKey = Mainline.GetKey(0);
			}
			this.prepared = true;
		}

	}

}