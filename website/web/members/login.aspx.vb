Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class Members_Login
    Inherits SitePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureSSL()
        If IsLoggedIn() Then
            Response.Redirect("/members/")
        End If
    End Sub

    Protected Sub btnRegister_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRegister.Click
        Response.Redirect("/members/register.aspx")
    End Sub

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        If Not IsValid Then Exit Sub

		If MemberRow.PerformMemberLogin(DB, txtUsername.Text, txtPassword.Text, chkPersist.Checked) Then
			If IsNumeric(Request("ItemId")) AndAlso Not CInt(Request("ItemId")) = Nothing Then
				Dim dtTree As DataTable = DB.GetDataTable("exec sp_GetAttributeTree " & CInt(Request("ItemId")))
				Dim Ids As String = Request("ItemAttributeIds")
				Dim aIds As String() = Ids.Split(",")
				Dim col As New ItemAttributeCollection
				For Each AttributeId As String In aIds
					If Not IsNumeric(AttributeId) Then
						Response.Redirect("/members/")
					Else
						For Each dr As DataRow In dtTree.Rows
							If dr("ItemAttributeId") = CInt(AttributeId) Then
								Dim attr As New ItemAttribute
								attr.AttributeType = IIf(IsDBNull(dr("AttributeType")), String.Empty, dr("AttributeType"))
								attr.AttributeName = IIf(IsDBNull(dr("AttributeName")), String.Empty, dr("AttributeName"))
								attr.AttributeValue = IIf(IsDBNull(dr("AttributeValue")), String.Empty, dr("AttributeValue"))
								attr.ImageName = IIf(IsDBNull(dr("ImageName")), String.Empty, dr("ImageName"))
								attr.ImageAlt = IIf(IsDBNull(dr("ImageAlt")), String.Empty, dr("ImageAlt"))
								attr.ParentAttributeId = IIf(IsDBNull(dr("ParentAttributeId")), Nothing, dr("ParentAttributeId"))
								attr.ProductAlt = IIf(IsDBNull(dr("ProductAlt")), String.Empty, dr("ProductAlt"))
								attr.ProductImage = IIf(IsDBNull(dr("ProductImage")), String.Empty, dr("ProductImage"))
								attr.Weight = IIf(IsDBNull(dr("Weight")), 0, dr("Weight"))
								attr.ItemAttributeId = IIf(IsDBNull(dr("ItemAttributeId")), 0, dr("ItemAttributeId"))
								attr.ItemId = Request("ItemId")
								attr.Price = IIf(IsDBNull(dr("Price")), 0, dr("Price"))
								attr.SKU = IIf(IsDBNull(dr("SKU")), String.Empty, dr("SKU"))
								attr.SortOrder = IIf(IsDBNull(dr("SortOrder")), 0, dr("SortOrder"))
								attr.TemplateAttributeId = IIf(IsDBNull(dr("TemplateAttributeId")), 0, dr("TemplateAttributeId"))
								col.Add(attr)
							End If
						Next
					End If
				Next

				If StoreItemRow.IsValidAttributes(DB, CInt(Request("ItemId")), col) Then
					Dim redirect As Boolean = False
					Try
						DB.BeginTransaction()
						Wishlist.Add2Wishlist(DB, Session("MemberId"), CInt(Request("ItemId")), CInt(Request("Qty")), col)
						DB.CommitTransaction()
						redirect = True
					Catch ex As Exception
						DB.RollbackTransaction()
					End Try
					If redirect Then Response.Redirect("/members/wishlist/")
				End If
			End If

			If Not Request("redir") = String.Empty Then
				Response.Redirect(Request("redir"))
			Else
				Response.Redirect("/members/")
			End If
		Else
			AddError("The password you entered does not match the one for this account. Please try again, or go to the <a href='/members/forgot.aspx'>forgot your password</a> page to retrieve it")
		End If
	End Sub
End Class