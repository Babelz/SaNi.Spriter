using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Microsoft.Xna.Framework;
using SaNi.Spriter.Data;

namespace SaNi.Spriter
{
    public delegate void AnimationFinishedEventHandler(SpriterAnimation animation);
    public delegate void AnimationChangedEventHandler(SpriterAnimation old, SpriterAnimation newAnim);
    public delegate void PlayerProcessEventHandler(SpriterAnimationPlayer player);
    public delegate void MainlineKeyChangedEventHandler(Key previous, Key newKey);

    public class SpriterAnimationPlayer
    {
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

        #region Vars
        protected SpriterEntity Entity { get; set; }

        private Vector2 pivot = Vector2.Zero;
        private Vector2 position = Vector2.Zero;
        private readonly Dictionary<SpriterObject, TimelineKey> objToTimeline;
        private float angle;
        private bool dirty = true;
        private Rectangle rect;

        private BoneIterator boneIterator;
        private ObjectIterator objectIterator;
        private Key currentKey, previousKey;
        private TimelineKey[] tempTweenedKeys, tempUnmappedTweenedKeys;
        #region Public

        public int Speed;
        public readonly List<Attachment> Attachments;
        public CharacterMap[] CharacterMaps;
        public bool copyObjects = true;

        #endregion

        #region Internals
        internal SpriterAnimation Animation;
        internal int Time;
        internal TimelineKey[] TweenedKeys, UnmappedTweenedKeys;
        internal Bone Root;


        #endregion

        #endregion

        #region Ctor

        public SpriterAnimationPlayer(SpriterEntity entity)
        {
            boneIterator = new BoneIterator(this);
            objectIterator = new ObjectIterator(this);
            Speed = 15;
            rect = new Rectangle();
            Attachments = new List<Attachment>();
            objToTimeline = new Dictionary<SpriterObject, TimelineKey>();
            SetEntity(entity);
        }

        #endregion

        #region Methods

        private void SetEntity(SpriterEntity entity)
        {
            Entity = entity;
            int maxAnims = entity.GetAnimationWithMostTimelines().Timelines;
            TweenedKeys = new TimelineKey[maxAnims];
            UnmappedTweenedKeys = new TimelineKey[maxAnims];

            for (int i = 0; i < maxAnims; i++)
            {
                TimelineKey key = new TimelineKey(i);
                TimelineKey keyU = new TimelineKey(i);
                key.Object = new SpriterObject(Vector2.Zero);
                keyU.Object = new SpriterObject(Vector2.Zero);
                TweenedKeys[i] = key;
                UnmappedTweenedKeys[i] = keyU;
                objToTimeline[keyU.Object] = keyU;
            }

            tempTweenedKeys = TweenedKeys;
            tempUnmappedTweenedKeys = UnmappedTweenedKeys;
            SetAnimation(Entity.GetAnimation(0));
        }

        public void SetAnimation(SpriterAnimation animation)
        {
            SpriterAnimation prevAnimation = Animation;
            if (animation == Animation) return;

            if (!Entity.ContainsAnimation(animation) && animation.ID != -1)
            {
                throw new Exception("Animation has to be in same entity as the current set one");
            }
            if (animation != Animation) Time = 0;

            Animation = animation;
            int tempTime = Time;
            Time = 0;
            Update();
            Time = tempTime;
            if (OnAnimationChanged != null)
            {
                OnAnimationChanged(prevAnimation, animation);
            }
        }

        public void SetAnimation(string name)
        {
            SetAnimation(Entity.GetAnimation(name));
        }

        public void SetAnimation(int index)
        {
            SetAnimation(Entity.GetAnimation(index));
        }

        public SpriterAnimationPlayer SetTime(int time)
        {
            Time = time;
            int prevSpeed = Speed;
            Speed = 0;
            IncreaseTime();
            Speed = prevSpeed;
            return this;
        }

        public SpriterAnimationPlayer SetScale(float scale)
        {
            Root.Scale.X = scale*FlippedX;
            Root.Scale.Y = scale * FlippedY;
            return this;
        }

        public SpriterAnimationPlayer Scale(float scale)
        {
            Root.Scale *= scale;
            return this;
        }

        public float GetScale()
        {
            return Root.Scale.X;
        }

        public SpriterAnimationPlayer Flip(bool x, bool y)
        {
            if (x) FlipX();
            if (y) FlipY();
            return this;
        }

        public SpriterAnimationPlayer FlipX()
        {
            Root.Scale.X *= -1;
            return this;
        }

        public SpriterAnimationPlayer FlipY()
        {
            Root.Scale.Y *= -1;
            return this;
        }

        public int FlippedX
        {
            get
            {
                return (int) Math.Sign(Root.Scale.X);
            }
        }

        public int FlippedY
        {
            get
            {
                return (int)Math.Sign(Root.Scale.Y);
            }
        }

        public SpriterAnimationPlayer SetPosition(float x, float y)
        {
            dirty = true;
            position.X = x;
            position.Y = y;
            return this;
        }

