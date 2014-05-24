<%@ Page Language="C#" %>
<%@ Import Namespace="System.Web.Security" %>
<!DOCTYPE html>
<script runat="server">
public void Login_OnClick(object sender, EventArgs args)
{
   if (FormsAuthentication.Authenticate(UsernameTextbox.Text, PasswordTextbox.Text))
      FormsAuthentication.RedirectFromLoginPage(UsernameTextbox.Text, NotPublicCheckBox.Checked);
   else
     Msg.Text = "Login failed. Please check your user name and password and try again.";
}
</script>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Web-based File Editor - Login</title>
    <link rel="stylesheet" type="text/css" href="~/Content/Site.css" />
    <link rel="stylesheet" type="text/css" href="~/Content/TodoList.css" />
</head>
<body>
<div class="todoList" id="loginPanel">
    <section id="localLoginPanel">
        <form id="form1" runat="server">
          <h3>Login</h3>
          <asp:Label id="Msg" ForeColor="maroon" runat="server" /><br />
          Username: <asp:Textbox id="UsernameTextbox" runat="server" /><br />
          Password: <asp:Textbox id="PasswordTextbox" runat="server" TextMode="Password" /><br />
          <asp:Button id="LoginButton" Text="Login" OnClick="Login_OnClick" runat="server" />
          <asp:CheckBox id="NotPublicCheckBox" runat="server" /> 
          Remember me. (Only enable if this is a private computer.)
        </form>
    </section>
    </div>
</body>
</html>