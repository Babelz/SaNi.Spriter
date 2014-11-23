using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaNi.Spriter.Data
{
    public class SpriterData
    {
        public readonly Folder[] Folders;
        public readonly SpriterEntity[] Entities;
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
            this.Folders = new Folder[folders];
            this.Entities = new SpriterEntity[entities];
        }

        #endregion

        #region Methods

        public void AddFolder(Folder folder)
        {
            Folders[folderPointer++] = folder;
        }

        public void AddEntity(SpriterEntity entity)
        {
            Entities[entityPointer++] = entity;
        }

        public Folder GetFolder(string name)
        {
            return Folders.First(f => f.Name == name);
        }

        public Folder GetFolder(int index)
        {
            return Folders[index];
        }

        public SpriterEntity GetEntity(int index)
        {
            return Entities[index];
        }

        public SpriterEntity GetEntity(string entity)
        {
            return Entities.First(e => e.Name == entity);
        }

        public File GetFile(ref Folder folder, int file)
        {
            return folder[file];
        }

        public File GetFile(int folder, int file)
        {
            return Folders[folder][file];
        }

        public File GetFile(ref FileReference fileref)
        {
            return GetFile(fileref.Folder, fileref.File);
        }

        #endregion
    }
}