        public SpriterAnimationPlayer SetPosition(Vector2 pos)
        {
            return SetPosition(pos.X, pos.Y);
        }

        public SpriterAnimationPlayer TranslatePosition(float x, float y)
        {
            return SetPosition(position.X + x, position.Y + y);
        }


        public SpriterAnimationPlayer TranslatePosition(Vector2 amount)
        {
            return TranslatePosition(amount.X, amount.Y);
        }

        public float X
        {
            get { return position.X;  }
        }

        public float Y
        {
            get { return position.X; }
        }

        public SpriterAnimationPlayer SetAngle(float angle)
        {
            dirty = true;
            this.angle = angle;
            return this;
        }

        public SpriterAnimationPlayer Rotate(float angle)
        {
            return SetAngle(angle + this.angle);
        }

        public SpriterAnimationPlayer SetPivot(float x, float y)
        {
            dirty = true;
            pivot.X = x;
            pivot.Y = y;
            return this;
        }

        public SpriterAnimationPlayer SetPivot(Vector2 p)
        {
            return SetPivot(p.X, p.Y);
        }

        public SpriterAnimationPlayer TranslatePivot(float x, float y)
        {
            return SetPivot(pivot.X + x, pivot.Y + y);
        }

        public SpriterAnimationPlayer TranslatePivot(Vector2 amount)
        {
            return TranslatePivot(amount.X, amount.Y);
        }

        public float PivotX
        {
            get { return pivot.X;  }
        }

        public float PivotY
        {
            get { return pivot.Y; }
        }


        public BoneIterator BoneIterator()
        {
            return BoneIterator(GetCurrentKey().BoneRefs[0]);
        }

        public BoneIterator BoneIterator(BoneRef start)
        {
            boneIterator.index = start.ID;
            return boneIterator;
        }

        public ObjectIterator ObjectIterator(ObjectRef start)
        {
            objectIterator.index = start.ID;
            return objectIterator;
        }

        public ObjectIterator ObjectIterator()
        {
            return ObjectIterator(GetCurrentKey().ObjectRefs[0]);
        }

