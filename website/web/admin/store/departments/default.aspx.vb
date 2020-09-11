Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Public Class _default
    Inherits AdminPage

    Dim DepartmentId As Integer
    Dim SourceId As Integer
    Dim TargetId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("STORE")

        Dim sDepartments As String = ""
        Dim sSourceDepartments As String = ""
        Dim sTargetDepartments As String = ""

        F.EnableViewState = False
        T.EnableViewState = False
        S.EnableViewState = False

        If Not Page.IsPostBack Then
            DepartmentId = CType(Request.QueryString("DepartmentId"), Integer)
            SourceId = CType(Request.QueryString("SourceId"), Integer)
            TargetId = CType(Request.QueryString("TargetId"), Integer)
        Else
            DepartmentId = CType(Request.Form("DepartmentId"), Integer)
            SourceId = CType(Request.Form("SourceId"), Integer)
            TargetId = CType(Request.Form("TargetId"), Integer)
        End If

        'Define javascript
        btnAdd.Attributes("onClick") = "return Add();"
        btnRename.Attributes("onClick") = "return Rename();"
        btnDelete.Attributes("onClick") = "return Delete();"
        btnMove.Attributes("onClick") = "return Move();"

        ' GET ALL PARENTS FOR DEPARTMENT
        If DepartmentId <> 0 Then sDepartments = StoreDepartmentRow.GetDepartmentsToKeepOpened(DB, DepartmentId)

        ' GET ALL PARENTS FOR SOURCE DEPARTMENT
        If SourceId <> 0 Then sSourceDepartments = StoreDepartmentRow.GetDepartmentsToKeepOpened(DB, SourceId)

        ' GET ALL PARENTS FOR TARGET DEPARTMENT
        If TargetId <> 0 Then sTargetDepartments = StoreDepartmentRow.GetDepartmentsToKeepOpened(DB, TargetId)

        F.Text = StoreDepartmentRow.GetDepartments(DB, "F", "/admin/store/departments/edit.aspx", sDepartments, "DepartmentId", DepartmentId, False, True)
        S.Text = StoreDepartmentRow.GetDepartments(DB, "S", "", sSourceDepartments, "SourceId", SourceId, True, True)
        T.Text = StoreDepartmentRow.GetDepartments(DB, "T", "", sTargetDepartments, "TargetId", TargetId, False, False)
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            If DepartmentId = 0 Then
                Throw New ApplicationException("Parent Department has not been selected")
            End If
            If NewDepartment.Text = String.Empty Then
                Throw New ApplicationException("Department Name cannot be empty")
            End If
            DB.BeginTransaction()
            DepartmentId = StoreDepartmentRow.DepartmentInsert(DB, DepartmentId, NewDepartment.Text)

            'Invalidate cached menu
            Context.Cache.Remove("MenuCache")

            DB.CommitTransaction()

            Response.Redirect("default.aspx?DepartmentId=" & DepartmentId)

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        Catch ex As ApplicationException
            AddError(ex.Message)
        Finally
        End Try
    End Sub

    Private Sub btnRename_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRename.Click
        Try
            If DepartmentId = 0 Then
                Throw New ApplicationException("Department to rename has not been selected")
            End If
            If NAME.Text = String.Empty Then
                Throw New ApplicationException("New Name for the department cannot be empty")
            End If

            DB.BeginTransaction()
            SQL = "exec sp_StoreDepartmentRename " & DB.Quote(DepartmentId) & "," & DB.Quote(NAME.Text)
            DB.ExecuteSQL(SQL)

            'Invalidate cached menu
            Context.Cache.Remove("MenuCache")

            DB.CommitTransaction()

            Response.Redirect("default.aspx?DepartmentId=" & DepartmentId)

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        Catch ex As ApplicationException
            AddError(ex.Message)
        Finally
        End Try

    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            If DepartmentId = 0 Then
                Throw New Exception("Department to delete has not been selected")
            End If

            Dim dbDepartment As DataLayer.StoreDepartmentRow = DataLayer.StoreDepartmentRow.GetRow(DB, DepartmentId)
            If dbDepartment.IsInactive Then
                Throw New ApplicationException("Inactive root department cannot be deleted")
            End If

            ' GET PARENT_ID
            Dim iParent As Integer = dbDepartment.ParentId
            Dim iLft As Integer = dbDepartment.Lft

            If iLft = 1 Then
                Throw New ApplicationException("Main department cannot be deleted")
            End If

            DB.BeginTransaction()

            SQL = "exec sp_StoreDepartmentDelete " & DB.Quote(DepartmentId)
            DB.ExecuteSQL(SQL)

            'Invalidate cached menu
            Context.Cache.Remove("MenuCache")

            DB.CommitTransaction()

            Response.Redirect("default.aspx?DepartmentId=" & iParent)

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        Catch ex As ApplicationException
            AddError(ex.Message)
        Finally
        End Try
    End Sub

    Private Sub btnMove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMove.Click
        Try
            If SourceId = 0 Then
                Throw New Exception("Please select the Department to Move")
            End If
            If TargetId = 0 Then
                Throw New Exception("Please select the Destination Department")
            End If

            DB.BeginTransaction()

            SQL = "exec sp_StoreDepartmentMove " & Db.Quote(SourceId) & "," & Db.Quote(TargetId)
            DB.ExecuteSQL(SQL)

            'Invalidate cached menu
            Context.Cache.Remove("MenuCache")

            DB.CommitTransaction()

            Response.Redirect("default.aspx?SourceId=" & SourceId)

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        Catch ex As ApplicationException
            AddError(ex.Message)
        Finally
        End Try
    End Sub
End Class