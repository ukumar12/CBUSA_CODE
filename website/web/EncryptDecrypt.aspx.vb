Imports Utility

Partial Class EncryptDecrypt
    Inherits System.Web.UI.Page

    Private Sub btnEncrypt_Click(sender As Object, e As EventArgs) Handles btnEncrypt.Click
        Dim EncryptedValue As String = Crypt.EncryptTripleDes(txtValueToEncrypt.Text.Trim())
        Me.lblEncryptedText.Text = EncryptedValue
    End Sub

    Private Sub btnDecrypt_Click(sender As Object, e As EventArgs) Handles btnDecrypt.Click
        Dim DecryptedValue As String = Crypt.DecryptTripleDes(txtValueToDecrypt.Text)
        Me.lblDecryptedText.Text = DecryptedValue
    End Sub

End Class
