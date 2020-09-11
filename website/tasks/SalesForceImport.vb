Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Public Class SalesForceImport

    Public Shared Sub Run(ByVal DB As Database)

        Dim wrapper As New SalesForceWrapperClient()
        wrapper.username = AppSettings("SalesForceUsername")
        wrapper.password = AppSettings("SalesForcePassword")
        wrapper.Login()

        Dim sQuery As String = _
            " select " _
            & "     Id," _
            & "     Agreement_Start_Date__c," _
            & "     Products_Offered__c, " _
            & "     Services_Offered__c, " _
            & "     Payment_Terms_for_CBUSA_Members__c, " _
            & "     If_yes_which_credit_cards__c, " _
            & "     Do_you_except_credit_cards__c, " _
            & "     Location_of_Supply__c, " _
            & "     Subsidiary_Explain__c, " _
            & "     Subsidiary__c, " _
            & "     Business_Type__c, " _
            & "     LLC_City__c, " _
            & "     Name, " _
            & "     BillingCity, " _
            & "     BillingCountry, " _
            & "     BillingState, " _
            & "     BillingPostalCode, " _
            & "     BillingStreet, " _
            & "     ShippingCity, " _
            & "     ShippingState, " _
            & "     ShippingCountry, " _
            & "     ShippingPostalCode, " _
            & "     ShippingStreet, " _
            & "     Phone, " _
            & "     Fax, " _
            & "     Website, " _
            & "     Year_Business_was_started__c," _
            & "     of_employees__c," _
            & "     of_homes_built_since_in_business__c," _
            & "     of_homes_built_last_year__c," _
            & "     of_homes_projected_this_year__c," _
            & "     Price_range_excluding_land__c," _
            & "     Avg_sqft__c," _
            & "     Company_revenue_last_year__c," _
            & "     Projected_company_revenue_this_year__c," _
            & "     Total_COGS_2006__c," _
            & "     Projected__c," _
            & "     Areas_Cities_Counties_in_Which_y__c," _
            & "     Company_Memberships_or_Affiliations__c," _
            & "     Industry_related_awards_received__c," _
            & "     Type" _
            & " from Account"

        Dim res As sforce.QueryResult = wrapper.executeQuery(sQuery)
        For Each item As sforce.sObject In res.records
            Dim acct As sforce.Account = item

            Select Case acct.Type.ToLower
                Case "builder"
                    Dim dbBuilder As BuilderRow = BuilderRow.GetBuilderByCRMID(DB, acct.Id)
                    If dbBuilder.BuilderID <> Nothing Then
                        Continue For
                    End If

                    Try

                        DB.BeginTransaction()

                        dbBuilder.Address = acct.BillingStreet
                        dbBuilder.City = acct.BillingCity
                        dbBuilder.CompanyName = acct.Name
                        dbBuilder.CRMID = acct.Id
                        dbBuilder.Fax = acct.Fax
                        Dim dbLLC As LLCRow = LLCRow.GetRowByCity(DB, acct.LLC_City__c)
                        dbBuilder.LLCID = dbLLC.LLCID
                        dbBuilder.Phone = acct.Phone
                        dbBuilder.State = acct.BillingState
                        dbBuilder.WebsiteURL = acct.Website
                        dbBuilder.Zip = acct.BillingPostalCode

                        If dbBuilder.BuilderID = Nothing Then
                            dbBuilder.Insert()
                        Else
                            dbBuilder.Update()
                        End If

                        Dim dbRegistration As BuilderRegistrationRow = BuilderRegistrationRow.GetRowByBuilder(DB, dbBuilder.BuilderID)
                        dbRegistration.Affiliations = acct.Company_Memberships_or_Affiliations__c
                        dbRegistration.AvgCostPerSqFt = acct.Avg_sqft__c
                        dbRegistration.Awards = acct.Industry_related_awards_received__c
                        dbRegistration.BuilderID = dbBuilder.BuilderID
                        dbRegistration.Employees = acct.of_employees__c
                        dbRegistration.HomesBuiltAndDelivered = acct.of_homes_built_since_in_business__c
                        dbRegistration.HomeStartsLastYear = acct.of_homes_built_last_year__c
                        dbRegistration.HomeStartsNextYear = acct.of_homes_projected_this_year__c
                        dbRegistration.RevenueLastYear = acct.Company_revenue_last_year__c
                        dbRegistration.RevenueNextYear = acct.Projected_company_revenue_this_year__c
                        dbRegistration.TotalCOGS = acct.Total_COGS_2006__c
                        dbRegistration.WhereYouBuild = acct.Areas_Cities_Counties_in_Which_y__c
                        dbRegistration.YearStarted = acct.Year_Business_was_started__c

                        If dbRegistration.BuilderRegistrationID = Nothing Then
                            dbRegistration.Insert()
                        Else
                            dbRegistration.Update()
                        End If

                        DB.CommitTransaction()

                    Catch ex As Exception
                        If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                        Logger.Error(Logger.GetErrorMessage(ex))
                    End Try
                Case "vendor"
                    Dim dbVendor As VendorRow = VendorRow.GetRowByCRMID(DB, acct.Id)
                    If dbVendor.VendorID <> Nothing Then
                        Continue For
                    End If

                    Try
                        DB.BeginTransaction()

                        dbVendor.AcceptedCards = acct.If_yes_which_credit_cards__c
                        dbVendor.Address = acct.BillingStreet
                        dbVendor.BillingAddress = acct.ShippingStreet
                        dbVendor.BillingCity = acct.ShippingCity
                        dbVendor.BillingState = acct.ShippingState
                        dbVendor.BillingZip = acct.ShippingPostalCode
                        dbVendor.City = acct.BillingCity
                        dbVendor.CompanyName = acct.Name
                        dbVendor.CRMID = acct.Id
                        dbVendor.Fax = acct.Fax
                        dbVendor.InsertToLLC(LLCRow.GetRowByCity(DB, acct.LLC_City__c).LLCID)
                        dbVendor.PaymentTerms = acct.Payment_Terms_for_CBUSA_Members__c
                        dbVendor.Phone = acct.Phone
                        dbVendor.ServicesOffered = acct.Services_Offered__c
                        dbVendor.SpecialtyServices = acct.Products_Offered__c
                        dbVendor.State = acct.BillingState
                        dbVendor.WebsiteURL = acct.Website
                        dbVendor.Zip = acct.BillingPostalCode

                        If dbVendor.VendorID = Nothing Then
                            dbVendor.Insert()
                        Else
                            dbVendor.Update()
                        End If

                        Dim dbRegistration As VendorRegistrationRow = VendorRegistrationRow.GetRowByVendor(DB, dbVendor.VendorID)
                        dbRegistration.BusinessType = acct.Business_Type__c
                        dbRegistration.CompanyMemberships = acct.Company_Memberships_or_Affiliations__c
                        dbRegistration.Employees = acct.of_employees__c
                        dbRegistration.IsSubsidiary = acct.Subsidiary__c
                        dbRegistration.ProductsOffered = acct.Products_Offered__c
                        dbRegistration.SubsidiaryExplanation = acct.Subsidiary_Explain__c
                        dbRegistration.SupplyArea = acct.Areas_Cities_Counties_in_Which_y__c
                        dbRegistration.VendorID = dbVendor.VendorID
                        dbRegistration.YearStarted = acct.Year_Business_was_started__c

                        If dbRegistration.VendorRegistrationID = Nothing Then
                            dbRegistration.Insert()
                        Else
                            dbRegistration.Update()
                        End If


                        DB.CommitTransaction()
                    Catch ex As Exception
                        If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                        Logger.Error(Logger.GetErrorMessage(ex))
                    End Try
                Case "piq"
                    'no piq import details from the client

            End Select
        Next

    End Sub
End Class
