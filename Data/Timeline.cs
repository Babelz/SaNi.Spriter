using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SaNi.Spriter.Data
{
    public class Timeline
    {
        public readonly Key[] Keys;
        private int keyPointer;
        public readonly int ID;
        public readonly string Name;
        public readonly ObjectInfo ObjectInfo;

        internal Timeline(int id, string name, ObjectInfo info, int keys)
        {
            ID = id;
            Name = name;
            ObjectInfo = info;
            Keys = new Key[keys];
        }

        internal void AddKey(Key key)
        {
            Keys[keyPointer++] = key;
        }

        #region Indexer

        public Key this[int index]
        {
            get { return Keys[index]; }
        }

        #endregion
    }

    public class TimelineKey
    {
        public readonly int ID, Spin;
        public readonly int Time;
        public readonly Curve Curve;

        private SpriterObject obj;

        public bool Active
        {
            get;
            private set;
        }

        public SpriterObject Object
        {
            get { return obj;  }
            private set
            {
                if (value == null) throw new ArgumentException("obj cannot be null", "value");
                    obj = value;
            }
        }


        #region Ctor

        public TimelineKey(int id, int time = 0, int spin = 1) : this(id, time, spin, new Curve())
        {
            
        }
        public TimelineKey(int id, int time, int spin, Curve curve)
        {
            ID = id;
            Time = time;
            Spin = spin;
            Curve = curve;
        }

        

        #endregion
    }

    public class Bone
    {

        protected Vector2 position;
        protected Vector2 scale;
        protected Vector2 pivot;

        public float Angle;

        public Vector2 Position
        {
            get { return position; }
            private set { position = value; }
        }

        public Vector2 Scale
        {
            get { return scale; }
            private set { scale = value; }
        }

        public Vector2 Pivot
        {
            get { return pivot; }
            private set { pivot = value; }
        }

        public Bone(Vector2 position, Vector2 scale, Vector2 pivot, float angle)
        {
            Position = position;
            Scale = scale;
            Pivot = pivot;
            Angle = angle;
        }

        public Bone(Vector2 position) : this(position, Vector2.One, Vector2.UnitY, 0f)
        {
            
        }

        public Bone(Bone bone):this(bone.Position, bone.Scale, bone.Pivot, bone.Angle)
        {
            
        }

        public Bone():this(Vector2.Zero)
        {
            
        }

        public bool IsBone()
        {
            return !(this as SpriterObject != null);
        }

        public void Set(Bone bone)
        {
            Set(bone.position, bone.Angle, bone.scale, bone.pivot);   
        }

        public virtual void Set(float x, float y, float angle, float scaleX, float scaleY, float pivotX, float pivotY)
        {
            Angle = angle;
            position.X = x;
            position.Y = y;
            scale.X = scaleX;
            scale.Y = scaleY;
            pivot.X = pivotX;
            pivot.Y = pivotY;
        }

        public virtual void Set(Vector2 pos, float angle, Vector2 scale, Vector2 pivot)
        {
            Angle = angle;
            Position = pos;
            Scale = scale;
            Pivot = pivot;
        }

        public void Unmap(Bone parent)
        {
            Angle *= Math.Sign(parent.scale.X)*Math.Sign(parent.scale.Y);
            Angle += parent.Angle;
            Scale *= parent.scale;
            Position *= parent.position;
            Position.Rotate(parent.Angle);
            Position += parent.position;
        }

        public void Map(Bone parent)
        {
            Position -= parent.position;
            Position.Rotate(-parent.Angle);
            Position *= Vector2.One/parent.position;
            Scale *= Vector2.One/parent.scale;
            Angle -= parent.Angle;
            Angle *= Math.Sign(parent.scale.X)*Math.Sign(parent.scale.Y);
        }
    }

    public class SpriterObject : Bone
    {
        public float Alpha;
        public readonly FileReference Ref;

        internal SpriterObject(Vector2 pos, Vector2 scale, Vector2 pivot, float angle, float alpha, FileReference fref)
            :base(pos,scale, pivot, angle)
        {
            Alpha = alpha;
            Ref = fref;
        }

        public SpriterObject(Vector2 pos) : this(pos, Vector2.One, Vector2.UnitY, 0f, 1f, new FileReference(-1, -1))
        {
            
        }

        public SpriterObject(SpriterObject o) : this(o.Position, o.Scale, o.Pivot, o.Angle, o.Alpha, o.Ref)
        {
            
        }

        public SpriterObject():this(Vector2.Zero)
        {
            
        }

        public void Set(float x, float y, float angle, float scaleX, float scaleY, float pivotX, float pivotY, float alpha, int folder, int file)
        {
            base.Set(x, y, angle, scaleX, scaleY, pivotX, pivotY);
            Alpha = alpha;
            Ref.Folder = folder;
            Ref.File = file;
        }

        public void Set(Vector2 position, float angle, Vector2 scale, Vector2 pivot, float alpha, FileReference fref)
        {
            Set(position.X, position.Y, angle, scale.X, scale.Y, pivot.X, pivot.Y, alpha, fref.Folder, fref.File);
        }

        public void Set(SpriterObject o)
        {
            Set(o.position, o.Angle, o.scale, o.pivot, o.Alpha, o.Ref);
        }
    }
}
