Imports Components
Imports DataLayer
Imports IDevSearch
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Data
Imports System.IO

Partial Class sku
    Inherits SitePage

    Protected VendorId As Integer

    Protected dbVendor As VendorRow
    Private bExport As Boolean = False

    Private EventArgsToRegister As New Collections.Specialized.StringDictionary

    Function GetVendorSupplyPhases(ByVal VendorId As Integer) As String
        Dim SupplyPhases As String = String.Empty
        Dim sConn As String = String.Empty
        Dim SQL As String = "Select sp.SupplyPhaseID, sp.SupplyPhase From SupplyPhase sp, VendorSupplyPhase vsp Where sp.SupplyPhaseID = vsp.SupplyPhaseID And vsp.VendorId = " & DB.Number(VendorId) & " order by sp.SupplyPhase"
        Dim dt As DataTable = DB.GetDataTable(SQL)
        For Each r As DataRow In dt.Rows
            SupplyPhases &= sConn & r("SupplyPhaseID")
            sConn = ","
        Next
        Return SupplyPhases
    End Function


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        VendorId = CType(Session("VendorId"), Integer)

        dbVendor = VendorRow.GetRow(Me.DB, VendorId)
        
        If VendorId = 0 Then
            Response.Redirect("/default.aspx")
        End If

        If Not IsPostBack Then

            If Request("sp") = "N" Then
                Session("SupplyPhase") = String.Empty
            End If

            Dim SQL As String
            SQL = "WITH Parents(ParentSupplyPhaseId,SupplyPhaseId,SupplyPhase,Depth) AS"
            SQL &= "("
            SQL &= "SELECT ParentSupplyPhaseId, SupplyPhaseId, SupplyPhase, 0 AS Depth FROM SupplyPhase WHERE SupplyPhaseId in (select SupplyPhaseId From VendorSupplyPhase Where VendorId = " & DB.Number(VendorId) & ")"
            SQL &= " UNION ALL "
            SQL &= "SELECT sp.ParentSupplyPhaseId, sp.SupplyPhaseId, sp.SupplyPhase, (Parents.Depth + 1) AS Depth FROM SupplyPhase sp INNER JOIN Parents ON sp.SupplyPhaseId=Parents.ParentSupplyPhaseId"
            SQL &= ")"
            SQL &= " SELECT distinct SupplyPhaseId, ParentSupplyPhaseId, SupplyPhase from Parents"

            tvSupplyPhases.DataSource = DB.GetDataTable(SQL)
            tvSupplyPhases.DataTextField = "SupplyPhase"
            tvSupplyPhases.DataValueField = "SupplyPhaseId"
            tvSupplyPhases.DataKeyField = "SupplyPhaseId"
            tvSupplyPhases.DataParentField = "ParentSupplyPhaseId"
            If txtKeyword.Text = Nothing Then
                tvSupplyPhases.UseFilter = False
            Else
                tvSupplyPhases.UseFilter = True
            End If
            tvSupplyPhases.DataBind()

            BindProducts()
            upProducts.Update()
        End If

    End Sub

    Protected Sub drpMatchingType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpMatchingType.SelectedIndexChanged
        Response.Redirect(drpMatchingType.SelectedValue)
    End Sub

    Protected Sub tvSupplyPhases_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tvSupplyPhases.SelectedIndexChanged

        AddSupplyPhase(tvSupplyPhases.CurrentNode.Value)

        BindProducts()
        upFacets.Update()
        upProducts.Update()
        ltrErrorMsg.Visible = False
    End Sub

    Protected Sub AddSupplyPhase(ByVal SupplyPhaseId As Integer)
        Dim dt As DataTable
        Dim SQL As String = "Select * From VendorSupplyPhase Where SupplyPhaseID = " & DB.Number(SupplyPhaseId) & " And VendorId = " & DB.Number(VendorId)

        dt = DB.GetDataTable(SQL)

        If dt.Rows.Count = 0 Then
            Try
                DB.BeginTransaction()
                SQL = "Insert Into VendorSupplyPhase (VendorId, SupplyPhaseID) Values (" & DB.Number(VendorId) & ", " & DB.Number(SupplyPhaseId) & ")"
                DB.ExecuteSQL(SQL)
                DB.CommitTransaction()
            Catch ex As Exception
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                AddError(ErrHandler.ErrorText(ex))
            End Try
        End If

    End Sub


    Private Sub BindProducts()

        If tvSupplyPhases.Value <> String.Empty Then
            Session("SupplyPhase") = tvSupplyPhases.Value
        End If

        Dim SQL As String = "SELECT p.ProductID, p.Product, p.SKU, vpp.VendorSKU, vpp.VendorPrice, vpp.IsSubstitution, vpp.IsDiscontinued, vpp.Updated "
        SQL &= "FROM Product p INNER JOIN "
        SQL &= "SupplyPhaseProduct spp ON p.ProductID = spp.ProductID LEFT OUTER JOIN "
        SQL &= "(Select * From VendorProductPrice Where VendorId = " & DB.Number(VendorId) & ") vpp ON p.ProductID = vpp.ProductID "
        SQL &= "Where "
        If Session("SupplyPhase") = String.Empty Then
            SQL &= "spp.SupplyPhaseID In " & DB.NumberMultiple(GetVendorSupplyPhases(VendorId))
            ltlBreadcrumb.Text = "<a href=""/"">Home</a> > All Supply Phases"
        Else
            SQL &= "spp.SupplyPhaseID = " & DB.Number(Session("SupplyPhase"))
            ltlBreadcrumb.Text = "<a href=""/"">Home</a> > <a href=""sku.aspx?sp=N"">All Supply Phases</a> > " & SupplyPhaseRow.GetRow(DB, Session("SupplyPhase")).SupplyPhase
        End If
        If txtKeyword.Text <> String.Empty And txtKeyword.Text <> "Keyword Search" Then
            SQL &= " And ("
            SQL &= "p.Product Like " & DB.FilterQuote(txtKeyword.Text)
            SQL &= " Or p.Description Like " & DB.FilterQuote(txtKeyword.Text)
            SQL &= " Or p.SKU = " & DB.Quote(txtKeyword.Text)
            SQL &= ")"
            ltlBreadcrumb.Text = "<a href=""/"">Home</a> > <a href=""sku.aspx?sp=N"">All Supply Phases</a> > " & txtKeyword.Text
            Session("SupplyPhase") = String.Empty
        End If

        Dim dtProducts As DataTable = DB.GetDataTable(SQL)
        rptProducts.DataSource = dtProducts
        rptProducts.DataBind()

        If dtProducts.Rows.Count = 0 Then
            txtNoResults.Visible = True
            rptProducts.Visible = False
        Else
            txtNoResults.Visible = False
            rptProducts.Visible = True
        End If

        If bExport Then
            SaveExport(SQL)
        End If

        btnImportUpdate.Visible = False

    End Sub



    Protected Sub rptProducts_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProducts.ItemCommand
        Dim txtSku As TextBox = CType(e.Item.FindControl("txtSku"), TextBox)
        'Dim txtPrice As TextBox = CType(e.Item.FindControl("txtPrice"), TextBox)
        Dim ckbSubstitute As CheckBox = CType(e.Item.FindControl("ckbSubstitute"), CheckBox)
        Dim ckbDiscontinued As CheckBox = CType(e.Item.FindControl("ckbDiscontinued"), CheckBox)
        Dim ErrorMsg As String = String.Empty
        Dim VendorPrice As Double = 0
        Dim VendorSku As String = String.Empty
        Dim imgReq As WebControls.Image = CType(e.Item.FindControl("imgReq"), WebControls.Image)

        btnImportUpdate.Visible = False

        Dim dtLLC As DataTable = DB.GetDataTable("Select l.* From LLC l, LLCProductPriceRequirement lp, LLCVendor lv Where l.LLCID = lp.LLCID And l.LLCID = lv.LLCID And lp.ProductId = " & DB.Number(e.CommandArgument) & " And lv.VendorId = " & DB.Number(VendorId))

        Dim PriceRequired As Boolean = dtLLC.Rows.Count > 0

        Dim dt As DataTable = DB.GetDataTable("Select Top 1 * From VendorProductPrice Where VendorId = " & DB.Number(VendorId) & " And ProductId = " & DB.Number(e.CommandArgument))

        For Each rvp As DataRow In dt.Rows
            If Not IsDBNull(rvp("VendorPrice")) Then
                VendorPrice = rvp("VendorPrice")
            End If
            If Not IsDBNull(rvp("VendorSku")) Then
                VendorSku = rvp("VendorSku")
            End If
        Next

        If VendorSku = String.Empty Then
            If txtSku.Text = String.Empty Then
                ErrorMsg = "<li>Please enter your Sku for the selected product"
            End If
        End If

        'If txtPrice.Text <> String.Empty Then
        '    If Not IsNumeric(txtPrice.Text) Then
        '        ErrorMsg &= "<li>Please enter a valid price for the selected product"
        '    ElseIf txtPrice.Text <= 0 Then
        '        ErrorMsg &= "<li>Please enter a valid price for the selected product"
        '    ElseIf VendorPrice > 0 And txtPrice.Text > VendorPrice Then
        '        ErrorMsg &= "<li>Entered price is greater than the current price"
        '    End If
        'ElseIf VendorPrice = 0 And PriceRequired Then
        '    ErrorMsg &= "<li>Price for selected product is required."
        'End If

        If ErrorMsg <> String.Empty Then
            ltrErrorMsg.Visible = True
            ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
            ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold"">" & ErrorMsg & "</span></td></tr></table>"
            imgReq.ImageUrl = "/images/exclam.gif"
            imgReq.Visible = True
            Exit Sub
        Else
            ltrErrorMsg.Visible = False
            Try
                DB.BeginTransaction()
                If dt.Rows.Count > 0 Then

                    SQL = " UPDATE VendorProductPrice SET "
                    SQL &= "Updated = " & DB.Quote(Now)
                    If txtSku.Text <> String.Empty Then
                        SQL &= ",VendorSKU = " & DB.Quote(txtSku.Text)
                    End If
                    'If txtPrice.Text <> String.Empty Then
                    '    SQL &= ",VendorPrice = " & DB.Number(txtPrice.Text)
                    'End If
                    SQL &= ",IsSubstitution = " & CInt(ckbSubstitute.Checked)
                    SQL &= ",IsDiscontinued = " & CInt(ckbDiscontinued.Checked)
                    SQL &= ",SubstituteQuantityMultiplier = " & DB.Number(0)
                    SQL &= ",IsUpload = " & CInt(False)
                    SQL &= ",UpdaterVendorAccountID = " & DB.Number(Session("VendorAccountId"))
                    SQL &= " WHERE VendorID = " & DB.Number(VendorId)
                    SQL &= " AND ProductID = " & DB.Number(e.CommandArgument)

                    DB.ExecuteSQL(SQL)

                Else

                    SQL = " INSERT INTO VendorProductPrice ("
                    SQL &= " ProductID"
                    SQL &= ",VendorID"
                    SQL &= ",VendorSKU"
                    SQL &= ",VendorPrice"
                    SQL &= ",IsSubstitution"
                    SQL &= ",IsDiscontinued"
                    SQL &= ",SubstituteQuantityMultiplier"
                    SQL &= ",IsUpload"
                    SQL &= ",Submitted"
                    SQL &= ",SubmitterVendorAccountID"
                    SQL &= ",Updated"
                    SQL &= ",UpdaterVendorAccountID"
                    SQL &= ") VALUES ("
                    SQL &= DB.Number(e.CommandArgument)
                    SQL &= "," & DB.Number(VendorId)
                    SQL &= "," & DB.Quote(txtSku.Text)
                    'SQL &= "," & DB.Number(txtPrice.Text)
                    SQL &= "," & CInt(ckbSubstitute.Checked)
                    SQL &= "," & CInt(ckbDiscontinued.Checked)
                    SQL &= "," & DB.Number(0)
                    SQL &= "," & CInt(False)
                    SQL &= "," & DB.Quote(Now)
                    SQL &= "," & DB.Number(Session("VendorAccountId"))
                    SQL &= "," & DB.Quote(Now)
                    SQL &= "," & DB.Number(Session("VendorAccountId"))
                    SQL &= ")"

                    DB.ExecuteSQL(SQL)

                End If
                DB.CommitTransaction()
            Catch ex As Exception
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                ltrErrorMsg.Text = ErrHandler.ErrorText(ex)
            End Try
            BindProducts()
            upProducts.Update()

        End If

    End Sub

    Protected Sub rptProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim imgReq As WebControls.Image = CType(e.Item.FindControl("imgReq"), WebControls.Image)
        Dim btnUpdate As Button = CType(e.Item.FindControl("btnUpdate"), Button)

        If Not IsDBNull(e.Item.DataItem("Updated")) Then
            If DateDiff("h", e.Item.DataItem("Updated"), Now()) < 2 Then
                imgReq.ImageUrl = "/images/admin/True.gif"
                imgReq.Visible = True
            End If
            btnUpdate.Text = "Update"
        Else
            btnUpdate.Text = "Add"
        End If


        Dim ckbSubstitute As CheckBox = e.Item.FindControl("ckbSubstitute")
        Dim ckbDiscontinued As CheckBox = e.Item.FindControl("ckbDiscontinued")

        If Not IsDBNull(e.Item.DataItem("IsSubstitution")) Then
            ckbSubstitute.Checked = CType(e.Item.DataItem("IsSubstitution"), Boolean)
        End If
        If Not IsDBNull(e.Item.DataItem("IsDiscontinued")) Then
            ckbDiscontinued.Checked = CType(e.Item.DataItem("IsDiscontinued"), Boolean)
        End If



    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        BindProducts()
        upProducts.Update()
        ltrErrorMsg.Visible = False
        txtKeyword.Text = ""
    End Sub


    Public Sub ExportCSV()
        bExport = True
        BindProducts()
        upProducts.Update()
    End Sub

    Private Sub SaveExport(ByVal SQL As String)
        Dim dt As DataTable = DB.GetDataTable(SQL)
        Dim SKU As String = String.Empty
        Dim Product As String = String.Empty
        Dim VendorSKU As String = String.Empty
        Dim VendorPrice As String = String.Empty
        Dim IsSubstitution As String = String.Empty
        Dim IsDiscontinued As String = String.Empty
        Dim Updated As String = String.Empty

        Dim fname As String = "/assets/vendor/product/" & Core.GenerateFileID & ".csv"
        Dim sw As IO.StreamWriter = IO.File.CreateText(Server.MapPath(fname))
        sw.WriteLine("CBUSA SKU,Product Name,Vendor SKU,Vendor Price,Substitution?,Discontinued?,Updated")
        For Each row As DataRow In dt.Rows
            If Not IsDBNull(row("SKU")) Then
                SKU = row("SKU")
            Else
                SKU = ""
            End If
            If Not IsDBNull(row("Product")) Then
                Product = row("Product")
            Else
                Product = ""
            End If
            If Not IsDBNull(row("VendorSKU")) Then
                VendorSKU = row("VendorSKU")
            Else
                VendorSKU = ""
            End If
            If Not IsDBNull(row("VendorPrice")) Then
                VendorPrice = row("VendorPrice")
            Else
                VendorPrice = ""
            End If
            If Not IsDBNull(row("IsSubstitution")) Then
                If row("IsSubstitution") Then
                    IsSubstitution = "Yes"
                Else
                    IsSubstitution = "No"
                End If
            Else
                IsSubstitution = ""
            End If
            If Not IsDBNull(row("IsDiscontinued")) Then
                If row("IsDiscontinued") Then
                    IsDiscontinued = "Yes"
                Else
                    IsDiscontinued = "No"
                End If
            Else
                IsDiscontinued = ""
            End If
            If Not IsDBNull(row("Updated")) Then
                Updated = row("Updated")
            Else
                Updated = ""
            End If
            sw.WriteLine(Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(Product) & "," & Core.QuoteCSV(VendorSKU) & "," & Core.QuoteCSV(VendorPrice) & "," & Core.QuoteCSV(IsSubstitution) & "," & Core.QuoteCSV(IsDiscontinued) & "," & Core.QuoteCSV(Updated))
        Next
        sw.Close()
        ltrErrorMsg.Text = "<div style=""float:right;""><a href=""" & fname & """><b>Click here to download CSV file</b></a></div>"
        ltrErrorMsg.Visible = True
        'Response.Redirect(fname)
    End Sub

    Protected Sub btnImport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnImport.Click
        Dim FileInfo As System.IO.FileInfo
        Dim OriginalExtension As String
        Dim NewFileName As String

        If fulDocument.NewFileName <> String.Empty Then

            OriginalExtension = System.IO.Path.GetExtension(fulDocument.MyFile.FileName)

            If OriginalExtension <> ".csv" And OriginalExtension <> ".xls" Then
                ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
                ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>Please enter a valid .csv or .xls file. Your file extension was: " & OriginalExtension & "</span></td></tr></table>"
                ltrErrorMsg.Visible = True
                Exit Sub
            End If

            fulDocument.Folder = "/assets/vendor/product/"
            fulDocument.SaveNewFile()

            FileInfo = New System.IO.FileInfo(Server.MapPath(fulDocument.Folder & fulDocument.NewFileName))

            NewFileName = Core.GenerateFileID
            FileInfo.CopyTo(Server.MapPath(fulDocument.Folder & NewFileName & OriginalExtension))

            FileInfo.Delete()
        Else
            ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
            ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>Please enter the file you want to upload</span></td></tr></table>"
            ltrErrorMsg.Visible = True
            Exit Sub
        End If

        Session("VendorImportFile") = NewFileName & OriginalExtension
        Try
            ImportCSV(Session("VendorImportFile"), False)
        Catch ex As Exception
            ltrErrorMsg.Text = Err.Description
            ltrErrorMsg.Visible = True
        End Try

    End Sub

    Protected Sub btnImportUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnImportUpdate.Click
        Try
            ImportCSV(Session("VendorImportFile"), True)
        Catch ex As Exception
            ltrErrorMsg.Text = Err.Description
            ltrErrorMsg.Visible = True
        End Try
    End Sub

    Private Sub ImportCSV(ByVal FileName As String, ByVal UpdateDatabase As Boolean)
        Dim aLine As String()
        Dim Count As Integer = 0
        Dim InsertCount As Integer = 0
        Dim UpdateCount As Integer = 0
        Dim BadCount As Integer = 0
        Dim SupplyPhases As String = GetVendorSupplyPhases(VendorId)
        Dim txtErr As String = String.Empty
        Dim tblErr As String = String.Empty
        Dim IsUpdate As Boolean = False
        Dim ProductId As Integer = 0
        Dim SKU As String = String.Empty
        Dim VendorSKU As String = String.Empty
        Dim VendorPrice As String = String.Empty
        Dim IsSubstitution As String = String.Empty
        Dim IsDiscontinued As String = String.Empty


        If Not File.Exists(Server.MapPath(fulDocument.Folder & FileName)) Then
            ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
            ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""></li>Cannot find the file to process</span></td></tr></table>"
            Exit Sub
        End If

        Dim f As StreamReader = New StreamReader(Server.MapPath(fulDocument.Folder & FileName))
        While Not f.EndOfStream

            Count = Count + 1

            Dim sLine As String = f.ReadLine()

            Dim bInside As Boolean = False
            For iLoop As Integer = 1 To Len(sLine)
                If Mid(sLine, iLoop, 1) = """" Then
                    If bInside = False Then
                        bInside = True
                    Else
                        bInside = False
                    End If
                End If
                If Mid(sLine, iLoop, 1) = "," Then
                    If Not bInside Then
                        sLine = Left(sLine, iLoop - 1) & "|" & Mid(sLine, iLoop + 1, Len(sLine) - iLoop)
                    End If
                End If
            Next

            aLine = sLine.Split("|")


            If aLine.Length > 5 AndAlso aLine(0) <> String.Empty Then
                IsUpdate = False
                ProductId = 0
                SKU = Trim(Core.StripDblQuote(aLine(0)))
                VendorSKU = Trim(Core.StripDblQuote(aLine(2)))
                VendorPrice = Trim(Core.StripDblQuote(aLine(3)))
                IsSubstitution = Trim(Core.StripDblQuote(aLine(4)))
                IsDiscontinued = Trim(Core.StripDblQuote(aLine(5)))

                If LCase(IsSubstitution) = "yes" Or LCase(IsSubstitution) = "y" Then
                    IsSubstitution = 1
                Else
                    IsSubstitution = 0
                End If

                If LCase(IsDiscontinued) = "yes" Or LCase(IsDiscontinued) = "y" Then
                    IsDiscontinued = 1
                Else
                    IsDiscontinued = 0
                End If

                If Count = 1 And SKU <> "CBUSA SKU" Then
                    ltrErrorMsg.Text = "<table><tr><td><img src=""/images/exclam.gif"" border=""0""/></td>"
                    ltrErrorMsg.Text &= "<td valign=""middle""><span class=""red bold""><li>The file you uploaded does not appear to be in the correct format. The file format must be identical to the one used in the Export CSV process.</td></tr></table>"
                    Exit Sub
                End If

                If SKU = "CBUSA SKU" Then
                    Continue While
                End If

                txtErr = String.Empty

                'SKU cannot be longer than 20 char 
                If VendorSKU.Length > 20 Then
                    BadCount = BadCount + 1
                    txtErr &= "<li>Vendor SKU too long"
                    tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                    Continue While
                End If

                'Price must be a numeric value and greater than 0
                If VendorPrice <> String.Empty Then
                    If Not IsNumeric(VendorPrice) Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Invalid price"
                        tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    ElseIf VendorPrice < 0 Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Negative price"
                        tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    End If
                End If
                If txtErr = String.Empty Then
                    Dim SQL As String = "SELECT Top 1 p.ProductID, p.Product, p.SKU, vpp.VendorSKU, Cast(vpp.VendorPrice As Money) As VendorPrice, vpp.IsSubstitution, vpp.IsDiscontinued, vpp.Updated "
                    SQL &= "FROM Product p INNER JOIN "
                    SQL &= "SupplyPhaseProduct spp ON p.ProductID = spp.ProductID LEFT OUTER JOIN "
                    SQL &= "(Select * From VendorProductPrice Where VendorId = " & DB.Number(VendorId) & ") vpp ON p.ProductID = vpp.ProductID "
                    SQL &= "Where "
                    SQL &= "spp.SupplyPhaseID In " & DB.NumberMultiple(SupplyPhases)
                    SQL &= " and p.SKU = " & DB.Quote(SKU)

                    Dim dtProduct As DataTable = DB.GetDataTable(SQL)

                    'Product must exist
                    If dtProduct.Rows.Count = 0 Then
                        BadCount = BadCount + 1
                        txtErr &= "<li>Product Not Found!"
                        tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                        Continue While
                    Else
                        For Each dr As DataRow In dtProduct.Rows
                            ProductId = dr("ProductId")
                            Dim dt As DataTable = DB.GetDataTable("Select Top 1 * From VendorProductPrice Where VendorId = " & DB.Number(VendorId) & " And ProductId = " & DB.Number(ProductId))
                            IsUpdate = dt.Rows.Count > 0
                            Dim dtLLC As DataTable = DB.GetDataTable("Select l.* From LLC l, LLCProductPriceRequirement lp, LLCVendor lv Where l.LLCID = lp.LLCID And l.LLCID = lv.LLCID And lp.ProductId = " & DB.Number(ProductId) & " And lv.VendorId = " & DB.Number(VendorId))
                            Dim PriceRequired As Boolean = dtLLC.Rows.Count > 0

                            If Not IsDBNull(dr("VendorPrice")) Then
                                'Vendor price cannot be greater than current

                                If dr("VendorPrice") > 0 And VendorPrice > dr("VendorPrice") Then
                                    txtErr &= "<li>New price is greater than current"
                                End If

                            ElseIf PriceRequired And VendorPrice = String.Empty Then
                                txtErr &= "<li>Price required"
                            End If

                            'Vendor SKU is required if not already provided
                            If VendorSKU = String.Empty Then
                                If SKU = String.Empty Then
                                    txtErr &= "<li>Vendor SKU empty"
                                End If
                            End If
                        Next
                    End If
                End If

                If txtErr <> String.Empty Then
                    BadCount = BadCount + 1
                    tblErr &= "<tr class=""red""><td>" & SKU & "</td><td>" & txtErr & "</td><tr>"
                ElseIf Not UpdateDatabase Then
                    If IsUpdate Then
                        UpdateCount = UpdateCount + 1
                    Else
                        InsertCount = InsertCount + 1
                    End If
                End If
            Else
                BadCount = BadCount + 1
                tblErr &= "<tr class=""red""><td>Row: " & Count & "</td><td>BAD ROW</td><tr>"
                Continue While
            End If

            If txtErr = String.Empty And UpdateDatabase Then

                Try
                    DB.BeginTransaction()
                    If IsUpdate Then
                        SQL = " UPDATE VendorProductPrice SET "
                        SQL &= "Updated = " & DB.Quote(Now)
                        If SKU <> String.Empty Then
                            SQL &= ",VendorSKU = " & DB.Quote(VendorSKU)
                        End If
                        If VendorPrice <> String.Empty Then
                            SQL &= ",VendorPrice = " & DB.Number(VendorPrice)
                        End If
                        If IsSubstitution <> String.Empty Then
                            SQL &= ",IsSubstitution = " & CInt(IsSubstitution)
                        End If

                        If IsDiscontinued <> String.Empty Then
                            SQL &= ",IsDiscontinued = " & CInt(IsDiscontinued)
                        End If
                        SQL &= ",SubstituteQuantityMultiplier = " & DB.Number(0)
                        SQL &= ",IsUpload = " & CInt(True)
                        SQL &= ",UpdaterVendorAccountID = " & DB.Number(Session("VendorAccountId"))
                        SQL &= " WHERE VendorID = " & DB.Number(VendorId)
                        SQL &= " AND ProductId = " & DB.Quote(ProductId)

                        DB.ExecuteSQL(SQL)

                        UpdateCount = UpdateCount + 1

                    Else

                        SQL = " INSERT INTO VendorProductPrice ("
                        SQL &= " ProductID"
                        SQL &= ",VendorID"
                        SQL &= ",VendorSKU"
                        SQL &= ",VendorPrice"
                        SQL &= ",IsSubstitution"
                        SQL &= ",IsDiscontinued"
                        SQL &= ",SubstituteQuantityMultiplier"
                        SQL &= ",IsUpload"
                        SQL &= ",Submitted"
                        SQL &= ",SubmitterVendorAccountID"
                        SQL &= ",Updated"
                        SQL &= ",UpdaterVendorAccountID"
                        SQL &= ") VALUES ("
                        SQL &= DB.Number(ProductId)
                        SQL &= "," & DB.Number(VendorId)
                        SQL &= "," & DB.Quote(VendorSKU)
                        SQL &= "," & DB.Number(VendorPrice)
                        SQL &= "," & CInt(IsSubstitution)
                        SQL &= "," & CInt(IsDiscontinued)
                        SQL &= "," & DB.Number(0)
                        SQL &= "," & CInt(True)
                        SQL &= "," & DB.Quote(Now)
                        SQL &= "," & DB.Number(Session("VendorAccountId"))
                        SQL &= "," & DB.Quote(Now)
                        SQL &= "," & DB.Number(Session("VendorAccountId"))
                        SQL &= ")"

                        DB.ExecuteSQL(SQL)

                        InsertCount = InsertCount + 1

                    End If
                    DB.CommitTransaction()
                Catch ex As Exception
                    If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                    ltrErrorMsg.Text = ErrHandler.ErrorText(ex)
                    btnImportUpdate.Visible = False
                    Exit Sub
                End Try
            End If
        End While
        f.Close()

        Count = Count - 1

        tblErr = "<tr class=""red bold""><td>Invalid Entries</td><td>" & BadCount & "</td><tr>" & tblErr

        If UpdateDatabase Then
            tblErr = "<tr class=""green""><td>Products Added</td><td>" & InsertCount & "</td><tr>" & tblErr
            tblErr = "<tr class=""green""><td>Products Updated</td><td>" & UpdateCount & "</td><tr>" & tblErr
            ltrErrorMsg.Text = "<b>Data import process completed with success. Please review the report below for more details.</b>"
            BindProducts()
            upProducts.Update()

        Else
            tblErr = "<tr class=""green""><td>Products To Add</td><td>" & InsertCount & "</td><tr>" & tblErr
            tblErr = "<tr class=""green""><td>Products To Update</td><td>" & UpdateCount & "</td><tr>" & tblErr
            btnImportUpdate.Visible = True
            ltrErrorMsg.Text = "<b>File uploaded successfully. Please review the report below and confirm the data import.</b>"
        End If

        tblErr = "<tr class=""green""><td>Valid Entries</td><td>" & Count - BadCount & "</td><tr>" & tblErr
        tblErr = "<tr class=""blue bold""><td>Total Rows Processed</td><td>" & Count & "</td><tr>" & tblErr

        ltrErrorMsg.Text &= "<hr><table align=""center"">" & tblErr & "</table><hr>"
        ltrErrorMsg.Visible = True

    End Sub

End Class
