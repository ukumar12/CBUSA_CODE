Option Strict Off

Imports Components
Imports DataLayer
Imports InfoSoftGlobal
Imports System.Configuration.ConfigurationManager

Partial Class MyMoney
    Inherits ModuleControl

    Protected StartDate As DateTime
    Protected EndDate As Datetime

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If IsAdminDisplay Or DesignMode Then
            Exit Sub
        End If

        Dim SQL As String = String.Empty
        Dim i As Integer = 1
        

        If Not IsPostBack Then
            
            For i = 1 to 4
                Me.drpStartQuarter.Items.Insert(i-1, (New ListItem(i,i)))
                Me.drpEndQuarter.Items.Insert(i-1, (New ListItem(i,i)))
            Next
        
            For i = 1 to 10
                Me.drpStartYear.Items.Insert(i-1, (New ListItem(Now.Year - 8 + i,Now.Year - 8 + i)))
                Me.drpEndYear.Items.Insert(i-1, (New ListItem(Now.Year - 8 + i,Now.Year - 8 + i)))
            Next

            Me.drpStartQuarter.SelectedValue = ((Now.AddMonths(-3).Month - 1) \ 3) + 1
            Me.drpEndQuarter.SelectedValue = ((Now.AddMonths(3).Month - 1) \ 3) + 1

            Me.drpStartYear.SelectedValue = Now.AddMonths(-3).Year
            Me.drpEndYear.SelectedValue = Now.AddMonths(3).Year

        End If

    End Sub

    Protected Sub btnViewMore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewMore.Click
        Response.Redirect("/builder/money/default.aspx")
    End Sub

    
    Protected Function GetChartFlashString() As String
        If IsAdminDisplay Then
            Return String.Empty
        End If
        Dim XML As String = String.Empty
        Dim SQL As String = String.Empty
        'Dim dt As DataTable

        If Me.drpEndYear.SelectedValue < Me.drpStartYear.SelectedValue Then
            Me.drpEndYear.SelectedValue = Me.drpStartYear.SelectedValue
            Me.drpEndQuarter.SelectedValue = Me.drpStartQuarter.SelectedValue
        ElseIf Me.drpEndYear.SelectedValue = Me.drpStartYear.SelectedValue And Me.drpEndQuarter.SelectedValue < Me.drpStartQuarter.SelectedValue Then
            Me.drpEndYear.SelectedValue = Me.drpStartYear.SelectedValue
            Me.drpEndQuarter.SelectedValue = Me.drpStartQuarter.SelectedValue
        End If

        'Dim StartDate As DateTime
        'If Me.drpStartQuarter.SelectedValue = "1" Then
        '    StartDate = New DateTime(drpStartYear.SelectedValue, 1, 1)
        'ElseIf Me.drpStartQuarter.SelectedValue = "2" Then
        '    StartDate = New DateTime(drpStartYear.SelectedValue, 4, 1)
        'ElseIf Me.drpStartQuarter.SelectedValue = "3" Then
        '    StartDate = New DateTime(drpStartYear.SelectedValue, 7, 1)
        'ElseIf Me.drpStartQuarter.SelectedValue = "4" Then
        '    StartDate = New DateTime(drpStartYear.SelectedValue, 10, 1)
        'Else
        '    StartDate = New DateTime(Year(Now()), 10, 1)
        'End If

        'Dim EndDate As DateTime
        'If Me.drpEndQuarter.SelectedValue = "1" Then
        '    EndDate = New DateTime(drpEndYear.SelectedValue, 4, 1)
        'ElseIf Me.drpEndQuarter.SelectedValue = "2" Then
        '    EndDate = New DateTime(drpEndYear.SelectedValue, 7, 1)
        'ElseIf Me.drpEndQuarter.SelectedValue = "3" Then
        '    EndDate = New DateTime(drpEndYear.SelectedValue, 10, 1)
        'ElseIf Me.drpEndQuarter.SelectedValue = "4" Then
        '    EndDate = New DateTime(drpEndYear.SelectedValue + 1, 1, 1)
        'Else
        '    EndDate = New DateTime(Year(Now()) + 1, 1, 1)
        'End If
        'EndDate = EndDate.AddDays(-1)

        'SQL = "Select SUM(o.Total) As TotalLLCPurchases"
        'SQL &= " From [Order] o "
        'SQL &= " Where o.VendorID IN (Select VendorID From LLCVendor "
        'SQL &= " Where LLCID = (Select LLCID From Builder Where BuilderId = " & DB.Number(Session("BuilderId")) & "))"
        'SQL &= " And o.OrderStatusID = (Select OrderStatusID From OrderStatus Where OrderStatus = 'Complete')"
        'SQL &= " And o.Created >= '" & StartDate & "'"
        'SQL &= " And o.Created <= '" & EndDate & "'"
        'SQL &= " And o.BuilderId = " & DB.Number(Session("BuilderId"))

        'Dim LLCPurchases = DB.ExecuteScalar(SQL)

        'If IsDBNull(LLCPurchases) Then
        '    LLCPurchases = 0
        'End If

        'SQL = "Select SUM(o.Total) As TotalNonLLCPurchases"
        'SQL &= " From [Order] o "
        'SQL &= " Where o.VendorID IN (Select VendorID From LLCVendor "
        'SQL &= " Where LLCID = (Select LLCID From Builder Where BuilderId = " & DB.Number(Session("BuilderId")) & "))"
        'SQL &= " And o.OrderStatusID = (Select OrderStatusID From OrderStatus Where OrderStatus = 'Complete')"
        'SQL &= " And o.Created >= '" & StartDate & "'"
        'SQL &= " And o.Created <= '" & EndDate & "'"
        'SQL &= " And o.BuilderId <> " & DB.Number(Session("BuilderId"))

        'Dim NonLLCPurchases = DB.ExecuteScalar(SQL)

        'If IsDBNull(NonLLCPurchases) Then
        '    NonLLCPurchases = 0
        'End If

        'debug
        'SQL = "SELECT TOP 1" & vbCrLf
        'SQL &= "  105058 TotalPurchases," & vbCrLf
        'SQL &= "  38000 NonLLCPurchases, " & vbCrLf
        'SQL &= "  67058 LLCPurchases" & vbCrLf

        'dt = Me.DB.GetDataTable(SQL)

        'If dt.Rows.Count = 1 Then
        Dim arDB As New Database()
        arDB.Open(DBConnectionString.GetConnectionString(AppSettings("AccountingConnectionString"), AppSettings("AccountingConnectionStringUsername"), AppSettings("AccountingConnectionStringPassword")))
        Try

            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
            SQL = "select sum(PURCHVOLCUR) as PurchaseVolume from cbusa_BuilderRebates where BLDRID=" & dbBuilder.HistoricID _
                & " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                & " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))"


            Dim BuilderPurchases As Double = Core.GetDouble(arDB.ExecuteScalar(SQL))

            SQL = "select sum(PURCHVOLCUR) from cbusa_BuilderRebates where LLCID=(select top 1 STRING1 from VBCloud_Param where CODE=1 and LONGINT1=" & DB.Number(dbBuilder.LLCID) & ") and BLDRID <> " & DB.Number(dbBuilder.HistoricID) _
                & " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                & " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))"

            Dim NonBuilderPurchases As Double = Core.GetDouble(arDB.ExecuteScalar(SQL))

            'build the xml to drive the pie chart
            If BuilderPurchases = 0 And NonBuilderPurchases = 0 Then
                XML = XML & "<graph caption='Purchase Comparison' decimalPrecision='0' showPercentageValues='0' showNames='0' numberPrefix='$' showValues='0' showPercentageInLabel='0' pieYScale='45' pieBorderAlpha='40' pieFillAlpha='70' pieSliceDepth='15' pieRadius='75'>"
                XML = XML & "</graph>"
            Else
                XML = XML & "<graph caption='Purchase Comparison' decimalPrecision='1' showPercentageValues='0' showNames='0' numberPrefix='$' showValues='0' showPercentageInLabel='0' pieYScale='45' pieBorderAlpha='40' pieFillAlpha='70' pieSliceDepth='15' pieRadius='75'>"
                XML = XML & "<set name='Other LLC Purchases' value='" & NonBuilderPurchases & "' color='AFD8F8' />"
                XML = XML & "<set name='My Purchases' value='" & BuilderPurchases & "' color='F6BD0F' />"
                XML = XML & "</graph>"
            End If

            'create the chart
            Return Server.UrlEncode(XML)

        Finally
            If arDB IsNot Nothing AndAlso arDB.IsOpen Then arDB.Close()
        End Try
    End Function

    
End Class
