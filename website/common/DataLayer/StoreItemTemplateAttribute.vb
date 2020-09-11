Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreItemTemplateAttributeRow
        Inherits StoreItemTemplateAttributeRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TemplateAttributeId As Integer)
            MyBase.New(DB, TemplateAttributeId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TemplateAttributeId As Integer) As StoreItemTemplateAttributeRow
            Dim row As StoreItemTemplateAttributeRow

            row = New StoreItemTemplateAttributeRow(DB, TemplateAttributeId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TemplateAttributeId As Integer)
            Dim row As StoreItemTemplateAttributeRow

            row = New StoreItemTemplateAttributeRow(DB, TemplateAttributeId)
            row.Remove()
        End Sub

        'Custom Methods
		Public Shared Function GetTemplateAttributes(ByVal DB As Database, ByVal TemplateId As Integer, Optional ByVal SortOrder As Integer = Nothing) As DataTable
			Return DB.GetDataTable("SELECT * FROM StoreItemTemplateAttribute WHERE TemplateId=" & TemplateId & IIf(SortOrder > 0, " and SortOrder=" & SortOrder, ""))
		End Function

		Public Function GetChild() As StoreItemTemplateAttributeRow
			Dim TemplateAttributeId As Integer = DB.ExecuteScalar("select top 1 TemplateAttributeId from StoreItemTemplateAttribute where TemplateId=" & TemplateId & " and sortorder = " & SortOrder + 1)
			Return StoreItemTemplateAttributeRow.GetRow(DB, TemplateAttributeId)
		End Function

		Public Shared Function GetChild(ByVal DB As Database, ByVal TemplateId As Integer, ByVal SortOrder As Integer) As StoreItemTemplateAttributeRow
			Dim TemplateAttributeId As Integer = DB.ExecuteScalar("select top 1 TemplateAttributeId from StoreItemTemplateAttribute where TemplateId=" & TemplateId & " and sortorder = " & SortOrder + 1)
			Return StoreItemTemplateAttributeRow.GetRow(DB, TemplateAttributeId)
		End Function

		Public Shared Function GetTemplates(ByVal DB As Database, ByVal TemplateId As Integer, ByVal ParentId As Integer, ByVal Guid As String, ByVal ParentAttributeId As Integer) As DataTable
			Dim SQL As String = "SELECT *, " & ParentAttributeId & " AS ParentTempAttributeId, (SELECT COUNT(*) FROM StoreItemAttributeTemp WHERE Guid = " & DB.Quote(Guid) & " AND TemplateAttributeId = ta.TemplateAttributeId AND COALESCE(ParentAttributeId,0) = " & ParentAttributeId & ") AS iCount FROM StoreItemTemplateAttribute ta WHERE TemplateId = " & TemplateId & " AND COALESCE(ParentId,0) = " & ParentId & " ORDER BY SortOrder"
			Return DB.GetDataTable(SQL)
		End Function

		Public Shared Function GetAdminTemplateAttributes(ByVal DB As Database, ByVal TemplateId As Integer, ByVal ItemId As Integer, ByVal ParentId As Integer, Optional ByVal ParentSortOrder As Integer = Nothing, Optional ByVal ParentAttributeId As Integer = Nothing) As DataTable
			Dim SQL As String = "select *, " & IIf(ParentAttributeId = Nothing, "NULL", ParentAttributeId) & " as ParentTempAttributeId, (select count(*) from storeitemattributetemp where parentattributeid " & IIf(ParentAttributeId = Nothing, " IS NULL", " = " & ParentAttributeId) & " AND ItemId = " & ItemId & ") as iCount from StoreItemTemplateAttribute where templateid = " & TemplateId & " and SortOrder > " & ParentSortOrder & IIf(ParentId = 0, " and ParentId is null", " and parentid = " & ParentId) & " order by SortOrder"
			Return DB.GetDataTable(SQL)
        End Function
    End Class

    Public MustInherit Class StoreItemTemplateAttributeRowBase
        Private m_DB As Database
        Private m_TemplateAttributeId As Integer = Nothing
		Private m_ParentId As Integer = Nothing
        Private m_TemplateId As Integer = Nothing
        Private m_AttributeName As String = Nothing
		Private m_FunctionType As String = Nothing
        Private m_AttributeType As String = Nothing
		Private m_SpecifyValue As String = Nothing
		Private m_LookupTable As String = Nothing
		Private m_LookupColumn As String = Nothing
		Private m_SKUColumn As String = Nothing
		Private m_IncludeSKU As Boolean = Nothing
		Private m_PriceColumn As String = Nothing
		Private m_WeightColumn As String = Nothing
		Private m_SwatchColumn As String = Nothing
		Private m_SwatchAltColumn As String = Nothing
		Private m_IsInventoryManagement As Boolean = Nothing
        Private m_SortOrder As Integer = Nothing


        Public Property TemplateAttributeId() As Integer
            Get
                Return m_TemplateAttributeId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateAttributeId = Value
            End Set
        End Property

		Public Property ParentId() As Integer
			Get
				Return m_ParentId
			End Get
			Set(ByVal value As Integer)
				m_ParentId = value
			End Set
		End Property

        Public Property TemplateId() As Integer
            Get
                Return m_TemplateId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateId = Value
            End Set
        End Property

        Public Property AttributeName() As String
            Get
                Return m_AttributeName
            End Get
            Set(ByVal Value As String)
                m_AttributeName = Value
            End Set
        End Property

		Public Property FunctionType() As String
			Get
				Return m_FunctionType
			End Get
			Set(ByVal Value As String)
				m_FunctionType = Value
			End Set
		End Property

        Public Property AttributeType() As String
            Get
                Return m_AttributeType
            End Get
            Set(ByVal Value As String)
                m_AttributeType = Value
            End Set
        End Property

		Public Property SpecifyValue() As String
			Get
				Return m_SpecifyValue
			End Get
			Set(ByVal Value As String)
				m_SpecifyValue = value
			End Set
		End Property

		Public Property LookupTable() As String
			Get
				Return m_LookupTable
			End Get
			Set(ByVal Value As String)
				m_LookupTable = value
			End Set
		End Property

		Public Property LookupColumn() As String
			Get
				Return m_LookupColumn
			End Get
			Set(ByVal Value As String)
				m_LookupColumn = value
			End Set
		End Property

		Public Property SKUColumn() As String
			Get
				Return m_SKUColumn
			End Get
			Set(ByVal Value As String)
				m_SKUColumn = Value
			End Set
		End Property

		Public Property IncludeSKU() As Boolean
			Get
				Return m_IncludeSKU
			End Get
			Set(ByVal value As Boolean)
				m_IncludeSKU = value
			End Set
		End Property

		Public Property PriceColumn() As String
			Get
				Return m_PriceColumn
			End Get
			Set(ByVal value As String)
				m_PriceColumn = value
			End Set
		End Property

		Public Property WeightColumn() As String
			Get
				Return m_WeightColumn
			End Get
			Set(ByVal value As String)
				m_WeightColumn = value
			End Set
		End Property

		Public Property SwatchColumn() As String
			Get
				Return m_SwatchColumn
			End Get
			Set(ByVal value As String)
				m_SwatchColumn = value
			End Set
		End Property

		Public Property SwatchAltColumn() As String
			Get
				Return m_SwatchAltColumn
			End Get
			Set(ByVal value As String)
				m_SwatchAltColumn = value
			End Set
		End Property

		Public Property IsInventoryManagement() As Boolean
			Get
				Return m_IsInventoryManagement
			End Get
			Set(ByVal value As Boolean)
				m_IsInventoryManagement = value
			End Set
		End Property

        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = Value
            End Set
        End Property


        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TemplateAttributeId As Integer)
            m_DB = DB
            m_TemplateAttributeId = TemplateAttributeId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreItemTemplateAttribute WHERE TemplateAttributeId = " & DB.Number(TemplateAttributeId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_TemplateAttributeId = Convert.ToInt32(r.Item("TemplateAttributeId"))
            m_TemplateId = Convert.ToInt32(r.Item("TemplateId"))
            m_AttributeName = Convert.ToString(r.Item("AttributeName"))
			m_FunctionType = Convert.ToString(r.Item("FunctionType"))
            m_AttributeType = Convert.ToString(r.Item("AttributeType"))
			If IsDBNull(r.Item("ParentId")) Then
				m_ParentId = Nothing
			Else
				m_ParentId = Convert.ToInt32(r.Item("ParentId"))
			End If
			If IsDBNull(r.Item("SpecifyValue")) Then
				m_SpecifyValue = Nothing
			Else
				m_SpecifyValue = Convert.ToString(r.Item("SpecifyValue"))
			End If
			If IsDBNull(r.Item("LookupTable")) Then
				m_LookupTable = Nothing
			Else
				m_LookupTable = Convert.ToString(r.Item("LookupTable"))
			End If
			If IsDBNull(r.Item("LookupColumn")) Then
				m_LookupColumn = Nothing
			Else
				m_LookupColumn = Convert.ToString(r.Item("LookupColumn"))
			End If
			If IsDBNull(r.Item("SKUColumn")) Then
				m_SKUColumn = Nothing
			Else
				m_SKUColumn = Convert.ToString(r.Item("SKUColumn"))
			End If
			m_IncludeSKU = Convert.ToBoolean(r.Item("IncludeSKU"))
			If IsDBNull(r.Item("PriceColumn")) Then
				m_PriceColumn = Nothing
			Else
				m_PriceColumn = Convert.ToString(r.Item("PriceColumn"))
			End If
			If IsDBNull(r.Item("WeightColumn")) Then
				m_WeightColumn = Nothing
			Else
				m_WeightColumn = Convert.ToString(r.Item("WeightColumn"))
			End If
			If IsDBNull(r.Item("SwatchColumn")) Then
				m_SwatchColumn = Nothing
			Else
				m_SwatchColumn = Convert.ToString(r.Item("SwatchColumn"))
			End If
			If IsDBNull(r.Item("SwatchAltColumn")) Then
				m_SwatchAltColumn = Nothing
			Else
				m_SwatchAltColumn = Convert.ToString(r.Item("SwatchAltColumn"))
			End If
			m_IsInventoryManagement = Convert.ToBoolean(r.Item("IsInventoryManagement"))
			m_SortOrder = Convert.ToInt32(r.Item("SortOrder"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

			Dim MaxSortOrder As Integer
			If Not ParentId = Nothing Then
				MaxSortOrder = DB.ExecuteScalar("select top 1 SortOrder from StoreItemTemplateAttribute WHERE TemplateAttributeId=" & ParentId)
			Else
				MaxSortOrder = DB.ExecuteScalar("select top 1 SortOrder from StoreItemTemplateAttribute WHERE TemplateId=" & TemplateId & " AND ParentId IS NULL order by SortOrder desc")
            MaxSortOrder += 1
			End If

            SQL = " INSERT INTO StoreItemTemplateAttribute (" _
             & " TemplateId" _
			 & ",ParentId" _
             & ",AttributeName" _
			 & ",FunctionType" _
             & ",AttributeType" _
			 & ",SpecifyValue" _
			 & ",LookupTable" _
			 & ",LookupColumn" _
			 & ",SKUColumn" _
			 & ",IncludeSKU" _
			 & ",PriceColumn" _
			 & ",WeightColumn" _
			 & ",SwatchColumn" _
			 & ",SwatchAltColumn" _
			 & ",IsInventoryManagement" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.NullNumber(TemplateId) _
			 & "," & m_DB.NullNumber(ParentId) _
             & "," & m_DB.Quote(AttributeName) _
			 & "," & m_DB.Quote(FunctionType) _
             & "," & m_DB.Quote(AttributeType) _
			 & "," & m_DB.Quote(SpecifyValue) _
			 & "," & m_DB.Quote(LookupTable) _
			 & "," & m_DB.Quote(LookupColumn) _
			 & "," & m_DB.Quote(SKUColumn) _
			 & "," & CInt(IncludeSKU) _
			 & "," & m_DB.Quote(PriceColumn) _
			 & "," & m_DB.Quote(WeightColumn) _
			 & "," & m_DB.Quote(SwatchColumn) _
			 & "," & m_DB.Quote(SwatchAltColumn) _
			 & "," & CInt(IsInventoryManagement) _
             & "," & MaxSortOrder _
             & ")"

            TemplateAttributeId = m_DB.InsertSQL(SQL)

            Return TemplateAttributeId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreItemTemplateAttribute SET " _
             & " TemplateId = " & m_DB.NullNumber(TemplateId) _
			 & ",ParentId = " & m_DB.NullNumber(ParentId) _
             & ",AttributeName = " & m_DB.Quote(AttributeName) _
			 & ",FunctionType = " & m_DB.Quote(FunctionType) _
             & ",AttributeType = " & m_DB.Quote(AttributeType) _
			 & ",SpecifyValue = " & m_DB.Quote(SpecifyValue) _
			 & ",LookupTable = " & m_DB.Quote(LookupTable) _
			 & ",LookupColumn = " & m_DB.Quote(LookupColumn) _
			 & ",SKUColumn = " & m_DB.Quote(SKUColumn) _
			 & ",IncludeSKU = " & CInt(IncludeSKU) _
			 & ",PriceColumn = " & m_DB.Quote(PriceColumn) _
			 & ",WeightColumn = " & m_DB.Quote(WeightColumn) _
			 & ",SwatchColumn = " & m_DB.Quote(SwatchColumn) _
			 & ",SwatchAltColumn = " & m_DB.Quote(SwatchAltColumn) _
			 & ",IsInventoryManagement = " & CInt(IsInventoryManagement) _
             & " WHERE TemplateAttributeId = " & m_DB.Quote(TemplateAttributeId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreItemTemplateAttribute WHERE TemplateAttributeId = " & m_DB.Quote(TemplateAttributeId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreItemTemplateAttributeCollection
        Inherits GenericCollection(Of StoreItemTemplateAttributeRow)
    End Class

End Namespace

