Imports Components
Imports System.Net.Mail
Imports System.Configuration.ConfigurationManager
Imports DataLayer
Imports System.IO
Imports System.Web
Imports System.Net
Public Class CleanDuplicateProducts
    Public Shared Sub Run(ByVal DB As Database)
        Try
            Console.WriteLine("Running CleanDuplicateProducts ... ")

            Dim dt As DataTable = DB.GetDataTable("Select Distinct Product From Product Where Product In (Select Product From Product Group By Product Having (COUNT(Product) > 1)) Order By Product")
            Dim Count As Integer = 0
            For Each row As DataRow In dt.Rows
                Count += 1
                Console.WriteLine("Processing row " & Count & " of " & dt.Rows.Count & " - Product: " & row("Product"))
                Dim GoodProductId As Integer = 0
                Try
                    GoodProductId = DB.ExecuteScalar("Select Min(ProductId) From Product Where Product = " & DB.Quote(row("Product")))
                Catch ex As Exception
                End Try
                If GoodProductId > 0 Then
                    DB.BeginTransaction()
                    Dim BadProducts As String = "Select ProductId From Product Where Product = " & DB.Quote(row("Product")) & " And ProductId <> " & DB.Number(GoodProductId)
                    Dim dtbad As DataTable = DB.GetDataTable(BadProducts)
                    If dtbad.Rows.Count > 0 Then
                        DB.ExecuteSQL("Insert Into ProductDuplicate (ProductID) " & BadProducts)
                        For Each r As DataRow In dtbad.Rows
                            If DB.GetDataTable("Select ProductID From BuilderIndicatorProduct Where ProductID = " & DB.Number(GoodProductId)).Rows.Count > 0 Then
                                DB.ExecuteSQL("Delete From BuilderIndicatorProduct Where ProductID = " & DB.Number(r("ProductId")))
                            Else
                                DB.ExecuteSQL("Update BuilderIndicatorProduct Set ProductID = " & DB.Number(GoodProductId) & " Where ProductID In (" & BadProducts & ")")
                            End If
                            If DB.GetDataTable("Select ProductID From LLCProductPriceRequirement Where ProductID = " & DB.Number(GoodProductId)).Rows.Count > 0 Then
                                DB.ExecuteSQL("Delete From LLCProductPriceRequirement Where ProductID = " & DB.Number(r("ProductId")))
                            Else
                                DB.ExecuteSQL("Update LLCProductPriceRequirement Set ProductID = " & DB.Number(GoodProductId) & " Where ProductID In (" & BadProducts & ")")
                            End If
                            If DB.GetDataTable("Select ProductID From PriceComparisonVendorProductPrice Where ProductID = " & DB.Number(GoodProductId)).Rows.Count > 0 Then
                                DB.ExecuteSQL("Delete From PriceComparisonVendorProductPrice Where ProductID = " & DB.Number(r("ProductId")))
                            Else
                                DB.ExecuteSQL("Update PriceComparisonVendorProductPrice Set ProductID = " & DB.Number(GoodProductId) & " Where ProductID In (" & BadProducts & ")")
                            End If
                            If DB.GetDataTable("Select ProductID From SupplyPhaseProduct Where ProductID = " & DB.Number(GoodProductId)).Rows.Count > 0 Then
                                DB.ExecuteSQL("Delete From SupplyPhaseProduct Where ProductID = " & DB.Number(r("ProductId")))
                            Else
                                DB.ExecuteSQL("Update SupplyPhaseProduct Set ProductID = " & DB.Number(GoodProductId) & " Where ProductID In (" & BadProducts & ")")
                            End If
                            If DB.GetDataTable("Select ProductID From TakeOffProduct Where ProductID = " & DB.Number(GoodProductId)).Rows.Count > 0 Then
                                DB.ExecuteSQL("Delete From TakeOffProduct Where ProductID = " & DB.Number(r("ProductId")))
                            Else
                                DB.ExecuteSQL("Update TakeOffProduct Set ProductID = " & DB.Number(GoodProductId) & " Where ProductID In (" & BadProducts & ")")
                            End If
                            If DB.GetDataTable("Select ProductID From VendorProductPrice Where ProductID = " & DB.Number(GoodProductId)).Rows.Count > 0 Then
                                DB.ExecuteSQL("Delete From VendorProductPrice Where ProductID = " & DB.Number(r("ProductId")))
                            Else
                                DB.ExecuteSQL("Update VendorProductPrice Set ProductID = " & DB.Number(GoodProductId) & " Where ProductID In (" & BadProducts & ")")
                            End If
                            If DB.GetDataTable("Select ProductID From VendorProductPriceHistory Where ProductID = " & DB.Number(GoodProductId)).Rows.Count > 0 Then
                                DB.ExecuteSQL("Delete From VendorProductPriceHistory Where ProductID = " & DB.Number(r("ProductId")))
                            Else
                                DB.ExecuteSQL("Update VendorProductPriceHistory Set ProductID = " & DB.Number(GoodProductId) & " Where ProductID In (" & BadProducts & ")")
                            End If
                            If DB.GetDataTable("Select ProductID From VendorProductPriceRequest Where ProductID = " & DB.Number(GoodProductId)).Rows.Count > 0 Then
                                DB.ExecuteSQL("Delete From VendorProductPriceRequest Where ProductID = " & DB.Number(r("ProductId")))
                            Else
                                DB.ExecuteSQL("Update VendorProductPriceRequest Set ProductID = " & DB.Number(GoodProductId) & " Where ProductID In (" & BadProducts & ")")
                            End If
                            If DB.GetDataTable("Select ProductID From VendorProductSubstitute Where ProductID = " & DB.Number(GoodProductId)).Rows.Count > 0 Then
                                DB.ExecuteSQL("Delete From VendorProductSubstitute Where ProductID = " & DB.Number(r("ProductId")))
                            Else
                                DB.ExecuteSQL("Update VendorProductSubstitute Set ProductID = " & DB.Number(GoodProductId) & " Where ProductID In (" & BadProducts & ")")
                            End If
                        Next
                    End If
                    DB.CommitTransaction()
                End If
            Next

            Console.WriteLine("Finished CleanDuplicateProducts ... ")
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
            DB.RollbackTransaction()
        Finally
            If Not DB Is Nothing Then DB.Close()
        End Try
    End Sub
End Class
