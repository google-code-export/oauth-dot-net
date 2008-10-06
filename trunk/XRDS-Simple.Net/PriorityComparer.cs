// Copyright (c) 2008 Madgex
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// OAuth.net uses the Windsor Container from the Castle Project. See "Castle 
// Project License.txt" in the Licenses folder.
//
// XRDS-Simple.net uses the HTMLAgility Pack. See "HTML Agility Pack License.txt"
// in the Licenses folder.
// 
// Authors: Chris Adams, Bruce Boughton
// Website: http://lab.madgex.com/oauth-net/
// Email:   oauth-dot-net@madgex.com

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
