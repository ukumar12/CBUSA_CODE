Imports System.Text.RegularExpressions
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports Components
Imports Controls.FCKEditor

Namespace Controls

#Region " Regular Validators "

    Public Class EmailValidator
        Inherits System.Web.UI.WebControls.RegularExpressionValidator

        Public Sub New()
            'MyBase.ValidationExpression = "^[A-Za-z0-9'#]([#_\.\-]?[a-zA-Z0-9']+)*\@([A-Za-z0-9\-]+\.)+[A-Za-z]{2,5}$"
            MyBase.ValidationExpression = "^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,30})$"
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Return MyBase.EvaluateIsValid()
        End Function
    End Class

    Public Class UserNameValidator
        Inherits System.Web.UI.WebControls.RegularExpressionValidator

        Public Sub New()
            MyBase.ValidationExpression = "^([^$\ ]+)$"
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Return MyBase.EvaluateIsValid()
        End Function
    End Class

    Public Class FloatValidator
        Inherits System.Web.UI.WebControls.RegularExpressionValidator

        Public Sub New()
            MyBase.ValidationExpression = "^(\+|\-)?([0-9]+)(((\.|\,)?([0-9]+))?)$"
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Return MyBase.EvaluateIsValid()
        End Function
    End Class

    Public Class IntegerValidator
        Inherits System.Web.UI.WebControls.RegularExpressionValidator

        Public Sub New()
            MyBase.ValidationExpression = "^([0-9]+)$"
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Return MyBase.EvaluateIsValid()
        End Function
    End Class

    Public Class PasswordValidator
        Inherits System.Web.UI.WebControls.RegularExpressionValidator

        Public Property MinLength() As Integer
            Get
                If ViewState("MinLength") Is Nothing Then ViewState("MinLength") = 4
                Return ViewState("MinLength")
            End Get
            Set(ByVal value As Integer)
                ViewState("MinLength") = value
            End Set
        End Property

        Protected Overrides Function EvaluateIsValid() As Boolean
            MyBase.ValidationExpression = "^(?=.*\d)(?=.*[a-zA-Z])(?!.*\s).{" & MinLength & ",}$"
            Return MyBase.EvaluateIsValid()
        End Function
    End Class

    Public Class URLValidator
        Inherits System.Web.UI.WebControls.RegularExpressionValidator

        Public Sub New()
            MyBase.ValidationExpression = "^http(s?)://([^$@\ ]+)$"
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Return MyBase.EvaluateIsValid()
        End Function
    End Class

    Public Class CustomURLValidator
        Inherits System.Web.UI.WebControls.RegularExpressionValidator

        Public Sub New()
            MyBase.ValidationExpression = "^(http(s?)://)?([^$@\ ]+)$"
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Return MyBase.EvaluateIsValid()
        End Function
    End Class

    Public Class RequiredTimeValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf FindControl(ControlToValidate) Is TimePicker Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isNotEmptyTime")
            End If
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim Text As String = CType(FindControl(ControlToValidate), TimePicker).Text
            Return Not String.Empty = Text
        End Function
    End Class

    Public Class TimeValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf FindControl(ControlToValidate) Is TimePicker Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)

            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isValidTime")
            End If
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim Text As String = CType(FindControl(ControlToValidate), TimePicker).Text
            If Text = String.Empty Then Return True

            Try
                Dim myTime As DateTime = Date.Parse(Text)
                Return True
            Catch ex As FormatException
                Return False
            End Try
        End Function
    End Class

    Public Class RequiredDateValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf FindControl(ControlToValidate) Is DatePicker Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isNotEmptyDate")
            End If
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim Text As String = CType(FindControl(ControlToValidate), DatePicker).Text
            Return Not String.Empty = Text
        End Function
    End Class

    Public Class DateValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Public Property MinYear() As Integer
            Get
                If ViewState("MinYear") Is Nothing Then ViewState("MinYear") = 0
                Return ViewState("MinYear")
            End Get
            Set(ByVal value As Integer)
                ViewState("MinYear") = value
            End Set
        End Property

        Public Property MinDate() As DateTime
            Get
                Return ViewState("MinDate")
            End Get
            Set(ByVal value As DateTime)
                ViewState("MinDate") = value
            End Set
        End Property

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf FindControl(ControlToValidate) Is DatePicker Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isValidDate")
            End If
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim Text As String = CType(FindControl(ControlToValidate), DatePicker).Text
            If Text = String.Empty Then Return True

            If Not IsDate(Text) Then
                Return False
            End If

            If MinYear > 0 Then
                Dim dt As DateTime = Date.Parse(Text)
                If Year(dt) < MinYear Then
                    Return False
                End If
            End If

            If MinDate <> Nothing Then
                Dim dt As DateTime = Date.Parse(Text)
                If dt < MinDate Then
                    Return False
                End If
            End If
            Return True
        End Function
    End Class

    Public Class RequiredFileUploadValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf FindControl(ControlToValidate) Is FileUpload Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isNotEmptyFile")
            End If
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim currentText As String = CType(FindControl(ControlToValidate), FileUpload).CurrentFileName
            Dim newText As String = CType(FindControl(ControlToValidate), FileUpload).NewFileName
            Dim bDelete As Boolean = CType(FindControl(ControlToValidate), FileUpload).MarkedToDelete

            If newText = String.Empty AndAlso currentText = String.Empty Then Return False
            If newText = String.Empty AndAlso bDelete Then Return False

            Return True
        End Function
    End Class

    Public Class FileUploadExtensionValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Private m_Extensions As String = "doc,txt,pdf,jpg,jpeg,gif,bmp,png"

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf FindControl(ControlToValidate) Is FileUpload Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Property Extensions() As String
            Get
                Return m_Extensions.ToLower
            End Get
            Set(ByVal value As String)
                If IsNothing(value) OrElse value = Nothing Then
                    m_Extensions = String.Empty
                Else
                    m_Extensions = value
                End If
            End Set
        End Property

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isValidFile")
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "extensions", Extensions)
            End If
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim FileName As String = CType(FindControl(ControlToValidate), FileUpload).NewFileName

            If FileName = String.Empty Then Return True

            Dim Extension As String = Replace(System.IO.Path.GetExtension(FileName).ToLower, ".", "")

            Dim aExtensions() As String = Extensions.Split(",")

            Return (Array.IndexOf(aExtensions, Extension, 0) <> -1)
        End Function
    End Class

    Public Class RequiredCheckboxListValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf FindControl(ControlToValidate) Is CheckBoxList Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isNotEmptyCheckBoxList")
            End If
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim ctrl As CheckBoxList = CType(FindControl(ControlToValidate), CheckBoxList)
            Dim IsEmpty As Boolean = True
            For Each li As ListItem In ctrl.Items
                If li.Selected Then
                    IsEmpty = False
                    Exit For
                End If
            Next
            Return Not IsEmpty
        End Function
    End Class

    Public Class RequiredCheckboxValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf FindControl(ControlToValidate) Is Checkbox Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isNotEmptyCheckbox")
            End If
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim ctrl As Checkbox = CType(FindControl(ControlToValidate), Checkbox)

            Return ctrl.Checked
        End Function
    End Class

    'Custom Classes
    Public Class CurrencyValidator
        Inherits System.Web.UI.WebControls.RegularExpressionValidator
        Public Sub New()
            MyBase.ValidationExpression = "^(([0-9]{1,3}(\,?[0-9]{3})*)|([0-9]{0,3}))(\.[0-9]{2})?$"
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Return MyBase.EvaluateIsValid()
        End Function
    End Class

    Public Class CustomCurrencyValidator
        Inherits System.Web.UI.WebControls.RegularExpressionValidator
        Public Sub New()
            MyBase.ValidationExpression = "^.*[\d]+.*$"
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Return MyBase.EvaluateIsValid()
        End Function
    End Class


    'Custom Validators
    Public Class RequiredPhoneValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf Me.FindControl(Me.ControlToValidate) Is Phone Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim sPhone As String = (CType(Me.FindControl(Me.ControlToValidate), Phone)).Value
            Return Not sPhone = String.Empty
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            If (Me.EnableClientScript) Then
                Me.ClientScript()
            End If
            MyBase.OnPreRender(e)
        End Sub

        Protected Sub ClientScript()
            Dim vs As Text.StringBuilder = New Text.StringBuilder()
            Dim ctrl As Control = FindControl(ControlToValidate)

            vs.Append("<script language=""javascript"">" & vbCrLf)
            vs.Append("function isNotEmptyPhone(val) {" & vbCrLf)
            vs.Append("   var aCtrl = val.controltovalidate.split(""_"");" & vbCrLf)
            vs.Append("   var CtrlId = aCtrl[aCtrl.length-1];" & vbCrLf)
            vs.Append("   var phone1 = document.getElementById(val.controltovalidate + '_' + CtrlId + '_PHONE1').value;" & vbCrLf)
            vs.Append("   var phone2 = document.getElementById(val.controltovalidate + '_' + CtrlId + '_PHONE2').value;" & vbCrLf)
            vs.Append("   var phone3 = document.getElementById(val.controltovalidate + '_' + CtrlId + '_PHONE3').value;" & vbCrLf & vbCrLf)
            vs.Append("    if ((phone1 == '' || phone1 == null) && (phone2 == '' || phone2 == null) && (phone3 == '' || phone3 == null)) { " & vbCrLf)
            vs.Append("        return false;" & vbCrLf & vbCrLf)
            vs.Append("    }" & vbCrLf)
            vs.Append("    return true;" & vbCrLf)
            vs.Append("}" & vbCrLf)
            vs.Append("</script>")

            If Not Me.Page.ClientScript.IsClientScriptBlockRegistered("isNotEmptyPhone") Then
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "isNotEmptyPhone", vs.ToString())
            End If
        End Sub

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isNotEmptyPhone")
            End If
        End Sub

    End Class

    Public Class PhoneValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf Me.FindControl(Me.ControlToValidate) Is Phone Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim sPhone As String = (CType(Me.FindControl(Me.ControlToValidate), Phone)).Value

            ' Return true if phone is empty
            If sPhone = String.Empty Then
                Return True
            End If

            Dim sPattern As String = "^\d{3}-\d{3}-\d{4}$"
            Return Regex.IsMatch(sPhone, sPattern, RegexOptions.IgnoreCase)
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            If (Me.EnableClientScript) Then
                Me.ClientScript()
            End If
            MyBase.OnPreRender(e)
        End Sub

        Protected Sub ClientScript()
            Dim vs As Text.StringBuilder = New Text.StringBuilder()
            vs.Append("<script language=""javascript"">" & vbCrLf)
            vs.Append("function isValidPhone(val) {" & vbCrLf)
            vs.Append("   var aCtrl = val.controltovalidate.split(""_"");" & vbCrLf)
            vs.Append("   var CtrlId = aCtrl[aCtrl.length-1];" & vbCrLf)
            vs.Append("   var phone1 = document.getElementById(val.controltovalidate + '_' + CtrlId + '_PHONE1').value;" & vbCrLf)
            vs.Append("   var phone2 = document.getElementById(val.controltovalidate + '_' + CtrlId + '_PHONE2').value;" & vbCrLf)
            vs.Append("   var phone3 = document.getElementById(val.controltovalidate + '_' + CtrlId + '_PHONE3').value;" & vbCrLf & vbCrLf)
            vs.Append("    if ((phone1 == '' || phone1 == null) && (phone2 == '' || phone2 == null) && (phone3 == '' || phone3 == null))" & vbCrLf)
            vs.Append("        return true;" & vbCrLf & vbCrLf)
            vs.Append("    var phone = phone1 + '-' + phone2 + '-' + phone3;" & vbCrLf)
            vs.Append("    regexp = /^\d{3}-\d{3}-\d{4}$/" & vbCrLf)
            vs.Append("    return regexp.test(phone);" & vbCrLf)
            vs.Append("}" & vbCrLf)
            vs.Append("</script>")

            If Not Me.Page.ClientScript.IsClientScriptBlockRegistered("isValidPhone") Then
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "isValidPhone", vs.ToString())
            End If
        End Sub

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isValidPhone")
            End If
        End Sub
    End Class

    Public Class CreditCardTypeValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf Me.FindControl(Me.ControlToValidate) Is System.Web.UI.WebControls.DropDownList Then
                If TypeOf Me.FindControl(CreditCardNumberControl) Is System.Web.UI.WebControls.TextBox Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim CardTypeListItem As ListItem = CType(FindControl(ControlToValidate), System.Web.UI.WebControls.DropDownList).SelectedItem
            Dim CreditCardNumber As String = CType(FindControl(CreditCardNumberControl), System.Web.UI.WebControls.TextBox).Text

            If CardTypeListItem.Value = String.Empty Then Return True
            If CreditCardNumber = String.Empty Then Return True

            Return isValidCreditCardType(CardTypeListItem.Text, CreditCardNumber)
        End Function

        Public Property CreditCardNumberControl() As String
            Get
                Return ViewState("CreditCardNumberControl")
            End Get
            Set(ByVal value As String)
                ViewState("CreditCardNumberControl") = value
            End Set
        End Property

        Private Function isValidCreditCardType(ByVal cctype As String, ByVal ccnum As String) As Boolean
            Dim iChkSum As Integer = 0

            cctype = cctype.ToLower
            ccnum = Regex.Replace(ccnum, "\D", "", RegexOptions.IgnoreCase)

            If ccnum(0) = "4" Then
                'VISA
                Return cctype.IndexOf("visa") >= 0
            ElseIf ccnum(0) = "3" Then
                'AmEx
                Return cctype.IndexOf("american") >= 0
            ElseIf ccnum(0) = "6" Then
                'Discover
                Return cctype.IndexOf("discover") >= 0
            ElseIf ccnum(0) = "5" Then
                'MasterCard
                Return cctype.IndexOf("mastercard") >= 0
            End If
            Return False
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            If Me.EnableClientScript Then
                ClientScript()
            End If
            MyBase.OnPreRender(e)
        End Sub

        Protected Sub ClientScript()
            Dim vs As New System.Text.StringBuilder
            vs.Append("<script language=""javascript"">" & vbCrLf)
            vs.Append("function isValidCreditCardType(val) {" & vbCrLf)
            vs.Append("	var ctrltype = document.getElementById(val.controltovalidate);" & vbCrLf)
            vs.Append("	var ctrlnum = document.getElementById(val.creditcardnumbercontrol);" & vbCrLf)
            vs.Append("	var cctype = ctrltype.options[ctrltype.selectedIndex].text;" & vbCrLf)
            vs.Append("	var ccnum = ctrlnum.value;" & vbCrLf & vbCrLf)
            vs.Append("	ccnum = ccnum.replace( /\D/g, """" );" & vbCrLf)
            vs.Append("	cctype = cctype.toLowerCase();" & vbCrLf)
            vs.Append("	if ( cctype == '') return true;" & vbCrLf)
            vs.Append("	if ( ccnum == '') return true;" & vbCrLf)
            vs.Append("	if ( ccnum.charAt(0) == '4' ){" & vbCrLf)
            vs.Append("	    return cctype.indexOf('visa') >= 0" & vbCrLf)
            vs.Append("	} else if ( ccnum.charAt(0) == '3') {" & vbCrLf)
            vs.Append("	    return cctype.indexOf('american') >= 0" & vbCrLf)
            vs.Append("	} else if ( ccnum.charAt(0) == '6') {" & vbCrLf)
            vs.Append("	    return cctype.indexOf('discover') >= 0" & vbCrLf)
            vs.Append("	} else if ( ccnum.charAt(0) == '5') {" & vbCrLf)
            vs.Append("	    return cctype.indexOf('mastercard') >= 0" & vbCrLf)
            vs.Append("	} " & vbCrLf)
            vs.Append("	return false;" & vbCrLf)
            vs.Append("}" & vbCrLf)
            vs.Append("</script>")

            If Not Me.Page.ClientScript.IsClientScriptBlockRegistered("isValidCreditCardType") Then
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "isValidCreditCardType", vs.ToString())
            End If
        End Sub

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "creditcardnumbercontrol", CreditCardNumberControl)
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isValidCreditCardType")
            End If
        End Sub
    End Class

    Public Class CreditCardValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf Me.FindControl(Me.ControlToValidate) Is System.Web.UI.WebControls.TextBox Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim Text As String = CType(Me.FindControl(Me.ControlToValidate), System.Web.UI.WebControls.TextBox).Text
            If Text = String.Empty Then Return True
            Return isCreditCardNumber(Text)
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            If Me.EnableClientScript Then
                Me.ClientScript()
            End If
            MyBase.OnPreRender(e)
        End Sub

        Protected Sub ClientScript()
            Dim vs As New System.Text.StringBuilder

            vs.Append("<script language=""javascript"">" & vbCrLf)
            vs.Append("function isCreditCardNumber(val) {" & vbCrLf)
            vs.Append("	var iChkSum=0;" & vbCrLf)
            vs.Append("	var ctrl = document.getElementById(val.controltovalidate);" & vbCrLf)
            vs.Append("	var ccnum = ctrl.value;" & vbCrLf & vbCrLf)

            vs.Append("	ccnum = ccnum.replace( /\D/g, """" );" & vbCrLf)
            vs.Append("	if (ccnum.length<13) return false;" & vbCrLf)
            vs.Append("    ccnumchk=new Array;" & vbCrLf)
            vs.Append("	for (iLoop=0; iLoop < ccnum.length; iLoop++) {" & vbCrLf)
            vs.Append("		ccnumchk[ccnum.length-1-iLoop] = ccnum.substring(iLoop, iLoop+1);" & vbCrLf)
            vs.Append("	}" & vbCrLf & vbCrLf)

            vs.Append("    var skemp=0;" & vbCrLf)
            vs.Append("	for (iLoop=0; iLoop < ccnum.length; iLoop++) {" & vbCrLf)
            vs.Append("        if (iLoop %2 != 0) {" & vbCrLf)
            vs.Append("			ccnumchk[iLoop]=ccnumchk[iLoop]*2;" & vbCrLf)
            vs.Append("			if (ccnumchk[iLoop] >= 10) ccnumchk[iLoop]=ccnumchk[iLoop]-9;" & vbCrLf)
            vs.Append("		}" & vbCrLf)
            vs.Append("		ccnumchk[iLoop]++;" & vbCrLf)
            vs.Append("		ccnumchk[iLoop]--;" & vbCrLf)
            vs.Append("		iChkSum = iChkSum + ccnumchk[iLoop].valueOf();" & vbCrLf)
            vs.Append("	}" & vbCrLf)
            vs.Append("	if (iChkSum%10 != 0) { return false; }" & vbCrLf)
            vs.Append("	return true;" & vbCrLf)
            vs.Append("}" & vbCrLf)
            vs.Append("</script>")

            If Not Me.Page.ClientScript.IsClientScriptBlockRegistered("isCreditCardNumber") Then
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "isCreditCardNumber", vs.ToString())
            End If
        End Sub

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isCreditCardNumber")
            End If
        End Sub

        Private Function isCreditCardNumber(ByVal ccnum As String) As Boolean
            Dim iChkSum = 0
            ccnum = Regex.Replace(ccnum, "\D", "", RegexOptions.IgnoreCase)

            'Check for correct card number length
            If ccnum.Length < 13 Then Return False

            If ccnum(0) = "4" Then
                'VISA
                If (ccnum.Length <> 13 And ccnum.Length <> 16) Then Return False
            ElseIf ccnum(0) = "3" Then
                'AmEx
                If ccnum.Length <> 15 Then Return False
            ElseIf ccnum(0) = "6" Then
                'Discover
                If ccnum.Length <> 16 Then Return False
            ElseIf ccnum(0) = "5" Then
                'MasterCard
                If ccnum.Length <> 16 Then Return False
            End If

            'Make an array and fill it with the individual digits of the cc number
            Dim ccnumchk() As Integer = New Integer(ccnum.Length) {}
            Dim iLoop As Integer
            For iLoop = 0 To ccnum.Length - 1
                ccnumchk(ccnum.Length - 1 - iLoop) = Int32.Parse(ccnum.Substring(iLoop, 1))
            Next

            'Perform the mathematical method (some base 10 stuff) to
            'convert the number to a two digit number
            For iLoop = 0 To ccnum.Length - 1
                'If splits an even number
                If iLoop Mod 2 <> 0 Then
                    ccnumchk(iLoop) = ccnumchk(iLoop) * 2
                    If ccnumchk(iLoop) >= 10 Then ccnumchk(iLoop) = ccnumchk(iLoop) - 9
                End If

                'Switch ccnumchk[splits] to a number
                ccnumchk(iLoop) = ccnumchk(iLoop) + 1
                ccnumchk(iLoop) = ccnumchk(iLoop) - 1

                iChkSum = iChkSum + ccnumchk(iLoop)
            Next

            If iChkSum Mod 10 <> 0 Then Return False 'The result isn't base 10

            Return True
        End Function
    End Class

    Public Class RequiredZipValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf Me.FindControl(Me.ControlToValidate) Is Zip Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim sZip As String = (CType(Me.FindControl(Me.ControlToValidate), Zip)).Value
            If sZip = String.Empty Then Return False
            Return True
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            If (Me.EnableClientScript) Then
                Me.ClientScript()
            End If
            MyBase.OnPreRender(e)
        End Sub

        Protected Sub ClientScript()
            Dim vs As Text.StringBuilder = New Text.StringBuilder()
            vs.Append("<script language=""javascript"">")
            vs.Append("function isNotEmptyZip(val) {" & vbCrLf)
            vs.Append("    var aCtrl = val.controltovalidate.split(""_"");" & vbCrLf)
            vs.Append("    var CtrlId = aCtrl[aCtrl.length-1];" & vbCrLf)
            vs.Append("    var z5 = document.getElementById(val.controltovalidate + '_' + CtrlId + '_ZIP5').value;" & vbCrLf)
            vs.Append("    var z4 = document.getElementById(val.controltovalidate + '_' + CtrlId + '_ZIP4').value;" & vbCrLf)
            vs.Append("    var z;" & vbCrLf & vbCrLf)
            vs.Append("    if (z5 == null || z5 == '') return false;" & vbCrLf)
            vs.Append("    return true;" & vbCrLf)
            vs.Append("}" & vbCrLf)
            vs.Append("</script>")

            If Not Me.Page.ClientScript.IsClientScriptBlockRegistered("isNotEmptyZip") Then
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "isNotEmptyZip", vs.ToString())
            End If
        End Sub

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isNotEmptyZip")
            End If
        End Sub
    End Class

    Public Class ZipValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf Me.FindControl(Me.ControlToValidate) Is Zip Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim sZip As String = (CType(Me.FindControl(Me.ControlToValidate), Zip)).Value
            Dim sPattern As String = "^\d{5}(-?\d{4})?$"
            Return sZip = String.Empty Or Regex.IsMatch(sZip, sPattern, RegexOptions.IgnoreCase)
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            If (Me.EnableClientScript) Then
                Me.ClientScript()
            End If
            MyBase.OnPreRender(e)
        End Sub

        Protected Sub ClientScript()
            Dim vs As Text.StringBuilder = New Text.StringBuilder()
            vs.Append("<script language=""javascript"">")
            vs.Append("function isValidZip(val) {" & vbCrLf)
            vs.Append("    var aCtrl = val.controltovalidate.split(""_"");" & vbCrLf)
            vs.Append("    var CtrlId = aCtrl[aCtrl.length-1];" & vbCrLf)
            vs.Append("    var z5 = document.getElementById(val.controltovalidate + '_' + CtrlId + '_ZIP5').value;" & vbCrLf)
            vs.Append("    var z4 = document.getElementById(val.controltovalidate + '_' + CtrlId + '_ZIP4').value;" & vbCrLf)
            vs.Append("    var z;" & vbCrLf & vbCrLf)
            vs.Append("    if ((z5 == null || z5 == '') && (z4 == null || z4 == ''))" & vbCrLf)
            vs.Append("        return true;" & vbCrLf & vbCrLf)
            vs.Append("    if (z4 == null || z4 == '') {" & vbCrLf)
            vs.Append("        z = z5;" & vbCrLf)
            vs.Append("    } else {" & vbCrLf)
            vs.Append("        z = z5 + '-' + z4;" & vbCrLf)
            vs.Append("    }" & vbCrLf)
            vs.Append("    regexp = /^\d{5}(-?\d{4})?$/" & vbCrLf)
            vs.Append("    return regexp.test(z);" & vbCrLf)
            vs.Append("}" & vbCrLf)
            vs.Append("</script>")

            If Not Me.Page.ClientScript.IsClientScriptBlockRegistered("isValidZip") Then
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "isValidZip", vs.ToString())
            End If
        End Sub

        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isValidZip")
            End If
        End Sub
    End Class

    ''' <summary>
    ''' Ensures that an CKEditor field is filled out on the site admin.
    ''' <seealso cref="CKEditor" />
    ''' </summary>
    Public Class RequiredCKValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        ''' <summary>
        ''' Returns whether the properties of the validator are set to valid values.
        ''' </summary>
        ''' <returns><see langword="True" /> if the control specified by <see cref="ControlToValidate" /> is a 
        ''' <see cref="CKEditor" /> control; <see langword="False" /> otherwise.</returns>
        ''' <remarks>This method requires the <see cref="ControlToValidate" /> to be in the same naming container 
        ''' as the <see cref="RequiredCKValidator" />.</remarks>
        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf FindControl(ControlToValidate) Is CKEditor Then
                Return True
            ElseIf TypeOf FindControl(ControlToValidate) Is CKHelper Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Adds the HTML and CSS attributes that need to be rendered to the control.
        ''' </summary>
        ''' <param name="writer">The <see cref="HtmlTextWriter" /> that receives the rendered output.</param>
        ''' <remarks>This method sets an attribute called "evaluationfunction" on the control to 
        ''' "isNotEmptyFCK" if client script is enabled.</remarks>
        Protected Overrides Sub AddAttributesToRender(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If (RenderUplevel And EnableClientScript) Then
                Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "isNotEmptyCK")
            End If
        End Sub

        ''' <summary>
        ''' Evaluates whether the control specified by <see cref="ControlToValidate" /> is filled out.
        ''' </summary>
        ''' <returns><see langword="True" /> if the CKeditor field is filled out; 
        ''' <see langword="False" /> otherwise.</returns>
        ''' <remarks><para>This method checks the value of 
        ''' <see cref="CKEditor.Value">CKEditor.Value</see> to determine if the field is filled out.</para>
        ''' <para>This method requires the <see cref="ControlToValidate" /> to be in the same naming container as
        ''' the <see cref="RequiredCKValidator" />.</para></remarks>
        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim Text As String = String.Empty
            Dim c As Control = FindControl(ControlToValidate)

            If TypeOf c Is CKEditor Then
                Text = CType(c, CKEditor).Value
            ElseIf TypeOf c Is CKHelper Then
                Text = CType(c, CKHelper).Value
            End If

            Return Not String.Empty = Text
        End Function

    End Class


