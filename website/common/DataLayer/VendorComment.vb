Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorCommentRow
        Inherits VendorCommentRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorCommentID As Integer)
            MyBase.New(DB, VendorCommentID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorCommentID As Integer) As VendorCommentRow
            Dim row As VendorCommentRow

            row = New VendorCommentRow(DB, VendorCommentID)
            row.Load()

            Return row
        End Function

        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal BuilderID As Integer) As VendorCommentRow
            Dim row As New VendorCommentRow(DB)
            Dim sql As String = "select * from vendorcomment where vendorid=" & DB.Number(VendorID) & " and builderid=" & DB.Number(BuilderID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                row.Load(sdr)
            End If
            sdr.Close()
            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorCommentID As Integer)
            Dim row As VendorCommentRow

            row = New VendorCommentRow(DB, VendorCommentID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorComment"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetVendorComments(ByVal DB As Database, ByVal VendorID As Integer) As DataTable
            Dim sql As String = "select vc.*, b.CompanyName from VendorComment vc inner join Builder b on vc.BuilderID=b.BuilderID where VendorID=" & DB.Number(VendorID) & " order by Submitted Desc"
            Return DB.GetDataTable(sql)
        End Function
    End Class

    Public MustInherit Class VendorCommentRowBase
        Private m_DB As Database
        Private m_VendorCommentID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_Comment As String = Nothing
        Private m_Submitted As DateTime = Nothing


        Public Property VendorCommentID() As Integer
            Get
                Return m_VendorCommentID
            End Get
            Set(ByVal Value As Integer)
                m_VendorCommentID = value
            End Set
        End Property

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
            End Set
        End Property

        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = value
            End Set
        End Property

        Public Property Comment() As String
            Get
                Return m_Comment
            End Get
            Set(ByVal Value As String)
                m_Comment = value
            End Set
        End Property

        Public ReadOnly Property Submitted() As DateTime
            Get
                Return m_Submitted
            End Get
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

        Public Sub New(ByVal DB As Database, ByVal VendorCommentID As Integer)
            m_DB = DB
            m_VendorCommentID = VendorCommentID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorComment WHERE VendorCommentID = " & DB.Number(VendorCommentID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorCommentID = Convert.ToInt32(r.Item("VendorCommentID"))
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            m_Comment = Convert.ToString(r.Item("Comment"))
            m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorComment (" _
             & " VendorID" _
             & ",BuilderID" _
             & ",Comment" _
             & ",Submitted" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.Quote(Comment) _
             & "," & m_DB.NullQuote(Now) _
             & ")"

            VendorCommentID = m_DB.InsertSQL(SQL)

            Return VendorCommentID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorComment SET " _
             & " VendorID = " & m_DB.NullNumber(VendorID) _
             & ",BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",Comment = " & m_DB.Quote(Comment) _
             & ",Submitted = " & m_DB.NullQuote(Now) _
             & " WHERE VendorCommentID = " & m_DB.Quote(VendorCommentID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorComment WHERE VendorCommentID = " & m_DB.Number(VendorCommentID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorCommentCollection
        Inherits GenericCollection(Of VendorCommentRow)
    End Class

End Namespace


