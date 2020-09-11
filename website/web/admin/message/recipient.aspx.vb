Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Protected m_AdminMessageId As Integer = 0

    Private Sub LoadSupplyPhases()
        Me.F_SupplyPhase.DataSource = DataLayer.SupplyPhaseRow.GetList(Me.DB)
        Me.F_SupplyPhase.DataValueField = "SupplyPhaseID"
        Me.F_SupplyPhase.DataTextField = "SupplyPhase"
        Me.F_SupplyPhase.DataBind()
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load
        CheckAccess("MESSAGES")


        m_AdminMessageId = Request("AdminMessageID")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_CompanyName.Text = Request("F_CompanyName")
            F_FirstName.Text = Request("F_FirstName")
            F_LastName.Text = Request("F_LastName")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = " CompanyName "

            LoadSupplyPhases()
        End If

        If gvList.Rows.Count > 0 Then
            btnClearRecipients.Visible = True
            btnAddRecipients.Visible = True
        Else

            btnClearRecipients.Visible = False
            btnAddRecipients.Visible = False
        End If

    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " WHERE "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT DISTINCT "
        SQLFields &= " TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " et.CompanyName, et." & Me.F_Audience.SelectedValue & "ID , '" & Me.F_Audience.SelectedValue & "' Audience, eta.FirstName, eta.LastName, et." & Me.F_Audience.SelectedValue & "ID ID"
        SQL = " FROM " & Me.F_Audience.SelectedValue & " et "
        SQL = SQL & " LEFT JOIN (SELECT " & Me.F_Audience.SelectedValue & "ID, " & Me.F_Audience.SelectedValue & "AccountID, FirstName, LastName FROM " & Me.F_Audience.SelectedValue & "Account WHERE IsPrimary = 1) eta ON et." & Me.F_Audience.SelectedValue & "ID = eta." & Me.F_Audience.SelectedValue & "ID "

        If Me.F_Audience.SelectedValue = "Vendor" Then
            SQL = SQL & " LEFT JOIN " & Me.F_Audience.SelectedValue & "Registration etr ON et." & Me.F_Audience.SelectedValue & "ID = etr." & Me.F_Audience.SelectedValue & "ID " & vbCrLf
            SQL = SQL & " LEFT JOIN SupplyPhase spp ON etr.PrimarySupplyPhaseID = spp.SupplyPhaseID " & vbCrLf
            SQL = SQL & " LEFT JOIN SupplyPhase sps ON etr.SecondarySupplyPhaseID = sps.SupplyPhaseID " & vbCrLf
            If Me.F_LLC.Text <> "" Then
                SQL = SQL & " JOIN LLCVendor lv ON et." & Me.F_Audience.SelectedValue & "ID = lv." & Me.F_Audience.SelectedValue & "ID"
                SQL = SQL & Conn & " lv.LLCID = " & Me.F_LLC.Value.ToString
                Conn = " AND "
            End If
            If Me.F_SupplyPhase.Text <> "" Then
                SQL = SQL & Conn & " (etr.PrimarySupplyPhaseID IN (" & SupplyPhases() & ") OR etr.SecondarySupplyPhaseID IN (" & SupplyPhases() & "))"
                Conn = " AND "
            End If
        ElseIf Me.F_Audience.SelectedValue = "Builder" And (Me.F_SupplyPhase.Text <> "" Or Me.F_LLC.Text <> "") Then
            SQL = SQL & " JOIN LLC llc ON et.LLCID = llc.LLCID" & vbCrLf
            If Me.F_SupplyPhase.Text <> "" Then
                SQL = SQL & " JOIN LLCSupplyPhases lsp ON llc.LLCID = lsp.LLCID" & vbCrLf
                SQL = SQL & Conn & " lsp.SupplyPhaseID IN (" & SupplyPhases() & ")"
                Conn = " AND "
            End If
            If Me.F_LLC.Text <> "" Then
                SQL = SQL & Conn & " llc.LLCID = " & Me.F_LLC.Value.ToString
                Conn = " AND "
            End If
        End If

        If Not F_FirstName.Text = String.Empty Or Not F_LastName.Text = String.Empty Then

            If Me.F_FirstName.Text <> "" Then
                SQL = SQL & Conn & " eta.FirstName LIKE " & DB.FilterQuote(Me.F_FirstName.Text) & vbCrLf
                Conn = " AND "
            End If

            If Me.F_LastName.Text <> "" Then
                SQL = SQL & Conn & " eta.LastName LIKE " & DB.FilterQuote(Me.F_LastName.Text) & vbCrLf
                Conn = " AND "
            End If

        End If

        If Not F_CompanyName.Text = String.Empty Then
            SQL = SQL & Conn & " et.CompanyName LIKE " & DB.FilterQuote(F_CompanyName.Text) & " "
            Conn = " AND "
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY et." & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()

        If res.Rows.Count > 0 Then
            btnClearRecipients.Visible = True
            btnAddRecipients.Visible = True
        Else
            btnClearRecipients.Visible = False
            btnAddRecipients.Visible = False
        End If
    End Sub

    Public Function SupplyPhases() As String

        Dim str, d As String
        d = String.Empty
        str = String.Empty
        For Each li As ListItem In Me.F_SupplyPhase.Items
            If li.Selected Then
                str += d + li.Value
                d = ","
            End If
        Next
        Return str

    End Function

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

            If isChecked Then
                Audience = row.Cells(2).Text
                ID = CType(row.Cells(1).Text, Integer)

                SQL = "INSERT INTO AdminMessage" & Audience & "Recipient(AdminMessageID, " & Audience & "ID, IsActive) VALUES (" & m_AdminMessageId.ToString & "," & ID & ",1)"
                Me.DB.ExecuteSQL(SQL)

            End If
        Next

        Response.Redirect("default.aspx")

    End Sub

    Protected Sub btnClearRecipients_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClearRecipients.Click

        Dim i As Integer = 0
        Dim row As GridViewRow

        For i = 0 To gvList.Rows.Count - 1
            row = gvList.Rows(i)
            CType(row.FindControl("chkSelect"), CheckBox).Checked = False
        Next

    End Sub

    'Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
    '    Try

    '        If e.Row IsNot Nothing Then
    '            If e.Row.Cells(1) IsNot Nothing Then
    '                e.Row.Cells(1).Visible = False
    '            End If
    '        End If

    '    Catch ex As Exception

    '    End Try
    'End Sub

End Class
