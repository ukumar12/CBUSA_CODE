Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class SupplyPhaseRow
		Inherits SupplyPhaseRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, SupplyPhaseID as Integer)
			MyBase.New(DB, SupplyPhaseID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal SupplyPhaseID As Integer) As SupplyPhaseRow
			Dim row as SupplyPhaseRow 
			
			row = New SupplyPhaseRow(DB, SupplyPhaseID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal SupplyPhaseID As Integer)
			Dim row as SupplyPhaseRow 
			
			row = New SupplyPhaseRow(DB, SupplyPhaseID)
			row.Remove()
		End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC", Optional ByVal Filter() As String = Nothing) As DataTable
            Dim SQL As String = " select * from SupplyPhase "
            If Not Filter Is Nothing Then
                SQL &= " WHERE SupplyPhaseId IN " & DB.NumberMultiple(Filter)
                'Get Parents too TODO - DO THIS RECURSIVELY
                    SQL &= " OR SupplyPhaseID IN (SELECT ParentSupplyPhaseID FROM SupplyPhase WHERE SupplyPhaseID IN " & DB.NumberMultiple(Filter) & ")"
                    SQL &= " OR SupplyPhaseID IN (SELECT ParentSupplyPhaseID FROM SupplyPhase WHERE SupplyPhaseID IN (SELECT ParentSupplyPhaseID FROM SupplyPhase WHERE SupplyPhaseID IN " & DB.NumberMultiple(Filter) & "))"
                    SQL &= " OR SupplyPhaseID IN (SELECT ParentSupplyPhaseID FROM SupplyPhase WHERE SupplyPhaseID IN (SELECT ParentSupplyPhaseID FROM SupplyPhase WHERE SupplyPhaseID IN (SELECT ParentSupplyPhaseID FROM SupplyPhase WHERE SupplyPhaseID IN " & DB.NumberMultiple(Filter) & ")))"
                    SQL &= " OR SupplyPhaseID IN (SELECT ParentSupplyPhaseID FROM SupplyPhase WHERE SupplyPhaseID IN (SELECT ParentSupplyPhaseID FROM SupplyPhase WHERE SupplyPhaseID IN (SELECT ParentSupplyPhaseID FROM SupplyPhase WHERE SupplyPhaseID IN (SELECT ParentSupplyPhaseID FROM SupplyPhase WHERE SupplyPhaseID IN " & DB.NumberMultiple(Filter) & "))))"
            End If
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        Public Shared Function GetListByParentId(ByVal DB As Database, ByVal ParentId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from SupplyPhase"
            If ParentId = Nothing Then
                SQL &= " where ParentSupplyPhaseId is null"
            Else
                SQL &= " where ParentSupplyPhaseId = " & ParentId
            End If
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function


        'Custom Methods

        Public Shared Function GetListNonExluded(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "Select * From SupplyPhase Where SupplyPhaseId Not In (Select SupplyPhaseId From SupplyPhaseLLCExclusion)"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        Public ReadOnly Property GetSelectedLLCs() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select LLCID from SupplyPhaseLLCExclusion where SupplyPhaseID = " & SupplyPhaseID)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("LLCID")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public Sub DeleteFromAllLLCs()
            DB.ExecuteSQL("delete from SupplyPhaseLLCExclusion where SupplyPhaseID = " & SupplyPhaseID)
        End Sub

        Public Sub InsertToLLCs(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToLLC(Element)
            Next
        End Sub

        Public Sub InsertToLLC(ByVal LLCID As Integer)
            Dim SQL As String = "insert into SupplyPhaseLLCExclusion (SupplyPhaseID, LLCID) values (" & SupplyPhaseID & "," & LLCID & ")"
            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Function GetAncestors(ByVal DB As Database, ByVal SupplyPhaseId As Integer) As DataTable
            Dim sql As String = _
                  "WITH Parents(ParentSupplyPhaseId,SupplyPhaseId,SupplyPhase,Depth) AS " _
                & "(" _
                & " SELECT ParentSupplyPhaseId, SupplyPhaseId, SupplyPhase, 0 AS Depth FROM SupplyPhase WHERE SupplyPhaseId=" & DB.Number(SupplyPhaseId) _
                & " UNION ALL " _
                & " SELECT sp.ParentSupplyPhaseId, sp.SupplyPhaseId, sp.SupplyPhase, (Parents.Depth + 1) AS Depth FROM SupplyPhase sp INNER JOIN Parents ON sp.SupplyPhaseId=Parents.ParentSupplyPhaseId" _
                & " )" _
                & " SELECT SupplyPhaseId, SupplyPhase, Depth from Parents ORDER BY Depth Desc"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetNames(ByVal DB As Database, ByVal ids As String, Optional ByVal Delimiter As String = "|") As String
            Dim sql As String = "select SupplyPhase from SupplyPhase where SupplyPhaseId in " & DB.NumberMultiple(ids)
            Dim out As New StringBuilder()
            Dim conn As String = String.Empty
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                out.Append(conn & sdr("SupplyPhase"))
                conn = Delimiter
            End While
            sdr.Close()
            Return out.ToString
        End Function

        Public Shared Function GetRootSupplyPhase(ByVal DB As Database) As SupplyPhaseRow
            Dim out As New SupplyPhaseRow(DB)
            Dim sql As String = "select * from SupplyPhase where ParentSupplyPhaseID is null"
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetChildren(ByVal DB As Database, ByVal ParentID As Integer, Optional ByVal ExcludeLLCIDs As String = Nothing) As DataTable
            Dim sql As String = "select * from SupplyPhase where ParentSupplyPhaseID=" & DB.Number(ParentID)
            If ExcludeLLCIDs <> Nothing Then
                sql &= " and SupplyPhaseID not in (select SupplyPhaseID from SupplyPhaseLLCExclusion where LLCID in " & DB.NumberMultiple(ExcludeLLCIDs) & ")"
            End If
            sql &= " order by SupplyPhase"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetVendorSupplyPhaseString(ByVal DB As Database, ByVal VendorID As Integer, Optional ByVal TopLevelOnly As Boolean = False) As String
            Dim sql As String = "select SupplyPhaseID from VendorSupplyPhase where VendorID=" & DB.Number(VendorID)
            If TopLevelOnly Then
                sql &= " and SupplyPhaseID in (select SupplyPhaseID from SupplyPhase where ParentSupplyPhaseID=" & DB.Number(GetRootSupplyPhase(DB).SupplyPhaseID) & ")"
            End If
            Dim out As New StringBuilder
            Dim conn As String = String.Empty
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                out.Append(conn & sdr("SupplyPhaseID"))
                conn = ","
            End While
            sdr.Close()
            Return out.ToString
        End Function

        Public Shared Function GetVendorPhaseProducts(ByVal DB As Database, ByVal VendorId As Integer) As DataTable
            Dim sql As String = _
                " with Parents(SupplyPhaseId, ParentSupplyPhaseId) as (" _
                & "     select SupplyPhaseId, ParentSupplyPhaseId from SupplyPhase where SupplyPhaseId in (select SupplyPhaseId from VendorSupplyPhase where VendorId=" & DB.Number(VendorId) & ")" _
                & "     union all " _
                & "     select SupplyPhaseId, ParentSupplyPhaseId from SupplyPhase sp inner join Parents p on sp.ParentSupplyPhaseId=p.SupplyPhaseId" _
                & " )" _
                & " select ProductId from SupplyPhaseProduct spp where spp.SupplyPhaseId in (select distinct SupplyPhaseId from Parents)"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetSupplierPhaseList(ByVal DB As Database, ByVal ProductID As String) As DataTable
            Dim SQL As String = "select * from supplyphaseproduct WHERE productid=" & ProductID
            Return DB.GetDataTable(SQL)
        End Function

        Public Shared Function GetLLCPricingList(ByVal DB As Database, ByVal ProductID As String) As DataTable
            Dim SQL As String = "select * from LLCProductPriceRequirement LPP INNER JOIN LLC ON LLC.LLCID=LPP.LLCID WHERE LPP.productid=" & ProductID
            Return DB.GetDataTable(SQL)
        End Function

        
    End Class

   
	
	Public MustInherit Class SupplyPhaseRowBase
		Private m_DB as Database
		Private m_SupplyPhaseID As Integer = nothing
		Private m_SupplyPhase As String = nothing
		Private m_ParentSupplyPhaseID As Integer = nothing
		Private m_PriceLockDays As Integer = nothing
	
	
		Public Property SupplyPhaseID As Integer
			Get
				Return m_SupplyPhaseID
			End Get
			Set(ByVal Value As Integer)
				m_SupplyPhaseID = value
			End Set
		End Property
	
		Public Property SupplyPhase As String
			Get
				Return m_SupplyPhase
			End Get
			Set(ByVal Value As String)
				m_SupplyPhase = value
			End Set
		End Property
	
		Public Property ParentSupplyPhaseID As Integer
			Get
				Return m_ParentSupplyPhaseID
			End Get
			Set(ByVal Value As Integer)
				m_ParentSupplyPhaseID = value
			End Set
		End Property
	
		Public Property PriceLockDays As Integer
			Get
				Return m_PriceLockDays
			End Get
			Set(ByVal Value As Integer)
				m_PriceLockDays = value
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
	
		Public Sub New(ByVal DB As Database, SupplyPhaseID as Integer)
			m_DB = DB
			m_SupplyPhaseID = SupplyPhaseID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM SupplyPhase WHERE SupplyPhaseID = " & DB.Number(SupplyPhaseID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_SupplyPhaseID = Convert.ToInt32(r.Item("SupplyPhaseID"))
			m_SupplyPhase = Convert.ToString(r.Item("SupplyPhase"))
			if IsDBNull(r.Item("ParentSupplyPhaseID")) then
				m_ParentSupplyPhaseID = nothing
			else
				m_ParentSupplyPhaseID = Convert.ToInt32(r.Item("ParentSupplyPhaseID"))
			end if	
			m_PriceLockDays = Convert.ToInt32(r.Item("PriceLockDays"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
			SQL = " INSERT INTO SupplyPhase (" _
				& " SupplyPhase" _
				& ",ParentSupplyPhaseID" _
				& ",PriceLockDays" _
				& ") VALUES (" _
				& m_DB.Quote(SupplyPhase) _
				& "," & m_DB.NullNumber(ParentSupplyPhaseID) _
				& "," & m_DB.Number(PriceLockDays) _
				& ")"

			SupplyPhaseID = m_DB.InsertSQL(SQL)
			
			Return SupplyPhaseID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE SupplyPhase SET " _
				& " SupplyPhase = " & m_DB.Quote(SupplyPhase) _
				& ",ParentSupplyPhaseID = " & m_DB.NullNumber(ParentSupplyPhaseID) _
				& ",PriceLockDays = " & m_DB.Number(PriceLockDays) _
				& " WHERE SupplyPhaseID = " & m_DB.quote(SupplyPhaseID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String

            SQL = "DELETE FROM SupplyPhaseLLCExclusion WHERE SupplyPhaseId=" & m_DB.Number(SupplyPhaseID)
            m_DB.ExecuteSQL(SQL)

			SQL = "DELETE FROM SupplyPhase WHERE SupplyPhaseID = " & m_DB.Number(SupplyPhaseID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class SupplyPhaseCollection
		Inherits GenericCollection(Of SupplyPhaseRow)
	End Class

End Namespace

