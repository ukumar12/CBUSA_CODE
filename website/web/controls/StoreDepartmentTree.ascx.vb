Option Explicit On

Imports System
Imports System.Data
Imports System.Collections.Specialized
Imports System.Data.SqlClient
Imports System.Text
Imports DataLayer
Imports Components

Partial Class StoreDepartmentTree
    Inherits BaseControl

    Public Property Link() As String
        Get
            Return ViewState("LINK")
        End Get
        Set(ByVal Value As String)
            ViewState("LINK") = Value
        End Set
    End Property

    Private Function GetCheckedNodes(ByVal input As TreeNode) As String
        Dim sNodes As String = ""
        Dim sConn As String = ""
        For Each node As TreeNode In input.ChildNodes
            If node.Checked Then
                sNodes = sNodes & sConn & node.Value
                sConn = ","
            End If
            Dim Checked As String = GetCheckedNodes(node)
            If Not Checked = String.Empty Then
                sNodes = sNodes & sConn & Checked
                sConn = ","
            End If
        Next
        Return sNodes
    End Function

    Public Property CheckedList() As String
        Get
            If Page.IsPostBack Then
                Return GetCheckedNodes(Tree.Nodes.Item(0))
            Else
                Return ViewState("CHECKED")
            End If
        End Get
        Set(ByVal Value As String)
            ViewState("CHECKED") = Value
        End Set
    End Property

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim SQL As String
        Dim ParentNode As TreeNode = Nothing

        If IsPostBack Then
            Exit Sub
        End If

        Dim ExpandedList As String = StoreDepartmentRow.GetDepartmentsToKeepOpened(DB, CheckedList)
        Dim aExpanded() As String = Split(ExpandedList, ",")
        Dim aChecked() As String = Split(CheckedList, ",")

        SQL = "select * from StoreDepartment order by lft"
        Dim dr As SqlDataReader = DB.GetReader(SQL)
        While dr.Read
            Dim ParentId As Integer = IIf(IsDBNull(dr("ParentId")), Nothing, dr("ParentId"))
            Dim DepartmentId As Integer = dr("DepartmentId")
            Dim DepartmentName As String = dr("NAME")
            Dim IsSpecial As Boolean = Convert.ToBoolean(dr("IsSpecial"))

            Dim tNode As TreeNode = New TreeNode
            tNode.Value = DepartmentId
            tNode.ImageUrl = "/images/folderminus.gif"
            tNode.ImageUrl = "/images/folderplus.gif"
            tNode.SelectAction = TreeNodeSelectAction.None
            tNode.ShowCheckBox = Not IsSpecial And ParentId <> 0
            If tNode.ShowCheckBox Then
                tNode.Text = DepartmentName
            Else
                tNode.Text = "&nbsp;&nbsp;" & DepartmentName
            End If

            'Keep expanded nodes
            tNode.Expanded = False
            If Array.IndexOf(aExpanded, DepartmentId.ToString) > -1 Then
                tNode.Expanded = True
            End If
            If Array.IndexOf(aChecked, DepartmentId.ToString) > -1 Then
                tNode.Checked = True
            End If

            If ParentId = Nothing Then
                Tree.Nodes.Add(tNode)
                tNode.Expanded = True
                ParentNode = tNode
            Else
                While ParentNode.Value <> ParentId
                    ParentNode = ParentNode.Parent
                End While
                ParentNode.ChildNodes.Add(tNode)
                ParentNode = tNode
            End If

        End While
        dr.Close()
    End Sub
End Class
