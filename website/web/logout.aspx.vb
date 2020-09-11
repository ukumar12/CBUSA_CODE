Imports Components
Imports DataLayer

Partial Class logout
    Inherits SitePage
    Private CurrentBuilderId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private CurrentVendorId As String = ""
    Private CurrentPIQId As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        PageURL = Request.Url.ToString()
        CurrentBuilderId = Session("BuilderAccountID")
        CurrentVendorId = Session("VendorAccountId")
        CurrentPIQId = Session("PIQAccountId")
        UserName = Session("Username")
        Session("BuilderId") = Nothing
        Session("BuilderAccountId") = Nothing

        Utility.CookieUtil.SetTripleDESEncryptedCookie("cBuilderId", "", Now.AddDays(-1))
        Utility.CookieUtil.SetTripleDESEncryptedCookie("cBuilderAccountId", "", Now.AddDays(-1))

        Core.DataLog("Logout", PageURL, CurrentBuilderId, "Builder Logout", "", "", "", "", UserName) 'log builder logout

        Session("VendorId") = Nothing
        Session("VendorAccountId") = Nothing

        Utility.CookieUtil.SetTripleDESEncryptedCookie("cVendorId", "", Now.AddDays(-1))
        Utility.CookieUtil.SetTripleDESEncryptedCookie("cVendorAccountId", "", Now.AddDays(-1))

        Core.DataLog("Logout", PageURL, CurrentVendorId, "Vendor Logout", "", "", "", "", UserName) 'log vendor logout

        Session("PIQId") = Nothing
        Session("PIQAccountId") = Nothing

        Utility.CookieUtil.SetTripleDESEncryptedCookie("cPIQId", "", Now.AddDays(-1))
        Utility.CookieUtil.SetTripleDESEncryptedCookie("cPIQAccountId", "", Now.AddDays(-1))

        Core.DataLog("Logout", PageURL, CurrentPIQId, "PIQ Logout", "", "", "", "", UserName) 'log PIQ logout

        Session("CurrentTakeoffId") = Nothing
        Session("PriceComparisonId") = Nothing
        Session("SkipRegistrationCheck") = Nothing
        Session("SubStatusConfirmed") = Nothing
        Response.Redirect(GlobalSecureName & "/default.aspx")
    End Sub
End Class
