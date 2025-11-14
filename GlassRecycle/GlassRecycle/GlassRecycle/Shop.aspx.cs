using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GlassRecycle
{
    public partial class Shop : System.Web.UI.Page
    {
        string connectionString = @"Server=tcp:appdeployments.database.windows.net,1433;Initial Catalog=GlassRecycling;Persist Security Info=False;User ID=appdeployments;Password=Disaster1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
                Response.Redirect("SignIn.aspx");

            lblUser.Text = Session["UserEmail"].ToString();

            if (!IsPostBack)
            {
                LoadProducts();
                UpdateCartBadge();
            }
        }

        private void LoadProducts()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Name, Price FROM Products_ ORDER BY CreatedAt DESC";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Add ImageUrl column dynamically for Repeater
                dt.Columns.Add("ImageUrl", typeof(string));
                foreach (DataRow row in dt.Rows)
                    row["ImageUrl"] = "ImageHandler.ashx?id=" + row["Id"];

                rptProducts.DataSource = dt;
                rptProducts.DataBind();
            }
        }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddToCart")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                TextBox txtQty = (TextBox)e.Item.FindControl("txtQuantity");
                int quantity = Convert.ToInt32(txtQty.Text);
                AddToCart(productId, quantity);
            }
        }

        private void AddToCart(int productId, int quantity)
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string check = "SELECT COUNT(*) FROM Cart_ WHERE UserId=@UserId AND ProductId=@ProductId";
                SqlCommand cmdCheck = new SqlCommand(check, conn);
                cmdCheck.Parameters.AddWithValue("@UserId", userId);
                cmdCheck.Parameters.AddWithValue("@ProductId", productId);
                int exists = (int)cmdCheck.ExecuteScalar();

                if (exists > 0)
                {
                    string update = "UPDATE Cart_ SET Quantity=Quantity+@Quantity WHERE UserId=@UserId AND ProductId=@ProductId";
                    SqlCommand cmdUpdate = new SqlCommand(update, conn);
                    cmdUpdate.Parameters.AddWithValue("@Quantity", quantity);
                    cmdUpdate.Parameters.AddWithValue("@UserId", userId);
                    cmdUpdate.Parameters.AddWithValue("@ProductId", productId);
                    cmdUpdate.ExecuteNonQuery();
                }
                else
                {
                    string insert = "INSERT INTO Cart_ (UserId, ProductId, Quantity) VALUES (@UserId,@ProductId,@Quantity)";
                    SqlCommand cmdInsert = new SqlCommand(insert, conn);
                    cmdInsert.Parameters.AddWithValue("@UserId", userId);
                    cmdInsert.Parameters.AddWithValue("@ProductId", productId);
                    cmdInsert.Parameters.AddWithValue("@Quantity", quantity);
                    cmdInsert.ExecuteNonQuery();
                }
                conn.Close();
            }

            UpdateCartBadge();
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", $"showToast('✅ Added to cart!');", true);
        }

        protected void btnCart_Click(object sender, EventArgs e)
        {
            LoadCart();
            ScriptManager.RegisterStartupScript(this, GetType(), "showCart", $"document.getElementById('{pnlCart.ClientID}').style.display='block';", true);
        }

        private void LoadCart()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT c.Id, p.Name AS ProductName, c.Quantity, p.Price, (c.Quantity*p.Price) AS Total
                                 FROM Cart_ c
                                 JOIN Products_ p ON c.ProductId=p.Id
                                 WHERE c.UserId=@UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvCart.DataSource = dt;
                gvCart.DataBind();

                decimal total = 0;
                foreach (DataRow row in dt.Rows)
                    total += Convert.ToDecimal(row["Total"]);
                lblTotal.Text = $"Total: R {total:F2}";
            }
            UpdateCartBadge();
        }

        protected void gvCart_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvCart.DataKeys[e.RowIndex].Value);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Cart_ WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            LoadCart();
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", $"showToast('🗑️ Item removed!', 'red');", true);
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Cart_ WHERE UserId=@UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            LoadCart();
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", $"showToast('✅ Checkout complete!');", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "hideCart", $"document.getElementById('{pnlCart.ClientID}').style.display='none';", true);
        }

        protected void btnCloseCart_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "hideCart", $"document.getElementById('{pnlCart.ClientID}').style.display='none';", true);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("SignIn.aspx");
        }

        private void UpdateCartBadge()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SUM(Quantity) FROM Cart_ WHERE UserId=@UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                var result = cmd.ExecuteScalar();
                conn.Close();
                if (result != DBNull.Value) count = Convert.ToInt32(result);
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "updateBadge", $"document.getElementById('cartBadge').innerText='{count}';", true);
        }
    }
}
