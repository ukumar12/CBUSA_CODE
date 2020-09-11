Imports Components
Imports DataLayer

Partial Class takeoffs_select_builder
    Inherits SitePage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

       PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

        If Not IsPostBack Then

         Core.DataLog("Edit Account Information", PageURL, CurrentUserId, "Vendor Left Menu Click", "", "", "", "", UserName)

            Session("SelectedLLCID") = Nothing

            'Dim LLCID As Integer = Core.GetInt(DB.ExecuteScalar("select top 1 LLCID from LLCVendor where VendorID=" & DB.Number(Session("VendorId"))))
            acBuilder.WhereClause = " IsActive =1 AND  LLCID In (" & VendorRow.GetLLCList(DB, Session("VendorId")) & ")"
        End If

    End Sub

    Protected Sub btnSelect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSelect.Click
        If Not acBuilder.Visible Then
            rfvacBuilder.Enabled = False
        End If
        Session("TakeoffForId") = acBuilder.Value
        Session("CurrentTakeoffId") = Nothing

        'log select builder 
        Core.DataLog("Create Takeoff", PageURL, CurrentUserId, "Select Builder", "", "", "", "", UserName)
        'end log

        Response.Redirect("edit.aspx")
    End Sub

    'Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
       ' Response.Redirect("/vendor/default.aspx")
   ' End Sub

End Class
