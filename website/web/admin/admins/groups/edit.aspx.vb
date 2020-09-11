Imports Components
Imports DataLayer
Imports System.Data
Imports System.Data.SqlClient

Partial Class edit
    Inherits AdminPage

    Private GroupId As Integer
    Private dbAdminGroup As AdminGroupRow
   

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("USERS")

        GroupId = Convert.ToInt32(Request("GroupId"))
        If Not IsPostBack Then
            If GroupId = 0 Then
                Delete.Visible = False
            Else
                LoadFromDB()
            End If
            BindPrivileges()
        End If
    End Sub

    Private Sub LoadFromDB()
        dbAdminGroup = AdminGroupRow.GetRow(DB, GroupId)
        DESCRIPTION.Text = dbAdminGroup.Description
    End Sub

    Private Sub BindPrivileges()
        'Bind Left List
        Dim dtLeft As DataTable = AdminAccessRow.LoadSectionsWithoutPrivileges(DB, GroupId)
        lbLeft.DataSource = dtLeft.DefaultView
        lbLeft.DataValueField = "SectionId"
        lbLeft.DataTextField = "Description"
        lbLeft.DataBind()

        'This stores the excluded privillege rights
        Dim lbLeftListItem As String = String.Empty
        Dim leftcount As Integer = 0
        While leftcount < lbLeft.Items.Count
            lbLeftListItem &= lbLeft.Items(leftcount).Text
            leftcount += 1
        End While
        ViewState("LeftListItems") = lbLeftListItem

        'Bind Right List
        If GroupId <> 0 Then
            Dim dtRight As DataTable = AdminAccessRow.LoadSectionsWithPrivileges(DB, GroupId)
            lbRight.DataSource = dtRight.DefaultView
            lbRight.DataValueField = "SectionId"
            lbRight.DataTextField = "Description"
            lbRight.DataBind()
        End If
    End Sub

    Private Sub Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Save.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            If GroupId <> 0 Then
                Dim dbAdminGroup As AdminGroupRow = AdminGroupRow.GetRow(DB, GroupId)
                dbAdminGroup.Description = DESCRIPTION.Text
                dbAdminGroup.Update()

                'This is used to get the current excluded privillege rights
                Dim lbLeftNewListItem As String = String.Empty
                Dim leftcount As Integer = 0
                While leftcount < lbLeft.Items.Count
                    lbLeftNewListItem &= lbLeft.Items(leftcount).Text
                    leftcount += 1
                End While

                'This record should be only added when an update is done to the user rights
                If Not lbLeftNewListItem = ViewState("LeftListItems") Then
                    Core.LogEvent("Privilleges were updated for group """ & dbAdminGroup.Description & """ by user """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
                End If
            Else
                Dim dbAdminGroup As AdminGroupRow = New AdminGroupRow(DB)
                dbAdminGroup.Description = DESCRIPTION.Text
                GroupId = dbAdminGroup.AutoInsert()
                Core.LogEvent("New Admin Group """ & dbAdminGroup.Description & """ was created by user """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
            End If

            'DELETE ALL PRIVILEGES
            AdminAccessRow.RemoveByGroup(DB, GroupId)

            'INSERT ONLY SELECTED PRIVILEGES
            For Each item As ListItem In lbRight.Items
                Dim dbAdminAccess As AdminAccessRow = New AdminAccessRow(DB)

                dbAdminAccess.GroupId = GroupId
                dbAdminAccess.SectionId = CInt(item.Value)
                dbAdminAccess.Insert()
            Next
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        Finally
            If Not DB Is Nothing Then DB.Close()
        End Try
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub Delete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Delete.Click
        If GroupId <> 0 Then
            Response.Redirect("delete.aspx?GroupId=" & GroupId & "&" & GetPageParams(FilterFieldType.All))
        End If
    End Sub

    Private Sub btnRight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRight.Click
        Dim item As ListItem
        Dim i As Integer = 0

        While i < lbLeft.Items.Count
            If lbLeft.Items(i).Selected Then
                item = lbLeft.Items(i)
                item.Selected = False
                Dim bInserted As Boolean = False
                For j As Integer = 0 To lbRight.Items.Count - 1
                    If lbRight.Items(j).Text > item.Text Then
                        lbRight.Items.Insert(j, item)
                        bInserted = True
                        Exit For
                    End If
                Next
                If Not bInserted Then
                    lbRight.Items.Add(item)
                End If
                lbLeft.Items.RemoveAt(i)
            Else
                i += 1
            End If
        End While
    End Sub

    Private Sub btnLeft_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLeft.Click
        Dim item As ListItem
        Dim i As Integer = 0

        While i < lbRight.Items.Count
            If lbRight.Items(i).Selected Then
                item = lbRight.Items(i)
                item.Selected = False
                Dim bInserted As Boolean = False
                For j As Integer = 0 To lbLeft.Items.Count - 1
                    If lbLeft.Items(j).Text > item.Text Then
                        lbLeft.Items.Insert(j, item)
                        bInserted = True
                        Exit For
                    End If
                Next
                If Not bInserted Then
                    lbLeft.Items.Add(item)
                End If
                lbRight.Items.RemoveAt(i)
            Else
                i += 1
            End If
        End While
    End Sub

End Class
