Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    ''' <summary>
    ''' Represents a row in the TwoPriceCampaign table in the database.
    ''' </summary>
    ''' <remarks>Custom methods should be placed in this class, not in <see cref="TwoPriceCampaignRowBase" />.</remarks>
    ''' 
    Public Class TwoPriceCampaignRow
        Inherits TwoPriceCampaignRowBase

        ''' <overloads>Initializes a new instance of the <see cref="TwoPriceCampaignRow" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="TwoPriceCampaignRow" /> class using default settings.
        ''' </summary>
        ''' <remarks>This constructor calls 
        ''' <see cref="M:DataLayer.TwoPriceCampaignRowBase.#ctor">TwoPriceCampaignRowBase.New</see>.</remarks>
        Public Sub New()
            MyBase.New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TwoPriceCampaignRow" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.TwoPriceCampaignRow.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.TwoPriceCampaignRowBase.#ctor(Database)">TwoPriceCampaignRowBase.New(Database)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TwoPriceCampaignRow" /> class representing the row which uses
        ''' <paramref name="TwoPriceCampaignId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="TwoPriceCampaignId">The primary key value of the row being referenced.</param>
        ''' <remarks><para>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.TwoPriceCampaignRow.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</para>
        ''' <para>This constructor calls 
        ''' <see cref="M:DataLayer.TwoPriceCampaignRowBase.#ctor(Database,System.Int32)">TwoPriceCampaignRowBase.New(Database, Integer)</see>.</para></remarks>
        Public Sub New(ByVal DB As Database, TwoPriceCampaignId As Integer)
            MyBase.New(DB, TwoPriceCampaignId)
        End Sub 'New

        ''' <summary>
        ''' Gets the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="TwoPriceCampaignId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="TwoPriceCampaignId">The primary key value of the row being retrieved.</param>
        ''' <returns>An instance of <see cref="TwoPriceCampaignRow" /> loaded with the values from the specified 
        ''' row in the database.</returns>
        ''' <remarks>This method uses <see cref="M:DataLayer.TwoPriceCampaignRowBase.Load">Load</see>.</remarks>
        Public Shared Function GetRow(ByVal DB As Database, ByVal TwoPriceCampaignId As Integer) As TwoPriceCampaignRow
            Dim row As TwoPriceCampaignRow

            row = New TwoPriceCampaignRow(DB, TwoPriceCampaignId)
            row.Load()

            Return row
        End Function

        ''' <summary>
        ''' Removes the row from the specified <see cref="Database" /> with the specified 
        ''' <paramref name="TwoPriceCampaignId" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="TwoPriceCampaignId">The primary key value of the row being removed.</param>
        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TwoPriceCampaignId As Integer)
            Dim SQL As String

            SQL = "DELETE FROM TwoPriceCampaign WHERE TwoPriceCampaignId = " & DB.Number(TwoPriceCampaignId)
            DB.ExecuteSQL(SQL)
        End Sub

        ''' <summary>
        ''' Removes this row from the specified <see cref="Database" />.
        ''' </summary>
        ''' <remarks>This method calls <c>RemoveRow(DB, TwoPriceCampaignId)</c></remarks>
        Public Sub Remove()
            RemoveRow(DB, TwoPriceCampaignId)
        End Sub

        ''' <summary>
        ''' Retrieves the TwoPriceCampaign table from the specified <see cref="Database" /> ordered based on 
        ''' <paramref name="SortBy" /> and <paramref name="SortOrder" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="SortBy">The SQL field name to sort the results by.</param>
        ''' <param name="SortOrder">The order by which to sort the results (ASC, DESC).  The default value of this
        ''' parameter is "ASC".</param>
        ''' <returns>A <see cref="DataTable" /> containing the data returned by the query.</returns>
        ''' <remarks>If <paramref name="SortBy" /> is not provided, the data is not sorted during the query.</remarks>
        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC", Optional ByVal Status As String = "") As DataTable
            Dim SQL As String = "select * from TwoPriceCampaign"

            If Not Status = String.Empty Then
                Status = Core.ProtectParam(Status)

                SQL &= " WHERE STATUS = " & Status
            End If

            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If

            
            Return DB.GetDataTable(SQL)
        End Function


        Public ReadOnly Property GetSelectedLLCs() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select LLCID from TwoPriceCampaignLLC_Rel where TwoPriceCampaignId = " & TwoPriceCampaignId)
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
            DB.ExecuteSQL("delete from TwoPriceCampaignLLC_Rel where TwoPriceCampaignId = " & TwoPriceCampaignId)
        End Sub


        Public Sub InsertToLLCs(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToLLC(Element)
            Next
        End Sub

        ''' <summary>
        ''' Inserts a single entry into the TwoPricingCampaignLLC_Rel table using the current value of <see cref="TwoPriceCampaignId" /> 
        ''' and the specified <paramref name="LLCID" />.
        ''' </summary>
        ''' <param name="LLCID">A value for LLCID in the TwoPricingCampaignLLC_Rel table.</param>
        ''' <remarks>This method will likely fail if the intended pairing is already in the table.</remarks>
        Public Sub InsertToLLC(ByVal LLCID As Integer)
            Dim SQL As String = "insert into TwoPriceCampaignLLC_Rel (TwoPriceCampaignId, LLCID) values (" & TwoPriceCampaignId & "," & LLCID & ")"
            DB.ExecuteSQL(SQL)
        End Sub


        Public Sub DeleteFromAllCampaignBuilders()
            DB.ExecuteSQL("delete from TwoPriceCampaignBuilder_Rel where TwoPriceCampaignId = " & TwoPriceCampaignId)
        End Sub

        Public ReadOnly Property GetSelectedCampaignBuilders() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select BuilderId from TwoPriceCampaignBuilder_Rel where TwoPriceCampaignId = " & TwoPriceCampaignId)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("BuilderId")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property


        Public Sub InsertToCampaignBuilders(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToCampaignBuilder(Element)
            Next
        End Sub

        ''' <summary>
        ''' Inserts a single entry into the TwoPricingCampaignBuilder_Rel table using the current value of <see cref="TwoPriceCampaignId" /> 
        ''' and the specified <paramref name="LLCID" />.
        ''' </summary>
        ''' <param name="BuilderId">A value for BuilderId in the TwoPricingCampaignLLC_Rel table.</param>
        ''' <remarks>This method will likely fail if the intended pairing is already in the table.</remarks>
        Public Sub InsertToCampaignBuilder(ByVal BuilderId As Integer)
            Dim SQL As String = "insert into TwoPriceCampaignBuilder_Rel (TwoPriceCampaignId, BuilderId) values (" & TwoPriceCampaignId & "," & BuilderId & ")"
            DB.ExecuteSQL(SQL)
        End Sub

        '' <summary>
        ''' Deletes all of the entries in the TwoPricingCampaignVendor_Rel table for the current value of <see cref="TwoPriceCampaignId" />.
        ''' </summary>
        ''' <remarks>By calling this method followed by <see cref="InsertToCampaignBuilder" />, you can replace the
        ''' selected values with those specified by the <see cref="Controls.CheckBoxListEx.SelectedValues" />
        ''' property of a <see cref="Controls.CheckBoxListEx" />.</remarks>
        ''' 


        Public Sub DeleteFromAllCampaignVendors()
            DB.ExecuteSQL("delete from TwoPriceCampaignVendor_Rel where TwoPriceCampaignId = " & TwoPriceCampaignId)
        End Sub

        Public Sub AddUpdateVendorBidDeadline(ByVal BidDeadlineDate As DateTime)
            DB.ExecuteSQL("update TwoPriceCampaign set VendorBidDeadline='" + BidDeadlineDate + "' where TwoPriceCampaignId = " & TwoPriceCampaignId)
        End Sub

        Public ReadOnly Property GetSelectedCampaignVendors() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select VendorId from TwoPriceCampaignVendor_Rel where TwoPriceCampaignId = " & TwoPriceCampaignId)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("VendorId")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property
        Public ReadOnly Property GetCampaignVendorswithNotDeclined() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select VendorId from TwoPriceCampaignVendor_Rel where HasDeclinedToBid = 0 And TwoPriceCampaignId = " & TwoPriceCampaignId)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("VendorId")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public Sub InsertToCampaignVendors(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToCampaignVendor(Element)
            Next
        End Sub

        ''' <summary>
        ''' Inserts a single entry into the TwoPricingCampaignVendor_Rel table using the current value of <see cref="TwoPriceCampaignId" /> 
        ''' and the specified <paramref name="VendorId" />.
        ''' </summary>
        ''' <param name="VendorId">A value for Vendor in the TwoPricingCampaignVendor_Rel table.</param>
        ''' <remarks>This method will likely fail if the intended pairing is already in the table.</remarks>
        Public Sub InsertToCampaignVendor(ByVal VendorId As Integer)
            Dim SQL As String = "insert into TwoPriceCampaignVendor_Rel (TwoPriceCampaignId, VendorId) values (" & TwoPriceCampaignId & "," & VendorId & ")"
            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Function GetTwoPriceTakeOffList(ByVal DB As Database, Optional ByVal Status As String = "", Optional ByVal VendorId As Integer = 0, Optional ByVal BuilderId As Integer = 0, Optional ByVal RestrictByDate As Boolean = False, Optional ByVal GetActiveOnly As Boolean = False) As DataTable
            Dim SQL As String = "select * from TwoPriceCampaign tpc JOIN TwoPriceTakeOff tpto ON tpc.TwoPriceCampaignId = tpto.TwoPriceCampaignId "
            Dim SQLConditions As String = ""

            Dim conn As String = " WHERE "

            If Not Status = String.Empty Then
                Status = Core.ProtectParam(Status)
                SQLConditions &= conn & " tpc.Status = " & DB.NQuote(Status)
                conn = " AND "
            End If

            If Not VendorId = 0 Then
                SQL &= " JOIN TwoPriceCampaignVendor_Rel tpcv ON tpc.TwoPriceCampaignId = tpcv.TwoPriceCampaignId "
                VendorId = Core.ProtectParam(VendorId)
                SQLConditions &= conn & " tpcv.VendorId = " & DB.NullNumber(VendorId)
                conn = " AND "
            End If

            If Not BuilderId = 0 Then
                SQL &= " JOIN TwoPriceCampaignBuilder_Rel tpbv ON tpc.TwoPriceCampaignId = tpbv.TwoPriceCampaignId "
                BuilderId = Core.ProtectParam(BuilderId)
                SQLConditions &= conn & " tpbv.BuilderId = " & DB.NullNumber(BuilderId)
                conn = " AND "
            End If

            If RestrictByDate Then
                'SQLConditions &= conn & " tpc.StartDate <= " & DB.Quote(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd")) & " AND tpc.EndDate >= " & DB.Quote(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd"))
                SQLConditions &= conn & " ((tpc.StartDate <= GETDATE() AND tpc.EndDate >= GETDATE()) OR (tpc.[Status] = 'BiddingInProgress')) "
                conn = " AND "
            End If

            If GetActiveOnly Then
                SQLConditions &= conn & " tpc.IsActive = 1 "
                conn = " AND "
            End If


            Return DB.GetDataTable(SQL & SQLConditions)
        End Function
        Public Shared Function GetVendorProductState(ByVal DB As Database, ByVal TwoPriceCampaignID As Integer)
            Dim sql As String = _
                "SELECT p.*, NULL AS VendorProductPriceRequestId " _
                & "FROM TwoPriceVendorProductPrice p " _
                & "INNER JOIN Vendor v " _
                & "ON p.VendorId = v.VendorId" _
                & "  where " _
                & "   p.TwoPriceCampaignId=" & DB.Number(TwoPriceCampaignID)

            Return DB.GetDataTable(sql)
        End Function

    End Class

End Namespace
