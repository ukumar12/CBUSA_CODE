Imports Components
Imports DataLayer
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections.Generic

Namespace Controls
    Public Class SubForm
        Inherits CompositeControl
        Implements IScriptControl
        Implements INamingContainer

#Region "Events"
        Public Class SubFormEventArguments
            Public Sub New(ByVal row As DataRow)
                DataRow = row
            End Sub
            Public DataRow As DataRow
            Public IsValid As Boolean
        End Class

        Public Delegate Sub SubFormEvent(ByVal sender As Object, ByVal args As SubFormEventArguments)
        Public Event ValidateRow As SubFormEvent
#End Region

#Region "Properties"
        Private m_InitialRows As Integer = 1
        Public Property InitialRows() As Integer
            Get
                Return m_InitialRows
            End Get
            Set(ByVal value As Integer)
                m_InitialRows = value
            End Set
        End Property

        Private m_InvalidRows As Generic.List(Of Integer)
        Public ReadOnly Property InvalidRows() As Generic.List(Of Integer)
            Get
                If m_InvalidRows Is Nothing Then m_InvalidRows = New Generic.List(Of Integer)
                Return m_InvalidRows
            End Get
        End Property


        Private m_Fields As List(Of SubFormField)
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property Fields() As List(Of SubFormField)
            Get
                Return m_Fields
            End Get
            Set(ByVal value As List(Of SubFormField))
                m_Fields = value
            End Set
        End Property

        Private m_OnRowAdded As String
        Public Property OnRowAdded() As String
            Get
                Return m_OnRowAdded
            End Get
            Set(ByVal value As String)
                m_OnRowAdded = value
            End Set
        End Property

        Private m_Data As DataView
        Public Property Data() As DataView
            Get
                Return m_Data
            End Get
            Set(ByVal value As DataView)
                m_Data = value
            End Set
        End Property

        Private m_AddRowMode As SubformAddRowMode
        Public Property AddRowMode() As SubformAddRowMode
            Get
                Return m_AddRowMode
            End Get
            Set(ByVal value As SubformAddRowMode)
                m_AddRowMode = value
            End Set
        End Property
#End Region

