 
Imports Components
Imports DataLayer
Imports Controls

Partial Class modules_Builders_NCPDocuments
    Inherits ModuleControl

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If IsAdminDisplay Then
            Exit Sub
        End If

        Dim contractid As Integer = Convert.ToInt32(Session("ContractID"))
        Dim builderllcid As Integer = BuilderRow.GetRow(DB, Session("BuilderID")).LLCID

        If contractid > 0 And builderllcid > 0 Then
            BindList(DB, contractid, builderllcid)
        End If
    End Sub

    Private Sub BindList(ByVal DB As Database, ByVal ContractID As Integer, ByVal LLCID As Integer)
        Dim Sql As String = String.Empty
        Sql &= "select ncpc.*, ncpl.LLCID , ncpcc .ContractID  from NCPContent ncpc inner join NCPContentLLC ncpl " & _
                "on ncpc .NCPContentID = ncpl .NCPContentID inner join NCPContentContract ncpcc on ncpc .NCPContentID = ncpcc .NCPContentID " & _
                " where ncpcc.ContractID = " & ContractID & " AND ncpc.Isactive = 1 AND ncpl.LLCID = " & LLCID
        Dim dt As DataTable = DB.GetDataTable(Sql)
        Me.rptVendors.DataSource = dt
        Me.rptVendors.DataBind()
    End Sub

    
    Protected Sub rptVendors_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptVendors.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim ContentID As String = e.Item.DataItem("NCPContentID")

          
            Dim gvDocuments As GridView = e.Item.FindControl("gvDocuments")
            Dim dtdocuments As DataTable = DB.GetDataTable("Select * from NCPDocument where NCPContentID = " & ContentID)
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
