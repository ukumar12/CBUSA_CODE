Imports System.IO
Imports System.Xml
Imports Lucene.Net.Documents
Imports Lucene.Net.QueryParsers
Imports Lucene.Net.Search
Imports Lucene.Net.Highlight
Imports Lucene.Net.Store
Imports Lucene.Net.Index
Imports Lucene.Net.Analysis
Imports Lucene.Net
Imports System.Web
Imports Lucene.Net.Analysis.Standard
Imports Lucene.Net.Search.Similar
Imports Components
Imports System.Diagnostics
Imports IDevSearch

Public Class SearchIndex

    Public Shared Function GetPrice(ByVal s As Double) As String
        Dim str As String = String.Empty
        Dim sStr As String = FormatNumber(s * 100, 0, TriState.UseDefault, TriState.False, TriState.False)

        For i As Integer = 0 To 6 - sStr.ToString.Length
            str &= "0"
        Next

        str &= sStr

        Return str
    End Function

    Public Shared Function GetNumber(ByVal s As Double) As String
        Dim str As String = String.Empty
        Dim sStr As String = FormatNumber(s * 10000, 0, TriState.UseDefault, TriState.False, TriState.False)

        For i As Integer = 0 To 10 - sStr.ToString.Length
            str &= "0"
        Next

        str &= sStr

        Return str
    End Function

    Public Shared Function RemoveFacet(ByVal f As String, ByVal facet As String) As String
        If f = String.Empty Then
            Return String.Empty
        End If
        facet = facet.ToLower.Trim
        Dim Facets As String = "," & f.ToLower.Trim

        Facets = Replace(Facets, "," & facet, String.Empty)
        If Facets = String.Empty Then Return String.Empty

        Return Right(Facets, Len(Facets) - 1)
    End Function

    Public Shared Function ReplaceFacet(ByVal f As String, ByVal facet As String) As String
        If f = String.Empty Then
            Return facet
        End If
        facet = facet.ToLower
        Dim Facets As String = "," & f.ToLower
        Facets = Replace(Facets, "," & facet, String.Empty)
        Facets = Facets & "," & facet
        Return Right(Facets, Len(Facets) - 1)
    End Function

    Private m_Fields As New GenericCollection(Of Field)
    Public Property Fields() As GenericCollection(Of Field)
        Get
            Return m_Fields
        End Get
        Set(ByVal value As GenericCollection(Of Field))
            m_Fields = value
        End Set
    End Property

    Public ReadOnly Property PrimaryKey() As String
        Get
            For Each f As Field In Fields
                If f.PrimaryKey = True Then Return f.Name
            Next
            Return String.Empty
        End Get
    End Property

    Private m_Directory As String
    Public Property Directory() As String
        Get
            Return m_Directory
        End Get
        Set(ByVal value As String)
            m_Directory = value
        End Set
    End Property

    Private m_Dictionary As DictionaryDef
    Public Property Dictionary() As DictionaryDef
        Get
            Return m_Dictionary
        End Get
        Set(ByVal value As DictionaryDef)
            m_Dictionary = value
        End Set
    End Property

    Private m_IndexName As String
    Public Property IndexName() As String
        Get
            Return m_IndexName
        End Get
        Set(ByVal value As String)
            m_IndexName = value
        End Set
    End Property

    Private m_DefaultOperator As String = "AND"
    Public Property DefaultOperator() As String
        Get
            Return m_DefaultOperator
        End Get
        Set(ByVal value As String)
            m_DefaultOperator = value
        End Set
    End Property

    Private ReadOnly Property LiveDataDirectory() As String
        Get
            Return Directory & "\Live\" & IndexName & "\data"
        End Get
    End Property

    Private ReadOnly Property LiveDictionaryDirectory() As String
        Get
            Return Directory & "\Live\" & IndexName & "\dictionary"
        End Get
    End Property

    Private m_ProductMap As Hashtable

    Public Sub New()
    End Sub

    Public Sub ParseDefinition()
        Dim c As Field = Nothing
        Dim d As DictionaryDef = Nothing
        Dim r As XmlTextReader = New XmlTextReader(Directory & "\Definition\" & IndexName & ".xml")
        r.NameTable.Add("definition")
        r.MoveToContent()
        While r.Read
            If r.Name = "field" And r.NodeType = XmlNodeType.Element Then
                c = New Field()
                While r.MoveToNextAttribute()
                    If LCase(r.Name) = "name" Then
                        c.Name = r.Value
                    End If
                    If LCase(r.Name) = "type" Then
                        c.Type = r.Value
                    End If
                    If LCase(r.Name) = "fieldtype" Then
                        c.FieldType = r.Value
                    End If
                    If LCase(r.Name) = "primarykey" Then
                        c.PrimaryKey = r.Value
                    End If
                    If LCase(r.Name) = "striphtml" Then
                        c.StripHTML = r.Value
                    End If
                    If LCase(r.Name) = "boost" Then
                        c.Boost = r.Value
                    End If
                End While
                Fields.Add(c)
            End If
            If r.Name = "dictionary" Then
                d = New DictionaryDef
                While r.MoveToNextAttribute()
                    If LCase(r.Name) = "name" Then
                        d.Name = r.Value
                    End If
                    If LCase(r.Name) = "fields" Then
                        Dim Fields As String = r.Value
                        d.Fields = Fields.Split(","c)
                    End If
                End While
                Dictionary = d
            End If
        End While
        r.Close()
    End Sub

    Protected Function GetProductMap(ByVal searcher As IndexSearcher)
        If HttpContext.Current.Cache("ProductMap") IsNot Nothing Then
            Return HttpContext.Current.Cache("ProductMap")
        Else
            Try
                System.Threading.Monitor.Enter(searcher)
                Dim reader As IndexReader = searcher.GetIndexReader()
                Dim map As New Hashtable
                Dim te As TermEnum = reader.Terms(New Term("ProductID", ""))
                While te.Next
                    Dim td As TermDocs = reader.TermDocs(te.Term)
                    If td.Next Then
                        map(te.Term.ToString.Remove(0, te.Term.ToString.IndexOf(":") + 1)) = td.Doc()
                    End If
                End While
                HttpContext.Current.Cache.Add("ProductMap", map, Nothing, DateAdd(DateInterval.Minute, 5, Now), Nothing, Caching.CacheItemPriority.Default, Nothing)

                Return map
            Finally
                System.Threading.Monitor.Exit(searcher)
            End Try
        End If
    End Function

    Private Function GetFacetDataTable(ByVal Facet As String) As DataTable
        Dim dt As DataTable = New DataTable(Facet)
        dt.Columns.Add("Name", GetType(String))
        dt.Columns.Add("Label", GetType(String))
        dt.Columns.Add("Value", GetType(String))
        dt.Columns.Add("Count", GetType(Integer))
        Return dt
    End Function

    Private Function GetDataTable() As DataTable
        Dim dt As DataTable = New DataTable("DataTable")
        For i As Integer = 0 To Fields.Count - 1
            Dim f As Field = Fields(i)
            dt.Columns.Add(f.Name, Type.GetType("System." & f.Type))
        Next
        dt.Columns.Add("Score", GetType(Double))
        dt.Columns.Add("BestFragment", GetType(String))
        Return dt
    End Function

    Private Function GetFacetQuery(ByVal Facets As GenericCollection(Of Facet), ByVal Exclude As Facet) As BooleanQuery
        Dim bq As BooleanQuery = Nothing

        If Facets Is Nothing Then
            Return Nothing
        End If

        For Each f As Facet In Facets
            If Not Exclude Is Nothing AndAlso f.Name = Exclude.Name Then
                Continue For
            End If
            If Not f.Narrow = String.Empty Then
                If Not f.Ranges Is Nothing Then
                    Dim r As Range = Range.GetRangeByValue(f.Ranges, f.Narrow)
                    Dim rq As New RangeQuery(New Term(f.Name, r.LBound), New Term(f.Name, r.UBound), True)
                    If bq Is Nothing Then bq = New BooleanQuery
                    bq.Add(rq, BooleanClause.Occur.MUST)
                Else
                    If bq Is Nothing Then bq = New BooleanQuery
                    Dim tq As TermQuery = Nothing
                    tq = New TermQuery(New Term(f.Name, f.Narrow))
                    bq.Add(tq, BooleanClause.Occur.MUST)
                End If
            End If
        Next
        Return bq
    End Function

    Public Function GetMoreLikeThis(ByVal value As String, ByVal FieldNames As String, ByVal q As IndexQuery) As DataSet
        'Make sure that the index definition is saved in Fields collection

        If Fields.Count = 0 Then ParseDefinition()

        Dim searcher As IndexSearcher = GetIndexSearcher(LiveDataDirectory, q)
        Dim reader As IndexReader = searcher.GetIndexReader
        Dim t As Term = New Term(PrimaryKey, value)
        Dim tdocs As TermDocs = reader.TermDocs(t)
        Dim DocId As Integer = 0
        If Not tdocs Is Nothing Then
            tdocs.Next()
            DocId = tdocs.Doc
            tdocs.Close()
        End If
        Dim mlt As MoreLikeThis = New MoreLikeThis(reader)
        mlt.setFieldNames(FieldNames.Split(","))
        mlt.setAnalyzer(New PorterStemmerAnalyzer)
        mlt.setMinDocFreq(5)
        mlt.setMinTermFreq(1)
        mlt.setStopWords(StopFilter.MakeStopSet(MoreLikeThis.ENGLISH_STOP_WORDS))

        Dim query As Query = mlt.like(DocId)
        Dim docs As TopFieldDocs = Nothing
        Dim tq As TermQuery = New TermQuery(t)

        Dim bq As BooleanQuery = New BooleanQuery
        bq.Add(query, BooleanClause.Occur.MUST)
        bq.Add(tq, BooleanClause.Occur.MUST_NOT)

        docs = searcher.Search(bq, Nothing, q.MaxDocs, New Sort(SortField.FIELD_SCORE))

        Dim scorer As QueryScorer = New QueryScorer(query)
        Dim highlighter As Highlighter = New Highlighter(scorer)
        highlighter.SetTextFragmenter(New SimpleFragmenter(q.BestFragmentCharCount))

        Dim ds As New DataSet
        Dim Results As DataTable = GetDataTable()
        For i As Integer = 0 To docs.scoreDocs.Length - 1
            Dim doc As Document = searcher.Doc(docs.scoreDocs(i).doc)
            Dim row As DataRow = Results.NewRow()
            For Each f As Field In Fields
                row(f.Name) = IIf(doc.Get(f.Name) = Nothing, DBNull.Value, doc.Get(f.Name))
            Next
            row("Score") = docs.scoreDocs(i).score

            Dim BestFragment As String = doc.Get(q.BestFragmentField)
            If Not BestFragment = String.Empty Then
                Dim stream As TokenStream = New PorterStemmerAnalyzer().TokenStream(q.BestFragmentField, New StringReader(BestFragment))
                row("BestFragment") = highlighter.GetBestFragment(stream, BestFragment)
            End If
            If Convert.IsDBNull(row("BestFragment")) Then row("BestFragment") = BestFragment

            Results.Rows.Add(row)
        Next
        ds.Tables.Add(Results)

        Return ds
    End Function

    Private Function GetIndexSearcher(ByVal DataDirectory As String, ByVal q As IndexQuery) As IndexSearcher
        'Searcher is kept in the Application object
        Dim searcher As IndexSearcher = HttpContext.Current.Application("Searcher" & DataDirectory)

        'Refresh searcher (close and re-open)
        Dim active As String = HttpContext.Current.Cache("Searcher" & DataDirectory)

        'force refresh the application and cache
        If q.ForceRefresh Then
            active = String.Empty
        End If

        If active = String.Empty AndAlso Not searcher Is Nothing Then
            Try
                searcher.Close()
            Catch ex As Exception
            End Try
            searcher = Nothing
            HttpContext.Current.Cache.Remove("ProductMap")
        End If
        If searcher Is Nothing Then
            Dim directory As FSDirectory = FSDirectory.GetDirectory(DataDirectory, False)
            searcher = New IndexSearcher(directory)

            HttpContext.Current.Application.Lock()
            HttpContext.Current.Application("Searcher" & DataDirectory) = searcher
            HttpContext.Current.Application.UnLock()

            HttpContext.Current.Cache.Insert("Searcher" & DataDirectory, "active", Nothing, DateAdd(DateInterval.Second, q.SearcherCacheDuration, Now), Nothing)
        End If
        Return searcher
    End Function

    Public Function Search(ByVal q As IndexQuery) As SearchResult
        'Make sure that the index definition is saved in Fields collection
        If Fields.Count = 0 Then ParseDefinition()

        Logger.Info("Before Opening Index Searcher")
        Dim searcher As IndexSearcher = GetIndexSearcher(LiveDataDirectory, q)
        Logger.Info("After Opening Index Searcher")

        Logger.Info("Before Parsing Query")
        Dim query As Query = Nothing
        If q.Keywords.Length > 0 Then
            query = CustomQueryParser.Parse(q.Keywords, Fields, New PorterStemmerAnalyzer, DefaultOperator)
        End If
        Logger.Info("After Parsing Query")

        Dim narrow As Filter = Nothing
        Dim bq As BooleanQuery = GetFacetQuery(q.Facets, Nothing)
        If Not bq Is Nothing Then narrow = New QueryFilter(bq)

        If query Is Nothing Then
            query = bq
            ' narrow = Nothing
        End If

        If Not bq Is Nothing Then Logger.Info("Narrow Query: " & bq.ToString())

        If q.FilterList IsNot Nothing Then
            Dim b As New BitArray(searcher.MaxDoc, False)
            Dim map As Hashtable = GetProductMap(searcher)
            For Each id As String In q.FilterList
                If map.Contains(id) Then b(map(id)) = True
            Next
            narrow = New BitFilter(b, narrow)
        End If

        'If q.FilterListCallback IsNot Nothing Then
        '    narrow = New KeyFilter(q.FilterCacheKey, q.FilterListCallback, narrow)
        'End If

        Dim docs As TopFieldDocs = Nothing
        Dim sort As Sort
        If q.SortBy = String.Empty Then
            Dim sorts As New Generic.List(Of SortField)

            If q.Keywords <> String.Empty Then
                'sort = New Sort(SortField.FIELD_SCORE)
                sorts.Add(SortField.FIELD_SCORE)
            End If
            sorts.Add(New SortField("Thickness", q.SortReverse))
            sorts.Add(New SortField("Width", q.SortReverse))
            sorts.Add(New SortField("Length", q.SortReverse))
            sorts.Add(New SortField("ProductName", q.SortReverse))
            sort = New Sort(sorts.ToArray)
        Else
            Dim sortfields(1) As SortField
            sortfields(0) = New SortField(q.SortBy, q.SortReverse)
            sortfields(1) = SortField.FIELD_SCORE
            sort = New Sort(sortfields)
        End If
        Logger.Info("Before Query")

        docs = searcher.Search(query, narrow, q.MaxDocs, sort)

        Logger.Info("After Query: " & query.ToString())


        Dim scorer As QueryScorer = New QueryScorer(query)
        Dim highlighter As Highlighter = New Highlighter(scorer)
        highlighter.SetTextFragmenter(New SimpleFragmenter(q.BestFragmentCharCount))

        Logger.Info("Before Loop")

        Dim ds As New DataSet
        Dim sr As New SearchResult
        sr.ds = ds
        sr.Count = docs.totalHits
        Dim Results As DataTable = GetDataTable()
        For i As Integer = Math.Max(q.MaxPerPage * (q.PageNo - 1), 0) To Math.Min(Math.Min(q.MaxPerPage * q.PageNo, docs.totalHits) - 1, UBound(docs.scoreDocs))
            If docs.scoreDocs(i) Is Nothing Then
                Continue For
            End If
            Dim doc As Document = searcher.Doc(docs.scoreDocs(i).doc)
            Dim row As DataRow = Results.NewRow()
            For Each f As Field In Fields
                row(f.Name) = IIf(doc.Get(f.Name) = Nothing, DBNull.Value, doc.Get(f.Name))
            Next
            row("Score") = docs.scoreDocs(i).score

            Dim BestFragment As String = doc.Get(q.BestFragmentField)
            If Not BestFragment = String.Empty Then
                Dim stream As TokenStream = New PorterStemmerAnalyzer().TokenStream(q.BestFragmentField, New StringReader(BestFragment))
                row("BestFragment") = highlighter.GetBestFragment(stream, BestFragment)
            End If
            If Convert.IsDBNull(row("BestFragment")) Then row("BestFragment") = BestFragment

            Results.Rows.Add(row)
        Next
        ds.Tables.Add(Results)

        Logger.Info("Before EnsureFacetCache")

        EnsureFacetCache(searcher, q)

        Logger.Info("After EnsureFacetCache")

        Dim NofHits As Integer = docs.totalHits

        'If number of hits is lower than X, then loop through facets and save unique values only
        Dim facetcache As Hashtable = Nothing
        Dim MaxHits As Integer = q.MaxHitsForCache
        If NofHits <= Math.Min(MaxHits, NofHits) AndAlso NofHits > 0 Then
            facetcache = New Hashtable
            For i As Integer = 0 To NofHits - 1
                Dim doc As Document = searcher.Doc(docs.scoreDocs(i).doc)
                For Each f As Facet In q.FacetCache
                    If facetcache(f.Field) Is Nothing Then facetcache(f.Field) = New Hashtable

                    Dim values() As String = doc.GetValues(f.Field)
                    If values Is Nothing Then Continue For

                    For k As Integer = 0 To values.Length - 1
                        If values(k) = String.Empty Then Continue For
                        CType(facetcache(f.Field), Hashtable)(f.Field & ":" & values(k)) = 1
                    Next
                Next
            Next
        End If

        Logger.Info("Before Second Execution (with hitcollector)")

        'Run search one more time but with hitcollector
        Dim CachedMaxDoc As Integer = HttpContext.Current.Cache("facets_maxDoc")
        Dim CachedFacets As Hashtable = HttpContext.Current.Cache("facets")

        Dim MaxMaxDoc As Integer = Math.Max(CachedMaxDoc, searcher.GetIndexReader.MaxDoc())
        Dim bits As BitSet

        Logger.Info("CachedMaxDoc = " & CachedMaxDoc)
        Logger.Info("Document MaxDoc = " & searcher.GetIndexReader.MaxDoc())
        Logger.Info("Max MaxDoc = " & MaxMaxDoc)

        bits = New BitSet(MaxMaxDoc)

        Dim res As CustomHitCollector = New CustomHitCollector(bits)
        If query Is Nothing Then
            'searcher.Search(bq, Nothing, res)
            searcher.Search(bq, narrow, res)
        Else
            searcher.Search(query, narrow, res)
        End If

        Logger.Info("After Second Execution (with hitcollector)")

        Dim FacetResult As String = String.Empty
        If Not q.Facets Is Nothing Then

            'Preprocessing to apply narrow selection
            For Each f As Facet In q.Facets
                If f.Ranges Is Nothing Then
                    If Not f.Narrow = String.Empty Then
                        Dim value As String = f.Narrow
                        Dim cachebitset As BitSet = CachedFacets(f.Field)(f.Field & ":" & value)
                        If cachebitset Is Nothing Then
                            Continue For
                        End If
                        Dim bitset As BitSet
                        If MaxMaxDoc = cachebitset.Length Then
                            bitset = cachebitset
                        Else
                            bitset = New BitSet(cachebitset, MaxMaxDoc)
                        End If
                        bits = bits.And(bitset)
                    End If
                End If
            Next

            For Each f As Facet In q.Facets
                Dim dt As DataTable = GetFacetDataTable(f.Name)
                Dim MaxDocs As Integer = IIf(f.MaxDocs = 0, q.MaxDocs, f.MaxDocs)

                ds.Tables.Add(dt)

                If NofHits = 0 Then
                    Continue For
                End If

                If Not f.ParentName = String.Empty AndAlso f.ParentValue = String.Empty Then
                    Continue For
                End If

                If Not f.Ranges Is Nothing Then
                    Dim bitsetvalues As Hashtable = CachedFacets(f.Field)
                    For Each r As Range In f.Ranges
                        Dim cachebitset As BitSet = CType(bitsetvalues(r.LBound & r.UBound), BitSet)
                        Dim bitset As BitSet
                        If MaxMaxDoc = cachebitset.Length Then
                            bitset = cachebitset
                        Else
                            bitset = New BitSet(cachebitset, MaxMaxDoc)
                        End If
                        Dim count As Integer = bitset.CardinalityIntersect(bits)

                        If f.ZerosIncluded OrElse count > 0 Then
                            'Insert facet to resultset
                            Dim row As DataRow = dt.NewRow()
                            row("Name") = f.Name
                            row("Label") = r.Label
                            row("Value") = r.Value
                            row("Count") = count
                            dt.Rows.Add(row)
                        End If
                    Next
                Else
                    Dim bitsetvalues As Hashtable = CachedFacets(f.Field)
                    Dim i As Integer = 0

                    Dim keys As ICollection = bitsetvalues.Keys
                    If Not facetcache Is Nothing Then
                        keys = CType(facetcache(f.Field), Hashtable).Keys
                    End If

                    For Each key As String In keys
                        'If facet is narrowed, then ignore other facets from the same field
                        If f.Name <> "SupplyPhase" And Not f.Narrow = String.Empty Then
                            Dim narrowvalue As String = f.Narrow
                            If InStr(key, f.Field & ":" & narrowvalue) <= 0 Then
                                Continue For
                            End If
                        End If
                        Dim cachebitset As BitSet = CType(bitsetvalues(key), BitSet)
                        Dim bitset As BitSet
                        If MaxMaxDoc = cachebitset.Length Then
                            bitset = cachebitset
                        Else
                            bitset = New BitSet(cachebitset, MaxMaxDoc)
                        End If
                        Dim process As Boolean = False
                        If Not facetcache Is Nothing Then
                            If CType(facetcache(f.Field), Hashtable)(key) = 1 Then
                                process = True
                            End If
                        Else
                            process = True
                        End If
                        If process Then
                            Dim count As Integer = bitset.CardinalityIntersect(bits)

                            If f.ZerosIncluded OrElse count > 0 Then
                                'Insert facet to resultset
                                Dim row As DataRow = dt.NewRow()
                                row("Name") = f.Name

                                Dim pipe As String = InStrRev(key, "|")
                                Dim value As String = Right(key, Len(key) - Len(f.Field) - 1)
                                row("Value") = value
                                If pipe > 0 Then
                                    value = Right(key, Len(key) - pipe)
                                End If
                                row("Label") = value

                                row("Count") = count
                                dt.Rows.Add(row)

                                i += 1
                            End If
                        End If
                        If i >= MaxDocs Then Exit For
                    Next
                End If
            Next
        End If
        Logger.Info("After processing facets")

        Return sr
    End Function

    Private Sub EnsureFacetCache(ByVal searcher As IndexSearcher, ByVal q As IndexQuery)
        'Save in the cache object
        If Not HttpContext.Current.Cache("facets") Is Nothing AndAlso Not q.ForceRefresh Then
            Logger.Info("Cache is still active")
            Exit Sub
        End If

        Dim reader As IndexReader = searcher.Reader
        Dim NumDocs As Integer = reader.MaxDoc

        Logger.Info("RefreshFacetCache: Start")

        Dim facetCache As Hashtable = New Hashtable
        Dim tDocs As TermDocs = reader.TermDocs()
        Dim NofFacets As Integer = 0
        For Each f As Facet In q.FacetCache
            Dim FieldFacets As Integer = 0
            If Not f.Ranges Is Nothing Then
                Dim bitsetValues As Hashtable = New Hashtable
                For Each r As Range In f.Ranges
                    Dim bool As New BooleanQuery
                    Dim rq As New RangeQuery(New Term(f.Name, r.LBound), New Term(f.Name, r.UBound), True)
                    Dim bits As New BitSet(NumDocs)
                    Dim res As CustomHitCollector = New CustomHitCollector(bits)
                    searcher.Search(rq, Nothing, res)
                    bitsetValues.Add(r.LBound & r.UBound, bits)
                    FieldFacets += 1
                Next
                facetCache.Add(f.Field, bitsetValues)
                Logger.Info("Field facets for " & f.Field & ": " & FieldFacets)
            Else
                Dim te As TermEnum = reader.Terms(New Term(f.Field, ""))
                Dim bitsetValues As Hashtable = New Hashtable
                Do
                    Dim t As Term = te.Term
                    If (t Is Nothing OrElse Not t.Field.Equals(f.Field)) Then
                        Exit Do
                    End If
                    tDocs.Seek(t)
                    Dim bitSet As BitSet = New BitSet(NumDocs)
                    While tDocs.Next()
                        bitSet.Set(tDocs.Doc(), True)
                    End While
                    bitsetValues.Add(t.ToString, bitSet)
                    FieldFacets += 1
                Loop While te.Next()
                facetCache.Add(f.Field, bitsetValues)
                Logger.Info("Field facets for " & f.Field & ": " & FieldFacets)
            End If
            NofFacets += FieldFacets
        Next
        Logger.Info("Number of facets: " & NofFacets)

        HttpContext.Current.Cache.Insert("facets", facetCache, Nothing, DateAdd(DateInterval.Second, q.FacetCacheDuration, Now), Nothing)
        HttpContext.Current.Cache.Insert("facets_maxDoc", NumDocs, Nothing, DateAdd(DateInterval.Second, q.FacetCacheDuration, Now), Nothing)

    End Sub

End Class

Public Class DictionaryDef
    Public Name As String
    Public Fields() As String

    Public Function Exists(ByVal s As String) As Boolean
        If Fields Is Nothing Then Return False
        For i As Integer = LBound(Fields) To UBound(Fields) - 1
            If Fields(i).ToLower = s.ToLower Then
                Return True
            End If
        Next
    End Function

End Class

Public Class Field
    Public Name As String = String.Empty
    Public Type As String = String.Empty
    Public FieldType As String = String.Empty
    Public StripHTML As Boolean = False
    Public Delete As Boolean = False
    Public Boost As Integer = 1
    Public PrimaryKey As Boolean = False

    Public Shared Function RemoveSpecialCharacters(ByVal sInput As String) As String
        If sInput = String.Empty Then
            Return String.Empty
        End If

        sInput = Replace(sInput, ":", "")
        sInput = Replace(sInput, "<", "")
        sInput = Replace(sInput, ">", "")
        sInput = Replace(sInput, "=", "")
        sInput = Replace(sInput, "+", "")
        sInput = Replace(sInput, "@", "")
        sInput = Replace(sInput, Chr(34), "")
        sInput = Replace(sInput, "%", "")
        sInput = Replace(sInput, "&", "")
        sInput = Replace(sInput, "/", "")
        sInput = Replace(sInput, ".", "")
        sInput = Replace(sInput, "_", "")
        sInput = Replace(sInput, "-", "")
        sInput = Replace(sInput, "#", "")

        Return sInput
    End Function
End Class

Public Class CustomHitCollector
    Inherits HitCollector

    Private bits As BitSet

    Public Sub New(ByVal bits As BitSet)
        Me.bits = bits
    End Sub

    Public Overloads Overrides Sub Collect(ByVal doc As Integer, ByVal score As Single)
        bits.Set(doc, True)
    End Sub
End Class
