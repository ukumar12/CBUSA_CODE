Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.ComponentModel
Imports Components
Imports System.Web
Imports System.Collections

Namespace Controls

    ' Summary description for DropDownListEx
    <ToolboxData("<{0}:DropDownListEx runat=server></{0}:DropDownListEx>")> _
    Public Class DropDownListEx
        Inherits System.Web.UI.WebControls.DropDownList

        ' The field in the datasource which provides values for groups
        <DefaultValue(""), Category("Data")> _
        Public Overridable Property DataGroupField() As String
            Get
                Dim obj As Object = ViewState("DataGroupField")
                If Not obj Is Nothing Then
                    Return obj
                End If
                Return String.Empty
            End Get
            Set(ByVal Value As String)
                ViewState("DataGroupField") = Value
            End Set
        End Property

        ' Render this control to the output parameter specified.
        ' Based on the source code of the original DropDownList method
        Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
            Dim items As ListItemCollection = Me.Items
            Dim itemCount As Integer = Me.Items.Count
            Dim curGroup As String = String.Empty
            Dim itemGroup As String = String.Empty
            Dim bSelected As Boolean = False

            If itemCount <= 0 Then
                Return
            End If

            Dim i As Integer
            For i = 0 To itemCount - 1
                Dim item As ListItem = items(i)
                itemGroup = CType(item.Attributes("DataGroupField"), String)

                If Not itemGroup Is Nothing And itemGroup <> curGroup Then
                    If curGroup <> String.Empty Then
                        writer.WriteEndTag("optgroup")
                        writer.WriteLine()
                    End If
                    curGroup = itemGroup
                    writer.WriteBeginTag("optgroup")
                    writer.WriteAttribute("label", curGroup, True)
                    writer.Write(">"c)
                    writer.WriteLine()
                End If


                writer.WriteBeginTag("option")
                If item.Selected Then
                    If (bSelected) Then
                        Throw New HttpException("Cannot multiselect in GroupDropDownList")
                    End If
                    bSelected = True
                    writer.WriteAttribute("selected", "selected", False)
                End If
                writer.WriteAttribute("value", item.Value, True)
                writer.Write(">"c)
                HttpUtility.HtmlEncode(item.Text, writer)
                writer.WriteEndTag("option")
                writer.WriteLine()
            Next
            If curGroup <> String.Empty Then
                writer.WriteEndTag("optgroup")
                writer.WriteLine()
            End If
        End Sub

        ' Perform data binding logic that is associated with the control
        Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)
            ' Call base method to bind data
            MyBase.OnDataBinding(e)

            If Me.DataGroupField = String.Empty Then
                Return
            End If

            ' For each Item add the attribute "DataGroupField" with value from the datasource
            Dim dataSource As IEnumerable = GetResolvedDataSource(Me.DataSource, Me.DataMember)
            If Not dataSource Is Nothing Then
                Dim items As ListItemCollection = Me.Items
                Dim i As Integer = 0

                Dim groupField As String = Me.DataGroupField
                Dim obj As Object
                For Each obj In dataSource
                    Dim groupFieldValue As String = DataBinder.GetPropertyValue(obj, groupField, Nothing)
                    Dim item As ListItem = items(i)
                    item.Attributes.Add("DataGroupField", groupFieldValue)
                    i = i + 1
                Next
            End If

        End Sub

        Private Function GetResolvedDataSource(ByVal dataSource As Object, ByVal dataMember As String) As IEnumerable
            If dataSource Is Nothing Then Return Nothing

            Dim Source As IListSource = CType(dataSource, IListSource)
            If Not Source Is Nothing Then
                Dim List As IList = Source.GetList()

                If Not Source.ContainsListCollection Then
                    Return List
                End If
                If TypeOf List Is ITypedList Then
                    Dim tmpList As ITypedList = CType(List, ITypedList)
                    Dim collection As PropertyDescriptorCollection = tmpList.GetItemProperties(Nothing)
                    Dim descriptor As PropertyDescriptor = Nothing
                    If dataMember Is Nothing Or dataMember.Length = 0 Then
                        descriptor = collection(0)
                    Else
                        descriptor = collection.Find(dataMember, True)
                    End If
                    If Not descriptor Is Nothing Then
                        Dim obj1 As Object = List(0)
                        Dim obj2 As Object = descriptor.GetValue(obj1)
                        If TypeOf obj2 Is IEnumerable Then
                            Return CType(obj2, IEnumerable)
                        End If
                    End If
                End If
            End If

            If TypeOf dataSource Is IEnumerable Then
                Return CType(dataSource, IEnumerable)
            End If
            Return Nothing

        End Function

        Protected Overrides Function SaveViewState() As Object
            ' Create an object array with one element for the CheckBoxList's
            ' ViewState contents, and one element for each ListItem in skmCheckBoxList
            Dim state() As Object = New Object(Items.Count + 1) {}

            Dim baseState As Object = MyBase.SaveViewState()
            state(0) = baseState

            ' Now, see if we even need to save the view state
            Dim bHasAttributes As Boolean = False
            For i As Integer = 0 To Items.Count - 1
                If Items(i).Attributes.Count > 0 Then
                    bHasAttributes = True

                    ' Create an array of the item's Attribute's keys and values
                    Dim attribKV() As Object = New Object(Items(i).Attributes.Count * 2) {}
                    Dim k As Integer = 0
                    For Each key As String In Items(i).Attributes.Keys
                        attribKV(k) = key
                        k += 1
                        attribKV(k) = Items(i).Attributes(key)
                        k += 1
                    Next
                    state(i + 1) = attribKV
                End If
            Next
            ' return either baseState or state, depending on whether or not
            ' any ListItems had attributes
            If bHasAttributes Then
                Return state
            Else
                Return baseState
            End If
        End Function

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            If savedState Is Nothing Then
                Return
            End If

            ' see if savedState is an object or object array
            If TypeOf savedState Is Object() Then
                ' we have an array of items with attributes
                Dim state() As Object = CType(savedState, Object())
                MyBase.LoadViewState(state(0)) 'load the base state 

                For i As Integer = 1 To state.Length - 1
                    If Not state(i) Is Nothing Then
                        ' Load back in the attributes
                        Dim attribKV() As Object = CType(state(i), Object())
                        For k As Integer = 0 To attribKV.Length - 2 Step 2
                            Me.Items(i - 1).Attributes.Add(attribKV(k).ToString(), attribKV(k + 1).ToString())
                        Next
                    End If
                Next
            Else
                ' we have just the base state
                MyBase.LoadViewState(savedState)
            End If
        End Sub

        Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
            Dim item As ListItem
            For Each item In Items
                item.Text = item.Text.Replace(" ", HttpUtility.HtmlDecode("&nbsp;"))
            Next
            MyBase.Render(writer)
        End Sub

    End Class
End Namespace
