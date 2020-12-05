using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelLearner.Algorithms
{
    public interface IClause
    {
        /// <summary>
        /// Returns TRUE if clause is true with given parametrization.
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        bool IsTrue(bool[] parameterValues);

        /// <summary>
        /// Flips one literal in clause in given parametrization.
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <param name="id"></param>
        void FlipLiteral(bool[] parameterValues, int id);

        /// <summary>
        /// Returns number of literals in clause.
        /// </summary>
        /// <returns></returns>
        int GetLiteralsCount();

        /// <summary>
        /// Returns weight of clause.
        /// </summary>
        /// <returns></returns>
        double GetWeight();
    }
}
