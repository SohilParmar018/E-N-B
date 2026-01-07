using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace EnoticeBoard_And_Suggestionbox
{
    public partial class UserLogin : Page
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["NoticeDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            // ✅ Prevent browser caching so page doesn’t restore form data
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(-1));
            Response.AppendHeader("Pragma", "no-cache");
            Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate, private, max-age=0");

            if (!IsPostBack)
            {
                // ✅ Clear fields on fresh load
                txtEmail.Text = "";
                txtPassword.Text = "";
            }
        }



        protected void btnLoginUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblMessage.Text = "Please enter both email and password.";
                return;
            }

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                try
                {
                    string query = "SELECT UserName, Email FROM Users WHERE Email=@Email AND Password=@Password";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        // ✅ Store user info in session
                        Session["UserName"] = dr["UserName"].ToString();
                        Session["UserEmail"] = dr["Email"].ToString();
                        Session["UserRole"] = "User";

                        Response.Redirect("ENSB.aspx");
                    }
                    else
                    {
                        lblMessage.Text = "Invalid email or password.";
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                }
            }
        }


    }
}
