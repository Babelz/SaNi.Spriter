using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaNi.Spriter.Data
{
    public struct FileReference
    {
        #region Vars
        public int Folder;
        public int File;
        #endregion

        #region Properties
        public bool HasFile
        {
            get { return File != -1; }
        }

        public bool HasFolder
        {
            get { return Folder != -1; }
        }
        #endregion

        public FileReference(int folder, int file)
        {
            Folder = folder;
            File = file;
        }

        public void Set(int folder, int file)
        {
            Folder = folder;
            File = file;
        }

        public void Set(ref FileReference fileRef)
        {
            Set(fileRef.Folder, fileRef.File);
        }

        public override string ToString()
        {
            return string.Format("[folder: {0}, file: {1}]", Folder, File);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            FileReference fileRef = (FileReference) obj;
            return Folder == fileRef.Folder && File == fileRef.File;
        }

        public override int GetHashCode()
        {
            return Folder*10000 + File;
        }
    }
}
