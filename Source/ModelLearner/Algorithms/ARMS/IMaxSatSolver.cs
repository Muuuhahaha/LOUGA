using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelLearner.Algorithms.ARMS
{
    interface IMaxSatSolver
    {
        /// <summary>
        /// Solves given formula
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        bool[] Solve(Formula formula);
    }
}
