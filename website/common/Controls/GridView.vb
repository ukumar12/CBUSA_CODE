Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Components
Imports System.Text

Namespace Controls

	Public Class GridView
		Inherits WebControls.GridView

		Public Event DragAndDrop As DragAndDrop
		Public Delegate Sub BindListHandler()
		Public BindList As BindListHandler

		Public Property CausesValidation() As Boolean
			Get
				If ViewState("CausesValidation") Is Nothing Then ViewState("CausesValidation") = True
				Return ViewState("CausesValidation")
			End Get
			Set(ByVal value As Boolean)
				ViewState("CausesValidation") = value
			End Set
		End Property

		Public Property HeaderText() As String
			Get
				Return IIf(ViewState("HeaderText") Is Nothing, String.Empty, ViewState("HeaderText"))
			End Get
			Set(ByVal value As String)
				ViewState("HeaderText") = value
			End Set
		End Property

		Public Property Pager() As CustomPager
			Get
				If ViewState("Pager") Is Nothing Then ViewState("Pager") = New CustomPager
				Return ViewState("Pager")
			End Get
			Set(ByVal value As CustomPager)
				ViewState("Pager") = value
			End Set
		End Property

		Public Property SortBy() As String
			Get
				Return ViewState("SortBy")
			End Get
			Set(ByVal value As String)
				ViewState("SortBy") = value
			End Set
		End Property

		Public Property SortOrder() As String
			Get
				If ViewState("SortOrder") Is Nothing OrElse ViewState("SortOrder") = String.Empty Then ViewState("SortOrder") = "ASC"
				Return ViewState("SortOrder")
			End Get
			Set(ByVal value As String)
				ViewState("SortOrder") = value
			End Set
		End Property

		Public ReadOnly Property SortByAndOrder() As String
			Get
				If SortBy = String.Empty Then Return String.Empty
				Return SortBy & " " & SortOrder
			End Get
		End Property

		Public Property SortImageAsc() As String
			Get
				If ViewState("SortImageAsc") Is Nothing Then ViewState("SortImageAsc") = "/images/admin/asc3.gif"
				Return ViewState("SortImageAsc")
			End Get
			Set(ByVal value As String)
				ViewState("SortImageAsc") = value
			End Set
		End Property

		Public Property SortImageDesc() As String
			Get
				If ViewState("SortImageDesc") Is Nothing Then ViewState("SortImageDesc") = "/images/admin/desc3.gif"
				Return ViewState("SortImageDesc")
			End Get
			Set(ByVal value As String)
				ViewState("SortImageDesc") = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets whether or not drag and drop is enabled on the <see cref="GridView" />.
		''' </summary>
		''' <value><see langword="True" /> if the control allows drag and drop sorting; <see langword="False" />
		''' otherwise.</value>
		''' <remarks><para>Setting this property to <see langword="True" /> will automatically disable the normal
		''' sorting mechanism of the control.</para>
		''' <para>The default value of this property is <see langword="False" />.</para></remarks>
		Public Property EnableDragAndDrop() As Boolean
			Get
				If ViewState("EnableDragAndDrop") Is Nothing Then ViewState("EnableDragAndDrop") = False
				Return ViewState("EnableDragAndDrop")
			End Get
			Set(ByVal value As Boolean)
				If value = True Then
					Me.AllowSorting = False
				End If
				ViewState("EnableDragAndDrop") = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the ID column, used for the drag and drop.
		''' </summary>
		''' <value>A <see cref="String" /> containing a field in the <see cref="DataSource" />.</value>
		''' <remarks>Set this property to the primary key of the table from which the data is being drawn.</remarks>
		Public Property DragAndDropIDColumnName() As String
			Get
				If ViewState("DragAndDropIDColumnName") Is Nothing Then ViewState("DragAndDropIDColumnName") = String.Empty
				Return ViewState("DragAndDropIDColumnName")
			End Get
			Set(ByVal value As String)
				ViewState("DragAndDropIDColumnName") = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the index of the column in which to place the drag and drop anchor.
		''' </summary>
		''' <value>An <see cref="Integer" /> between 0 and the number of columns in the 
		''' <see cref="GridView" />.</value>
		''' <remarks>This should probably be set to either 0 or 1 in order to be next to the column containing
		''' the edit and delete buttons.</remarks>
		Public Property DragAndDropColumnIndex() As Integer
			Get
				If ViewState("DragAndDropColumnIndex") Is Nothing Then ViewState("DragAndDropColumnIndex") = 1
				Return ViewState("DragAndDropColumnIndex")
			End Get
			Set(ByVal value As Integer)
				ViewState("DragAndDropColumnIndex") = value
			End Set
		End Property

		Protected Overrides Sub OnRowCreated(ByVal e As GridViewRowEventArgs)
			If e.Row.RowType = DataControlRowType.Header Then
				'Specify nowrap style for each header column
				For i As Integer = 0 To e.Row.Cells.Count - 1
					e.Row.Cells(i).Style.Add("white-space", "nowrap")
				Next

				'Display arrows	
				If SortBy <> String.Empty Then
					DisplaySortOrderImages(e.Row)
				End If

			ElseIf e.Row.RowType = DataControlRowType.Pager Then

				'Create navigator control and bind hookup PagingEvent
				Dim nav As Navigator = New Navigator
				nav.CssClass = Pager.cssClass
				nav.NofRecords = Pager.NofRecords
				nav.MaxPerPage = PageSize
				nav.PageNumber = PageIndex + 1
				AddHandler nav.NavigatorEvent, AddressOf myNavigator_PagingEvent
				e.Row.Cells(0).Controls.Add(nav)
				nav.DataBind()
			End If

			MyBase.OnRowCreated(e)
		End Sub

		Protected Sub DisplaySortOrderImages(ByVal dgItem As GridViewRow)
			For i As Integer = 0 To dgItem.Cells.Count - 1
				If (dgItem.Cells(i).Controls.Count > 0) AndAlso (TypeOf dgItem.Cells(i).Controls(0) Is LinkButton) Then
					Dim Link As LinkButton = CType(dgItem.Cells(i).Controls(0), LinkButton)
					If SortBy = Link.CommandArgument Then
						Dim imgSortDirection As Image = New Image()
						If SortOrder = "DESC" Then
							imgSortDirection.ImageUrl = SortImageDesc
						Else
							imgSortDirection.ImageUrl = SortImageAsc
						End If
						imgSortDirection.Attributes("align") = "absmiddle"
						dgItem.Cells(i).Controls.Add(imgSortDirection)
					End If
				End If
			Next
		End Sub

		Private Function PageValidate() As Boolean
			If CausesValidation Then
				Page.Validate()
				If Not Page.IsValid Then
					Return False
				End If
			End If
			Return True
		End Function

		Protected Overrides Sub OnSorting(ByVal e As GridViewSortEventArgs)
			If Not PageValidate() Then Exit Sub

			If SortOrder = "ASC" And SortBy = e.SortExpression Then
				SortOrder = "DESC"
			Else
				SortOrder = "ASC"
			End If
			SortBy = Core.ProtectParam(e.SortExpression)
			MyBase.OnSorting(e)
		End Sub

		Private Sub GridView_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
			'Generate blank template, so the standar Pager is not created - we are using custom pager
			PagerTemplate = New BlankTemplate

			'Set default styles
			HeaderStyle.VerticalAlign = VerticalAlign.Top
			If AlternatingRowStyle.IsEmpty Then
				AlternatingRowStyle.CssClass = "alternate"
				AlternatingRowStyle.VerticalAlign = VerticalAlign.Top
			End If
			If RowStyle.IsEmpty Then
				RowStyle.CssClass = "row"
				RowStyle.VerticalAlign = VerticalAlign.Top
			End If
		End Sub

		Private Sub GridView_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
			'Make sure that the pager is displayed when only one page is returned as well
			If Not ViewState("EnableDragAndDrop") Is Nothing AndAlso ViewState("EnableDragAndDrop") = True Then
				If ViewState("DragAndDropIDColumnName") Is Nothing OrElse ViewState("DragAndDropIDColumnName") = String.Empty Then
					Throw New Exception("Must define DragAndDropIDColumnName if using gridview Drag and Drop functionality. GridViewID: " & Me.ID)
				End If
				RegisterScript()
			End If
			If PagerSettings.Position = PagerPosition.Top Or PagerSettings.Position = PagerPosition.TopAndBottom Then
				On Error Resume Next
				If PageCount > 0 Then TopPagerRow.Visible = True
			End If
			If PagerSettings.Position = PagerPosition.Bottom Or PagerSettings.Position = PagerPosition.TopAndBottom Then
				On Error Resume Next
				If PageCount > 0 Then
					If BottomPagerRow IsNot Nothing Then BottomPagerRow.Visible = True
				End If

			End If

		End Sub

		Private Sub RegisterScript()
            ScriptManager.RegisterClientScriptInclude(Page, Me.GetType, "formdnd.js", "/includes/formdnd.js")

			Dim sb As New StringBuilder
			sb.Append("<script type=""text/javascript"" language=""javascript"">" & vbCrLf)
			sb.Append("var dndTable_" & Me.ClientID & " = document.getElementById('" & Me.ClientID & "');" & vbCrLf)
			sb.Append("var dndObj_" & Me.ClientID & " = new TableDnD();" & vbCrLf)
			sb.Append("var dndStartId_" & Me.ClientID & ";" & vbCrLf)
			sb.Append("var dndEndId_" & Me.ClientID & ";" & vbCrLf)
			sb.Append("dndObj_" & Me.ClientID & ".init(dndTable_" & Me.ClientID & ");" & vbCrLf)
			sb.Append("</script>" & vbCrLf)

			ScriptManager.RegisterStartupScript(Page, Me.GetType(), "dnd_" & Me.ClientID, sb.ToString, False)
			If Not Page.ClientScript.IsStartupScriptRegistered("AjaxSortTable") Then
				Dim st As New StringBuilder()
				st.Append("<script type=""text/javascript"">" & vbCrLf)
				st.Append("    function postSortOrderChange(table, sortId, dropId) {" & vbCrLf)
				st.Append("        __doPostBack(table.id.replace(/_/g,'$'),'GridDragging,'+sortId+','+dropId);" & vbCrLf)
				st.Append("    }" & vbCrLf)
				st.Append("</script>" & vbCrLf)
				Page.ClientScript.RegisterStartupScript(Me.GetType(), "AjaxSortTable", st.ToString)
			End If
		End Sub

		Protected Overrides Sub RaisePostBackEvent(ByVal eventArgument As String)
			MyBase.RaisePostBackEvent(eventArgument)
			If Not ViewState("EnableDragAndDrop") Is Nothing AndAlso ViewState("EnableDragAndDrop") = True Then
				'Handle the post back event
				'and set the values of the source and destination items
				If Page.Request("__EVENTARGUMENT") IsNot Nothing AndAlso Page.Request("__EVENTARGUMENT") <> "" AndAlso Page.Request("__EVENTARGUMENT").StartsWith("GridDragging") Then
					Dim sep As Char() = {","c}
					Dim col As String() = eventArgument.Split(sep)
					If IsNumeric(col(1)) And IsNumeric(col(2)) Then
						RaiseEvent DragAndDrop(Me, New DragAndDropEventArgs(Convert.ToInt32(col(1)), Convert.ToInt32(col(2))))
					End If
				End If
			End If
		End Sub

		Protected Overloads Overrides Sub OnRowDataBound(ByVal e As GridViewRowEventArgs)
			MyBase.OnRowDataBound(e)
			If Not ViewState("EnableDragAndDrop") Is Nothing AndAlso ViewState("EnableDragAndDrop") = True Then
				If ViewState("DragAndDropIDColumnName") Is Nothing OrElse ViewState("DragAndDropIDColumnName") = String.Empty Then
					Throw New Exception("Must define DragAndDropIDColumnName if using gridview Drag and Drop functionality. GridViewID: " & Me.ID)
				End If

				'set the java script method for the dragging
				If e.Row.RowType = DataControlRowType.DataRow Then
					e.Row.Cells(ViewState("DragAndDropColumnIndex")).Attributes.Add("class", "sortCell")
					e.Row.Attributes.Add("sortId", e.Row.DataItem(ViewState("DragAndDropIDColumnName")))
				Else
					e.Row.Attributes.Add("nodrop", "true")
					e.Row.Attributes.Add("nodrag", "true")
				End If

			End If

		End Sub

		Protected Sub gvList_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles Me.Sorting
			'if BindList is hooked up, then reset page index and refresh list
			If Not BindList Is Nothing Then
				PageIndex = 0
				BindList()
			End If
		End Sub

		Private Sub myNavigator_PagingEvent(ByVal sender As Object, ByVal e As NavigatorEventArgs)
			If Not PageValidate() Then Exit Sub

			'if BindList is hooked up, then change page index and refresh list
			If Not BindList Is Nothing Then
				PageIndex = e.PageNumber - 1
				BindList()
			End If
		End Sub

		Protected Overrides Sub RenderChildren(ByVal writer As System.Web.UI.HtmlTextWriter)
			If PageCount > 0 Then writer.Write("<span class=""smaller"">" & HeaderText & "</span>")
			MyBase.RenderChildren(writer)
		End Sub

	End Class

	Public Class BlankTemplate
		Implements ITemplate

		Public Sub New()
		End Sub

		Private Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
		End Sub
	End Class

	<Serializable()> _
	Public Class CustomPager
		Public NofRecords As Integer
		Public cssClass As String
	End Class

	''' <summary>
	''' Represents the method that handles <see cref="GridView.DragAndDrop">GridView.DragAndDrop</see>.
	''' </summary>
	''' <param name="sender">The source of the event.</param>
	''' <param name="e">A <see cref="DragAndDropEventArgs" /> which contains the start and end rows of the
	''' drag and drop event.</param>
	''' <remarks>Use this delegate to store the method called by the <see cref="GridView" /> as part of 
	''' the <see cref="GridView.DragAndDrop" /> event.  A handler for this event should probably at some point
	''' call <see cref="Core.ChangeSortOrderDragDrop">Core.ChangeSortOrderDragDrop</see> in order to effect the 
	''' movement of the appropriate item.
	''' <seealso cref="GridView" />
	''' <seealso cref="DragAndDropEventArgs" /></remarks>
	Public Delegate Sub DragAndDrop(ByVal sender As Object, ByVal e As DragAndDropEventArgs)

	''' <summary>
	''' Provides data for the <see cref="GridView.DragAndDrop" /> event.
	''' </summary>
	''' <remarks>This class is generated by the <see cref="GridView.RaisePostBackEvent" /> method in order to
	''' specify where the row being dragged and dropped began and ended.
	''' <seealso cref="GridView" />
	''' <seealso cref="DragAndDrop" /></remarks>
	Public Class DragAndDropEventArgs
		Inherits EventArgs

		Private m_StartRowId As Integer
		Private m_EndRowId As Integer

		''' <summary>
		''' Gets the sort order of the row at which the drag and drop began.
		''' </summary>
		''' <value>An <see cref="Integer" /> which references the SortOrder field of the 
		''' <see cref="GridView" />'s <see cref="GridView.DataSource" />.</value>
		Public ReadOnly Property StartRowId() As Integer
			Get
				Return m_StartRowId
			End Get
		End Property

		''' <summary>
		''' Gets the sort order of the row at which the drag and drop ended.
		''' </summary>
		''' <value>An <see cref="Integer" /> which references the SortOrder field of the 
		''' <see cref="GridView" />'s <see cref="GridView.DataSource" />.</value>
		Public ReadOnly Property EndRowId() As Integer
			Get
				Return m_EndRowId
			End Get
		End Property

		''' <summary>
		''' Initializes a new instance of the <see cref="DragAndDropEventArgs" /> class with the specified 
		''' <paramref name="StartRowId" /> and <paramref name="EndRowId" />.
		''' </summary>
		''' <param name="StartRowId">The value to set <see cref="StartRowId" /> to.</param>
		''' <param name="EndRowId">The value to set <see cref="EndRowId" /> to.</param>
		Public Sub New(ByVal StartRowId As Integer, ByVal EndRowId As Integer)
			m_StartRowId = StartRowId
			m_EndRowId = EndRowId
		End Sub
	End Class

End Namespace
