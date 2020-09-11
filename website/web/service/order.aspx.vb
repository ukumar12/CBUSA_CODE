Imports Components
Imports DataLayer

Public Class order
	Inherits SitePage

	Protected dbOrder As StoreOrderRow

	Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
		If Not IsPostBack Then
			dbOrder = New StoreOrderRow
			pnlOrder.Visible = False
		Else
			sc.OrderId = DB.ExecuteScalar("select top 1 orderid from storeorder where orderno = " & DB.Quote(txtOrderNo.Text) & " and billingzip = " & DB.Quote(txtZipcode.Text) & " and processdate is not null")
			dbOrder = StoreOrderRow.GetRow(DB, sc.OrderId)
		End If
	End Sub

	Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
		If Not IsValid Then Exit Sub

		If dbOrder.OrderId = Nothing Then
			AddError("Sorry, we could not find order information for the order number and zipcode you provided. Please check your order number and try again.")
			pnlOrder.Visible = False
			pnlTrack.Visible = True
		Else
			pnlTrack.Visible = False
			pnlOrder.Visible = True
		End If
	End Sub
End Class
