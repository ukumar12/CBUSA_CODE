Imports Components

Partial Class MenuPage
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If TypeOf Context.User Is AdminPrincipal Then
            FullName.Text = CType(Context.User.Identity, AdminIdentity).FirstName + " " + CType(Context.User.Identity, AdminIdentity).LastName
        End If

        Dim bLast As Boolean = False

        Dim m As AdminMenu = New AdminMenu

        If HasRights("AUTOMATIC_MESSAGES") Or HasRights("CALLOUTS") Or HasRights("MESSAGES") Or HasRights("DOCUMENTS") Then

            m.WriteRoot("Communications")

            If HasRights("AUTOMATIC_MESSAGES") 
                If HasRights("CALLOUTS") And HasRights("MESSAGES") And HasRights("DOCUMENTS") = False Then bLast = True
                m.WriteLeaf("/admin/automaticmessages/", "Automatic Messages", bLast)
                bLast = False
            End If

            If HasRights("MESSAGES") Then
                If HasRights("DOCUMENTS") And HasRights("CALLOUTS") = False Then bLast = True
                m.WriteLeaf("/admin/message/", "Dashboard Messages", bLast)
                bLast = False
            End If

            If HasRights("DOCUMENTS") Then
                If HasRights("CALLOUTS") = False Then bLast = True
                m.WriteLeaf("/admin/document/", "Documents", bLast)
                bLast = False
            End If

            If HasRights("CALLOUTS") Then m.WriteLeaf("/admin/callout/", "Callouts", True)

        End If

        If HasRights("BUILDERS") Then
            m.WriteRoot("Builders")
            m.WriteLeaf("/admin/builders/", "Builders", False)
            If HasRights("BUILDER_ACCOUNTS") Then m.WriteLeaf("/admin/builderaccount/", "Builder Accounts", False)
            m.WriteLeaf("/admin/BuilderRole/", "Builder Roles", False)
            m.WriteLeaf("/admin/builders/builderregistration/", "Builder Registrations", False)
            m.WriteLeaf("/admin/builderbiddata/", "Builder Bid Data", False)
            m.WriteLeaf("/admin/buildermonthlydata/", "Builder Monthly Data", False)
            m.WriteLeaf("/admin/statements/", "Statements", False)

            m.WriteLeaf("/admin/builders/RebatesReport", "Builder Rebate Emails", False)
            m.WriteLeaf("/admin/Builderreports/NonReportedBuilders.aspx", "Pending Builder Purchase Reports", False)
            m.WriteLeaf("/admin/Builderreports/APVQuarterlyReports.aspx", "APV Quarterly Report", False)
            m.WriteLeaf("/admin/builders/PerformanceSurvey/", "Performance Survey", True)
        End If

        If HasRights("VENDORS") Then

            m.WriteRoot("Vendors")
            m.WriteLeaf("/admin/vendor/", "Vendors", False)





            If HasRights("VENDOR_ACCOUNTS") Then
                If HasRights("VENDOR_REGISTRATIONS") And HasRights("VENDOR_CATEGORYS") And HasRights("VENDOR_ROLES") = False Then bLast = True
                m.WriteLeaf("/admin/vendoraccount/", "Vendor Accounts", bLast)
                bLast = False
            End If

            If HasRights("VENDOR_REGISTRATIONS") Then
                If HasRights("VENDOR_CATEGORYS") And HasRights("VENDOR_ROLES") = False Then bLast = True
                m.WriteLeaf("/admin/vendorregistrations/", "Vendor Registrations", bLast)
                bLast = False
            End If

            If HasRights("VENDOR_CATEGORYS") Then
                If HasRights("VENDOR_COMMENTS") = False Then bLast = True
                m.WriteLeaf("/admin/vendorcategory/", "Vendor Categories", bLast)
                bLast = False
            End If

            If HasRights("VENDOR_COMMENTS") Then
                If HasRights("VENDOR_ROLES") = False Then bLast = True
                m.WriteLeaf("/admin/vendorcomment/", "Vendor Comments", bLast)
                bLast = False
            End If
            'If HasRights("VENDORS") Then
            '    If HasRights("VENDORS") = False Then bLast = True
            '    m.WriteLeaf("/admin/vendor/flaggedvendors/", "Flagged Vendors", bLast)
            '    bLast = False
            'End If
            If HasRights("VENDORS") Then
                m.WriteLeaf("/admin/VendorBidData/", "Vendor Bid Data", False)
            End If
            m.WriteLeaf("/admin/Vendorreports/", "Vendor Reports", False)
           
            m.WriteLeaf("/admin/VendorPaymentTermsReport/", "Payment Terms Report", False)
            m.WriteLeaf("/admin/Vendorreports/NonReportedVendors.aspx", "Pending Vendor Sales Reports", False)


            If HasRights("VENDOR_ROLES") Then m.WriteLeaf("/admin/vendorroles/", "Vendor Roles", False)

            If HasRights("VENDOR_ACCOUNTS") Then m.WriteLeaf("/admin/rebateterm/", "Rebate Terms", False)
            m.WriteLeaf("/admin/vendor/photos/", "Photos", False)
            If HasRights("VENDORS") Then
                m.WriteLeaf("/admin/Vendorreports/PendingPriceRequests.aspx", "Pending PriceRequests", True)
            End If

        End If
        If HasRights("BUILDERS") Then
            m.WriteRoot("Manufacturers")
            m.WriteLeaf("/admin/ContractManufacturer/", "Add Manufacturers", True)

        End If

        If HasRights("REPORTS") Then
            m.WriteRoot("Reports")
            m.WriteLeaf("/admin/statements/RebateStatements.aspx", "Rebate Documents Archive", False)
            m.WriteLeaf("/admin/statements/NCPRebateStatements.aspx", "NCP Rebate Documents Archive", False)
            m.WriteLeaf("/admin/apv/APVReportWithDNR.aspx", "APV Report", False)
            m.WriteLeaf("/admin/apv/QtrComparisionByVendor.aspx", "QTR Comparision(By Vendor)", False)
            m.WriteLeaf("/admin/apv/QtrComparisionReport.aspx", "QTR Comparision(By Market)", True)
        End If
       

        'If HasRights("VENDORS") Then
        '    m.WriteRoot("Vendors")
        '    m.WriteLeaf("/admin/vendor/", "Vendors", False)
        '    m.WriteLeaf("/admin/vendor/edit.aspx", "Add New Vendor", True)
        'End If

        'If HasRights("VENDOR_ACCOUNTS") Then
        '    m.WriteRoot("Vendor Accounts")
        '    m.WriteLeaf("/admin/vendoraccount/", "Vendor Accounts", False)
        '    m.WriteLeaf("/admin/vendoraccount/edit.aspx", "Add New Vendor Account", True)
        'End If

        'If HasRights("VENDOR_REGISTRATIONS") Then
        '    m.WriteRoot("Vendor Registrations")
        '    m.WriteLeaf("/admin/vendorregistrations/", "Vendor Registrations", False)
        '    m.WriteLeaf("/admin/vendorregistrations/edit.aspx", "Add New Vendor Registration", True)
        'End If

        If HasRights("PIQ") Then
            m.WriteRoot("PIQ's")
            m.WriteLeaf("/admin/piq/default.aspx", "PIQ's", True)
        End If

        If HasRights("PRICE_COMPARISONS") Then
            m.WriteRoot("Price Comparisons")
            m.WriteLeaf("/admin/pricecomparison/", "Settings", True)
        End If

        If HasRights("MARKET_INDICATORS") Then
            m.WriteRoot("Market Indicators")
            m.WriteLeaf("/admin/marketindicator/", "Settings", False)
            m.WriteLeaf("/admin/marketindicator/info/", "Builder Info", True)
        End If

        If HasRights("TWO_PRICE_CAMPAIGNS") Then
            m.WriteRoot("Two Price")
            m.WriteLeaf("/admin/twoprice/campaigns/default.aspx", "Committed Purchase Event Management", False)
            m.WriteLeaf("/admin/twoprice/status/default.aspx", "Status", True)
        End If

        If HasRights("LLCs") Then
            m.WriteRoot("LLCs")
            m.WriteLeaf("/admin/llc/", "LLCs", False)
            m.WriteLeaf("/admin/llc/edit.aspx", "Add New LLC", True)
        End If

        If HasRights("BILLING_PLANS") Then
            m.WriteRoot("Billing Plans")
            m.WriteLeaf("/admin/billingplan/default.aspx", "Billing Plans", False)
            m.WriteLeaf("/admin/billingplan/edit.aspx", "Add New Billing Plan", True)
        End If

        If HasRights("BUILDERS") Then
            m.WriteRoot("TakeOff Service")
            m.WriteLeaf("/admin/TakeOffService/", "TakeOff Service", True)
        End If

        If HasRights("BUSINESS_TYPES") Then
            m.WriteRoot("Business Types")
            m.WriteLeaf("/admin/businesstype/", "Business Types", False)
            m.WriteLeaf("/admin/businesstype/edit.aspx", "Add New Business Type", True)
        End If

        If HasRights("PRODUCTS") Then
            m.WriteRoot("Products")
            m.WriteLeaf("/admin/products/", "Products", False)
            m.WriteLeaf("/admin/products/VendorProductPricing/", "Vendor Product Pricing", False)
            m.WriteLeaf("/admin/products/productimport.aspx", "Import Products", False)
            m.WriteLeaf("/admin/products/edit.aspx", "Add New Product", True)
        End If

        If HasRights("PRODUCT_TYPES") Then
            m.WriteRoot("Product Types")
            m.WriteLeaf("/admin/products/types/", "Product Types", False)
            m.WriteLeaf("/admin/products/types/edit.aspx", "Add Product Type", True)
        End If

        If HasRights("SUPPLY_PHASES") Then
            m.WriteRoot("Supply Phases")
            m.WriteLeaf("/admin/supplyphase/", "Supply Phases", False)
            m.WriteLeaf("/admin/supplyphase/edit.aspx", "Add Supply Phase", True)
        End If

        If HasRights("SUPPLY_PHASE_CATEGORYS") Then
            m.WriteRoot("Phases of Supply")
            m.WriteLeaf("/admin/supplyphasecategory/", "Phases of Supply", False)
            m.WriteLeaf("/admin/supplyphasecategory/edit.aspx", "Add New Phase of Supply", True)
        End If

        If HasRights("SAMPLE_MONTHLY_STATS") Then
            m.WriteRoot("Sample Charts")
            m.WriteLeaf("/admin/samplemonthlystat/", "Sample Data", False)
            m.WriteLeaf("/admin/samplemonthlystat/edit.aspx", "Add Sample Data", True)
        End If

        If HasRights("EAGLE_ONE") Then
            m.WriteRoot("Eagle One Comparisons")
            m.WriteLeaf("/admin/eagleone/", "Eagle One Comparisons", False)
            m.WriteLeaf("/admin/eagleone/edit.aspx", "Add Eagle One Comparison", True)
        End If

        If HasRights("RATING_CATEGORIES") Then

            m.WriteRoot("Vendor Rating Categories")
            m.WriteLeaf("/admin/ratingcommentsdata/", "Rating And Comments Data", False)
            m.WriteLeaf("/admin/ratingcategory/", "Rating Categories", False)
            m.WriteLeaf("/admin/ratingcategory/edit.aspx", "Add Rating Category", True)
        End If

        If HasRights("SURVEY") Then
            m.WriteEmptyRoot("/admin/surveys/", "Surveys")
        End If

        If HasRights("VINDICIA_SOAP_LOGS") Then
            m.WriteEmptyRoot("/admin/vindiciasoaplog/", "Vindicia SOAP Logs")
        End If
        m.WriteEmptyRoot("/admin/salesforceapilog/default.aspx", "SalesForce Logs")
        If HasRights("TASK_LOGS") Then
            m.WriteEmptyRoot("/admin/tasklog/", "Task Logs")
        End If

        If HasRights("BUILDERS") Then
            m.WriteEmptyRoot("/admin/APV/", "APV")
        End If

        m.WriteRoot("National Contracts")
        m.WriteLeaf("/admin/nationalcontracts/contractcentral", "Contract Central", False)
        m.WriteLeaf("/admin/nationalcontracts/", "NCP Dashboard", False)
        m.WriteLeaf("/admin/nationalcontracts/NCPContent/", "Manage NCP Content", False)
        m.WriteLeaf("/admin/nationalcontracts/ContentItems/", "Upload Images/Docs", True)


        m.WriteHTML("<p></p>")
        If HasRights("CONTENT_TOOL") Then
            m.WriteRoot("Content Tool")
            m.WriteLeaf("/admin/content/pages", "Pages", False)
            m.WriteLeaf("/admin/content/sections", "Sections", False)
            m.WriteLeaf("/admin/content/navigation", "Navigation", False)
            If LoggedInIsInternal Then
                m.WriteLeaf("/admin/content/modules", "Modules (AE)", False)
                m.WriteLeaf("/admin/help", "Help Messages(AE)", False)
            End If
            m.WriteLeaf("/admin/content/templates", "Templates", True)
        End If

        m.WriteHTML("<p></p>")
        If HasRights("USERS") Then
            m.WriteRoot("Admin Users")
            m.WriteLeaf("/admin/admins/", "Admin Users", False)
            m.WriteLeaf("/admin/admins/edit.aspx", "Add New Admin", False)
            m.WriteLeaf("/admin/admins/groups/", "Admin Groups", False)
            m.WriteLeaf("/admin/main.aspx", "Last login activity", True)
        End If

        m.WriteHTML("<p></p>")
        If HasRights("USERS") Then
            m.WriteEmptyRoot("/admin/settings/", "System Parameters")
        End If

        m.WriteHTML("<p></p>")
        If "admin" <> LoggedInUsername Then
            m.WriteEmptyRoot("/admin/password/", "Change Password")
        End If
        m.WriteEmptyRoot("/admin/logout.aspx", "Logout")

        'If HasRights("ORDERS") Then
        '    m.WriteRoot("Orders")
        '    m.WriteLeaf("/admin/store/orders/", "View Orders", False)
        '    m.WriteLeaf("/admin/store/payments/", "Payment Log", False)
        '    m.WriteLeaf("/admin/store/text/edit.aspx?Code=PackingListFooter", "Packing List Footer Text", False)
        '    m.WriteLeaf("/admin/store/text/edit.aspx?Code=OrderConfirmation", "Order Confirmation Text", True)
        'End If

        ''If HasRights("REPORTS") Then
        ''    m.WriteRoot("Reports")
        ''    m.WriteLeaf("/admin/store/sales/", "Sales Report", False)
        ''    m.WriteLeaf("/admin/store/promotions/promotionreports.aspx", "Promotion Code Report", False)            
        ''    m.WriteLeaf("/admin/store/search/", "Search Term Report", True)
        ''End If

        'If HasRights("MEMBERS") Then
        '    m.WriteRoot("Members")
        '    m.WriteLeaf("/admin/members/", "Members", False)
        '    m.WriteLeaf("/admin/members/add.aspx", "Add New Member", True)
        'End If

        'If HasRights("BANNERS") Then
        '    m.WriteRoot("Banners")
        '    m.WriteLeaf("/admin/banners/", "Banners", False)
        '    m.WriteLeaf("/admin/banners/edit.aspx", "Add New Banner", False)
        '    m.WriteLeaf("/admin/banners/report.aspx", "Summary Report", False)
        '    m.WriteLeaf("/admin/banners/daily.aspx", "Daily Report", False)
        '    m.WriteEmptyBranch("Banner Groups")
        '    m.WriteLeaf("/admin/banners/groups", "Banner Groups", False)
        '    m.WriteLeaf("/admin/banners/groups/edit.aspx", "Add New Group", True)
        'End If

        'If HasRights("MARKETING_TOOLS") Then
        '    m.WriteRoot("Marketing Tools")
        '    m.WriteLeaf("/admin/store/promotions/", "Promotions", False)
        '    m.WriteLeaf("/admin/store/referrals/", "Referrals", False)
        '    m.WriteLeaf("/admin/store/giftmessage/", "Gift Messages", False)
        '    m.WriteLeaf("/admin/store/howheard/", "Discovery Sources", False)
        '    m.WriteLeaf("/admin/store/features/", "Item Features", False)
        '    m.WriteLeaf("/admin/content/edit.aspx?PageURL=" & Server.UrlEncode("/404.aspx"), "Customize 404 Page", True)
        'End If

        'If HasRights("BROADCAST") Then
        '    m.WriteRoot("Broadcast")
        '    If LoggedInIsInternal Then m.WriteLeaf("/admin/broadcast/templates/", "Templates (AE)", False)
        '    m.WriteLeaf("/admin/broadcast/lists/", "Lists", False)
        '    m.WriteLeaf("/admin/broadcast/subscribers/", "Subscribers", False)
        '    m.WriteLeaf("/admin/broadcast/groups/", "Recipient Groups", False)
        '    m.WriteLeaf("/admin/broadcast/", "E-mails", True)
        'End If

        'If HasRights("STORE") Then
        '    m.WriteRoot("Store")
        '    If LoggedInIsInternal Then m.WriteLeaf("/admin/store/template/", "Item Templates (AE)", False)
        '    m.WriteLeaf("/admin/store/departments/", "Departments", False)
        '    m.WriteLeaf("/admin/store/brands/", "Brands", False)
        '    m.WriteLeaf("/admin/store/items/", "Items", False)
        '    m.WriteLeaf("/admin/store/search/exclude/", "Excluded Search Words", False)
        '    m.WriteLeaf("/admin/store/swatch/", "Swatches", True)
        'End If

        'If HasRights("SHIPPING_TAX") Then
        '	m.WriteRoot("Shipping & Tax")
        '	m.WriteLeaf("/admin/store/shipping/shippingparam.aspx", "Shipping Calculation", False)
        '	m.WriteLeaf("/admin/store/shipping/", "Shipping Methods", False)
        '	m.WriteLeaf("/admin/store/countries/", "Country Shipping", False)
        '	m.WriteLeaf("/admin/store/states/", "State Tax", True)
        'End If

        'If HasRights("CONTACT_US") Then
        '	m.WriteRoot("Contact Us")
        '	m.WriteLeaf("/admin/contactus/default.aspx", "Contact Us Submissions", False)
        '	m.WriteLeaf("/admin/contactus/question/default.aspx", "Contact Us Questions", False)
        '	m.WriteLeaf("/admin/content/edit.aspx?PageId=85", "Edit Thank You Page", True)
        'End If

        'If HasRights("FAQ") Then
        '	m.WriteRoot("FAQ's")
        '	m.WriteLeaf("/admin/faq/default.aspx", "FAQ's", False)
        '	m.WriteLeaf("/admin/faq/category/default.aspx", "FAQ Categories", True)
        'End If

        lblMenu.Text = m.Menu
    End Sub

End Class
