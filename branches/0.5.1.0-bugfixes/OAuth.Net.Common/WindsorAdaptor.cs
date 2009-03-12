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
// Authors: Bruce Boughton, Chris Adams
// Website: http://lab.madgex.com/oauth-net/
// Email:   oauth-dot-net@madgex.com

using System.Diagnostics.CodeAnalysis;
using Castle.Windsor;

namespace OAuth.Net.Common
{
    public class WindsorAdaptor : IServiceLocator
    {
        public WindsorAdaptor(IWindsorContainer container)
        {
            this.Container = container;
        }

        public IWindsorContainer Container
        {
            get;
            private set;
        }
        
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type parameter defines service to resolve")]
        public T GetInstance<T>()
        {
            return this.Container.Resolve<T>();
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type parameter defines service to resolve")]
        public T TryGetInstance<T>()
        {
            try
            {
                return this.Container.Resolve<T>();
            }
            catch (Castle.MicroKernel.ComponentNotFoundException)
            {
                return default(T);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type parameter defines service to resolve")]
        public T GetInstance<T>(string name)
        {
            return this.Container.Resolve<T>(name);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type parameter defines service to resolve")]
        public T TryGetInstance<T>(string name)
        {
            try
            {
                return this.Container.Resolve<T>(name);
            }
            catch (Castle.MicroKernel.ComponentNotFoundException)
            {
                return default(T);
            }
        }
    }
}
