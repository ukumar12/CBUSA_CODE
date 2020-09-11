Namespace Components
    Public Interface IAssetLookup
        ''' <summary>
        ''' Gets or sets the current AssetId of the <see cref="IAssetLookup" />.
        ''' </summary>
        ''' <value>An AssetId of a file.</value>
        ''' <remarks>See AssetLookup.ascx for an example implementation, and SitePage.vb for an example use.</remarks>
        Property AssetId() As Integer
        ''' <summary>
        ''' Gets or sets the current Extension <see cref="IAssetLookup" />  
        ''' </summary>
        ''' <value>File Extension of Asset File </value> 
        ''' <remarks>See AssetLookup.ascx for an example implementation, and SitePage.vb for an example use.</remarks>
        Property AssetExtension() As String
    End Interface
End Namespace