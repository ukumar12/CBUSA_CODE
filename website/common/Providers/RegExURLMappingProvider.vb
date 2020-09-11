Imports System
Imports System.Configuration
Imports System.Configuration.Provider
Imports DataLayer

Namespace Components

    Public Class RegExURLMappingProvider
        Inherits URLMappingProvider

        Public Overrides Function GetURL(ByVal DB As Database, ByVal Url As String) As String
            'Ignore any .axd pages
            If Core.GetFileExtension(Url).ToLower = ".axd" Then
                Return String.Empty
            End If

            Url = Replace(Url, "~", "")

            'if the page exists in the custom url history table, rewrite to the 301 page, to perform a 301 redirect.
            Dim dbCustomURLHistory As DataLayer.CustomURLHistoryRow = DataLayer.CustomURLHistoryRow.GetFromCustomURL(DB, Url)
            If dbCustomURLHistory.CustomURLHistoryId <> 0 Then
                Return "~/301.aspx"
            End If

            Dim dbStoreDepartment As StoreDepartmentRow = StoreDepartmentRow.GetRowByCustomURL(DB, Url)
            If dbStoreDepartment.DepartmentId <> 0 Then
                Dim DefaultDepartmentId As Integer = StoreDepartmentRow.GetDefaultDepartmentId(DB)
                If dbStoreDepartment.ParentId = DefaultDepartmentId Then
                    Return "~/store/main.aspx?DepartmentId=" & dbStoreDepartment.DepartmentId
                Else
                    Return "~/store/default.aspx?DepartmentId=" & dbStoreDepartment.DepartmentId
                End If
            End If

            Dim dbStoreBrand As StoreBrandRow = StoreBrandRow.GetRowByCustomURL(DB, Url)
            If dbStoreBrand.BrandId <> 0 Then
                Return "~/store/brand.aspx?BrandId=" & dbStoreBrand.BrandId
            End If

            Dim dbStoreItem As StoreItemRow = StoreItemRow.GetRowByCustomURL(DB, Url)
            If dbStoreItem.ItemId <> 0 Then
                Return "~/store/item.aspx?ItemId=" & dbStoreItem.ItemId
            End If

            Dim dbContentToolPage As ContentToolPageRow = ContentToolPageRow.GetRowByCustomURL(DB, Url)
            If dbContentToolPage.PageId <> 0 Then
                Return "~" & dbContentToolPage.PageURL
            End If
            Return String.Empty
        End Function

        Public Overrides Function IsValidURL(ByVal DB As Database, ByVal Url As String) As Boolean
            If GetURL(DB, Url) = String.Empty Then
                Return True
            End If
            Return False
        End Function

    End Class
End Namespace