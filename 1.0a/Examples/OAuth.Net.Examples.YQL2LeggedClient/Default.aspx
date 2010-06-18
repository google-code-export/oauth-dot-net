<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OAuth.Net.Examples.YQL2LeggedClient._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    
    <h1>This example makes a two legged OAuth consumer request to YQL.</h1>
    
    <p>If the request is successfull you should see some XML displayed below.</p>
    
    <div>
        <asp:Xml ID="YQLDisplay" runat="server"></asp:Xml>
    </div>
    </form>    
</body>
</html>
