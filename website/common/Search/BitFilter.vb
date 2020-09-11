Imports Components
Imports IDevSearch
Imports Lucene.Net.Search

Public Class BitFilter
    Inherits Filter

    Private m_Bits As BitArray
    Private m_BaseFilter As Filter

    Public Sub New(ByVal bits As BitArray, Optional ByVal baseFilter As Filter = Nothing)
        m_Bits = bits
        m_BaseFilter = baseFilter
    End Sub

    Public Overrides Function Bits(ByVal reader As Lucene.Net.Index.IndexReader) As System.Collections.BitArray
        If m_BaseFilter IsNot Nothing Then
            Dim baseBits As BitArray = m_BaseFilter.Bits(reader)
            If baseBits.Count > m_Bits.Count Then
                m_Bits.Length = baseBits.Count
            End If
            Return m_Bits.And(baseBits)
        Else
            Return m_Bits
        End If
    End Function
End Class
