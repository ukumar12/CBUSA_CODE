Imports SpellChecker.Net.Search
Imports Lucene.Net.Store
Imports System.Text
Imports System.IO
Imports SpellChecker.Net.Search.Spell
Imports Lucene.Net.Index
Imports Lucene.Net.Search
Imports Components

Public Class SpellCheck
    Private m_Index As SearchIndex
    Private m_Directory As String

    Public Property Index() As SearchIndex
        Get
            Return m_Index
        End Get
        Set(ByVal value As SearchIndex)
            m_Index = value
        End Set
    End Property

    Public Property Directory() As String
        Get
            Return m_Directory
        End Get
        Set(ByVal value As String)
            m_Directory = value
        End Set
    End Property

    Public Sub New()
    End Sub

    Public Sub New(ByVal index As SearchIndex)
        Me.Index = index
        Directory = index.Directory
    End Sub

    Public Sub New(ByVal directory As String)
        Me.Directory = directory
    End Sub

    Public Function SuggestSimilar(ByVal keyword As String) As String
        Dim spellcheckdirectory As String = Index.Directory & "\Live\" & Index.IndexName & "\spellcheck"
        Dim fsdir As FSDirectory = FSDirectory.GetDirectory(spellcheckdirectory, Not IndexReader.IndexExists(spellcheckdirectory))
        Dim dictdirectory As String = Index.Directory & "\Live\" & Index.IndexName & "\dictionary"
        Dim dictdir As FSDirectory = FSDirectory.GetDirectory(dictdirectory, Not IndexReader.IndexExists(dictdirectory))
        Dim dictionaryreader As IndexReader = IndexReader.Open(dictdir)
        Dim sc As Spell.SpellChecker = New Spell.SpellChecker(fsdir)
        Dim words() As String = keyword.Split(" "c)
        Dim sb As New StringBuilder
        Dim conn As String = ""
        Dim Fields() As String = Nothing
        Dim counter As Integer = 0

        If Index.Fields.Count = 0 Then Index.ParseDefinition()
        For i As Integer = LBound(words) To UBound(words)
            Dim word As String = words(i).ToLower
            sc.SetAccuracy(0.5)
            Dim similar() As String = sc.SuggestSimilar(word, 1, dictionaryreader, Index.Dictionary.Name, True)
            If similar.Length > 0 Then
                sb.Append(conn & similar(0))
                conn = " "
            End If
        Next
        fsdir.Close()
        dictionaryreader.Close()
        dictdir.Close()

        Return sb.ToString()
    End Function

    Public Sub ReindexSpellchecker()
        Dim spellcheckdirectory As String = Index.Directory & "\Live\" & Index.IndexName & "\spellcheck"
        Dim dictionarydirectory As String = Index.Directory & "\Live\" & Index.IndexName & "\dictionary"

        Dim writer As IndexWriter = New IndexWriter(spellcheckdirectory, New PorterStemmerAnalyzer, Not IndexReader.IndexExists(spellcheckdirectory))
        writer.Close()

        Dim fsdir As FSDirectory = FSDirectory.GetDirectory(spellcheckdirectory, Not IndexReader.IndexExists(spellcheckdirectory))
        Dim sc As Spell.SpellChecker = New Spell.SpellChecker(fsdir)
        sc.ClearIndex()

        Dim dictionaryreader As IndexReader = IndexReader.Open(dictionarydirectory)
        sc.IndexDictionary(New LuceneDictionary(dictionaryreader, Index.Dictionary.Name))
        dictionaryreader.Close()
        fsdir.Close()
    End Sub

End Class