#Region "Methods"
        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim tbl As New ScriptControlDescriptor("AE.SubForm", ClientID)
            Dim sFields As String = String.Empty
            Dim conn As String = ""
            For Each f As SubFormField In Fields
                sFields &= conn & "'" & f.ID & "'"
                conn = ","
            Next
            If sFields <> String.Empty Then
                tbl.AddScriptProperty("fields", "[" & sFields & "]")
            End If
            If Data IsNot Nothing Then
                Dim sData As String = String.Empty
                conn = ""
                For Each row As DataRowView In Data
                    Dim sRow As String = String.Empty
                    Dim rowConn As String = ""
                    For Each col As DataColumn In Data.Table.Columns
                        If Not IsDBNull(row(col.ColumnName)) Then
                            sRow &= rowConn & "'" & col.ColumnName & "':'" & row(col.ColumnName) & "'"
                            rowConn = ","
                        Else
                            sRow &= rowConn & "null"
                            rowConn = ","
                        End If
                    Next
                    If sRow <> String.Empty Then
                        sData &= conn & "{" & sRow & "}"
                        conn = ","
                    End If
                Next
                If sData <> String.Empty Then tbl.AddScriptProperty("initData", "[" & sData & "]")
            End If
            If OnRowAdded <> Nothing Then tbl.AddScriptProperty("OnRowAdded", OnRowAdded)
            Dim postbackIds As String = GetPostbackIds(Page)
            If postbackIds <> String.Empty Then
                tbl.AddProperty("pbBtns", postbackIds)
            End If
            If AddRowMode = SubformAddRowMode.Button Then
                tbl.AddProperty("showAddBtn", True)
            End If
            'For Each f As SubFormField In Fields
            '    tbl.AddComponentProperty("field", f.ID)
            'Next
            tbl.AddProperty("initRows", InitialRows)
            Return New ScriptDescriptor() {tbl}
        End Function

        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim a As New ScriptReference("/includes/controls/SubForm.js")
            Return New ScriptReference() {a}
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
            sm.RegisterScriptControl(Of SubForm)(Me)

            MyBase.OnPreRender(e)
        End Sub

        Protected Overrides Sub CreateChildControls()
            For Each f As SubFormField In Fields
                Controls.Add(f)
            Next
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            If Not Page.ClientScript.IsClientScriptBlockRegistered("InitSubformFields") Then
                Dim s As String = String.Empty
                For Each f As SubFormField In Fields
                    Dim descriptors As ScriptDescriptor() = f.GetScriptDescriptors()
                    For Each sd As ModifiedScriptDescriptor In descriptors
                        If sd IsNot Nothing Then
                            s &= sd.GetScriptPublic()
                        End If
                    Next
                Next

                Page.ClientScript.RegisterStartupScript(Me.GetType, "InitSubformFields", s, True)
            End If

            Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
            sm.RegisterScriptDescriptors(Me)

            writer.AddAttribute("id", ClientID)
            writer.AddAttribute("name", UniqueID)
            writer.RenderBeginTag("table")
            writer.RenderEndTag()
        End Sub

        Public Function GetData() As DataRow()
            Dim dt As New DataTable()
            Dim dc As DataColumn

            Dim re As New Text.RegularExpressions.Regex("(?<=tr)[\d]+", Text.RegularExpressions.RegexOptions.IgnoreCase)
            Dim req As HttpRequest = HttpContext.Current.Request

            dc = New DataColumn("RowIndex", GetType(Integer))
            dt.Columns.Add(dc)

            For Each f As SubFormField In Fields
                For Each i As SubFormInput In f.Inputs
                    dc = New DataColumn(i.ServerId, GetType(String))
                    dt.Columns.Add(dc)
                    For Each key As String In req.Form.Keys
                        If key.Contains(i.ServerId & i.UniqueIdSuffix) Then
                            Dim m As Text.RegularExpressions.Match = re.Match(key.Replace("$", "\$"))
                            If m.Success Then
                                Dim row As Integer = m.Value
                                Dim aRows As DataRow() = dt.Select("RowIndex=" & row)
                                If aRows.Length = 0 Then
                                    Dim newRow As DataRow = dt.NewRow
                                    newRow("RowIndex") = row
                                    dt.Rows.Add(newRow)
                                    ReDim aRows(0)
                                    aRows(0) = newRow
                                End If
                                Dim dr As DataRow = aRows(0)
                                dr(i.ServerId) = req.Form(key)
                            End If
                        End If
                    Next
                Next
            Next

            Dim out As DataRow() = dt.Select("", "RowIndex ASC")

            Return out
        End Function

        Public Function Validate(Optional ByVal ValidateLastRow As Boolean = False) As Boolean
            Dim data As DataRow() = GetData()
            Dim IsValid As Boolean = True
            For i As Integer = 0 To data.Length - 1
                Dim args As New SubFormEventArguments(data(i))
                RaiseEvent ValidateRow(Me, args)
                If i < data.Length - 1 Or ValidateLastRow Then
                    IsValid = IsValid And args.IsValid
                End If
                If Not args.IsValid Then
                    InvalidRows.Add(i)
                End If
            Next
            Return IsValid
        End Function

        Private Function GetPostbackIds(ByVal parent As Control) As String
            Dim children As String = String.Empty
            For Each ctl As Control In parent.Controls
                Dim tmp = GetPostbackIds(ctl)
                If tmp <> String.Empty Then
                    children &= IIf(children.Length = 0, tmp, "," & tmp)
                End If
            Next
            If TypeOf parent Is IButtonControl Then
                Return IIf(children.Length = 0, parent.ClientID, "," & parent.ClientID)
            Else
                Return children
            End If
        End Function
