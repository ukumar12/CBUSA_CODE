Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Protected m_AdminDocumentId As Integer = 0

    Private dtRecipients As DataTable

    Private Sub LoadSupplyPhases()
        Me.F_SupplyPhase.DataSource = DataLayer.SupplyPhaseRow.GetListByParentId(DB, SupplyPhaseRow.GetRootSupplyPhase(DB).SupplyPhaseID)
        Me.F_SupplyPhase.DataValueField = "SupplyPhaseID"
        Me.F_SupplyPhase.DataTextField = "SupplyPhase"
        Me.F_SupplyPhase.DataBind()
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load
        CheckAccess("DOCUMENTS")

        m_AdminDocumentId = Request("AdminDocumentID")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataTextField = "LLC"
            F_LLC.DataBind()

            F_CompanyName.Text = Request("F_CompanyName")
            F_LLC.SelectedValues = Request("F_LLC")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = " CompanyName "

            LoadSupplyPhases()

            dtRecipients = AdminDocumentRow.GetAllRecipients(DB, m_AdminDocumentId)
            gvList.DataSource = dtRecipients
            gvList.Pager.NofRecords = gvList.DataSource.rows.count
            gvList.DataBind()
        End If

        If gvList.Rows.Count > 0 Then
            btnAddRecipients.Visible = True
        Else
            btnAddRecipients.Visible = False
        End If

    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " WHERE "
        Dim innerSQL As String = String.Empty

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        If F_Audience.SelectedValue = "Vendor" Or F_Audience.SelectedValue = "All" Then
            If innerSQL <> String.Empty Then
                innerSQL &= " union "
            End If

            innerSQL &= "select CompanyName, VendorID as ID, 'Vendor' as Audience from Vendor"
            Conn = " where "
            If F_CompanyName.Text <> Nothing Then
                innerSQL &= Conn & " CompanyName like " & DB.FilterQuote(F_CompanyName.Text)
                Conn = " and "
            End If
            If F_LLC.SelectedValues <> Nothing Then
                innerSQL &= Conn & " VendorID in (select VendorID from LLCVendor where LLCID in " & DB.NumberMultiple(F_LLC.SelectedValues) & ")"
                Conn = " and "
            End If
            If F_SupplyPhase.SelectedValues <> String.Empty Then
                innerSQL &= Conn & " VendorID in (select VendorID from VendorSupplyPhase where SupplyPhaseId in " & DB.NumberMultiple(F_SupplyPhase.SelectedValues) & ")"
            End If
        End If

        If F_Audience.SelectedValue = "Builder" Or F_Audience.SelectedValue = "All" Then
            If innerSQL <> String.Empty Then
                innerSQL &= " union "
            End If

            innerSQL &= "select CompanyName, BuilderID as ID, 'Builder' as Audience from Builder"
            Conn = " where "
            If F_CompanyName.Text <> Nothing Then
                innerSQL &= Conn & " CompanyName like " & DB.FilterQuote(F_CompanyName.Text)
                Conn = " and "
            End If
            If F_LLC.SelectedValues <> Nothing Then
                innerSQL &= Conn & " LLCID in " & DB.NumberMultiple(F_LLC.SelectedValues)
                Conn = " and "
            End If
        End If

        If F_Audience.SelectedValue = "PIQ" Or F_Audience.SelectedValue = "All" Then
            If innerSQL <> String.Empty Then
                innerSQL &= " union "
            End If

            innerSQL &= "select CompanyName, PIQID as ID, 'PIQ' as Audience from PIQ"
            Conn = " where "
            If F_CompanyName.Text <> String.Empty Then
                innerSQL &= Conn & " CompanyName like " & DB.FilterQuote(F_CompanyName.Text)
                Conn = " and "
            End If
        End If

        SQLFields = "select CompanyName, ID, Audience"
        SQL = " from (" & innerSQL & ") as temp"

        'gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        dtRecipients = AdminDocumentRow.GetAllRecipients(DB, m_AdminDocumentId)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()

        If res.Rows.Count > 0 Then
            btnAddRecipients.Visible = True
        Else
            btnAddRecipients.Visible = False
        End If
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Protected Sub btnAddRecipients_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddRecipients.Click

        Dim i As Integer = 0
        Dim row As GridViewRow
        Dim isChecked As Boolean = False
        Dim Audience As String
        Dim ID As Integer = 0
        Dim SQL As String

        For i = 0 To gvList.Rows.Count - 1
            row = gvList.Rows(i)
            isChecked = CType(row.FindControl("chkSelect"), CheckBox).Checked

            Audience = row.Cells(2).Text
            ID = CType(row.Cells(1).Text, Integer)

            If isChecked Then
                SQL = "INSERT INTO AdminDocument" & Audience & "Recipient(AdminDocumentID, " & Audience & "ID) VALUES (" & m_AdminDocumentId.ToString & "," & ID & ")"
                Try
                    Me.DB.ExecuteSQL(SQL)
                Catch ex As SqlClient.SqlException
                End Try
            Else
                SQL = "DELETE FROM AdminDocument" & Audience & "Recipient where " & Audience & "ID=" & DB.Number(ID)
                DB.ExecuteSQL(SQL)
            End If
        Next

        Response.Redirect("default.aspx")

    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        If dtRecipients.Select("ID=" & e.Row.DataItem("ID") & " and Audience=" & DB.Quote(e.Row.DataItem("Audience"))).Length > 0 Then
            Dim cb As CheckBox = e.Row.FindControl("chkSelect")
            cb.Checked = True
        End If

        If e.Row.DataItem("Audience") = "Vendor" Then
            Dim vendorid As String = e.Row.DataItem("ID")
            Dim LLCName As Literal = e.Row.FindControl("LLCName")
            LLCName.Text = ListLLC(vendorid)
        ElseIf e.Row.DataItem("Audience") = "Builder" Then
            Dim BuilderID As String = e.Row.DataItem("ID")
            Dim LLCName As Literal = e.Row.FindControl("LLCName")
            LLCName.Text = LLCRow.GetBuilderLLC(DB, BuilderID).LLC
        End If

       

    End Sub
    Private Function ListLLC(ByVal ID As String) As String
        Dim dtLLCPricing As DataTable = VendorRow.GetLLCList(DB, ID)
        Dim strLLCPricing As String = String.Empty
        For Each row As DataRow In dtLLCPricing.Rows
            strLLCPricing &= row("LLC") & " ,"
        Next
        If strLLCPricing <> String.Empty AndAlso strLLCPricing.EndsWith(" ,") Then strLLCPricing = strLLCPricing.Substring(0, strLLCPricing.Length - 1)
        Return strLLCPricing
    End Function
    Protected Sub btnClearAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClearAll.Click
        AdminDocumentRow.ClearAllRecipients(DB, m_AdminDocumentId)
        BindList()
    End Sub
End Class
