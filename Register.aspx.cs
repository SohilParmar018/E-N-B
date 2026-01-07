using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace EnoticeBoard_And_Suggestionbox
{
    public partial class Register : Page
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["NoticeDB"].ConnectionString;


    protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GenerateAutoCode();
            }
        }

        // Generate random verification code
        private void GenerateAutoCode()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random rnd = new Random();
            char[] code = new char[6];
            for (int i = 0; i < code.Length; i++)
            {
                code[i] = chars[rnd.Next(chars.Length)];
            }
            Session["AutoCode"] = new string(code);
            lblAutoCode.Text = Session["AutoCode"].ToString();
        }

        protected void btnRefreshCode_Click(object sender, EventArgs e)
        {
            GenerateAutoCode();
        }

        protected void btnRegisterUser_Click(object sender, EventArgs e)
        {
            // CAPTCHA check
            if (txtAutoCode.Text.Trim().ToUpper() != Session["AutoCode"].ToString())
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Incorrect code. Please try again!";
                GenerateAutoCode();
                return;
            }

            if (txtRegPassword.Text != txtRegConfirmPassword.Text)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Passwords do not match.";
                return;
            }

            string name = txtRegName.Text.Trim();
            string email = txtRegEmail.Text.Trim();
            string password = txtRegPassword.Text.Trim();

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                try
                {
                    con.Open();

                    // Check Users AND Admins tables for duplicate email
                    string checkQuery = @"
                    SELECT COUNT(*) FROM Users WHERE Email=@Email
                    UNION ALL
                    SELECT COUNT(*) FROM Admins WHERE AdminEmail=@Email
                ";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@Email", email);

                    int totalExists = 0;
                    using (SqlDataReader dr = checkCmd.ExecuteReader())
                    {
                        while (dr.Read())
                            totalExists += dr.GetInt32(0);
                    }

                    if (totalExists > 0)
                    {
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        lblMessage.Text = "⚠️ This email already exists as User !";
                        return;
                    }

                    // Insert user
                    string insertQuery = "INSERT INTO Users (UserName, Email, Password) VALUES (@Name, @Email, @Password)";
                    SqlCommand cmd = new SqlCommand(insertQuery, con);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        lblMessage.ForeColor = System.Drawing.Color.Green;
                        lblMessage.Text = "Registration successful! Redirecting...";
                        Response.AddHeader("REFRESH", "2;URL=UserLogin.aspx");
                    }
                    else
                    {
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        lblMessage.Text = "Registration failed. Try again.";
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "Error: " + ex.Message;
                }
            }
        }
    }


}
