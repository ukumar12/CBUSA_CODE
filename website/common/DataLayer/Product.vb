Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ProductRow
        Inherits ProductRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ProductID As Integer)
            MyBase.New(DB, ProductID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ProductID As Integer) As ProductRow
            Dim row As ProductRow

            row = New ProductRow(DB, ProductID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ProductID As Integer)
            Dim row As ProductRow

            row = New ProductRow(DB, ProductID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from Product"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

        Public Shared Function GetNextSKU(ByVal DB As Database)
            Return DB.InsertSQL("Insert Into ProductSKUSequense (CreateDate) values (" & DB.NullQuote(Now) & ")")
        End Function

        Public ReadOnly Property GetSelectedLLCProductPriceRequirements() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select LLCID from LLCProductPriceRequirement where ProductID = " & ProductID)
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

        Public Sub DeleteFromAllLLCProductPriceRequirements()
            DB.ExecuteSQL("delete from LLCProductPriceRequirement where ProductID = " & ProductID)
        End Sub

        Public Sub InsertToLLCProductPriceRequirements(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToLLCProductPriceRequirement(Element)
            Next
        End Sub

        Public Sub InsertToLLCProductPriceRequirement(ByVal LLCID As Integer)
            Dim SQL As String = "insert into LLCProductPriceRequirement (ProductID, LLCID) values (" & ProductID & "," & LLCID & ")"
            DB.ExecuteSQL(SQL)
        End Sub

        Public ReadOnly Property GetSelectedSupplyPhases() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select SupplyPhaseId from SupplyPhaseProduct where ProductId=" & DB.Number(ProductID))
                Dim conn As String = String.Empty
                Dim res As String = String.Empty

                While dr.Read
                    res &= conn & dr("SupplyPhaseId")
                    conn = ","
                End While
                dr.Close()
                Return res
            End Get
        End Property

        Public Sub DeleteFromAllSupplyPhases()
            DB.ExecuteSQL("delete from SupplyPhaseProduct where ProductId=" & DB.Number(ProductID))
        End Sub

        Public Sub InsertToSupplyPhases(ByVal elements As String)
            If elements = String.Empty Then Exit Sub
            Dim aElements As String() = elements.Split(",")
            For Each element As String In aElements
                InsertToSupplyPhase(element)
            Next
        End Sub

        Public Sub InsertToSupplyPhase(ByVal SupplyPhaseId As Integer)
            Dim sql As String = "insert into SupplyPhaseProduct (ProductId, SupplyPhaseId) values (" & ProductID & "," & SupplyPhaseId & ")"
            DB.ExecuteSQL(sql)
        End Sub
        Public Shared Function GetAllAttributeValues(ByVal DB As Database, ByVal ProductId As Integer) As DataTable
            Dim sql As String = "select * from ProductTypeAttribute pta inner join ProductTypeAttributeProductValue ptav on pta.ProductTypeAttributeId=ptav.ProductTypeAttributeId" _
                & " where ptav.ProductId=" & DB.Number(ProductId)
            Return DB.GetDataTable(sql)
        End Function

        Public Sub DeleteAllAttributeValues()
            DB.ExecuteSQL("delete from ProductTypeAttributeProductValue where ProductId=" & DB.Number(ProductID))
        End Sub

        Public Shared Function GetSupplyPhaseProducts(ByVal DB As Database, ByVal SupplyPhaseIds As String, Optional ByVal VendorId As Integer = Nothing, Optional ByVal PageNo As Integer = -1, Optional ByVal PageSize As Integer = 10) As DataTable
            Dim sql As String
            Dim orderby As String = " order by SupplyPhase ASC, SKU ASC"
            If VendorId = Nothing Then
                sql = "select ROW_NUMBER() OVER(" & orderby & ") as RowNumber, p.* from Product p inner join SupplyPhaseProduct sp on p.ProductId=sp.ProductId inner join SupplyPhase s on s.SupplyPhaseId=sp.SupplyPhaseId where sp.SupplyPhaseId in " & DB.NumberMultiple(SupplyPhaseIds)
            Else
                sql = "select ROW_NUMBER() OVER(" & orderby & ") as RowNumber, p.*, v.VendorSku, v.VendorPrice from Product p inner join SupplyPhaseProduct sp on p.ProductId=sp.ProductId inner join SupplyPhase s on s.SupplyPhaseId=sp.SupplyPhaseId inner join VendorProductPrice v on p.ProductId=v.ProductId where sp.SupplyPhaseId in " & DB.NumberMultiple(SupplyPhaseIds) & " and v.VendorId=" & DB.Number(VendorId)
            End If
            If PageNo > 0 Then
                sql = "select top " & PageSize & " * from (" & sql & ") as temp where temp.RowNumber >= " & DB.Number((PageNo - 1) * PageSize) & " order by " & orderby
            Else
                sql &= orderby
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetSupplyPhaseProducts(ByVal DB As Database, ByVal SupplyPhaseId As Integer, Optional ByVal VendorId As Integer = Nothing) As DataTable
            Dim sql As String
            If VendorId = Nothing Then
                sql = "select distinct p.* from Product p inner join SupplyPhaseProduct sp on p.ProductId=sp.ProductId where sp.SupplyPhaseId=" & DB.Number(SupplyPhaseId)
            Else
                sql = "select distinct p.*, v.VendorSku, v.VendorPrice from Product p inner join SupplyPhaseProduct sp on p.ProductId=sp.ProductID inner join VendorProductPrice v on p.ProductId=v.ProductId where sp.SupplyPhaseId=" & DB.Number(SupplyPhaseId) & " and v.VendorId=" & DB.Number(VendorId)
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetPriceLockDate(ByVal DB As Database, ByVal ProductID As Integer) As DateTime
            Dim sql As String = "select min(PriceLockDays) from SupplyPhase where SupplyPhaseID in (select SupplyPhaseID from SupplyPhaseProduct where ProductID=" & DB.Number(ProductID) & ")"
            Dim interval As Integer = IIf(IsDBNull(DB.ExecuteScalar(sql)), 0, DB.ExecuteScalar(sql))
            'minimum interval is 30 days, but data does not reflect that
            Dim months As Integer = IIf(interval <= 30, 1, interval / 30)
            Dim lastIntervalStart As Integer = Math.Floor((Now.Month - 1) / months) * months + 1
            Return DateAdd("m", months, lastIntervalStart & "/1/" & IIf(lastIntervalStart > Now.Month, Now.Year - 1, Now.Year))
        End Function

        Public Shared Function GetName(ByVal DB As Database, ByVal ProductID As Integer) As String
            Return DB.ExecuteScalar("select Product from Product where ProductID=" & DB.Number(ProductID))
        End Function

        Public Shared Function GetChildSupplyPhaseProducts(ByVal DB As Database, ByVal ParentSupplyPhaseId As Integer) As DataTable
            Dim sql As String
            sql = "select distinct p.* from Product p inner join SupplyPhaseProduct sp on p.ProductId=sp.ProductId where sp.SupplyPhaseId IN (select SupplyPhaseId from SupplyPhase Where ParentSupplyPhaseId = " & DB.Number(ParentSupplyPhaseId) & ")"

            Return DB.GetDataTable(sql)
        End Function
    End Class

    Public MustInherit Class ProductRowBase
        Private m_DB As Database
        Private m_ProductID As Integer = Nothing
        Private m_Product As String = Nothing
        Private m_SKU As String = Nothing
        Private m_Description As String = Nothing
        Private m_ManufacturerID As Integer = Nothing
        Private m_Size As String = Nothing
        Private m_SizeUnitOfMeasureID As Integer = Nothing
        Private m_Width As Integer = Nothing
        Private m_Length As Integer = Nothing
        Private m_Height As Integer = Nothing
        Private m_WidthUnitOfMeasureID As Integer = Nothing
        Private m_LengthUnitOfMeasureID As Integer = Nothing
        Private m_HeightUnitOfMeasureID As Integer = Nothing
        Private m_Grade As String = Nothing
        Private m_ProductTypeID As Integer = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_Created As DateTime = Nothing
        Private m_CreatorAdminID As Integer = Nothing
        Private m_Updated As DateTime = Nothing
        Private m_UpdaterAdminID As Integer = Nothing


        Public Property ProductID() As Integer
            Get
                Return m_ProductID
            End Get
            Set(ByVal Value As Integer)
                m_ProductID = value
            End Set
        End Property

        Public Property Product() As String
            Get
                Return m_Product
            End Get
            Set(ByVal Value As String)
                m_Product = value
            End Set
        End Property

        Public Property SKU() As String
            Get
                Return m_SKU
            End Get
            Set(ByVal Value As String)
                m_SKU = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal Value As String)
                m_Description = value
            End Set
        End Property

        Public Property ManufacturerID() As Integer
            Get
                Return m_ManufacturerID
            End Get
            Set(ByVal Value As Integer)
                m_ManufacturerID = value
            End Set
        End Property

        Public Property Size() As String
            Get
                Return m_Size
            End Get
            Set(ByVal Value As String)
                m_Size = value
            End Set
        End Property

        Public Property SizeUnitOfMeasureID() As Integer
            Get
                Return m_SizeUnitOfMeasureID
            End Get
            Set(ByVal Value As Integer)
                m_SizeUnitOfMeasureID = value
            End Set
        End Property

        Public Property Width() As Integer
            Get
                Return m_Width
            End Get
            Set(ByVal Value As Integer)
                m_Width = value
            End Set
        End Property

        Public Property Length() As Integer
            Get
                Return m_Length
            End Get
            Set(ByVal Value As Integer)
                m_Length = value
            End Set
        End Property

        Public Property Height() As Integer
            Get
                Return m_Height
            End Get
            Set(ByVal Value As Integer)
                m_Height = value
            End Set
        End Property

        Public Property WidthUnitOfMeasureID() As Integer
            Get
                Return m_WidthUnitOfMeasureID
            End Get
            Set(ByVal Value As Integer)
                m_WidthUnitOfMeasureID = value
            End Set
        End Property

        Public Property LengthUnitOfMeasureID() As Integer
            Get
                Return m_LengthUnitOfMeasureID
            End Get
            Set(ByVal Value As Integer)
                m_LengthUnitOfMeasureID = value
            End Set
        End Property

        Public Property HeightUnitOfMeasureID() As Integer
            Get
                Return m_HeightUnitOfMeasureID
            End Get
            Set(ByVal Value As Integer)
                m_HeightUnitOfMeasureID = value
            End Set
        End Property

        Public Property Grade() As String
            Get
                Return m_Grade
            End Get
            Set(ByVal Value As String)
                m_Grade = value
            End Set
        End Property

        Public Property ProductTypeID() As Integer
            Get
                Return m_ProductTypeID
            End Get
            Set(ByVal Value As Integer)
                m_ProductTypeID = value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = value
            End Set
        End Property

        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property

        Public Property CreatorAdminID() As Integer
            Get
                Return m_CreatorAdminID
            End Get
            Set(ByVal Value As Integer)
                m_CreatorAdminID = value
            End Set
        End Property

        Public ReadOnly Property Updated() As DateTime
            Get
                Return m_Updated
            End Get
        End Property

        Public Property UpdaterAdminID() As Integer
            Get
                Return m_UpdaterAdminID
            End Get
            Set(ByVal Value As Integer)
                m_UpdaterAdminID = value
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

        Public Sub New(ByVal DB As Database, ByVal ProductID As Integer)
            m_DB = DB
            m_ProductID = ProductID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Product WHERE ProductID = " & DB.Number(ProductID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ProductID = Convert.ToInt32(r.Item("ProductID"))
            m_Product = Convert.ToString(r.Item("Product"))
            m_SKU = Convert.ToString(r.Item("SKU"))
            If IsDBNull(r.Item("Description")) Then
                m_Description = Nothing
            Else
                m_Description = Convert.ToString(r.Item("Description"))
            End If
            If IsDBNull(r.Item("ManufacturerID")) Then
                m_ManufacturerID = Nothing
            Else
                m_ManufacturerID = Convert.ToInt32(r.Item("ManufacturerID"))
            End If
            If IsDBNull(r.Item("Size")) Then
                m_Size = Nothing
            Else
                m_Size = Convert.ToString(r.Item("Size"))
            End If
            If IsDBNull(r.Item("SizeUnitOfMeasureID")) Then
                m_SizeUnitOfMeasureID = Nothing
            Else
                m_SizeUnitOfMeasureID = Convert.ToInt32(r.Item("SizeUnitOfMeasureID"))
            End If
            If IsDBNull(r.Item("Width")) Then
                m_Width = Nothing
            Else
                If IsNumeric(r.Item("Width")) Then
                    m_Width = Convert.ToInt32(r.Item("Width"))
                Else
                    m_Width = Nothing
                End If
            End If
            If IsDBNull(r.Item("Length")) Then
                m_Length = Nothing
            Else
                If IsNumeric(r.Item("Length")) Then
                    m_Length = Convert.ToInt32(r.Item("Length"))
                Else
                    m_Length = Nothing
                End If
            End If
            If IsDBNull(r.Item("Height")) Then
                m_Height = Nothing
            Else
                If IsNumeric(r.Item("Height")) Then
                    m_Height = Convert.ToInt32(r.Item("Height"))
                Else
                    m_Height = Nothing
                End If
            End If
            If IsDBNull(r.Item("WidthUnitOfMeasureID")) Then
                m_WidthUnitOfMeasureID = Nothing
            Else
                m_WidthUnitOfMeasureID = Convert.ToInt32(r.Item("WidthUnitOfMeasureID"))
            End If
            If IsDBNull(r.Item("LengthUnitOfMeasureID")) Then
                m_LengthUnitOfMeasureID = Nothing
            Else
                m_LengthUnitOfMeasureID = Convert.ToInt32(r.Item("LengthUnitOfMeasureID"))
            End If
            If IsDBNull(r.Item("HeightUnitOfMeasureID")) Then
                m_HeightUnitOfMeasureID = Nothing
            Else
                m_HeightUnitOfMeasureID = Convert.ToInt32(r.Item("HeightUnitOfMeasureID"))
            End If
            If IsDBNull(r.Item("Grade")) Then
                m_Grade = Nothing
            Else
                m_Grade = Convert.ToString(r.Item("Grade"))
            End If
            m_ProductTypeID = Convert.ToInt32(r.Item("ProductTypeID"))
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            m_Created = Convert.ToDateTime(r.Item("Created"))
            m_CreatorAdminID = Convert.ToInt32(r.Item("CreatorAdminID"))
            If IsDBNull(r.Item("Updated")) Then
                m_Updated = Nothing
            Else
                m_Updated = Convert.ToDateTime(r.Item("Updated"))
            End If
            If IsDBNull(r.Item("UpdaterAdminID")) Then
                m_UpdaterAdminID = Nothing
            Else
                m_UpdaterAdminID = Convert.ToInt32(r.Item("UpdaterAdminID"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO Product (" _
             & " Product" _
             & ",SKU" _
             & ",Description" _
             & ",ManufacturerID" _
             & ",Size" _
             & ",SizeUnitOfMeasureID" _
             & ",Width" _
             & ",Length" _
             & ",Height" _
             & ",WidthUnitOfMeasureID" _
             & ",LengthUnitOfMeasureID" _
             & ",HeightUnitOfMeasureID" _
             & ",Grade" _
             & ",ProductTypeID" _
             & ",IsActive" _
             & ",Created" _
             & ",CreatorAdminID" _
             & ",Updated" _
             & ",UpdaterAdminID" _
             & ") VALUES (" _
             & m_DB.Quote(Product) _
             & "," & m_DB.Quote(SKU) _
             & "," & m_DB.Quote(Description) _
             & "," & m_DB.NullNumber(ManufacturerID) _
             & "," & m_DB.Quote(Size) _
             & "," & m_DB.NullNumber(SizeUnitOfMeasureID) _
             & "," & m_DB.Number(Width) _
             & "," & m_DB.Number(Length) _
             & "," & m_DB.Number(Height) _
             & "," & m_DB.NullNumber(WidthUnitOfMeasureID) _
             & "," & m_DB.NullNumber(LengthUnitOfMeasureID) _
             & "," & m_DB.NullNumber(HeightUnitOfMeasureID) _
             & "," & m_DB.Quote(Grade) _
             & "," & m_DB.NullNumber(ProductTypeID) _
             & "," & CInt(IsActive) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorAdminID) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(UpdaterAdminID) _
             & ")"

            ProductID = m_DB.InsertSQL(SQL)

            Return ProductID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Product SET " _
             & " Product = " & m_DB.Quote(Product) _
             & ",SKU = " & m_DB.Quote(SKU) _
             & ",Description = " & m_DB.Quote(Description) _
             & ",ManufacturerID = " & m_DB.NullNumber(ManufacturerID) _
             & ",Size = " & m_DB.Quote(Size) _
             & ",SizeUnitOfMeasureID = " & m_DB.NullNumber(SizeUnitOfMeasureID) _
             & ",Width = " & m_DB.Number(Width) _
             & ",Length = " & m_DB.Number(Length) _
             & ",Height = " & m_DB.Number(Height) _
             & ",WidthUnitOfMeasureID = " & m_DB.NullNumber(WidthUnitOfMeasureID) _
             & ",LengthUnitOfMeasureID = " & m_DB.NullNumber(LengthUnitOfMeasureID) _
             & ",HeightUnitOfMeasureID = " & m_DB.NullNumber(HeightUnitOfMeasureID) _
             & ",Grade = " & m_DB.Quote(Grade) _
             & ",ProductTypeID = " & m_DB.NullNumber(ProductTypeID) _
             & ",IsActive = " & CInt(IsActive) _
             & ",Updated = " & m_DB.NullQuote(Now) _
             & ",UpdaterAdminID = " & m_DB.NullNumber(UpdaterAdminID) _
             & " WHERE ProductID = " & m_DB.Quote(ProductID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM Product WHERE ProductID = " & m_DB.Number(ProductID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ProductCollection
        Inherits GenericCollection(Of ProductRow)
    End Class

End Namespace


