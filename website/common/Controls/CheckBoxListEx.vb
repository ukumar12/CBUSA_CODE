Imports System.Web.UI.WebControls
Imports Components.Core

Namespace Controls
    Public Class CheckBoxListEx
        Inherits CheckBoxList

        Public ReadOnly Property SelectedTexts() As String
            Get
                Dim str, d As String
                d = String.Empty
                str = String.Empty
                For Each li As ListItem In Me.Items
                    If li.Selected Then
                        str += d + li.Text
                        d = ","
                    End If
                Next
                Return str
            End Get
        End Property

        Public Property SelectedValues() As String
            Get
                Dim str, d As String
                d = String.Empty
                str = String.Empty
                For Each li As ListItem In Me.Items
                    If li.Selected Then
                        str += d + li.Value
                        d = ","
                    End If
                Next
                Return str
            End Get
            Set(ByVal value As String)
                If value = String.Empty Then Exit Property

                Dim vals As String() = value.Split(",")
                For Each li As ListItem In Me.Items
                    li.Selected = False
                    For Each v As String In vals
                        If li.Value.ToLower() = v.ToLower() Then
                            li.Selected = True
                            Exit For
                        End If
                    Next
                Next
            End Set
        End Property

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class
End Namespace