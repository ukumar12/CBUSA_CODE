Imports System.Collections.Specialized
Imports System.Web

Namespace Components

    Public Class URLParameters
        Dim m_Params As New NameValueCollection

        Public Sub New()
        End Sub

        Public Sub New(ByVal qs As NameValueCollection)
            Dim keys() As String = qs.AllKeys
            For Each key As String In keys
                Add(key, qs(key))
            Next
        End Sub

        Public Sub New(ByVal qs As NameValueCollection, ByVal Exclude As String)
            Dim m_Exclude As ArrayList = New ArrayList(Exclude.ToLower.Split(";"))
            Dim keys() As String = qs.AllKeys

            For Each key As String In keys
                If Not key = String.Empty Then
                    If Not m_Exclude.Contains(key.ToLower) Then
                        Add(key, qs(key))
                    End If
                End If
            Next
        End Sub

        Public Sub New(ByVal text As String, ByVal exclude As String)
            Dim m_Exclude As ArrayList = New ArrayList(exclude.ToLower.Split(";"))
            Dim m_Items As ArrayList = New ArrayList(text.ToLower.Split(";"))
            For Each pair As String In m_Items
                Dim m_Pair = pair.Split("=")
                If Not m_Exclude.Contains(m_Pair(0)) Then
                    Add(m_Pair(0), m_Pair(1))
                End If
            Next
        End Sub

        Public Sub Add(ByVal name As String, ByVal value As String)
            If Not Core.IsDangerousString(name) AndAlso Not Core.IsDangerousString(value) Then m_Params.Add(name, value)
        End Sub

        Public Sub Remove(ByVal name As String)
            m_Params.Remove(name)
        End Sub

        Default Public ReadOnly Property Item(ByVal name As String)
            Get
                Return m_Params(name)
            End Get
        End Property

        Public ReadOnly Property Items() As NameValueCollection
            Get
                Return m_Params
            End Get
        End Property

        Public Overloads Function ToString(ByVal name As String, ByVal value As String) As String
            Add(name, value)
            Dim Result As String = Me.ToString
            m_Params.Remove(name)

            Return Result
        End Function

        Public Overrides Function ToString() As String
            Dim Conn As String = String.Empty
            Dim Result As String = String.Empty

            For i As Integer = 0 To m_Params.Keys.Count - 1
                If Not m_Params.Keys(i) = Nothing Then
                    Result &= Conn & m_Params.Keys(i) & "=" & HttpContext.Current.Server.UrlEncode(m_Params.Item(i))
                    Conn = "&"
                End If
            Next
            If Not Result = String.Empty Then Result = "?" & Result
            Return Result
        End Function
    End Class
End Namespace
