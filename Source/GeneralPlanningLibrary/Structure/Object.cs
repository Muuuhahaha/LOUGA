using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    public class Object
    {
        public Type Type;
        public string Name;

        public Object(string name, Type type)
        {
            this.Type = type;
            this.Name = name;
        }

        /// <summary>
        /// Outputs object in PDDL format. (with type declaration if possible)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Type == Type.NoType) return Name;
            else return Name + " - " + Type.Name;
        }
    }
}
