Imports System.Xml
Imports System.Net
Imports System.io
Imports System.Text
Imports System.Configuration.ConfigurationManager

Namespace Components

    'PRIORITYOVERNIGHT = "PRIORITYOVERNIGHT"
    'STANDARDOVERNIGHT = "STANDARDOVERNIGHT"
    'FIRSTOVERNIGHT = "FIRSTOVERNIGHT"
    'FEDEX2DAY = "FEDEX2DAY"
    'FEDEXEXPRESSSAVER = "FEDEXEXPRESSSAVER"
    'INTERNATIONALPRIORITY = "INTERNATIONALPRIORITY"
    'INTERNATIONALECONOMY = "INTERNATIONALECONOMY"
    'INTERNATIONALFIRST = "INTERNATIONALFIRST"
    'FEDEX1DAYFREIGHT = "FEDEX1DAYFREIGHT"
    'FEDEX2DAYFREIGHT = "FEDEX2DAYFREIGHT"
    'FEDEX3DAYFREIGHT = "FEDEX3DAYFREIGHT"
    'FEDEXGROUND = "FEDEXGROUND"
    'GROUNDHOMEDELIVERY = "GROUNDHOMEDELIVERY"
    'INTERNATIONALPRIORITY_FREIGHT = "INTERNATIONALPRIORITY FREIGHT"
    'INTERNATIONALECONOMY_FREIGHT = "INTERNATIONALECONOMY FREIGHT"

    Public Class FedEx

        Public Shared Function GetRate(ByVal OrderId As Integer, ByVal FromCity As String, ByVal FromState As String, ByVal FromZip As String, ByVal FromAddressLn1 As String, ByVal FromAddressLn2 As String, ByVal ToCity As String, ByVal ToState As String, ByVal ToZip As String, ByVal ToAddressLn1 As String, ByVal ToAddressLn2 As String, ByVal ToCountry As String, ByVal Weight As Double, ByVal Width As Double, ByVal Height As Double, ByVal Thickness As Double, ByVal ServiceRating As String, ByVal IsCommercial As Boolean, ByRef ErrorDesc As String) As Double
            Dim Packaging As String = "YOURPACKAGING"

            Dim CarrierCode As String = String.Empty
            If ServiceRating = "FEDEXGROUND" Then
                CarrierCode = "FDXG"
            Else
                CarrierCode = "FDXE"
            End If

            If Weight < 1 Then Weight = 1

            Dim XML As String = String.Empty

            XML &= "<?xml version=""1.0"" encoding=""UTF-8"" ?>" & vbCrLf
            XML &= "<FDXRateRequest xmlns:api=""http://www.fedex.com/fsmapi"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""FDXRateRequest.xsd"">" & vbCrLf
            XML &= "<RequestHeader>" & vbCrLf
            XML &= "<CustomerTransactionIdentifier>" & OrderId & "</CustomerTransactionIdentifier>" & vbCrLf
            XML &= "<AccountNumber>" & AppSettings("FedExAccountNo") & "</AccountNumber>" & vbCrLf
            XML &= "<MeterNumber>" & AppSettings("FedExMeterNo") & "</MeterNumber>" & vbCrLf
            XML &= "<CarrierCode>" & CarrierCode & "</CarrierCode>" & vbCrLf
            XML &= "</RequestHeader>" & vbCrLf
            XML &= "<ShipDate>" & Date.Today.ToString("YYYY-MM-DD") & "</ShipDate>" & vbCrLf
            XML &= "<DropoffType>REGULARPICKUP</DropoffType>" & vbCrLf
            XML &= "<Service>" & ServiceRating & "</Service>" & vbCrLf
            XML &= "<Packaging>" & Packaging & "</Packaging>" & vbCrLf
            XML &= "<WeightUnits>LBS</WeightUnits>" & vbCrLf
            XML &= "<Weight>" & FormatNumber(Weight, 1) & "</Weight>" & vbCrLf
            XML &= "<OriginAddress>" & vbCrLf
            XML &= "	<Line1>" & RemoveSpecialCharacters(FromAddressLn1) & "</Line1>"
            If Not RemoveSpecialCharacters(FromAddressLn2) = String.Empty Then
                XML &= "	<Line2>" & RemoveSpecialCharacters(FromAddressLn2) & "</Line2>"
            End If
            XML &= "    <City>" & RemoveSpecialCharacters(FromCity) & "</City>"
            XML &= "	<StateOrProvinceCode>" & RemoveSpecialCharacters(FromState) & "</StateOrProvinceCode>"
            XML &= "	<PostalCode>" & RemoveSpecialCharacters(FromZip) & "</PostalCode>"
            XML &= "	<CountryCode>US</CountryCode>"
            XML &= "</OriginAddress>" & vbCrLf
            XML &= "<DestinationAddress>" & vbCrLf
            XML &= "	<Line1>" & RemoveSpecialCharacters(ToAddressLn1) & "</Line1>"
            If Not RemoveSpecialCharacters(ToAddressLn2) = String.Empty Then
                XML &= "	<Line2>" & RemoveSpecialCharacters(ToAddressLn2) & "</Line2>"
            End If
            XML &= "	<City>" & RemoveSpecialCharacters(ToCity) & "</City>" & vbCrLf
            XML &= "	<StateOrProvinceCode>" & RemoveSpecialCharacters(ToState) & "</StateOrProvinceCode>" & vbCrLf
            XML &= "	<PostalCode>" & RemoveSpecialCharacters(ToZip) & "</PostalCode>" & vbCrLf
            XML &= "	<CountryCode>" & RemoveSpecialCharacters(ToCountry) & "</CountryCode>" & vbCrLf
            XML &= "</DestinationAddress>" & vbCrLf
            XML &= "<Payment>" & vbCrLf
            XML &= "<PayorType>SENDER</PayorType>" & vbCrLf
            XML &= "</Payment>" & vbCrLf
            If Width <> 0 And Height <> 0 And Thickness <> 0 Then
                XML &= " <Dimensions>" & vbCrLf
                XML &= "   <Length>" & Thickness & "</Length>" & vbCrLf
                XML &= "   <Width>" & Width & "</Width>" & vbCrLf
                XML &= "   <Height>" & Height & "</Height>" & vbCrLf
                XML &= "   <Units>IN</Units>" & vbCrLf
                XML &= " </Dimensions>" & vbCrLf
            End If
            XML &= "<PackageCount>1</PackageCount>" & vbCrLf
            XML &= "</FDXRateRequest>"

            Dim Result As String = String.Empty
            Try
                Dim req As HttpWebRequest = WebRequest.Create("https://gateway.fedex.com/GatewayDC")
                req.Method = "POST"
                req.ContentType = "application/x-www-form-urlencoded"
                req.ContentLength = XML.Length()
                Dim outStream As Stream = req.GetRequestStream()
                For x As Integer = 0 To XML.Length() - 1
                    outStream.WriteByte(Convert.ToByte(XML.Chars(x)))
                Next
                outStream.Flush()
                outStream.Close()
            Catch ex As WebException
                ErrorDesc = "The website timed out when attempting to connect to FedEx Online Tools, please try again later."
                Return -1
            Catch ex As Exception
                ErrorDesc = "There was a problem communicating with FedEx Online Tools, please try again later."
                Return -1
            End Try

            Dim xmlDoc As XmlDocument = New XmlDocument
            xmlDoc.LoadXml(Result)

            Dim TotalChargesNode As XmlNode = xmlDoc.SelectSingleNode("//EstimatedCharges/DiscountedCharges/NetCharge")
            If TotalChargesNode Is Nothing Then
                Return TotalChargesNode.InnerText
            End If

            Dim ErrorNode As XmlNode = xmlDoc.SelectSingleNode("//Error/Code")
            Dim PassFail As String = ErrorNode.InnerText
            If PassFail <> 0 Then
                Dim ErrorValueNode As XmlNode = ErrorNode.SelectSingleNode("//Error/Message")
                ErrorDesc = ErrorValueNode.InnerText
                Return -1
            End If
            ErrorDesc = "There was a problem communicating with FedEx Online Tools, please try again later."
            Return -1
        End Function

        Private Shared Function RemoveSpecialCharacters(ByVal Input As String) As String
            If Input = String.Empty Then Return String.Empty

            Dim Result As String = Input
            Result = Replace(Result, "#", "")

            Return Result
        End Function
    End Class

End Namespace
