Imports Components

Namespace IDevSearch
    Public Class IndexQuery
        Public BestFragmentField As String
        Public BestFragmentCharCount As Integer = 300
        Public MaxDocs As Integer
        Public Keywords As String
        Public Facets As GenericCollection(Of Facet)
        Public FacetCache As GenericCollection(Of Facet)
        Public FacetCacheDuration As Integer = 15 * 60  '15 minutes
        Public SearcherCacheDuration As Integer = 3 * 60  '3 minutes
        Public SortBy As String
        Public SortReverse As Boolean
        Public PageNo As Integer
        Public MaxPerPage As Integer
        Public ForceRefresh As Boolean = False
        Public MaxHitsForCache As Integer = 100 ' default value
        Public FilterCacheKey As String
        Public FilterListCallback As FilterListDelegate
        Public FacetCacheKey As String
        Public FilterList As Generic.IEnumerable(Of String)
    End Class

    Public Delegate Sub FilterListDelegate(ByRef list As Generic.List(Of String))

    Public Class SearchResult
        Public ds As DataSet
        Public Count As Integer
    End Class

    Public Class Facet
        Public Name As String
        Public Ranges As GenericCollection(Of Range)
        Public Field As String
        Public MaxDocs As Integer
        Public ZerosIncluded As Boolean
        Public Narrow As String
        Public ParentName As String
        Public ParentValue As String
    End Class

    Public Class Range
        Public Label As String
        Public Value As String
        Public LBound As String
        Public UBound As String
        Public IsInclusive As String

        Public Sub New(ByVal Label As String, ByVal value As String, ByVal LBound As String, ByVal UBound As String, ByVal IsInclusive As Boolean)
            Me.Label = Label
            Me.Value = value
            Me.LBound = LBound
            Me.UBound = UBound
            Me.IsInclusive = IsInclusive
        End Sub

        Public Shared Function GetRangeByValue(ByVal ranges As GenericCollection(Of Range), ByVal value As String) As Range
            For Each r As Range In ranges
                If value = r.Value Then
                    Return r
                    Exit For
                End If
            Next
            Return Nothing
        End Function

    End Class


End Namespace