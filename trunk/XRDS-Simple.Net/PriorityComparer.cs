using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRDS_Simple.Net
{
    /// <summary>
    /// Compares IPriority Elements to provide the correct ordering in 6.4 of the specification.
    /// </summary>
    internal class PriorityComparer : IComparer<IPriority>
    {
        Random equalityBreaker = new Random();

        #region IComparer<IPriority> Members

        public int Compare(IPriority x, IPriority y)
        {
            int? xPriority = null;
            int? yPriority = null;

            if (x != null && !String.IsNullOrEmpty(x.Priority))
            {
                int xValue;
                if (Int32.TryParse(x.Priority, out xValue))                
                    xPriority = xValue;
                else                
                    xPriority = null;                                 
            }

            if (y != null && !String.IsNullOrEmpty(y.Priority))
            {
                int yValue;
                if( Int32.TryParse(y.Priority, out yValue) )
                    yPriority = yValue;
                else
                    yPriority = null;                                   
            }

            //If the reference is the same then say they are the same
            if (x == y)
            {
                return 0;
            }
            //If their value is equal randomly choose which one is better
            else if (xPriority.Equals(yPriority))
            {
                int breaker = equalityBreaker.Next(0, 1);
                return breaker == 1 ? 1 : -1;                
            }
            else if (!xPriority.HasValue)
            {
                return 1;
            }
            else if (!yPriority.HasValue)
            {
                return -1;
            }
            else if (xPriority.Value < yPriority.Value)
            {
                return 1;
            }
            else
                return -1;
        }

        #endregion
    }
}
