<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ENSB.aspx.cs" Inherits="EnoticeBoard_And_Suggestionbox.ENSB" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>
    <asp:Literal ID="pageTitle" runat="server" Text="Home - E-Notice Board"></asp:Literal>
  </title>
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
  <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons/font/bootstrap-icons.css" rel="stylesheet" />
  <style>
    .user-info { color: white; margin-right: 15px; }
    .card { border-radius: 10px; }
    .card-title { font-weight: bold; }
    html, body, form { height: 100%; }
    body { display: flex; flex-direction: column; }
    form { flex: 1; display: flex; flex-direction: column; }
    .container { flex: 1; }
    .user-logo { font-size: 1.5rem; color: white; margin-right: 10px; }

    #adCarousel .carousel-inner img {
        height: 200px;
        object-fit: cover;
    }

    .app-titlebar {
    background-color: #212529;
    color: white;
    display: flex;
    align-items: center;
    justify-content: center; /* centers text */
    gap: 10px; /* space between logo and text */
    padding: 12px 20px;
    font-size: 1.8rem;
    font-weight: 600;
    letter-spacing: 1px;
    box-shadow: 0 2px 5px rgba(0,0,0,0.3);
    position: sticky;
    top: 0;
    z-index: 1050;
}

.app-titlebar img {
    height: 45px;
    width: auto;
    border-radius: 8px;
}

@media (max-width: 768px) {
    .app-titlebar {
        font-size: 1.2rem;
        padding: 8px 12px;
    }
    .app-titlebar img {
        height: 35px;
    }
}


    @media (max-width: 768px) {
        .app-titlebar {
            font-size: 1.2rem;
            padding: 8px 0;
        }
        .user-logo {
            font-size: 1.2rem;
            margin-right: 5px;
        }
        .user-info {
            font-size: 0.9rem;
        }
        .container, .container-fluid {
            padding-left: 15px;
            padding-right: 15px;
        }
        .card-title {
            font-size: 1rem;
        }
    }

    /* small badge styling */
    .badge-notify {
        position: relative;
        top: -10px;
        left: -8px;
    }
  </style>
</head>
<body>
<form id="form1" runat="server">
  <asp:ScriptManager ID="ScriptManager1" runat="server" />

 <!-- Title Bar -->
<div class="app-titlebar d-flex align-items-center justify-content-center position-relative">
    <img src="images/logo.png" alt="ENSB Logo" />
    <i class="bi bi-display me-2"></i> E-Notice Board & Suggestion Box System
</div>

  <!-- Navbar -->
  <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
      <div class="container-fluid">
          <a class="navbar-brand" href="#">
              <asp:Label ID="lblNavTitle" runat="server" Text="Home"></asp:Label>
          </a>
          <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav"
              aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
              <span class="navbar-toggler-icon"></span>
          </button>
          <div class="collapse navbar-collapse" id="navbarNav">
              <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                  <li class="nav-item">
                      <asp:LinkButton ID="btnHome" runat="server" CssClass="nav-link" OnClick="BtnHome_Click">Home</asp:LinkButton>
                  </li>
                  <li class="nav-item">
                      <asp:LinkButton ID="btnNotices" runat="server" CssClass="nav-link" OnClick="BtnNotices_Click">Notices</asp:LinkButton>
                  </li>
                  <li class="nav-item position-relative">
                      <asp:LinkButton ID="btnSuggest" runat="server" CssClass="nav-link" OnClick="BtnSuggest_Click">
                        Suggestion Box
                        &nbsp;<span id="notifBadge" runat="server"></span>
                      </asp:LinkButton>
                  </li>

              </ul>

             <div class="d-flex align-items-center gap-2">
    <i class="bi bi-person-circle user-logo"></i>
    <asp:Label ID="lblUserInfo" runat="server" CssClass="user-info" Text="Not logged in"></asp:Label>

    <!-- ✅ Admin Management button – visible only for admins -->
    <asp:Button ID="btnManageAdmins" runat="server"
        Text="Admin Management"
        CssClass="btn btn-warning btn-sm me-2"
        OnClick="btnManageAdmins_Click"
        Visible="false" />

    <!-- Login and Logout buttons -->
    <asp:Button ID="btnLogin" runat="server"
        Text="Login"
        CssClass="btn btn-outline-light btn-sm me-2"
        OnClientClick="var myModal = new bootstrap.Modal(document.getElementById('loginModal')); myModal.show(); return false;" />

    <asp:Button ID="btnLogout" runat="server"
        Text="Logout"
        CssClass="btn btn-outline-danger btn-sm"
        OnClick="BtnLogout_Click"
        Visible="false" />
