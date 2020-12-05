using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms
{
    /// <summary>
    /// Interface for general model learning algorithm.
    /// </summary>
    interface ILearningAlgorithm
    {
        /// <summary>
        /// Learns operators' definitions from input worlds.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="worlds"></param>
        /// <returns></returns>
        Model Learn(Model m, List<World> worlds);
    }
}
