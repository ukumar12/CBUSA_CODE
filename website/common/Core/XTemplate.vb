Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Net.Mail
Imports System.Text.RegularExpressions
Imports System.Collections.Specialized

Namespace Components

    Class XTemplate

        ' FILE CONTENT
        Private m_sFileContent As String = String.Empty

        ' BLOCK DICTIONARY
        Private m_dctBlockContents As NameValueCollection
        Private m_dctVars As NameValueCollection
        Private m_dctBlocks As NameValueCollection
        Private m_aContent As String()

        '**
        '	Property Content
        '***
        Public Property Content() As String
            Get
                Return m_sFileContent
            End Get
            Set(ByVal value As String)
                m_sFileContent = value
                MakeTree()
            End Set
        End Property

        '**
        '	Initialize class
        '***
        Public Sub New()
            m_dctBlockContents = New NameValueCollection
            m_dctVars = New NameValueCollection
            m_dctBlocks = New NameValueCollection
        End Sub

        '**
        '	Method loads file and store it in m_sFile
        '***
        Public Sub Load(ByVal FileName As String)
            ' CHECK IF FILENAME IS NOT EMPTY
            If FileName = String.Empty Then
                Throw New ApplicationException("File Name Expected")
            End If

            ' LOAD CONTENT OF THE FILE
            If Not Core.FileExists(FileName) Then
                Throw New ApplicationException("File Not Found")
            End If

            Dim f As StreamReader = New StreamReader(FileName)
            m_sFileContent = f.ReadToEnd
            f.Close()
            f.Dispose()

            MakeTree()
        End Sub

        '**
        '	Create tree of blocks
        '***
        Private Sub MakeTree()
            Dim iPos1 As Integer, iPos2 As Integer, iSize As Integer
            Dim oMatches As MatchCollection
            Dim sCurrentBlock As String, sParentBlock As String, sBlock As String
            Dim iLoop As Integer, iPos As Integer
            Dim bFound As Boolean
            Dim sBlockContent As String

            ' FIND EVERY OCCURENCE OF <!-- IN THE CONTENT
            iSize = 0
            iPos1 = InStr(1, m_sFileContent, "<!--")
            If iPos1 <> 0 Then
                While InStr(iPos1 + 5, m_sFileContent, "<!--") <> 0
                    iPos2 = InStr(iPos1 + 5, m_sFileContent, "<!--")
                    iSize = iSize + 1

                    ReDim Preserve m_aContent(iSize)
                    m_aContent(iSize) = Mid(m_sFileContent, iPos1 + 5, iPos2 - iPos1 - 5)

                    iPos1 = iPos2
                End While
            End If

            ' INITIALIZE REGLAR EXPRESSION OBJECT
            sCurrentBlock = ""
            sParentBlock = ""
            For iLoop = 1 To iSize
                oMatches = Regex.Matches(m_aContent(iLoop), "(BEGIN:|END:)\s*([0-9a-zA-Z_]+)\s*(-->|//-->)((\r\n|\n)*)", RegexOptions.IgnoreCase)
                bFound = False
                For Each oMatch As Match In oMatches
                    bFound = True
                    If oMatch.Index = 0 Then
                        If InStr(oMatch.Value, "BEGIN:") = 1 Then

                            ' GET BLOCK NAME AND BLOCK CONTENT

                            sBlock = Trim(Mid(oMatch.Value, 8, InStrRev(oMatch.Value, " ") - 8))
                            sBlockContent = Mid(m_aContent(iLoop), oMatch.Length + 1, Len(m_aContent(iLoop)) - oMatch.Length + 1)

                            sParentBlock = sCurrentBlock
                            If Not sCurrentBlock = String.Empty Then
                                sCurrentBlock = sCurrentBlock & "."
                            End If
                            sCurrentBlock = sCurrentBlock & sBlock

                            If Not sParentBlock = String.Empty Then
                                m_dctBlockContents(sParentBlock) = m_dctBlockContents(sParentBlock) & "{_BLOCK_." & sCurrentBlock & "}"
                            End If
                            If Not sCurrentBlock = String.Empty Then
                                m_dctBlockContents(sCurrentBlock) = sBlockContent
                            End If

                        ElseIf InStr(oMatch.Value, "END:") = 1 Then

                            ' GET BLOCK NAME AND BLOCK CONTENT
                            sBlock = Trim(Mid(oMatch.Value, 6, InStrRev(oMatch.Value, " ") - 5))
                            sBlockContent = Mid(m_aContent(iLoop), oMatch.Length + 1, Len(m_aContent(iLoop)) - oMatch.Length + 1)

                            sCurrentBlock = sParentBlock
                            iPos = InStrRev(sParentBlock, ".")

                            If iPos = 0 Then
                                sParentBlock = ""
                            Else
                                sParentBlock = Left(sParentBlock, iPos - 1)
                            End If
                            If Not sCurrentBlock = String.Empty Then
                                m_dctBlockContents(sCurrentBlock) = m_dctBlockContents(sCurrentBlock) & sBlockContent
                            End If
                        End If
                    End If
                Next

                ' PATTERN NOT FOUND IN COMMENTS
                ' ADD TO CURRENT BLOCK, IF NOT EMPTY
                If Not bFound Then
                    If Not sCurrentBlock = String.Empty Then
                        m_dctBlockContents(sCurrentBlock) = m_dctBlockContents(sCurrentBlock) & "<!-- " & m_aContent(iLoop)
                    End If
                End If
            Next
        End Sub

        '**
        '	Get Block
        '***
        Public ReadOnly Property GetBlock(ByVal sBlock As String) As String
            Get
                If sBlock = String.Empty Then
                    Throw New ApplicationException("Block Name Expected")
                End If
                Return m_dctBlocks(sBlock)
            End Get
        End Property

        '**
        '	Get Block
        '***
        Public ReadOnly Property GetBlockDef(ByVal sBlock) As String
            Get
                If sBlock = String.Empty Then
                    Throw New ApplicationException("Block Name Expected")
                End If
                Return m_dctBlockContents(sBlock)
            End Get
        End Property

        '**
        '	Assign value
        '***
        Public Sub Assign(ByVal sKey As String, ByVal sValue As String)
            If sKey = String.Empty Then
                Throw New ApplicationException("Key Name Expected")
            End If
            m_dctVars(sKey) = sValue
        End Sub

        '**
        '	Parse block
        '***
        Public Sub Parse(ByVal sBlock As String)
            If sBlock = String.Empty Then
                Throw New ApplicationException("Block Name Expected")
            End If

            Dim sBlockContent As String = m_dctBlockContents(sBlock)
            If sBlockContent = String.Empty Then
                Throw New ApplicationException("Block Not Found")
            End If

            ' INITIALIZE REGLAR EXPRESSION OBJECT
            Dim oMatches As MatchCollection = Regex.Matches(sBlockContent, "\{([A-Za-z0-9_.]+)\}", RegexOptions.IgnoreCase)
            For Each oMatch As Match In oMatches
                Dim sValue As String = oMatch.Value
                sValue = Mid(sValue, 2, Len(sValue) - 2)

                If Mid(sValue, 1, 7) = "_BLOCK_" Then
                    sBlockContent = Replace(sBlockContent, oMatch.Value, m_dctBlocks(Right(sValue, Len(sValue) - 8)))
                    m_dctBlocks(Right(sValue, Len(sValue) - 8)) = ""
                Else
                    sBlockContent = Replace(sBlockContent, oMatch.Value, Trim(m_dctVars(sValue)))
                End If
            Next
            m_dctBlocks(sBlock) = m_dctBlocks(sBlock) & sBlockContent
        End Sub
    End Class

End Namespace