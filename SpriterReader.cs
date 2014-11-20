using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SaNi.Spriter.Data;
using Curve = SaNi.Spriter.Data.Curve;

namespace SaNi.Spriter
{
    public class SpriterReader : ContentTypeReader<SpriterData>
    {
        protected override SpriterData Read(ContentReader input, SpriterData data)
        {
            
            string scmlversion = input.ReadString();
            string generator = input.ReadString();
            string generatorVersion = input.ReadString();
            int folderCount = input.ReadInt32();
            int entityCount = input.ReadInt32();
            data = new SpriterData(scmlversion, generator, generatorVersion, folderCount, entityCount);
            /*int[] files = new int[folderCount];

            for (int i = 0; i < folderCount; i++)
            {
                // filujen määrä per folder
                files[i] = input.ReadInt32();
            }*/
            LoadFolders(input, data, folderCount);
            LoadEntities(input, data, entityCount);

            return data;
        }

        #region Folders and files

        private void LoadFolders(ContentReader input, SpriterData data, int folderCount)
        {
            for (int folder = 0; folder < folderCount; folder++)
            {
                string name = input.ReadString();
                // folder on id
                int count = input.ReadInt32();
                data.AddFolder(new Folder(folder, name, count));
                LoadFiles(input, data.GetFolder(folder), count);
            }
        }

        private void LoadFiles(ContentReader input, Folder folder, int fileCount)
        {
            for (int file = 0; file < fileCount; file++)
            {
                // file on id
                string name = input.ReadString();
                int w = input.ReadInt32();
                int h = input.ReadInt32();
                float pivotX = input.ReadSingle();
                float pivotY = input.ReadSingle();
                folder.AddFile(new File(file, name, new Vector2(w, h), new Vector2(pivotX, pivotY)));
            }
        }

        #endregion


        #region Entities

        private void LoadEntities(ContentReader input, SpriterData data, int entityCount)
        {
            for (int i = 0; i < entityCount; i++)
            {
                // i on id
                string name = input.ReadString();
                int objInfoCount = input.ReadInt32();
                int charMapCount = input.ReadInt32();
                int animationCount = input.ReadInt32();
                SpriterEntity entity = new SpriterEntity(i, name, objInfoCount, charMapCount, animationCount);
                data.AddEntity(entity);
                // TODO lataa object infot, charmapit ja animaatiot
                LoadObjectInfos(input, entity, objInfoCount);
                LoadCharacterMaps(input, entity, charMapCount);
                LoadAnimations(input, entity, animationCount);
            }
        }

        #region Object infos

        private void LoadObjectInfos(ContentReader input, SpriterEntity entity, int count)
        {
            for (int i = 0; i < count; i++)
            {
                string name = input.ReadString();
                string type = input.ReadString();
                int w = input.ReadInt32();
                int h = input.ReadInt32();
                ObjectInfo info = new ObjectInfo(name, SpriterUtils.GetObjectInfoFor(type), new Vector2(w, h), new List<FileReference>());
                entity.AddInfo(info);
                // todo frameja? ei näy dokkarissa
            }
        }

        #endregion

        #region Character maps

        private void LoadCharacterMaps(ContentReader input, SpriterEntity entity, int charMapCount)
        {
            for (int i = 0; i < charMapCount; i++)
            {
                // i on id
                string name = input.ReadString();
                CharacterMap map = new CharacterMap(i, name);


                int mapCount = input.ReadInt32();
                LoadMaps(input, map, mapCount);


                entity.AddCharacterMap(map);
            }
        }

        private void LoadMaps(ContentReader input, CharacterMap map, int count)
        {
            for (int j = 0; j < count; j++)
            {
                int folder = input.ReadInt32();
                int file = input.ReadInt32();
                int targetFolder = input.ReadInt32();
                int targetFile = input.ReadInt32();
                map.Add(new FileReference(folder, file), new FileReference(targetFolder, targetFile));
            }
        }


        #endregion

        #region Animations

        private void LoadAnimations(ContentReader input, SpriterEntity entity, int animationCount)
        {
            for (int i = 0; i < animationCount; i++)
            {
                // i on id
                string name = input.ReadString();
                int length = input.ReadInt32();
                bool looping = input.ReadBoolean();

                int mainlineKeysCount = input.ReadInt32();
                int timelineCount = input.ReadInt32();
                SpriterAnimation anim = new SpriterAnimation(
                    new Mainline(mainlineKeysCount),
                    i,
                    name,
                    length,
                    looping,
                    timelineCount
                    );
                entity.AddAnimation(anim);
                
                LoadMainlineKeys(input, anim.Mainline, mainlineKeysCount);
            }
        }

        private void LoadMainlineKeys(ContentReader input, Mainline mainline, int count)
        {
            for (int id = 0; id < count; id++)
            {
                int time = input.ReadInt32();
                int objectRefCount = input.ReadInt32();
                int boneRefCount = input.ReadInt32();
                string curveType = input.ReadString();
                float[] cs = new[]
                {
                    input.ReadSingle(),
                    input.ReadSingle(),
                    input.ReadSingle(),
                    input.ReadSingle(),
                };
                Curve curve = new Curve();
                curve.Type = Curve.GetType(curveType);
                curve.Constraints.Set(cs[0], cs[1], cs[2], cs[3]);

                Key key = new Key(id, time, curve, boneRefCount, objectRefCount);
                mainline.AddKey(key);
            }

        }

        #endregion

        #endregion

    }
}
