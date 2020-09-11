Option Strict Off

Imports Components
Imports DataLayer
Imports InfoSoftGlobal

Partial Class MyMoneyVendor
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

            For i = 1 To 4
                Me.drpStartQuarter.Items.Insert(i - 1, (New ListItem(i, i)))
                Me.drpEndQuarter.Items.Insert(i - 1, (New ListItem(i, i)))
            Next

            For i = 1 To 10
                Me.drpStartYear.Items.Insert(i - 1, (New ListItem(Now.Year - 8 + i, Now.Year - 8 + i)))
                Me.drpEndYear.Items.Insert(i - 1, (New ListItem(Now.Year - 8 + i, Now.Year - 8 + i)))
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

        Dim StartDate As DateTime
        If Me.drpStartQuarter.SelectedValue = "1" Then
            StartDate = New DateTime(drpStartYear.SelectedValue, 1, 1)
        ElseIf Me.drpStartQuarter.SelectedValue = "2" Then
            StartDate = New DateTime(drpStartYear.SelectedValue, 4, 1)
        ElseIf Me.drpStartQuarter.SelectedValue = "3" Then
            StartDate = New DateTime(drpStartYear.SelectedValue, 7, 1)
        ElseIf Me.drpStartQuarter.SelectedValue = "4" Then
            StartDate = New DateTime(drpStartYear.SelectedValue, 10, 1)
        Else
            StartDate = New DateTime(Year(Now()), 10, 1)
        End If

        Dim EndDate As DateTime
        If Me.drpEndQuarter.SelectedValue = "1" Then
            EndDate = New DateTime(drpEndYear.SelectedValue, 4, 1)
        ElseIf Me.drpEndQuarter.SelectedValue = "2" Then
            EndDate = New DateTime(drpEndYear.SelectedValue, 7, 1)
        ElseIf Me.drpEndQuarter.SelectedValue = "3" Then
            EndDate = New DateTime(drpEndYear.SelectedValue, 10, 1)
        ElseIf Me.drpEndQuarter.SelectedValue = "4" Then
            EndDate = New DateTime(drpEndYear.SelectedValue + 1, 1, 1)
        Else
            EndDate = New DateTime(Year(Now()) + 1, 1, 1)
        End If
        EndDate = EndDate.AddDays(-1)

        SQL = "Select SUM(o.Total) As TotalLLCSales"
        SQL &= " From [Order] o "
        SQL &= " Where o.BuilderId IN (Select BuilderId From Builder Where LLCID In (Select LLCID From LLCVendor Where VendorId = " & DB.Number(Session("VendorId")) & "))"
        SQL &= " And o.OrderStatusID = (Select OrderStatusID From OrderStatus Where OrderStatus = 'Complete')"
        SQL &= " And o.Created >= '" & StartDate & "'"
        SQL &= " And o.Created <= '" & EndDate & "'"
        SQL &= " And o.VendorId = " & DB.Number(Session("VendorId"))

        Dim LLCPurchases = DB.ExecuteScalar(SQL)

        If IsDBNull(LLCPurchases) Then
            LLCPurchases = 0
        End If

        SQL = "Select SUM(o.Total) As TotalNonLLCSales"
        SQL &= " From [Order] o "
        SQL &= " Where o.BuilderId IN (Select BuilderId From Builder Where LLCID In (Select LLCID From LLCVendor Where VendorId = " & DB.Number(Session("VendorId")) & "))"
        SQL &= " And o.OrderStatusID = (Select OrderStatusID From OrderStatus Where OrderStatus = 'Complete')"
        SQL &= " And o.Created >= '" & StartDate & "'"
        SQL &= " And o.Created <= '" & EndDate & "'"
        SQL &= " And o.VendorId <> " & DB.Number(Session("VendorId"))

        Dim NonLLCPurchases = DB.ExecuteScalar(SQL)

        If IsDBNull(NonLLCPurchases) Then
            NonLLCPurchases = 0
        End If

        'build the xml to drive the pie chart
        If LLCPurchases = 0 And NonLLCPurchases = 0 Then
            XML = XML & "<graph caption='Sales Comparison' decimalPrecision='0' showPercentageValues='0' showNames='0' numberPrefix='$' showValues='0' showPercentageInLabel='0' pieYScale='45' pieBorderAlpha='40' pieFillAlpha='70' pieSliceDepth='15' pieRadius='75'>"
            XML = XML & "</graph>"
        Else
            XML = XML & "<graph caption='Sales Comparison' decimalPrecision='1' showPercentageValues='0' showNames='0' numberPrefix='$' showValues='0' showPercentageInLabel='0' pieYScale='45' pieBorderAlpha='40' pieFillAlpha='70' pieSliceDepth='15' pieRadius='75'>"
            XML = XML & "<set name='Non LLC Sales' value='" & NonLLCPurchases & "' color='AFD8F8' />"
            XML = XML & "<set name='LLC Sales' value='" & LLCPurchases & "' color='F6BD0F' />"
            XML = XML & "</graph>"
        End If

        'create the chart
        Return Server.UrlEncode(XML)
 
    End Function


End Class
