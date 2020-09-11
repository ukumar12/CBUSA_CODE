Imports Components
Imports DataLayer
Imports Controls
Partial Class modules_Builders_TakeoffService
    Inherits ModuleControl

 

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If IsAdminDisplay Then
            Exit Sub
        End If


        Dim builderllcid As Integer = BuilderRow.GetRow(DB, Session("BuilderID")).LLCID

        If builderllcid > 0 Then
            BindList(DB, builderllcid)
        End If
    End Sub

    Private Sub BindList(ByVal DB As Database, ByVal LLCID As Integer)
        Dim Sql As String = String.Empty
        Sql &= "Select * from TakeOffService ts inner join TakeOffServiceLLC tsl on tsl.TakeOffServiceID = ts.TakeOffServiceID   where ts.isactive =1 and tsl.LLCID = " & LLCID
        Dim dt As DataTable = DB.GetDataTable(Sql)
        Me.rptTakeOFfService.DataSource = dt
        Me.rptTakeOFfService.DataBind()
    End Sub


    Protected Sub rptTakeOFfService_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTakeOFfService.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim TakeOffServiceID As String = e.Item.DataItem("TakeOffServiceID")


            Dim gvDocuments As GridView = e.Item.FindControl("gvDocuments")
            Dim dtdocuments As DataTable = DB.GetDataTable("Select * from TakeOffServiceDocument where TakeOffServiceID = " & TakeOffServiceID)
            If dtdocuments.Rows.Count > 0 Then
                gvDocuments.DataSource = dtdocuments
                AddHandler gvDocuments.RowDataBound, AddressOf gvDocuments_RowDataBound
                gvDocuments.DataBind()
            End If

        End If

    End Sub
    Protected Sub gvDocuments_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs)
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If

        Dim LnkDocument As HyperLink = e.Row.FindControl("LnkDocument")
        LnkDocument.NavigateUrl = "/assets/NCPDocuments/" & e.Row.DataItem("FileName").ToString
    End Sub



    Protected Sub btndashboard_Click(sender As Object, e As System.EventArgs) Handles btndashboard.Click
        Response.Redirect("/builder/default.aspx")
    End Sub
End Class
