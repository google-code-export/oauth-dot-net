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

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security.Permissions;
using OAuth.Net.Common;

namespace OAuth.Net.Consumer
{
    [Serializable]
    [DebuggerDisplay("Type: {Type} Token: {Token} Secret: {Secret} Consumer Key: {ConsumerKey} Status: {Status}")]
    public class OAuthToken
        : IToken, ISerializable
    {
        /// <summary>
        /// Create a simple token received from a service provider.
        /// </summary>
        /// <param name="type">Token type</param>
        /// <param name="token">Token</param>
        /// <param name="secret">Secret</param>
        /// <param name="consumer">Consumer</param>
        public OAuthToken(TokenType type, string token, string secret, IConsumer consumer)
        {
            this.Type = type;
            this.Token = token;
            this.Secret = secret;
            this.ConsumerKey = consumer.Key;
        }

        /// <summary>
        /// Create a simple token received from a service provider.
        /// </summary>
        /// <param name="type">Token type</param>
        /// <param name="token">Token</param>
        /// <param name="secret">Secret</param>
        /// <param name="consumerKey">Consumer key</param>
        public OAuthToken(TokenType type, string token, string secret, string consumerKey)
        {
            this.Type = type;
            this.Token = token;
            this.Secret = secret;
            this.ConsumerKey = consumerKey;
        }

        protected OAuthToken(SerializationInfo info, StreamingContext context)
        {
            OAuthToken token = OAuthToken.Deserialize(info.GetString("serializedForm"));

            this.Type = token.Type;
            this.Token = token.Token;
            this.Secret = token.Secret;
            this.ConsumerKey = token.ConsumerKey;
            this.Status = token.Status;
        }

        /// <summary>
        /// The token type
        /// </summary>
        public TokenType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// The token 
        /// </summary>
        public string Token
        {
            get;
            private set;
        }

        /// <summary>
        /// The token secret
        /// </summary>
        public string Secret
        {
            get;
            private set;
        }

        /// <summary>
        /// The key of the consumer this token was issued to
        /// </summary>
        public string ConsumerKey
        {
            get;
            private set;
        }

        /// <summary>
        /// The status of the token
        /// </summary>
        public TokenStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// Serializes a SimpleToken to a string representation.
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>String serialization of the token</returns>
        /// <exception cref="System.ArgumentNullException">if <paramref name="token"/> is <c>null</c></exception>
        public static string Serialize(OAuthToken token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            return "[" + Rfc3986.Encode(Enum.Format(typeof(TokenType), token.Type, "G"))
                + "|" + Rfc3986.Encode(token.Token)
                + "|" + Rfc3986.Encode(token.Secret)
                + "|" + Rfc3986.Encode(token.ConsumerKey)
                + "|" + Rfc3986.Encode(Enum.Format(typeof(TokenStatus), token.Status, "G"))
                + "]";
        }

        /// <summary>
        /// De-serializes a SimpleToken from its serialized form.
        /// </summary>
        /// <param name="serializedForm">Serialized token</param>
        /// <returns>Token</returns>
        /// <exception cref="System.ArgumentException">
        /// if <paramref name="serializedForm"/> is null or empty
        /// </exception>
        /// <exception cref="System.FormatException">
        /// if <paramref name="serializedForm"/> is not a valid serialized form
        /// </exception>
        public static OAuthToken Deserialize(string serializedForm)
        {
            if (string.IsNullOrEmpty(serializedForm))
                throw new ArgumentException("serializedForm argument must not be null or empty", "serializedForm");

            if (!serializedForm.StartsWith("[", StringComparison.Ordinal))
                throw new FormatException("Serialized SimpleToken must start with [");

            if (!serializedForm.EndsWith("]", StringComparison.Ordinal))
                throw new FormatException("Serialized SimpleToken must end with ]");

            string[] parts = serializedForm.Substring(1, serializedForm.Length - 2)
                .Split(new char[] { '|' }, StringSplitOptions.None);

            if (parts.Length != 5)
                throw new FormatException("Serialized SimpleToken must consist of 5 pipe-separated fields");

            if (string.IsNullOrEmpty(parts[0]))
                throw new FormatException("Error deserializing SimpleToken.Type (field 0): cannot be null or empty");

            TokenType type;
            try 
            {
                type = (TokenType) Enum.Parse(typeof(TokenType), Rfc3986.Decode(parts[0]), true);
            }
            catch (Exception e)
            {
                throw new FormatException("Error deserializing SimpleToken.Type (field 0)", e);
            }

            if (string.IsNullOrEmpty(parts[1]))
                throw new FormatException("Error deserializing SimpleToken.Token (field 1): cannot be null or empty");

            string token;
            try
            {
                token = Rfc3986.Decode(parts[1]);
            }
            catch (Exception e)
            {
                throw new FormatException("Error deserializing SimpleToken.Token (field 1)", e);
            }

            string secret;
            try
            {
                secret = Rfc3986.Decode(parts[2]);
            }
            catch (Exception e)
            {
                throw new FormatException("Error deserializing SimpleToken.Secret (field 2)", e);
            }

            string consumerKey;
            try
            {
                consumerKey = Rfc3986.Decode(parts[3]);
            }
            catch (Exception e)
            {
                throw new FormatException("Error deserializing SimpleToken.ConsumerKey (field 3)", e);
            }

            if (string.IsNullOrEmpty(parts[4]))
                throw new FormatException("Error deserializing SimpleToken.Status (field 4): cannot be null or empty");

            TokenStatus status;
            try 
            {
                status = (TokenStatus) Enum.Parse(typeof(TokenStatus), Rfc3986.Decode(parts[4]), true);
            }
            catch (Exception e)
            {
                throw new FormatException("Error deserializing SimpleToken.Status (field 4)", e);
            }

            return new OAuthToken(type, token, secret, consumerKey) { Status = status };
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("serializedForm", OAuthToken.Serialize(this));
        }
        
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Casts aren't unnecessary")]
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is OAuthToken)
                return this.Equals(obj as OAuthToken);
            else if (obj is IToken)
                return this.Equals(obj as IToken);
            else
                return false;
        }

        public bool Equals(OAuthToken other)
        {
            return other != null
                && this.Type == other.Type
                && string.Equals(this.Token, other.Token)
                && string.Equals(this.Secret, other.Secret)
                && string.Equals(this.ConsumerKey, other.ConsumerKey)
                && this.Status == other.Status;
        }

        public bool Equals(IToken other)
        {
            return other != null
                && string.Equals(this.Token, other.Token)
                && string.Equals(this.Secret, other.Secret)
                && string.Equals(this.ConsumerKey, other.ConsumerKey)
                && this.Status == other.Status;
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode() ^ this.Token.GetHashCode() ^ this.Secret.GetHashCode()
                ^ this.ConsumerKey.GetHashCode() ^ this.Status.GetHashCode();
        }
    }
}
