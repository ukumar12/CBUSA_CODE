Imports Components
Imports Lucene.Net
Imports Lucene.Net.Search
Imports Lucene.Net.Index
Imports IDevSearch
Imports System.Web.UI
Imports System.Web
Imports DataLayer


Public Class KeyFilter
    Inherits Lucene.Net.Search.Filter

    Private bFilter As Filter
    Private m_Callback As FilterListDelegate
    Private m_CacheKey As String
    Private DB As Database

    Public Sub New(ByVal CacheKey As String, ByVal GetListCallback As FilterListDelegate, ByVal baseFilter As Filter)
        bFilter = baseFilter
        m_Callback = GetListCallback
        m_CacheKey = CacheKey
    End Sub

    Private m_Data As Generic.List(Of String)
    Private ReadOnly Property Data() As Generic.List(Of String)
        Get
            If m_Data Is Nothing Then
                m_Data = New Generic.List(Of String)
                m_Callback.Invoke(m_Data)
            End If
            Return m_Data
        End Get
    End Property

    Public Overrides Function Bits(ByVal reader As Lucene.Net.Index.IndexReader) As System.Collections.BitArray
        Dim base As BitArray = bFilter.Bits(reader)
        Dim len As Integer = IIf(base Is Nothing, reader.MaxDoc, base.Length)

        Dim te As TermEnum = reader.Terms(New Term("ProductId", ""))
        Dim out As BitArray = Nothing
        'If HttpContext.Current.Session("LLCFilter_BitCacheId") = m_cachekey Then
        '    out = HttpContext.Current.Session("LLCFilter_BitCache")
        '    If out.Length <> len Then
        '        out = Nothing
        '    End If
        'End If
        If out Is Nothing Then
            out = New BitArray(len)
            If Data Is Nothing OrElse Data.Count = 0 Then
                out.SetAll(True)
            Else
                out.SetAll(False)

                Dim td As TermDocs
                For Each id As String In Data
                    'Dim t As New Term("ProductID", id)
                    td = reader.TermDocs(New Term("ProductID", id))
                    'td.Seek(t)
                    If td.Next Then
                        out(td.Doc) = True
                    End If
                Next
            End If
            'HttpContext.Current.Session("LLCFilter_BitCacheId") = m_CacheKey
            'HttpContext.Current.Session("LLCFilter_BitCache") = out
        End If
        If base Is Nothing Then
            Return out
        Else
            Return out.And(base)
        End If

    End Function
End Class