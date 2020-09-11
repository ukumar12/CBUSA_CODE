Imports Components
Imports DataLayer
Imports System.Data
Imports System.Data.SqlClient

Public Class faq
	Inherits SitePage

	Private dtFaqCategory As DataTable
	Private dtFaq As DataTable

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		If Not IsPostBack Then
			LoadData()
		End If
	End Sub

	Private Sub LoadData()
		dtFaqCategory = Me.DB.GetDataSet("Select * from FaqCategory  where IsActive = 1 order by SortOrder").Tables(0)
		dtFaq = Me.DB.GetDataSet("Select * from Faq where IsActive = 1 order by SortOrder").Tables(0)
		Me.rptTop.DataSource = dtFaqCategory
		Me.rptTop.DataBind()
		Me.rptMain.DataSource = dtFaqCategory
		Me.rptMain.DataBind()
	End Sub

	Protected Sub rptMain_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMain.ItemDataBound
		DindInnerRepeater(e)
	End Sub

	Protected Sub rptTop_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTop.ItemDataBound
		DindInnerRepeater(e)
	End Sub

	Private Sub DindInnerRepeater(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
		If Not (e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item) Then
			Exit Sub
		End If

		Dim upSend As UpdatePanel = e.Item.FindControl("upSend")
		If Not upSend Is Nothing AndAlso Not IsDBNull(e.Item.DataItem("AdminId")) Then
			upSend.Visible = True
			Dim lnkAsk As LinkButton = e.Item.FindControl("lnkAsk")
			If lnkAsk Is Nothing Then lnkAsk = e.Item.FindControl("lnkAsk2")
			lnkAsk.Text = "Ask a question about """ & e.Item.DataItem("CategoryName") & "."""
			lnkAsk.CommandArgument = e.Item.DataItem("FaqCategoryId")

			Dim btn As Button = e.Item.FindControl("btnSubmit")
			If btn Is Nothing Then btn = e.Item.FindControl("btnSubmit2")
			btn.CommandArgument = e.Item.DataItem("FaqCategoryId")

			btn = e.Item.FindControl("btnCancel")
			If btn Is Nothing Then btn = e.Item.FindControl("btnCancel2")
			btn.CommandArgument = e.Item.DataItem("FaqCategoryId")
		End If

		Dim rpt As Repeater = CType(e.Item.FindControl("rptInner"), Repeater)

		dtFaq.DefaultView.RowFilter = "FaqCategoryId = " & e.Item.DataItem("FaqCategoryId")
		If dtFaq.DefaultView.Count = 0 Then
			e.Item.Visible = False
			Exit Sub
		End If
		rpt.DataSource = dtFaq.DefaultView
		rpt.DataBind()
	End Sub

	Private Sub rpt_ItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs) Handles rptMain.ItemCommand, rptTop.ItemCommand
		Select Case e.CommandName
			Case "Ask"
				Dim ph As PlaceHolder = e.Item.FindControl("phAsk")
				If ph Is Nothing Then ph = e.Item.FindControl("phAsk2")
				ph.Visible = True

				ph = e.Item.FindControl("phSubmitted")
				If ph Is Nothing Then ph = e.Item.FindControl("phSubmitted2")
				ph.Visible = False


				Dim txt As TextBox = e.Item.FindControl("txtEmail")
				If txt Is Nothing Then txt = e.Item.FindControl("txtEmail2")
				If Session("MemberId") = Nothing Then
					txt.Text = Nothing
				Else
					Dim dbAddress As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, Session("MemberId"))
					txt.Text = MemberAddressRow.GetDefaultBillingRow(DB, Session("MemberId")).Email
				End If

				txt = e.Item.FindControl("txtMessage")
				If txt Is Nothing Then txt = e.Item.FindControl("txtMessage2")
				txt.Text = Nothing

			Case "Cancel"
				Dim ph As PlaceHolder = e.Item.FindControl("phAsk")
				If ph Is Nothing Then ph = e.Item.FindControl("phAsk2")
				ph.Visible = False
			Case "Submit"
				If Not IsValid Then Exit Sub

				Dim dbCategory As FaqCategoryRow = FaqCategoryRow.GetRow(DB, CInt(e.CommandArgument))

				Dim txt As TextBox = e.Item.FindControl("txtEmail")
				If txt Is Nothing Then txt = e.Item.FindControl("txtEmail2")

				Dim faq As New FaqRow(DB)
				faq.Email = txt.Text

				txt = e.Item.FindControl("txtMessage")
				If txt Is Nothing Then txt = e.Item.FindControl("txtMessage2")

				faq.Question = txt.Text
				faq.FaqCategoryId = dbCategory.FaqCategoryId
				faq.Insert()

				If Not dbCategory.AdminId = Nothing Then
					Dim dbAdmin As AdminRow = AdminRow.GetRow(DB, dbCategory.AdminId)
					Try
						Core.SendSimpleMail(faq.Email, faq.Email, dbAdmin.Email, dbAdmin.FirstName & " " & dbAdmin.LastName, "FAQ Question Submitted", "The following FAQ Question was submitted for FAQ Category """ & dbCategory.CategoryName & """" & vbCrLf & vbCrLf & faq.Question)
					Catch ex As Exception
					End Try
				End If

				Dim ph As PlaceHolder = e.Item.FindControl("phAsk")
				If ph Is Nothing Then ph = e.Item.FindControl("phAsk2")
				ph.Visible = False

				ph = e.Item.FindControl("phSubmitted")
				If ph Is Nothing Then ph = e.Item.FindControl("phSubmitted2")
				ph.Visible = True
		End Select
	End Sub
End Class
