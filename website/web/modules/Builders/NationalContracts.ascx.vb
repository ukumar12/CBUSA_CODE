Imports Components
Imports DataLayer
Imports Controls

Partial Class modules_Builder_NationalCOntracts
    Inherits ModuleControl
    Public builderllcid As Integer
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If IsAdminDisplay Then
            Exit Sub
        End If
        builderllcid = BuilderRow.GetRow(DB, Session("BuilderID")).LLCID
        Try
            gvListAvail.BindList = AddressOf BindListJoined
            gvListAvail.BindList = AddressOf BindListAvailable

            If Not IsPostBack Then
                BindListJoined()
                BindListAvailable()
            End If
        Catch ex As Exception
            Logger.Error("Contracts module" & ex.ToString)
        End Try


    End Sub

    'Private Sub BindListJoined()
    '    BindList(" WHERE ContractID IN (SELECT ContractID FROM NationalContractBuilder WHERE BuilderId = " & Session("BuilderId") & ") AND   ArchiveDate = 0 ", gvListJoined)
    'End Sub

    'Private Sub BindListAvailable()
    '    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
    '    Dim Where As String = " WHERE ContractID NOT IN (SELECT ContractID FROM NationalContractBuilder WHERE BuilderId = " & dbBuilder.BuilderID & ")" & _
    '        " AND ContractID IN (SELECT ContractID FROM NationalContractLLC WHERE LLCID = " & dbBuilder.LLCID & ")" & _
    '        " AND ArchiveDate = 0 "
    '    BindList(Where, gvListAvail)
    'End Sub
    Private Sub BindListJoined()
        Dim SQLFields, SQL As String
        SQLFields = "SELECT TOP " & (gvListJoined.PageIndex + 1) * gvListJoined.PageSize & " * "
        SQL = " FROM NationalContract    WHERE ContractID IN (SELECT ContractID FROM NationalContractBuilder WHERE BuilderId = " & Session("BuilderId") & ") AND   ArchiveDate = 0 "

        gvListJoined.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvListJoined.SortByAndOrder)
        gvListJoined.DataSource = res.DefaultView
        gvListJoined.DataBind()
    End Sub

    Private Sub BindListAvailable()
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        Dim SQLFields, SQL As String
        Dim Where As String = " WHERE ContractID NOT IN (SELECT ContractID FROM NationalContractBuilder WHERE BuilderId = " & dbBuilder.BuilderID & ")" & _
           " AND ContractID IN (SELECT ContractID FROM NationalContractLLC WHERE LLCID = " & dbBuilder.LLCID & ")" & _
           " AND ArchiveDate = 0 "
        SQLFields = "SELECT TOP " & (gvListAvail.PageIndex + 1) * gvListAvail.PageSize & " * "
        SQL = " FROM NationalContract    " & Where

        gvListAvail.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvListAvail.SortByAndOrder)
        gvListAvail.DataSource = res.DefaultView
        gvListAvail.DataBind()
    End Sub
    
    Protected Sub gvListJoined_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvListJoined.RowCommand
        If e.CommandName <> "ViewDetails" Then
            Exit Sub
        End If
        Dim id As Integer = e.CommandArgument
        If e.CommandName = "ViewDetails" Then
            Session("ContractID") = id
            Response.Redirect("/contracts.aspx")
        End If
    End Sub

    Protected Sub gvListAvail_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvListAvail.RowCommand
        If e.CommandName <> "ViewDetails" Then
            Exit Sub
        End If
        Dim id As Integer = e.CommandArgument
        If e.CommandName = "ViewDetails" Then
            Session("ContractID") = id
            Response.Redirect("/contracts.aspx")
        End If
    End Sub

    Protected Sub gvListAvail_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvListAvail.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        Dim lnkListAvailDetails As Button = e.Row.FindControl("lnkListAvailDetails")
        Dim lnkListAvailLandingPage As HyperLink = e.Row.FindControl("lnkListAvailLandingPage")
        'Dim ltlEndDate As Literal = e.Row.FindControl("ltlEndDate")
        Dim ltlManufacturer As Literal = e.Row.FindControl("ltlManufacturer")
        Dim ltlTitle As Literal = e.Row.FindControl("ltlTitle")
        Dim lnkDetails As Button = e.Row.FindControl("lnkDetails")
        Dim ltlContractTerms As Literal = e.Row.FindControl("ltlContractTerms")
        Dim lnkAllDetails As HyperLink = e.Row.FindControl("lnkAllDetails")
        ' ltlEndDate.Text = Core.GetDate(e.Row.DataItem("EndDate")).ToString("dd MMM, yyyy")

        Dim Manufacturer As String = String.Empty
        If Not IsDBNull(e.Row.DataItem("Manufacturer")) Then
            Manufacturer = e.Row.DataItem("Manufacturer")
        End If

        Dim Contractterm As String = String.Empty
        If Not IsDBNull(e.Row.DataItem("ContractTerm")) Then
            Contractterm = e.Row.DataItem("ContractTerm")
        End If
        ltlContractTerms.Text = Contractterm
        If Not IsDBNull(e.Row.DataItem("ManufacturerSite")) Then
            Dim ManufacturerLink As String = e.Row.DataItem("ManufacturerSite")
            If Not ManufacturerLink.StartsWith("http") Then ManufacturerLink = "http://" & ManufacturerLink
            Manufacturer = "<a target='_blank' href='" & ManufacturerLink & "'>" & Manufacturer & "</a>"
        End If
        ltlManufacturer.Text = Manufacturer

        Dim Title As String = e.Row.DataItem("Title")
        ltlTitle.Text = Title




        If Not IsDBNull(e.Row.DataItem("DescriptionPage")) Then
            Dim TitleLink As String = e.Row.DataItem("DescriptionPage")
            If Not TitleLink.StartsWith("http") Then TitleLink = "http://" & TitleLink
            lnkListAvailLandingPage.NavigateUrl = TitleLink
            lnkListAvailDetails.Visible = False
        Else
            lnkListAvailDetails.Visible = False
            lnkListAvailLandingPage.Visible = False
        End If
    End Sub
    Protected Sub gvListJoined_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvListJoined.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub


        'Dim ltlEndDate As Literal = e.Row.FindControl("ltlEndDate")
        Dim ltlManufacturer As Literal = e.Row.FindControl("ltlManufacturer")
        Dim ltlTitle As Literal = e.Row.FindControl("ltlTitle")
        Dim lnkListJoinedDetails As Button = e.Row.FindControl("lnkListJoinedDetails")
        Dim ltlContractTerms As Literal = e.Row.FindControl("ltlContractTerms")
        Dim lnkListJoinedLandingPage As HyperLink = e.Row.FindControl("lnkListJoinedLandingPage")
        ' ltlEndDate.Text = Core.GetDate(e.Row.DataItem("EndDate")).ToString("dd MMM, yyyy")

        Dim Manufacturer As String = String.Empty
        If Not IsDBNull(e.Row.DataItem("Manufacturer")) Then
            Manufacturer = e.Row.DataItem("Manufacturer")
        End If

        Dim Contractterm As String = String.Empty
        If Not IsDBNull(e.Row.DataItem("ContractTerm")) Then
            Contractterm = e.Row.DataItem("ContractTerm")
        End If
        ltlContractTerms.Text = Contractterm
        If Not IsDBNull(e.Row.DataItem("ManufacturerSite")) Then
            Dim ManufacturerLink As String = e.Row.DataItem("ManufacturerSite")
            If Not ManufacturerLink.StartsWith("http") Then ManufacturerLink = "http://" & ManufacturerLink
            Manufacturer = "<a target='_blank' href='" & ManufacturerLink & "'>" & Manufacturer & "</a>"
        End If
        ltlManufacturer.Text = Manufacturer

        Dim Title As String = e.Row.DataItem("Title")
        ltlTitle.Text = Title




       If Contractlist(DB, e.Row.DataItem("ContractID"), builderllcid) > 0 Then
            lnkListJoinedDetails.Visible = True
            lnkListJoinedLandingPage.Visible = False
        Else
            lnkListJoinedDetails.Visible = False
            lnkListJoinedLandingPage.Visible = False
        End If
    End Sub

    Protected Function Contractlist(ByVal DB As Database, ByVal ContractID As Integer, ByVal LLCID As Integer) As Integer
        Dim Sql As String = String.Empty
        Sql &= "select ncpc.*, ncpl.LLCID , ncpcc .ContractID  from NCPContent ncpc inner join NCPContentLLC ncpl " & _
                "on ncpc .NCPContentID = ncpl .NCPContentID inner join NCPContentContract ncpcc on ncpc .NCPContentID = ncpcc .NCPContentID " & _
                " where ncpcc.ContractID = " & ContractID & " AND ncpc.Isactive = 1 AND ncpl.LLCID = " & LLCID
        Dim dt As DataTable = DB.GetDataTable(Sql)
        Return dt.Rows.Count
    End Function


End Class
