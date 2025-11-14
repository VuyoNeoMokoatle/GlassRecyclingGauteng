using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace GlassRecycle
{
    public partial class AdminPortal : System.Web.UI.Page
    {
        string connectionString = @"Server=tcp:appdeployments.database.windows.net,1433;Initial Catalog=GlassRecycling;Persist Security Info=False;User ID=appdeployments;Password=Disaster1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
                Response.Redirect("SignIn.aspx");

            if (!IsPostBack)
                LoadProducts();
        }

        private void LoadProducts()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Name, Description, Price FROM Products_ ORDER BY CreatedAt DESC";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvProducts.DataSource = dt;
                gvProducts.DataBind();
            }
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string desc = txtDescription.Text.Trim();

            if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price))
            {
                ShowToast("Please enter a valid price.", "red");
                return;
            }

            byte[] imageData = null;
            if (fuImage.HasFile)
            {
                using (var stream = fuImage.PostedFile.InputStream)
                using (var br = new System.IO.BinaryReader(stream))
                    imageData = br.ReadBytes((int)stream.Length);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Products_ (Name, Description, Price, ImageData) VALUES (@Name, @Desc, @Price, @ImageData)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Desc", desc);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@ImageData", (object)imageData ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                ClearForm();
                LoadProducts();
                ShowToast("✅ Product added successfully!");
            }
        }

        protected void gvProducts_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvProducts.EditIndex = e.NewEditIndex;
            LoadProducts();
        }

        protected void gvProducts_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvProducts.EditIndex = -1;
            LoadProducts();
        }

        protected void gvProducts_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvProducts.DataKeys[e.RowIndex].Value);
            string name = ((System.Web.UI.WebControls.TextBox)gvProducts.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
            string desc = ((System.Web.UI.WebControls.TextBox)gvProducts.Rows[e.RowIndex].Cells[2].Controls[0]).Text;
            decimal price = Convert.ToDecimal(((System.Web.UI.WebControls.TextBox)gvProducts.Rows[e.RowIndex].Cells[3].Controls[0]).Text);

            // Check for new image in EditItemTemplate
            byte[] imageData = null;
            var fuEdit = (System.Web.UI.WebControls.FileUpload)gvProducts.Rows[e.RowIndex].FindControl("fuEditImage");
            if (fuEdit != null && fuEdit.HasFile)
            {
                using (var stream = fuEdit.PostedFile.InputStream)
                using (var br = new System.IO.BinaryReader(stream))
                    imageData = br.ReadBytes((int)stream.Length);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = imageData != null
                    ? "UPDATE Products_ SET Name=@Name, Description=@Desc, Price=@Price, ImageData=@ImageData WHERE Id=@Id"
                    : "UPDATE Products_ SET Name=@Name, Description=@Desc, Price=@Price WHERE Id=@Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Desc", desc);
                cmd.Parameters.AddWithValue("@Price", price);
                if (imageData != null) cmd.Parameters.AddWithValue("@ImageData", imageData);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            gvProducts.EditIndex = -1;
            LoadProducts();
            ShowToast("✅ Product updated!");
        }

        protected void gvProducts_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvProducts.DataKeys[e.RowIndex].Value);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Products_ WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            LoadProducts();
            ShowToast("❌ Product deleted!", "red");
        }

        protected void btnClear_Click(object sender, EventArgs e) => ClearForm();
        private void ClearForm() => txtName.Text = txtDescription.Text = txtPrice.Text = "";

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("SignIn.aspx");
        }

        private void ShowToast(string message, string color = "green")
        {
            string script = $"showToast('{message}', '{color}');";
            ClientScript.RegisterStartupScript(this.GetType(), "toast", script, true);
        }
    }
}
