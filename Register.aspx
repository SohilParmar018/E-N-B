<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="EnoticeBoard_And_Suggestionbox.Register" %>

<!DOCTYPE html>
<html runat="server">
<head>
    <meta charset="UTF-8" />
    <title>Register</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server" class="container mt-5" style="max-width:400px;">
        <h3 class="mb-4">Register New User</h3>
        <asp:Label ID="lblMessage" runat="server" ForeColor="green"></asp:Label>

        <!-- Name Field -->
        <div class="mb-3">
            <asp:TextBox ID="txtRegName" runat="server" CssClass="form-control" Placeholder="Full Name" />
            <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtRegName"
                ErrorMessage="Name is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>

        <!-- Email Field -->
        <div class="mb-3">
            <asp:TextBox ID="txtRegEmail" runat="server" CssClass="form-control" Placeholder="Email" TextMode="Email" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtRegEmail"
                ErrorMessage="Email is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtRegEmail"
                ErrorMessage="Invalid email format" CssClass="text-danger" Display="Dynamic"
                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$">*</asp:RegularExpressionValidator>
        </div>

        <!-- Password Field -->
        <div class="mb-3">
            <asp:TextBox ID="txtRegPassword" runat="server" CssClass="form-control" Placeholder="Password" TextMode="Password" />
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtRegPassword"
                ErrorMessage="Password is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>

        <!-- Confirm Password Field -->
        <div class="mb-3">
            <asp:TextBox ID="txtRegConfirmPassword" runat="server" CssClass="form-control" Placeholder="Confirm Password" TextMode="Password" />
            <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtRegConfirmPassword"
                ErrorMessage="Confirm Password is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="cvPassword" runat="server"
                ControlToCompare="txtRegPassword" ControlToValidate="txtRegConfirmPassword"
                ErrorMessage="Passwords do not match" CssClass="text-danger" Display="Dynamic"></asp:CompareValidator>
        </div>

        <!-- Auto-generated Code (Simple CAPTCHA) -->
        <div class="mb-3">
            <asp:Label ID="lblAutoCode" runat="server" Font-Bold="true"></asp:Label>
            <asp:TextBox ID="txtAutoCode" runat="server" CssClass="form-control" Placeholder="Enter code shown above" />
            <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtAutoCode"
                ErrorMessage="Code is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:Button ID="btnRefreshCode" runat="server" Text="↻ Refresh" CssClass="btn btn-secondary mt-2" OnClick="btnRefreshCode_Click" />
        </div>

        <!-- Register Button -->
        <asp:Button ID="btnRegisterUser" runat="server" Text="Register" CssClass="btn btn-success w-100"
            OnClick="btnRegisterUser_Click" OnClientClick="return Page_ClientValidate();" />

        <div class="mt-3">
            <asp:HyperLink runat="server" NavigateUrl="~/ENSB.aspx">← Back</asp:HyperLink>
        </div>
    </form>

    </body>
</html>
