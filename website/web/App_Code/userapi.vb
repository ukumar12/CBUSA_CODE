Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.Web.Script.Services
Imports System.ComponentModel
Imports System.Security.Cryptography
Imports Utility
' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="https://app.custombuilders.com/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
<ToolboxItem(False)> _
Public Class userapi
    Inherits System.Web.Services.WebService
#Region "Global Objects"
    Private m_DB As Database
    Public ServiceAlg As String = "HmacSHA256"
    Public ServiceSalt As String = "ameagle"
    Private HashString As String
    Dim SecretToken As String = "changeme"
    Private ReadOnly Property DB() As Database
        Get
            If m_DB Is Nothing Then
                m_DB = New Database
                m_DB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))
            End If
            Return m_DB
        End Get
    End Property

    Private Sub api_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        If Not DB Is Nothing Then DB.Dispose()
    End Sub



    Public Function GetHashedToken(ByVal Username As String, ByVal Password As String) As String
        Dim hash As String = String.Join(":", New String() {Username, SecretToken})
        Dim hashLeft As String = ""
        Dim hashRight As String = ""

        Using hmac As System.Security.Cryptography.HMAC = HMACSHA256.Create(ServiceAlg)
            hmac.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(GetHashedPassword(Password))
            hmac.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(hash))
            hashLeft = Convert.ToBase64String(hmac.Hash)
            hashRight = String.Join(":", New String() {Username, Password})
        End Using
        Return Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Join(":", New String() {hashLeft, hashRight})))
    End Function


    Public Function GetHashedPassword(password As String) As String
        'Dim ServiceAlg1 As String = "HmacSHA256"
        'Dim ServiceSalt1 As String = "ameagle"
        Dim key As String = String.Join(":", New String() {password, ServiceSalt})

        Using hmac As HMAC = HMACSHA256.Create(ServiceAlg)
            ' Hash the key.
            hmac.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(ServiceSalt)
            hmac.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(key))
            Return Convert.ToBase64String(hmac.Hash)
        End Using
    End Function


    Private Const expirationMinutes As Integer = 180

    Public Function IsTokenValid(ByVal token As String, ByRef UserID As String) As Boolean
        Dim result As Boolean = False
        Dim errori As New ErrorResponse
        Try
            ' Base64 decode the string, obtaining the token:username:timeStamp.
            Dim key As String = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(token))
            Dim IsValidUser As Boolean = False
            ' Split the parts.
            Dim parts As String() = key.Split(New Char() {":"c})
            If parts.Length = 3 Then
                ' Get the hash message, username, and timestamp.
                Dim hash As String = parts(0)
                Dim username As String = parts(1)
                ' Lookup the user's account from the db.
                Dim password As String = parts(2)


                Return EnsureUserNamePassword(username, password, errori)


            End If
        Catch
        End Try

        Return result
    End Function






