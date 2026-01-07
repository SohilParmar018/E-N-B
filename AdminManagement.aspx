<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminManagement.aspx.cs" Inherits="EnoticeBoard_And_Suggestionbox.AdminManagement" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <title>Admin Management</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server" class="container mt-5">

        <h2 class="text-center mb-4 text-danger fw-bold">Admin Management Panel</h2>

        <!-- ✅ Message Label -->
        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger fw-bold mb-3 d-block"></asp:Label>

        <!-- ✅ Add Admin Section -->
        <div class="card shadow-sm p-4 mb-4">
            <h4 class="text-primary mb-3">Add New Admin</h4>
            <div class="row g-3">
                <div class="col-md-4">
                    <asp:TextBox ID="txtAdminName" runat="server" CssClass="form-control" Placeholder="Admin Name"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="txtAdminEmail" runat="server" CssClass="form-control" Placeholder="Admin Email" TextMode="Email"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="txtAdminPassword" runat="server" CssClass="form-control" Placeholder="Admin Password" TextMode="Password"></asp:TextBox>
                </div>
            </div>
            <div class="mt-3">
                <asp:Button ID="btnAddAdmin" runat="server" Text="Add Admin" CssClass="btn btn-success" OnClick="btnAddAdmin_Click" />
            </div>
        </div>

        <!-- ✅ Search Section -->
        <div class="card shadow-sm p-4 mb-4">
            <h4 class="text-primary mb-3">Search</h4>
            <div class="row g-3 align-items-center">
                <div class="col-md-5">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" Placeholder="Enter name or email to search"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:DropDownList ID="ddlSearchType" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Admins" Value="Admins"></asp:ListItem>
                        <asp:ListItem Text="Users" Value="Users"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnShowAll" runat="server" Text="Show All" CssClass="btn btn-outline-secondary w-100" OnClick="btnShowAll_Click" />
                </div>
            </div>
        </div>

        <!-- ✅ Admins Grid -->
        <div class="card shadow-sm p-4 mb-4">
            <h4 class="text-danger">Admins List</h4>
            <asp:GridView ID="gvAdmins" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped"
                OnRowCommand="gvAdmins_RowCommand">
                <Columns>
                    <asp:BoundField DataField="AdminID" HeaderText="ID" />
                    <asp:BoundField DataField="AdminName" HeaderText="Name" />
                    <asp:BoundField DataField="AdminEmail" HeaderText="Email" />
                    <asp:ButtonField Text="Delete" CommandName="DeleteAdmin" ButtonType="Button" ControlStyle-CssClass="btn btn-danger btn-sm" />
                </Columns>
            </asp:GridView>
        </div>

        <!-- ✅ Users Grid -->
        <div class="card shadow-sm p-4 mb-4">
            <h4 class="text-danger">Users List</h4>
            <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped"
                OnRowCommand="gvUsers_RowCommand">
                <Columns>
                    <asp:BoundField DataField="UserID" HeaderText="ID" />
                    <asp:BoundField DataField="UserName" HeaderText="Name" />
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:ButtonField Text="Delete" CommandName="DeleteUser" ButtonType="Button" ControlStyle-CssClass="btn btn-danger btn-sm" />
                </Columns>
            </asp:GridView>
        </div>

        <!-- ✅ Home Button -->
        <div class="text-center mt-4">
            <asp:Button ID="btnHome" runat="server" Text="Back to Home" CssClass="btn btn-secondary" OnClick="btnHome_Click" />
        </div>

    </form>

    <script>
        window.addEventListener("pageshow", function (event) {
            if (event.persisted) {
                window.location.reload();
            }
        });
    </script>
</body>
</html>
