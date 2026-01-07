<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserLogin.aspx.cs" Inherits="EnoticeBoard_And_Suggestionbox.UserLogin" %>

<!DOCTYPE html>
<html runat="server">
<head>
    <meta charset="UTF-8" />
    <title>User Login</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        /* Background gradient + animated circles */
        body {
            margin: 0;
            padding: 0;
            height: 100vh;
            background: linear-gradient(135deg, #a8edea, #fed6e3);
            overflow: hidden;
            position: relative;
        }

        .circle1, .circle2 {
            position: absolute;
            border-radius: 50%;
            background: rgba(255, 255, 255, 0.25);
            animation: float 6s ease-in-out infinite;
        }

        .circle1 {
            top: -60px;
            left: -60px;
            width: 220px;
            height: 220px;
        }

        .circle2 {
            bottom: -70px;
            right: -50px;
            width: 260px;
            height: 260px;
            animation: float2 8s ease-in-out infinite;
        }

        @keyframes float {
            0% { transform: translateY(0); }
            50% { transform: translateY(30px); }
            100% { transform: translateY(0); }
        }

        @keyframes float2 {
            0% { transform: translateY(0); }
            50% { transform: translateY(-30px); }
            100% { transform: translateY(0); }
        }

        .login-box {
            max-width: 400px;
            background: rgba(255, 255, 255, 0.9);
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0px 6px 20px rgba(0,0,0,0.15);
        }
    </style>
</head>

<body>
    <!-- Animated Background Shapes -->
    <div class="circle1"></div>
    <div class="circle2"></div>

    <form id="form1" runat="server" class="container mt-5 login-box">
        <h3 class="mb-4 text-center text-primary fw-bold">User Login</h3>

        <asp:Label ID="lblMessage" runat="server" ForeColor="red"></asp:Label>

        <div class="mb-3">
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="Email" TextMode="Email" />
        </div>

        <div class="mb-3">
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" Placeholder="Password" TextMode="Password" />
        </div>

        <asp:Button ID="btnLoginUser" runat="server" Text="Login" CssClass="btn btn-primary w-100" OnClick="btnLoginUser_Click" />

        <div class="mt-3 text-center">
            <asp:HyperLink runat="server" NavigateUrl="~/ENSB.aspx">← Back to Home</asp:HyperLink>
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
