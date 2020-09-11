Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorRatingCategoryRatingRow
        Inherits VendorRatingCategoryRatingRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal BuilderID As Integer, ByVal RatingCategoryID As Integer)
            MyBase.New(DB, VendorID, BuilderID, RatingCategoryID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal BuilderID As Integer, ByVal RatingCategoryID As Integer) As VendorRatingCategoryRatingRow
            Dim row As VendorRatingCategoryRatingRow

            row = New VendorRatingCategoryRatingRow(DB, VendorID, BuilderID, RatingCategoryID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal BuilderID As Integer, ByVal RatingCategoryID As Integer)
            Dim row As VendorRatingCategoryRatingRow

            row = New VendorRatingCategoryRatingRow(DB, VendorID, BuilderID, RatingCategoryID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorRatingCategoryRating"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetVendorRatings(ByVal DB As Database, ByVal VendorID As Integer, ByVal BuilderID As Integer) As DataTable
            Dim sql As String = _
                " select c.*, coalesce(r.Rating,0) as VendorRating from RatingCategory c left outer join (select * from VendorRatingCategoryRating where VendorID=" & DB.Number(VendorID) & " and BuilderID=" & DB.Number(BuilderID) & ") as r on c.RatingCategoryID=r.RatingCategoryID order by c.RatingCategory"
            Return DB.GetDataTable(sql)
        End Function
    End Class

    Public MustInherit Class VendorRatingCategoryRatingRowBase
        Private m_DB As Database
        Private m_VendorID As Integer = Nothing
        Private m_RatingCategoryID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_Rating As Integer = Nothing


        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
            End Set
        End Property

        Public Property RatingCategoryID() As Integer
            Get
                Return m_RatingCategoryID
            End Get
            Set(ByVal Value As Integer)
                m_RatingCategoryID = value
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

        Public Property Rating() As Integer
            Get
                Return m_Rating
            End Get
            Set(ByVal Value As Integer)
                m_Rating = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal BuilderID As Integer, ByVal RatingCategoryID As Integer)
            m_DB = DB
            m_VendorID = VendorID
            m_BuilderID = BuilderID
            m_RatingCategoryID = RatingCategoryID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorRatingCategoryRating WHERE VendorID = " & DB.Number(VendorID) & " and BuilderID=" & DB.Number(BuilderID) & " and RatingCategoryID=" & DB.Number(RatingCategoryID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            m_RatingCategoryID = Convert.ToInt32(r.Item("RatingCategoryID"))
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            m_Rating = Convert.ToInt32(r.Item("Rating"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorRatingCategoryRating (" _
             & " RatingCategoryID" _
             & ",BuilderID" _
             & ",VendorID" _
             & ",Rating" _
             & ") VALUES (" _
             & m_DB.NullNumber(RatingCategoryID) _
             & "," & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Number(Rating) _
             & ")"

            m_DB.ExecuteSQL(SQL)

            Return VendorID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorRatingCategoryRating SET " _
             & " Rating = " & m_DB.Number(Rating) _
             & " WHERE VendorID = " & m_DB.Quote(VendorID) _
             & " AND BuilderID = " & m_DB.Quote(BuilderID) _
             & " AND RatingCategoryID = " & m_DB.Quote(RatingCategoryID)


            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorRatingCategoryRating WHERE VendorID = " & m_DB.Number(VendorID) & " and BuilderID=" & DB.Number(BuilderID) & " and RatingCategoryID=" & DB.Number(RatingCategoryID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorRatingCategoryRatingCollection
        Inherits GenericCollection(Of VendorRatingCategoryRatingRow)
    End Class

End Namespace

