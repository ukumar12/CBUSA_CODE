Imports System.Web
Imports System.Web.UI
Imports System.Collections.Generic
Imports Components
Imports System.Text
Imports System.Text.RegularExpressions

Namespace Controls
    Public Class EditTable
        Inherits HtmlControls.HtmlTable
        Implements IScriptControl
        Implements IPostBackDataHandler

        Public Sub New()
            MyBase.New()
            m_Fields = New List(Of EditTableField)
            m_Validators = New List(Of String)
        End Sub

        Public Sub Add(ByVal Name As String, ByVal Html As String, ByVal Validator As String, Optional ByVal DefaultValue As String = "", Optional ByVal NormalClass As String = "etnormal", Optional ByVal ErrorClass As String = "eterror")
            Dim f As New EditTableField()
            f.name = Name
            f.html = Html
            f.defaultValue = DefaultValue
            f.normalClass = NormalClass
            f.errorClass = ErrorClass
            Fields.Add(f)
            Validators.Add(Validator)
        End Sub

        Public Sub AddSelect(ByVal Name As String, ByVal Options As IDictionary(Of String, String), Optional ByVal IsRequired As Boolean = True, Optional ByVal ClassName As String = "", Optional ByVal DefaultValue As String = "")
            Dim html As New StringBuilder()
            html.Append("<select name=""" & Name & """")
            If ClassName <> String.Empty Then
                html.Append(" class=""" & ClassName & """")
            End If
            html.Append(" >")
            For Each o As String In Options.Keys
                Dim s As String = IIf(DefaultValue = Options(o), " selected=""selected""", String.Empty)
                html.Append("<option value=""" & Options(o) & """ " & s & " >" & o & "</option>")
            Next
            html.Append("</select>")
            Add(Name, html.ToString, IIf(IsRequired, "function(c) {return getListValue(c) != '" & DefaultValue & "'}", "function(c) {return true}"), DefaultValue)
        End Sub

        Public Sub AddText(ByVal Name As String, Optional ByVal MaxLength As Integer = 0, Optional ByVal Validator As String = "function(c) {return !isEmptyField(c)}", Optional ByVal ClassName As String = "", Optional ByVal DefaultValue As String = "")
            Dim html As New StringBuilder()
            html.Append("<input type=""text"" name=""" & Name & """")
            If ClassName <> String.Empty Then html.Append(" class=""" & ClassName & """")
            If DefaultValue <> String.Empty Then html.Append(" value=""" & DefaultValue & """")
            If MaxLength <> 0 Then html.Append(" maxlength=""" & MaxLength & """")
            html.Append(" />")
            Add(Name, html.ToString, Validator, DefaultValue)
        End Sub

        Public Sub AddRadioGroup(ByVal Name As String, ByVal Options As IDictionary(Of String, String), Optional ByVal IsRequired As Boolean = True, Optional ByVal ClassName As String = "", Optional ByVal DefaultValue As String = "", Optional ByVal Spacer As String = "<br/>")
            Dim html As New StringBuilder()
            For Each o As String In Options.Keys
                html.Append("<input type=""radio"" name=""" & Name & """ value=""" & Options(o) & """")
                If DefaultValue = Options(o) Then html.Append(" selected=""selected""")
                html.Append(" />")
                If ClassName <> String.Empty Then
                    html.Append("<span class=""" & ClassName & """>" & o & "</span>")
                Else
                    html.Append(o)
                End If
                html.Append(Spacer)
            Next
            Add(Name, html.ToString, IIf(IsRequired, "function(c) {for(var i=0;i<c.length;i++) if(c[i].checked) return true;return false;}", "function(c) {return true}"))
        End Sub

        Public Sub AddEmail(ByVal Name As String, Optional ByVal MaxLength As Integer = 0, Optional ByVal IsRequired As Boolean = True, Optional ByVal ClassName As String = "", Optional ByVal DefaultValue As String = "")
            AddText(Name, MaxLength, IIf(IsRequired, "function(c) {return isEmail(c)}", "function(c) {return isEmptyField(c)||isEmail(c)}"), ClassName, DefaultValue)
        End Sub

        Public Sub AddPhone(ByVal Name As String, Optional ByVal IsRequired As Boolean = True, Optional ByVal ClassName As String = "", Optional ByVal DefaultValue As String = "")
            AddText(Name, 20, IIf(IsRequired, "function(c) {return isPhone(c)}", "return isEmptyField(c)||isPhone(c)}"), ClassName, DefaultValue)
        End Sub

        'Public Sub AddCheckboxGroup(ByVal Name As String, ByVal Options As IDictionary(Of String, String), Optional ByVal IsRequired As Boolean = True, Optional ByVal ClassName As String = "", Optional ByVal DefaultList As String = "", Optional ByVal Spacer As String = "<br/>")
        '    Dim html As New StringBuilder()
        '    For Each o As String In Options.Keys
        '        html.Length = 0
        '        html.Append("<input type=""checkbox"" name=""" & o & """ value=""" & Options(o) & """")
        '        If Regex.IsMatch(DefaultList, "(?:^|,)" & Options(o) & "(?:$|,)") Then
        '            html.Append(" checked=""checked""")
        '        End If
        '        html.Append(" />")
        '        If ClassName <> String.Empty Then
        '            html.Append("<span class=""" & ClassName & """>" & o & "</span>")
        '        Else
        '            html.Append(o)
        '        End If
        '        html.Append(Spacer)
        '    Next


        '    For Each o As String In Options.Keys
        '        html.Append("<input type=""checkbox"" value=""" & Options(o) & """")
        '        If Regex.IsMatch(DefaultList, "(?:^|,)" & Options(o) & "(?:$|,)") Then
        '            html.Append(" checked=""checked""")
        '        End If
        '        html.Append(" />")
        '        If ClassName <> String.Empty Then
        '            html.Append("<span class=""" & ClassName & """>" & Options(o) & "</span")
        '        Else
        '            html.Append(Options(o))
        '        End If
        '        html.Append(Spacer)
        '    Next
        'End Sub


        Private m_Fields As List(Of EditTableField)
        Private Property Fields() As List(Of EditTableField)
            Get
                Return m_Fields
            End Get
            Set(ByVal value As List(Of EditTableField))
                m_Fields = value
            End Set
        End Property

        Private m_Validators As List(Of String)
        Private Property Validators() As List(Of String)
            Get
                Return m_Validators
            End Get
            Set(ByVal value As List(Of String))
                m_Validators = value
            End Set
        End Property


        Private m_className As String
        Public Property ClassName() As String
            Get
                Return m_className
            End Get
            Set(ByVal value As String)
                m_className = value
            End Set
        End Property

        Private m_IsAjax As Boolean = False
        Public Property IsAjax() As Boolean
            Get
                Return m_IsAjax
            End Get
            Set(ByVal value As Boolean)
                m_IsAjax = value
            End Set
        End Property

        Private m_MinCount As Integer = 0
        Public Property MinCount() As Integer
            Get
                Return m_MinCount
            End Get
            Set(ByVal value As Integer)
                m_MinCount = value
            End Set
        End Property

        Private m_dt As DataTable
        Public ReadOnly Property Data() As DataTable
            Get
                If m_dt Is Nothing Then
                    m_dt = New DataTable
                    Dim c As New DataColumn("RowId", GetType(Integer))
                    m_dt.Columns.Add(c)
                    For Each f As EditTableField In m_Fields
                        c = New DataColumn(f.name, GetType(String))
                        m_dt.Columns.Add(c)
                    Next
                End If
                Return m_dt
            End Get
        End Property

        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            'Dim s As New ScriptBehaviorDescriptor("FourSale.EditTable", targetControl.ClientID)
            Dim s As New ScriptControlDescriptor("FourSale.EditTable", Me.ClientID)
            Dim sFields As String = "["
            Dim sConn As String = ""
            Dim js As New System.Web.Script.Serialization.JavaScriptSerializer()
            For Each field As EditTableField In Fields
                sFields &= sConn & js.Serialize(field)
                sConn = ","
            Next
            sFields &= "]"
            s.AddScriptProperty("fields", sFields)
            s.AddScriptProperty("validators", "[" & String.Join(",", Validators.ToArray) & "]")
            s.AddProperty("minCount", MinCount)
            s.AddProperty("isAjax", IsAjax)

            Return New ScriptDescriptor() {s}
        End Function

        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim a As New List(Of ScriptReference)
            a.Add(New ScriptReference("/includes/EditTable.js"))
            Return a.ToArray
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
            sm.RegisterScriptControl(Of EditTable)(Me)
            MyBase.Attributes.Add("name", UniqueID)
            MyBase.OnPreRender(e)
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
            sm.RegisterScriptDescriptors(Me)
            MyBase.Render(writer)
        End Sub

        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            Dim r As New Regex(Regex.Escape(UniqueID) & "\$tr([\d]+)\$td([\d]+)\$(.*)$")
            If postCollection(postDataKey) Is Nothing Then
                Return False
            End If
            For Each key As String In postCollection.Keys
                Dim m As Match = r.Match(key)
                If m IsNot Nothing AndAlso m.Success Then
                    Dim row As Integer = m.Groups(1).Value
                    Dim field As String = m.Groups(3).Value
                    Dim dr As DataRow() = Data.Select("RowId=" & row)
                    If dr.Length = 0 Then
                        Array.Resize(Of DataRow)(dr, 1)
                        dr(0) = Data.NewRow
                        dr(0)("RowId") = row
                        Data.Rows.Add(dr(0))
                    End If
                    dr(0)(field) = postCollection(key)
                End If
            Next
        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent

        End Sub
    End Class

    <Serializable()> _
    Public Class EditTableField
        Public name As String
        Public html As String
        Public normalClass As String
        Public errorClass As String
        Public defaultValue As String
    End Class

End Namespace