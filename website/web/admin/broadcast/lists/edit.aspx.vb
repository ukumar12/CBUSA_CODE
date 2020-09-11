Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports System.IO

Public Class Edit
    Inherits AdminPage

    Protected ListId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BROADCAST")

        Dim mTimespan As New System.TimeSpan(0, 10, 0)
        Dim config As System.Configuration.Configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/web.config")
        Dim m_RuntimeSection As System.Web.Configuration.HttpRuntimeSection = CType(config.GetSection("system.web/httpRuntime"), System.Web.Configuration.HttpRuntimeSection)
        m_RuntimeSection.MaxRequestLength = 8196
        m_RuntimeSection.ExecutionTimeout = mTimespan

        ListId = Convert.ToInt32(Request("ListId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If ListId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If
        Dim dbMailingList As MailingListRow = MailingListRow.GetRow(DB, ListId)
        txtName.Text = dbMailingList.Name
        drpStatus.SelectedValue = dbMailingList.Status
        fuFilename.CurrentFileName = dbMailingList.Filename
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            Dim dbMailingList As MailingListRow

            If ListId <> 0 Then
                dbMailingList = MailingListRow.GetRow(DB, ListId)
            Else
                dbMailingList = New MailingListRow(DB)
            End If

            If fuFilename.NewFileName <> String.Empty Then
                If fuFilename.CurrentFileName <> String.Empty Then
                    fuFilename.RemoveOldFile()
                End If
                fuFilename.SaveNewFile()
                dbMailingList.Filename = fuFilename.NewFileName
            End If

            dbMailingList.Name = txtName.Text
            dbMailingList.Status = drpStatus.SelectedValue

            If ListId <> 0 Then
                dbMailingList.ModifyAdminId = Me.LoggedInAdminId
                dbMailingList.Update()
            Else
                dbMailingList.CreateAdminId = Me.LoggedInAdminId
                dbMailingList.ModifyAdminId = Me.LoggedInAdminId
                ListId = dbMailingList.Insert
            End If

            DB.CommitTransaction()

            ImportMembers(dbMailingList.Filename, dbMailingList.ListId)

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?ListId=" & ListId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub ImportMembers(ByVal FileName As String, ByVal ListId As Integer)
        Dim aLine As String()

        If Not File.Exists(Server.MapPath(fuFilename.Folder & FileName)) Then
            Throw New Exception("Cannot find the file to process")
        End If

        Dim f As StreamReader = New StreamReader(Server.MapPath(fuFilename.Folder & FileName))
        While Not f.EndOfStream
            Dim sLine As String = f.ReadLine()

            Dim bInside As Boolean = False
            For iLoop As Integer = 1 To Len(sLine)
                If Mid(sLine, iLoop, 1) = """" Then
                    If bInside = False Then
                        bInside = True
                    Else
                        bInside = False
                    End If
                End If
                If Mid(sLine, iLoop, 1) = "," Then
                    If Not bInside Then
                        sLine = Left(sLine, iLoop - 1) & "|" & Mid(sLine, iLoop + 1, Len(sLine) - iLoop)
                    End If
                End If
            Next

            aLine = sLine.Split("|")
            If aLine.Length = 3 AndAlso aLine(1) <> String.Empty Then
                Dim Name As String = Trim(Core.StripDblQuote(aLine(0)))
                Dim Email As String = Trim(Core.StripDblQuote(aLine(1)))
                Dim MimeType As String = UCase(Trim(Core.StripDblQuote(aLine(2))))
                If MimeType <> "HTML" And MimeType <> "TEXT" Then
                    MimeType = "HTML"
                End If
                Dim mm As MailingMemberRow = MailingMemberRow.GetRowByEmail(DB, Email)
                If mm.Unsubscribe = Nothing Then
                    If mm.MemberId = Nothing Then
                        mm.Email = Email
                        mm.Name = Name
                        mm.MimeType = MimeType
                        mm.Status = "ACTIVE"
                        mm.Insert()
                    Else
                        mm.Name = Name
                        mm.MimeType = MimeType
                        mm.Status = "ACTIVE"
                        mm.Update()
                    End If
                    On Error Resume Next
                    mm.InsertToList(ListId)
                    On Error GoTo 0
                End If
            End If
        End While
        f.Close()

    End Sub
End Class
