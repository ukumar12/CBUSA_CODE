Imports Components
Imports DataLayer
Imports System.IO
Imports System.Web.HttpUtility
Imports System.Configuration.ConfigurationManager
Imports System.Collections.Generic

Partial Class AssetManager_FileUploadAndPublish
    Inherits SitePage
    Private Admin As String = Nothing
    Private SiteId As String
    Private ImageQuality As Integer = Nothing
    Private Tool As String = String.Empty
    Private QuoteId As Integer = 0
    Private BuilderId As Integer = 0
    Private VendorId As Integer = 0
    Private TwoPriceCampaignId As Integer = 0
    Private TwoPriceDocumentId As Integer = 0
    Private NCPContentID As Integer = 0
    Private NCPContentDocumentId As Integer = 0
    Private TakeOFFServiceId As Integer = 0
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'IsLoggedIn will return false because this script is called by a swf file, and the 
        'session cookie is not sent. therefore, the MemberId must be attached to the request.

        Try
            If Request("QuoteId") Is Nothing OrElse (Request("BuilderId") Is Nothing And Request("VendorId") Is Nothing) OrElse Request.Files.Count <> 1 Then
                Response.Write("Upload Error: Invalid Parameters.")
                Logger.Error("Invalid Upload Attempt: " & vbCrLf & "URL: " & Request.RawUrl & vbCrLf & "Files Uploaded: " & Request.Files.Count)
                Exit Sub
            End If

            Try
                QuoteId = Convert.ToInt32(Request("QuoteId"))
                BuilderId = DecodeAndDecryptUrlParameter("BuilderId")
                VendorId = DecodeAndDecryptUrlParameter("VendorId")
                TwoPriceCampaignId = Convert.ToInt32(Request("TwoPriceCampaignId"))
                TwoPriceDocumentId = Convert.ToInt32(Request("TwoPriceDocumentId"))
                NCPContentID = Convert.ToInt32(Request("NCPContentID"))
                NCPContentDocumentId = Convert.ToInt32(Request("NCPContentDocumentId"))
                TakeOFFServiceId = Convert.ToInt32(Request("TakeOffServiceId"))
            Catch ex As Exception

            End Try

            Dim NewAssetFile As HttpPostedFile = Request.Files(0)

            UploadAndPublish(NewAssetFile)
        Catch ex As Exception
            Logger.Error("Error during Asset Upload: " & ex.ToString)
            Response.Write(ex.ToString())
        End Try
    End Sub

    Private Function DecodeAndDecryptUrlParameter(ByRef Param As String) As String
        Dim DecodedParam As String = String.Empty
        Try
            DecodedParam = Utility.Crypt.DecryptTripleDes(Request(Param))
        Catch ex As Exception
            Try
                DecodedParam = Utility.Crypt.DecryptTripleDes(HttpUtility.UrlDecode(Request(Param)))
            Catch ex2 As Exception
                DecodedParam = String.Empty
            End Try
        End Try

        Return DecodedParam
    End Function

    Private Sub UploadAndPublish(ByVal f As HttpPostedFile)
        Dim UploadFilename As String = RemoveSpecialCharacters(f.FileName)

        ' this is where the saving happens...
        Dim destFolder As String = AppSettings("BuilderDocumentPlansOnline")
        If TwoPriceCampaignId > 0 Then
            destFolder = AppSettings("TwopriceDocument")
        End If

        If TakeOFFServiceId > 0 Then
            destFolder = AppSettings("TakeOFFServiceDocument")
        End If


        If NCPContentID > 0 Then
            destFolder = AppSettings("NCPDocuments")
        End If

        If VendorId > 0 Then
            destFolder = AppSettings("VendorDocumentPlansOnline")
        End If
        Dim diAsset As DirectoryInfo = New DirectoryInfo(destFolder)
        If Not diAsset.Exists Then diAsset.Create()

        Dim MapPath As String = destFolder & UploadFilename
        Dim cleanName As String = UploadFilename.Substring(0, InStrRev(UploadFilename, ".") - 1)
        Dim cleanExt As String = UploadFilename.Substring(InStrRev(UploadFilename, "."))

        Dim fi As New FileInfo(MapPath)
        Dim n As Integer = 0
        If fi.Exists Then
            While fi.Exists
                n += 1
                fi = New FileInfo(destFolder & cleanName & n & "." & cleanExt)
            End While
        End If

        If n > 0 Then
            MapPath = destFolder & cleanName & n & "." & cleanExt
            UploadFilename = cleanName & n & "." & cleanExt
        End If

        If File.Exists(MapPath) Then File.Delete(MapPath)
        f.SaveAs(MapPath)

        ' file saving done, now do some database stuff
        Dim ResourceTypeId As Integer = 0
        Dim TempId As Integer = 0
        Dim FileExtension As String = String.Empty
        Try
            DB.BeginTransaction()

            'dbResourceTemp = ResourceTempRow.GetByGUID(DB, Guid)
            FileExtension = cleanExt

            Select Case FileExtension.Trim.ToLower.Replace(".", "")
                Case "3gp", "asf", "asx", "avi", "mov", "mp4", "mpg", "qt", "rm", "swf", "wmv", "flv"
                    ResourceTypeId = 1
                Case "jpg", "jpeg", "gif", "png", "bmp"
                    ResourceTypeId = 2
                Case "txt", "csv", "doc", "pdf", "ppt", "xls", "docx", "pptx", "xlsx", "dwf", "dwg", "rtf", "zip", "cad"
                    ResourceTypeId = 3
                Case Else
                    Response.ClearContent()
                    Response.Write("Unrecognized file type")
                    Response.Flush()
                    Response.Close()
                    DB.RollbackTransaction()
                    Exit Sub
            End Select

            Dim ThumbURL As String = String.Empty

            Select Case ResourceTypeId
                Case 1
                    ThumbURL = "\assets\DocumentMultiUploadThumbnails\fileVideo.jpg"
                Case 2
                    ThumbURL = "\assets\DocumentMultiUploadThumbnails\filePhoto.jpg"
                Case 3
                    ThumbURL = "\assets\DocumentMultiUploadThumbnails\fileNews.jpg"
                Case Else
                    ThumbURL = "\assets\DocumentMultiUploadThumbnails\na50.jpg"
            End Select
            If NCPContentID > 0 Then
                Dim dbDocument As NCPDocumentRow

                dbDocument = New NCPDocumentRow(Me.DB)

                dbDocument.GUID = Guid.NewGuid.ToString
                dbDocument.Title = UploadFilename
                dbDocument.Uploaded = Now
                dbDocument.FileName = UploadFilename
                dbDocument.NCPContentID = NCPContentID
                dbDocument.Insert()
                Response.Write(dbDocument.DocumentId & ";" & ThumbURL & ";" & dbDocument.Title & ";" & ThumbURL & ";" & dbDocument.DocumentId & ";" & "\assets\DocumentMultiUploadThumbnails")
            End If
            If TakeOFFServiceId > 0 Then
                Dim dbDocument As TakeoffServiceDocumentRow

                dbDocument = New TakeoffServiceDocumentRow(Me.DB)

                dbDocument.GUID = Guid.NewGuid.ToString
                dbDocument.Title = UploadFilename
                dbDocument.Uploaded = Now
                dbDocument.FileName = UploadFilename
                dbDocument.TakeOffServiceID = TakeOFFServiceId
                dbDocument.Insert()
                Response.Write(dbDocument.DocumentId & ";" & ThumbURL & ";" & dbDocument.Title & ";" & ThumbURL & ";" & dbDocument.DocumentId & ";" & "\assets\DocumentMultiUploadThumbnails")
            End If

            If TwoPriceCampaignId > 0 Then
                Dim dbDocument As TwoPriceDocumentRow

                dbDocument = New TwoPriceDocumentRow(Me.DB)

                dbDocument.GUID = Guid.NewGuid.ToString
                dbDocument.Title = UploadFilename
                dbDocument.Uploaded = Now
                dbDocument.FileName = UploadFilename
                dbDocument.TwoPriceCampaignId = TwoPriceCampaignId
                dbDocument.Insert()
 
                Response.Write(dbDocument.DocumentId & ";" & ThumbURL & ";" & dbDocument.Title & ";" & ThumbURL & ";" & dbDocument.DocumentId & ";" & "\assets\DocumentMultiUploadThumbnails")
            End If


            If BuilderId > 0 Then
                Dim dbDocument As POBuilderDocumentRow

                dbDocument = New POBuilderDocumentRow(Me.DB)
                dbDocument.BuilderId = BuilderId
                dbDocument.GUID = Guid.NewGuid.ToString
                dbDocument.Title = UploadFilename
                dbDocument.Uploaded = Now
                dbDocument.FileName = UploadFilename
                dbDocument.Insert()

                If TwoPriceCampaignId > 0 Then
                    DB.ExecuteSQL("Insert Into TwoPriceCompaignBuilderDocument (TwoPriceCampaignId, BuilderDocumentId) Values (" & DB.Number(TwoPriceCampaignId) & ", " & DB.Number(dbDocument.BuilderDocumentId) & ")")
                ElseIf QuoteId > 0 Then
                    DB.ExecuteSQL("Insert Into POQuoteBuilderDocument (QuoteId, BuilderDocumentId) Values (" & DB.Number(QuoteId) & ", " & DB.Number(dbDocument.BuilderDocumentId) & ")")
                ElseIf TakeOFFServiceId > 0 Then
                    DB.ExecuteSQL("Insert Into TakeoffServiceDocument (TakeOffServiceID, DocumentID) Values (" & DB.Number(TakeOFFServiceId) & ", " & DB.Number(dbDocument.BuilderDocumentId) & ")")
                End If
                Response.Write(dbDocument.BuilderDocumentId & ";" & ThumbURL & ";" & dbDocument.Title & ";" & ThumbURL & ";" & dbDocument.BuilderDocumentId & ";" & "\assets\DocumentMultiUploadThumbnails")
            ElseIf VendorId > 0 Then
                Dim dbDocument As POVendorDocumentRow

                dbDocument = New POVendorDocumentRow(Me.DB)
                dbDocument.VendorId = VendorId
                dbDocument.GUID = Guid.NewGuid.ToString
                dbDocument.Title = UploadFilename
                dbDocument.Uploaded = Now
                dbDocument.FileName = UploadFilename
                dbDocument.Insert()
                If TwoPriceCampaignId > 0 Then
                    DB.ExecuteSQL("Insert Into TwoPriceCompaignVendorDocument (TwoPriceCampaignId, VendorDocumentId) Values (" & DB.Number(TwoPriceCampaignId) & ", " & DB.Number(dbDocument.VendorDocumentId) & ")")
                ElseIf QuoteId > 0 Then
                    DB.ExecuteSQL("Insert Into POQuoteVendorDocument (QuoteId, VendorDocumentId) Values (" & DB.Number(QuoteId) & ", " & DB.Number(dbDocument.VendorDocumentId) & ")")
                End If
                Response.Write(dbDocument.VendorDocumentId & ";" & ThumbURL & ";" & dbDocument.Title & ";" & ThumbURL & ";" & dbDocument.VendorDocumentId & ";" & "\assets\DocumentMultiUploadThumbnails")
            End If

            DB.CommitTransaction()


        Catch ex As Exception
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            Logger.Error(ex.Message)
        End Try

        Response.Write(TempId & ";" & cleanName & ";" & AppSettings("DocumentsFolder") & UploadFilename & ";")
    End Sub

    'same as the core version, but we just use underscores instead of completely stripping them.
    Private Function RemoveSpecialCharacters(ByVal input As String) As String
        If input = String.Empty Then
            Return String.Empty
        End If

        input = Replace(input, ":", "_")
        input = Replace(input, "<", "_")
        input = Replace(input, ">", "_")
        input = Replace(input, "=", "_")
        input = Replace(input, "+", "_")
        input = Replace(input, "@", "_")
        input = Replace(input, Chr(34), "_")
        input = Replace(input, "%", "_")
        input = Replace(input, "&", "_")
        input = Replace(input, "/", "_")
        input = Replace(input, "#", "_")
        input = Replace(input, " ", "_")
        input = Replace(input, "'", "")

        Return input
    End Function

End Class