</div>


          </div>
      </div>
  </nav>

  <!-- Login Modal -->
  <div class="modal fade" id="loginModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Login As</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
        </div>
        <div class="modal-body text-center">
          <asp:Button ID="btnUserLogin" runat="server" CssClass="btn btn-primary m-2" Text="User Login" OnClick="BtnUserLogin_Click" />
          <asp:Button ID="btnAdminLogin" runat="server" CssClass="btn btn-danger m-2" Text="Admin Login" OnClick="BtnAdminLogin_Click" />
          <hr />
          <asp:Button ID="btnRegister" runat="server" CssClass="btn btn-success" Text="Register" OnClick="BtnRegister_Click" />
        </div>
      </div>
    </div>
  </div>

  <!-- Page Content -->
  <div class="container mt-4">
   <div id="adCarousel" class="carousel slide mb-5 shadow-sm rounded" data-bs-ride="carousel" data-bs-interval="3500">
  <div class="carousel-inner rounded">
    <!-- Tech Event / Conference Ad -->
    <div class="carousel-item active">
      <a href="https://www.qualcomm.com" target="_blank">
        <img src="https://img.freepik.com/free-vector/technology-event-poster-template_23-2148927342.jpg?w=1200"
             class="d-block w-100" alt="Tech Innovation Event" />
      </a>
    </div>

    <!-- Education & E-learning Ad -->
    <div class="carousel-item">
      <a href="https://www.coursera.org" target="_blank">
        <img src="https://img.freepik.com/free-psd/education-online-learning-banner-template_23-2148915565.jpg?w=1200"
             class="d-block w-100" alt="Online Learning Ad" />
      </a>
    </div>

    <!-- Campus / College Life Promotion -->
    <div class="carousel-item">
      <a href="https://www.microsoft.com" target="_blank">
        <img src="https://img.freepik.com/free-psd/university-college-banner-template_23-2149178110.jpg?w=1200"
             class="d-block w-100" alt="Campus Event Banner" />
      </a>
    </div>

    <!-- Student Discount or Offer Ad -->
    <div class="carousel-item">
      <a href="https://www.apple.com/in/education/" target="_blank">
        <img src="https://img.freepik.com/free-psd/back-school-sale-banner-template_23-2148594058.jpg?w=1200"
             class="d-block w-100" alt="Student Discount Offer" />
      </a>
    </div>
  </div>

  <!-- Optional Carousel Controls -->
  <button class="carousel-control-prev" type="button" data-bs-target="#adCarousel" data-bs-slide="prev">
    <span class="carousel-control-prev-icon"></span>
  </button>
  <button class="carousel-control-next" type="button" data-bs-target="#adCarousel" data-bs-slide="next">
    <span class="carousel-control-next-icon"></span>
  </button>
