Imports Lucene.Net.Analysis
Imports Lucene.Net.Analysis.Standard
Imports Components

Public Class PorterStemmerAnalyzer
    Inherits Analyzer

    Private Shared ReadOnly STOP_WORDS As String()

    Shared Sub New()
        STOP_WORDS = StopAnalyzer.ENGLISH_STOP_WORDS
    End Sub

    Public Overrides Function TokenStream(ByVal fieldName As String, ByVal reader As System.IO.TextReader) As TokenStream
        Dim result As TokenStream = New StandardTokenizer(reader)
        result = New StandardFilter(result)
        result = New LowerCaseFilter(result)
        result = New PorterStemFilter(result)
        result = New StopFilter(result, STOP_WORDS)
        Return result
    End Function

End Class
