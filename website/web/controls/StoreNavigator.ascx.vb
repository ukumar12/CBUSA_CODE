Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Partial Class StoreNavigator
	Inherits BaseControl

	Const NofLinks As Integer = 7

	Public Property URL() As String
		Get
            Return IIf(ViewState("URL") Is Nothing, AppSettings("GlobalRefererName") & "/store/default.aspx", ViewState("URL"))
		End Get
		Set(ByVal value As String)
            ViewState("URL") = AppSettings("GlobalRefererName") & value
		End Set
	End Property

    Public Property Exclude() As String
        Get
            Return ViewState("Exclude")
        End Get
        Set(ByVal value As String)
            ViewState("Exclude") = value
        End Set
    End Property

	Public Property Mode() As String
		Get
			Return IIf(ViewState("Mode") Is Nothing, "more", ViewState("Mode"))
		End Get
		Set(ByVal value As String)
			ViewState("Mode") = value
		End Set
	End Property

	Public Property Pg() As Integer
		Get
			If ViewState("pg") Is Nothing Then ViewState("pg") = 1
			If ViewState("pg") = 0 Then ViewState("pg") = 1
			Return ViewState("pg")
		End Get
		Set(ByVal value As Integer)
			ViewState("pg") = value
		End Set
    End Property
    Public Property Sort() As String
        Get
            If ViewState("sort") Is Nothing Then ViewState("sort") = "ItemName"
            Return ViewState("sort")
        End Get
        Set(ByVal value As String)
            ViewState("sort") = value
        End Set
    End Property

	Public Property MaxPerPage() As Integer
		Get
			Return ViewState("MaxPerPage")
		End Get
		Set(ByVal value As Integer)
			ViewState("MaxPerPage") = value
		End Set
	End Property

	Public Property NofRecords() As Integer
		Get
			Return ViewState("NofRecords")
		End Get
		Set(ByVal value As Integer)
			ViewState("NofRecords") = value
		End Set
	End Property

	Public ReadOnly Property NofPages() As Integer
		Get
			If MaxPerPage = 0 Then Return 0
			Return CType(Math.Round(CType(NofRecords / MaxPerPage + 0.499999, Single)), Integer)
		End Get
	End Property

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		If Not IsPostBack Then
			BindData()
		End If
	End Sub

	Private Sub BindData()
		If MaxPerPage < 0 Then MaxPerPage = 9999999
        Dim qs As URLParameters = Nothing

		ltl.Text = String.Empty

        qs = New URLParameters(Request.QueryString, "pg;" & Exclude)
        'Dim qssort As URLParameters = New URLParameters(Request.QueryString, "sort;pg" & Exclude)

		Dim StartLoop As Integer = Math.Max(Pg - NofLinks, 1)
		Dim EndLoop As Integer = Math.Min(Pg + NofLinks, NofPages)
		Dim bEnd As Boolean = False
		While EndLoop - StartLoop >= NofLinks
			If EndLoop - Pg > Pg - StartLoop Then
				EndLoop -= 1
			Else
				StartLoop += 1
			End If
		End While

		For i As Integer = StartLoop To EndLoop
			If i = Pg Then
				ltl.Text &= i & " "
			Else
				ltl.Text &= "<a href=""" & URL & IIf(i = 1, New URLParameters(Request.QueryString, "pg;" & Exclude).ToString, qs.ToString("pg", i)) & """>" & i & "</a> "
			End If
		Next
        lnkAll.Visible = NofRecords <= 400


        If NofPages = 0 Or NofPages = 1 Then
            ph.Visible = False
            lnkAll.Visible = False
        End If

        litDivider.Visible = lnkAll.Visible

        lnkPrev.HRef = URL & qs.ToString("pg", Pg - 1)
        If Pg = 1 Then lnkPrev.Visible = False
        lnkPrev2.Visible = Not lnkPrev.Visible

        lnkNext.HRef = URL & qs.ToString("pg", Pg + 1)
        If Pg = NofPages Then lnkNext.Visible = False
        lnkNext2.Visible = Not lnkNext.Visible

        spanPerPage.Attributes.Add("class", "bold")
        If Request("F_All") = Nothing Then lnkAll.HRef = URL & qs.ToString("F_All", "Y") Else lnkAll.HRef = URL & New URLParameters(Request.QueryString, "F_All;" & Exclude).ToString
        If Not Request("F_All") Is Nothing AndAlso Request("F_All") <> String.Empty Then
            lnkAll.InnerHtml = "View 12 per page"
            ph.Visible = False
        Else
            Select Case MaxPerPage
                Case 12
                    lnk12.Attributes.Add("class", "bold")
                Case 24
                    lnk24.Attributes.Add("class", "bold")
                Case Else
                    lnk48.Attributes.Add("class", "bold")
            End Select
        End If


        qs = New URLParameters(Request.QueryString, "pg;F_All;" & Exclude)
        lnk12.HRef = URL & qs.ToString("PerPage", 12)
        lnk24.HRef = URL & qs.ToString("PerPage", 24)
        lnk48.HRef = URL & qs.ToString("PerPage", 48)

        qs = New URLParameters(Request.QueryString, "pg;sort;" & Exclude)
        If Sort = "ItemName" Then
            ltlAlphabetical.Text = "<b>Alphabetical</b>"
        Else
            ltlAlphabetical.Text = "<a href=""" & URL & qs.ToString("sort", "ItemName") & """>Alphabetical</a>"
        End If
        If Sort = "Price Asc" Then
            ltlPriceAsc.Text = "<b>price ascending</b>"
        Else
            ltlPriceAsc.Text = "<a href=""" & URL & qs.ToString("sort", "Price Asc") & """>price ascending</a>"
        End If
        If Sort = "Price Desc" Then
            ltlPriceDesc.Text = "<b>price descending</b>"
        Else
            ltlPriceDesc.Text = "<a href=""" & URL & qs.ToString("sort", "Price Desc") & """>price descending</a>"
        End If
    End Sub

	Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
		If NofRecords = 0 Then Visible = False
	End Sub

End Class
