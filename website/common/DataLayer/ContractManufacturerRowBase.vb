Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class ContractManufacturerRowBase
        Private m_DB As Database
        Private m_ContractManufacturerID As Integer = Nothing
        Private m_HistoricID As Integer = Nothing
        Private m_ClassID As String = Nothing
        Private m_Contact As String = Nothing
        Private m_Name As String = Nothing
        Private m_Address As String = Nothing
        Private m_City As String = Nothing
        Private m_State As String = Nothing
        Private m_Zip As String = Nothing
        Private m_Phone As String = Nothing
        Private m_Fax As String = Nothing
        Private m_PaymentTerms As String = Nothing
        Private m_Email As String = Nothing

        Public Property ContractManufacturerID As Integer
            Get
                Return m_ContractManufacturerID
            End Get
            Set(ByVal Value As Integer)
                m_ContractManufacturerID = value
            End Set
        End Property

        Public Property HistoricID As Integer
            Get
                Return m_HistoricID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricID = value
            End Set
        End Property

        Public Property ClassID As String
            Get
                Return m_ClassID
            End Get
            Set(ByVal Value As String)
                m_ClassID = value
            End Set
        End Property

        Public Property Contact As String
            Get
                Return m_Contact
            End Get
            Set(ByVal Value As String)
                m_Contact = value
            End Set
        End Property

        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = value
            End Set
        End Property

        Public Property Address As String
            Get
                Return m_Address
            End Get
            Set(ByVal Value As String)
                m_Address = value
            End Set
        End Property

        Public Property City As String
            Get
                Return m_City
            End Get
            Set(ByVal Value As String)
                m_City = value
            End Set
        End Property

        Public Property State As String
            Get
                Return m_State
            End Get
            Set(ByVal Value As String)
                m_State = value
            End Set
        End Property

        Public Property Zip As String
            Get
                Return m_Zip
            End Get
            Set(ByVal Value As String)
                m_Zip = value
            End Set
        End Property

        Public Property Phone As String
            Get
                Return m_Phone
            End Get
            Set(ByVal Value As String)
                m_Phone = value
            End Set
        End Property

        Public Property Fax As String
            Get
                Return m_Fax
            End Get
            Set(ByVal Value As String)
                m_Fax = value
            End Set
        End Property

        Public Property PaymentTerms As String
            Get
                Return m_PaymentTerms
            End Get
            Set(ByVal Value As String)
                m_PaymentTerms = value
            End Set
        End Property

        Public Property Email As String
            Get
                Return m_Email
            End Get
            Set(ByVal Value As String)
                m_Email = value
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

        Public Sub New(ByVal DB As Database, ContractManufacturerID As Integer)
            m_DB = DB
            m_ContractManufacturerID = ContractManufacturerID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ContractManufacturer WHERE ContractManufacturerID = " & DB.Number(ContractManufacturerID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_ContractManufacturerID = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ContractManufacturerID = Core.GetInt(r.Item("ContractManufacturerID"))
            m_HistoricID = Core.GetInt(r.Item("HistoricID"))
            m_ClassID = Core.GetString(r.Item("ClassID"))
            m_Contact = Core.GetString(r.Item("Contact"))
            m_Name = Core.GetString(r.Item("Name"))
            m_Address = Core.GetString(r.Item("Address"))
            m_City = Core.GetString(r.Item("City"))
            m_State = Core.GetString(r.Item("State"))
            m_Zip = Core.GetString(r.Item("Zip"))
            m_Phone = Core.GetString(r.Item("Phone"))
            m_Fax = Core.GetString(r.Item("Fax"))
            m_PaymentTerms = Core.GetString(r.Item("PaymentTerms"))
            m_Email = Core.GetString(r.Item("Email"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String
            Dim MaximumHistoricId As Integer = Me.DB.ExecuteScalar("SELECT MAX(HistoricId) FROM (SELECT MAX(HistoricId) HistoricId FROM Builder UNION SELECT MAX(HistoricId) HistoricId FROM Vendor UNION SELECT MAX(HistoricId) HistoricId FROM ContractManufacturer) a")
            MaximumHistoricId = MaximumHistoricId + 1

            SQL = " INSERT INTO ContractManufacturer (" _
             & " HistoricID" _
             & ",ClassID" _
             & ",Contact" _
             & ",Name" _
             & ",Address" _
             & ",City" _
             & ",State" _
             & ",Zip" _
             & ",Phone" _
             & ",Fax" _
             & ",PaymentTerms" _
             & ",Email" _
             & ") VALUES (" _
             & m_DB.NullNumber(MaximumHistoricId) _
             & "," & m_DB.Quote(ClassID) _
             & "," & m_DB.Quote(Contact) _
             & "," & m_DB.Quote(Name) _
             & "," & m_DB.Quote(Address) _
             & "," & m_DB.Quote(City) _
             & "," & m_DB.Quote(State) _
             & "," & m_DB.Quote(Zip) _
             & "," & m_DB.Quote(Phone) _
             & "," & m_DB.Quote(Fax) _
             & "," & m_DB.Quote(PaymentTerms) _
             & "," & m_DB.Quote(Email) _
             & ")"

            ContractManufacturerID = m_DB.InsertSQL(SQL)

            Return ContractManufacturerID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ContractManufacturer WITH (ROWLOCK) SET " _
             & " HistoricID = " & m_DB.NullNumber(HistoricID) _
             & ",ClassID = " & m_DB.Quote(ClassID) _
             & ",Contact = " & m_DB.Quote(Contact) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",Address = " & m_DB.Quote(Address) _
             & ",City = " & m_DB.Quote(City) _
             & ",State = " & m_DB.Quote(State) _
             & ",Zip = " & m_DB.Quote(Zip) _
             & ",Phone = " & m_DB.Quote(Phone) _
             & ",Fax = " & m_DB.Quote(Fax) _
             & ",PaymentTerms = " & m_DB.Quote(PaymentTerms) _
             & ",Email = " & m_DB.Quote(Email) _
             & " WHERE ContractManufacturerID = " & m_DB.quote(ContractManufacturerID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class ContractManufacturerCollection
        Inherits GenericCollection(Of ContractManufacturerRow)
    End Class

End Namespace

