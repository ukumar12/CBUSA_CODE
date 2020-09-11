<%@ Application Language="VB" %>

<script runat="server">

    'Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
    '    ' Code that runs on application startup
    'End Sub

    'Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
    '    ' Code that runs on application shutdown
    'End Sub

    'Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
    '    ' Code that runs when an unhandled error occurs
    'End Sub

    'Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
    '    ' Code that runs when a new session is started
    'End Sub

    'Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
    '    ' Code that runs when a session ends. 
    '    ' Note: The Session_End event is raised only when the sessionstate mode
    '    ' is set to InProc in the Web.config file. If session mode is set to StateServer 
    '    ' or SQLServer, the event is not raised.
    'End Sub
    
    Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
        If HttpContext.Current.Items("GlobalDB") IsNot Nothing Then
            Dim db As Database = HttpContext.Current.Items("GlobalDB")
            db.Close()
            db = Nothing
            HttpContext.Current.Items.Remove("GlobalDB")
        End If
        
        Dim SearchDataDirectory As String = System.Configuration.ConfigurationManager.AppSettings("SearchIndexDirectory")
        Dim SearchIndexName As String = System.Configuration.ConfigurationManager.AppSettings("SearchIndexName")
        If HttpContext.Current.Items("Searcher" & SearchDataDirectory & "\Live\" & SearchIndexName & "\data") IsNot Nothing Then
            Dim searcher As Lucene.Net.Search.IndexSearcher = HttpContext.Current.Items("Searcher" & SearchDataDirectory & "\Live\" & SearchIndexName & "\data")
            searcher.Close()
            searcher = Nothing
            HttpContext.Current.Items.Remove("Searcher" & SearchDataDirectory & "\Live\" & SearchIndexName & "\data")
            HttpContext.Current.Cache.Remove("Searcher" & SearchDataDirectory & "\Live\" & SearchIndexName & "\data")
        End If
        
        Dim keys(HttpContext.Current.Items.Keys.Count - 1) As Object
        HttpContext.Current.Items.Keys.CopyTo(keys, 0)
        For i As Integer = 0 To keys.Length - 1
            HttpContext.Current.Items.Remove(keys(i))
        Next
    End Sub

</script>