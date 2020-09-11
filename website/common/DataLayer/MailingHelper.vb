Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports System.Collections.Specialized
Imports Components
Imports System.IO

Namespace DataLayer

    Public Class MailingHelper

        Public Shared Function GetHTMLQueryCount(ByVal Db As Database, ByVal dct As NameValueCollection, ByVal Mime As String) As String
            Return GetQuery(Db, dct, Nothing, "HTML", Mime, True)
        End Function

        Public Shared Function GetHTMLQuery(ByVal Db As Database, ByVal dct As NameValueCollection, ByVal MessageId As Integer, ByVal Mime As String) As String
            Return GetQuery(Db, dct, MessageId, "HTML", Mime, False)
        End Function

        Public Shared Function GetTextQueryCount(ByVal Db As Database, ByVal dct As NameValueCollection, ByVal Mime As String) As String
            Return GetQuery(Db, dct, Nothing, "TEXT", Mime, True)
        End Function

        Public Shared Function GetTextQuery(ByVal Db As Database, ByVal dct As NameValueCollection, ByVal MessageId As Integer, ByVal Mime As String) As String
            Return GetQuery(Db, dct, MessageId, "TEXT", Mime, False)
        End Function

        Private Shared Function GetQuery(ByVal Db As Database, ByVal dct As NameValueCollection, ByVal MessageId As Integer, ByVal MimeType As String, ByVal Mime As String, ByVal Count As Boolean) As String
            Dim SQL As String = String.Empty
            If Count Then
                SQL = "SELECT COUNT(*)"
            Else
                SQL = "SELECT " & MessageId & ", MemberId, Email, Name, " & Db.Quote(MimeType) & " AS MimeType"
            End If
            SQL &= " FROM MailingMember m WHERE Email IS NOT NULL AND len(Email) > 0"
            SQL &= " AND Status = 'ACTIVE'"

            'LIST FILTER
            SQL &= " AND MemberId IN (SELECT MemberId FROM MailingListMember WHERE m.MemberId = MemberId AND ListId IN " & Db.NumberMultiple(dct("Lists")) & ")"

            If Mime = "TEXT" Then
                'If the user sends just a TEXT email, then ignore MIME settings in the MAILING_MEMBER table
            Else
                SQL &= " AND MimeType = " & Db.Quote(MimeType)
            End If

            'ADDITIONAL FILTER FIELDS
            If Not Db.IsEmpty(dct("StartDate")) Then
                SQL &= " AND DateDiff(d, CreateDate, " & Db.Quote(dct("StartDate")) & ") <= 0"
            End If
            If Not Db.IsEmpty(dct("EndDate")) Then
                SQL &= " AND DateDiff(d, CreateDate, " & Db.Quote(dct("EndDate")) & ") >= 0"
            End If

            Return SQL
        End Function

        Public Shared Function CreateTempFile(ByVal Path As String, ByVal sBody As String, ByVal sExt As String) As String
            Dim FileName As String
            Dim FullFileName As String

            CreateTempFile = ""

            ' GENERATE UNIQUE .HTM FILE NAME
            FileName = System.Guid.NewGuid().ToString() & "." & sExt

            ' CREATE FULL MAPPED FILE NAME
            FullFileName = Path & "\" & FileName
            Dim sw As StreamWriter = File.CreateText(FullFileName)
            sw.Write(sBody)
            sw.Close()
            CreateTempFile = FileName
        End Function

    End Class

End Namespace


