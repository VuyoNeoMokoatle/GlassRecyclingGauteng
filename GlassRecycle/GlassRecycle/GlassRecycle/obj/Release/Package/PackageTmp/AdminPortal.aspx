<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminPortal.aspx.cs" Inherits="GlassRecycle.AdminPortal" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Portal - Glass Recycling</title>
    <script src="https://cdn.tailwindcss.com"></script>
    <style>
        body { background: linear-gradient(135deg, #4CAF50 0%, #81C784 100%); font-family: 'Poppins', sans-serif; }
        .glass-panel { backdrop-filter: blur(20px); background: rgba(255,255,255,0.2); border:1px solid rgba(255,255,255,0.3); box-shadow:0 10px 25px rgba(0,0,0,0.1);}
        .btn-green{background-color:#4CAF50; transition:all 0.3s ease;} .btn-green:hover{background-color:#43A047; transform:translateY(-2px);}
        .btn-gray{background-color:#9E9E9E; transition:all 0.3s ease;} .btn-gray:hover{background-color:#757575; transform:translateY(-2px);}
        .btn-red{background-color:#E53935; transition:all 0.3s ease;} .btn-red:hover{background-color:#C62828; transform:translateY(-2px);}
        table{border-collapse:collapse; width:100%; border-radius:10px; overflow:hidden;}
        th{background-color:#388E3C; color:white; padding:12px;}
        td{background:rgba(255,255,255,0.8); padding:10px;}
        tr:nth-child(even) td{background:rgba(255,255,255,0.6);}
        .product-img{width:80px; height:80px; object-fit:cover; border-radius:10px; border:2px solid #E0E0E0;}
        #toast{position:fixed; bottom:2rem; right:2rem; padding:1rem 1.5rem; border-radius:10px; color:white; font-weight:600; opacity:0; transition:opacity 0.5s; z-index:50;}
    </style>
</head>
<body class="flex items-center justify-center min-h-screen p-10">
    <form id="form1" runat="server" class="w-full max-w-6xl">
        <div id="toast"><span id="toastMessage"></span></div>
        <div class="glass-panel rounded-3xl p-10 text-gray-800">
            <h2 class="text-4xl font-bold text-center mb-10 text-white drop-shadow-md">🧩 Admin Portal - Glass Recycling</h2>
            <asp:Label ID="lblMessage" runat="server" CssClass="block text-center mb-6 text-lg font-semibold text-red-200"></asp:Label>

            <!-- Product Entry -->
            <div class="grid grid-cols-2 md:grid-cols-4 gap-5 mb-8 text-gray-900">
                <div class="col-span-2 md:col-span-1">
                    <label class="block mb-2 font-semibold text-white">Product Name</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="w-full border border-gray-300 rounded-md px-3 py-2"></asp:TextBox>
                </div>
                <div class="col-span-2 md:col-span-1">
                    <label class="block mb-2 font-semibold text-white">Price (R)</label>
                    <asp:TextBox ID="txtPrice" runat="server" CssClass="w-full border border-gray-300 rounded-md px-3 py-2"></asp:TextBox>
                </div>
                <div class="col-span-2">
                    <label class="block mb-2 font-semibold text-white">Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="2" CssClass="w-full border border-gray-300 rounded-md px-3 py-2"></asp:TextBox>
                </div>
                <div class="col-span-2">
                    <label class="block mb-2 font-semibold text-white">Product Image</label>
                    <asp:FileUpload ID="fuImage" runat="server" CssClass="w-full border border-gray-300 rounded-md px-3 py-2" />
                </div>
            </div>

            <!-- Buttons -->
            <div class="flex justify-center space-x-6 mb-8">
                <asp:Button ID="btnAddProduct" runat="server" Text="Add Product" CssClass="btn-green text-white py-2 px-6 rounded-lg font-semibold" OnClick="btnAddProduct_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn-gray text-white py-2 px-6 rounded-lg font-semibold" OnClick="btnClear_Click" />
            </div>

            <!-- Product Table -->
            <div class="overflow-x-auto rounded-xl">
                <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                    CssClass="text-center w-full border"
                    OnRowEditing="gvProducts_RowEditing"
                    OnRowCancelingEdit="gvProducts_RowCancelingEdit"
                    OnRowUpdating="gvProducts_RowUpdating"
                    OnRowDeleting="gvProducts_RowDeleting">

                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="Name" HeaderText="Name" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="Price" HeaderText="Price (R)" DataFormatString="{0:F2}" />
                        <asp:TemplateField HeaderText="Image">
                            <ItemTemplate>
                                <img src='ImageHandler.ashx?id=<%# Eval("Id") %>' alt="Product Image" class="product-img" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:FileUpload ID="fuEditImage" runat="server" CssClass="w-full border rounded px-2 py-1" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Logout -->
            <div class="text-center mt-8">
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-red text-white py-2 px-8 rounded-lg font-semibold" OnClick="btnLogout_Click" />
            </div>
        </div>
    </form>

    <script>
        function showToast(message, color = 'green') {
            const toast = document.getElementById("toast");
            const toastMessage = document.getElementById("toastMessage");
            toastMessage.innerText = message;
            toast.style.backgroundColor = color === 'green' ? '#4CAF50' : '#E53935';
            toast.style.opacity = '1';
            setTimeout(() => { toast.style.opacity = '0'; }, 2500);
        }
    </script>
</body>
</html>