        public void Update()
        {
            if (OnPreProcess != null)
            {
                OnPreProcess(this);
            }
            if (dirty)
            {
                UpdateRoot();
            }

            Animation.Update(Time, Root);
            currentKey = Animation.CurrentKey;
            if (previousKey != currentKey)
            {
                if (OnMainlineKeyChanged != null)
                {
                    OnMainlineKeyChanged(previousKey, currentKey);
                }
                previousKey = currentKey;
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

            foreach (var attachment in Attachments)
            {
                attachment.Update();
            }

            if (OnPostProcess != null)
            {
                OnPostProcess(this);
            }
            IncreaseTime();
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

        private void CopyObjects()
        {
            for (int i = 0; i < Animation.TweenedKeys.Length; i++)
            {
                TweenedKeys[i].Active = Animation.TweenedKeys[i].Active;
                UnmappedTweenedKeys[i].Active = Animation.UnmappedTweenedKeys[i].Active;
                TweenedKeys[i].Object.Set(Animation.TweenedKeys[i].Object);
                UnmappedTweenedKeys[i].Object.Set(Animation.UnmappedTweenedKeys[i].Object);
            }
        }

        private void UpdateRoot()
        {
            Root.Angle = angle;
            Root.Position = pivot;
            Root.Position.Rotate(angle);
            Root.Position += position;
            dirty = false;
        }

        public Bone GetBone(int index)
        {
            return UnmappedTweenedKeys[GetCurrentKey().GetBoneRef(index).Timeline].Object;
        }

        public Bone GetBone(string name)
        {
            return UnmappedTweenedKeys[Animation[name].ID].Object;
        }

        public SpriterObject GetObject(int index)
        {
            return UnmappedTweenedKeys[GetCurrentKey().GetBoneRef(index).Timeline].Object;
        }

        public int GetBoneIndex(string name)
        {
            BoneRef b = GetCurrentKey().BoneRefs.FirstOrDefault(bref => name == Animation[bref.Timeline].Name);
            if (b == null)
                return -1;
            return b.ID;
        }

        public BoneRef GetBoneRef(Bone bone)
        {
            return GetCurrentKey().GetBoneRefByTimeline(objToTimeline[bone as SpriterObject].ID);
        }

        public int GetObjectIndex(string name)
        {
            ObjectRef oref = GetCurrentKey().ObjectRefs.FirstOrDefault(o => Animation[o.Timeline].Name == name);
            if (oref == null) return -1;
            return oref.ID;
        }

        public SpriterObject GetObject(string name)
        {
            return UnmappedTweenedKeys[Animation[name].ID].Object;
        }

        public ObjectRef GetObjectRef(SpriterObject obj)
        {
            return GetCurrentKey().GetObjectRefByTimeline(objToTimeline[obj].ID);
        }

        public string GetNameFor(Bone boneOrObject)
        {
            return Animation[objToTimeline[boneOrObject as SpriterObject].ID].Name;
        }

        public ObjectInfo GetObjectInfoFor(Bone boneOrObject)
        {
            return Animation[objToTimeline[boneOrObject as SpriterObject].ID].ObjectInfo;
        }

        public TimelineKey GetKeyFor(Bone boneOrObject)
        {
            return objToTimeline[boneOrObject as SpriterObject];
        }

        public object GetBox(Bone boneOrObject)
        {
            throw new NotImplementedException();
        }

        public bool CollidesFor(Bone boneOrObject, float x, float y)
        {
            throw new NotImplementedException();
        }

        public bool CollidesFor(Bone boneOrObject, Vector2 v)
        {
            return CollidesFor(boneOrObject, v.X, v.Y);
        }

        public bool CollidesFor(Bone boneOrObject, Rectangle are)
        {
            throw new NotImplementedException();
        }

        public void SetBone(string name, float x, float y, float angle, float scaleX, float scaleY)
        {
            int index = GetBoneIndex(name);
            if (index == -1) throw new Exception("No bone found for name " + name);

            BoneRef bref = GetCurrentKey().GetBoneRef(index);
            Bone bone = GetBone(index);
            bone.Set(x, y, angle, scaleX, scaleY, 0f, 0f);
            UnmapObjects(bref);
        }

        public void UnmapObjects(BoneRef baseRef)
        {
            throw new NotImplementedException();
        }

        public void SetBone(string name, Vector2 pos, float angle, Vector2 scale)
        {
            SetBone(name, pos.X, pos.Y, angle, scale.X, scale.Y);
        }

        public void SetBone(string name, Vector2 pos, float angle)
        {
            Bone b = GetBone(name);
            SetBone(name, pos.X, pos.Y, angle, b.Scale.X, b.Scale.Y);
        }

        public void SetBone(string name, float x, float y)
        {
            Bone b = GetBone(name);
            SetBone(name, x, y, b.Angle);
        }

        public void SetBone(string name, float x, float y, float angle)
        {
            Bone b = GetBone(name);
            SetBone(name, x, y, angle, b.Scale.X, b.Scale.Y);
        }

        public void SetBone(string name, Vector2 position)
        {
            SetBone(name, position.X, position.Y);
        }

        public void SetBone(string name, float angle)
        {
            Bone b = GetBone(name);
            SetBone(name, b.Position.X, b.Position.Y, angle);
        }

        public void SetBone(string name, Bone bone)
        {
            SetBone(name, bone.Position, bone.Angle, bone.Scale);
        }

        public void SetObject(string name, float x, float y, float angle, float scaleX, float scaleY, float pivotX,
            float pivotY, float alpha, int folder, int file)
        {
            throw new NotImplementedException();
        }
        public Key GetCurrentKey()
        {
            return currentKey;
        }
        #endregion



    }

    public class BoneIterator : IEnumerator<Bone>
    {
        private SpriterAnimationPlayer player;
        internal int index = 0;
        public BoneIterator(SpriterAnimationPlayer player)
        {
            this.player = player;
        }
        public bool MoveNext()
        {
            if (index < player.GetCurrentKey().BoneRefs.Length)
            {
                Current = player.UnmappedTweenedKeys[player.GetCurrentKey().BoneRefs[index++].Timeline].Object;
                return true;
            }
            Current = null;
            return false;
        }

        public void Reset()
        {
            index = 0;
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

    public class ObjectIterator : IEnumerator<SpriterObject>
    {
        private SpriterAnimationPlayer player;
        internal int index = 0;
        public ObjectIterator(SpriterAnimationPlayer player)
        {
            this.player = player;
        }

        public void Dispose()
        {
            // TODO mitä?
        }

        public bool MoveNext()
        {
            if (index < player.GetCurrentKey().ObjectRefs.Length)
            {
                Current = player.UnmappedTweenedKeys[player.GetCurrentKey().ObjectRefs[index++].Timeline].Object;
                return true;
            }
            Current = null;
            return false;
        }

        public void Reset()
        {
            index = 0;
        }

        public SpriterObject Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }

    public abstract class Attachment : Bone
    {
        public Bone Parent
        {
            get;
            set;
        }
        private Vector2 positionTemp, scaleTemp;
        private float angleTemp;

        protected Attachment(Bone parent)
        {
            positionTemp = Vector2.Zero;
            scaleTemp = Vector2.Zero;
            Parent = parent;
        }

        public void Update()
        {
            positionTemp = Position;
            scaleTemp = Scale;
            angleTemp = Angle;

            Unmap(Parent);
            SetPosition(Position);
            SetScale(Scale);
            SetAngle(Angle);

            Position = positionTemp;
            Scale = scaleTemp;
            Angle = angleTemp;
        }

        protected abstract void SetPosition(Vector2 pos);
        protected abstract void SetScale(Vector2 pos);
        protected abstract void SetAngle(float angle);
    }
}
