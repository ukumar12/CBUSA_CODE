Imports Components
Imports DataLayer

Partial Class controls_ProductTypeAttributesAdmin
    Inherits BaseControl

    Public Property ProductTypeId() As Integer
        Get
            Return ViewState("ProductTypeId")
        End Get
        Set(ByVal value As Integer)
            ViewState("ProductTypeId") = value
        End Set
    End Property

    Public Property ProductId() As Integer
        Get
            Return ViewState("ProductId")
        End Get
        Set(ByVal value As Integer)
            ViewState("ProductId") = value
        End Set
    End Property

    Private m_Values As New Generic.Dictionary(Of String, Object)
    Public Property Value(ByVal AttributeName As String) As Object
        Get
            For Each item As RepeaterItem In rptAttributes.Items
                Dim ctl As Object = item.FindControl("phControl").FindControl(AttributeName)
                If ctl IsNot Nothing Then
                    If TypeOf ctl Is TextBox Then
                        Return ctl.Text
                    ElseIf TypeOf ctl Is DropDownList Or TypeOf ctl Is RadioButtonList Then
                        Return ctl.SelectedValue
                    End If
                End If
            Next
            Return Nothing
            'If ViewState("Values") Is Nothing OrElse Not ViewState("Values").ContainsKey(AttributeName) Then
            '    Return Nothing
            'Else
            '    Return ViewState("Values")(AttributeName)
            'End If
        End Get
        Set(ByVal value As Object)
            m_Values(AttributeName) = value
            'If ViewState("Values") Is Nothing Then
            '    ViewState("Values") = New Generic.Dictionary(Of String, Object)
            'End If
            'ViewState("Values")(AttributeName) = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If ProductTypeId = Nothing And ProductId = Nothing Then
            Exit Sub
        ElseIf ProductTypeId = Nothing Then
            Dim dbProduct As ProductRow = ProductRow.GetRow(DB, ProductId)
            ProductTypeId = dbProduct.ProductTypeID
        End If

        'note -- databind on every load to re-create dynamic controls
        BindData()
    End Sub

    Private m_dtOptions As DataTable
    Private ReadOnly Property dtOptions() As DataTable
        Get
            If m_dtOptions Is Nothing Then
                m_dtOptions = ProductTypeAttributeValueOptionRow.GetList(DB)
            End If
            Return m_dtOptions
        End Get
    End Property

    'Private Sub LoadValues()
    '    For Each item As RepeaterItem In rptAttributes.Items
    '        Select Case item.DataItem("InputTypeId")
    '            Case InputType.Dropdown
    '                Dim ctl As DropDownList = item.FindControl("ctlAttribute")
    '                Value(item.DataItem("Attribute")) = ctl.SelectedValue
    '            Case InputType.Text, InputType.Number
    '                Dim ctl As TextBox = item.FindControl("ctlAttribute")
    '                Value(item.DataItem("Attribute")) = ctl.Text
    '            Case InputType.YesNo
    '                Dim ctl As RadioButtonList = item.FindControl("ctlAttribute")
    '                Value(item.DataItem("Attribute")) = ctl.SelectedValue
    '        End Select
    '    Next
    'End Sub

    Public Sub BindData()
        Dim dt As DataTable = ProductTypeAttributeRow.GetListByType(DB, ProductTypeId)

        rptAttributes.DataSource = dt
        rptAttributes.DataBind()
    End Sub

    Protected Overrides Sub OnDataBinding(ByVal e As System.EventArgs)
        MyBase.OnDataBinding(e)
        Me.DataBindChildren()
        BindData()
    End Sub

    Protected Sub rptAttributes_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptAttributes.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim phControl As PlaceHolder = e.Item.FindControl("phControl")
        Dim phValidator As PlaceHolder = e.Item.FindControl("phValidator")

        Select Case e.Item.DataItem("InputTypeId")
            Case InputType.Dropdown
                Dim ctl As New DropDownList()
                Dim options As DataView = dtOptions.DefaultView
                options.RowFilter = "ProductTypeAttributeId=" & DB.Number(e.Item.DataItem("ProductTypeAttributeId"))
                ctl.ID = e.Item.DataItem("Attribute")
                ctl.DataSource = options
                ctl.DataTextField = "ValueOption"
                ctl.DataValueField = "ValueOption"
                ctl.DataBind()
                If m_Values.ContainsKey(e.Item.DataItem("Attribute")) Then
                    ctl.SelectedValue = m_Values(e.Item.DataItem("Attribute"))
                ElseIf Not IsDBNull(e.Item.DataItem("DefaultValue")) Then
                    ctl.SelectedValue = e.Item.DataItem("DefaultValue")
                End If
                phControl.Controls.Add(ctl)

                If e.Item.DataItem("IsRequired") Then
                    Dim val As New RequiredFieldValidator()
                    val.ID = "rfvctlAttribute"
                    val.ControlToValidate = e.Item.DataItem("Attribute")
                    val.ErrorMessage = e.Item.DataItem("Attribute") & " is blank"
                    phValidator.Controls.Add(val)
                End If

            Case InputType.Number
                Dim ctl As New TextBox
                ctl.ID = e.Item.DataItem("Attribute")
                ctl.MaxLength = 50
                ctl.Columns = 50
                ctl.Style.Add("width", "100px")
                If m_Values.ContainsKey(e.Item.DataItem("Attribute")) Then
                    ctl.Text = m_Values(e.Item.DataItem("Attribute"))
                ElseIf Not IsDBNull(e.Item.DataItem("DefaultValue")) Then
                    ctl.Text = e.Item.DataItem("DefaultValue")
                End If
                phControl.Controls.Add(ctl)

                If e.Item.DataItem("IsRequired") Then
                    Dim val As New RequiredFieldValidator
                    val.ID = "rfvctlAttribute"
                    val.ControlToValidate = e.Item.DataItem("Attribute")
                    val.ErrorMessage = e.Item.DataItem("Attribute") & " is blank"
                    phValidator.Controls.Add(val)

                    Dim re As New RegularExpressionValidator
                    re.ID = "revctlAttribute"
                    re.ValidationExpression = "^[\d]*\.?[\d]+$"
                    re.ControlToValidate = e.Item.DataItem("Attribute")
                    re.ErrorMessage = e.Item.DataItem("Attribute") & " is invalid"
                    phValidator.Controls.Add(re)
                End If

            Case InputType.Text
                Dim ctl As New TextBox
                ctl.ID = e.Item.DataItem("Attribute")
                ctl.Columns = 50
                ctl.MaxLength = 50
                ctl.Style.Add("width", "319px")
                If m_Values.ContainsKey(e.Item.DataItem("Attribute")) Then
                    ctl.Text = m_Values(e.Item.DataItem("Attribute"))
                ElseIf Not IsDBNull(e.Item.DataItem("DefaultValue")) Then
                    ctl.Text = e.Item.DataItem("DefaultValue")
                End If
                phControl.Controls.Add(ctl)

                If e.Item.DataItem("IsRequired") Then
                    Dim val As New RequiredFieldValidator
                    val.ID = "rfvctlAttribute"
                    val.ControlToValidate = e.Item.DataItem("Attribute")
                    val.ErrorMessage = e.Item.DataItem("Attribute") & " is blank"
                    phValidator.Controls.Add(val)
                End If

            Case InputType.YesNo
                Dim ctl As New RadioButtonList
                ctl.ID = e.Item.DataItem("Attribute")
                ctl.RepeatLayout = RepeatLayout.Flow
                ctl.RepeatDirection = RepeatDirection.Horizontal
                ctl.Items.Add(New ListItem("Yes", True))
                ctl.Items.Add(New ListItem("No", False))
                If m_Values.ContainsKey(e.Item.DataItem("Attribute")) Then
                    ctl.SelectedValue = m_Values(e.Item.DataItem("Attribute"))
                ElseIf Not IsDBNull(e.Item.DataItem("DefaultValue")) Then
                    ctl.SelectedValue = e.Item.DataItem("DefaultValue")
                End If
                phControl.Controls.Add(ctl)

                If e.Item.DataItem("IsRequired") Then
                    Dim val As New RequiredFieldValidator
                    val.ID = "rfvctlAttribute"
                    val.ControlToValidate = e.Item.DataItem("Attribute")
                    val.ErrorMessage = e.Item.DataItem("Attribute") & " is not selected"
                    phValidator.Controls.Add(val)
                End If
        End Select
    End Sub
End Class
