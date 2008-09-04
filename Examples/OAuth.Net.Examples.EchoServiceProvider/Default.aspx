<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OAuth.Net.Examples.EchoServiceProvider.DemoPage" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="OAuth.Net.Examples.EchoServiceProvider" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!--

Copyright (c) 2008 Madgex

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

OAuth.net uses the Windsor Container from the Castle Project. See "Castle 
Project License.txt" in the Licenses folder.

Authors: Bruce Boughton, Chris Adams
Website: http://lab.madgex.com/oauth-net/
Email:   oauth-dot-net@madgex.com

-->
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>OAuth Echo Service Provider Demo</title>
    <link rel="Stylesheet" type="text/css" href="css/main.css" />
</head>
<body>
    <div id="content">
        <div id="main-content">
            <h1>OAuth Echo Service Provider Demo</h1>
            <p>This is a test OAuth service provider with pre-defined keys and secrets and no user authentication step.</p>
            
            <h2>Service settings</h2>
            <dl>
                <dt>Request Token URL</dt>
                    <dd><kbd><%= this.RequestTokenBaseUri.AbsoluteUri %></kbd></dd>
                <dt>Authorization URL</dt>
                    <dd>not required</dd>
                <dt>Access Token URL</dt>
                    <dd><kbd><%= this.AccessTokenBaseUri.AbsoluteUri %></kbd></dd>
                <dt>HTTP Methods supported</dt>
                    <dd><kbd>GET</kbd> and <kbd>POST</kbd></dd>
                <dt>Is Authorization header supported?</dt>
                    <dd>yes</dd>
                <dt>Realm</dt>
                    <dd><kbd>""</kbd> (empty)</dd>
                <dt>Signature Methods supported</dt>
                    <dd><kbd>HMAC-SHA1</kbd></dd>
                <dt>OAuth Versions supported</dt>
                    <dd><kbd>1.0</kbd></dd>
            </dl>
            
            <h2>Consumer details</h2>
            <dl>
                <dt>Key</dt>
                    <dd><kbd>key</kbd></dd>
                <dt>Secret</dt>
                    <dd><kbd>secret</kbd></dd>
            </dl>
            
            <h2>Getting a request token</h2>
            
            <p>A successful request will return the following token:</p>
            <dl>
                <dt>Token</dt>
                    <dd><kbd>requestkey</kbd></dd>
                <dt>Secret</dt>
                    <dd><kbd>requestsecret</kbd></dd>
            </dl>
            
            <p>An unsuccessful request will describe the error using the <a href="http://wiki.oauth.net/ProblemReporting">Problem Reporting extension</a>.</p>
            
            <%
                Uri requestUri = this.BuildRequestTokenUri();
            %>
            
            <p>For example: <a href="<%= requestUri %>"><%= requestUri%></a></p>
            
            <h2>Getting an access token</h2>
            
            <p>A successful request will return the following token:</p>
            <dl>
                <dt>Token</dt>
                    <dd><kbd>accesskey</kbd></dd>
                <dt>Secret</dt>
                    <dd><kbd>accesssecret</kbd></dd>
            </dl>
            
            <p>An unsuccessful request will describe the error using the <a href="http://wiki.oauth.net/ProblemReporting">Problem Reporting extension</a>.</p>
            
            <%
                Uri accessUri = this.BuildAccessTokenUri();
            %>
            
            <p>For example: <a href="<%= accessUri %>"><%= accessUri %></a></p>
            
            <h2>Calling the Echo API</h2>
            
            <dl>
                <dt>Echo API URL</dt>
                    <dd><kbd><%= this.EchoApiBaseUri.AbsoluteUri %></kbd></dd>
            </dl>
            
            <p>A successful request will echo the non-OAuth parameters.</p>
            
            <p>An unsuccessful request will describe the error using the <a href="http://wiki.oauth.net/ProblemReporting">Problem Reporting extension</a>.</p>
            
            <%
                Uri echoUri = this.BuildEchoCallUri(
                    new KeyValuePair<string, string>("method", "foo bar"),
                    new KeyValuePair<string, string>("bar", "baz"));   
            %>
            
            <p>For example: <a href="<%= echoUri %>"><%= echoUri %></a> should echo: <kbd>method=foo bar&amp;bar=baz</kbd></p>
        </div>
    </div>
    <div id="footer">
        <p>&copy;2008 Copyright Madgex Limited. All Rights Reserved.</p>
    </div>
</body>
</html>