#End Region
    <WebMethod(Description:="GetAuthenticated ")> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function GetAuthenticated(ByVal SecretKey As String, ByVal Username As String, ByVal Password As String) As String
        'If AuthToken = String.Empty OrElse AuthToken.Trim = String.Empty OrElse Not AuthToken = AppSettings("WebServiceAuthToken") Then
        '    Return String.Empty
        'End If
        'Dim ServiceAlg As String = "HmacSHA256"
        'Dim ServiceSalt As String = "ameagle"
        Dim errori As New ErrorResponse
        Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()
        If Not EnsureAuthToken(SecretKey, errori) Then
            Return ser.Serialize(errori).ToString
        End If


        If Not EnsureUserNamePassword(Username, Password, errori) Then
            Return ser.Serialize(errori).ToString
        End If


        Dim ari As New AuthenticationResponse
            ari.Status = "Success"
            ari.token = GetHashedToken(Username, Password)
            Return ser.Serialize(ari).ToString






        'Dim hash As String = String.Join(":", New String() {Username, SecretKey, ValidUserDetails})
        'Dim hashLeft As String = ""
        'Dim hashRight As String = ""

        'Using hmac As System.Security.Cryptography.HMAC = HMACSHA256.Create(ServiceAlg)
        '    hmac.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(GetHashedPassword(Password))
        '    hmac.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(hash))
        '    hashLeft = Convert.ToBase64String(hmac.Hash)
        '    hashRight = String.Join(":", New String() {Username, ValidUserDetails, Now.Ticks})
        'End Using


    End Function
    <WebMethod(Description:="GetUsers- Gets Builder Account Details from Token")> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function GetUsers(ByVal SecretKey As String, ByVal AuthToken As String) As String
        Dim ri As New ResponseInfo
        Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()
        If Not EnsureAuthToken(SecretKey, ri) Then
            Return ser.Serialize(ri).ToString
        End If

        Dim UserID As String = String.Empty

        Dim strResponse As String = String.Empty

        Dim ari As New AuthenticationResponse
        If IsTokenValid(AuthToken, UserID) Then
            ' UserID = UserID.Split("_")(1)
            ser.MaxJsonLength = Int32.MaxValue
            Dim BuilderAccount As New BuilderAccount

            ' Dim dt As DataTable = DB.GetDataTable("Select * from BuilderAccount Where BuilderAccountID = " & DB.Number(UserID))
            Dim dt As DataTable = DB.GetDataTable("Select * from BuilderAccount Where Isactive = 1")
            Dim BuilderAccountDetailslist As Generic.List(Of BuilderAccountDetails) = New Generic.List(Of BuilderAccountDetails)
            For Each row In dt.Rows
                Dim BuilderAccountDetails As New BuilderAccountDetails()
                BuilderAccountDetails.AEUSERID = row("BuilderAccountID")
                BuilderAccountDetails.AEBuilderID = row("BuilderID")
                BuilderAccountDetails.FirstName = IIf(IsDBNull(row("FirstName")), "", row("FirstName")) 'row("FirstName")
                BuilderAccountDetails.LastName = IIf(IsDBNull(row("LastName")), "", row("LastName")) 'row("LastName")
                BuilderAccountDetails.MiddleName = String.Empty
                BuilderAccountDetails.UserName = IIf(IsDBNull(row("UserName")), "", row("UserName"))
                BuilderAccountDetails.Email = IIf(IsDBNull(row("Email")), "", row("Email"))

                BuilderAccountDetailslist.Add(BuilderAccountDetails)
            Next
            BuilderAccount.BuilderAccountDetails = BuilderAccountDetailslist

            strResponse = ser.Serialize(BuilderAccount)

            Return strResponse


        Else
            Dim errori As New ErrorResponse
            errori.Errordetails = "Invalid token."
            Return ser.Serialize(errori).ToString
        End If

        Return ""
    End Function
    <WebMethod(Description:="Gets All Product Categories")> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function GetProductCategories(ByVal SecretKey As String, ByVal UserAuthenticatedToken As String) As String
        Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()
         Dim errori As New ErrorResponse

        If Not EnsureAuthToken(SecretKey, errori) Then
            Return ser.Serialize(errori).ToString
        End If
        Dim UserID As String = String.Empty
        If Not IsTokenValid(UserAuthenticatedToken, UserID) Then
            errori.Errordetails = "Invalid token."
            Return ser.Serialize(errori).ToString
        End If
        Dim ProductCategories As New ProductCategories
        Dim dt As DataTable = DB.GetDataTable("Select  * from SupplyPhase")
        Dim ProductCategoriesList As Generic.List(Of ProductCategoryDetails) = New Generic.List(Of ProductCategoryDetails)
        For Each row In dt.Rows
            Dim ProductCategoryDetails As New ProductCategoryDetails()
            ProductCategoryDetails.ProductCategoryName = IIf(IsDBNull(row("SupplyPhase")), "", row("SupplyPhase"))
            ProductCategoryDetails.AEProductCategoryID = IIf(IsDBNull(row("SupplyPhaseID")), "", row("SupplyPhaseID")) ' row("SupplyPhaseID")
            ProductCategoryDetails.ProductCategoryParentID = IIf(IsDBNull(row("ParentSupplyPhaseID")), "", row("ParentSupplyPhaseID")) ' row("ParentSupplyPhaseID")
            ProductCategoriesList.Add(ProductCategoryDetails)
        Next
        ProductCategories.ProductCategories = ProductCategoriesList
        ' ProductCategories.Status = Status.Success
        'Dim strResponse As String = ser.Serialize(ProductCategories)

        Return ser.Serialize(ProductCategories)
    End Function

    <WebMethod(Description:="Gets All manufacturers")> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function GetAllManufacturers(ByVal SecretKey As String) As String
        Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()
        Dim errori As New ErrorResponse

        If Not EnsureAuthToken(SecretKey, errori) Then
            Return ser.Serialize(errori).ToString
        End If

        Dim dt As DataTable = DB.GetDataTable("Select  * from NCPManufacturer")
        Dim Manufacturers As New Manufacturers
        Dim ManufacturersDetailsList As Generic.List(Of ManufacturersDetails) = New Generic.List(Of ManufacturersDetails)
        For Each row In dt.Rows
            Dim ManufacturersDetails As New ManufacturersDetails()
            ManufacturersDetails.AEManufacturerID = row("NCPManufacturerID")
            ManufacturersDetails.HistoricID = row("HistoricID")
            ManufacturersDetails.ClassID = row("ClassID")
            ManufacturersDetails.CompanyName = IIf(IsDBNull(row("CompanyName")), "", row("CompanyName"))
            ManufacturersDetails.MailingAddress = IIf(IsDBNull(row("MailingAddress")), "", row("MailingAddress"))
            ManufacturersDetails.MailingCity = IIf(IsDBNull(row("MailingCity")), "", row("MailingCity"))
            ManufacturersDetails.MailingState = IIf(IsDBNull(row("MailingState")), "", row("MailingState"))
            ManufacturersDetails.MailingZip = IIf(IsDBNull(row("MailingZip")), "", row("MailingZip"))
            ManufacturersDetails.Website = IIf(IsDBNull(row("Website")), "", row("Website"))
            ManufacturersDetails.PrimaryContactName = IIf(IsDBNull(row("PrimaryContactName")), "", row("PrimaryContactName"))
            ManufacturersDetails.PrimaryContactEmail = IIf(IsDBNull(row("PrimaryContactEmail")), "", row("PrimaryContactEmail"))
            ManufacturersDetails.PrimaryContactPhone = IIf(IsDBNull(row("PrimaryContactPhone")), "", row("PrimaryContactPhone"))
            ManufacturersDetails.APContactName = IIf(IsDBNull(row("APContactName")), "", row("APContactName"))
            ManufacturersDetails.APContactEmail = IIf(IsDBNull(row("APContactEmail")), "", row("APContactEmail"))
            ManufacturersDetails.APContactPhone = IIf(IsDBNull(row("APContactPhone")), "", row("APContactPhone"))
            ManufacturersDetails.PaymentTerms = IIf(IsDBNull(row("PaymentTerms")), "", row("PaymentTerms"))
            ManufacturersDetailsList.Add(ManufacturersDetails)
        Next
        Manufacturers.ManufacturersDetails = ManufacturersDetailsList
        Dim strResponse As String = ser.Serialize(Manufacturers)




        ' ProductCategories.Status = Status.Success
        'Dim strResponse As String = ser.Serialize(ProductCategories)

        Return strResponse
    End Function

    <WebMethod(Description:="Gets All Active Products ")> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function GetProducts(ByVal SecretKey As String, ByVal UserAuthenticatedToken As String) As String
        Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()
        ser.MaxJsonLength = Int32.MaxValue
        Dim errori As New ErrorResponse

        If Not EnsureAuthToken(SecretKey, errori) Then
            Return ser.Serialize(errori).ToString
        End If
        Dim UserID As String = String.Empty
        If Not IsTokenValid(UserAuthenticatedToken, UserID) Then
            errori.Errordetails = "Invalid token."
            Return ser.Serialize(errori).ToString
        End If
        Dim Product As New Product

        Dim dt As DataTable = DB.GetDataTable("Select   * from Product Where IsActive = 1   and ProductID in (select productID from SupplyPhaseProduct)")
        Dim dtSupplyPhase As DataTable = DB.GetDataTable("select  spp.SupplyPhaseID , sp.SupplyPhase, spp.productID  from SupplyPhaseProduct spp inner join supplyphase sp on sp.SupplyPhaseID = spp.SupplyPhaseID")

        Dim ProductDetailsList As Generic.List(Of ProductDetails) = New Generic.List(Of ProductDetails)
        
        For Each row In dt.Rows
            Dim ProductDetails As New ProductDetails()

            Dim ProductProductCategoryList As Generic.List(Of ProductProductCategory) = New Generic.List(Of ProductProductCategory)

            ProductDetails.AEProductID = GetString(row("ProductID"))
            ProductDetails.ProductName = GetString(row("Product"))


            Dim dtProductCategory As DataRow() = dtSupplyPhase.Select("ProductID=" & DB.Number(row("ProductID")))
            For Each drrow As DataRow In dtProductCategory
                Dim ProductProductCategoryDetails As New ProductProductCategory()
                ProductProductCategoryDetails.AEProductCategoryID = GetString(drrow("SupplyPhaseID"))
                ProductProductCategoryDetails.ProductCategoryName = GetString(drrow("SupplyPhase"))

                ProductProductCategoryList.Add(ProductProductCategoryDetails)
            Next

            ProductDetails.ProductProductCategory = ProductProductCategoryList

            ProductDetailsList.Add(ProductDetails)
            dtSupplyPhase.RejectChanges()

        Next
        Product.ProductDetails = ProductDetailsList
        ' Product.Status = Status.Success
        'Dim strResponse As String = ser.Serialize(Product)
        'Context.Response.Clear()
        'Context.Response.ContentType = "application/json"
        'Context.Response.AddHeader("content-length", strResponse.Length.ToString())
        'Context.Response.Write(strResponse)
        Return ser.Serialize(Product)

    End Function

    <WebMethod(Description:="Gets Market/LLC details(associated with Builders) from UserToken ")> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function GetMarkets(ByVal SecretKey As String, ByVal UserAuthenticatedToken As String) As String
        Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()
        ser.MaxJsonLength = Int32.MaxValue
        Dim LLC As New LLC
        Dim errori As New ErrorResponse

        If Not EnsureAuthToken(SecretKey, errori) Then
            Return ser.Serialize(errori).ToString
        End If
        Dim UserID As String = String.Empty
        If Not IsTokenValid(UserAuthenticatedToken, UserID) Then
            errori.Errordetails = "Invalid token."
            Return ser.Serialize(errori).ToString
        End If

        '  Dim BuilderID As Integer = Core.GetInt(DB.ExecuteScalar("Select BuilderID From BuilderAccount where BuilderAccountID = " & DB.Number(UserID.Split("_")(1))))

        Dim dt As DataTable = DB.GetDataTable("select * from LLC where IsActive = 1 ")

        Dim LLCDetailslist As Generic.List(Of LLCDetails) = New Generic.List(Of LLCDetails)
        For Each row In dt.Rows
            Dim LLCDetails As New LLCDetails()
            LLCDetails.AELLCID = GetString(row("LLCID"))
            LLCDetails.LLCName = GetString(row("LLC"))
            LLCDetails.IsActive = GetString(row("IsActive"))
            LLCDetailslist.Add(LLCDetails)
        Next
        LLC.LLCDetails = LLCDetailslist

        Dim strResponse As String = ser.Serialize(LLC)
        'Context.Response.Clear()
        'Context.Response.ContentType = "application/json"
        'Context.Response.AddHeader("content-length", strResponse.Length.ToString())
        'Context.Response.Write(strResponse)
        Return strResponse

    End Function

    <WebMethod(Description:="Gets   Builders Details from Token ")> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function GetBuilders(ByVal SecretKey As String, ByVal UserAuthenticatedToken As String) As String
        Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()
        ser.MaxJsonLength = Int32.MaxValue

        Dim errori As New ErrorResponse

        If Not EnsureAuthToken(SecretKey, errori) Then
            Return ser.Serialize(errori).ToString
        End If
        Dim UserID As String = String.Empty
        If Not IsTokenValid(UserAuthenticatedToken, UserID) Then
            errori.Errordetails = "Invalid token."
            Return ser.Serialize(errori).ToString
        End If
        Dim Builders As New Builders

        'Dim BuilderID As Integer = Core.GetInt(DB.ExecuteScalar("Select BuilderID From BuilderAccount where BuilderAccountID = " & DB.Number(UserID.Split("_")(1))))

        Dim dt As DataTable = DB.GetDataTable("Select b.*,LLC from Builder b Left Join LLC on b.LLCID = LLC.LLCID Where b.Isactive =1 ")

        Dim BuildersDetailslist As Generic.List(Of BuildersDetails) = New Generic.List(Of BuildersDetails)
        For Each row In dt.Rows
            Dim BuildersDetails As New BuildersDetails()
            BuildersDetails.AEBuilderID = GetString(row("BuilderID"))
            BuildersDetails.AEBuilderHistoricID = GetString(row("HistoricID"))
            BuildersDetails.BuilderName = GetString(row("CompanyName"))
            BuildersDetails.PhoneNumber = GetString(row("Phone"))
            BuildersDetails.BuilderEmail = GetString(row("Email"))
            BuildersDetails.BuilderMarket = GetString(row("LLC"))
            BuildersDetailslist.Add(BuildersDetails)
        Next
        Builders.BuildersDetails = BuildersDetailslist

        Dim strResponse As String = ser.Serialize(Builders)

        Return strResponse

    End Function

    <WebMethod(Description:="Gets PIQ from Token")> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function getPIQs(ByVal SecretKey As String, ByVal UserAuthenticatedToken As String) As String
        Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()
        ser.MaxJsonLength = Int32.MaxValue
        Dim ri As New ResponseInfo
        If Not EnsureAuthToken(SecretKey, ri) Then
            Return ser.Serialize(ri).ToString
        End If
        Dim UserID As String = String.Empty
        If Not IsTokenValid(UserAuthenticatedToken, UserID) Then
            Dim errori As New ErrorResponse
            errori.Errordetails = "Invalid token."
            Return ser.Serialize(errori).ToString
        End If
        Dim PIq As New PIQ



        Dim dt As DataTable = DB.GetDataTable("Select TOp 1 * from PIQACCOUNT PA inner join PIQ on PIQ.PIQID= PA.PIQID Where PIQACCOUNTID = " & Me.DB.Number(UserID.Split("_")(1)))
        Dim dtVendors As DataTable = Me.DB.GetDataTable("Select v.VendorId, v.CompanyName from Vendor v, PIQPreferredVendor pv where v.VendorId = pv.PreferredVendorId And pv.PIQID = " & Me.DB.Number(UserID.Split("_")(1)) & " order by v.CompanyName")

        Dim preferredVendors As String = String.Empty
        Dim Conn As String = String.Empty
        For Each drVendors In dtVendors.Rows
            preferredVendors &= Conn & drVendors("CompanyName")
            Conn = ","
        Next

        '     Dim dt As DataTable = DB.GetDataTable("Select * from PIQACCOUNT PA inner join PIQ on PIQ.PIQID= PA.PIQID Where PIQACCOUNTID = " & DB.Number(UserID))



        Dim PIQDetailslist As Generic.List(Of PIQDetails) = New Generic.List(Of PIQDetails)
        For Each row In dt.Rows
            Dim PIQDetails As New PIQDetails()
            PIQDetails.AEPIQID = row("PIQACCOUNTID")
            PIQDetails.CompanyName = row("CompanyName")
            PIQDetails.Address = If(IsDBNull(row("Address")), "", row("Address")) & If(IsDBNull(row("Address2")), "", "," & row("Address2")) & If(IsDBNull(row("City")), "", "," & row("City")) & If(IsDBNull(row("State")), "", "," & row("State")) & If(IsDBNull(row("zip")), "", "," & row("zip"))
            PIQDetails.Phone = If(IsDBNull(row("Phone")), "", row("Phone"))
            PIQDetails.Website = If(IsDBNull(row("WebsiteURL")), "", row("WebsiteURL"))
            PIQDetails.PreferredVendors = preferredVendors
            PIQDetailslist.Add(PIQDetails)
        Next
        PIq.PIQDetails = PIQDetailslist
        Dim strResponse As String = ser.Serialize(PIq)
        Return strResponse

    End Function

