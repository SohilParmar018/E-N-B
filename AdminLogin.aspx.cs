using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace EnoticeBoard_And_Suggestionbox
{
    public partial class AdminLogin : Page
    {
        // 🔗 Get connection string from Web.config
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["NoticeDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // ✅ Prevent browser cache restoring old values
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(-1));
            Response.AppendHeader("Pragma", "no-cache");
            Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate, private, max-age=0");

            if (!IsPostBack)
            {
                txtAdminEmail.Text = "";
                txtAdminPassword.Text = "";
            }
        }
        protected void btnLoginAdmin_Click(object sender, EventArgs e)
        {
            string email = txtAdminEmail.Text.Trim();
            string password = txtAdminPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Please enter both Email and Password.";
                return;
            }

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query = "SELECT AdminName FROM Admins WHERE AdminEmail=@Email AND AdminPassword=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    // ✅ Successful login
                    Session["UserName"] = result.ToString();
                    Session["UserRole"] = "Admin";
                    Response.Redirect("ENSB.aspx");
                }
                else
                {
                    lblMessage.Text = "Invalid email or password.";
                }
            }
        }
    }
}
