using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace GlassRecycle
{
    public partial class SignIn : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string passwordHash = HashPassword(password);

            string connectionString = @"Server=tcp:appdeployments.database.windows.net,1433;Initial Catalog=GlassRecycling;Persist Security Info=False;User ID=appdeployments;Password=Disaster1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Role FROM Users_ WHERE Email = @Email AND PasswordHash = @PasswordHash";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Session["UserId"] = reader["Id"];
                        Session["UserEmail"] = email;
                        Session["UserRole"] = reader["Role"].ToString();
                        lblMessage.ForeColor = System.Drawing.ColorTranslator.FromHtml("#A5D6A7");
                        lblMessage.Text = "Login successful for role: " + reader["Role"].ToString();
                    }
                    else
                    {
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        lblMessage.Text = "Invalid email or password.";
                    }
                }
                catch (SqlException ex)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
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