#Region "Helper Functions and Subs"
    Private Function EnsureAuthToken(ByVal AuthToken As String, ByRef ErrorResponseObject As Object) As Boolean
        ' AuthToken = "cbusaapi306359"
        If AuthToken = String.Empty OrElse AuthToken.Trim = String.Empty OrElse Not AppSettings("WebServiceAuthToken") = AuthToken Then
            If TypeOf ErrorResponseObject Is ErrorResponse Then
                ErrorResponseObject = CType(ErrorResponseObject, ErrorResponse)
                ErrorResponseObject.Errordetails = GetErrorMessage(ErrorNumber.InvalidAuthToken)
            End If
            Return False
        End If

        Return True
    End Function
    'This is a temp function until they  decide to pass single builder information
    ' Currently they want all builder information and downloading it once a week
    Private Function EnsureUserNamePassword(ByVal UserName As String, ByVal Password As String, ByRef ErrorResponseObject As Object) As Boolean

        If UserName = String.Empty OrElse UserName.Trim = String.Empty OrElse Not AppSettings("AEServiceUsername") = UserName Then
            If TypeOf ErrorResponseObject Is ErrorResponse Then
                ErrorResponseObject = CType(ErrorResponseObject, ErrorResponse)
                ErrorResponseObject.Errordetails = GetErrorMessage(ErrorNumber.InvalidAuthToken)
            End If
            Return False
        End If
        If Password = String.Empty OrElse Password.Trim = String.Empty OrElse Not AppSettings("AEServicePassword") = Password Then
            If TypeOf ErrorResponseObject Is ErrorResponse Then
                ErrorResponseObject = CType(ErrorResponseObject, ErrorResponse)
                ErrorResponseObject.Errordetails = GetErrorMessage(ErrorNumber.InvalidAuthToken)
            End If
            Return False
        End If


        Return True
    End Function

    Private Function GetErrorMessage(ByVal en As Integer) As String
        Select Case en
            Case ErrorNumber.Success
                Return "Success"
            Case ErrorNumber.GeneralError
                Return "General Error"
            Case ErrorNumber.InvalidAuthToken
                Return "Invalid Authorization Token"
            Case Else
                Return "Invalid or Undefined Error"
        End Select
    End Function
    Public Shared Function GetString(ByVal sObject As Object) As String
        If IsDBNull(sObject) Then Return Nothing Else Return Convert.ToString(sObject)
    End Function
    Private Function GetLogInUserTypeAndID(ByVal username As String, ByVal Password As String) As String

        Dim dbBuilderAccount As BuilderAccountRow = Nothing
        dbBuilderAccount = BuilderAccountRow.GetAccountByUsername(DB, username)

        If dbBuilderAccount.BuilderAccountID <> Nothing AndAlso dbBuilderAccount.Password = Password Then
            Return "Builder_" & dbBuilderAccount.BuilderAccountID
        End If

        Dim dbPIQAccount As PIQAccountRow = PIQAccountRow.GetRowByUsername(DB, username)
        Dim dbPIQ As PIQRow = PIQRow.GetRow(DB, dbPIQAccount.PIQID)

        If (dbPIQAccount.PIQAccountID <> Nothing Or dbPIQ.PIQID <> Nothing) OrElse dbPIQAccount.Password = Password Then
            Return "PIQ_" & dbPIQAccount.PIQAccountID
        End If


        Return String.Empty

    End Function

