Imports System.Xml
Imports System.Net
Imports System.io
Imports System.Text
Imports System.Configuration.ConfigurationManager

Namespace Components

    'RATE_GROUND = "03"
    'RATE_SECOND_DAY = "02"
    'RATE_NEXT_DAY = "01"
    'RATE_STANDARD = "11"
    'RATE_3DAY = "12"
    'RATE_NEXT_DAY_AIR_SAVER = "13"
    'RATE_NEXT_DAY_EARLY_AM = "14"
    'RATE_WORLDWIDE_EXPRESS = "07"
    'RATE_WORLDWIDE_EXPRESS_PLUS = "54"
    'RATE_2DAY_AIR_AM = "59"
    'RATE_WORLDWIDE_EXPEDITED = "08"

    Public Class UPS

        Public Shared Function ValidateAddress(ByVal ToCity As String, ByVal ToState As String, ByVal ToZip As String, ByVal ToCountry As String, ByRef ErrorDesc As String) As Boolean
            Dim doc As XmlDocument = New XmlDocument
            Dim AccessRequest, License, UserId, Password As XmlElement

            AccessRequest = doc.CreateElement("AccessRequest")
            AccessRequest.SetAttribute("xml:lang", "en-US")

            License = doc.CreateNode("element", "AccessLicenseNumber", "")
            License.InnerText = AppSettings("UPSLicenseNo")
            AccessRequest.AppendChild(License)

            UserId = doc.CreateNode("element", "UserId", "")
            UserId.InnerText = AppSettings("UPSUsername")
            AccessRequest.AppendChild(UserId)

            Password = doc.CreateNode("element", "Password", "")
            Password.InnerText = AppSettings("UPSPassword")
            AccessRequest.AppendChild(Password)
            doc.AppendChild(AccessRequest)

            Dim addr As XmlDocument = New XmlDocument
            Dim RequestType, RequestNode, Reference, Customer As XmlElement
            Dim Version, RequestAction, Address, PostalCode, CountryCode As XmlElement
            Dim City, State As XmlElement

            RequestType = addr.CreateElement("AddressValidationRequest")
            RequestType.SetAttribute("xml:lang", "en-US")
            RequestNode = addr.CreateNode("element", "Request", "")
            Reference = addr.CreateNode("element", "TransactionReference", "")
            Customer = addr.CreateNode("element", "CustomerContext", "")
            Customer.InnerText = "Customer Data"
            Reference.AppendChild(Customer)

            Version = addr.CreateNode("element", "XpciVersion", "")
            Version.SetAttribute("version", "1.0001")
            Reference.AppendChild(Version)
            RequestNode.AppendChild(Reference)

            RequestAction = addr.CreateNode("element", "RequestAction", "")
            RequestAction.InnerText = "AV"
            RequestNode.AppendChild(RequestAction)
            RequestType.AppendChild(RequestNode)

            Address = addr.CreateNode("element", "Address", "")
            City = addr.CreateNode("element", "City", "")
            City.InnerText = ToCity
            Address.AppendChild(City)

            State = addr.CreateNode("element", "StateProvinceCode", "")
            State.InnerText = ToState
            Address.AppendChild(State)

            PostalCode = addr.CreateNode("element", "PostalCode", "")
            PostalCode.InnerText = ToZip
            Address.AppendChild(PostalCode)

            CountryCode = addr.CreateNode("element", "CountryCode", "")
            CountryCode.InnerText = ToCountry
            Address.AppendChild(CountryCode)
            RequestType.AppendChild(Address)

            addr.AppendChild(RequestType)

            Dim Result As String = String.Empty
            Try
                Dim XML As String = "<?xml version=""1.0""?>" & doc.InnerXml & "<?xml version=""1.0""?>" & addr.InnerXml
                Dim r As HttpWebRequest = WebRequest.Create("https://www.ups.com/ups.app/xml/AV")
                Dim encodedData As ASCIIEncoding = New ASCIIEncoding
                Dim byteArray() As Byte = encodedData.GetBytes(XML)

                r.Method = "POST"
                r.ContentType = "application/x-www-form-urlencoded"
                r.ContentLength = XML.Length()
                r.KeepAlive = False

                Dim SendStream As Stream = r.GetRequestStream
                SendStream.Write(byteArray, 0, byteArray.Length)
                SendStream.Close()

                Dim wr As HttpWebResponse = CType(r.GetResponse(), HttpWebResponse)

                Dim reader As New IO.StreamReader(wr.GetResponseStream())
                Result = reader.ReadToEnd()
                reader.Close()
            Catch ex As WebException
                ErrorDesc = "The website timed out when attempting to connect to UPS Online Tools, please try again later."
                Return False
            Catch ex As Exception
                ErrorDesc = "There was a problem communicating with UPS Online Tools, please try again later."
                Return False
            End Try

            'Parse response
            Dim xmlDoc As XmlDocument = New XmlDocument
            xmlDoc.LoadXml(Result)

            Dim Errors As XmlNodeList = xmlDoc.SelectNodes("AddressValidationResponse/Response")
            Dim Status As String = Errors.Item(0).SelectSingleNode("ResponseStatusDescription").InnerText
            If Status = "Success" Then
                Dim Locations As XmlNodeList = xmlDoc.SelectNodes("AddressValidationResponse/AddressValidationResult")
                If Locations.Count > 1 Then
                    ErrorDesc = "The address is not valid. Please verify the City, State and Zipcode are correct."
                    Return False
                End If
                Dim Quality As Double = Locations(0).SelectSingleNode("Quality").InnerText
                If Quality <> 1 Then
                    ErrorDesc = "The address is not valid. Please verify the City, State and Zipcode are correct."
                    Return False
                End If
            Else
                ErrorDesc = Errors.Item(0).SelectSingleNode("Error/ErrorDescription").InnerText
                Return False
            End If
            Return True
        End Function

        Public Shared Function GetRate(ByVal FromCity As String, ByVal FromState As String, ByVal FromZip As String, ByVal ToCity As String, ByVal ToState As String, ByVal ToZip As String, ByVal ToCountry As String, ByVal Weight As Double, ByVal Width As Double, ByVal Height As Double, ByVal Thickness As Double, ByVal ServiceRating As String, ByVal IsCommercial As Boolean, ByRef ErrorDesc As String) As Double
            ' For UPS Rate/Service Utility, a minimum weight of 1lb is required.
            If Weight < 1 Then Weight = 1

            Dim XML As String = String.Empty
            XML &= "<?xml version=""1.0""?>" & vbCrLf & "<AccessRequest><AccessLicenseNumber>" & AppSettings("UPSLicenseNo") & "</AccessLicenseNumber><UserId>" & AppSettings("UPSUsername") & "</UserId><Password>" & AppSettings("UPSPassword") & "</Password></AccessRequest>" & vbCrLf
            XML &= "<?xml version=""1.0""?>" & vbCrLf
            XML &= "<RatingServiceSelectionRequest xml:lang=""en-US"">" & vbCrLf
            XML &= "<Request>" & vbCrLf
            XML &= "    <TransactionReference>" & vbCrLf
            XML &= "    <CustomerContext>Rating and Service</CustomerContext>" & vbCrLf
            XML &= "    <XpciVersion Version=""1.0001""/>" & vbCrLf
            XML &= "    </TransactionReference>" & vbCrLf
            XML &= "    <RequestAction>rate</RequestAction>" & vbCrLf
            XML &= "	<RequestOption>rate</RequestOption>" & vbCrLf
            XML &= "</Request>" & vbCrLf
            XML &= "<Shipment>" & vbCrLf
            XML &= "    <Shipper>" & vbCrLf
            XML &= "	    <Address>" & vbCrLf
            XML &= "		    <City>" & FromCity & "</City>" & vbCrLf
            XML &= "			<StateProvinceCode>" & FromState & "</StateProvinceCode>" & vbCrLf
            XML &= "			<PostalCode>" & FromZip & "</PostalCode>" & vbCrLf
            XML &= "			<CountryCode>US</CountryCode>" & vbCrLf
            XML &= "            <ResidentialAddressIndicator/>" & vbCrLf
            XML &= "		</Address>" & vbCrLf
            XML &= "    </Shipper>" & vbCrLf
            XML &= "    <ShipTo>" & vbCrLf
            XML &= "    <Address>" & vbCrLf
            XML &= "        <City>" & ToCity & "</City>" & vbCrLf
            XML &= "        <StateProvinceCode>" & ToState & "</StateProvinceCode>" & vbCrLf
            XML &= "		<PostalCode>" & ToZip & "</PostalCode>" & vbCrLf
            XML &= "		<CountryCode>" & ToCountry & "</CountryCode>" & vbCrLf
            If IsCommercial Then
                XML &= "        <ResidentialAddress>0</ResidentialAddress>" & vbCrLf
            Else
                XML &= "        <ResidentialAddress>1</ResidentialAddress>" & vbCrLf
            End If
            XML &= "    </Address>" & vbCrLf
            XML &= "    </ShipTo>" & vbCrLf
            XML &= "    <ShipmentWeight>" & vbCrLf
            XML &= "        <UnitOfMeasurement>" & vbCrLf
            XML &= "        <Code>lbs</Code>" & vbCrLf
            XML &= "        <Description>pounds</Description>" & vbCrLf
            XML &= "        </UnitOfMeasurement>" & vbCrLf
            XML &= "        <Weight>" & FormatNumber(Weight, 0, TriState.True, TriState.False, TriState.False) & "</Weight>" & vbCrLf
            XML &= "    </ShipmentWeight>" & vbCrLf
            XML &= "    <Service>" & vbCrLf
            XML &= "        <Code>" & ServiceRating & "</Code>" & vbCrLf
            XML &= "    </Service>" & vbCrLf
            XML &= "    <Package>" & vbCrLf
            XML &= "        <PackagingType>" & vbCrLf
            XML &= "        <Code>02</Code>" & vbCrLf
            XML &= "        </PackagingType>" & vbCrLf
            XML &= "        <PackageWeight>" & vbCrLf
            XML &= "        <UnitOfMeasurement>" & vbCrLf
            XML &= "        <Code>lbs</Code>" & vbCrLf
            XML &= "        </UnitOfMeasurement>" & vbCrLf
            XML &= "        <Weight>" & FormatNumber(Weight, 0, TriState.True, TriState.False, TriState.False) & "</Weight>" & vbCrLf
            XML &= "        </PackageWeight>" & vbCrLf
            If Width <> 0 And Height <> 0 And Thickness <> 0 Then
                XML &= "    <Dimensions>" & vbCrLf
                XML &= "    <Width>" & Width & "</Width>" & vbCrLf
                XML &= "    <Height>" & Height & "</Height>" & vbCrLf
                XML &= "    <Length>" & Thickness & "</Length>" & vbCrLf
                XML &= "    </Dimensions>" & vbCrLf
            End If
            XML &= "    </Package>" & vbCrLf
            XML &= "</Shipment>" & vbCrLf
            XML &= "</RatingServiceSelectionRequest>"

            Dim Result As String = String.Empty
            Try
                Dim req As HttpWebRequest = WebRequest.Create("https://www.ups.com/ups.app/xml/Rate")
                req.Method = "POST"
                req.ContentType = "application/x-www-form-urlencoded"
                req.ContentLength = XML.Length()
                Dim outStream As Stream = req.GetRequestStream()
                For x As Integer = 0 To XML.Length() - 1
                    outStream.WriteByte(Convert.ToByte(XML.Chars(x)))
                Next
                outStream.Flush()
                outStream.Close()

                Dim res As HttpWebResponse = CType(req.GetResponse(), HttpWebResponse)
                Dim reader As New IO.StreamReader(res.GetResponseStream())
                Result = reader.ReadToEnd()
                reader.Close()

            Catch ex As WebException
                ErrorDesc = "The website timed out when attempting to connect to UPS Online Tools, please try again later."
                Return -1
            Catch ex As Exception
                ErrorDesc = "There was a problem communicating with UPS Online Tools, please try again later."
                Return -1
            End Try

            Dim xmlDoc As XmlDocument = New XmlDocument
            xmlDoc.LoadXml(Result)

            Dim responseNode As XmlNode = xmlDoc.SelectSingleNode("//RatingServiceSelectionResponse/Response")
            Dim ResponseValueNode As XmlNode = responseNode.SelectSingleNode("//ResponseStatusDescription")
            Dim PassFail As String = ResponseValueNode.InnerText

            If PassFail = "Failure" Then
                Dim ErrorNode As XmlNode = responseNode.SelectSingleNode("//Error")
                Dim ErrorValueNode As XmlNode = ErrorNode.SelectSingleNode("//ErrorDescription")

                ErrorDesc = ErrorValueNode.InnerText
                Return -1
            End If

            Dim TotalChargesNode As XmlNode = responseNode.SelectSingleNode("//TotalCharges/MonetaryValue")
            Dim TotalCharges As String = TotalChargesNode.InnerText
            If IsNumeric(TotalCharges) Then
                Return Double.Parse(TotalCharges)
            End If

            ErrorDesc = "There was a problem communicating with UPS Online Tools."
            Return -1
        End Function
    End Class

End Namespace
