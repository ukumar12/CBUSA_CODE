Imports System.Diagnostics
Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

Namespace Utility
    Public Class Crypt

        '24 byte or 192 bit key for TripleDES
        Private KEY_192(24) As Byte
        '8 byte or 64 bit IV for TripleDES
        Private IV_192(8) As Byte
        Private bKeyLoaded As Boolean = False

        'Default constructor
        Public Sub New()
        End Sub

        Public Shared ReadOnly Property WebsiteKey() As String
            Get
                Return System.Configuration.ConfigurationManager.AppSettings("GlobalWebsiteKey")
            End Get
        End Property

        Public Shared ReadOnly Property Password() As String
            Get
                Return System.Configuration.ConfigurationManager.AppSettings("GlobalWebsitePassword")
            End Get
        End Property

        Public Shared ReadOnly Property PasswordEx() As String
            Get
                Return System.Configuration.ConfigurationManager.AppSettings("GlobalWebsitePasswordEx")
            End Get
        End Property

        Public Function HexValuesToByteArray24(ByVal str As String) As Byte()
            Dim result(23) As Byte
            Dim j As Integer = 0

            For i As Integer = 1 To Len(str) Step 2
                result(j) = CInt("&H" & Mid(str, i, 2))
                j += 1
            Next
            Return result
        End Function

        Public Function HexValuesToByteArray8(ByVal str As String) As Byte()
            Dim result(7) As Byte
            Dim j As Integer = 0

            For i As Integer = 1 To Len(str) Step 2
                result(j) = CInt("&H" & Mid(str, i, 2))
                j += 1
            Next
            Return result
        End Function

        Private Function GetSalt(ByVal str As String, ByVal Password As String) As String
            Dim encoding As New UnicodeEncoding
            Dim hashBytes As Byte() = encoding.GetBytes(str & Password)

            ' Compute the SHA-1 hash
            Dim sha1 As New SHA1CryptoServiceProvider
            Dim cryptPassword As Byte() = sha1.ComputeHash(hashBytes)
            Return UnicodeEncoding.ASCII.GetString(cryptPassword)
        End Function

        Private Sub EnsureKey()
            If bKeyLoaded Then
                Exit Sub
            End If
            Dim Key As String = "5C50F250F21134FC5ECF4C5EA8EA313BB1F21B41AB35A5CF"
            KEY_192 = HexValuesToByteArray24(Key)

            Dim IVKey As String = "37C7197380B1FB71"
            IV_192 = HexValuesToByteArray8(IVKey)

            bKeyLoaded = True
        End Sub

        'TRIPLE DES encryption
        Public Function Encrypt(ByVal value As String) As String
            EnsureKey()
            If Not value = String.Empty Then
                Dim cryptoProvider As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider
                Dim ms As MemoryStream = New MemoryStream
                Dim cs As CryptoStream = New CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_192, IV_192), CryptoStreamMode.Write)
                Dim sw As StreamWriter = New StreamWriter(cs)

                sw.Write(value & GetSalt(WebsiteKey, Password))
                sw.Flush()
                cs.FlushFinalBlock()
                ms.Flush()

                'convert back to a string
                Return Convert.ToBase64String(ms.GetBuffer(), 0, ms.Length)
            Else
                Return String.Empty
            End If
        End Function

        'TRIPLE DES decryption
        Public Function Decrypt(ByVal value As String) As String
            EnsureKey()
            If Not value = String.Empty Then
                Dim cryptoProvider As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider

                'convert from string to byte array
                Dim buffer As Byte() = Convert.FromBase64String(value)
                Dim ms As MemoryStream = New MemoryStream(buffer)
                Dim cs As CryptoStream = New CryptoStream(ms, cryptoProvider.CreateDecryptor(KEY_192, IV_192), CryptoStreamMode.Read)
                Dim sr As StreamReader = New StreamReader(cs)

                Dim val As String = sr.ReadToEnd()
                val = Left(val, Len(val) - Len(GetSalt(WebsiteKey, Password)))

                Return val
            Else
                Return String.Empty
            End If
        End Function

        Public Shared Function EncryptTripleDes(ByVal value As String) As String
            Dim c As New Crypt()
            Dim r As String = c.Encrypt(value)
            c = Nothing
            Return r
        End Function

        Public Shared Function DecryptTripleDes(ByVal value As String) As String
            Dim c As New Crypt()
            Dim r = c.Decrypt(value)
            c = Nothing
            Return r
        End Function

        Public Shared Function EncryptTripleDesEx(ByVal value As String, ByVal hash As String) As String
            If value = String.Empty Then Return String.Empty
            Dim c As New Crypt()
            Dim r As String = c.Encrypt(value)
            c = Nothing
            Return r
        End Function

        Public Shared Function DecryptTripleDesEx(ByVal value As String, ByVal hash As String) As String
            Dim c As New Crypt()
            Dim r = c.Decrypt(value)
            c = Nothing
            Return r
        End Function

    End Class
End Namespace