#End Region

#Region "Enumerated Types"
    Public Enum Status As Integer
        Success = 0
        Failure = 1
        Cancel = 2
    End Enum

    Public Enum ErrorNumber
        Success
        GeneralError
        InvalidAuthToken
        Failure
    End Enum

#End Region

#Region "Response Objects"
    Public Class ResponseInfo
        Public ErrorMessage As String
    End Class
    Public Class AuthenticationResponse
        Public Status As String
        Public token As String
    End Class
    Public Class ErrorResponse
        Public Errordetails As String
    End Class
  
    Public Class ProductCategories

        Public ProductCategories As Generic.List(Of ProductCategoryDetails)
    End Class
    Public Class Manufacturers

        Public ManufacturersDetails As Generic.List(Of ManufacturersDetails)
    End Class

    Public Class ManufacturersDetails
        Public AEManufacturerID As String
        Public HistoricID As String
        Public ClassID As String
        Public CompanyName As String
        Public MailingAddress As String
        Public MailingCity As String
        Public MailingState As String
        Public MailingZip As String
        Public Website As String
        Public PrimaryContactName As String
        Public PrimaryContactEmail As String
        Public PrimaryContactPhone As String
        Public APContactName As String
        Public APContactEmail As String
        Public APContactPhone As String
        Public PaymentTerms As String
        
    End Class

    Public Class ProductCategoryDetails
        Public AEProductCategoryID As String
        Public ProductCategoryName As String
        Public ProductCategoryParentID As String
    End Class
    Public Class Product

        Public ProductDetails As Generic.List(Of ProductDetails)
    End Class
    Public Class LLC

        Public LLCDetails As Generic.List(Of LLCDetails)
    End Class
    Public Class LLCDetails
        Public AELLCID As String
        Public LLCName As String
        Public IsActive As String
    End Class
    Public Class Builders

        Public BuildersDetails As Generic.List(Of BuildersDetails)
    End Class
    Public Class BuildersDetails
        Public AEBuilderID As String
        Public AEBuilderHistoricID As String
        Public BuilderName As String
        Public PhoneNumber As String
        Public BuilderEmail As String
        Public BuilderMarket As String
        Public FirstName As String
        Public LastName As String
    End Class

    Public Class PIQ

        Public PIQDetails As Generic.List(Of PIQDetails)
    End Class
    Public Class PIQDetails
        Public AEPIQID As String
        Public CompanyName As String
        Public Address As String
        Public Phone As String
        Public Website As String
        Public PreferredVendors As String
    End Class

    Public Class BuilderAccount

        Public BuilderAccountDetails As Generic.List(Of BuilderAccountDetails)
    End Class
    Public Class BuilderAccountDetails
        Public AEBuilderID As String
        Public AEUSERID As String
        Public UserName As String
        Public FirstName As String
        Public LastName As String
        Public MiddleName As String
        Public Email As String
    End Class
    Public Class ProductDetails
        Public AEProductID As String
        Public ProductName As String
        Public ProductProductCategory As Generic.List(Of ProductProductCategory)
    End Class
    Public Class ProductProductCategory
        Public AEProductCategoryID As String
        Public ProductCategoryName As String
    End Class

  
#End Region
End Class