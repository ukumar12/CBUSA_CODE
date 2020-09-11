Imports Components
Imports DataLayer
Imports System.Web

Partial Class controls_HelpMessage
    Inherits BaseControl

    'This control is used for adding help message tags to the admin forms. To define and add the codes and text
    'for the help meesage please go to the admin section in content tool under help messages and add a tag code and message
    'An example of this control has been shown on admin/store/items/edit.aspx file
    Private m_HelpCode As String = String.Empty
    Private m_HelpImage As String = "/images/utility/question.gif"
    Public Property HelpCode() As String
        Get
            Return m_HelpCode
        End Get
        Set(ByVal Value As String)
            m_HelpCode = Value
        End Set
    End Property
    Public Property HelpImage() As String
        Get
            Return m_HelpImage
        End Get
        Set(ByVal Value As String)
            m_HelpImage = Value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim dbCustomTextRow As CustomTextRow = CustomTextRow.GetRowByCode(DB, HelpCode)
            HelpIcon.Src = HelpImage
            If TypeOf Page Is AdminPage Then
                If CType(Page, AdminPage).LoggedInIsInternal Then
                    lnkEdit.Visible = True
                    lnkEdit.PostBackUrl = "/admin/help/edit.aspx?TextId=" & dbCustomTextRow.TextId & "&RedirectUrl=" & Request.Url.ToString()
                End If
            End If
            If dbCustomTextRow.Value <> String.Empty Then
                Help.InnerHtml = dbCustomTextRow.Value
            Else
                Help.InnerHtml = "There is no help available for selected field"
                lnkEdit.Visible = False
            End If
        End If
    End Sub
End Class
