using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaNi.Spriter.Data
{
    class SpriterAnimation
    {
        public readonly Mainline Mainline;
        private readonly Timeline[] timelines;
        private int timelinePointer;
        private readonly Dictionary<string, Timeline> nameToTimeline;
        public readonly int ID, Length;
        public readonly string Name;
        public readonly bool Looping;

        internal Key CurrentKey;
        internal TimelineKey[] TweenedKeys, UnmappedTweenedKeys;
        private bool prepared;

        public SpriterAnimation(Mainline mainline, int id, string name, int length, bool looping, int timelineCount)
        {
            Mainline = mainline;
            ID = id;
            Name = name;
            Length = length;
            Looping = looping;
            timelines = new Timeline[timelineCount];
            prepared = false;
            nameToTimeline = new Dictionary<string, Timeline>();
        }

        #region Indexer
        public Timeline this[int index]
        {
            get { return timelines[index]; }
        }

        public Timeline this[string name]
        {
            get { return nameToTimeline[name]; }
        }
        #endregion

        #region Internal

        internal void AddTimeline(Timeline tl)
        {
            timelines[timelinePointer++] = tl;
            nameToTimeline[tl.Name] = tl;
        }

        #endregion

        #region Properties

        public int Timelines
        {
            get { return timelines.Length; }
        }

        #endregion

        #region Methods

        public void Update(int time, Bone root)
        {
            if (!prepared) throw new Exception("This animation is not ready yet to animate itself, Please call Prepare()!");
            if (root == null) throw new ArgumentNullException("root", "root cannot be null");
            CurrentKey = Mainline.GetKeyBeforeTime(time);

            foreach (var timelineKey in UnmappedTweenedKeys)
            {
                timelineKey.Active = false;
                foreach (var boneRef in CurrentKey.BoneRefs)
                {
                    Update(boneRef, root, time);
                }
                foreach (var objectRef in CurrentKey.ObjectRefs)
                {
                    Update(objectRef, root, time);
                }
            }
        }

        protected void Update(BoneRef boneRef, Bone root, int time)
        {
            bool isObject = boneRef as ObjectRef != null;
            Timeline timeline = this[boneRef.Timeline];
            TimelineKey key = timeline[boneRef.Key];
            TimelineKey nextKey = timeline[(boneRef.Key + 1)%timeline.Keys.Length];

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


            // normalize time
            float t = (float) (time - currentTime)/(float) (nextTime - currentTime);
            if (float.IsNaN(t) || float.IsInfinity(t))
            {
                t = 1f;
            }
            if (CurrentKey.Time > currentTime)
            {
                float tMid = (float) (CurrentKey.Time - currentTime)/(float) (nextTime - currentTime);
                if (float.IsNaN(tMid) || float.IsInfinity(tMid))
                {
                    tMid = 0f;
                }
                t = (float) (time - CurrentKey.Time)/(float) (nextTime - CurrentKey.Time);
                if (float.IsNaN(t) || float.IsInfinity(t))
                {
                    t = 1f;
                }
                t = CurrentKey.Curve.tween(tMid, 1f, t);
            }
            else
            {
                t = CurrentKey.Curve.tween(0f, 1f, t);
            }

            // tween bone/object
            Bone bone1 = key.Object;
            Bone bone2 = nextKey.Object;

            Bone tweenTarget = TweenedKeys[boneRef.Timeline].Object;
            if (isObject)
            {
                TweenObject(bone1 as SpriterObject, bone2 as SpriterObject, tweenTarget as SpriterObject, t, key.Curve,
                    key.Spin);
            }
            else
            {
                TweenBone(bone1, bone2, tweenTarget, t, key.Curve, key.Spin);
            }
            UnmappedTweenedKeys[boneRef.Timeline].Active = true;
            UnmapTimelineObject(boneRef.Timeline, isObject,
                (boneRef.Parent != null) ? UnmappedTweenedKeys[boneRef.Parent.Timeline].Object : root);
        }

        private void TweenObject(SpriterObject object1, SpriterObject object2, SpriterObject target, float t, Curve curve, int spin)
        {
            TweenBone(object1, object2, target, t, curve, spin);
            target.Alpha = curve.tweenAngle(object1.Alpha, object2.Alpha, t);
            target.Ref.Set(object1.Ref);
        }

        private void TweenBone(Bone bone1, Bone bone2, Bone target, float t, Curve curve, int spin)
        {
            target.Angle = curve.tweenAngle(bone1.Angle, bone2.Angle, t, spin);
            curve.tweenPoint(bone1.Position, bone2.Position, t, ref target.Position);
            curve.tweenPoint(bone1.Scale, bone2.Scale, t, ref target.Scale);
            curve.tweenPoint(bone1.Pivot, bone2.Pivot, t, ref target.Pivot);
        }

        private void UnmapTimelineObject(int timeline, bool isObject, Bone root)
        {
            Bone tweenTarget = TweenedKeys[timeline].Object;
            Bone mapTarget = UnmappedTweenedKeys[timeline].Object;
            if (isObject)
            {
                (mapTarget as SpriterObject).Set(tweenTarget as SpriterObject);
            }
            else
            {
                mapTarget.Set(tweenTarget);
            }
            mapTarget.Unmap(root);
        }

        internal Timeline GetSimilarTimeline(Timeline t)
        {
            Timeline found = this[t.Name];
            if (found == null && t.ID < Timelines) found = this[t.ID];
            return found;
        }

        public void Prepare()
        {
            if (prepared) return;
            TweenedKeys = new TimelineKey[Timelines];
            UnmappedTweenedKeys = new TimelineKey[Timelines];

            for (int i = 0; i < TweenedKeys.Length; i++)
            {
                TweenedKeys[i] = new TimelineKey(i);
                UnmappedTweenedKeys[i] = new TimelineKey(i);
                throw new NotImplementedException();
                // TODO jotku objectit
                //TweenedKeys[i].SetObject(new Time)
            }
        }

        #endregion

    }
}
