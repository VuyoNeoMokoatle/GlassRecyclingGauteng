<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Shop.aspx.cs" Inherits="GlassRecycle.Shop" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Shop - Glass Recycling</title>
    <script src="https://cdn.tailwindcss.com"></script>
    <style>
        body {
            background: linear-gradient(135deg, #4CAF50 0%, #81C784 100%);
            font-family: 'Poppins', sans-serif;
        }

        .glass-card {
            backdrop-filter: blur(15px);
            background: rgba(255, 255, 255, 0.15);
            border: 1px solid rgba(255, 255, 255, 0.3);
            border-radius: 20px;
            padding: 1rem;
            margin-bottom: 1.5rem;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            text-align: center;
        }

        .glass-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 15px 25px rgba(0,0,0,0.2);
        }

        .glass-card img {
            border-radius: 15px;
            transition: transform 0.3s ease;
        }

        .glass-card img:hover {
            transform: scale(1.1);
        }

        .btn-glass {
            background-color: #4CAF50;
            color: white;
            font-weight: 600;
            border-radius: 8px;
            padding: 0.5rem 1rem;
            transition: all 0.3s ease;
        }

        .btn-glass:hover {
            background-color: #43A047;
            transform: translateY(-2px);
        }

        .btn-cart {
            background-color: #FFC107;
            color: white;
            font-weight: 600;
            border-radius: 8px;
            padding: 0.5rem 1rem;
            transition: all 0.3s ease;
        }

        .btn-cart:hover {
            background-color: #FFA000;
            transform: translateY(-2px);
        }

        .cart-panel {
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            width: 90%;
            max-width: 700px;
            background: rgba(255,255,255,0.25);
            backdrop-filter: blur(20px);
            border: 1px solid rgba(255,255,255,0.3);
            border-radius: 20px;
            padding: 2rem;
            z-index: 50;
            display: none;
        }

        .cart-panel h3 {
            font-size: 1.5rem;
            font-weight: 700;
            color: #fff;
            margin-bottom: 1rem;
        }

        #toast {
            position: fixed;
            bottom: 2rem;
            right: 2rem;
            padding: 1rem 1.5rem;
            border-radius: 10px;
            color: white;
            font-weight: 600;
            opacity: 0;
            transition: opacity 0.5s;
            z-index: 50;
        }

        #cartBadge {
            position: absolute;
            top: -8px;
            right: -8px;
            background-color: #E53935;
            color: white;
            font-size: 0.8rem;
            font-weight: bold;
            padding: 2px 6px;
            border-radius: 50%;
        }

        .table-cart {
            width: 100%;
            border-collapse: collapse;
            color: white;
        }

        .table-cart th, .table-cart td {
            padding: 0.5rem;
            border-bottom: 1px solid rgba(255,255,255,0.3);
            text-align: center;
        }

        .table-cart th {
            font-weight: 700;
        }
    </style>
</head>
<body class="min-h-screen flex flex-col items-center justify-start py-10 relative">

    <!-- Toast Notification -->
    <div id="toast"><span id="toastMessage"></span></div>

    <form id="form1" runat="server" class="w-full max-w-6xl">

        <!-- Header -->
        <div class="flex justify-between w-full mb-8 px-4 relative">
            <h2 class="text-3xl font-bold text-white drop-shadow-md">
                Welcome, <asp:Label ID="lblUser" runat="server"></asp:Label>
            </h2>
            <div class="relative">
                <asp:Button ID="btnCart" runat="server" Text="🛒 Cart" CssClass="btn-cart" OnClick="btnCart_Click" />
                <span id="cartBadge">0</span>
            </div>
            <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-glass ml-4" OnClick="btnLogout_Click" />
        </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="block mb-6 text-white font-semibold text-lg"></asp:Label>

        <!-- Products Grid -->
        <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
            <asp:Repeater ID="rptProducts" runat="server" OnItemCommand="rptProducts_ItemCommand">
                <ItemTemplate>
                    <div class="glass-card">
                        <img src='ImageHandler.ashx?id=<%# Eval("Id") %>' class="w-full h-40 object-cover mb-4" />
                        <h3 class="text-xl font-semibold mb-2 text-white"><%# Eval("Name") %></h3>
                        <p class="text-white mb-2">Price: R <%# Eval("Price") %></p>
                        <asp:TextBox ID="txtQuantity" runat="server" Text="1" CssClass="w-16 mx-auto mb-2 text-center rounded border border-gray-300"></asp:TextBox><br />
                        <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" CommandName="AddToCart" CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn-cart" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- Cart Panel -->
        <asp:Panel ID="pnlCart" runat="server" CssClass="cart-panel p-6 rounded-lg">
            <h3>Your Cart</h3>
            <asp:GridView ID="gvCart" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvCart_RowDeleting" DataKeyNames="Id"
                CssClass="table-cart mb-4">
                <Columns>
                    <asp:BoundField DataField="ProductName" HeaderText="Product" />
                    <asp:BoundField DataField="Quantity" HeaderText="Qty" />
                    <asp:BoundField DataField="Price" HeaderText="Price (R)" DataFormatString="{0:F2}" />
                    <asp:BoundField DataField="Total" HeaderText="Total (R)" DataFormatString="{0:F2}" />
                    <asp:CommandField ShowDeleteButton="True" />
                </Columns>
            </asp:GridView>
            <asp:Label ID="lblTotal" runat="server" CssClass="text-lg font-bold text-white mb-4"></asp:Label><br />
            <div class="flex justify-between">
                <asp:Button ID="btnCheckout" runat="server" Text="Checkout" CssClass="btn-cart" OnClick="btnCheckout_Click" />
                <asp:Button ID="btnCloseCart" runat="server" Text="Close Cart" CssClass="btn-glass" OnClick="btnCloseCart_Click" />
            </div>
        </asp:Panel>

    </form>

    <script>
        // Toast Notification
        function showToast(message, color = 'green') {
            const toast = document.getElementById("toast");
            const toastMessage = document.getElementById("toastMessage");
            toastMessage.innerText = message;
            toast.style.backgroundColor = color === 'green' ? '#4CAF50' : '#E53935';
            toast.style.opacity = '1';
            setTimeout(() => { toast.style.opacity = '0'; }, 2500);
        }

        // Prevent back button
        history.pushState(null, null, location.href);
        window.onpopstate = function () {
            history.go(1);
        };
    </script>
</body>
</html>
