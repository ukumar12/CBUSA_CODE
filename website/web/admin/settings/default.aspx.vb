Imports Components
Imports System.Data
Imports DataLayer
Imports Controls

Public Class default_settings
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("USERS")

        If Not IsPostBack Then
            BindRepeater()
        End If
    End Sub

    Private Sub BindRepeater()
        Dim dbAdmin As AdminRow = AdminRow.GetRow(DB, LoggedInAdminId)
        sysparamRepeater.DataSource = SysparamRow.GetList(DB, dbAdmin.IsInternal)
        sysparamRepeater.DataBind()
    End Sub

    Private prevGroup As String = ""

    Private Sub SysparamRepeater_OnItemDataBound(ByVal sender As System.Object, ByVal e As RepeaterItemEventArgs) Handles sysparamRepeater.ItemDataBound
        Dim plcEditPlace As PlaceHolder = Nothing
        Dim plcValidatePlace As PlaceHolder = Nothing
        Dim lblSysparamName As Label = Nothing
        Dim trHeaderRow As HtmlTableRow = Nothing
        Dim lblHeaderLabel As Label = Nothing
        Dim bHasRow As Boolean = False
        Dim btnSave As Button = Nothing

        Select Case e.Item.ItemType
            Case ListItemType.Item
                lblSysparamName = e.Item.FindControl("sysparamName")
                plcEditPlace = e.Item.FindControl("editPlace")
                plcValidatePlace = e.Item.FindControl("validatePlace")
                trHeaderRow = e.Item.FindControl("headerRow")
                lblHeaderLabel = e.Item.FindControl("headerLabel")
                btnSave = e.Item.FindControl("saveButton")
                bHasRow = True
            Case ListItemType.AlternatingItem
                lblSysparamName = e.Item.FindControl("sysparamNameALT")
                plcEditPlace = e.Item.FindControl("editPlaceALT")
                plcValidatePlace = e.Item.FindControl("validatePlaceALT")
                trHeaderRow = e.Item.FindControl("headerRowALT")
                lblHeaderLabel = e.Item.FindControl("headerLabelALT")
                btnSave = e.Item.FindControl("saveButtonALT")
                bHasRow = True
        End Select

        If bHasRow Then
            Dim id As Integer = e.Item.DataItem("ParamId")
            Dim groupName As String = e.Item.DataItem("GroupName")
            Dim name As String = e.Item.DataItem("Name")
            Dim value As String
            If IsDBNull(e.Item.DataItem("Value")) Then
                value = String.Empty
            Else
                If e.Item.DataItem("IsEncrypted") Then
                    value = Utility.Crypt.DecryptTripleDes(e.Item.DataItem("Value"))
                Else
                    value = e.Item.DataItem("Value")
                End If
            End If
            Dim type As String = IIf(IsDBNull(e.Item.DataItem("Type")), String.Empty, e.Item.DataItem("Type"))
            Dim comments As String
            If IsDBNull(e.Item.DataItem("Comments")) Then
                comments = ""
            Else
                comments = e.Item.DataItem("Comments")
            End If

            lblSysparamName.Text = comments

            Dim box As New TextBox
            Dim drp As New DropDownList
            Select Case type
                Case "TRANSACTION"
                    drp.ID = "PARAM" & id
                    drp.Items.Add(New ListItem("Authorization", "A"))
                    drp.Items.Add(New ListItem("Sale", "S"))
                    drp.SelectedValue = value
                    plcEditPlace.Controls.Add(drp)
                    btnSave.CommandArgument = id & "|" & drp.UniqueID
                Case "SHIPPING"
                    drp.ID = "PARAM" & id
                    drp.Items.Add(New ListItem(""))
                    drp.Items.Add(New ListItem("Defined for each product", "ShippingProd"))
                    drp.Items.Add(New ListItem("Based On Purchase Range", "ShippingRange"))
                    drp.Items.Add(New ListItem("Straight Percentage of Amount", "ShippingPerc"))
                    drp.Items.Add(New ListItem("Always the same", "ShippingSame"))
                    drp.Items.Add(New ListItem("Shiping via UPS", "ShippingUPS"))
                    drp.Items.Add(New ListItem("Shiping via FedEx", "ShippingFedEx"))
                    drp.Items.Add(New ListItem("Custom Shipping", "ShippingCustom"))
                    drp.SelectedValue = value
                    plcEditPlace.Controls.Add(drp)
                    btnSave.CommandArgument = id & "|" & drp.UniqueID
                Case "YESNO"
                    drp.ID = "PARAM" & id
                    drp.Items.Add(New ListItem("Yes", "1"))
                    drp.Items.Add(New ListItem("No", "0"))
                    drp.SelectedValue = value
                    plcEditPlace.Controls.Add(drp)
                    btnSave.CommandArgument = id & "|" & drp.UniqueID
                Case String.Empty
                    btnSave.Visible = False
                Case Else
                    box.ID = "PARAM" & id
                    box.Text = value
                    plcEditPlace.Controls.Add(box)
                    btnSave.CommandArgument = id & "|" & box.UniqueID
            End Select

            'Add required validator
            Dim ReqVal As New RequiredFieldValidator
            Select Case type
                Case "TRANSACTION"
                    ReqVal.ControlToValidate = drp.ID
                    ReqVal.ErrorMessage = "Value is blank"
                    ReqVal.Display = ValidatorDisplay.Dynamic
                    ReqVal.ValidationGroup = "Group"
                    plcValidatePlace.Controls.Add(ReqVal)
                Case "YESNO"
                    'Do nothing    
            End Select

            Select Case type
                Case "STRING"
                    box.Columns = 40
                    box.MaxLength = 100
                Case "TA"
                    box.Columns = 40
                    box.Rows = 10
                    box.TextMode = TextBoxMode.MultiLine
                    box.MaxLength = 1000
                Case "INTEGER"
                    box.Columns = 5
                    box.MaxLength = 5

                    'Add integer validator
                    Dim IntVal As New IntegerValidator
                    IntVal.ControlToValidate = box.ID
                    IntVal.ErrorMessage = "Integer Value is invalid"
                    IntVal.Display = ValidatorDisplay.Dynamic
                    IntVal.ValidationGroup = "Group"
                    plcValidatePlace.Controls.Add(IntVal)
                Case "FLOAT"
                    box.Columns = 5
                    box.MaxLength = 8

                    'Add float validator
                    Dim FloatVal As New FloatValidator
                    FloatVal.ControlToValidate = box.ID
                    FloatVal.ErrorMessage = "Integer Value is invalid"
                    FloatVal.Display = ValidatorDisplay.Dynamic
                    FloatVal.ValidationGroup = "Group"
                    plcValidatePlace.Controls.Add(FloatVal)

                Case "TRANSACTION", "YESNO"
            End Select

            If groupName = prevGroup Then
                trHeaderRow.Visible = False
            Else
                trHeaderRow.Visible = True
                lblHeaderLabel.Text = groupName
                prevGroup = groupName
            End If
        End If
    End Sub

    Private Sub sysparamRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles sysparamRepeater.ItemCommand
        If e.CommandName = "Save" Then
            Dim tmp As String = e.CommandArgument
            Dim aTmp() As String = tmp.Split("|")
            Dim value As String = Request(aTmp(1))
            Dim id As Integer = aTmp(0)

            Dim dbSysParam As SysparamRow = SysparamRow.GetRow(DB, id)
            If dbSysParam.Name = "PasswordEx" AndAlso value > 0 Then
                DB.ExecuteSQL("Update Admin Set PasswordEx=Password,PasswordDate=null where Password is not null")
            End If

            dbSysParam.Value = value
            dbSysParam.Update()
            Core.LogEvent("Value for System Parameter """ & dbSysParam.Name & """ was set to """ & dbSysParam.Value & """ by user """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
        End If
        BindRepeater()
    End Sub
End Class
