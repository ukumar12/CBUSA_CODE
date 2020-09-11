Imports Lucene.Net.QueryParsers
Imports Lucene.Net.Analysis.Standard
Imports Lucene.Net.Analysis
Imports Lucene.Net.Search
Imports Lucene.Net.Store
Imports Components
Imports System.Text
Imports Lucene.Net.Index

Public Class CustomQueryParser
    Inherits QueryParser

    Private Shared Function GetQuery(ByVal Field As String, ByVal keywords As String, ByVal a As Analyzer, ByVal DefaultOperator As String) As Query
        Dim parser As QueryParser = New QueryParser(Field, a)
        If DefaultOperator = "AND" Then
            parser.SetDefaultOperator(QueryParser.AND_OPERATOR)
        Else
            parser.SetDefaultOperator(QueryParser.OR_OPERATOR)
        End If
        Dim q As Query
        If keywords.Contains("*") Then
            q = New WildcardQuery(New Term(Field, keywords.ToLower))
        Else
            q = parser.Parse(keywords)
        End If
        Return q
    End Function

    Public Overloads Shared Function Parse(ByVal keywords As String, ByVal Fields As GenericCollection(Of Field), ByVal DefaultAnalyzer As Analyzer, ByVal DefaultOperator As String) As Query
        Dim sb As New StringBuilder

        If keywords = String.Empty Then
            Return New Lucene.Net.Search.MatchAllDocsQuery()
        End If

        Dim Replaced As String = Field.RemoveSpecialCharacters(keywords)

        Dim bq As New BooleanQuery
        For Each f As Field In Fields
            Select Case LCase(f.FieldType)
                'Parse only fields with type of string
                Case "text", "unstored"
                    If LCase(f.Type) = "string" Then
                        Dim q As Query = GetQuery(f.Name, keywords, New KeywordAnalyzer, "OR")
                        bq.Add(q, BooleanClause.Occur.SHOULD)
                    End If
                Case "keyword"
                    Dim q As Query = GetQuery(f.Name, keywords, New KeywordAnalyzer, "OR")
                    bq.Add(q, BooleanClause.Occur.SHOULD)
            End Select
        Next
        Return bq
    End Function

    Public Sub New(ByVal field As String, ByVal a As Analyzer)
        MyBase.New(field, a)
    End Sub

End Class
