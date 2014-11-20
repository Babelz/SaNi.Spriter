using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaNi.Spriter.Data
{
    public class Folder
    {
        public readonly File[] Files;
        public readonly int ID;
        public readonly string Name;
        private int filePointer;


        public Folder(int id, string name, int files)
        {
            filePointer = 0;
            ID = id;
            Name = name;
            Files = new File[files];
        }

        public void AddFile(File file)
        {
            Files[filePointer++] = file;
        }

        public File this[int index]
        {
            get { return Files[index];  }
        }

        public File this[string name]
        {
            get
            {
                return Files.FirstOrDefault(f => f.Name == name);
            }
        }
    }
}