#End Region

#Region " Front End Validators "

    Public Class FrontValidator
        Private m_Page As Page
        Private m_Validator As BaseValidator
        Private m_ControlToValidate As String
        Private m_Bar As HtmlControl
        Private m_Label As HtmlControl
        Private m_Viewstate As StateBag
		Private m_BarID As String = Nothing
		Private m_LabelID As String = Nothing

        Sub New()
        End Sub

        Sub New(ByVal p As BasePage, ByVal c As WebControl, ByVal s As StateBag, ByVal v As String)
            m_Page = p
            m_Validator = c
            m_ControlToValidate = v
            m_Viewstate = s
        End Sub

		Public Property BarID() As String
			Get
                If m_BarID = String.Empty Then
                    m_BarID = "bar" & ControlToValidate
                End If
				Return m_BarID
			End Get
			Set(ByVal value As String)
				m_BarID = value
			End Set
		End Property

		Public Property LabelID() As String
			Get
                If m_LabelID = String.Empty Then
                    m_LabelID = "label" & ControlToValidate
                End If
				Return m_LabelID
			End Get
			Set(ByVal value As String)
				m_LabelID = value
			End Set
		End Property

        Private Property Bar() As HtmlControl
            Get
                Return m_Bar
            End Get
            Set(ByVal value As HtmlControl)
                m_Bar = value
            End Set
        End Property

        Private Property Label() As HtmlControl
            Get
                Return m_Label
            End Get
            Set(ByVal value As HtmlControl)
                m_Label = value
            End Set
        End Property

        Private Property Viewstate() As StateBag
            Get
                Return m_Viewstate
            End Get
            Set(ByVal value As StateBag)
                m_Viewstate = value
            End Set
        End Property

        Public Sub Initialize()
            Bar = GetBar()
            Label = GetLabel()

            Validator.EnableClientScript = False
            Validator.Display = ValidatorDisplay.None

            If Not Bar Is Nothing Then
                If Not p.IsPostBack Then
                    Viewstate("orig" + Bar.ID) = Bar.Attributes("class")
                End If
                Bar.Attributes("class") = Viewstate("orig" + Bar.ID)
            End If
            If Not Label Is Nothing Then
                If Not p.IsPostBack Then
                    Viewstate("orig" + Label.ID) = Label.Attributes("class")
                End If
                Label.Attributes("class") = Viewstate("orig" + Label.ID)
            End If
        End Sub

        Public Property p() As Page
            Get
                Return m_Page
            End Get
            Set(ByVal value As Page)
                m_Page = value
            End Set
        End Property

        Public Property Validator() As BaseValidator
            Get
                Return m_Validator
            End Get
            Set(ByVal value As BaseValidator)
                m_Validator = value
            End Set
        End Property

        Public Property ControlToValidate() As String
            Get
                Return m_ControlToValidate
            End Get
            Set(ByVal value As String)
                m_ControlToValidate = value
            End Set
        End Property

        Private Function GetBar() As HtmlControl
            Dim m_Bar As HtmlControl = Nothing
			Dim Bar As String = BarID
            If TypeOf Validator.FindControl(Bar) Is HtmlGenericControl Then
                m_Bar = (CType(Validator.FindControl(Bar), HtmlGenericControl))
            End If
            If TypeOf Validator.FindControl(Bar) Is HtmlTableCell Then
                m_Bar = (CType(Validator.FindControl(Bar), HtmlTableCell))
            End If
            Return m_Bar
        End Function

        Private Function GetLabel() As HtmlControl
            Dim m_Label As HtmlControl = Nothing
			Dim Label As String = LabelID
            If TypeOf Validator.FindControl(Label) Is HtmlGenericControl Then
                m_Label = (CType(Validator.FindControl(Label), HtmlGenericControl))
            End If
            If TypeOf Validator.FindControl(Label) Is HtmlTableCell Then
                m_Label = (CType(Validator.FindControl(Label), HtmlTableCell))
            End If
            Return m_Label
        End Function

        Public Sub ChangeStyles()
            If Not Bar Is Nothing Then
                If Not Viewstate("orig" + Bar.ID) Is Nothing Then
                    Bar.Attributes("class") = Viewstate("orig" + Bar.ID).ToString().Replace("req", "red")
                End If
            End If
            If Not Label Is Nothing Then
                Label.Attributes("class") = "fielderror"
            End If
        End Sub
    End Class

    Public Class RequiredFieldValidatorFront
        Inherits System.Web.UI.WebControls.RequiredFieldValidator

        Private validator As FrontValidator
		Private m_BarID As String = Nothing
		Private m_LabelID As String = Nothing

		Public Property BarID() As String
			Get
				Return m_BarID
			End Get
			Set(ByVal value As String)
				m_BarID = value
			End Set
		End Property

		Public Property LabelID() As String
			Get
				Return m_LabelID
			End Get
			Set(ByVal value As String)
				m_LabelID = value
			End Set
		End Property

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            If Not BarID = String.Empty Then validator.BarID = BarID
            If Not LabelID = String.Empty Then validator.LabelID = LabelID
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function

    End Class

    Public Class IntegerValidatorFront
        Inherits IntegerValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function

    End Class

	Public Class FloatValidatorFront
		Inherits FloatValidator

		Private validator As FrontValidator

		Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
			MyBase.OnLoad(e)

			validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
			validator.Initialize()
		End Sub

		Protected Overrides Function EvaluateIsValid() As Boolean
			If MyBase.EvaluateIsValid() Then
				Return True
			Else
				If Enabled Then validator.ChangeStyles()
				Return False
			End If
		End Function

	End Class

    Public Class DateValidatorFront
        Inherits DateValidator
        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

	Public Class FileUploadExtensionValidatorFront
		Inherits FileUploadExtensionValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class RequiredExpDateValidatorFront
        Inherits RequiredExpDateValidator
        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class ExpDateValidatorFront
        Inherits ExpDateValidator
        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class RequiredDateValidatorFront
        Inherits RequiredDateValidator
        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class PhoneValidatorFront
        Inherits PhoneValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class RequiredExpDateValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
        End Sub

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf FindControl(ControlToValidate) Is ExpDate Then Return True Else Return False
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim Value As String = CType(Me.FindControl(Me.ControlToValidate), ExpDate).Value
            If Value = String.Empty Then Return False
            Return True
        End Function
    End Class

    Public Class ExpDateValidator
        Inherits System.Web.UI.WebControls.BaseValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
        End Sub

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf FindControl(ControlToValidate) Is ExpDate Then Return True Else Return False
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim bInvalid As Boolean = False
            Dim sDate As String = CType(Me.FindControl(Me.ControlToValidate), ExpDate).Value
            If (sDate = String.Empty) Then
                Return True
            ElseIf DateDiff(DateInterval.Month, Convert.ToDateTime(sDate), Now) > 0 Then
                bInvalid = True
            End If

            If bInvalid = False Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    Public Class RequiredZipValidatorFront
        Inherits ZipValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim sZip As String = CType(Me.FindControl(Me.ControlToValidate), Zip).Value
            If MyBase.EvaluateIsValid() And sZip <> "" Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class ZipValidatorFront
        Inherits ZipValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class EmailValidatorFront
        Inherits EmailValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class PasswordValidatorFront
        Inherits PasswordValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class CompareValidatorFront
        Inherits CompareValidator

        Private validator As FrontValidator
        Private validator2 As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
            validator2 = New FrontValidator(Page, Me, ViewState, ControlToCompare)
            validator2.Initialize()
        End Sub

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                If Enabled Then validator2.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class CustomValidatorFront
        Inherits CustomValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function ControlPropertiesValid() As Boolean
            Return True
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class RequiredCheckboxListValidatorFront
        Inherits RequiredCheckboxListValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function ControlPropertiesValid() As Boolean
            Return True
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class RequiredCheckboxValidatorFront
        Inherits RequiredCheckboxValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function ControlPropertiesValid() As Boolean
            Return True
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class CreditCardValidatorFront
        Inherits CreditCardValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function ControlPropertiesValid() As Boolean
            Return True
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

    Public Class CreditCardTypeValidatorFront
        Inherits CreditCardTypeValidator

        Private validator As FrontValidator

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            validator = New FrontValidator(Page, Me, ViewState, ControlToValidate)
            validator.Initialize()
        End Sub

        Protected Overrides Function ControlPropertiesValid() As Boolean
            Return True
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            If MyBase.EvaluateIsValid() Then
                Return True
            Else
                If Enabled Then validator.ChangeStyles()
                Return False
            End If
        End Function
    End Class

#End Region

End Namespace