#End Region
    End Class

    Public Class SubFormField
        Inherits CompositeControl

        Public Event FieldDataBound As EventHandler

        Public Sub New()
            m_Inputs = New List(Of SubFormInput)
        End Sub

        Private m_InvalidClass As String
        Public Property InvalidClass()
            Get
                Return m_InvalidClass
            End Get
            Set(ByVal value)
                m_InvalidClass = value
            End Set
        End Property

        Private m_ValidClass As String
        Public Property ValidClass() As String
            Get
                Return m_ValidClass
            End Get
            Set(ByVal value As String)
                m_ValidClass = value
            End Set
        End Property

        Private m_FieldName As String
        Public Property FieldName() As String
            Get
                Return m_FieldName
            End Get
            Set(ByVal value As String)
                m_FieldName = value
            End Set
        End Property

        Private m_HtmlTemplate As ITemplate
        <TemplateContainer(GetType(SubFormField))> _
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Property HtmlTemplate() As ITemplate
            Get
                Return m_HtmlTemplate
            End Get
            Set(ByVal value As ITemplate)
                m_HtmlTemplate = value
            End Set
        End Property

        Private m_Inputs As List(Of SubFormInput)
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public Overridable Property Inputs() As List(Of SubFormInput)
            Get
                Return m_Inputs
            End Get
            Set(ByVal value As List(Of SubFormInput))
                m_Inputs = value
            End Set
        End Property

        Protected Overridable Function GetTemplateHtml() As String
            Dim out As New System.Text.StringBuilder
            Dim sw As New System.IO.StringWriter(out)
            Dim html As New HtmlTextWriter(sw)

            RenderContents(html)
            Return out.ToString
        End Function

        Protected Overrides Sub CreateChildControls()
            HtmlTemplate.InstantiateIn(Me)
            For Each c As Control In Controls
                If TypeOf c Is ISubFormScriptControl Then
                    CType(c, ISubFormScriptControl).RenderScript = False
                End If
            Next
            RaiseEvent FieldDataBound(Me, EventArgs.Empty)
        End Sub

        Public Overridable Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of ModifiedScriptDescriptor)
            EnsureChildControls()

            Dim s As New ModifiedScriptDescriptor("AE.SubFormTemplate")
            s.AddProperty("fieldName", FieldName)
            s.AddProperty("html", GetTemplateHtml)
            s.AddProperty("idPrefix", Me.ClientID)
            s.AddProperty("namePrefix", Me.UniqueID)
            Dim jsonInputs As String = String.Empty
            Dim conn As String = "["
            For Each i As SubFormInput In Inputs
                Dim c As Control = FindControl(i.ServerId)
                jsonInputs &= conn & "{'id':'" & c.ClientID & i.ClientIdSuffix & "'"
                If i.ValidateFunction <> Nothing Then
                    jsonInputs &= ",'validate':" & i.ValidateFunction
                End If
                If TypeOf c Is ISubFormScriptControl Then
                    jsonInputs &= ",'create':'" & CType(c, ISubFormScriptControl).GetScript.Replace("'", "\'") & "'"
                End If
                If i.IsDefaultFunction <> Nothing Then
                    jsonInputs &= ",'isDefault':" & i.IsDefaultFunction
                End If
                If i.SetValueFunction <> Nothing Then
                    jsonInputs &= ",'setValue':" & i.SetValueFunction
                End If
                If i.IsDataField Then
                    jsonInputs &= ",'isDataField':true"
                End If
                If i.ErrorSpanId <> Nothing Then
                    jsonInputs &= ",'errorSpan':'" & i.ErrorSpanId & "'"
                End If
                jsonInputs &= "}"
                conn = ","
            Next
            If jsonInputs <> String.Empty Then
                jsonInputs &= "]"
                s.AddScriptProperty("inputs", jsonInputs)
            End If
            s.AddProperty("validClass", "etnormal")
            s.AddProperty("invalidClass", "eterror")
            s.AddProperty("id", Me.ID)
            Return New ModifiedScriptDescriptor() {s}
        End Function
    End Class

    Public Enum SubFormAddRowMode
        Auto
        Button
    End Enum

    Public Class SubFormInput
        Private m_IsDataField As Boolean
        Public Property IsDataField() As Boolean
            Get
                Return m_IsDataField
            End Get
            Set(ByVal value As Boolean)
                m_IsDataField = value
            End Set
        End Property

        Private m_ClientIdSuffix As String
        Public Property ClientIdSuffix() As String
            Get
                Return m_ClientIdSuffix
            End Get
            Set(ByVal value As String)
                m_ClientIdSuffix = value
            End Set
        End Property

        Private m_UniqueIdSuffix As String
        Public Property UniqueIdSuffix() As String
            Get
                Return m_UniqueIdSuffix
            End Get
            Set(ByVal value As String)
                m_UniqueIdSuffix = value
            End Set
        End Property

        Private m_ServerId As String
        Public Property ServerId() As String
            Get
                Return m_ServerId
            End Get
            Set(ByVal value As String)
                m_ServerId = value
            End Set
        End Property

        Private m_ValidateFunction As String
        Public Property ValidateFunction() As String
            Get
                Return m_ValidateFunction
            End Get
            Set(ByVal value As String)
                m_ValidateFunction = value
            End Set
        End Property

        Private m_IsDefaultFunction As String
        Public Property IsDefaultFunction() As String
            Get
                Return m_IsDefaultFunction
            End Get
            Set(ByVal value As String)
                m_IsDefaultFunction = value
            End Set
        End Property

        Private m_SetValueFunction As String
        Public Property SetValueFunction() As String
            Get
                Return m_SetValueFunction
            End Get
            Set(ByVal value As String)
                m_SetValueFunction = value
            End Set
        End Property

        Private m_ErrorSpanId As String
        Public Property ErrorSpanId() As String
            Get
                Return m_ErrorSpanId
            End Get
            Set(ByVal value As String)
                m_ErrorSpanId = value
            End Set
        End Property
    End Class

    Public Class SubFormValidator
        Inherits BaseValidator

        Protected Overrides Function ControlPropertiesValid() As Boolean
            If TypeOf Page.FindControl(ControlToValidate) Is SubForm Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides Function EvaluateIsValid() As Boolean
            Dim ctl As SubForm = Page.FindControl(ControlToValidate)
            Return ctl.Validate()
        End Function
    End Class

    Public Class ModifiedScriptDescriptor
        Inherits ScriptComponentDescriptor

        Public Sub New(ByVal Type As String)
            MyBase.New(Type)
        End Sub

        Public Function GetScriptPublic() As String
            Return MyBase.GetScript()
        End Function
    End Class

    Public Class SimpleContainer
        Inherits CompositeControl
        Implements INamingContainer
    End Class

    Public Interface ISubFormScriptControl
        Function GetScript() As String
        Property RenderScript() As Boolean
    End Interface

