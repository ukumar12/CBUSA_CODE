Option Strict Off

Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer
Imports Components
Imports MasterPages
Imports System.Reflection

Partial Class InsertModule
    Inherits AdminPage

    Private ModuleWidth As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CONTENT_TOOL")

        Dim NofColumns As Integer = Request("NofColumns")
        If NofColumns < 1 Then NofColumns = 1
        dlModules.RepeatColumns = NofColumns

        ModuleWidth = Request("Width")
        dlModules.ItemStyle.Width = ModuleWidth

        LoadModules()
    End Sub

    Private Sub LoadModules()
        Dim SQL As String = "select * from ContentToolModule where MinWidth <= " & ModuleWidth & " and MaxWidth >= " & ModuleWidth & " and ModuleId <> 1"
        Dim dt As DataTable = DB.GetDataTable(SQL)

        dlModules.DataSource = dt.DefaultView
        dlModules.DataBind()
    End Sub

    Private Sub dlModules_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlModules.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim ph As PlaceHolder = e.Item.FindControl("ph")
        Dim ctrl As Control = Page.LoadControl(e.Item.DataItem("ControlURL"))
        ph.Controls.Add(ctrl)

        If TypeOf ctrl Is System.Web.UI.PartialCachingControl Then
            Dim cached As Control = CType(ctrl, PartialCachingControl).CachedControl
            If Not cached Is Nothing Then
                ph.Controls.Remove(ctrl)
                ctrl = cached
                ph.Controls.Add(ctrl)
            End If
        End If

        Dim c As IModule = Nothing
        If (TypeOf ctrl Is IModule) Then
            c = CType(ctrl, IModule)
        Else
            Exit Sub
        End If
        ctrl.ID = "m" & "_" & e.Item.DataItem("ModuleId")

        Dim ltlModuleName As Literal = e.Item.FindControl("ltlModuleName")
        Dim btnAdd As Button = e.Item.FindControl("btnAdd")

        ltlModuleName.Text = e.Item.DataItem("Name")

        Dim hdnField As HtmlInputHidden = CType(ctrl.FindControl("hdnField"), HtmlInputHidden)
        If hdnField Is Nothing Then
            btnAdd.Attributes("OnClick") = "window.opener.document.getElementById('__CommandArgument').value = '" & e.Item.DataItem("ModuleId") & "'; window.opener.__doPostBack('" & Request("ClientId") & "',''); window.close();"
        Else
            btnAdd.Attributes("OnClick") = "window.opener.document.getElementsByName('__CommandArgument')[0].value = '" & e.Item.DataItem("ModuleId") & "'; if(document.getElementById('" & hdnField.ClientID & "')) { window.opener.document.getElementsByName('__CommandArgs')[0].value = document.getElementById('" & hdnField.ClientID & "').value } else {window.opener.document.getElementsByName('__CommandArgs')[0].value = ''}; window.opener.__doPostBack('" & Request("ClientId") & "',''); window.close();"
        End If

        Dim m As IModule = CType(ctrl, IModule)
        If Not IsDBNull(e.Item.DataItem("Args")) Then
            m.Args = e.Item.DataItem("Args")
        End If
        If Not IsDBNull(e.Item.DataItem("HTML")) Then
            m.HTMLContent = e.Item.DataItem("HTML")
        End If
        m.Width = ModuleWidth
        m.IsDesignMode = True
    End Sub

End Class
