Imports Components

Partial Class index
    Inherits AdminPage

    Protected ReadOnly Property FrameURL() As String
        Get
            If Session("FrameURL") Is Nothing Then
                Return "main.aspx"
            End If
            If Session("FrameURL") = String.Empty Then
                Return "main.aspx"
            Else
                Return Session("FrameURL")
            End If
        End Get
    End Property
End Class

