<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form1.aspx.cs" Inherits="L1.Form1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Gauti atsakymus!" />
            <asp:Table ID="Table1" runat="server">
            </asp:Table>
            <br />
            <asp:Table ID="Table2" runat="server" GridLines="Both">
            </asp:Table>
        </div>
        <br />
        <asp:Label ID="Label1" runat="server"></asp:Label>
        <br />
        <asp:Label ID="Label2" runat="server"></asp:Label>
        <br />
        <asp:Label ID="Label3" runat="server"></asp:Label>
        <br />
        <asp:Label ID="Label4" runat="server"></asp:Label>
    </form>
</body>
</html>
