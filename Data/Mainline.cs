using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaNi.Spriter.Data
{
    public class Mainline
    {
        internal Key[] keys;
        private int keyPointer;

        public Mainline(int mainlineKeysCount)
        {
            keys = new Key[mainlineKeysCount];
        }

        public void AddKey(Key key)
        {
            keys[keyPointer++] = key;
        }

        public Key this[int index]
        {
            get { return keys[index]; }
        }

        public Key GetKeyBeforeTime(int time)
        {
            return keys.First(k => k.Time <= time);
        }
    }

    public class Key
    {
        public readonly int ID, Time;
        internal BoneRef[] BoneRefs;
        internal ObjectRef[] ObjectRefs;
        private int bonePointer, objectPointer;
        public readonly Curve Curve;


        public Key(int id, int time, Curve curve, int boneRefs, int objectRefs)
        {
            ID = id;
            Time = time;
            Curve = curve;
            BoneRefs = new BoneRef[boneRefs];
            ObjectRefs = new ObjectRef[objectRefs];
        }

        public void AddBoneRef(BoneRef boneRef)
        {
            BoneRefs[bonePointer++] = boneRef;
        }

        public void AddObjectRef(ObjectRef objref)
        {
            ObjectRefs[objectPointer++] = objref;
        }

        public BoneRef GetBoneRef(int index)
        {
            if (index < 0 || index >= BoneRefs.Length) return null;
            return BoneRefs[index];
        }

        public ObjectRef GetObjectRef(int index)
        {
            if (index < 0 || index >= ObjectRefs.Length) return null;
            return ObjectRefs[index];
        }

        public BoneRef GetBoneRefByTimeline(int timeline)
        {
            return BoneRefs.FirstOrDefault(b => b.Timeline == timeline);
        }

        public ObjectRef GetObjectRefByTimeline(int timeline)
        {
            return ObjectRefs.FirstOrDefault(b => b.Timeline == timeline);
        }

        public BoneRef GetBoneRef(BoneRef boneRef)
        {
            return GetBoneRefByTimeline(boneRef.Timeline);
        }

        public ObjectRef GetObjectRef(ObjectRef objectRef)
        {
            return GetObjectRef(objectRef.Timeline);
        }
    }

    public class BoneRef
    {
        public readonly int ID, Key, Timeline;
        public readonly BoneRef Parent;

        public BoneRef(int id, int timeline, int key, BoneRef parent)
        {
            ID = id;
            Timeline = timeline;
            Key = key;
            Parent = parent;
        }
    }

    public class ObjectRef : BoneRef
    {
        public readonly int ZIndex;

        public ObjectRef(int id, int timeline, int key, BoneRef parent, int zIndex) : base(id, timeline, key, parent)
        {
            ZIndex = zIndex;
        }
    }
}


