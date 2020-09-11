Imports System.DirectoryServices
Imports System.Configuration.ConfigurationManager

Namespace Components

    Public Class ActiveDirectory

        Private Shared Function IsInGroup(ByVal Searcher As DirectorySearcher, ByVal binSid As Byte(), ByVal aGroups As String())
            Searcher.Filter = "(&(objectcategory=group)(objectsid=" + BinaryEncodeSid(binSid) + "))"
            Dim rc As SearchResultCollection = Searcher.FindAll()
            Dim bFound As Boolean = False
            For Each rs As System.DirectoryServices.SearchResult In rc
                Dim DistinguishedName As String = rs.Properties("distinguishedname")(0).ToString()
                For i As Integer = LBound(aGroups) To UBound(aGroups)
                    If DistinguishedName.IndexOf("CN=" & aGroups(i) & ",") = 0 Then
                        bFound = True
                        Exit For
                    End If
                Next
                If bFound Then Exit For
            Next
            rc.Dispose()

            Return bFound
        End Function

        Private Shared Function IsMemberOfDomainGroup(ByVal User As DirectoryEntry, ByVal Searcher As DirectorySearcher, ByVal Groups As String)
            Dim aGroups As String() = Groups.Split(","c)
            Dim bFound As Boolean = False

            User.RefreshCache(New String() {"tokengroups"})
            Searcher.PropertiesToLoad.Add("distinguishedName")
            Dim sids As Object = User.Properties("tokengroups").Value
            If TypeOf sids(0) Is Byte() Then
                For Each binSid As Byte() In CType(sids, IEnumerable)
                    bFound = IsInGroup(Searcher, binSid, aGroups)
                    If bFound Then Exit For
                Next
            Else
                bFound = IsInGroup(Searcher, sids, aGroups)
            End If
            Return bFound
        End Function

        Private Shared Function BinaryEncodeSid(ByVal bSid As Byte()) As String
            Dim sb As System.Text.StringBuilder = New System.Text.StringBuilder
            For i As Integer = 0 To bSid.Length - 1
                sb.AppendFormat("\{0:x2}", bSid(i))
            Next
            BinaryEncodeSid = sb.ToString()
        End Function

        Public Shared Function ValidateAdminCredentials(ByVal Username As String, ByVal Password As String, ByVal Groups As String) As Boolean
            Dim LDAPQuery As String = AppSettings("LDAPQuery")
            Try
                Dim Entry As DirectoryEntry = New DirectoryEntry(LDAPQuery, Username, Password, AuthenticationTypes.Secure)
                Entry.RefreshCache()

                Dim Searcher As DirectorySearcher = New DirectorySearcher()
                Searcher.SearchRoot = Entry
                Searcher.Filter = "(&(objectClass=user)(samAccountName=" + Username + "))"
                Dim Results As SearchResultCollection = Searcher.FindAll()
                If Results.Count > 0 Then
                    Return IsMemberOfDomainGroup(Results(0).GetDirectoryEntry(), Searcher, Groups)
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

    End Class
End Namespace
