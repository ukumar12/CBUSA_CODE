Imports Components
Imports DataLayer

Partial Class CssMenu
    Inherits BaseControl

    Protected Function GenerateDepartmentMenu(ByVal DepartmentId As Integer) As String
        Dim sb As New StringBuilder
        Dim CloseTags As New Hashtable, i As Integer
        Dim GlobalRefererName As String = System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName")

        Dim dt As DataTable = StoreDepartmentRow.GetMenuDepartments(DB, DepartmentId)
        For Each row As DataRow In dt.Rows
            Dim LftClose As Integer = IIf(IsDBNull(row("LftClose")), 0, row("LftClose"))
            If LftClose > 0 Then
                If CloseTags(Trim(LftClose)) Is Nothing Then CloseTags(Trim(LftClose)) = 0
                CloseTags(Trim(LftClose)) += 1
            End If
            Dim CustomURL As String = IIf(IsDBNull(row("CustomURL")), String.Empty, row("CustomURL"))
            If CustomURL = String.Empty Then
                Dim PageName As String = "default.aspx"
                Dim ParentId As Integer = IIf(IsDBNull(row("ParentId")), 0, row("ParentId"))
                If ParentId = StoreDepartmentRow.GetDefaultDepartmentId(DB) Then
                    PageName = "main.aspx"
                End If
                sb.Append("<a href=""" & GlobalRefererName & "/store/" & PageName & "?DepartmentId=" & row("DepartmentId") & """>" & Server.HtmlEncode(row("Name")) & "</a>" & vbCrLf)
            Else
                sb.Append("<a href=""" & GlobalRefererName & CustomURL & """>" & Server.HtmlEncode(row("Name")) & "</a>" & vbCrLf)
            End If
            If row("HasChildren") Then
                sb.Append("<div>" & vbCrLf)
            End If
            If Not CloseTags(Trim(row("Lft"))) Is Nothing Then
                For i = 1 To CloseTags(Trim(row("Lft")))
                    sb.Append("</div>" & vbCrLf)
                Next
            End If
        Next
        dt = Nothing
        CloseTags = Nothing

        Return sb.ToString
    End Function

End Class
