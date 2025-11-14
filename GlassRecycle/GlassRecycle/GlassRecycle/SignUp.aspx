<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="GlassRecycle.SignUp" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sign Up - Glass Recycling</title>
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

        /* Popup styling */
        .popup {
            position: fixed;
            inset: 0;
            background: rgba(0,0,0,0.5);
            display: flex;
            justify-content: center;
            align-items: center;
            visibility: hidden;
            opacity: 0;
            transition: all 0.3s ease;
        }

        .popup.active {
            visibility: visible;
            opacity: 1;
        }

        .popup-card {
            background: white;
            color: #333;
            padding: 2rem;
            border-radius: 1rem;
            text-align: center;
            max-width: 400px;
            box-shadow: 0 10px 25px rgba(0,0,0,0.2);
            animation: popupIn 0.4s ease;
        }

        @keyframes popupIn {
            from { transform: scale(0.8); opacity: 0; }
            to { transform: scale(1); opacity: 1; }
        }
    </style>
</head>

<body class="flex items-center justify-center min-h-screen">
    <form id="form1" runat="server" class="w-full max-w-sm">
        <div class="glass-card shadow-2xl rounded-2xl p-8 text-white text-center">
            <h1 class="text-3xl font-semibold mb-2">Create Your Account 🌿</h1>
            <p class="text-sm mb-8 opacity-90">Join Glass Recycling Gauteng and help make a greener tomorrow</p>

            <asp:Label ID="lblMessage" runat="server" CssClass="block mb-4 text-sm text-red-200 font-semibold"></asp:Label>

            <!-- Email Field -->
            <div class="mb-5 text-left">
                <label class="block mb-2 text-sm font-medium">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="input-style w-full px-4 py-2 rounded-md border border-gray-300 text-gray-800"></asp:TextBox>
            </div>

            <!-- Password Field -->
            <div class="mb-5 text-left">
                <label class="block mb-2 text-sm font-medium">Password</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input-style w-full px-4 py-2 rounded-md border border-gray-300 text-gray-800"></asp:TextBox>
            </div>

            <!-- Role Selection -->
            <div class="mb-6 text-left">
                <label class="block mb-2 text-sm font-medium">Role</label>
                <asp:DropDownList ID="ddlRole" runat="server" CssClass="input-style w-full px-4 py-2 rounded-md border border-gray-300 text-gray-800">
                    <asp:ListItem Text="Select Role" Value=""></asp:ListItem>
                    <asp:ListItem Text="User" Value="User"></asp:ListItem>
                    <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <!-- Sign Up Button -->
            <asp:Button ID="btnSignUp" runat="server" Text="Sign Up" CssClass="w-full btn-green text-white py-2 rounded-md font-semibold" OnClick="btnSignUp_Click" />

            <div class="mt-6 text-sm">
                <p class="text-gray-200">
                    Already have an account?
                    <a href="SignIn.aspx" class="font-semibold text-white underline hover:text-green-200">Sign In</a>
                </p>
            </div>
        </div>

        <!-- Success Popup -->
        <div id="successPopup" class="popup">
            <div class="popup-card">
                <h2 class="text-2xl font-bold mb-4 text-green-600">✅ Account Created!</h2>
                <p class="text-gray-700 mb-6">Your account has been successfully registered.</p>
                <button type="button" onclick="redirectToSignIn()" class="bg-green-600 text-white px-6 py-2 rounded hover:bg-green-700">Continue to Sign In</button>
            </div>
        </div>
    </form>

    <script>
        // Detect when signup succeeded (based on label text)
        document.addEventListener("DOMContentLoaded", function () {
            const lbl = document.getElementById("<%= lblMessage.ClientID %>");
            if (lbl && lbl.innerText.includes("Account created successfully")) {
                const popup = document.getElementById("successPopup");
                popup.classList.add("active");
            }
        });

        function redirectToSignIn() {
            window.location.href = "SignIn.aspx";
        }
    </script>
</body>
</html>
