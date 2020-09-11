Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
Imports System.Linq
	
Namespace DataLayer

	Public Class VendorProductPriceHistoryRow
		Inherits VendorProductPriceHistoryRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, VendorProductPriceHistoryID as Integer)
			MyBase.New(DB, VendorProductPriceHistoryID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal VendorProductPriceHistoryID As Integer) As VendorProductPriceHistoryRow
			Dim row as VendorProductPriceHistoryRow 
			
			row = New VendorProductPriceHistoryRow(DB, VendorProductPriceHistoryID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal VendorProductPriceHistoryID As Integer)
			Dim row as VendorProductPriceHistoryRow 
			
			row = New VendorProductPriceHistoryRow(DB, VendorProductPriceHistoryID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from VendorProductPriceHistory"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods
        Public Shared Function GetPriceHistory(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductID As Integer, Optional ByVal StartDate As DateTime = Nothing, Optional ByVal EndDate As DateTime = Nothing) As DataTable
            Dim dbCurrent As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, VendorID, ProductID)
            'Ticket #199934
            Dim sql As String = "select ProductID ,VendorID ,VendorSKU ,IsSubstitution,VendorPrice ,IsUpload ,SubmitterVendorAccountID ,  DATEADD(MM, -1, DATEDIFF(DD, 0, Submitted)) As Submitted  from VendorProductPriceHistory where VendorID=" & DB.Number(VendorID) & " and ProductID=" & DB.Number(ProductID)
            If StartDate <> Nothing Then
                sql &= " and Submitted >= " & DB.Quote(DateAdd(DateInterval.Month, 1, StartDate))
            End If
            If EndDate <> Nothing Then
                sql &= " and Submitted <= " & DB.Quote(DateAdd(DateInterval.Month, 1, EndDate))
            End If

            If dbCurrent.Updated <> Nothing AndAlso dbCurrent.VendorPrice <> 0 Then
                sql &= " UNION ALL "
                sql &= " Select ProductID ,VendorID ,VendorSKU ,IsSubstitution,VendorPrice ,IsUpload ,  SubmitterVendorAccountID ,   Coalesce( Updated ,Getdate())  As Submitted  from VendorProductPrice   where VendorID=" & DB.Number(VendorID) & " and ProductID = " & DB.Number(ProductID)
            End If

            sql &= " order by Submitted Asc"

            Dim dt As DataTable = DB.GetDataTable(sql)



            If EndDate = Nothing OrElse EndDate >= dbCurrent.Submitted Then
                Dim dr As DataRow = dt.NewRow

                dr("VendorID") = VendorID
                dr("ProductID") = ProductID
                dr("VendorPrice") = dbCurrent.VendorPrice
                dr("Submitted") = Now
                dr("VendorSku") = dbCurrent.VendorSKU
                dr("SubmitterVendorAccountID") = dbCurrent.SubmitterVendorAccountID
                dr("IsSubstitution") = dbCurrent.IsSubstitution
                dr("IsUpload") = dbCurrent.IsUpload

                dt.Rows.Add(dr)
            ElseIf dt.Rows.Count > 0 Then
                Dim lastDate As DateTime = (From dr As DataRow In dt.AsEnumerable Order By dr("Submitted") Descending Select dr("Submitted")).First
                Dim dtPrice As DataTable = DB.GetDataTable("select top 1 * from VendorProductPriceHistory where VendorID=" & DB.Number(VendorID) & " and ProductID=" & DB.Number(ProductID) & " and Submitted > " & DB.Quote(EndDate) & " order by Submitted Asc")
                If dtPrice.Rows.Count > 0 Then
                    Dim dr As DataRow = dt.NewRow

                    dr("VendorID") = VendorID
                    dr("ProductID") = ProductID
                    dr("VendorPrice") = dtPrice.Rows(0)("VendorPrice")
                    dr("Submitted") = StartDate
                    dr("VendorSku") = dtPrice.Rows(0)("VendorSku")
                    dr("SubmitterVendorAccountID") = dtPrice.Rows(0)("SubmitterVendorAccountID")
                    dr("IsSubstitution") = dtPrice.Rows(0)("IsSubstitution")
                    dr("IsUpload") = dtPrice.Rows(0)("IsUpload")

                    dt.Rows.Add(dr)
                End If
            End If

            If StartDate <> Nothing Then
                If dt.Rows.Count > 0 Then
                    Dim firstDate As DateTime = (From dr As DataRow In dt.AsEnumerable Order By dr("Submitted") Ascending Select dr("Submitted")).First
                    If firstDate <> Nothing AndAlso StartDate.Date < firstDate.Date Then
                        Dim dtPrice As DataTable = DB.GetDataTable("select top 1 * from VendorProductPriceHistory where VendorID=" & DB.Number(VendorID) & " and ProductID=" & DB.Number(ProductID) & " and Submitted < " & DB.Quote(StartDate) & " order by Submitted Desc")
                        If dtPrice.Rows.Count > 0 Then
                            Dim dr As DataRow = dt.NewRow
                            dr("VendorID") = VendorID
                            dr("ProductID") = ProductID
                            dr("VendorPrice") = dtPrice.Rows(0)("VendorPrice")
                            dr("Submitted") = StartDate
                            dr("VendorSku") = dtPrice.Rows(0)("VendorSku")
                            dr("SubmitterVendorAccountID") = dtPrice.Rows(0)("SubmitterVendorAccountID")
                            dr("IsSubstitution") = dtPrice.Rows(0)("IsSubstitution")
                            dr("IsUpload") = dtPrice.Rows(0)("IsUpload")

                            dt.Rows.InsertAt(dr, 0)
                        End If
                    End If
                Else
                    Dim dr As DataRow = dt.NewRow

                    dr("VendorID") = VendorID
                    dr("ProductID") = ProductID
                    dr("VendorPrice") = dbCurrent.VendorPrice
                    dr("Submitted") = Now
                    dr("VendorSku") = dbCurrent.VendorSKU
                    dr("SubmitterVendorAccountID") = dbCurrent.SubmitterVendorAccountID
                    dr("IsSubstitution") = dbCurrent.IsSubstitution
                    dr("IsUpload") = dbCurrent.IsUpload

                    dt.Rows.InsertAt(dr, 0)
                End If
            End If
            Return dt
        End Function

        Public Shared Function GetRowByDate(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductID As Integer, ByVal QuoteDate As DateTime) As VendorProductPriceHistoryRow
            Dim row As New VendorProductPriceHistoryRow(DB)
            Dim sql As String = "select top 1 * from VendorProductPriceHistory where VendorID=" & DB.Number(VendorID) & " and ProductID=" & DB.Number(ProductID) & " and Submitted <= " & DB.Quote(Now) & " order by Submitted desc"
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                row.Load(sdr)
            End If
            sdr.Close()
            Return row
        End Function
	End Class
	
	Public MustInherit Class VendorProductPriceHistoryRowBase
		Private m_DB as Database
		Private m_VendorProductPriceHistoryID As Integer = nothing
		Private m_ProductID As Integer = nothing
		Private m_VendorID As Integer = nothing
		Private m_VendorSKU As String = nothing
		Private m_VendorPrice As Double = nothing
		Private m_IsSubstitution As Boolean = nothing
		Private m_IsUpload As Boolean = nothing
		Private m_Submitted As DateTime = nothing
		Private m_SubmitterVendorAccountID As Integer = nothing
	
	
		Public Property VendorProductPriceHistoryID As Integer
			Get
				Return m_VendorProductPriceHistoryID
			End Get
			Set(ByVal Value As Integer)
				m_VendorProductPriceHistoryID = value
			End Set
		End Property
	
		Public Property ProductID As Integer
			Get
				Return m_ProductID
			End Get
			Set(ByVal Value As Integer)
				m_ProductID = value
			End Set
		End Property
	
		Public Property VendorID As Integer
			Get
				Return m_VendorID
			End Get
			Set(ByVal Value As Integer)
				m_VendorID = value
			End Set
		End Property
	
		Public Property VendorSKU As String
			Get
				Return m_VendorSKU
			End Get
			Set(ByVal Value As String)
				m_VendorSKU = value
			End Set
		End Property
	
		Public Property VendorPrice As Double
			Get
				Return m_VendorPrice
			End Get
			Set(ByVal Value As Double)
				m_VendorPrice = value
			End Set
		End Property
	
		Public Property IsSubstitution As Boolean
			Get
				Return m_IsSubstitution
			End Get
			Set(ByVal Value As Boolean)
				m_IsSubstitution = value
			End Set
		End Property
	
		Public Property IsUpload As Boolean
			Get
				Return m_IsUpload
			End Get
			Set(ByVal Value As Boolean)
				m_IsUpload = value
			End Set
		End Property
	
        Public ReadOnly Property Submitted() As DateTime
            Get
                Return m_Submitted
            End Get
        End Property
	
		Public Property SubmitterVendorAccountID As Integer
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
	
		Public Sub New(ByVal DB As Database, VendorProductPriceHistoryID as Integer)
			m_DB = DB
			m_VendorProductPriceHistoryID = VendorProductPriceHistoryID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM VendorProductPriceHistory WHERE VendorProductPriceHistoryID = " & DB.Number(VendorProductPriceHistoryID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_VendorProductPriceHistoryID = Core.GetInt(r.Item("VendorProductPriceHistoryID"))
			m_ProductID = Core.GetInt(r.Item("ProductID"))
			m_VendorID = Core.GetInt(r.Item("VendorID"))
			m_VendorSKU = Core.GetString(r.Item("VendorSKU"))
			m_VendorPrice = Core.GetDouble(r.Item("VendorPrice"))
			m_IsSubstitution = Core.GetBoolean(r.Item("IsSubstitution"))
			m_IsUpload = Core.GetBoolean(r.Item("IsUpload"))
			m_Submitted = Core.GetDate(r.Item("Submitted"))
			m_SubmitterVendorAccountID = Core.GetInt(r.Item("SubmitterVendorAccountID"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
	
            SQL = " INSERT INTO VendorProductPriceHistory (" _
             & " ProductID" _
             & ",VendorID" _
             & ",VendorSKU" _
             & ",VendorPrice" _
             & ",IsSubstitution" _
             & ",IsUpload" _
             & ",Submitted" _
             & ",SubmitterVendorAccountID" _
             & ") VALUES (" _
             & m_DB.NullNumber(ProductID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Quote(VendorSKU) _
             & "," & m_DB.Number(VendorPrice) _
             & "," & CInt(IsSubstitution) _
             & "," & CInt(IsUpload) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(SubmitterVendorAccountID) _
             & ")"

			VendorProductPriceHistoryID = m_DB.InsertSQL(SQL)
			
			Return VendorProductPriceHistoryID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
            SQL = " UPDATE VendorProductPriceHistory SET " _
             & " ProductID = " & m_DB.NullNumber(ProductID) _
             & ",VendorID = " & m_DB.NullNumber(VendorID) _
             & ",VendorSKU = " & m_DB.Quote(VendorSKU) _
             & ",VendorPrice = " & m_DB.Number(VendorPrice) _
             & ",IsSubstitution = " & CInt(IsSubstitution) _
             & ",IsUpload = " & CInt(IsUpload) _
             & " WHERE VendorProductPriceHistoryID = " & m_DB.Quote(VendorProductPriceHistoryID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM VendorProductPriceHistory WHERE VendorProductPriceHistoryID = " & m_DB.Number(VendorProductPriceHistoryID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class VendorProductPriceHistoryCollection
		Inherits GenericCollection(Of VendorProductPriceHistoryRow)
	End Class

End Namespace

