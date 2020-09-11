Imports Components
Imports DataLayer

Partial Class DiscoveryDropDown
	Inherits BaseControl

	Protected intCtrlWidth As Integer = Me.Width

	Public Property HowHeardId() As Integer
		Get
			Return ViewState("HowHeardId")
		End Get
		Set(ByVal value As Integer)
			ViewState("HowHeardId") = value
		End Set
	End Property

	Public Property OtherText() As String
		Get
			Return ViewState("DiscoveryOtherText")
		End Get
		Set(ByVal value As String)
			ViewState("DiscoveryOtherText") = value
		End Set
	End Property

	Protected ReadOnly Property Label() As String
		Get
			Return Me.ID
		End Get
	End Property

	Public ReadOnly Property SelectedValue() As String
		Get
			If drpHowHeardList.SelectedItem.Value <> String.Empty Then
				If HowHeardRow.GetRow(DB, CInt(drpHowHeardList.SelectedValue)).IsUserInput Then
					Return drpHowHeardList.SelectedItem.Text & ": " & txtDiscoveryOther.Text
				Else
					Return drpHowHeardList.SelectedItem.Text
				End If
			Else
				Return String.Empty
			End If
		End Get
	End Property

	Public ReadOnly Property SelectedID() As Integer
		Get
			If drpHowHeardList.SelectedItem.Value <> String.Empty Then
				Return CInt(drpHowHeardList.SelectedItem.Value)
			Else
				Return Nothing
			End If
		End Get
	End Property

	Public ReadOnly Property IsUserInput() As Boolean
		Get
			Return HowHeardRow.GetRow(DB, HowHeardId).IsUserInput
		End Get
	End Property

	Public Overridable Property Width() As String
		Get
			Return ViewState("Width")
		End Get
		Set(ByVal value As String)
			ViewState("Width") = value
		End Set
	End Property

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

		Dim strOthers As String = ""
		Dim strLabels As String = ""

		Dim dt As DataTable = HowHeardRow.GetAllHowHeard(DB)
		For Each drHH As DataRow In dt.Rows
			If drHH("IsUserInput") = True Then
				If strOthers = String.Empty Then
					strOthers = Core.Escape(drHH("HowHeardId"))
                    strLabels = Core.Escape(IIf(IsDBNull(drHH("UserInputLabel")), "", drHH("UserInputLabel")))
				Else
					strOthers &= "," & Core.Escape(drHH("HowHeardId"))
					strLabels &= "," & Core.Escape(IIf(IsDBNull(drHH("UserInputLabel")), "", drHH("UserInputLabel")))
				End If
			End If
		Next

		Dim strCallFunctCode As String = "ShowDiscoveryOther('" & drpHowHeardList.ClientID & "','" & trOther.ClientID & "','" & txtDiscoveryOther.ClientID & "','" & divUserInputLabel.ClientID & "', new Array(" & strOthers & "), new Array(" & strLabels & "));"

		drpHowHeardList.Attributes.Add("onchange", strCallFunctCode)

		'call the function at the end of page load to load the existing vals if needed
		Dim strJS As New StringBuilder("<script language=""JavaScript"" type=""text/javascript"">")
		strJS.AppendLine("<!--//")
		strJS.AppendLine(vbCrLf & strCallFunctCode & vbCrLf)
		strJS.AppendLine("//-->")
		strJS.AppendLine("</script>")
		If Not Page.ClientScript.IsStartupScriptRegistered("DiscoveryOtherCall_" & Me.ClientID) Then
			Page.ClientScript.RegisterStartupScript(Me.GetType, "DiscoveryOtherCall_" & Me.ClientID, strJS.ToString)
		End If

		'add the ShowDiscoveryOther Function to the top of the page if it hasnt been already added
		If Not Page.ClientScript.IsClientScriptBlockRegistered("ShowDiscoveryOther") Then
			Dim strSDCFunction As New StringBuilder("<script language=""JavaScript"" type=""text/javascript"">")
			strSDCFunction.AppendLine("<!--//")
			strSDCFunction.AppendLine("function ShowDiscoveryOther(ctrlId, ctrlTrId, ctrlTxtId, ctrlLabelId, aOthers, aLabels) {")
			strSDCFunction.AppendLine("	var ctrl = document.getElementById(ctrlId);")
			strSDCFunction.AppendLine("	var ctrlTr = document.getElementById(ctrlTrId);")
			strSDCFunction.AppendLine("	var ctrlTxt = document.getElementById(ctrlTxtId);" & vbCrLf)
			strSDCFunction.AppendLine("	var ctrlLabel = document.getElementById(ctrlLabelId);" & vbCrLf)

			strSDCFunction.AppendLine("	var iId;")
			strSDCFunction.AppendLine("	var bShowOther = false;" & vbCrLf)

			strSDCFunction.AppendLine("	for (iId in aOthers) {")
			strSDCFunction.AppendLine("		if (ctrl && aOthers[iId] == ctrl.value) {")
			strSDCFunction.AppendLine("			ctrlLabel.innerHTML = aLabels[iId];")
			strSDCFunction.AppendLine("			if (aLabels[iId] == '') {")
			strSDCFunction.AppendLine("				ctrlLabel.style.display = 'none';")
			strSDCFunction.AppendLine("			} else { ")
			strSDCFunction.AppendLine("				ctrlLabel.style.display = '';")
			strSDCFunction.AppendLine("			}")
			strSDCFunction.AppendLine("			bShowOther = true;")
			strSDCFunction.AppendLine("		}")
			strSDCFunction.AppendLine("	}" & vbCrLf)

			strSDCFunction.AppendLine("	if (!ctrlTr) return;")
			strSDCFunction.AppendLine("	if (bShowOther == true) {")
			strSDCFunction.AppendLine("		ctrlTr.style.display = '';")
			strSDCFunction.AppendLine("	} else {")
			strSDCFunction.AppendLine("		ctrlTr.style.display = 'none';")
			strSDCFunction.AppendLine("	}" & vbCrLf)

			strSDCFunction.AppendLine("}" & vbCrLf)
			strSDCFunction.AppendLine("//-->")
			strSDCFunction.AppendLine("</script>")
			Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "ShowDiscoveryOther", strSDCFunction.ToString)
		End If

		If Not IsPostBack Then
			BindData()
			If IsNumeric(HowHeardId) AndAlso HowHeardId > 0 Then
				txtDiscoveryOther.Text = OtherText
				drpHowHeardList.SelectedValue = HowHeardId
			End If
		End If

		If Me.Width <> String.Empty Then
			drpHowHeardList.Attributes("style") = "width:" & Me.Width
			txtDiscoveryOther.Attributes("style") = "width:" & Me.Width
		End If

	End Sub

	Private Sub BindData()

		Dim dt As DataTable = HowHeardRow.GetAllHowHeard(DB)
		drpHowHeardList.DataSource = dt
		drpHowHeardList.DataValueField = "HowHeardId"
		drpHowHeardList.DataTextField = "HowHeard"
		drpHowHeardList.DataBind()
		drpHowHeardList.Items.Insert(0, New ListItem("-- please select --", ""))

	End Sub

	Protected Sub cvfDiscovery_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvfDiscovery.ServerValidate

		If drpHowHeardList.SelectedValue <> String.Empty Then

			If HowHeardRow.GetRow(DB, CInt(drpHowHeardList.SelectedValue)).IsUserInput AndAlso txtDiscoveryOther.Text = String.Empty Then
				cvfDiscovery.ErrorMessage = "Please input a """ & drpHowHeardList.SelectedItem.Text & """ value."
				args.IsValid = False
			Else
				args.IsValid = True
			End If

		End If

	End Sub

End Class