</div>

    <!-- Home / Other Sections -->
    <asp:Panel ID="homeSection" runat="server" Visible="true">
      <div class="row">
        <div class="col-md-8">
          <div class="p-4 bg-light rounded shadow-sm mb-4">
            <h2 class="mb-3">👋 Welcome to E-Notice Board</h2>
            <p class="lead">Stay updated with the latest campus news, announcements, and suggestions. Use the menu to explore.</p>
          </div>
          <div class="row">
            <div class="col-md-4">
              <div class="card text-center shadow-sm">
                <div class="card-body">
                  <i class="bi bi-megaphone-fill fs-1 text-primary"></i>
                  <h5 class="card-title mt-2">Notices</h5>
                  <p class="card-text">Check the latest announcements.</p>
                  <asp:LinkButton ID="lnkNoticeCard" runat="server" CssClass="btn btn-primary btn-sm" OnClick="BtnNotices_Click">View</asp:LinkButton>
                </div>
              </div>
            </div>
            <div class="col-md-4">
              <div class="card text-center shadow-sm">
                <div class="card-body">
                  <i class="bi bi-lightbulb-fill fs-1 text-warning"></i>
                  <h5 class="card-title mt-2">Suggestions</h5>
                  <p class="card-text">Share your valuable ideas with us.</p>
                  <asp:LinkButton ID="lnkSuggestCard" runat="server" CssClass="btn btn-warning btn-sm" OnClick="BtnSuggest_Click">Contribute</asp:LinkButton>
                </div>
              </div>
            </div>
            <div class="col-md-4">
              <div class="card text-center shadow-sm">
                <div class="card-body">
                  <i class="bi bi-info-circle-fill fs-1 text-success"></i>
                  <h5 class="card-title mt-2">About</h5>
                  <p class="card-text">Learn more about our platform.</p>
                  <asp:LinkButton ID="lnkAboutCard" runat="server" CssClass="btn btn-success btn-sm" OnClick="BtnAbout_Click">Read</asp:LinkButton>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="bg-white p-3 rounded shadow-sm">
            <h5 class="mb-3"><i class="bi bi-calendar-event-fill"></i> Upcoming Events</h5>
            <ul class="list-group">
              <asp:Panel runat="server" ID="pnlUpcomingEventsWrapper">
                <asp:Repeater ID="rptEvents" runat="server" OnItemCommand="rptEvents_ItemCommand" OnItemDataBound="rptEvents_ItemDataBound">
                  <ItemTemplate>
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                      <span><%# Eval("EventTitle") %> - <%# Convert.ToDateTime(Eval("EventDate")).ToString("MMM dd") %></span>
                      <asp:Panel ID="pnlEventAdmin" runat="server" Visible="false">
                        <asp:LinkButton ID="btnDeleteEvent" runat="server" Text="Delete" CssClass="btn btn-sm btn-danger"
                            CommandName="DeleteEvent" CommandArgument='<%# Eval("EventID") %>'
                            OnClientClick="return confirm('Are you sure you want to delete this event?');" />
                      </asp:Panel>
                    </li>
                  </ItemTemplate>
                </asp:Repeater>
              </asp:Panel>
            </ul>
            <asp:Panel ID="pnlAddEventBtn" runat="server" CssClass="mt-2 text-end" Visible="false">
              <asp:Button ID="btnAddEvent" runat="server" Text="+ Add Event" CssClass="btn btn-success btn-sm"
                  OnClientClick="var m = new bootstrap.Modal(document.getElementById('addEventModal')); m.show(); return false;" />
            </asp:Panel>
          </div>
        </div>
      </div>
    </asp:Panel>

    <!-- Notices Section -->
    <asp:Panel ID="noticeSection" runat="server" Visible="false">
      <h2 class="mb-4">📢 Latest Notices</h2>
      <asp:Panel ID="adminControls" runat="server" CssClass="mb-3" Visible="false">
        <asp:Button ID="btnAddNotice" runat="server" CssClass="btn btn-success"
          Text="+ Add New Notice"
          OnClientClick="var m = new bootstrap.Modal(document.getElementById('addNoticeModal')); m.show(); return false;" />
      </asp:Panel>
      <asp:Repeater ID="rptNotices" runat="server" OnItemCommand="rptNotices_ItemCommand" OnItemDataBound="rptNotices_ItemDataBound">
        <ItemTemplate>
          <div class="card mb-3 shadow-sm">
            <div class="card-body">
              <h5 class="card-title"><%# Eval("Title") %></h5>
              <p class="card-text"><%# Eval("Content") %></p>
              <small class="text-muted">📅 <%# Eval("DateCreated", "{0:MMMM dd, yyyy h:mm tt}") %></small>
              <asp:Panel ID="pnlItemAdmin" runat="server" CssClass="mt-2" Visible="false">
                <asp:Button ID="btnDelete" runat="server" Text="Delete"
                  CommandName="DeleteNotice"
                  CommandArgument='<%# Eval("NoticeID") %>'
                  CssClass="btn btn-sm btn-danger"
                  OnClientClick="return confirm('Delete this notice?');" />
              </asp:Panel>
            </div>
          </div>
        </ItemTemplate>
      </asp:Repeater>
    </asp:Panel>

    <!-- Suggestion Section (submission + lists + chat) -->
    <asp:Panel ID="suggestionSection" runat="server" Visible="false" CssClass="position-relative p-3">
      <h2 class="mb-4">💡 Suggestions</h2>

      <!-- Submission (visible only to logged-in non-admin users) -->
      <asp:Panel ID="pnlSubmitSuggestion" runat="server" Visible="false" CssClass="mb-3">
        <asp:TextBox ID="txtName" runat="server" CssClass="form-control mb-3" Placeholder="Your Name (Optional)" />
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control mb-3" Placeholder="Your Email (Optional)" />
        <asp:TextBox ID="txtSuggestion" runat="server" CssClass="form-control mb-3" TextMode="MultiLine" Rows="4" Placeholder="Your Suggestion" />
        <asp:Label ID="lblSuggestMessage" runat="server" CssClass="d-block mb-3" Visible="false" />
        <asp:Button ID="btnSubmitSuggestion" runat="server" Text="Submit Suggestion" CssClass="btn btn-primary" OnClick="BtnSubmitSuggestion_Click" />
      </asp:Panel>

      <!-- Admin: list all suggestions -->
      <asp:Panel ID="pnlSuggestAdmin" runat="server" Visible="false">
          <h5>All Suggestions (Admin)</h5>
          <asp:Repeater ID="rptAllSuggestions" runat="server" OnItemCommand="rptAllSuggestions_ItemCommand">
              <ItemTemplate>
                  <div class="card mb-2">
                      <div class="card-body">
                          <div class="d-flex justify-content-between align-items-start">
                              <div>
                                  <h6 class="mb-1"><b><%# Eval("UserName") %></b></h6>
                                  <p class="mb-1"><%# Eval("SuggestionText") %></p>
                                  <small class="text-muted"><%# Eval("DateCreated", "{0:MMM dd, yyyy hh:mm tt}") %></small>
                              </div>
                              <div>
                                  <asp:LinkButton ID="lnkViewAdmin" runat="server" Text="View" CssClass="btn btn-sm btn-primary"
                                      CommandName="View" CommandArgument='<%# Eval("SuggestionID") %>' />
                              </div>
                          </div>
                      </div>
                  </div>
              </ItemTemplate>
          </asp:Repeater>
      </asp:Panel>

      <!-- User: list only their suggestions -->
      <asp:Panel ID="pnlSuggestUser" runat="server" Visible="false">
          <h5>My Suggestions</h5>
          <asp:Repeater ID="rptMySuggestions" runat="server" OnItemCommand="rptMySuggestions_ItemCommand">
              <ItemTemplate>
                  <div class="card mb-2">
                      <div class="card-body">
                          <div class="d-flex justify-content-between align-items-start">
                              <div>
                                  <p class="mb-1"><%# Eval("SuggestionText") %></p>
                                  <small class="text-muted"><%# Eval("DateCreated", "{0:MMM dd, yyyy hh:mm tt}") %></small>
                              </div>
                              <div>
                                  <asp:LinkButton ID="lnkViewUser" runat="server" Text="View" CssClass="btn btn-sm btn-outline-primary"
                                      CommandName="View" CommandArgument='<%# Eval("SuggestionID") %>' />
                              </div>
                          </div>
                      </div>
                  </div>
              </ItemTemplate>
          </asp:Repeater>
      </asp:Panel>

      <!-- Chat Panel (shared: admin & user) -->
      <asp:Panel ID="pnlChat" runat="server" Visible="false" CssClass="mt-3">
        <h5>Conversation</h5>
        <asp:HiddenField ID="hfSuggestionID" runat="server" />
        <div id="chatContainer" class="border rounded p-3 mb-3" style="height:300px; overflow-y:auto;">
            <asp:Repeater ID="rptChat" runat="server">
                <ItemTemplate>
                    <div class='<%# (Eval("SenderRole").ToString()=="Admin") ? "text-end text-danger" : "text-start text-primary" %>'>
                        <b><%# Eval("SenderRole") %>:</b> <%# Eval("MessageText") %>
                        <small class="text-muted d-block"><%# Eval("DateSent", "{0:MMM dd, yyyy hh:mm tt}") %></small>
                    </div>
                    <hr class="my-1" />
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <asp:TextBox ID="txtReply" runat="server" CssClass="form-control mb-2" Placeholder="Type your reply..." />
        <asp:Button ID="btnSendReply" runat="server" Text="Send Reply" CssClass="btn btn-success" OnClick="btnSendReply_Click" />
        &nbsp; <asp:Button ID="btnCloseChat" runat="server" Text="Close" CssClass="btn btn-secondary" OnClick="btnCloseChat_Click" />
      </asp:Panel>

    </asp:Panel>

    <!-- About Section -->
    <asp:Panel ID="aboutSection" runat="server" Visible="false">
      <div class="p-4 bg-light rounded shadow-sm">
        <h2 class="mb-3 text-success"><i class="bi bi-info-circle-fill"></i> About E-Notice Board & Suggestion Box System</h2>
        <p class="lead">
          The <b>E-Notice Board & Suggestion Box</b> is a digital platform designed to replace traditional notice boards.
          It allows students, faculty, and administrators to easily share announcements, post notices, and collect
          suggestions in a transparent and user-friendly way.
        </p>
        <div class="row mt-4">
          <div class="col-md-4">
            <div class="card shadow-sm h-100">
              <div class="card-body text-center">
                <i class="bi bi-broadcast fs-1 text-primary"></i>
                <h5 class="mt-2">Instant Notices</h5>
                <p class="text-muted">View and share important updates instantly.</p>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="card shadow-sm h-100">
              <div class="card-body text-center">
                <i class="bi bi-lightbulb fs-1 text-warning"></i>
                <h5 class="mt-2">Suggestions</h5>
                <p class="text-muted">Encourage users to share feedback and ideas.</p>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="card shadow-sm h-100">
              <div class="card-body text-center">
                <i class="bi bi-shield-check fs-1 text-success"></i>
                <h5 class="mt-2">Secure Access</h5>
                <p class="text-muted">Role-based login for admin and users.</p>
              </div>
            </div>
          </div>
        </div>
        <div class="alert alert-success mt-4 shadow-sm">
          <i class="bi bi-check-circle-fill"></i> Our goal is to improve communication and transparency across the campus.
        </div>
      </div>
    </asp:Panel>

  </div>

  <!-- Add Notice Modal -->
  <div class="modal fade" id="addNoticeModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Add New Notice</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
        </div>
        <div class="modal-body">
          <div class="mb-3">
            <label>Title</label>
            <asp:TextBox ID="txtNewTitle" runat="server" CssClass="form-control" />
          </div>
          <div class="mb-3">
            <label>Content</label>
            <asp:TextBox ID="txtNewContent" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
          </div>
          <asp:Label ID="lblAddNoticeError" runat="server" CssClass="text-danger" Visible="false" />
        </div>
        <div class="modal-footer">
          <asp:Button ID="btnSaveNewNotice" runat="server" Text="Save Notice" CssClass="btn btn-primary" OnClick="BtnSaveNewNotice_Click" />
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
        </div>


      </div>
    </div>
  </div>

  <!-- Add / Edit Event Modal -->
  <div class="modal fade" id="addEventModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="eventModalTitle">Add Upcoming Event</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
        </div>
        <div class="modal-body">
          <asp:HiddenField ID="hfEditingEventID" runat="server" />
          <div class="mb-3">
            <label>Event Title</label>
            <asp:TextBox ID="txtEventTitle" runat="server" CssClass="form-control" />
          </div>
          <div class="mb-3">
            <label>Event Date</label>
            <asp:TextBox ID="txtEventDate" runat="server" CssClass="form-control" TextMode="Date" />
          </div>
          <asp:Label ID="lblEventError" runat="server" CssClass="text-danger" Visible="false" />
        </div>
        <div class="modal-footer">
          <asp:Button ID="btnSaveEvent" runat="server" Text="Save Event" CssClass="btn btn-primary" OnClick="btnSaveEvent_Click" />
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
        </div>
      </div>
    </div>
  </div>

  <footer class="bg-dark text-white text-center py-3 mt-5">
    &copy; 2025 <asp:Label ID="lblNavTitleFooter" runat="server" Text="E-Notice Board"></asp:Label> | <span id="clock"></span>
  </footer>
</form>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
<script>
    function updateClock() {
        document.getElementById("clock").textContent = new Date().toLocaleTimeString();
    }
    setInterval(updateClock, 1000);
    updateClock();

    window.addEventListener('pageshow', function (event) {
        try {
            if (event.persisted || (performance && performance.navigation && performance.navigation.type === 2)) {
                // Force full reload from server
                window.location.reload(true);
            }
        } catch (e) {
            // fallback: do a normal reload
            window.location.reload();
        }
    });
</script>
</body>
</html>
