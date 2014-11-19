using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaNi.Spriter.Data
{
    public class SpriterData
    {
        private readonly Folder[] folders;
        private readonly SpriterEntity[] entities;
        private int folderPointer, entityPointer;


        #region Properties

        public string ScmlVersion
        {
            get;
            private set;
        }

        public string Generator
        {
            get;
            private set;
        }

        public string GeneratorVersion
        {
            get;
            private set;
        }

        #endregion

        #region Ctor

        public SpriterData(string scmlv, string gen, string genv, int folders, int entities)
        {
            ScmlVersion = scmlv;
            Generator = gen;
            GeneratorVersion = genv;
            this.folders = new Folder[folders];
            this.entities = new SpriterEntity[entities];
        }

        #endregion

        #region Methods

        public void AddFolder(Folder folder)
        {
            folders[folderPointer++] = folder;
        }

        public void AddEntity(SpriterEntity entity)
        {
            entities[entityPointer++] = entity;
        }

        public Folder GetFolder(string name)
        {
            return folders.First(f => f.Name == name);
        }

        public Folder GetFolder(int index)
        {
            return folders[index];
        }

        public SpriterEntity GetEntity(int index)
        {
            return entities[index];
        }

        public SpriterEntity GetEntity(string entity)
        {
            return entities.First(e => e.Name == entity);
        }

        public File GetFile(ref Folder folder, int file)
        {
            return folder[file];
        }

        public File GetFile(int folder, int file)
        {
            return folders[folder][file];
        }

        public File GetFile(ref FileReference fileref)
        {
            return GetFile(fileref.Folder, fileref.File);
        }

        #endregion
    }
}
