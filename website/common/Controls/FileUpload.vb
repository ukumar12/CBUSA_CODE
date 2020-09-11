Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Text
Imports System.Web
Imports System
Imports System.ComponentModel
Imports System.Globalization
Imports System.Collections
Imports System.Collections.Specialized
Imports Components

Namespace Controls

	Public Class FileUpload
		Inherits System.Web.UI.WebControls.WebControl
		Implements System.Web.UI.IPostBackDataHandler

		Private m_OldFileName As String = String.Empty
		Private m_NewFileName As String = String.Empty
		Private m_Checked As Boolean
		Private m_Required As Boolean = False

		Private chk As New HtmlInputCheckBox
		Private hidden As New HtmlInputHidden
		Private File As New HtmlInputFile
		Private m_ImageWidth As Integer = Nothing
		Private m_ImageHeight As String = Nothing
		Private m_AutoResize As Boolean = True

		Public MyFile As HttpPostedFile

		Public Property Folder() As String
			Get
				Dim f As String = CStr(ViewState("Folder"))
				If Not Right(f, 1) = "/" Then
					f &= "/"
				End If
				Return f
			End Get
			Set(ByVal value As String)
				ViewState("Folder") = value
			End Set
		End Property

		Public Property ImageDisplayFolder() As String
			Get
				Dim f As String = CStr(ViewState("ImageDisplayFolder"))
				If Not Right(f, 1) = "/" Then
					f &= "/"
				End If
				Return f
			End Get
			Set(ByVal value As String)
				ViewState("ImageDisplayFolder") = value
			End Set
		End Property

		Public Property DisplayImage() As Boolean
			Get
				Return CBool(ViewState("DisplayImage"))
			End Get
			Set(ByVal value As Boolean)
				ViewState("DisplayImage") = value
			End Set
		End Property

		Public Property ImageWidth() As Integer
			Get
				Return m_ImageWidth
			End Get
			Set(ByVal value As Integer)
				m_ImageWidth = value
			End Set
		End Property

		Public Property ImageHeight() As Integer
			Get
				Return m_ImageHeight
			End Get
			Set(ByVal value As Integer)
				m_ImageHeight = value
			End Set
		End Property

		Public Property AutoResize() As Boolean
			Get
				Return m_AutoResize AndAlso Not ImageWidth = Nothing AndAlso Not ImageHeight = Nothing
			End Get
			Set(ByVal value As Boolean)
				m_AutoResize = value
			End Set
		End Property

		Public Property CurrentFileName() As String
			Get
				Return CStr(ViewState("CurrentFileName"))
			End Get
			Set(ByVal value As String)
				ViewState("CurrentFileName") = value
			End Set
		End Property

		Public Property Required() As Boolean
			Get
				Return m_Required
			End Get
			Set(ByVal value As Boolean)
				m_Required = value
			End Set
		End Property

		Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
			Page.RegisterRequiresPostBack(Me)
			MyBase.OnInit(e)
		End Sub

		Public Sub New()
		End Sub

		Public ReadOnly Property MarkedToDelete()
			Get
				Me.EnsureChildControls()
				If MyFile Is Nothing Then LoadValues()
				Return m_Checked
			End Get
		End Property

		Public Sub RemoveOldFile()
			Me.EnsureChildControls()
			If MyFile Is Nothing Then LoadValues()
			Try
				System.IO.File.Delete(HttpContext.Current.Server.MapPath(Folder & CurrentFileName))
				If Not (IsNothing(ImageDisplayFolder) OrElse ImageDisplayFolder = Nothing) Then
					System.IO.File.Delete(HttpContext.Current.Server.MapPath(ImageDisplayFolder & CurrentFileName))
				End If
			Catch ex As Exception
			End Try
		End Sub

		Public Sub RemoveOldFile(ByVal path As String)
			Me.EnsureChildControls()
			If MyFile Is Nothing Then LoadValues()
			Try
				System.IO.File.Delete(HttpContext.Current.Server.MapPath(System.IO.Path.Combine(path, CurrentFileName)))
			Catch ex As Exception
			End Try
		End Sub

		Public ReadOnly Property NewFileName() As String
			Get
				Me.EnsureChildControls()
				If MyFile Is Nothing Then LoadValues()
				Return m_NewFileName
			End Get
		End Property

		Private Function GetFileName() As String
			If MyFile Is Nothing Then Return String.Empty

			Dim FileName As String = System.IO.Path.GetFileName(MyFile.FileName)
			If FileName = String.Empty Then Return String.Empty

			Dim OriginalName As String = System.IO.Path.GetFileNameWithoutExtension(FileName)
			Dim OriginalExtension As String = System.IO.Path.GetExtension(FileName)
			OriginalName = OriginalName.Replace(" ", "_")
			OriginalName = OriginalName.Replace(",", "_")
			OriginalName = OriginalName.Replace(".", "_")
			Dim tmpName As String = OriginalName & OriginalExtension
			Dim iCounter As Integer = 1
			While System.IO.File.Exists(HttpContext.Current.Server.MapPath(Folder & tmpName))
				tmpName = OriginalName & iCounter.ToString() & OriginalExtension
				iCounter += 1
			End While
			Return tmpName
		End Function

		Public Sub SaveNewFile(ByVal DestFolder As String)
			Me.EnsureChildControls()

			m_NewFileName = GetFileName()

			If MyFile Is Nothing Then LoadValues()
			If NewFileName = String.Empty Then Exit Sub
			If MyFile.ContentLength = 0 Then Throw New ArgumentException("Error while attempting to save file: " & NewFileName)
			MyFile.SaveAs(HttpContext.Current.Server.MapPath(DestFolder & NewFileName))

			If AutoResize Then
				Core.ResizeImage(HttpContext.Current.Server.MapPath(DestFolder & NewFileName), HttpContext.Current.Server.MapPath(DestFolder & NewFileName), ImageWidth, ImageHeight)
			End If
		End Sub

		Public Sub SaveNewFile()
			SaveNewFile(Folder)
		End Sub

		Public Sub SaveNewFile(ByVal DestFolderName As String, ByVal Width As Integer, ByVal Height As Integer)
			SaveNewFile(DestFolderName)
			Core.ResizeImage(HttpContext.Current.Server.MapPath(DestFolderName & NewFileName), HttpContext.Current.Server.MapPath(DestFolderName & NewFileName), Width, Height)
		End Sub

		Public Sub SaveNewFile(ByVal Width As Integer, ByVal Height As Integer)
			SaveNewFile(Folder)
			Core.ResizeImage(HttpContext.Current.Server.MapPath(Folder & NewFileName), HttpContext.Current.Server.MapPath(Folder & NewFileName), Width, Height)
		End Sub

		Protected Overrides Sub CreateChildControls()
			Dim panel As Panel = New Panel()
			Dim link As HyperLink = New HyperLink
			Dim img As Image = New Image
			Dim lbreak As Label = New Label
			Dim ltlCurrentFile As LiteralControl = New LiteralControl("<FONT size=""1"">Current File:</FONT>")

			Controls.Clear()

			Dim ltl As LiteralControl

			If DisplayImage Then
				ltl = New LiteralControl("delete this image")
			Else
				ltl = New LiteralControl("delete this file")
				panel.Controls.Add(ltlCurrentFile)
			End If

			If DisplayImage Then
				panel.Controls.Add(img)
				img.ImageUrl = ImageDisplayFolder & CurrentFileName
			Else
				panel.Controls.Add(link)
				link.ID = Me.ID.ToString() & "_LINK"
				link.Target = "_blank"
			End If

			panel.Controls.Add(lbreak)

			chk.ID = Me.ID.ToString() & "_CHK"
			chk.Value = "Y"
			panel.Controls.Add(chk)
			panel.Controls.Add(ltl)

			panel.ID = Me.ID.ToString() & "_PNL"
			Controls.Add(panel)

			panel.Visible = Not CurrentFileName = String.Empty
			link.NavigateUrl = Folder & CurrentFileName
			link.Text = CurrentFileName
			chk.Visible = (Not CurrentFileName = String.Empty AndAlso Not Required)
			ltl.Visible = (Not CurrentFileName = String.Empty AndAlso Not Required)
			img.Visible = (Not CurrentFileName = String.Empty)
			ltlCurrentFile.Visible = (Not CurrentFileName = String.Empty)
			link.Visible = (Not CurrentFileName = String.Empty)
			lbreak.Text = IIf(chk.Visible, "<br />", "")

			hidden.ID = Me.ID.ToString() & "_OLD"
			hidden.Value = CurrentFileName
			Controls.Add(hidden)

			File.ID = Me.ID.ToString() & "_FILE"
			File.Attributes("style") = Me.Attributes("style")
			Controls.Add(File)
			If Not ImageWidth = Nothing AndAlso Not ImageHeight = Nothing Then Controls.Add(New LiteralControl("<br />Optimal Image Size: <b>" & ImageWidth.ToString & " x " & ImageHeight.ToString & "</b>"))
		End Sub

		Private Sub LoadValues()
			MyFile = File.PostedFile
			m_OldFileName = hidden.Value
			m_NewFileName = GetFileName()
		End Sub

		Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
			Dim context As HttpContext = HttpContext.Current
			m_Checked = (context.Request.Form(Me.UniqueID.ToString() & "_CHK") = "Y")
			Return False
		End Function

		Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
		End Sub
	End Class

End Namespace
