Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorSpecialOrderProductPriceRow
        Inherits VendorSpecialOrderProductPriceRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal SpecialOrderProductId As Integer)
            MyBase.New(DB, VendorID, SpecialOrderProductId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal SpecialOrderProductId As Integer) As VendorSpecialOrderProductPriceRow
            Dim row As VendorSpecialOrderProductPriceRow

            row = New VendorSpecialOrderProductPriceRow(DB, VendorID, SpecialOrderProductId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal SpecialOrderProductId As Integer)
            Dim row As VendorSpecialOrderProductPriceRow

            row = New VendorSpecialOrderProductPriceRow(DB, VendorID, SpecialOrderProductId)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorSpecialOrderProductPrice"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class VendorSpecialOrderProductPriceRowBase
        Private m_DB As Database
        Private m_VendorID As Integer = Nothing
        Private m_SpecialOrderProductID As Integer = Nothing
        Private m_VendorSKU As String = Nothing
        Private m_VendorPrice As Double = Nothing
        Private m_IsSubstitution As Boolean = Nothing
        Private m_Submitted As DateTime = Nothing
        Private m_SubmitterVendorAccountID As Integer = Nothing


        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
            End Set
        End Property

        Public Property SpecialOrderProductID() As Integer
            Get
                Return m_SpecialOrderProductID
            End Get
            Set(ByVal Value As Integer)
                m_SpecialOrderProductID = value
            End Set
        End Property

        Public Property VendorSKU() As String
            Get
                Return m_VendorSKU
            End Get
            Set(ByVal Value As String)
                m_VendorSKU = value
            End Set
        End Property

        Public Property VendorPrice() As Double
            Get
                Return m_VendorPrice
            End Get
            Set(ByVal Value As Double)
                m_VendorPrice = value
            End Set
        End Property

        Public Property IsSubstitution() As Boolean
            Get
                Return m_IsSubstitution
            End Get
            Set(ByVal Value As Boolean)
                m_IsSubstitution = value
            End Set
        End Property

        Public ReadOnly Property Submitted() As DateTime
            Get
                Return m_Submitted
            End Get
        End Property

        Public Property SubmitterVendorAccountID() As Integer
            Get
                Return m_SubmitterVendorAccountID
            End Get
            Set(ByVal Value As Integer)
                m_SubmitterVendorAccountID = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal SpecialOrderProductId As Integer)
            m_DB = DB
            m_VendorID = VendorID
            m_SpecialOrderProductID = SpecialOrderProductId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorSpecialOrderProductPrice WHERE VendorID = " & DB.Number(VendorID) & " and SpecialOrderProductId=" & DB.Number(SpecialOrderProductID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            m_SpecialOrderProductID = Convert.ToInt32(r.Item("SpecialOrderProductID"))
            If IsDBNull(r.Item("VendorSKU")) Then
                m_VendorSKU = Nothing
            Else
                m_VendorSKU = Convert.ToString(r.Item("VendorSKU"))
            End If
            If IsDBNull(r.Item("VendorPrice")) Then
                m_VendorPrice = Nothing
            Else
                m_VendorPrice = Convert.ToDouble(r.Item("VendorPrice"))
            End If
            m_IsSubstitution = Convert.ToBoolean(r.Item("IsSubstitution"))
            m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
            m_SubmitterVendorAccountID = Convert.ToInt32(r.Item("SubmitterVendorAccountID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorSpecialOrderProductPrice (" _
             & " SpecialOrderProductID" _
             & ",VendorId" _
             & ",VendorSKU" _
             & ",VendorPrice" _
             & ",IsSubstitution" _
             & ",Submitted" _
             & ",SubmitterVendorAccountID" _
             & ") VALUES (" _
             & m_DB.NullNumber(SpecialOrderProductID) _
             & "," & m_DB.Number(VendorID) _
             & "," & m_DB.Quote(VendorSKU) _
             & "," & m_DB.Number(VendorPrice) _
             & "," & CInt(IsSubstitution) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(SubmitterVendorAccountID) _
             & ")"

            m_DB.ExecuteSQL(SQL)

            Return VendorID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorSpecialOrderProductPrice SET " _
             & " VendorPrice = " & m_DB.Number(VendorPrice) _
             & ",IsSubstitution = " & CInt(IsSubstitution) _
             & ",SubmitterVendorAccountID = " & m_DB.NullNumber(SubmitterVendorAccountID) _
             & " WHERE VendorID = " & m_DB.Quote(VendorID) _
             & " AND SpecialOrderProductId = " & m_DB.Quote(SpecialOrderProductID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorSpecialOrderProductPrice WHERE VendorID = " & m_DB.Number(VendorID) & " and SpecialOrderProductId=" & m_DB.Number(SpecialOrderProductID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorSpecialOrderProductPriceCollection
        Inherits GenericCollection(Of VendorSpecialOrderProductPriceRow)
    End Class

End Namespace


