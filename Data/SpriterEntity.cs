using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        #endregion

        #region Ctor

        public SpriterEntity(int id, string name)
        {
            ID = id;
            Name = name;
        }

        #endregion
    }
}
