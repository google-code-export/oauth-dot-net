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
// OAuth.net uses the Common Service Locator interface, released under the MS-PL
// license. See "CommonServiceLocator License.txt" in the Licenses folder.
// 
// The examples and test cases use the Windsor Container from the Castle Project
// and Common Service Locator Windsor adaptor, released under the Apache License,
// Version 2.0. See "Castle Project License.txt" in the Licenses folder.
// 
// XRDS-Simple.net uses the HTMLAgility Pack. See "HTML Agility Pack License.txt"
// in the Licenses folder.
//
// Authors: Bruce Boughton, Chris Adams
// Website: http://lab.madgex.com/oauth-net/
// Email:   oauth-dot-net@madgex.com

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace OAuth.Net.Common
{
    [DebuggerDisplay("Success: {Success} Data: {Data}")]
    public struct ResultInfo<T>
    {
        public ResultInfo(bool success, T data)
            : this()
        {
            this.Success = success;
            this.Data = data;
        }

        public bool Success
        {
            get;
            private set;
        }

        public T Data
        {
            get;
            private set;
        }

        public bool IsTrue
        {
            get { return this ? true : false; }
        }

        public bool IsFalse
        {
            get { return this ? false : true; }
        }

        /// <summary>
        /// Checks whether the result info evaluates to true.
        /// </summary>
        /// <param name="result">The result info</param>
        /// <returns>True, iff result.Success == true</returns>
        public static bool operator true(ResultInfo<T> result)
        {
            return result.Success;
        }

        /// <summary>
        /// Checks whether the result info evaluates to false.
        /// </summary>
        /// <param name="result">The result info</param>
        /// <returns>True, iff result.Success == false</returns>
        public static bool operator false(ResultInfo<T> result)
        {
            return !result.Success;
        }

        /// <summary>
        /// Logical negation operator. Returns a result info with
        /// the Success value flipped.
        /// </summary>
        /// <param name="result">The result info</param>
        /// <returns>A new result info with the same data but with the
        /// success flag flipped</returns>
        public static ResultInfo<T> operator !(ResultInfo<T> result)
        {
            return new ResultInfo<T>(!result.Success, result.Data);
        }

        /// <summary>
        /// Logical and operator. Returns a result info with the Success
        /// value set to left.Success and right.Success and the data set to default(T)
        /// </summary>
        /// <param name="left">The left result info</param>
        /// <param name="right">The right result info</param>
        /// <returns>A new result info with the Success value set to the result
        /// of left.Success and right.Success, and the data set to default(T)</returns>
        public static ResultInfo<T> operator &(ResultInfo<T> left, ResultInfo<T> right)
        {
            return new ResultInfo<T>(left.Success && right.Success, default(T));
        }

        /// <summary>
        /// Logical or operator. Returns a result info with the Success
        /// value set to left.Success || right.Success and the data set to default(T)
        /// </summary>
        /// <param name="left">The left result info</param>
        /// <param name="right">The right result info</param>
        /// <returns>A new result info with the Success value set to the result
        /// of left.Success || right.Success, and the data set to default(T)</returns>
        public static ResultInfo<T> operator |(ResultInfo<T> left, ResultInfo<T> right)
        {
            return new ResultInfo<T>(left.Success || right.Success, default(T));
        }

        /// <summary>
        /// Logical equality operator. Returns a boolean value indicating whether
        /// the left and right Success values are the same. The data is ignored for
        /// the comparison.
        /// </summary>
        /// <param name="left">The left result info</param>
        /// <param name="right">The right result info</param>
        /// <returns>True, iff left.Success == right.Success</returns>
        public static bool operator ==(ResultInfo<T> left, ResultInfo<T> right)
        {
            return left.Success == right.Success;
        }

        /// <summary>
        /// Logical inequality operator. Returns a boolean value indicating whether
        /// the left and right Success values are different. The data is ignored for
        /// the comparison.
        /// </summary>
        /// <param name="left">The left result info</param>
        /// <param name="right">The right result info</param>
        /// <returns>True, iff left.Success != right.Success</returns>
        public static bool operator !=(ResultInfo<T> left, ResultInfo<T> right)
        {
            return left.Success != right.Success;
        }

        /// <summary>
        /// Implicit conversion to type T.
        /// </summary>
        /// <param name="result">The result info</param>
        /// <returns>The data stored in the result info</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Data property provides explicit conversion")]
        public static implicit operator T(ResultInfo<T> result)
        {
            return result.Data;
        }

        public ResultInfo<T> LogicalNot()
        {
            return !this;
        }

        public ResultInfo<T> BitwiseAnd(ResultInfo<T> other)
        {
            return this & other;
        }

        public ResultInfo<T> And(ResultInfo<T> other)
        {
            return this && other;
        }

        public ResultInfo<T> BitwiseOr(ResultInfo<T> other)
        {
            return this | other;
        }

        public ResultInfo<T> Or(ResultInfo<T> other)
        {
            return this || other;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is ResultInfo<T>))
                return false;

            return this.Equals((ResultInfo<T>)obj);
        }

        public bool Equals(ResultInfo<T> other)
        {
            return this.Success == other.Success;
        }

        public override int GetHashCode()
        {
            return this.Success.GetHashCode() ^ this.Data.GetHashCode();
        }
    }
}
