using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SaNi.Spriter.Data
{
    public class SpriterEntity
    {
        #region Properties

        public int ID
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        private readonly Dictionary<string, SpriterAnimation> namedAnimations;
        private readonly SpriterAnimation[] animations;
        private readonly CharacterMap[] characterMaps;
        private readonly ObjectInfo[] objectInfos;

        private int objInfoPointer;
        private int charMapPointer;
        private int animationPointer;
            
        #endregion

        #region Ctor

        public SpriterEntity(int id, string name, int objInfoCount, int charMapCount, int animationCount)
        {
            ID = id;
            Name = name;
            objectInfos = new ObjectInfo[objInfoCount];
            characterMaps = new CharacterMap[charMapCount];
            namedAnimations = new Dictionary<string, SpriterAnimation>();
            animations = new SpriterAnimation[animationCount];
        }

        #endregion

        #region Methods

        internal void AddInfo(ObjectInfo info)
        {
            objectInfos[objInfoPointer++] = info;
        }

        internal void AddCharacterMap(CharacterMap map)
        {
            characterMaps[charMapPointer++] = map;
        }

        internal void AddAnimation(SpriterAnimation anim)
        {
            animations[animationPointer++] = anim;
            namedAnimations[anim.Name] = anim;
        }

        #endregion

        public ObjectInfo GetInfo(string name)
        {
            return objectInfos.FirstOrDefault(o => o.Name == name);
        }
    }

    public enum ObjectType
    {
        Sprite,
        Bone,
        Box,
        Point,
        Skin
    }

    public class ObjectInfo
    {
        public readonly ObjectType Type;
        public readonly List<FileReference> Frames;
        public readonly string Name;
        public readonly Vector2 Size;

        public ObjectInfo(string name, ObjectType type, Vector2 size, List<FileReference> frames)
        {
            Name = name;
            Type = type;
            Frames = frames;
            Size = size;
        }
    }

    public class CharacterMap
    {

        private readonly Dictionary<FileReference, FileReference> maps;

        public int ID
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public CharacterMap(int id, string name)
        {
            ID = id;
            Name = name;
            maps = new Dictionary<FileReference, FileReference>();
        }

        public void Add(FileReference key, FileReference value)
        {
            maps[key] = value;
        }

        /*
        public FileReference this[FileReference index]
        {
            get
            {
                FileReference key;
                if (TryGetValue(index, out key))
                    return key;
                return index;
            }
        }*/
    }
}
