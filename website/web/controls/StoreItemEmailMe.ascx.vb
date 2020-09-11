Imports Components
Imports DataLayer
Imports Controls

Partial Class StoreItemEmailMe
	Inherits BaseControl

	Public Property ItemId() As Integer
		Get
			Return ViewState("ItemId")
		End Get
		Set(ByVal value As Integer)
			ViewState("ItemId") = value
		End Set
	End Property

	Public Property ItemAttributeId() As Integer
		Get
			Return ViewState("ItemAttributeId")
		End Get
		Set(ByVal value As Integer)
			ViewState("ItemAttributeId") = value
		End Set
	End Property

	Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
		divEmail.Visible = True
		divSubmit.Visible = False
		divRemove.Visible = False
	End Sub

	Private Sub btnNotify_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnNotify.Click
		If Not Page.IsValid Then
			UpdateSummary()
			Exit Sub
		End If

		Dim dbNotify As New StoreItemNotifyRow
		dbNotify.Email = Trim(txtEmail.Text)
		If Not ItemAttributeId = Nothing Then
			dbNotify = StoreItemNotifyRow.GetRow(DB, ItemAttributeId, dbNotify.Email, True)
			dbNotify.ItemAttributeId = ItemAttributeId
		Else
			dbNotify = StoreItemNotifyRow.GetRow(DB, ItemId, dbNotify.Email)
			dbNotify.ItemId = ItemId
		End If

		If dbNotify.ItemNotifyId = Nothing Then
			ltlMessage.Text = "Thank you. You will be notified when this product becomes available."
			dbNotify.Insert()

			divEmail.Visible = False
			divSubmit.Visible = True
			divRemove.Visible = False
		Else
			divEmail.Visible = False
			divSubmit.Visible = False
			divRemove.Visible = True
		End If

		txtEmail.Text = String.Empty
	End Sub

	Private Sub lnk_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkRemove.Click
		Dim dbNotify As New StoreItemNotifyRow
		dbNotify.Email = Trim(txtEmail.Text)
		If Not ItemAttributeId = Nothing Then
			dbNotify = StoreItemNotifyRow.GetRow(DB, ItemAttributeId, dbNotify.Email, True)
			dbNotify.ItemAttributeId = ItemAttributeId
		Else
			dbNotify = StoreItemNotifyRow.GetRow(DB, ItemId, dbNotify.Email)
			dbNotify.ItemId = ItemId
		End If

		If Not dbNotify.ItemNotifyId = Nothing Then
			dbNotify.Remove()
		End If

		divEmail.Visible = False
		divSubmit.Visible = True
		divRemove.Visible = False

		ltlMessage.Text = "Your email address has been removed."
		ViewState("Refresh") = "Y"
	End Sub

	Public Sub UpdateSummary()
		Dim valCol As ValidatorCollection = Page.Validators
		ltlErrorMsg.Text = String.Empty
		For Each val As IValidator In valCol
			If Not val.IsValid Then
				ltlErrorMsg.Text &= "<div class=""red"">" & val.ErrorMessage & "</div>"
			End If
		Next
	End Sub
End Class

