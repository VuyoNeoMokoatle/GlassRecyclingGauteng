<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="GlassRecycle.SignIn" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sign In - Glass Recycling</title>
    <script src="https://cdn.tailwindcss.com"></script>

    <style>
        body {
            background: linear-gradient(135deg, #4CAF50 0%, #81C784 100%);
            font-family: 'Poppins', sans-serif;
        }

        .glass-card {
            backdrop-filter: blur(20px);
            background: rgba(255, 255, 255, 0.15);
            border: 1px solid rgba(255, 255, 255, 0.3);
        }

        .input-style {
            transition: all 0.3s ease;
        }

        .input-style:focus {
            border-color: #4CAF50;
            box-shadow: 0 0 0 3px rgba(76, 175, 80, 0.2);
        }

        .btn-green {
            background-color: #4CAF50;
            transition: all 0.3s ease;
        }

        .btn-green:hover {
            background-color: #43A047;
        }

        /* Loader overlay */
        .loader-overlay {
            position: fixed;
            inset: 0;
            background: rgba(0, 0, 0, 0.4);
            display: flex;
            align-items: center;
            justify-content: center;
            visibility: hidden;
            opacity: 0;
            transition: all 0.3s ease;
        }

        .loader-overlay.active {
            visibility: visible;
            opacity: 1;
        }

        .loader {
            border: 4px solid rgba(255, 255, 255, 0.3);
            border-top: 4px solid #4CAF50;
            border-radius: 50%;
            width: 60px;
            height: 60px;
            animation: spin 1s linear infinite;
        }

        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
    </style>
</head>

<body class="flex items-center justify-center min-h-screen">
    <form id="form1" runat="server" class="w-full max-w-sm">
        <div class="glass-card shadow-2xl rounded-2xl p-8 text-white text-center">
            <h1 class="text-3xl font-semibold mb-2">Welcome Back 👋</h1>
            <p class="text-sm mb-8 opacity-90">Sign in to continue your recycling journey</p>

            <asp:Label ID="lblMessage" runat="server" CssClass="block mb-4 text-sm font-semibold text-red-200"></asp:Label>

            <!-- Email -->
            <div class="mb-5 text-left">
                <label class="block mb-2 text-sm font-medium">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="input-style w-full px-4 py-2 rounded-md border border-gray-300 text-gray-800"></asp:TextBox>
            </div>

            <!-- Password -->
            <div class="mb-6 text-left">
                <label class="block mb-2 text-sm font-medium">Password</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input-style w-full px-4 py-2 rounded-md border border-gray-300 text-gray-800"></asp:TextBox>
            </div>

            <!-- Sign In -->
            <asp:Button ID="btnLogin" runat="server" Text="Sign In"
                CssClass="w-full btn-green text-white py-2 rounded-md font-semibold"
                OnClick="btnLogin_Click"
                OnClientClick="showLoader();" />

            <div class="mt-6 text-sm">
                <p class="text-gray-200">
                    Don’t have an account?
                    <a href="SignUp.aspx" class="font-semibold text-white underline hover:text-green-200">Sign Up</a>
                </p>
            </div>
        </div>

        <!-- Loader -->
        <div id="loaderOverlay" class="loader-overlay">
            <div class="text-center">
                <div class="loader mx-auto mb-4"></div>
                <p class="text-white text-lg font-semibold">Signing you in...</p>
            </div>
        </div>
    </form>

    <script>
        function showLoader() {
            document.getElementById("loaderOverlay").classList.add("active");
        }

        document.addEventListener("DOMContentLoaded", function () {
            const lbl = document.getElementById("<%= lblMessage.ClientID %>");
            const loader = document.getElementById("loaderOverlay");

            if (lbl && lbl.innerText.includes("Login successful")) {
                loader.classList.add("active");
                setTimeout(() => redirectToDashboard(lbl.innerText), 1500);
            } else if (lbl && lbl.innerText.length > 0 && !lbl.innerText.includes("Login successful")) {
                loader.classList.remove("active");
            }
        });

        function redirectToDashboard(message) {
            if (message.includes("Admin"))
                window.location.href = "AdminPortal.aspx";
            else
                window.location.href = "Shop.aspx";
        }
    </script>
</body>
</html>
