Imports Components
Imports DataLayer
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Collections.Generic

Namespace Controls

#Region "Main Control"
    Public Class SubForm
        Inherits CompositeDataBoundControl
        Implements INamingContainer
        Implements IPostBackEventHandler

        Public Event FormUpdated As eventhandler

        Private m_CallbackArgument As String
        Private DataCount As Integer
        Private DisplayCount As Integer

        Public Delegate Sub SubFormEventHandler(ByVal sender As Object, ByVal e As SubFormEventArgs)

        Public Event DataUpdated As SubFormEventHandler

        Private m_OnUpdateScript As String
        Public ReadOnly Property OnUpdateScript() As String
            Get
                If m_OnUpdateScript = Nothing OrElse m_OnUpdateScript = String.Empty Then
                    m_OnUpdateScript = Page.ClientScript.GetPostBackEventReference(Me, "")
                End If
                Return m_OnUpdateScript
            End Get
        End Property

        Private m_Errors As Text.StringBuilder
        Protected Property Errors() As Text.StringBuilder
            Get
                If m_Errors Is Nothing Then
                    m_Errors = New Text.StringBuilder()
                End If
                Return m_Errors
            End Get
            Set(ByVal value As Text.StringBuilder)
                m_Errors = value
            End Set
        End Property

        Private m_NumRequired As Integer
        Public Property NumRequired() As Integer
            Get
                Return m_NumRequired
            End Get
            Set(ByVal value As Integer)
                m_NumRequired = value
            End Set
        End Property

        Private m_Fields As List(Of SubFormField)
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        <TemplateContainer(GetType(SubFormCell))> _
        Public Property Fields() As List(Of SubFormField)
            Get
                Return m_Fields
            End Get
            Set(ByVal value As List(Of SubFormField))
                m_Fields = value
            End Set
        End Property

        Public ReadOnly Property Items() As List(Of SubFormCell)
            Get
                If ViewState("Items") Is Nothing Then
                    ViewState("Items") = New List(Of SubFormCell)
                End If
                Return ViewState("Items")
            End Get
        End Property

        Public Sub New()
            m_Fields = New List(Of SubFormField)
        End Sub

        Protected Overloads Overrides Function CreateChildControls(ByVal dataSource As System.Collections.IEnumerable, ByVal dataBinding As Boolean) As Integer
            DataCount = 0
            DisplayCount = 0

            If dataBinding And dataSource IsNot Nothing Then
                Dim e As IEnumerator = dataSource.GetEnumerator
                Dim tbl As New Table()
                Controls.Add(tbl)
                While e.MoveNext()
                    Dim tr As New TableRow
                    tbl.Rows.Add(tr)
                    For Each f As SubFormField In Fields
                        Dim i As New SubFormCell(DataCount, DisplayCount, e.Current)
                        Dim td As New TableCell()
                        td.Controls.Add(i)
                        tr.Cells.Add(td)
                        f.RowId = DataCount
                        f.OnUpdate = OnUpdateScript
                        f.InstantiateIn(i)
                        i.DataBind()
                        DisplayCount += 1
                        Items.Add(i)
                    Next
                    DataCount += 1
                End While
            End If
            Return DataCount
        End Function

        Public Overrides Sub RenderBeginTag(ByVal writer As System.Web.UI.HtmlTextWriter)
            'MyBase.RenderBeginTag(writer)
            writer.Write("<div>")
            writer.Write("<input type=""hidden"" name=""" & Me.UniqueID & """ id=""" & Me.ClientID & """>")
            If Errors.Length > 0 Then
                writer.Write("<span class=""fielderror"">" & Errors.ToString & "</span><br/>")
            End If
        End Sub

        Public Overrides Sub RenderEndTag(ByVal writer As System.Web.UI.HtmlTextWriter)
            'MyBase.RenderEndTag(writer)
            writer.Write("</div>")
        End Sub

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            Dim IsValid As Boolean = True
            For Each item As SubFormCell In Items
                For Each c As IValidator In item.Controls
                    If Not c.IsValid Then
                        Errors.Append(IIf(Errors.Length = 0, String.Empty, "<br/>") & c.ErrorMessage)
                    End If
                Next
            Next
            Dim args As New SubFormEventArgs()
            args.IsValid = IsValid

            RaiseEvent FormUpdated(Me, args)
        End Sub

        Private Sub RegisterScripts()
            If Not Page.ClientScript.IsClientScriptBlockRegistered("SubFormScripts") Then
                Dim s As String = _
                      "function OnSubFormBlur(e) {" _
                    & " var t=e.target?e.target:e.srcElement;" _
                    & " if(!t) return;" _
                    & " var form=window[t.formId];" _
                    & " form._curEl=null;" _
                    & " form._oldEl=t;" _
                    & " if(form._blurTimeout) {" _
                    & "     window.clearTimeout(form._blurTimeout);" _
                    & " }" _
                    & " this._blurTimeout=window.setTimeout(Function.createDelegate(form,SubFormFieldBlurred),50);" _
                    & "}" _
                    & "function SubFormFieldBlurred() {" _
                    & " if(this._oldEl && (!this._curEl || this._oldEl.rowId != this._curEl.rowId)) {" _
                    & "     $get(this.formId).value='true';" _
                    & "     eval(this.onUpdate);" _
                    & " }" _
                    & "}" _
                    & "function OnSubFormFocus(e) {" _
                    & " var t=e.target?e.target:e.srcElement;" _
                    & " if(!t) return;" _
                    & " if(!t.formId) {" _
                    & "     var m=/(?:\$)?([^\$]*)\$([^\$]*)\$([^\$]*)$/.exec(t.name);" _
                    & "     t.formId=m[1];" _
                    & "     t.rowId=m[2];" _
                    & " }" _
                    & " if(!window[t.formId]) {" _
                    & "     window[t.formId]=new Object();" _
                    & "     window[t.formId].formId=t.formId;" _
                    & "     window[t.formId].onUpdate=t.onUpdate;" _
                    & " }" _
                    & " var form=window[t.formId];" _
                    & " if(e.target && e.target.row) {" _
                    & "     this._curEl=e.target;" _
                    & " }" _
                    & "}"

                Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "SubFormScripts", s, True)
            End If
        End Sub

        Private Sub SubForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            RegisterScripts()
        End Sub
    End Class
#End Region

#Region "Custom EventArgs"
    Public Class SubFormEventArgs
        Inherits System.EventArgs

        Public IsValid As Boolean
    End Class
#End Region

#Region "DataItemContainer"
    <Serializable()> _
    Public Class SubFormCell
        Inherits Control
        Implements IDataItemContainer
        Implements INamingContainer

        Private m_DataIndex As Integer
        Private m_DisplayIndex As Integer
        Private m_DataItem As Object

        Public Sub New(ByVal DataIndex As Integer, ByVal DisplayIndex As Integer, ByVal DataItem As Object)
            m_DataIndex = DataIndex
            m_DisplayIndex = DisplayIndex
            m_DataItem = DataItem
        End Sub

        Public ReadOnly Property DataItem() As Object Implements System.Web.UI.IDataItemContainer.DataItem
            Get
                Return m_DataItem
            End Get
        End Property

        Public ReadOnly Property DataItemIndex() As Integer Implements System.Web.UI.IDataItemContainer.DataItemIndex
            Get
                Return m_DataIndex
            End Get
        End Property

        Public ReadOnly Property DisplayIndex() As Integer Implements System.Web.UI.IDataItemContainer.DisplayIndex
            Get
                Return m_DisplayIndex
            End Get
        End Property
    End Class
#End Region

#Region "Field Types"
    Public MustInherit Class SubFormField
        Inherits Control
        Implements ITemplate

        Public Sub New()
            m_Validators = New List(Of Control)
        End Sub

        Private m_OnUpdate As String
        Public Property OnUpdate() As String
            Get
                Return m_OnUpdate
            End Get
            Set(ByVal value As String)
                m_OnUpdate = value
            End Set
        End Property

        Private m_RowId As String
        Public Property RowId() As String
            Get
                Return m_RowId
            End Get
            Set(ByVal value As String)
                m_RowId = value
            End Set
        End Property

        Public MustOverride Property DataFieldName() As String
        Public MustOverride Property HeaderContent() As String

        Private m_Validators As List(Of Control)
        Public Property Validators() As List(Of Control)
            Get
                Return m_Validators
            End Get
            Set(ByVal value As List(Of Control))
                m_Validators = value
            End Set
        End Property

        Public MustOverride Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
    End Class

    Public Class SubFormTextField
        Inherits SubFormField

        Private m_Text As TextBox

        Private m_IsRequired As Boolean
        Public Property IsRequired() As Boolean
            Get
                Return m_IsRequired
            End Get
            Set(ByVal value As Boolean)
                m_IsRequired = value
            End Set
        End Property

        Private m_Header As String
        Public Overrides Property HeaderContent() As String
            Get
                Return m_Header
            End Get
            Set(ByVal value As String)
                m_Header = value
            End Set
        End Property

        Private m_FieldName As String
        Public Overrides Property DataFieldName() As String
            Get
                Return m_FieldName
            End Get
            Set(ByVal value As String)
                m_FieldName = value
            End Set
        End Property

        Private Sub BindData(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim txt As TextBox = CType(sender, TextBox)
            Dim data As SubFormCell = CType(txt.NamingContainer, SubFormCell)
            txt.Text = CType(data.DataItem, DataRowView)(DataFieldName)
        End Sub

        Public Overrides Sub InstantiateIn(ByVal container As System.Web.UI.Control)
            m_Text = New TextBox
            AddHandler m_Text.DataBinding, AddressOf BindData
            container.Controls.Add(m_Text)

            m_Text.Attributes.Add("onblur", "OnSubFormBlur(event)")
            m_Text.Attributes.Add("onfocus", "OnSubFormFocus(event)")
            Dim tbl As SubForm = container.Parent.Parent.Parent.Parent
            m_Text.Attributes.Add("onUpdate", tbl.OnUpdateScript)

            If IsRequired Then
                Dim v As New RequiredFieldValidatorFront
                v.ControlToValidate = Me.ID
                v.ErrorMessage = "Field '" & DataFieldName & "' is required"
                'v.ValidationGroup = container.NamingContainer.UniqueID & CType(container, SubFormCell).DataItem.dataitemindex
                container.Controls.Add(v)
            End If

            For Each v As BaseValidator In Validators
                If v IsNot Nothing Then
                    'v.ValidationGroup = container.NamingContainer.UniqueID & CType(container, SubFormCell).DataItem.dataitemindex
                    container.Controls.Add(v)
                End If
            Next
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            If m_Text IsNot Nothing Then
                m_Text.Attributes.Add("rowId", RowId)
                m_Text.Attributes.Add("formId", Me.NamingContainer.NamingContainer.ClientID)
                m_Text.Attributes.Add("onUpdate", OnUpdate)
            End If
            MyBase.Render(writer)
        End Sub
    End Class

    Public Class SubFormPhoneField
        Inherits SubFormTextField

        Public Sub New()
            MyBase.new()

            Dim v As New PhoneValidatorFront
            v.ControlToValidate = Me.ID
            Validators.Add(v)
        End Sub
    End Class
#End Region

End Namespace