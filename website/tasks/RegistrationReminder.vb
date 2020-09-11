Imports Components
Imports DataLayer

Public Class RegistrationReminder

    Public Shared Sub Run(ByVal DB As Database)
        Logger.Info("Running RegistrationReminder task...")

        If Now.Date <> New Date(Now.Year, 1, 1) Then
            Exit Sub
        End If

        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "BuilderReRegistrationOpening")
        Dim dtBuilders As DataTable = BuilderRow.GetList(DB)
        For Each row As DataRow In dtBuilders.Rows
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))
            dbMsg.Send(dbBuilder, "http://app.custombuilders-usa.com")
        Next

        dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "VendorReRegistrationOpening")
        Dim dtVendors As DataTable = VendorRow.GetList(DB)
        For Each row As DataRow In dtVendors.Rows
            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, row("VendorId"))
            dbMsg.Send(dbVendor, "http://app.custombuilders-usa.com")
        Next

        Logger.Info("RegistrationReminder task completed.")
    End Sub
End Class
