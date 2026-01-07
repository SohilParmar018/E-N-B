using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;

namespace EnoticeBoard_And_Suggestionbox
{
    public partial class ENSB : System.Web.UI.Page
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["NoticeDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Strong no-cache headers (set on every request)
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(-1));
            Response.Cache.SetValidUntilExpires(false);
            Response.AppendHeader("Pragma", "no-cache");
            Response.AppendHeader("Expires", "0");
            Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate, private, max-age=0");
            // Some browsers require this header
            try { Response.Headers.Add("Surrogate-Control", "no-store"); } catch { /* ignore on older servers */ }

            // If session is gone, ensure UI is guest-only (runs on every request)
            if (Session["UserName"] == null && Request.Url.AbsolutePath.EndsWith("ENSB.aspx", StringComparison.OrdinalIgnoreCase))
            {
                btnLogin.Visible = true;
                btnLogout.Visible = false;
                adminControls.Visible = false;
                pnlAddEventBtn.Visible = false;
                lblUserInfo.Text = "Not logged in";
            }
            else
            {
                // Normal visibility based on session
                bool loggedIn = Session["UserName"] != null;
                btnLogin.Visible = !loggedIn;
                btnLogout.Visible = loggedIn;

                bool isAdmin = string.Equals(Convert.ToString(Session["UserRole"]), "Admin", StringComparison.OrdinalIgnoreCase);

                // ✅ Show admin-only panels
                adminControls.Visible = isAdmin;
                pnlAddEventBtn.Visible = isAdmin;

                // ✅ Show Admin Management button only if admin
                btnManageAdmins.Visible = isAdmin;

                lblUserInfo.Text = loggedIn ? "Welcome, " + Convert.ToString(Session["UserName"]) : "Not logged in";
            }

