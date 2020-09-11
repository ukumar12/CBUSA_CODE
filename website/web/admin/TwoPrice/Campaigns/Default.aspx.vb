Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.IO

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("TWO_PRICE_CAMPAIGNS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_Name.Text = Request("F_Name")
            F_StartDateLbound.Text = Request("F_StartDateLBound")
            F_StartDateUbound.Text = Request("F_StartDateUBound")
            F_EndDateLbound.Text = Request("F_EndDateLBound")
            F_EndDateUbound.Text = Request("F_EndDateUBound")
            F_CreateDateLbound.Text = Request("F_CreateDateLBound")
            F_CreateDateUbound.Text = Request("F_CreateDateUBound")

            F_Status.DataSource = TwoPriceStatusRow.GetList(DB, "SortOrder")
            F_Status.DataValueField = "Value"
            F_Status.DataTextField = "Name"
            F_Status.DataBind()
            F_Status.Items.Insert(0, New ListItem("", ""))


            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "TwoPriceCampaignId"
                gvList.SortOrder = "DESC"
            End If
            BindCpDropdownList()
            BindList()
        End If
    End Sub
    Private Sub BindCpDropdownList()
        Dim dtCpEvent As DataTable
        dtCpEvent = DB.GetDataTable("select distinct TwoPriceCampaignId,Name from TwoPriceCampaign where IsActive = 1 order by Name")

        Dim ddlCpEvent As DropDownList = frmSelectCPEvent.FindControl("ddlCpEvent")
        ddlCpEvent.DataSource = dtCpEvent
        ddlCpEvent.DataTextField = "Name"
        ddlCpEvent.DataValueField = "TwoPriceCampaignId"
        ddlCpEvent.DataBind()


    End Sub
    Protected Sub btnNavExistingCP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNavExistingCP.Click
        'Dim SelectedValue As Int32
        Dim ddlCpEvent As DropDownList = frmSelectCPEvent.FindControl("ddlCpEvent")
        'SelectedValue = ddlCpEvent.SelectedValue
        Dim NewTwoPriceCampaignId As Int32 = CloneCPEvent(ddlCpEvent.SelectedValue)
        If (NewTwoPriceCampaignId <> 0) Then
            Response.Redirect("edit.aspx?TwoPriceCampaignId=" & NewTwoPriceCampaignId & "")
        Else
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "Notification", "alert('Files Not Found for Copy')", True)
        End If

    End Sub
    Public Function CloneCPEvent(ByVal TwoPriceCampaignId As Int32) As Integer
        Try
            DB.BeginTransaction()
            Dim dbTwoPriceCampaignExisting As TwoPriceCampaignRow = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)
            Dim dbTwoPriceCampaignBuilder As TwoPriceCampaignRow = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)

            Dim dbTwoPriceCampaign = New TwoPriceCampaignRow(DB)
            dbTwoPriceCampaign.Name = "Copy Of - " & dbTwoPriceCampaignExisting.Name
            dbTwoPriceCampaign.Status = "New"
            dbTwoPriceCampaign.StartDate = dbTwoPriceCampaignExisting.StartDate
            dbTwoPriceCampaign.EndDate = dbTwoPriceCampaignExisting.EndDate
            dbTwoPriceCampaign.IsActive = dbTwoPriceCampaignExisting.IsActive
            Dim TwoPriceCampaignIdNew As Int32 = dbTwoPriceCampaign.Insert
            dbTwoPriceCampaign.DeleteFromAllLLCs()
            dbTwoPriceCampaign.InsertToLLCs(dbTwoPriceCampaignExisting.GetSelectedLLCs)

            dbTwoPriceCampaign.DeleteFromAllCampaignBuilders()
            dbTwoPriceCampaign.InsertToCampaignBuilders(dbTwoPriceCampaignBuilder.GetSelectedCampaignBuilders)
            dbTwoPriceCampaign.DeleteFromAllCampaignVendors()
            dbTwoPriceCampaign.InsertToCampaignVendors(dbTwoPriceCampaignBuilder.GetSelectedCampaignVendors)


            Dim dbTwoPriceTakeOff As TwoPriceTakeOffRow = TwoPriceTakeOffRow.GetRowByTwoPriceCampaignId(DB, TwoPriceCampaignId)
            Dim dbTwoPriceTakeOffProduct As DataTable = TwoPriceTakeOffRow.GetTwoPriceTakeOffProducts(DB, dbTwoPriceTakeOff.TwoPriceTakeOffID)
            'dbTwoPriceTakeOffProduct.
            Dim dbTwoPriceTakeOff_Copy As TwoPriceTakeOffRow = TwoPriceTakeOffRow.GetRowByTwoPriceCampaignId(DB, TwoPriceCampaignIdNew)
            If dbTwoPriceTakeOff_Copy Is Nothing OrElse dbTwoPriceTakeOff_Copy.TwoPriceTakeOffID = Nothing Then
                dbTwoPriceTakeOff_Copy = New TwoPriceTakeOffRow(DB)
                dbTwoPriceTakeOff_Copy.TwoPriceCampaignId = TwoPriceCampaignIdNew
                dbTwoPriceTakeOff_Copy.Insert()
            End If
            For Each drTakeOff As DataRow In dbTwoPriceTakeOffProduct.Rows



                'Session("CurrentTwoPriceTakeOffId") = 
                'If (dbTwoPriceTakeOffProduct.Quantity = 0) Then
                '    dbTwoPriceTakeOff_Copy.Remove()
                'Else
                Dim dbTwoPriceTakeOffProduct_Copy As TwoPriceTakeOffProductRow = TwoPriceTakeOffProductRow.GetRow(DB, dbTwoPriceTakeOff_Copy.TwoPriceTakeOffID)
                dbTwoPriceTakeOffProduct_Copy.TwoPriceTakeOffID = dbTwoPriceTakeOff_Copy.TwoPriceTakeOffID
                dbTwoPriceTakeOffProduct_Copy.ProductID = drTakeOff("ProductID")
                dbTwoPriceTakeOffProduct_Copy.Quantity = drTakeOff("Quantity")
                dbTwoPriceTakeOffProduct_Copy.Insert()

                'End If

                'Dim dbTwoPriceTakeOffProduct_Copy As TwoPriceTakeOffProductRow = TwoPriceTakeOffProductRow.GetRow(DB, dbTwoPriceTakeOff.TwoPriceTakeOffID)

            Next

            Dim dtTwoPriceCampaignDoc As DataTable = DB.GetDataTable("select * from TwoPriceDocument where TwoPriceCampaignId = " & TwoPriceCampaignId & "")
            For Each drDoc As DataRow In dtTwoPriceCampaignDoc.Rows
                Dim destFolder As String = AppSettings("TwopriceDocument")
                Dim MapPath As String = destFolder & drDoc("FileName")
                Dim cleanName As String = drDoc("FileName").Substring(0, InStrRev(drDoc("FileName"), ".") - 1)
                Dim cleanExt As String = drDoc("FileName").Substring(InStrRev(drDoc("FileName"), "."))
                Dim UploadFilename As String = ""
                Dim f As HttpPostedFile
                Dim fi As New FileInfo(MapPath)
                Dim n As Integer = 0
                If fi.Exists Then
                    While fi.Exists
                        n += 1
                        fi = New FileInfo(destFolder & cleanName & n & "." & cleanExt)
                    End While
                End If

                If n > 0 Then
                    MapPath = destFolder & cleanName & n & "." & cleanExt
                    UploadFilename = cleanName & n & "." & cleanExt
                End If

                If File.Exists(MapPath) Then File.Delete(MapPath)
                'f.SaveAs(MapPath)
                File.Copy(destFolder & drDoc("FileName"), MapPath)


                Dim dbDocument As TwoPriceDocumentRow = New TwoPriceDocumentRow(DB)
                dbDocument.GUID = Guid.NewGuid.ToString
                dbDocument.Title = UploadFilename
                dbDocument.Uploaded = Now
                dbDocument.FileName = UploadFilename
                dbDocument.TwoPriceCampaignId = TwoPriceCampaignIdNew
                dbDocument.Insert()

            Next
            DB.CommitTransaction()

            Return TwoPriceCampaignIdNew
        Catch ex As Exception
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            'Response.Write(ex.Message)
            AddError(ErrHandler.ErrorText(ex))
            Return 0
        End Try

    End Function

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & "  tpc.TwoPriceCampaignId  , tpc.Name  , tpc.Status  , tpc.StartDate  , tpc.EndDate  , tpc.CreateDate  , tpc.IsActive  , isnull(tpc.AwardedVendorId,0) AwardedVendorId , AllowPriceUpdate , tmp.Name StatusName, ISNULL(tpbi.InvitationStatus,0) InvitationStatus , "
        SQLFields = SQLFields & " convert(varchar,isnull(EndDate,getdate()+2),101) EndDateFormatted, "
        SQLFields = SQLFields & " convert(varchar,isnull(ResponseDeadline,getdate()+2),101) ResponseDeadline "
        SQL = " FROM TwoPriceCampaign  tpc "
        SQL = SQL & " INNER JOIN TwoPriceStatus            tmp       ON  tpc.Status              =       tmp.Value "
        SQL = SQL & " LEFT JOIN  TwoPriceBuilderInvitation tpbi      ON  tpc.TwoPriceCampaignId  =       tpbi.TwoPriceCampaignId "

        If Not F_Name.Text = String.Empty Then
            SQL = SQL & Conn & "tpc.Name LIKE " & DB.FilterQuote(F_Name.Text)
            Conn = " AND "
        End If
        If Not F_Status.Text = String.Empty Then
            SQL = SQL & Conn & "tpc.Status LIKE " & DB.FilterQuote(F_Status.Text)
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "tpc.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_StartDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "tpc.StartDate >= " & DB.Quote(F_StartDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_StartDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "tpc.StartDate < " & DB.Quote(DateAdd("d", 1, F_StartDateUbound.Text))
            Conn = " AND "
        End If
        If Not F_EndDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "tpc.EndDate >= " & DB.Quote(F_EndDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_EndDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "tpc.EndDate < " & DB.Quote(DateAdd("d", 1, F_EndDateUbound.Text))
            Conn = " AND "
        End If
        If Not F_CreateDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "tpc.CreateDate >= " & DB.Quote(F_CreateDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_CreateDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "tpc.CreateDate < " & DB.Quote(DateAdd("d", 1, F_CreateDateUbound.Text))
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & "ORDER BY tpc." & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Protected Sub RedirectToPostDeadlineEnrollment(ByVal sender As Object, ByVal e As EventArgs)
        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)
        Dim TwoPriceCampaignId As Integer = CType(sender, LinkButton).CommandArgument

        Response.Redirect("/admin/twoprice/campaigns/PostDeadlineEnrollment.aspx?TwoPriceCampaignId=" + TwoPriceCampaignId.ToString())


    End Sub
End Class