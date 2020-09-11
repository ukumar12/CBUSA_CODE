Imports Components
Imports DataLayer

Partial Class StoreRecipientsDropDown
    Inherits BaseControl

    Public Property RecipientId() As Integer
        Get
            Return ViewState("RecipientId")
        End Get
        Set(ByVal value As Integer)
            ViewState("RecipientId") = value
        End Set
    End Property

    Public Property OrderId() As Integer
        Get
            Return ViewState("OrderId")
        End Get
        Set(ByVal value As Integer)
            ViewState("OrderId") = value
        End Set
    End Property

    Public Property MemberId() As Integer
        Get
            Return ViewState("MemberId")
        End Get
        Set(ByVal value As Integer)
            ViewState("MemberId") = value
        End Set
    End Property

	Public ReadOnly Property Label() As String
		Get
			Return Me.ID
		End Get
	End Property

    Public ReadOnly Property SelectedValue() As String
        Get
            If drpRecipient.SelectedValue = "OtherSpecify" Then
                Return txtNew.Text
            Else
                Return drpRecipient.SelectedValue
            End If
        End Get
	End Property

	Public ReadOnly Property RecipientDropDown() As DropDownList
		Get
			Return drpRecipient
		End Get
	End Property

	Public ReadOnly Property NewRow() As HtmlTableRow
		Get
			Return trNew
		End Get
	End Property

	Public ReadOnly Property NewText() As TextBox
		Get
			Return txtNew
		End Get
	End Property

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		drpRecipient.Attributes.Add("onchange", "LabelChange" & Label & "();")
		If Not IsPostBack Then
			BindData()
		End If
	End Sub

	Private Sub BindData()
		Dim dtRecipients As DataTable = Nothing
		drpRecipient.Items.Add(New ListItem("Myself", "Myself"))
		If Not OrderId = 0 Then
			dtRecipients = StoreOrderRecipientRow.GetOrderRecipientsOrderByLabel(DB, OrderId)
			Dim i As Integer = 1
			For Each row As DataRow In dtRecipients.Rows
				Dim item As ListItem = New ListItem(row("Label"), row("RecipientId") & "_sor")
				drpRecipient.Items.Add(item)
				If row("RecipientId") = RecipientId Then drpRecipient.SelectedIndex = i
				i += 1
			Next
		End If
		If Not MemberId = 0 Then
			dtRecipients = MemberAddressRow.GetAddressBookRecipients(DB, MemberId, OrderId)
			For Each row As DataRow In dtRecipients.Rows
				drpRecipient.Items.Add(New ListItem(row("Label"), row("AddressId") & "_mab"))
			Next
		End If
		drpRecipient.Items.Add(New ListItem("Other (please specify below)", "OtherSpecify"))
	End Sub

	Protected Sub cvLabel_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvLabel.ServerValidate
		If drpRecipient.SelectedValue = "OtherSpecify" AndAlso txtNew.Text = String.Empty Then
			args.IsValid = False
		Else
			args.IsValid = True
		End If
	End Sub
End Class
