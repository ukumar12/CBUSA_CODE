Imports Components
Imports DataLayer

Partial Class modules_StoreHome
    Inherits ModuleControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        BindDepartments()
    End Sub

    Private Sub BindDepartments()
        Dim dt As DataTable = StoreDepartmentRow.GetMainLevelDepartmentsHome(DB)
        dlDepartments.DataSource = dt
        dlDepartments.DataBind()
    End Sub

    Private Sub dlDepartments_ItemDataBound(ByVal sender As Object, ByVal e As DataListItemEventArgs) Handles dlDepartments.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim r As Repeater = e.Item.FindControl("rpt")
        Dim d As StoreDepartmentRow = StoreDepartmentRow.GetRow(DB, e.Item.DataItem("DepartmentId"))
        AddHandler r.ItemDataBound, AddressOf rpt_ItemDataBound
        r.DataSource = d.GetChildrenDepartments()
        r.DataBind()
    End Sub

    Private Sub rpt_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
        If Not e.Item.ItemType = ListItemType.Item AndAlso Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim Name As String = e.Item.DataItem("Name")
        If Name.Length > 32 Then Name = Left(Name, Left(Name, 32).LastIndexOf(" "))

        CType(e.Item.FindControl("lit"), Literal).Text = Name
    End Sub

End Class
