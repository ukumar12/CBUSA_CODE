Option Strict Off

Imports Components
Imports DataLayer

Partial Class ServiceLeftNavigation
	Inherits ModuleControl

	Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
		If Not Request.Path.ToLower = "/service/default.aspx" Then
			ltlHeader.Text = "<a href=""/service/default.aspx"">Customer Service</a>"
		Else
			ltlHeader.Text = "Customer Service"
		End If

		If Request.Path.ToLower = "/service/faq.aspx" Then
			ltlFAQ.Text = "View FAQ's"
		Else
			ltlFAQ.Text = "<a href=""/service/faq.aspx"">View FAQ's</a>"
		End If

		If Request.Path.ToLower = "/service/contact.aspx" Then
			ltlContact.Text = "Contact Us"
		Else
			ltlContact.Text = "<a href=""/service/contact.aspx"">Contact Us</a>"
		End If

		If Session("MemberId") = Nothing Then
			If Request.Path.ToLower = "/service/order.aspx" Then
				ltlOrder.Text = "Check Order Status"
			Else
				ltlOrder.Text = "<a href=""/service/order.aspx"">Check Order Status</a>"
			End If
		Else
			ltlOrder.Text = "<a href=""/members/orderhistory/default.aspx"">Check Order Status</a>"
		End If
	End Sub
End Class
