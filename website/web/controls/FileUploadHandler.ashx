<%@ WebHandler Language="VB" Class="FileUploadHandler" %>

Imports System
Imports System.IO
Imports System.Net
Imports System.Web
Imports System.Web.Script.Serialization
Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Public Class FileUploadHandler : Implements IHttpHandler

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

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        If HttpContext.Current.Request("QuoteId") Is Nothing OrElse (HttpContext.Current.Request("BuilderId") Is Nothing And HttpContext.Current.Request("VendorId") Is Nothing) OrElse HttpContext.Current.Request.Files.Count <> 1 Then
            HttpContext.Current.Response.Write("Upload Error: Invalid Parameters.")
            Exit Sub
        End If

        Try
            QuoteId = Convert.ToInt32(HttpContext.Current.Request("QuoteId"))
            BuilderId = DecodeAndDecryptUrlParameter("BuilderId")
            VendorId = DecodeAndDecryptUrlParameter("VendorId")
            TwoPriceCampaignId = Convert.ToInt32(HttpContext.Current.Request("TwoPriceCampaignId"))
            TwoPriceDocumentId = Convert.ToInt32(HttpContext.Current.Request("TwoPriceDocumentId"))
            NCPContentID = Convert.ToInt32(HttpContext.Current.Request("NCPContentID"))
            NCPContentDocumentId = Convert.ToInt32(HttpContext.Current.Request("NCPContentDocumentId"))
            TakeOFFServiceId = Convert.ToInt32(HttpContext.Current.Request("TakeOffServiceId"))
        Catch ex As Exception

        End Try

        If HttpContext.Current.Request.Files.AllKeys.Any() Then
            Dim PostedFile = HttpContext.Current.Request.Files("UploadedFile")

            If Not PostedFile Is Nothing Then
                UploadAndPublish(PostedFile)

                'Set the Folder Path.
                'Dim folderPath As String = context.Server.MapPath("~/assets/")

                'Set the File Name.
                'Dim fileName As String = Path.GetFileName(PostedFile.FileName)

                'PostedFile.SaveAs(folderPath + fileName)
            End If
        End If

    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

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

    Private Function DecodeAndDecryptUrlParameter(ByRef Param As String) As String
        Dim DecodedParam As String = String.Empty
        Try
            DecodedParam = Utility.Crypt.DecryptTripleDes(HttpContext.Current.Request(Param))
        Catch ex As Exception
            Try
                DecodedParam = Utility.Crypt.DecryptTripleDes(HttpUtility.UrlDecode(HttpContext.Current.Request(Param)))
            Catch ex2 As Exception
                DecodedParam = String.Empty
            End Try
        End Try

        Return DecodedParam
    End Function

    Private Sub UploadAndPublish(ByVal f As HttpPostedFile)
        Dim DB As New Database()
        DB.Open(AppSettings("ConnectionString"))

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
                    HttpContext.Current.Response.ClearContent()
                    HttpContext.Current.Response.Write("Unrecognized file type")
                    HttpContext.Current.Response.Flush()
                    HttpContext.Current.Response.Close()

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

                dbDocument = New NCPDocumentRow(DB)

                dbDocument.GUID = Guid.NewGuid.ToString
                dbDocument.Title = UploadFilename
                dbDocument.Uploaded = Now
                dbDocument.FileName = UploadFilename
                dbDocument.NCPContentID = NCPContentID
                dbDocument.Insert()
                HttpContext.Current.Response.Write(dbDocument.DocumentId & ";" & ThumbURL & ";" & dbDocument.Title & ";" & ThumbURL & ";" & dbDocument.DocumentId & ";" & "\assets\DocumentMultiUploadThumbnails")
            End If
            If TakeOFFServiceId > 0 Then
                Dim dbDocument As TakeoffServiceDocumentRow

                dbDocument = New TakeoffServiceDocumentRow(DB)

                dbDocument.GUID = Guid.NewGuid.ToString
                dbDocument.Title = UploadFilename
                dbDocument.Uploaded = Now
                dbDocument.FileName = UploadFilename
                dbDocument.TakeOffServiceID = TakeOFFServiceId
                dbDocument.Insert()
                HttpContext.Current.Response.Write(dbDocument.DocumentId & ";" & ThumbURL & ";" & dbDocument.Title & ";" & ThumbURL & ";" & dbDocument.DocumentId & ";" & "\assets\DocumentMultiUploadThumbnails")
            End If

            If TwoPriceCampaignId > 0 Then
                Dim dbDocument As TwoPriceDocumentRow

                dbDocument = New TwoPriceDocumentRow(DB)

                dbDocument.GUID = Guid.NewGuid.ToString
                dbDocument.Title = UploadFilename
                dbDocument.Uploaded = Now
                dbDocument.FileName = UploadFilename
                dbDocument.TwoPriceCampaignId = TwoPriceCampaignId
                dbDocument.Insert()

                HttpContext.Current.Response.Write(dbDocument.DocumentId & ";" & ThumbURL & ";" & dbDocument.Title & ";" & ThumbURL & ";" & dbDocument.DocumentId & ";" & "\assets\DocumentMultiUploadThumbnails")
            End If


            If BuilderId > 0 Then
                Dim dbDocument As POBuilderDocumentRow

                dbDocument = New POBuilderDocumentRow(DB)
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
                HttpContext.Current.Response.Write(dbDocument.BuilderDocumentId & ";" & ThumbURL & ";" & dbDocument.Title & ";" & ThumbURL & ";" & dbDocument.BuilderDocumentId & ";" & "\assets\DocumentMultiUploadThumbnails")
            ElseIf VendorId > 0 Then
                Dim dbDocument As POVendorDocumentRow

                dbDocument = New POVendorDocumentRow(DB)
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
                HttpContext.Current.Response.Write(dbDocument.VendorDocumentId & ";" & ThumbURL & ";" & dbDocument.Title & ";" & ThumbURL & ";" & dbDocument.VendorDocumentId & ";" & "\assets\DocumentMultiUploadThumbnails")
            End If

            DB.CommitTransaction()


        Catch ex As Exception
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            Logger.Error(ex.Message)
        End Try

        HttpContext.Current.Response.Write(TempId & ";" & cleanName & ";" & AppSettings("DocumentsFolder") & UploadFilename & ";")
    End Sub

End Class