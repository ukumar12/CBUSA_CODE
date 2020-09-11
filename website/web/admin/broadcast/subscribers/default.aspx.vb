Imports Components
Imports Controls
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports datalayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            BindDropDownList()

            F_Email.Text = Request("F_Email")
            F_Name.Text = Request("F_Name")
            F_MimeType.SelectedValue = Request("F_MimeType")
            If F_MimeType.SelectedIndex = -1 Then F_MimeType.SelectedIndex = 0
            F_ListId.SelectedValues = Request("F_ListId")
            Try
                F_JoinDate_Lower.Value = Request("F_JoinDate_Lower")
            Catch
            End Try
            Try
                F_JoinDate_Upper.Value = Request("F_JoinDate_Upper")
            Catch
            End Try

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "Email"

            BindList()
        End If
    End Sub

    Private Sub BindDropDownList()
        F_ListId.DataSource = MailingListRow.GetLists(DB)
        F_ListId.DataTextField = "name"
        F_ListId.DataValueField = "listid"
        F_ListId.DataBind()
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " and "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM MailingMember where Status = 'ACTIVE' "

        If Not F_Email.Text = String.Empty Then
            SQL = SQL & Conn & "Email LIKE " & DB.FilterQuote(F_Email.Text)
            Conn = " AND "
        End If
        If Not F_Name.Text = String.Empty Then
            SQL = SQL & Conn & "Name LIKE " & DB.FilterQuote(F_Name.Text)
            Conn = " AND "
        End If
        If Not F_MimeType.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "MimeType = " & DB.Quote(F_MimeType.SelectedValue)
            Conn = " AND "
        End If
        If Not F_JoinDate_Lower.Value = Nothing Then
            SQL = SQL & Conn & "CreateDate >= " & DB.Quote(F_JoinDate_Lower.Text)
            Conn = " AND "
        End If
        If Not F_JoinDate_Upper.Value = Nothing Then
            SQL = SQL & Conn & "CreateDate < " & DB.Quote(DateAdd("d", 1, F_JoinDate_Upper.Text))
            Conn = " AND "
        End If
        If Not F_ListId.SelectedValues = Nothing Then
            SQL = SQL & Conn & "MemberId IN (SELECT MemberId FROM MailingListMember WHERE MailingMember.MemberId = MemberId AND ListId in " & DB.NumberMultiple(F_ListId.SelectedValues) & ")"
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub ExportList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " and "

        SQLFields = "SELECT * "
        SQL = " FROM MailingMember where Status = 'ACTIVE' "

        If Not F_Email.Text = String.Empty Then
            SQL = SQL & Conn & "Email LIKE " & DB.FilterQuote(F_Email.Text)
            Conn = " AND "
        End If
        If Not F_Name.Text = String.Empty Then
            SQL = SQL & Conn & "Name LIKE " & DB.FilterQuote(F_Name.Text)
            Conn = " AND "
        End If
        If Not F_MimeType.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "MimeType = " & DB.Quote(F_MimeType.SelectedValue)
            Conn = " AND "
        End If
        If Not F_JoinDate_Lower.Value = Nothing Then
            SQL = SQL & Conn & "CreateDate >= " & DB.Quote(F_JoinDate_Lower.Text)
            Conn = " AND "
        End If
        If Not F_JoinDate_Upper.Value = Nothing Then
            SQL = SQL & Conn & "CreateDate < " & DB.Quote(DateAdd("d", 1, F_JoinDate_Upper.Text))
            Conn = " AND "
        End If
        If Not F_ListId.SelectedValues = Nothing Then
            SQL = SQL & Conn & "MemberId IN (SELECT MemberId FROM MailingListMember WHERE MailingMember.MemberId = MemberId AND ListId in " & DB.NumberMultiple(F_ListId.SelectedValues) & ")"
            Conn = " AND "
        End If
        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

        Dim Folder As String = "/assets/broadcast/excel/"
        Dim i As Integer = 0
        Dim FileName As String = Core.GenerateFileID & ".csv"

        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Subscribers Export")
        sw.WriteLine("Report generated on " & DateTime.Now.ToString("d"))
        sw.WriteLine(String.Empty)
        sw.WriteLine("Search Criteria")
        sw.WriteLine("Join Date From:," & F_JoinDate_Lower.Text)
        sw.WriteLine("Join Date To:," & F_JoinDate_Upper.Text)
        sw.WriteLine("Name:," & F_Name.Text)
        sw.WriteLine("E-mail:," & F_Email.Text)
        sw.WriteLine("Mime Type:," & F_MimeType.SelectedItem.Text)
        sw.WriteLine("List: " & F_ListId.SelectedTexts)
        sw.WriteLine(String.Empty)
        sw.WriteLine(String.Empty)

        For Each dr As DataRow In res.Rows
            Dim Name As String = IIf(IsDBNull(dr("name")), String.Empty, dr("name"))
            Dim Email As String = IIf(IsDBNull(dr("email")), String.Empty, dr("email"))
            Dim MimeType As String = IIf(IsDBNull(dr("MimeType")), String.Empty, dr("MimeType"))
            Dim JoinDate As String = dr("CreateDate")

            sw.WriteLine(Core.QuoteCSV(Name) & "," & Core.QuoteCSV(Email) & "," & Core.QuoteCSV(MimeType) & "," & Core.QuoteCSV(JoinDate))
        Next
        sw.Flush()
        sw.Close()

        divDownload.Visible = True
        lnkDownload.NavigateUrl = Folder & FileName
        gvList.Visible = False
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        divDownload.Visible = False
        gvList.Visible = True

        gvList.PageIndex = 0
        BindList()
    End Sub

    Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExport.Click
        ExportList()
    End Sub
End Class
