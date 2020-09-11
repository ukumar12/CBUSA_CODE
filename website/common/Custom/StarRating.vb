Imports Components
Imports System.Web
Imports System.Web.UI

Namespace Controls
    Public Class StarRating
        Inherits Control
        Implements IPostBackDataHandler

        Public Event RatingUpdate As EventHandler

        Public Property MinRating() As Integer
            Get
                Return ViewState("MinRating")
            End Get
            Set(ByVal value As Integer)
                ViewState("MinRating") = value
            End Set
        End Property

        Public Property MaxRating() As Integer
            Get
                Return IIf(ViewState("MaxRating") Is Nothing, 10, ViewState("MaxRating"))
            End Get
            Set(ByVal value As Integer)
                ViewState("MaxRating") = value
            End Set
        End Property

        Public Property Rating() As Integer
            Get
                Return ViewState("Rating")
            End Get
            Set(ByVal value As Integer)
                ViewState("Rating") = value
            End Set
        End Property

        Public Property NAOnImage() As String
            Get
                Return IIf(ViewState("NAOnImage") Is Nothing, "/images/rating/na-red.gif", ViewState("NAOnImage"))
            End Get
            Set(ByVal value As String)
                ViewState("NAOnImage") = value
            End Set
        End Property

        Public Property NAOffImage() As String
            Get
                Return IIf(ViewState("NAOffImage") Is Nothing, "/images/rating/na-gr.gif", ViewState("NAOffImage"))
            End Get
            Set(ByVal value As String)
                ViewState("NAOffImage") = value
            End Set
        End Property

        Public Property StarOnImage() As String
            Get
                Return IIf(ViewState("StarOnImage") Is Nothing, "/images/rating/star-red.gif", ViewState("StarOnImage"))
            End Get
            Set(ByVal value As String)
                ViewState("StarOnImage") = value
            End Set
        End Property

        Public Property StarOffImage() As String
            Get
                Return IIf(ViewState("StarOffImage") Is Nothing, "/images/rating/star-gr.gif", ViewState("StarOffImage"))
            End Get
            Set(ByVal value As String)
                ViewState("StarOffImage") = value
            End Set
        End Property

        Private Sub RenderScripts()
            If Not Page.ClientScript.IsClientScriptBlockRegistered("StarRating") Then
                Dim s As String = _
                      "function SetRating(id,rating) {" _
                    & "     var hdn=$get(id);" _
                    & "     hdn.value = rating;" _
                    & "}" _
                    & "function Highlight(id,rating) {" _
                    & " var hdn=$get(id);" _
                    & " var child = hdn.nextSibling;" _
                    & " if(rating < 0) rating = hdn.value;" _
                    & " if(rating == 0) {" _
                    & "     child.src = '" & NAOnImage & "';" _
                    & " } else if(rating > 0) {" _
                    & "     child.src = '" & NAOffImage & "';" _
                    & " }" _
                    & " child = child.nextSibling;" _
                    & " while(child && rating > 0) {" _
                    & "     child.src = '" & StarOnImage & "';" _
                    & "     child = child.nextSibling;" _
                    & "     rating--;" _
                    & " }" _
                    & " while(child) {" _
                    & "     child.src = '" & StarOffImage & "';" _
                    & "     child = child.nextSibling;" _
                    & " }" _
                    & "}"

                Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "StarRating", s, True)
            End If
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

            writer.RenderBeginTag("span")

            writer.AddAttribute("id", ClientID)
            writer.AddAttribute("name", UniqueID)
            writer.AddAttribute("type", "hidden")
            writer.AddAttribute("value", Convert.ToString(Rating))
            writer.RenderBeginTag("input")
            writer.RenderEndTag()

            For i As Integer = MinRating To MaxRating
                writer.AddStyleAttribute("cursor", "pointer")
                writer.AddStyleAttribute("border", "none")
                writer.AddAttribute("onclick", "SetRating('" & ClientID & "'," & i & ");")
                writer.AddAttribute("onmouseover", "Highlight('" & ClientID & "'," & i & ");")
                writer.AddAttribute("onmouseout", "Highlight('" & ClientID & "',-1);")
                If i = 0 Then
                    If Rating = i Then
                        writer.AddAttribute("src", NAOnImage)
                    Else
                        writer.AddAttribute("src", NAOffImage)
                    End If
                ElseIf i <= Rating Then
                    writer.AddAttribute("src", StarOnImage)
                Else
                    writer.AddAttribute("src", StarOffImage)
                End If
                writer.RenderBeginTag("img")
                writer.RenderEndTag()
            Next
        End Sub

        Private Sub StarRating_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            RenderScripts()
        End Sub

        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            Dim DoRaise As Boolean = False
            If postCollection(postDataKey) <> Nothing Then
                If postCollection(postDataKey) <> Rating Then
                    DoRaise = True
                End If
                Rating = postCollection(postDataKey)
            End If
            Return DoRaise
        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
            RaiseEvent RatingUpdate(Me, EventArgs.Empty)
        End Sub
    End Class
End Namespace