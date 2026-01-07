using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace EnoticeBoard_And_Suggestionbox
{
    public partial class AdminManagement : Page
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["NoticeDB"].ConnectionString;


    protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                Response.Redirect("AdminLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadAdmins();
                LoadUsers();
            }
        }

        private void LoadAdmins(string search = "")
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query = "SELECT AdminID, AdminName, AdminEmail FROM Admins";

                if (!string.IsNullOrEmpty(search))
                    query += " WHERE AdminName LIKE @Search OR AdminEmail LIKE @Search";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                if (!string.IsNullOrEmpty(search))
                    da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                gvAdmins.DataSource = dt;
                gvAdmins.DataBind();
            }
        }

        private void LoadUsers(string search = "")
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                string query = "SELECT UserID, UserName, Email FROM Users";

                if (!string.IsNullOrEmpty(search))
                    query += " WHERE UserName LIKE @Search OR Email LIKE @Search";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                if (!string.IsNullOrEmpty(search))
                    da.SelectCommand.Parameters.AddWithValue("@Search", "%" + search + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                gvUsers.DataSource = dt;
                gvUsers.DataBind();
            }
        }

        protected void btnAddAdmin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAdminName.Text) ||
                string.IsNullOrWhiteSpace(txtAdminEmail.Text) ||
                string.IsNullOrWhiteSpace(txtAdminPassword.Text))
            {
                lblMessage.Text = "⚠️ Please fill all fields.";
                return;
            }

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                try
                {
                    con.Open();

                    // Prevent duplicate email: Check both Admins and Users tables
                    string checkQuery = @"
                    SELECT COUNT(*) FROM Admins WHERE AdminEmail=@Email
                    UNION ALL
                    SELECT COUNT(*) FROM Users WHERE Email=@Email
                ";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@Email", txtAdminEmail.Text.Trim());

                    int totalExists = 0;
                    using (SqlDataReader dr = checkCmd.ExecuteReader())
                    {
                        while (dr.Read())
                            totalExists += dr.GetInt32(0);
                    }

                    if (totalExists > 0)
                    {
                        lblMessage.CssClass = "text-danger fw-bold";
                        lblMessage.Text = "⚠️ This email is already used by a Admin.";
                        return;
                    }

                    // Insert admin
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Admins (AdminName, AdminEmail, AdminPassword) VALUES (@Name, @Email, @Password)", con);
                    cmd.Parameters.AddWithValue("@Name", txtAdminName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtAdminEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", txtAdminPassword.Text.Trim());
                    cmd.ExecuteNonQuery();

                    lblMessage.CssClass = "text-success fw-bold";
                    lblMessage.Text = "✅ Admin added successfully!";
                    txtAdminName.Text = txtAdminEmail.Text = txtAdminPassword.Text = "";

                    LoadAdmins();
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "❌ Error: " + ex.Message;
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            string type = ddlSearchType.SelectedValue;

            if (type == "Admins")
                LoadAdmins(searchText);
            else
                LoadUsers(searchText);
        }

        protected void btnShowAll_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            lblMessage.Text = "Showing all records...";
            lblMessage.CssClass = "text-success";
            LoadAdmins();
            LoadUsers();
        }

        protected void gvAdmins_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteAdmin")
            {
                int adminId = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Admins WHERE AdminID=@ID", con);
                    cmd.Parameters.AddWithValue("@ID", adminId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadAdmins();
            }
        }

        protected void gvUsers_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteUser")
            {
                int userId = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserID=@ID", con);
                    cmd.Parameters.AddWithValue("@ID", userId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadUsers();
            }
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("ENSB.aspx");
        }
    }


}