            // Load data only on first request
            if (!IsPostBack)
            {
                pageTitle.Text = "Home - E-Notice Board";
                lblNavTitle.Text = "Home";
                lblNavTitleFooter.Text = "E-Notice Board";

                LoadUpcomingEvents();
                RestoreLastSection();
                UpdateNotificationBadge();
            }
        }

        private void RestoreLastSection()
        {
            string lastSection = Request.QueryString["section"];

            switch (lastSection)
            {
                case "notices":
                    BtnNotices_Click(null, null);
                    break;
                case "suggestions":
                    BtnSuggest_Click(null, null);
                    break;
                case "about":
                    BtnAbout_Click(null, null);
                    break;
                default:
                    BtnHome_Click(null, null);
                    break;
            }
        }




        #region Events
        private void LoadUpcomingEvents()
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT EventID, EventTitle, EventDate FROM UpcomingEvents ORDER BY EventDate", con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptEvents.DataSource = dt;
                rptEvents.DataBind();
            }
        }

        protected void rptEvents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Panel pnl = e.Item.FindControl("pnlEventAdmin") as Panel;
                if (pnl != null)
                {
                    pnl.Visible = string.Equals(Convert.ToString(Session["UserRole"]), "Admin", StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        protected void rptEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!string.Equals(Convert.ToString(Session["UserRole"]), "Admin", StringComparison.OrdinalIgnoreCase))
                return;

            int eventId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "DeleteEvent")
            {
                using (SqlConnection con = new SqlConnection(_conStr))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM UpcomingEvents WHERE EventID = @ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", eventId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadUpcomingEvents();
            }
            else if (e.CommandName == "EditEvent")
            {
                // Load event data into modal for editing
                using (SqlConnection con = new SqlConnection(_conStr))
                using (SqlCommand cmd = new SqlCommand("SELECT EventTitle, EventDate FROM UpcomingEvents WHERE EventID = @ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", eventId);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string title = dr["EventTitle"].ToString();
                            DateTime date = Convert.ToDateTime(dr["EventDate"]);

                            hfEditingEventID.Value = eventId.ToString();
                            txtEventTitle.Text = title;
                            txtEventDate.Text = date.ToString("yyyy-MM-dd");

                            // Show modal (via script)
                            ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                                "var m = new bootstrap.Modal(document.getElementById('addEventModal')); m.show(); " +
                                "document.getElementById('eventModalTitle').innerText = 'Edit Event';",
                                true);
                        }
                    }
                }
            }
        }

        protected void btnSaveEvent_Click(object sender, EventArgs e)
        {
            lblEventError.Visible = false;

            string title = txtEventTitle.Text?.Trim();
            DateTime date;

            if (string.IsNullOrWhiteSpace(title) || !DateTime.TryParse(txtEventDate.Text, out date))
            {
                lblEventError.Text = "Please enter valid title and date.";
                lblEventError.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowEventModalOnError",
                    "var m = new bootstrap.Modal(document.getElementById('addEventModal')); m.show();", true);
                return;
            }

            int editingId = 0;
            if (!string.IsNullOrEmpty(hfEditingEventID.Value))
            {
                int.TryParse(hfEditingEventID.Value, out editingId);
            }

            if (editingId > 0)
            {
                // Update existing event
                using (SqlConnection con = new SqlConnection(_conStr))
                using (SqlCommand cmd = new SqlCommand("UPDATE UpcomingEvents SET EventTitle = @Title, EventDate = @Date WHERE EventID = @ID", con))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@ID", editingId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                // New insert
                using (SqlConnection con = new SqlConnection(_conStr))
                using (SqlCommand cmd = new SqlCommand("INSERT INTO UpcomingEvents (EventTitle, EventDate) VALUES (@Title, @Date)", con))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Date", date);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Clear fields
            hfEditingEventID.Value = "";
            txtEventTitle.Text = "";
            txtEventDate.Text = "";

            LoadUpcomingEvents();

            ScriptManager.RegisterStartupScript(this, GetType(), "EventSaved",
                "alert('Event saved successfully.');", true);
        }
        #endregion

        #region Navigation Buttons
        protected void BtnHome_Click(object sender, EventArgs e)
        {
            homeSection.Visible = true;
            noticeSection.Visible = false;
            suggestionSection.Visible = false;
            aboutSection.Visible = false;

            pageTitle.Text = "Home - E-Notice Board";
            lblNavTitle.Text = "Home";

            ScriptManager.RegisterStartupScript(this, GetType(), "updateUrlHome",
            "window.history.pushState({}, '', '?section=home');", true);

        }

        protected void BtnNotices_Click(object sender, EventArgs e)
        {
            if (Session["UserName"] != null)
            {
                homeSection.Visible = false;
                suggestionSection.Visible = false;
                noticeSection.Visible = true;
                aboutSection.Visible = false;

                LoadNotices();

                pageTitle.Text = "Notices - E-Notice Board";
                lblNavTitle.Text = "Notices";
                ScriptManager.RegisterStartupScript(this, GetType(), "updateUrlNotices",
    "window.history.pushState({}, '', '?section=notices');", true);

            }
            else
            {
                homeSection.Visible = true;
                noticeSection.Visible = false;
                suggestionSection.Visible = false;
                aboutSection.Visible = false;

                ScriptManager.RegisterStartupScript(this, GetType(), "needLogin",
                    "alert('Please login to view notices.');", true);
            }
        }

        protected void BtnSuggest_Click(object sender, EventArgs e)
        {
            homeSection.Visible = false;
            noticeSection.Visible = false;
            suggestionSection.Visible = true;
            aboutSection.Visible = false;

            pageTitle.Text = "Suggestion Box - E-Notice Board";
            lblNavTitle.Text = "Suggestion Box";

            // Show appropriate panels depending on role
            bool isAdmin = string.Equals(Convert.ToString(Session["UserRole"]), "Admin", StringComparison.OrdinalIgnoreCase);
            if (isAdmin)
            {
                pnlSuggestAdmin.Visible = true;
                pnlSuggestUser.Visible = false;
                pnlSubmitSuggestion.Visible = false;
                LoadAllSuggestions();
            }
            else
            {
                // logged-in user can submit and view their suggestions
                if (Session["UserName"] != null)
                {
                    pnlSubmitSuggestion.Visible = true;
                    pnlSuggestUser.Visible = true;
                    pnlSuggestAdmin.Visible = false;
                    LoadMySuggestions();
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateUrlSuggestions",
    "window.history.pushState({}, '', '?section=suggestions');", true);

                }
                else
                {
                    homeSection.Visible = true;

                    pnlSubmitSuggestion.Visible = false;
                    pnlSuggestUser.Visible = false;
                    pnlSuggestAdmin.Visible = false;

                    ScriptManager.RegisterStartupScript(this, GetType(), "needLoginSuggest",
                        "alert('You must login to access the suggestion box.');", true);
                }
            }

            // hide chat initially
            pnlChat.Visible = false;
        }

        protected void BtnAbout_Click(object sender, EventArgs e)
        {
            homeSection.Visible = false;
            noticeSection.Visible = false;
            suggestionSection.Visible = false;
            aboutSection.Visible = true;

            pageTitle.Text = "About - E-Notice Board";
            lblNavTitle.Text = "About";
            ScriptManager.RegisterStartupScript(this, GetType(), "updateUrlAbout",
    "window.history.pushState({}, '', '?section=about');", true);

        }

        protected void BtnUserLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UserLogin.aspx", false);
            
        }

        protected void BtnAdminLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AdminLogin.aspx", false);
            
        }

        protected void BtnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Register.aspx", false);
            
        }

        protected void btnManageAdmins_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminManagement.aspx");

        }


        protected void BtnLogout_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Expire session cookie
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var c = new HttpCookie("ASP.NET_SessionId") { Expires = DateTime.Now.AddDays(-1) };
                Response.Cookies.Add(c);
            }

            // Strong server-side no-cache again
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(-1));
            Response.AppendHeader("Pragma", "no-cache");
            Response.AppendHeader("Expires", "0");
            Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate, private, max-age=0");
            try { Response.Headers.Add("Surrogate-Control", "no-store"); } catch { }

            // Redirect to ENSB.aspx — use false + CompleteRequest to avoid thread abort
            Response.Redirect("~/ENSB.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        #endregion

        #region Notices (existing)
        private void LoadNotices()
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT NoticeID, Title, Content, DateCreated FROM Notices ORDER BY DateCreated DESC", con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptNotices.DataSource = dt;
                rptNotices.DataBind();
            }
        }

        protected void BtnSaveNewNotice_Click(object sender, EventArgs e)
        {
            if (!string.Equals(Convert.ToString(Session["UserRole"]), "Admin", StringComparison.OrdinalIgnoreCase))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "notAdmin",
                    "alert('Only admins can add notices.');", true);
                return;
            }

            lblAddNoticeError.Visible = false;

            string title = txtNewTitle.Text?.Trim() ?? "";
            string content = txtNewContent.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                lblAddNoticeError.Text = "Title and Content are required.";
                lblAddNoticeError.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowAddModalOnError",
                    "var m = new bootstrap.Modal(document.getElementById('addNoticeModal')); m.show();", true);
                return;
            }

            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Notices (Title, Content, DateCreated) VALUES (@Title, @Content, @DateCreated)", con))
            {
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Content", content);
                cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            LoadNotices();
            txtNewTitle.Text = "";
            txtNewContent.Text = "";

            ScriptManager.RegisterStartupScript(this, GetType(), "HideAddModal",
                "var modalEl = document.getElementById('addNoticeModal'); var bsModal = bootstrap.Modal.getInstance(modalEl); if(!bsModal){ bsModal = new bootstrap.Modal(modalEl); } bsModal.hide(); alert('Notice added successfully.');",
                true);
        }

        protected void rptNotices_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Panel pnl = e.Item.FindControl("pnlItemAdmin") as Panel;
                if (pnl != null)
                {
                    pnl.Visible = string.Equals(Convert.ToString(Session["UserRole"]), "Admin", StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        protected void rptNotices_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!string.Equals(Convert.ToString(Session["UserRole"]), "Admin", StringComparison.OrdinalIgnoreCase)) return;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int id)) return;

            if (e.CommandName == "DeleteNotice")
            {
                using (SqlConnection con = new SqlConnection(_conStr))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Notices WHERE NoticeID=@ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadNotices();
            }
        }
        #endregion

        #region Suggestions & Chat (NEW)

        // Submit suggestion (user)
        protected void BtnSubmitSuggestion_Click(object sender, EventArgs e)
        {
            if (Session["UserName"] == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "needLogin",
                    "alert('You must be logged in to submit a suggestion.');", true);
                return;
            }

            string userName = Convert.ToString(Session["UserName"]);
            string email = txtEmail.Text?.Trim() ?? "";
            string suggestionText = txtSuggestion.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(suggestionText))
            {
                lblSuggestMessage.Text = "Please enter a suggestion.";
                lblSuggestMessage.CssClass = "text-danger d-block mb-3";
                lblSuggestMessage.Visible = true;
                return;
            }

            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Suggestions (UserName, Email, SuggestionText, DateCreated) VALUES (@UserName, @Email, @SuggestionText, GETDATE())", con))
            {
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@SuggestionText", suggestionText);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            txtSuggestion.Text = "";
            txtEmail.Text = "";
            txtName.Text = "";

            lblSuggestMessage.Text = "✅ Suggestion submitted successfully!";
            lblSuggestMessage.CssClass = "text-success d-block mb-3";
            lblSuggestMessage.Visible = true;

            // reload lists and notification badge
            LoadMySuggestions();
            LoadAllSuggestions();
            UpdateNotificationBadge();
        }

        // Admin: load all suggestions
        private void LoadAllSuggestions()
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT SuggestionID, UserName, Email, SuggestionText, DateCreated FROM Suggestions ORDER BY DateCreated DESC", con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptAllSuggestions.DataSource = dt;
                rptAllSuggestions.DataBind();
            }
        }

        // User: load only their suggestions
        private void LoadMySuggestions()
        {
            if (Session["UserName"] == null) return;

            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT SuggestionID, UserName, Email, SuggestionText, DateCreated FROM Suggestions WHERE UserName=@User ORDER BY DateCreated DESC", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@User", Session["UserName"]);
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptMySuggestions.DataSource = dt;
                rptMySuggestions.DataBind();
            }
        }

        // Admin clicked View
        protected void rptAllSuggestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                int suggestionId = Convert.ToInt32(e.CommandArgument);
                hfSuggestionID.Value = suggestionId.ToString();
                pnlChat.Visible = true;
                pnlSuggestAdmin.Visible = true;
                pnlSuggestUser.Visible = false;
                LoadChat(suggestionId);
            }
        }

        // User clicked View
        protected void rptMySuggestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                int suggestionId = Convert.ToInt32(e.CommandArgument);
                hfSuggestionID.Value = suggestionId.ToString();
                pnlChat.Visible = true;
                pnlSuggestUser.Visible = true;
                pnlSuggestAdmin.Visible = false;
                LoadChat(suggestionId);

                // mark admin replies as read for this user when they open conversation
                MarkAdminRepliesRead(suggestionId);
                UpdateNotificationBadge();
            }
        }

        // Load chat messages for a suggestion
        private void LoadChat(int suggestionId)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT ReplyID, SuggestionID, SenderRole, MessageText, DateSent, IsRead FROM SuggestionReplies WHERE SuggestionID = @ID ORDER BY DateSent", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@ID", suggestionId);
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptChat.DataSource = dt;
                rptChat.DataBind();
            }

            // scroll to bottom can be done client-side if desired (not included)
        }

        // Send reply (admin or user)
        protected void btnSendReply_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReply.Text)) return;
            if (string.IsNullOrEmpty(hfSuggestionID.Value)) return;

            int suggestionId = Convert.ToInt32(hfSuggestionID.Value);

            string role = (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin") ? "Admin" : "User";
            string message = txtReply.Text.Trim();

            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO SuggestionReplies (SuggestionID, SenderRole, MessageText, DateSent, IsRead) VALUES (@ID, @Role, @Text, GETDATE(), @IsRead)", con))
            {
                cmd.Parameters.AddWithValue("@ID", suggestionId);
                cmd.Parameters.AddWithValue("@Role", role);
                cmd.Parameters.AddWithValue("@Text", message);
                // if admin sends a reply, it's unread for user (IsRead = 0). If user sends reply, mark as unread for admin (we keep IsRead = 0)
                cmd.Parameters.AddWithValue("@IsRead", 0);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            txtReply.Text = "";
            LoadChat(suggestionId);
            UpdateNotificationBadge();
        }

        protected void btnCloseChat_Click(object sender, EventArgs e)
        {
            pnlChat.Visible = false;
            hfSuggestionID.Value = "";
            txtReply.Text = "";
            // Refresh lists
            LoadMySuggestions();
            LoadAllSuggestions();
            UpdateNotificationBadge();
        }

        // Mark admin replies as read when the user opens the thread
        private void MarkAdminRepliesRead(int suggestionId)
        {
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin")
                return; // admin doesn't need to mark

            using (SqlConnection con = new SqlConnection(_conStr))
            using (SqlCommand cmd = new SqlCommand("UPDATE SuggestionReplies SET IsRead = 1 WHERE SuggestionID = @ID AND SenderRole = 'Admin' AND IsRead = 0", con))
            {
                cmd.Parameters.AddWithValue("@ID", suggestionId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Notification badge: for user => unread admin replies; for admin => new suggestions without replies
        private void UpdateNotificationBadge()
        {
            string badgeHtml = "";
            int count = 0;

            bool isAdmin = string.Equals(Convert.ToString(Session["UserRole"]), "Admin", StringComparison.OrdinalIgnoreCase);
            if (isAdmin)
            {
                // Count suggestions with zero replies (i.e., need admin attention)
                using (SqlConnection con = new SqlConnection(_conStr))
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Suggestions s WHERE NOT EXISTS (SELECT 1 FROM SuggestionReplies r WHERE r.SuggestionID = s.SuggestionID)", con))
                {
                    con.Open();
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (count > 0)
                {
                    badgeHtml = $"<span class='badge bg-danger badge-notify'>{count}</span>";
                }
            }
            else
            {
                // For regular user: count unread admin replies to their suggestions
                if (Session["UserName"] != null)
                {
                    using (SqlConnection con = new SqlConnection(_conStr))
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM SuggestionReplies r INNER JOIN Suggestions s ON r.SuggestionID = s.SuggestionID WHERE s.UserName = @User AND r.SenderRole = 'Admin' AND r.IsRead = 0", con))
                    {
                        cmd.Parameters.AddWithValue("@User", Session["UserName"]);
                        con.Open();
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (count > 0)
                    {
                        badgeHtml = $"<span class='badge bg-danger badge-notify'>{count}</span>";
                    }
                }
            }

            // place into notifBadge span
            notifBadge.InnerHtml = badgeHtml;
        }

        #endregion



    }
}
