Imports Components
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient

Partial Class ViewCacheItems
    Inherits SitePage
    Private ResDb As New Database
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblItems.Text = String.Empty

        Try
            ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            ResDb.GetDataTable("SELECT * FROM RG_ARReport")

            Dim dtTree As New DataTable
            Dim dtTree2 As DataTable = Nothing

            'ResDb.GetDataTable("EXEC RG_BuilderStmtVolumeFee '01/01/2015', '01/31/2015', '2089')
            'Dim param(2) As SqlParameter
            'param(0) = New SqlParameter ("@
            'param(1) = "01/31/2015"
            'param(2) = "2089"

            Dim prams(2) As SqlParameter
            prams(0) = New SqlParameter("@DATEFROM", "01/01/2015")
            prams(1) = New SqlParameter("@DATETO", "01/31/2015")
            prams(2) = New SqlParameter("@BUILDERID", 2089)


            ResDb.RunProc("RG_BuilderStmtVolumeFee", prams, dtTree)

            Dim dtresults As DataTable = dtTree

            lblItems.Text = dtresults.Rows.Count

        Catch ex As Exception

            lblItems.Text = "failed  -->" & ex.ToString
        Finally
            ResDb.Close()

        End Try


        'Dim LabelText As String = String.Empty

        'LabelText &= " Count = " & HttpContext.Current.Cache.Count & "<br />"
        'LabelText &= " EffectivePercentagePhysicalMemoryLimit = " & HttpContext.Current.Cache.EffectivePercentagePhysicalMemoryLimit & "<br />"
        'LabelText &= " EffectivePrivateBytesLimit = " & HttpContext.Current.Cache.EffectivePrivateBytesLimit & "<br />"
        'Dim id As IDictionaryEnumerator = HttpContext.Current.Cache.GetEnumerator
        'While id.MoveNext
        '    LabelText &= "[" & id.Current.ToString & "] " & id.Key
        '    If id.Current.ToString = "System.Collections.DictionaryEntry" Then
        '        LabelText &= " [size = "
        '        If id.Entry.GetType.ToString = "System.Collections.DictionaryEntry" Then
        '        End If
        '        LabelText &= "]"

        '    End If
        '    LabelText &= "<br />"
        '    'CType(id, DataTable).Rows.Count
        '    'LabelText &= CType(id, DataRow).Table.MinimumCapacity & "<br />"


        'End While


    End Sub



End Class
