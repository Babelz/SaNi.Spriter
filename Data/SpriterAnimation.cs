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

        //public void Update(int time, Bone root)


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
