using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace GlassRecycle
{
    public partial class SignUp : System.Web.UI.Page
    {
        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = ddlRole.SelectedValue;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                lblMessage.Text = "All fields are required.";
                return;
            }

            string passwordHash = HashPassword(password);
            string connectionString = @"Server=tcp:appdeployments.database.windows.net,1433;Initial Catalog=GlassRecycling;Persist Security Info=False;User ID=appdeployments;Password=Disaster1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users_ (Email, PasswordHash, Role) VALUES (@Email, @PasswordHash, @Role)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@Role", role);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    lblMessage.Text = "Account created successfully!";
                }
                catch (SqlException ex)
                {
                    if (ex.Message.Contains("UNIQUE"))
                        lblMessage.Text = "Email already registered.";
                    else
                        lblMessage.Text = "Error: " + ex.Message;
                }
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
