Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AdminDocumentRow
        Inherits AdminDocumentRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AdminDocumentID As Integer)
            MyBase.New(DB, AdminDocumentID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AdminDocumentID As Integer) As AdminDocumentRow
            Dim row As AdminDocumentRow

            row = New AdminDocumentRow(DB, AdminDocumentID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AdminDocumentID As Integer)
            Dim row As AdminDocumentRow

            row = New AdminDocumentRow(DB, AdminDocumentID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AdminDocument"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetAllRecipients(ByVal DB As Database, ByVal AdminDocumentID As Integer) As DataTable
            Dim sql As String = _
                "select b.BuilderID as ID, (select CompanyName from Builder where BuilderID=b.BuilderID) as CompanyName, 'Builder' as Audience from AdminDocumentBuilderRecipient b where b.AdminDocumentID=" & DB.Number(AdminDocumentID) _
                & " union " _
                & "select v.VendorID as ID, (select CompanyName from Vendor where VendorID=v.VendorID) as CompanyName, 'Vendor' as Audience from AdminDocumentVendorRecipient v where v.AdminDocumentID=" & DB.Number(AdminDocumentID) _
                & " union " _
                & "select p.PIQID as ID, (select CompanyName from PIQ where PIQID=p.PIQID) as CompanyName, 'PIQ' as Audience from AdminDocumentPIQRecipient p where p.AdminDocumentID=" & DB.Number(AdminDocumentID)

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Sub ClearAllRecipients(ByVal DB As Database, ByVal AdminDocumentID As Integer)
            DB.ExecuteSQL("delete from AdminDocumentBuilderRecipient where AdminDocumentID=" & DB.Number(AdminDocumentID))
            DB.ExecuteSQL("delete from AdminDocumentPIQRecipient where AdminDocumentID=" & DB.Number(AdminDocumentID))
            DB.ExecuteSQL("delete from AdminDocumentVendorRecipient where AdminDocumentID=" & DB.Number(AdminDocumentID))
        End Sub


        Public Shared Sub RemoveAllLLCRecipients(ByVal DB As Database, ByVal AdminDocumentID As Integer)
            DB.ExecuteSQL("delete from AdminDocumentLLC where AdminDocumentID=" & DB.Number(AdminDocumentID))
            DB.ExecuteSQL("delete from AdminDocumentDocumentAudienceType where AdminDocumentID=" & DB.Number(AdminDocumentID))
            DB.ExecuteSQL("delete from AdminDocumentBuilderRecipient where AdminDocumentID=" & DB.Number(AdminDocumentID))
            DB.ExecuteSQL("delete from AdminDocumentPIQRecipient where AdminDocumentID=" & DB.Number(AdminDocumentID))
            DB.ExecuteSQL("delete from AdminDocumentVendorRecipient where AdminDocumentID=" & DB.Number(AdminDocumentID))
        End Sub

    End Class

    Public MustInherit Class AdminDocumentRowBase
        Private m_DB As Database
        Private m_AdminDocumentID As Integer = Nothing
        Private m_AdminID As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_FileName As String = Nothing
        Private m_FileGUID As String = Nothing
        Private m_Uploaded As DateTime = Nothing
        Private m_IsApproved As Boolean = Nothing
        Private m_DocumentHistoryNotes As String = Nothing

        Public Property AdminDocumentID() As Integer
            Get
                Return m_AdminDocumentID
            End Get
            Set(ByVal Value As Integer)
                m_AdminDocumentID = value
            End Set
        End Property

        Public Property AdminID() As Integer
            Get
                Return m_AdminID
            End Get
            Set(ByVal Value As Integer)
                m_AdminID = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = value
            End Set
        End Property

        Public Property FileName() As String
            Get
                Return m_FileName
            End Get
            Set(ByVal Value As String)
                m_FileName = value
            End Set
        End Property

        Public Property FileGUID() As String
            Get
                Return m_FileGUID
            End Get
            Set(ByVal Value As String)
                m_FileGUID = value
            End Set
        End Property

        Public Property Uploaded() As DateTime
            Get
                Return m_Uploaded
            End Get
            Set(ByVal Value As DateTime)
                m_Uploaded = value
            End Set
        End Property

        Public Property IsApproved() As Boolean
            Get
                Return m_IsApproved
            End Get
            Set(ByVal Value As Boolean)
                m_IsApproved = value
            End Set
        End Property

        Public Property DocumentHistoryNotes() As String
            Get
                Return m_DocumentHistoryNotes
            End Get
            Set(ByVal Value As String)
                m_DocumentHistoryNotes = Value
            End Set
        End Property

        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As DataBase)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AdminDocumentID As Integer)
            m_DB = DB
            m_AdminDocumentID = AdminDocumentID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AdminDocument WHERE AdminDocumentID = " & DB.Number(AdminDocumentID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_AdminDocumentID = Convert.ToInt32(r.Item("AdminDocumentID"))
            m_AdminID = Convert.ToInt32(r.Item("AdminID"))
            m_Title = Convert.ToString(r.Item("Title"))
            m_FileName = Convert.ToString(r.Item("FileName"))
            m_FileGUID = Convert.ToString(r.Item("FileGUID"))
            m_Uploaded = Convert.ToDateTime(r.Item("Uploaded"))
            m_IsApproved = Convert.ToBoolean(r.Item("IsApproved"))
            m_DocumentHistoryNotes = Convert.ToString(r.Item("DocumentHistoryNotes"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer

            Dim SQL As String


            SQL = " INSERT INTO AdminDocument (" _
             & " AdminID" _
             & ",Title" _
             & ",FileName" _
             & ",FileGUID" _
             & ",Uploaded" _
             & ",IsApproved" _
             & ",DocumentHistoryNotes" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminID) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.Quote(FileName) _
             & "," & m_DB.Quote(FileGUID) _
             & "," & m_DB.NullQuote(Uploaded) _
             & "," & CInt(IsApproved) _
              & "," & m_DB.Quote(DocumentHistoryNotes) _
             & ")"

            AdminDocumentID = m_DB.InsertSQL(SQL)

            Return AdminDocumentID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AdminDocument SET " _
             & " AdminID = " & m_DB.NullNumber(AdminID) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",FileName = " & m_DB.Quote(FileName) _
             & ",FileGUID = " & m_DB.Quote(FileGUID) _
             & ",Uploaded = " & m_DB.NullQuote(Uploaded) _
             & ",IsApproved = " & CInt(IsApproved) _
             & ",DocumentHistoryNotes = " & m_DB.Quote(DocumentHistoryNotes) _
             & " WHERE AdminDocumentID = " & m_DB.Quote(AdminDocumentID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AdminDocument WHERE AdminDocumentID = " & m_DB.Number(AdminDocumentID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove



        Public Sub InsertToList(ByVal DocumentAudienceTypeID As Integer)
            Dim SQL As String = "if not exists (select AdminDocumentID from AdminDocumentDocumentAudienceType where AdminDocumentID =" & AdminDocumentID & " and DocumentAudienceTypeID =" & DocumentAudienceTypeID & ") begin insert into AdminDocumentDocumentAudienceType (AdminDocumentID, DocumentAudienceTypeID) values (" & AdminDocumentID & "," & DocumentAudienceTypeID & ") end"
            DB.ExecuteSQL(SQL)
        End Sub
        Public Sub DeleteFromAllLists()
            DB.ExecuteSQL("delete from AdminDocumentDocumentAudienceType where AdminDocumentID=" & AdminDocumentID)
        End Sub
        Public Sub DeleteFromAdminDocumentLLCLists()
            DB.ExecuteSQL("delete from AdminDocumentLLC where AdminDocumentID=" & AdminDocumentID)
        End Sub
        Public Sub InsertToLists(ByVal Lists As String)
            If Lists = String.Empty Then Exit Sub

            Dim aLists As String()
            aLists = Lists.Split(",")
            For Each s As String In aLists
                InsertToList(CInt(s))
            Next
        End Sub
        Public Sub InsertToAdminDocumentLLCList(ByVal DocumentAudienceLLCID As Integer)
            Dim SQL As String = "if not exists (select AdminDocumentID from AdminDocumentLLC where AdminDocumentID =" & AdminDocumentID & " and LLCID =" & DocumentAudienceLLCID & ") begin insert into AdminDocumentLLC (AdminDocumentID, LLCID) values (" & AdminDocumentID & "," & DocumentAudienceLLCID & ") end"
            DB.ExecuteSQL(SQL)
        End Sub
        Public Sub InsertToAdminDocumentLLCLists(ByVal Lists As String)
            If Lists = String.Empty Then Exit Sub

            Dim aLists As String()
            aLists = Lists.Split(",")
            For Each s As String In aLists
                InsertToAdminDocumentLLCList(CInt(s))
            Next
        End Sub


        Public ReadOnly Property DocumentAudiences() As String
            Get
                Dim sdr As SqlDataReader = Me.DB.GetReader("select DocumentAudienceTypeId from AdminDocumentDocumentAudienceType where AdminDocumentID =" & Me.AdminDocumentID)
                Dim d As String = String.Empty
                Dim ret As String = String.Empty

                While sdr.Read()
                    ret &= d & sdr("DocumentAudienceTypeId")
                    d = ","
                End While
                sdr.Close()
                Return ret
            End Get
        End Property

        Public ReadOnly Property DocumentLLCAudiences() As String
            Get
                Dim sdr As SqlDataReader = Me.DB.GetReader("select LLCID from AdminDocumentLLC where AdminDocumentID =" & Me.AdminDocumentID)
                Dim d As String = String.Empty
                Dim ret As String = String.Empty

                While sdr.Read()
                    ret &= d & sdr("LLCID")
                    d = ","
                End While
                sdr.Close()
                Return ret
            End Get
        End Property
    End Class

    Public Class AdminDocumentCollection
        Inherits GenericCollection(Of AdminDocumentRow)
    End Class

End Namespace


