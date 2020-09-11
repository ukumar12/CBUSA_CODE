Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BuilderPricingInformationRow
        Inherits BuilderPricingInformationRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderPricingInformationID As Integer)
            MyBase.New(DB, BuilderPricingInformationID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderPricingInformationID As Integer) As BuilderPricingInformationRow
            Dim row As BuilderPricingInformationRow

            row = New BuilderPricingInformationRow(DB, BuilderPricingInformationID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderPricingInformationID As Integer)
            Dim row As BuilderPricingInformationRow

            row = New BuilderPricingInformationRow(DB, BuilderPricingInformationID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from BuilderPricingInformation"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class BuilderPricingInformationRowBase
        Private m_DB As Database
        Private m_BuilderPricingInformationID As Integer = Nothing
        Private m_AdminID As Integer = Nothing
        Private m_PricingInformation As String = Nothing
        Private m_Updated As DateTime = Nothing


        Public Property BuilderPricingInformationID() As Integer
            Get
                Return m_BuilderPricingInformationID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderPricingInformationID = value
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

        Public Property PricingInformation() As String
            Get
                Return m_PricingInformation
            End Get
            Set(ByVal Value As String)
                m_PricingInformation = value
            End Set
        End Property

        Public Property Updated() As DateTime
            Get
                Return m_Updated
            End Get
            Set(ByVal Value As DateTime)
                m_Updated = value
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

        Public Sub New(ByVal DB As Database, ByVal BuilderPricingInformationID As Integer)
            m_DB = DB
            m_BuilderPricingInformationID = BuilderPricingInformationID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BuilderPricingInformation WHERE BuilderPricingInformationID = " & DB.Number(BuilderPricingInformationID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BuilderPricingInformationID = Convert.ToInt32(r.Item("BuilderPricingInformationID"))
            m_AdminID = Convert.ToInt32(r.Item("AdminID"))
            m_PricingInformation = Convert.ToString(r.Item("PricingInformation"))
            m_Updated = Convert.ToDateTime(r.Item("Updated"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO BuilderPricingInformation (" _
             & " AdminID" _
             & ",PricingInformation" _
             & ",Updated" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminID) _
             & "," & m_DB.Quote(PricingInformation) _
             & "," & m_DB.NullQuote(Updated) _
             & ")"

            BuilderPricingInformationID = m_DB.InsertSQL(SQL)

            Return BuilderPricingInformationID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BuilderPricingInformation SET " _
             & " AdminID = " & m_DB.NullNumber(AdminID) _
             & ",PricingInformation = " & m_DB.Quote(PricingInformation) _
             & ",Updated = " & m_DB.NullQuote(Updated) _
             & " WHERE BuilderPricingInformationID = " & m_DB.quote(BuilderPricingInformationID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BuilderPricingInformation WHERE BuilderPricingInformationID = " & m_DB.Number(BuilderPricingInformationID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BuilderPricingInformationCollection
        Inherits GenericCollection(Of BuilderPricingInformationRow)
    End Class

End Namespace

