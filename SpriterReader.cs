using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SaNi.Spriter.Data;

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
                string name = input.ReadString();
                int objInfoCount = input.ReadInt32();
                int charMapCount = input.ReadInt32();
                int animationCount = input.ReadInt32();
                SpriterEntity entity = new SpriterEntity(i, name, objInfoCount, charMapCount, animationCount);
                data.AddEntity(entity);
                // TODO lataa object infot, charmapit ja animaatiot
            }
        }

        #endregion

    }
}
