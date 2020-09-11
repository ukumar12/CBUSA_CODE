Imports DataLayer
Imports Components
Imports Vindicia
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Data
Imports System.IO
Imports Controls
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer
Imports System.Collections.Generic
Imports System.Web.Services
Imports Utility
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.Windows.Forms

Partial Class BuilderBillingPlanUpgrade
    Inherits System.Web.UI.Page

    Private Sub frmBuilderUpgradation_Load(sender As Object, e As EventArgs) Handles frmBuilderUpgradation.Load

        If Not Page.IsPostBack Then
            Session("BuilderCounter") = "0"
        End If

        PopulateBuilderList()
    End Sub

    Private Sub PopulateBuilderList()

        Dim sqlConn As SqlConnection = New SqlConnection(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))
        sqlConn.Open()

        'Dim strQuery As String = ("SELECT B.BuilderID, B.HistoricID, B.CompanyName, COALESCE(BP.ExternalKey, 'N/A') AS ExternalKey " &
        '                           "FROM Builder B LEFT JOIN BuilderBillingPlan BBP ON B.BuilderId = BBP.BuilderId " &
        '                           "LEFT JOIN BillingPlan BP ON BBP.SubBillingPlanId = BP.BillingPlanId WHERE B.IsActive = 1 " &
        '                           "AND B.HistoricID IN (4906) " &
        '                           "ORDER BY B.BuilderID DESC")

        Dim strQuery As String = ("SELECT BBP.BuilderId, B.HistoricID, B.CompanyName, BP.ExternalKey AS ExternalKey " &
                                    "FROM BuilderBillingPlan BBP JOIN Builder B ON B.BuilderId = BBP.BuilderId " &
                                    "JOIN BillingPlan BP ON BBP.SubBillingPlanId = BP.BillingPlanId " &
                                    "WHERE BBP.BuilderId IN (SELECT BuilderId FROM BillingPlanUpgrade_12172019 " &
                                    "WHERE LastBillDate IS NOT NULL AND UpgradeNow = 2 AND CAST(LastBillDate AS datetime) BETWEEN '11-26-2019' AND '11-30-2019')")

        Dim sqlDR As SqlDataReader

        Dim sqlComm As New SqlCommand(strQuery, sqlConn)
        sqlDR = sqlComm.ExecuteReader()

        Dim iCounter As Int16 = 1
        Dim iFirstPendingRowIndex As Int16 = -1

        If sqlDR.HasRows Then
            While sqlDR.Read()
                Dim tblRow As New TableRow

                Dim tblCellCounter As New TableCell
                tblCellCounter.Text = iCounter
                tblCellCounter.HorizontalAlign = HorizontalAlign.Center
                tblRow.Cells.Add(tblCellCounter)

                Dim tblCellBuilderID As New TableCell
                tblCellBuilderID.Text = sqlDR("BuilderID")
                tblCellBuilderID.HorizontalAlign = HorizontalAlign.Center
                tblRow.Cells.Add(tblCellBuilderID)

                Dim tblCellAccID As New TableCell
                tblCellAccID.Text = sqlDR("HistoricID")
                tblCellAccID.HorizontalAlign = HorizontalAlign.Center
                tblRow.Cells.Add(tblCellAccID)

                Dim tblCellBuilderName As New TableCell
                tblCellBuilderName.Text = sqlDR("CompanyName")
                tblCellBuilderName.HorizontalAlign = HorizontalAlign.Center
                tblRow.Cells.Add(tblCellBuilderName)

                Dim tblCellBillingPlan As New TableCell
                tblCellBillingPlan.Text = sqlDR("ExternalKey")
                tblCellBillingPlan.HorizontalAlign = HorizontalAlign.Center
                tblRow.Cells.Add(tblCellBillingPlan)

                Dim tblCellStatus As New TableCell
                'If sqlDR("ExternalKey") = "VIN-SUB-0099-01" Then
                '    tblCellStatus.Text = "Upgraded"
                'ElseIf sqlDR("ExternalKey") = "N/A" Then
                '    tblCellStatus.Text = "-"
                'Else
                '    tblCellStatus.Text = "Pending"

                '    If iFirstPendingRowIndex = -1 Then
                '        iFirstPendingRowIndex = iCounter
                '        hdnBuilderCounter.Value = iFirstPendingRowIndex
                '    End If
                'End If

                tblCellStatus.Text = "Pending"

                If iFirstPendingRowIndex = -1 Then
                    iFirstPendingRowIndex = iCounter
                    '  Session("BuilderCounter") = iFirstPendingRowIndex
                End If

                tblCellStatus.HorizontalAlign = HorizontalAlign.Center
                tblRow.Cells.Add(tblCellStatus)

                tblBuilderList.Rows.Add(tblRow)

                iCounter = iCounter + 1
            End While
        End If

    End Sub

    Private Sub btnUpgradeBillingPlan_Click(sender As Object, e As EventArgs) Handles btnUpgradeBillingPlan.Click
        hdnUpgradeStatus.Value = "InProgress"

        Dim CurrentRowCounter As Int16 = Convert.ToInt16(Session("BuilderCounter")) ' + 1

        If CurrentRowCounter < tblBuilderList.Rows.Count Then

            Dim CurrentRow As New TableRow
            CurrentRow = tblBuilderList.Rows.Item(CurrentRowCounter)

            Dim strBuilderAccID As String = CurrentRow.Cells.Item(1).Text
            Dim strCurrentBillingPlan As String = CurrentRow.Cells.Item(4).Text
            Dim strCurrentStatus As String = CurrentRow.Cells.Item(5).Text

            If strCurrentStatus = "Pending" Then
                If strCurrentBillingPlan = "VIN-SUB-0099-01" Or strCurrentBillingPlan = "VIN-SUB-0120-01" Or strCurrentBillingPlan = "VIN-SUB-0000-01" Then
                    '----------- UPGRADE BILLING PLAN -------------
                    Dim strReturn As String = UpdateBuilderAutoBill(Convert.ToInt32(strBuilderAccID), strCurrentBillingPlan)

                    If strReturn = "Upgraded" Then
                        'Dim objDB As New Database
                        'objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

                        'Dim dbBBP As BuilderBillingPlanRow = BuilderBillingPlanRow.GetRow(objDB, Convert.ToInt32(strBuilderAccID))
                        'dbBBP.SubBillingPlanId = BillingPlanRow.GetDefaultBillingPlan(objDB, 2).BillingPlanId
                        'dbBBP.Update()
                    End If
                    '----------------------------------------------

                    CurrentRow.Cells.Item(5).Text = strReturn

                Else
                    CurrentRow.Cells.Item(5).Text = "Skipped"
                End If
                If strCurrentBillingPlan <> "VIN-SUB-0099-01" And strCurrentBillingPlan <> "VIN-SUB-0120-01" Then


                Else

                End If
            End If

            Session("BuilderCounter") = CurrentRowCounter + 1
        Else
            hdnUpgradeStatus.Value = "Completed"
        End If

    End Sub

    Private Sub btnStopProcess_Click(sender As Object, e As EventArgs) Handles btnStopProcess.Click
        hdnUpgradeStatus.Value = "Stopped"
    End Sub

    '------------- UPDATE BUILDER AUTOBILL ---------------
    Private Function UpdateBuilderAutoBill(ByVal pBuilderId As Integer, ByVal pBillingPlan As String) As String
        Dim objDB As New Database
        objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

        Dim p As VindiciaPaymentProcessor

        p = New VindiciaPaymentProcessor(objDB)
        p.IsTestMode = DataLayer.SysParam.GetValue(objDB, "TestMode")

        Dim VinBillingPlans As Vindicia.BillingPlan() = p.GetBillingPlansForAdmin()
        Dim selectedBillingPlan As New Vindicia.BillingPlan

        For Each bp As BillingPlan In VinBillingPlans
            If bp.status = BillingPlanStatus.Active Then
                If bp.merchantBillingPlanId = pBillingPlan Then             ' "VIN-SUB-0099-01"
                    selectedBillingPlan = bp
                End If
            End If
        Next

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(objDB, pBuilderId)
        Dim autoBills() As Vindicia.AutoBill = p.GetAutobills(dbBuilder)

        If Not autoBills Is Nothing Then
            For Each ab As Vindicia.AutoBill In autoBills
                Dim Product As String = ab.items(0).product.merchantProductId
                If Product = "Subscription" Then
                    If ab.status = Vindicia.AutoBillStatus.Active Then

                        Dim tran1 As New Vindicia.Transaction
                        Dim trans As Vindicia.Transaction()

                        Dim srd As String = "'{""transactions"": [""VID"", ""merchantTransactionId"", ""amount"", ""timestamp""]}'"
                        tran1.fetchByAccount(srd, ab.account, False, True, 0, True, 10, True, trans)

                        Dim strBillingLastSuccess As String = trans(0).originalActivityDate.Date.ToShortDateString()

                        Dim Tran As Vindicia.Transaction = Nothing
                        Dim Ref As Vindicia.Refund() = Nothing

                        Dim sitem As New Vindicia.AutoBillItem
                        For Each itm As Vindicia.AutoBillItem In ab.items
                            sitem = itm
                        Next

                        Dim atmo() As Vindicia.AutoBillItemModification = Nothing
                        ReDim atmo(0)

                        Dim abi As New Vindicia.AutoBillItem

                        Dim ss As New AutoBillItemModification()
                        ss.removeAutoBillItem = sitem

                        sitem.startDate = Now.Date.AddDays(1).Date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")
                        ss.addAutoBillItem = sitem

                        atmo(0) = ss

                        Dim ret As Vindicia.Return

                        ret = ab.modify("", False, "nextBill", selectedBillingPlan, atmo, False, False, Tran, sitem.startDate, True, Ref)
                        'Return strBillingLastSuccess
                        If ret.returnString = "OK" Then
                            Return "Upgraded"
                        Else
                            Return "Error"
                        End If
                    End If
                End If
            Next
        End If

        Return "Skipped"

    End Function

End Class
