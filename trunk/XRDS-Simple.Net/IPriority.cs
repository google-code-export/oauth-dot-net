using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRDS_Simple.Net
{
    /// <summary>
    /// Ensures that those elements that need to be sorted
    /// by priority expose the same contract.
    /// </summary>
    internal interface IPriority
    {        
        string Priority
        {
            get;            
        }
    }
}
