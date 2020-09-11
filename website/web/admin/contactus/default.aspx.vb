Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		CheckAccess("CONTACT_US")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_QuestionId.DataSource = ContactUsQuestionRow.GetAllContactUsQuestions(DB)
			F_QuestionId.DataValueField = "QuestionId"
			F_QuestionId.DataTextField = "Question"
			F_QuestionId.DataBind()
			F_QuestionId.Items.Insert(0, New ListItem("-- ALL --", ""))

			F_FullName.Text = Request("F_FullName")
			F_Email.Text = Request("F_Email")
			F_OrderNumber.Text = Request("F_OrderNumber")
			F_Phone.Text = Request("F_Phone")
			F_QuestionId.SelectedValue = Request("F_QuestionId")
			F_CreateDateLBound.Text = Request("F_CreateDateLBound")
			F_CreateDateUbound.Text = Request("F_CreateDateUBound")

			F_OutputAs.selectedvalue = Request("F_OutputAs")

			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then
				gvList.SortBy = "createdate"
				gvList.SortOrder = "desc"
			End If

			BindList()
		End If

		gvList.Columns(8).Visible = F_OutputAs.SelectedValue = "Excel"
	End Sub

	Private Sub ExportList()
		Dim SQLFields, SQL As String

		SQLFields = "SELECT cu.*, cuq.question "
		SQL = BuildQuery()

		Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

		Dim Folder As String = "/assets/contactus/"
		Dim i As Integer = 0
		Dim FileName As String = Core.GenerateFileID & ".csv"

		Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
		sw.WriteLine("Web Orders Report")
		sw.WriteLine("Report generated on " & DateTime.Now.ToString("d"))
		sw.WriteLine(String.Empty)
		sw.WriteLine("Search Criteria")
		sw.WriteLine("Full Name:," & F_FullName.Text)
		sw.WriteLine("Email:," & F_Email.Text)
		sw.WriteLine("Order Number:," & F_OrderNumber.Text)
		sw.WriteLine("Phone:," & F_Phone.Text)
		sw.WriteLine("Question:," & F_QuestionId.SelectedItem.Text)
		sw.WriteLine("Create Date From:," & F_CreateDateLbound.Text)
		sw.WriteLine("Create Date To:," & F_CreateDateUbound.Text)

		sw.WriteLine(String.Empty)
		sw.WriteLine(String.Empty)

		sw.WriteLine("Full Name,Email,Order Number,Phone,How Heard,Question,Message,Create Date")
		For Each dr As DataRow In res.Rows
			Dim FullName As String = IIf(IsDBNull(dr("FullName")), String.Empty, dr("FullName"))
			Dim Email As String = Convert.ToString(dr("Email"))
			Dim OrderNumber As String = Convert.ToString(dr("OrderNumber"))
			Dim Phone As String = Convert.ToString(dr("Phone"))
			Dim HowHeardName As String = Convert.ToString(dr("HowHeardName"))
			Dim Question As String = Convert.ToString(dr("Question"))
			Dim Message As String = Convert.ToString(dr("YourMessage"))
			Dim CreateDate As String = dr("CreateDate")

			sw.WriteLine(Core.QuoteCSV(FullName) & "," & Core.QuoteCSV(Email) & "," & Core.QuoteCSV(OrderNumber) & "," & Core.QuoteCSV(Phone) & _
			 "," & Core.QuoteCSV(HowHeardName) & "," & Core.QuoteCSV(Question) & "," & Core.QuoteCSV(Message) & "," & Core.QuoteCSV(CreateDate))
		Next
		sw.Flush()
		sw.Close()

		lnkDownload.NavigateUrl = Folder & FileName
	End Sub

	Private Function BuildQuery() As String
		Dim Conn As String = " where "

		Dim SQL As String =  " FROM ContactUs cu inner join contactusquestion cuq on cu.questionid = cuq.questionid "

		If Not F_Phone.Text = String.Empty Then
			SQL = SQL & Conn & "Phone = " & DB.Quote(F_Phone.Text)
			Conn = " AND "
		End If
		If Not F_QuestionId.SelectedValue = String.Empty Then
			SQL = SQL & Conn & "cu.QuestionId = " & DB.Quote(F_QuestionId.SelectedValue)
			Conn = " AND "
		End If
		If Not F_FullName.Text = String.Empty Then
			SQL = SQL & Conn & "FullName LIKE " & DB.FilterQuote(F_FullName.Text)
			Conn = " AND "
		End If
		If Not F_Email.Text = String.Empty Then
			SQL = SQL & Conn & "Email LIKE " & DB.FilterQuote(F_Email.Text)
			Conn = " AND "
		End If
		If Not F_OrderNumber.Text = String.Empty Then
			SQL = SQL & Conn & "OrderNumber LIKE " & DB.FilterQuote(F_OrderNumber.Text)
			Conn = " AND "
		End If
		If Not F_CreateDateLbound.Text = String.Empty Then
			SQL = SQL & Conn & "CreateDate >= " & DB.Quote(F_CreateDateLbound.Text)
			Conn = " AND "
		End If
		If Not F_CreateDateUbound.Text = String.Empty Then
			SQL = SQL & Conn & "CreateDate < " & DB.Quote(DateAdd("d", 1, F_CreateDateUbound.Text))
			Conn = " AND "
		End If
		Return SQL
	End Function

	Private Sub BindList()
		Dim SQLFields, SQL As String
		Dim Conn As String = " where "

		ViewState("F_SortBy") = gvList.SortBy
		ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " cu.*, cuq.question "
		SQL = BuildQuery()

		gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

		Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
		gvList.DataSource = res.DefaultView
		gvList.DataBind()
	End Sub

	Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
		If Not IsValid Then Exit Sub

		gvList.PageIndex = 0

		If F_OutputAs.SelectedValue = "Excel" Then
			divDownload.Visible = True
			gvList.Visible = False
			ExportList()
		Else
			divDownload.Visible = False
			gvList.Visible = True
			BindList()
		End If
	End Sub
End Class

