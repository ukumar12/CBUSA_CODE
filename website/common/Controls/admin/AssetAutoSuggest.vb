Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls

Namespace Controls

    ''' <summary>
    ''' Represents a variant of the <see cref="AutoSuggest" /> class designed for use with assets.
    ''' </summary>
    ''' <remarks>This class, which derives from <see cref="AutoSuggest" /> differs from that class in that it
    ''' specifies a specific <see cref="AssetAutoSuggest.AssetTypeId" /> and 
    ''' <see cref="AssetAutoSuggest.AssetToolId" /> by which to filter the results of the web service query.
    ''' <seealso cref="AutoSuggestResult" /></remarks>
    Public Class AssetAutoSuggest
        Inherits AutoSuggest

        ''' <summary>
        ''' Gets or sets the <see cref="DataLayer.AssetTypeRow.AssetTypeId" /> by which the web service query should
        ''' be filtered.
        ''' </summary>
        ''' <value>A <see cref="String" /> that is passed through the JavaScript code to the web service in
        ''' order to filter the autosuggest results.</value>
        ''' <remarks>Make sure this is set to a valid <see cref="DataLayer.AssetTypeRow.AssetTypeId" /> (i.e. 
        ''' images, files, or streaming media).</remarks>
        Public Property AssetTypeId() As String
            Get
                Return ViewState("AssetTypeId")
            End Get
            Set(ByVal value As String)
                ViewState("AssetTypeId") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the <see cref="DataLayer.WorkflowToolRow.ToolId" /> by which the web service query 
        ''' should be filtered.
        ''' </summary>
        ''' <value>A <see cref="String" /> that is passed through the JavaScript code to the web service in
        ''' order to filter the autosuggest results.</value>
        ''' <remarks>Make sure this is set to a valid <see cref="DataLayer.WorkflowToolRow.ToolId" /> such as
        ''' could be retrieved by 
        ''' <see cref="DataLayer.WorkflowToolRow.GetRowByCode">WorkflowToolRow.GetRowByCode.</see>.</remarks>
        Public Property AssetToolId() As String
            Get
                Return ViewState("AssetToolId")
            End Get
            Set(ByVal value As String)
                ViewState("AssetToolId") = value
            End Set
        End Property

        ''' <summary>
        ''' Generates the <see cref="ScriptControlDescriptor">ScriptControlDescriptors</see> used to store the
        ''' properties on the client side that are necessary in order to make the JavaScript work.
        ''' </summary>
        ''' <returns>A single <see cref="ScriptControlDescriptor" /> which represents the 
        ''' <see cref="AutoSuggest" /> class.</returns>
        ''' <remarks>This method allows the AutoSuggest.js file the information it needs in order to 
        ''' operate the essentials of the autosuggest control.</remarks>
        Public Overrides Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor)
            Dim s As New ScriptControlDescriptor("AmericanEagle.AutoSuggest", ClientID)

            s.AddElementProperty("divSuggest", divSuggest.ClientID)
            s.AddElementProperty("txtDisplay", txtDisplay.ClientID)
            s.AddElementProperty("txtSuggest", txtSuggest.ClientID)
            s.AddElementProperty("pnlResults", pnlResults.ClientID)
            s.AddElementProperty("lnkClose", lnkClose.ClientID)
            s.AddElementProperty("hdnValue", hdnValue.ClientID)

            'If TypeOf Page Is Components.BasePage Then
            '    s.AddProperty("SiteId", CType(Page, Components.BasePage).1)
            'End If
            s.AddProperty("ServicePath", ServicePath)
            s.AddProperty("ServiceMethod", ServiceMethod)

            s.AddProperty("AssetTypeId", AssetTypeId)
            s.AddProperty("AssetToolId", AssetToolId)

            If AutoPostBack Then
                s.AddProperty("PostBack", Page.ClientScript.GetPostBackEventReference(Me, Nothing))
            End If

            s.AddScriptProperty("NumResults", NumResults)

            Return New ScriptDescriptor() {s}
        End Function
    End Class
End Namespace
