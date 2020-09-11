Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ProjectRow
        Inherits ProjectRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ProjectID As Integer)
            MyBase.New(DB, ProjectID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ProjectID As Integer) As ProjectRow
            Dim row As ProjectRow

            row = New ProjectRow(DB, ProjectID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ProjectID As Integer)
            Dim row As ProjectRow

            row = New ProjectRow(DB, ProjectID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from Project"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetBuilderProjects(ByVal DB As Database, ByVal BuilderId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim sql As String = "select * from Project where BuilderId=" & DB.Number(BuilderId)
            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy) & " " & Core.ProtectParam(SortOrder)
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetBuilderOrderProjects(ByVal DB As Database, ByVal BuilderID As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim sql As String = "select * from Project where BuilderId=" & DB.Number(BuilderID) & " and (ProjectID in (select ProjectID from [Order] o where o.BuilderID=" & DB.Number(BuilderID) & ")"
            sql &= " or ProjectID in (select ProjectID from [twopriceorder] o where o.BuilderID=" & DB.Number(BuilderID) & "))"

            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy) & " " & Core.ProtectParam(SortOrder)
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetProjectProducts(ByVal DB As Database, ByVal ProjectId As Integer, Optional ByVal VendorId As Integer = Nothing) As DataTable
            Dim sql As String = _
                  " select pd.*, tp.Quantity, t.Title, tp.SortOrder, sop.SpecialOrderProduct " _
                & " from TakeOffProduct tp left outer join Product pd on pd.ProductId=tp.ProductId " _
                & "     inner join TakeOff t on tp.TakeoffID=t.TakeoffID" _
                & "     left outer join SpecialOrderProduct sop on tp.SpecialOrderProductId=sop.SpecialOrderProductId" _
                & " where" _
                & "     t.ProjectId=" & DB.Number(ProjectId) _
                & " union all " _
                & " select po.*, op.Quantity, o.Title, op.SortOrder, sopo.SpecialOrderProduct " _
                & " from OrderProduct op left outer join Product po on po.ProductID=op.ProductID " _
                & "     inner join [Order] o on op.OrderID=o.OrderID" _
                & "     left outer join SpecialOrderProduct sopo on op.SpecialOrderProductId=sopo.SpecialOrderProductId " _
                & " where" _
                & "     o.ProjectId=" & DB.Number(ProjectId) _
                & " order by Title ASC, SortOrder ASC"

            If VendorId <> Nothing Then
                sql = "select v.VendorSku, v.VendorPrice, tmp.* from VendorProductPrice v inner join (" & sql & ") as tmp on v.ProductId=tmp.ProductId where v.VendorID=" & DB.Number(VendorId) & " order by Title ASC, SortOrder ASC"
            End If
            Return DB.GetDataTable(sql)
        End Function

    End Class

    Public MustInherit Class ProjectRowBase
        Private m_DB As Database
        Private m_ProjectID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_ProjectName As String = Nothing
        Private m_LotNumber As String = Nothing
        Private m_Subdivision As String = Nothing
        Private m_Address As String = Nothing
        Private m_Address2 As String = Nothing
        Private m_City As String = Nothing
        Private m_State As String = Nothing
        Private m_Zip As String = Nothing
        Private m_County As String = Nothing
        Private m_PortfolioID As Integer = Nothing
        Private m_ProjectStatusID As Integer = Nothing
        Private m_StartDate As DateTime = Nothing
        Private m_IsArchived As Boolean = Nothing
        Private m_Submitted As DateTime = Nothing
        Private m_ContactName As String = Nothing
        Private m_ContactEmail As String = Nothing
        Private m_ContactPhone As String = Nothing


        Public Property ProjectID() As Integer
            Get
                Return m_ProjectID
            End Get
            Set(ByVal Value As Integer)
                m_ProjectID = value
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

        Public Property ProjectName() As String
            Get
                Return m_ProjectName
            End Get
            Set(ByVal Value As String)
                m_ProjectName = value
            End Set
        End Property

        Public Property LotNumber() As String
            Get
                Return m_LotNumber
            End Get
            Set(ByVal Value As String)
                m_LotNumber = value
            End Set
        End Property

        Public Property Subdivision() As String
            Get
                Return m_Subdivision
            End Get
            Set(ByVal Value As String)
                m_Subdivision = value
            End Set
        End Property

        Public Property Address() As String
            Get
                Return m_Address
            End Get
            Set(ByVal Value As String)
                m_Address = value
            End Set
        End Property

        Public Property Address2() As String
            Get
                Return m_Address2
            End Get
            Set(ByVal Value As String)
                m_Address2 = value
            End Set
        End Property

        Public Property City() As String
            Get
                Return m_City
            End Get
            Set(ByVal Value As String)
                m_City = value
            End Set
        End Property

        Public Property State() As String
            Get
                Return m_State
            End Get
            Set(ByVal Value As String)
                m_State = value
            End Set
        End Property

        Public Property Zip() As String
            Get
                Return m_Zip
            End Get
            Set(ByVal Value As String)
                m_Zip = value
            End Set
        End Property

        Public Property County() As String
            Get
                Return m_County
            End Get
            Set(ByVal Value As String)
                m_County = value
            End Set
        End Property

        Public Property PortfolioID() As Integer
            Get
                Return m_PortfolioID
            End Get
            Set(ByVal Value As Integer)
                m_PortfolioID = value
            End Set
        End Property

        Public Property ProjectStatusID() As Integer
            Get
                Return m_ProjectStatusID
            End Get
            Set(ByVal Value As Integer)
                m_ProjectStatusID = value
            End Set
        End Property

        Public Property StartDate() As DateTime
            Get
                Return m_StartDate
            End Get
            Set(ByVal Value As DateTime)
                m_StartDate = value
            End Set
        End Property

        Public Property IsArchived() As Boolean
            Get
                Return m_IsArchived
            End Get
            Set(ByVal Value As Boolean)
                m_IsArchived = value
            End Set
        End Property

        Public ReadOnly Property Submitted() As DateTime
            Get
                Return m_Submitted
            End Get
        End Property

        Public Property ContactName() As String
            Get
                Return m_ContactName
            End Get
            Set(ByVal Value As String)
                m_ContactName = value
            End Set
        End Property

        Public Property ContactEmail() As String
            Get
                Return m_ContactEmail
            End Get
            Set(ByVal Value As String)
                m_ContactEmail = value
            End Set
        End Property

        Public Property ContactPhone() As String
            Get
                Return m_ContactPhone
            End Get
            Set(ByVal Value As String)
                m_ContactPhone = value
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

        Public Sub New(ByVal DB As Database, ByVal ProjectID As Integer)
            m_DB = DB
            m_ProjectID = ProjectID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Project WHERE ProjectID = " & DB.Number(ProjectID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ProjectID = Convert.ToInt32(r.Item("ProjectID"))
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            m_ProjectName = Convert.ToString(r.Item("ProjectName"))
            If IsDBNull(r.Item("LotNumber")) Then
                m_LotNumber = Nothing
            Else
                m_LotNumber = Convert.ToString(r.Item("LotNumber"))
            End If
            If IsDBNull(r.Item("Subdivision")) Then
                m_Subdivision = Nothing
            Else
                m_Subdivision = Convert.ToString(r.Item("Subdivision"))
            End If
            m_Address = Convert.ToString(r.Item("Address"))
            If IsDBNull(r.Item("Address2")) Then
                m_Address2 = Nothing
            Else
                m_Address2 = Convert.ToString(r.Item("Address2"))
            End If
            m_City = Convert.ToString(r.Item("City"))
            m_State = Convert.ToString(r.Item("State"))
            m_Zip = Convert.ToString(r.Item("Zip"))
            m_County = Convert.ToString(r.Item("County"))
            If IsDBNull(r.Item("PortfolioID")) Then
                m_PortfolioID = Nothing
            Else
                m_PortfolioID = Convert.ToInt32(r.Item("PortfolioID"))
            End If
            m_ProjectStatusID = Convert.ToInt32(r.Item("ProjectStatusID"))
            If IsDBNull(r.Item("StartDate")) Then
                m_StartDate = Nothing
            Else
                m_StartDate = Convert.ToDateTime(r.Item("StartDate"))
            End If
            m_IsArchived = Convert.ToBoolean(r.Item("IsArchived"))
            m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
            m_ContactName = Core.GetString(r.Item("ContactName"))
            m_ContactEmail = Core.GetString(r.Item("ContactEmail"))
            m_ContactPhone = Core.GetString(r.Item("ContactPhone"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO Project (" _
             & " BuilderID" _
             & ",ProjectName" _
             & ",LotNumber" _
             & ",Subdivision" _
             & ",Address" _
             & ",Address2" _
             & ",City" _
             & ",State" _
             & ",Zip" _
             & ",County" _
             & ",PortfolioID" _
             & ",ProjectStatusID" _
             & ",StartDate" _
             & ",IsArchived" _
             & ",Submitted" _
             & ",ContactName" _
             & ",ContactEmail" _
             & ",ContactPhone" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.Quote(ProjectName) _
             & "," & m_DB.Quote(LotNumber) _
             & "," & m_DB.Quote(Subdivision) _
             & "," & m_DB.Quote(Address) _
             & "," & m_DB.Quote(Address2) _
             & "," & m_DB.Quote(City) _
             & "," & m_DB.Quote(State) _
             & "," & m_DB.Quote(Zip) _
             & "," & m_DB.Quote(County) _
             & "," & m_DB.NullNumber(PortfolioID) _
             & "," & m_DB.NullNumber(ProjectStatusID) _
             & "," & m_DB.NullQuote(StartDate) _
             & "," & CInt(IsArchived) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Quote(ContactName) _
             & "," & m_DB.Quote(ContactEmail) _
             & "," & m_DB.Quote(ContactPhone) _
             & ")"

            ProjectID = m_DB.InsertSQL(SQL)

            Return ProjectID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Project SET " _
             & " BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",ProjectName = " & m_DB.Quote(ProjectName) _
             & ",LotNumber = " & m_DB.Quote(LotNumber) _
             & ",Subdivision = " & m_DB.Quote(Subdivision) _
             & ",Address = " & m_DB.Quote(Address) _
             & ",Address2 = " & m_DB.Quote(Address2) _
             & ",City = " & m_DB.Quote(City) _
             & ",State = " & m_DB.Quote(State) _
             & ",Zip = " & m_DB.Quote(Zip) _
             & ",County = " & m_DB.Quote(County) _
             & ",PortfolioID = " & m_DB.NullNumber(PortfolioID) _
             & ",ProjectStatusID = " & m_DB.NullNumber(ProjectStatusID) _
             & ",StartDate = " & m_DB.NullQuote(StartDate) _
             & ",IsArchived = " & CInt(IsArchived) _
             & ",ContactName = " & m_DB.Quote(ContactName) _
             & ",ContactEmail = " & m_DB.Quote(ContactEmail) _
             & ",ContactPhone = " & m_DB.Quote(ContactPhone) _
             & " WHERE ProjectID = " & m_DB.Quote(ProjectID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM Project WHERE ProjectID = " & m_DB.Number(ProjectID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ProjectCollection
        Inherits GenericCollection(Of ProjectRow)
    End Class

End Namespace


