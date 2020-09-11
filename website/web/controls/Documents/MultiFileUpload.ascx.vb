Imports System
Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports System.IO
Imports System.Net
Imports Controls

Public Class controls_MultiFileUpload
    Inherits ModuleControl

    'see http://www.swfupload.org/ for documentation 
    Private m_UploadUrl As String
    Private m_FilePostName As String = "uploadfile"
    Private m_FileSizeLimit As Integer = 1024 * 20 'size in Kilobytes of total upload
    Private m_FileTypes As String = "*.bmp;*.jpg;*.png;*.gif;*.jpeg;*.tif;*.tiff;*.pdf;*.docx;*.doc;*.xls;*.xlsx;*.txt;*.csv;*.ppt;*.pptx;*.dwf;*.dwg;*.rtf;*.zip;*.cad"
    Private m_FileTypesDescription As String = "Documents and Images"
    Private m_FileUploadLimit As Integer = 20
    Private m_ButtonImageUrl As String = "/includes/swfupload/wdp_buttons_upload_114x29.png"
    Private m_ButtonWidth As Integer = 114
    Private m_ButtonHeight As Integer = 29
    Private m_UniqueBulkId As Integer = 0
    Private m_UpdatePanelClientId As String = String.Empty
    Private m_QuoteId As Integer = 0
    Private m_BuildierId As Integer = 0
    Private m_VendorId As Integer = 0
    Private m_TwoPriceCampaignId As Integer = 0
    Private m_DocumentId As Integer = 0
    Private m_NCPContentId As Integer = 0
    Private m_TakeOffServiceId As Integer = 0
    Public Sub PrepareBulkUpload(Optional ByVal fromobj As Boolean = False)
        UploadUrl = "/controls/documents/fileuploadandpublish.aspx"
        'any other parameters added to the UploadUrl need to be encrypted and encoded:
        UploadUrl &= "?QuoteId=" & QuoteId
        UploadUrl &= "&BuilderId=" & Server.UrlEncode(Utility.Crypt.EncryptTripleDes(BuilderId))
        UploadUrl &= "&VendorId=" & Server.UrlEncode(Utility.Crypt.EncryptTripleDes(VendorId))
        UploadUrl &= "&TwoPriceCampaignId=" & TwoPriceCampaignId
        UploadUrl &= "&DocumentId=" & DocumentId
        UploadUrl &= "&NCPContentID=" & ncpContentID
        UploadUrl &= "&TakeOffServiceId=" & TakeOffServiceId
        'UploadUrl &= "?SiteId=" & Core.URLEncode(Utility.Rijndael.Encrypt(GlobalSiteId))
        'UploadUrl &= "&a=" & Core.URLEncode(Utility.Rijndael.Encrypt(CType(Me.Page, AdminPage).LoggedInAdminId))
        'UploadUrl &= "&AssetDimensionKey=" & Core.URLEncode(Utility.Crypt.EncryptTripleDes(AssetDimensionKey))
        'UniqueBulkId = BulkUploader.UniqueId
        Session("ShowTagMessage") = "ShowTagMessage"
    End Sub



    Public Property DocumentId() As Integer
        Get
            Return m_DocumentId
        End Get
        Set(ByVal value As Integer)
            m_DocumentId = value
        End Set
    End Property

     
    Public Property TwoPriceCampaignId() As Integer
        Get
            Return m_TwoPriceCampaignId
        End Get
        Set(ByVal value As Integer)
            m_TwoPriceCampaignId = value
        End Set
    End Property

    Public Property ncpContentID() As Integer
        Get
            Return m_NCPContentId
        End Get
        Set(ByVal value As Integer)
            m_NCPContentId = value
        End Set
    End Property
    Public Property BuilderId() As Integer
        Get
            Return m_BuildierId
        End Get
        Set(ByVal value As Integer)
            m_BuildierId = value
        End Set
    End Property
    Public Property TakeOffServiceId() As Integer
        Get
            Return m_TakeOffServiceId
        End Get
        Set(ByVal value As Integer)
            m_TakeOffServiceId = value
        End Set
    End Property
    Public Property VendorId() As Integer
        Get
            Return m_VendorId
        End Get
        Set(ByVal value As Integer)
            m_VendorId = value
        End Set
    End Property

    Public Property QuoteId() As Integer
        Get
            Return m_QuoteId
        End Get
        Set(ByVal value As Integer)
            m_QuoteId = value
        End Set
    End Property

    Public Property UpdatePanelClientId() As String
        Get
            Return m_UpdatePanelClientId
        End Get
        Set(ByVal value As String)
            m_UpdatePanelClientId = value
        End Set
    End Property

    Public Property UploadUrl() As String
        Get
            Return m_UploadUrl
        End Get
        Set(ByVal value As String)
            m_UploadUrl = value
        End Set
    End Property

    Public Property FilePostName() As String
        Get
            Return m_FilePostName
        End Get
        Set(ByVal value As String)
            m_FilePostName = value
        End Set
    End Property

    Public Property FileSizeLimit() As Integer
        Get
            Return m_FileSizeLimit
        End Get
        Set(ByVal value As Integer)
            m_FileSizeLimit = value
        End Set
    End Property

    Public Property FileTypes() As String
        Get
            Return m_FileTypes
        End Get
        Set(ByVal value As String)
            m_FileTypes = value
        End Set
    End Property

    Public Property FileTypesDescription() As String
        Get
            Return m_FileTypesDescription
        End Get
        Set(ByVal value As String)
            m_FileTypesDescription = value
        End Set
    End Property

    Public Property FileUploadLimit() As Integer
        Get
            Return m_FileUploadLimit
        End Get
        Set(ByVal value As Integer)
            m_FileUploadLimit = value
        End Set
    End Property

    Public Property ButtonImageUrl() As String
        Get
            Return m_ButtonImageUrl
        End Get
        Set(ByVal value As String)
            m_ButtonImageUrl = value
        End Set
    End Property

    Public Property ButtonWidth() As Integer
        Get
            Return m_ButtonWidth
        End Get
        Set(ByVal value As Integer)
            m_ButtonWidth = value
        End Set
    End Property

    Public Property ButtonHeight() As Integer
        Get
            Return m_ButtonHeight
        End Get
        Set(ByVal value As Integer)
            m_ButtonHeight = value
        End Set
    End Property

    Public Property UniqueBulkId() As Integer
        Get
            Return m_UniqueBulkId
        End Get
        Set(ByVal value As Integer)
            m_UniqueBulkId = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        PrepareBulkUpload(True)
    End Sub
End Class