#Region "Custom SubForm Fields"
    Public Class SubFormPasswordField
        Inherits SubFormField

        Protected Overrides Sub CreateChildControls()
            Dim txtPw As New HtmlControls.HtmlInputPassword()
            txtPw.ID = "pw"
            Me.Controls.Add(txtPw)

            Dim txtConfirm As New HtmlControls.HtmlInputPassword
            txtConfirm.ID = "confirm"
            Me.Controls.Add(txtConfirm)

            Dim input As New SubFormInput()
            input.ServerId = Me.ID
            input.ClientIdSuffix = Me.ClientIDSeparator & "pw"
            input.UniqueIdSuffix = Me.IdSeparator & "pw"
            input.ValidateFunction = "function() {return !isEmptyField(this)}"
            Inputs.Add(input)

            input = New SubFormInput()
            input.ServerId = Me.ID
            input.ClientIdSuffix = Me.ClientIDSeparator & "confirm"
            input.UniqueIdSuffix = Me.IdSeparator & "confirm"
            input.ValidateFunction = "function() {var pw=$get(this.id.replace('confirm','pw')); return (!isEmptyField(this) && this.value==pw.value);}"
            Inputs.Add(input)
        End Sub

        Protected Overrides Function GetTemplateHtml() As String
            Dim out As New Text.StringBuilder()
            out.AppendLine("<input type=""password"" name=""" & Me.UniqueID & Me.IdSeparator & "pw"" id=""" & Me.ClientID & Me.ClientIDSeparator & "pw"" /><br/>")
            out.AppendLine("<input type=""password"" name=""" & Me.UniqueID & Me.IdSeparator & "confirm"" id=""" & Me.ClientID & Me.ClientIDSeparator & "confirm"" /><br/>")
            Return out.ToString()
        End Function

        Public Overrides Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of ModifiedScriptDescriptor)
            Dim s As New ModifiedScriptDescriptor("AE.SubFormTemplate")
            s.AddProperty("html", GetTemplateHtml)
            s.AddProperty("idPrefix", Me.ClientID)
            s.AddProperty("namePrefix", Me.UniqueID)
            Dim jsonInputs As String = String.Empty
            Dim conn As String = "["
            For Each i As SubFormInput In Inputs
                Dim c As Control = FindControl(i.ServerId)
                jsonInputs &= conn & "{'id':'" & Me.ClientID & i.ClientIdSuffix & "'"
                If i.ValidateFunction <> Nothing Then
                    jsonInputs &= ",'validate':" & i.ValidateFunction
                End If
                jsonInputs &= "}"
                conn = ","
            Next
            If jsonInputs <> String.Empty Then
                jsonInputs &= "]"
                s.AddScriptProperty("inputs", jsonInputs)
            End If
            's.AddProperty("validClass", "etnormal")
            's.AddProperty("invalidClass", "eterror")
            s.AddProperty("validClass", ValidClass)
            s.AddProperty("invalidClass", InvalidClass)
            s.AddProperty("id", Me.ID)
            Return New ModifiedScriptDescriptor() {s}
        End Function
    End Class



    Public Class SubFormDropdownField
        Inherits SubFormField

        Private m_ControlId As String
        Public Property ControlId() As String
            Get
                Return m_ControlId
            End Get
            Set(ByVal value As String)
                m_ControlId = value
            End Set
        End Property

        Private m_DataSource As Object
        Public Property DataSource() As Object
            Get
                Return m_DataSource
            End Get
            Set(ByVal value As Object)
                m_DataSource = value
            End Set
        End Property

        Private m_DataTextField As String
        Public Property DataTextField()
            Get
                Return m_DataTextField
            End Get
            Set(ByVal value)
                m_DataTextField = value
            End Set
        End Property

        Private m_DataValueField As String
        Public Property DataValueField() As String
            Get
                Return m_DataValueField
            End Get
            Set(ByVal value As String)
                m_DataValueField = value
            End Set
        End Property

        Private m_AddEmptyOption As Boolean
        Public Property AddEmptyOption() As Boolean
            Get
                Return m_AddEmptyOption
            End Get
            Set(ByVal value As Boolean)
                m_AddEmptyOption = value
            End Set
        End Property

        Private m_EmptyOption As String
        Public Property EmptyOption() As String
            Get
                Return m_EmptyOption
            End Get
            Set(ByVal value As String)
                m_EmptyOption = value
            End Set
        End Property

        Public Overrides Property Inputs() As System.Collections.Generic.List(Of SubFormInput)
            Get
                Dim temp As New SubFormInput()
                temp.ServerId = ControlId
                temp.ValidateFunction = "function() {return true}"
                Return New List(Of SubFormInput)(New SubFormInput() {temp})
            End Get
            Set(ByVal value As System.Collections.Generic.List(Of SubFormInput))
                MyBase.Inputs = value
            End Set
        End Property

        Private Property NumOptions() As Integer
            Get
                Return ViewState("NumOptions")
            End Get
            Set(ByVal value As Integer)
                ViewState("NumOptions") = value
            End Set
        End Property

        Protected Overrides Sub CreateChildControls()
            If DataSource Is Nothing And NumOptions = Nothing Then
                Exit Sub
            End If

            Dim drp As New DropDownList()
            drp.ID = ControlId

            If DataSource IsNot Nothing Then
                drp.DataSource = DataSource
                drp.DataTextField = DataTextField
                drp.DataValueField = DataValueField
                drp.DataBind()
                If AddEmptyOption Then
                    drp.Items.Insert(0, New ListItem(EmptyOption, ""))
                End If
            Else
                For i As Integer = 0 To NumOptions
                    drp.Items.Add(New ListItem())
                Next
            End If

            NumOptions = drp.Items.Count

            Me.Controls.Add(drp)

            'Dim input As New SubFormInput()
            'input.ServerId = drp.ID
            'input.ValidateFunction = "function() {return true}"
            'Inputs.Add(input)
        End Sub

        'Public Overrides Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of ModifiedScriptDescriptor)
        '    Dim s As New ModifiedScriptDescriptor("AE.SubFormTemplate")
        '    s.AddProperty("html", GetTemplateHtml)
        '    s.AddProperty("idPrefix", Me.ClientID)
        '    s.AddProperty("namePrefix", Me.UniqueID)
        '    Dim jsonInputs As String = String.Empty
        '    Dim conn As String = "["
        '    For Each i As SubFormInput In Inputs
        '        Dim c As Control = FindControl(i.ServerId)
        '        jsonInputs &= conn & "{'id':'" & Me.ClientID & i.ClientIdSuffix & "'"
        '        If i.ValidateFunction <> Nothing Then
        '            jsonInputs &= ",'validate':" & i.ValidateFunction
        '        End If
        '        jsonInputs &= "}"
        '        conn = ","
        '    Next
        '    If jsonInputs <> String.Empty Then
        '        jsonInputs &= "]"
        '        s.AddScriptProperty("inputs", jsonInputs)
        '    End If
        '    s.AddProperty("validClass", "etnormal")
        '    s.AddProperty("invalidClass", "eterror")
        '    s.AddProperty("id", Me.ID)
        '    Return New ModifiedScriptDescriptor() {s}
        'End Function
    End Class

#End Region
End Namespace