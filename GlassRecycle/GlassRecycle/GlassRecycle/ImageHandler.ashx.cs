using System;
using System.Web;
using System.Data.SqlClient;

namespace GlassRecycle
{
    public class ImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (!int.TryParse(context.Request.QueryString["id"], out int id))
                return;

            string connStr = @"Server=tcp:appdeployments.database.windows.net,1433;Initial Catalog=GlassRecycling;Persist Security Info=False;User ID=appdeployments;Password=Disaster1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT ImageData FROM Products_ WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                byte[] data = cmd.ExecuteScalar() as byte[];
                conn.Close();

                if (data != null)
                {
                    context.Response.ContentType = "image/png"; // adjust if needed
                    context.Response.BinaryWrite(data);
                }
            }
        }

        public bool IsReusable => false;
    }
}
