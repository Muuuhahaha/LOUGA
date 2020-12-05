using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary.Utility
{
    /// <summary>
    /// Statistic info about numbers errors of model in example plans.
    /// </summary>
    public struct ErrorInfo
    {
        public int TotalAdd;
        public int TotalPre;
        public int TotalDel;
        public int TotalObservation;
        public int ErrorsAdd;
        public int ErrorsPre;
        public int ErrorsDel;
        public int ErrorsObservation;

        public override string ToString()
        {
            string s = "Add error rate: " + ((double)ErrorsAdd / TotalAdd).ToString("N5") + "(" + ErrorsAdd + "/" + TotalAdd + ")\n";
            s += "Del error rate: " + ((double)ErrorsDel / TotalDel).ToString("N5") + "(" + ErrorsDel + "/" + TotalDel + ")\n";
            s += "Pre error rate: " + ((double)ErrorsPre / TotalPre).ToString("N5") + "(" + ErrorsPre + "/" + TotalPre + ")\n";
            s += "Observation error rate: " + ((double)ErrorsObservation / TotalObservation).ToString("N5") + "(" + ErrorsObservation + "/" + TotalObservation + ")\n";

            return s;
        }
    }
}
