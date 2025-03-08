Imports System.Diagnostics
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Net
Imports Newtonsoft.Json
Imports System.Text
Imports System.Threading
Public Class Form1
    Dim portnum As String = ""
    Dim outputa As String = ""
    Dim lastNumber As String = ""
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        On Error Resume Next
        With DataGridView1
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60)
            .RowHeadersDefaultCellStyle.ForeColor = Color.White
            .DefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60)
            .DefaultCellStyle.ForeColor = Color.White
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 80, 80)
            .DefaultCellStyle.SelectionForeColor = Color.White
            .GridColor = Color.FromArgb(60, 60, 60)
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        End With
        With DataGridView2
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60)
            .RowHeadersDefaultCellStyle.ForeColor = Color.White
            .DefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60)
            .DefaultCellStyle.ForeColor = Color.White
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 80, 80)
            .DefaultCellStyle.SelectionForeColor = Color.White
            .GridColor = Color.FromArgb(60, 60, 60)
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        End With
        DataGridView1.AutoGenerateColumns = False
        DataGridView1.Columns.Clear()
        DataGridView1.Columns.Add("IPAddress", "IP Address")
        DataGridView1.Columns.Add("Port", "Port")
        DataGridView1.Columns.Add("Country", "Country")
        DataGridView1.Columns.Add("Organization", "Organization")
        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView1.ReadOnly = True
        DataGridView1.AllowUserToAddRows = False
        DataGridView1.AllowUserToDeleteRows = False
        DataGridView1.MultiSelect = False
        DataGridView1.AllowUserToResizeRows = False
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataGridView2.AutoGenerateColumns = False
        DataGridView2.Columns.Clear()
        DataGridView2.Columns.Add("Details", "Connection Details")
        DataGridView2.ReadOnly = True
        DataGridView2.AllowUserToAddRows = False
        DataGridView2.AllowUserToDeleteRows = False
        DataGridView2.AllowUserToResizeRows = False
        DataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        On Error Resume Next
        DataGridView1.Rows.Clear()
        DataGridView1.Rows.Add("Loading...", "", "", "")
        Timer1.Start()
    End Sub
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        On Error Resume Next
        If CheckBox4.Checked = True Then
            If DataGridView1.SelectedRows.Count > 0 Then
                Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ipAddress As String = row.Cells("IPAddress").Value.ToString()
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                Dim request As HttpWebRequest = DirectCast(WebRequest.Create("https://ipinfo.io/" & ipAddress & "?token=" & TextBox1.Text), HttpWebRequest)
                Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                Dim reader As New System.IO.StreamReader(response.GetResponseStream(), UTF8Encoding.UTF8)
                Dim responseText = reader.ReadToEnd()
                Dim responseJson = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(responseText)
                Dim countryRegion = DirectCast(responseJson("region"), String)
                Dim CountryTag = DirectCast(responseJson("country"), String)
                Dim countryName As String = CountryTagConverter.ConvertTag(CountryTag)
                Dim Organization = DirectCast(responseJson("org"), String)
                Label3.Text = ("|| Region : " & countryRegion & " || Country : " & countryName & " || Organization : " & Organization & " ||")
            End If
        End If
        GetProcessInfo()
    End Sub
    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        On Error Resume Next
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ipAddress As String = row.Cells("IPAddress").Value.ToString()
            If CheckBox1.Checked = True Then
                Dim port As String = row.Cells("Port").Value.ToString()
                Dim Result = ipAddress & ":" & port
                Clipboard.SetText(Result)
            Else
                Clipboard.SetText(ipAddress)
            End If
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        On Error Resume Next
        DataGridView1.Rows.Clear()
        DataGridView1.Rows.Add("Loading...", "", "", "")
        DataGridView1.Enabled = False
        Timer2.Start()
    End Sub
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        On Error Resume Next
        If CheckBox1.Checked = True Then
            CheckBox2.Checked = False
        Else
            If CheckBox2.Checked = True Then
            Else
                CheckBox2.Checked = True
            End If
        End If
    End Sub
    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        On Error Resume Next
        If CheckBox2.Checked = True Then
            CheckBox1.Checked = False
        Else
            If CheckBox1.Checked = True Then
            Else
                CheckBox1.Checked = True
            End If
        End If
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        Dim process As New Process()
        Dim startInfo As New ProcessStartInfo()
        startInfo.WindowStyle = ProcessWindowStyle.Hidden
        startInfo.FileName = "cmd.exe"
        startInfo.Arguments = "/C netstat -n"
        startInfo.RedirectStandardOutput = True
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = True
        process.StartInfo = startInfo
        process.Start()
        Dim output As String = process.StandardOutput.ReadToEnd()
        Dim lines() As String = output.Split(vbNewLine)
        Dim uniqueLines As New HashSet(Of String)
        For Each line As String In lines
            If Not uniqueLines.Contains(line) Then
                uniqueLines.Add(line)
            End If
        Next
        Dim filteredLines() As String = uniqueLines.ToArray()
        DataGridView1.Rows.Clear()
        Dim counter As Integer = 1
        For Each line As String In filteredLines
            If line.Length > 0 AndAlso Regex.IsMatch(line, "\d+\.\d+\.\d+\.\d+:\d+") AndAlso line.Contains("TCP") AndAlso Not line.Contains("127.0.0.1") AndAlso Not line.Contains(":443 ") AndAlso Not line.Contains(":80 ") Then
                Dim parts() As String = line.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
                For Each part As String In parts
                    If Regex.IsMatch(part, "\d+\.\d+\.\d+\.\d+:\d+") Then
                        Dim foreignAddress As String = part
                        Dim parts2() As String = foreignAddress.Split(":")
                        Dim ipAddress As String = parts2(0)
                        Dim port As String = parts2(1)
                        If counter Mod 2 = 0 Then
                            If CheckBox3.Checked = True Then
                                Try
                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                    Dim request As HttpWebRequest = DirectCast(WebRequest.Create("https://ipinfo.io/" & ipAddress & "?token=" & TextBox1.Text), HttpWebRequest)
                                    Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                                    Dim reader As New System.IO.StreamReader(response.GetResponseStream(), UTF8Encoding.UTF8)
                                    Dim responseText = reader.ReadToEnd()
                                    Dim responseJson = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(responseText)
                                    Dim CountryTag = DirectCast(responseJson("country"), String)
                                    Dim countryName As String = CountryTagConverter.ConvertTag(CountryTag)
                                    Dim Organization = DirectCast(responseJson("org"), String)
                                    DataGridView1.Rows.Add(ipAddress, port, countryName, Organization)
                                Catch ex As Exception
                                    DataGridView1.Rows.Add(ipAddress, port, "Error", "Error fetching data")
                                End Try
                            Else
                                DataGridView1.Rows.Add(ipAddress, port, "", "")
                            End If
                            If DataGridView1.Rows.Count > 0 Then
                                DataGridView1.FirstDisplayedScrollingRowIndex = DataGridView1.Rows.Count - 1
                            End If
                            Dim pause As New TimeSpan(0, 0, 0, 0, 500)
                            Application.DoEvents()
                            Threading.Thread.Sleep(pause)
                        End If
                        counter += 1
                    End If
                Next
            End If
        Next
        DataGridView1.Enabled = True
    End Sub
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Timer2.Stop()
        Dim process As New Process()
        Dim startInfo As New ProcessStartInfo()
        startInfo.WindowStyle = ProcessWindowStyle.Hidden
        startInfo.FileName = "cmd.exe"
        startInfo.Arguments = "/C netstat -n"
        startInfo.RedirectStandardOutput = True
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = True
        process.StartInfo = startInfo
        process.Start()
        Dim output As String = process.StandardOutput.ReadToEnd()
        Dim lines() As String = output.Split(vbNewLine)
        Dim uniqueLines As New HashSet(Of String)
        For Each line As String In lines
            If Not uniqueLines.Contains(line) Then
                uniqueLines.Add(line)
            End If
        Next
        Dim filteredLines() As String = uniqueLines.ToArray()
        DataGridView1.Rows.Clear()
        Dim counter As Integer = 1
        For Each line As String In filteredLines
            If line.Length > 0 AndAlso Regex.IsMatch(line, "\d+\.\d+\.\d+\.\d+:\d+") AndAlso line.Contains("TCP") AndAlso Not line.Contains("127.0.0.1") Then
                Dim parts() As String = line.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
                For Each part As String In parts
                    If Regex.IsMatch(part, "\d+\.\d+\.\d+\.\d+:\d+") Then
                        Dim foreignAddress As String = part
                        Dim parts2() As String = foreignAddress.Split(":")
                        Dim ipAddress As String = parts2(0)
                        Dim port As String = parts2(1)
                        If counter Mod 2 = 0 Then
                            If CheckBox3.Checked = True Then
                                Try
                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                    Dim request As HttpWebRequest = DirectCast(WebRequest.Create("https://ipinfo.io/" & ipAddress & "?token=" & TextBox1.Text), HttpWebRequest)
                                    Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                                    Dim reader As New System.IO.StreamReader(response.GetResponseStream(), UTF8Encoding.UTF8)
                                    Dim responseText = reader.ReadToEnd()
                                    Dim responseJson = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(responseText)
                                    Dim CountryTag = DirectCast(responseJson("country"), String)
                                    Dim countryName As String = CountryTagConverter.ConvertTag(CountryTag)
                                    Dim Organization = DirectCast(responseJson("org"), String)
                                    DataGridView1.Rows.Add(ipAddress, port, countryName, Organization)
                                Catch ex As Exception
                                    DataGridView1.Rows.Add(ipAddress, port, "Error", "Error fetching data")
                                End Try
                            Else
                                DataGridView1.Rows.Add(ipAddress, port, "", "")
                            End If
                            If DataGridView1.Rows.Count > 0 Then
                                DataGridView1.FirstDisplayedScrollingRowIndex = DataGridView1.Rows.Count - 1
                            End If
                            Dim pause As New TimeSpan(0, 0, 0, 0, 500)
                            Application.DoEvents()
                            Threading.Thread.Sleep(pause)
                        End If
                        counter += 1
                    End If
                Next
            End If
        Next
        DataGridView1.Enabled = True
    End Sub
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        On Error Resume Next
        Process.Start("https://jadadev.com")
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        On Error Resume Next
        Process.Start("https://ipinfo.io/account/home")
    End Sub
    Private Sub GetProcessInfo()
        On Error Resume Next
        DataGridView2.Rows.Clear()
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)
            If row.Cells("Port").Value IsNot Nothing Then
                Dim port As String = row.Cells("Port").Value.ToString()
                portnum = port
            Else
                Return
            End If
        Else
            Return
        End If
        Dim process As New Process()
        Dim startInfo As New ProcessStartInfo()
        startInfo.WindowStyle = ProcessWindowStyle.Hidden
        startInfo.FileName = "cmd.exe"
        startInfo.Arguments = "/C netstat -ano | find " & My.Settings.txt & portnum & My.Settings.txt
        startInfo.RedirectStandardOutput = True
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = True
        process.StartInfo = startInfo
        process.Start()
        outputa = process.StandardOutput.ReadToEnd()
        Timer3.Interval = 100
        Timer3.Start()
    End Sub
    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        Timer3.Stop()
        On Error Resume Next
        If DataGridView1.SelectedRows.Count <= 0 Then Return
        Dim ipAddress As String = ""
        If DataGridView1.SelectedRows(0).Cells("IPAddress").Value IsNot Nothing Then
            ipAddress = DataGridView1.SelectedRows(0).Cells("IPAddress").Value.ToString()
        Else
            Return
        End If
        Dim selectedLine As String = ""
        For Each line As String In outputa.Split(vbCrLf)
            If line.Contains(ipAddress) Then
                lastNumber = line.Substring(line.LastIndexOf(" ") + 1)
                selectedLine = line
            End If
        Next
        If String.IsNullOrEmpty(lastNumber) Then Return
        Dim process As New Process()
        Dim startInfo As New ProcessStartInfo()
        startInfo.WindowStyle = ProcessWindowStyle.Hidden
        startInfo.FileName = "cmd.exe"
        startInfo.Arguments = "/C tasklist /fi " & My.Settings.txt & "pid eq " & lastNumber & My.Settings.txt
        startInfo.RedirectStandardOutput = True
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = True
        process.StartInfo = startInfo
        process.Start()
        Dim output As String = process.StandardOutput.ReadToEnd()
        Dim lines() As String = output.Split(vbCrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        DataGridView2.Rows.Clear()
        DataGridView2.Rows.Add(selectedLine)
        For Each line As String In lines
            DataGridView2.Rows.Add(line)
        Next
    End Sub
    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        On Error Resume Next
        If String.IsNullOrEmpty(lastNumber) Then
            MessageBox.Show("No process selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim startInfo As New ProcessStartInfo()
        startInfo.FileName = "cmd.exe"
        startInfo.Arguments = "/C taskkill /PID " & lastNumber & " /F"
        startInfo.WindowStyle = ProcessWindowStyle.Hidden
        Process.Start(startInfo)

        DataGridView2.Rows.Clear()
        MessageBox.Show("Process with PID " & lastNumber & " has been terminated", "Process Terminated", MessageBoxButtons.OK, MessageBoxIcon.Information)

        If DataGridView1.SelectedRows.Count > 0 Then
            DataGridView1.Rows.Remove(DataGridView1.SelectedRows(0))
        End If
    End Sub
    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView1.DataBindingComplete
        For Each row As DataGridViewRow In DataGridView1.Rows
            If row.Cells("Country").Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(row.Cells("Country").Value.ToString()) Then
                Dim country As String = row.Cells("Country").Value.ToString()
                If country = "United States" Then
                    row.DefaultCellStyle.BackColor = Color.LightBlue
                ElseIf country = "China" Then
                    row.DefaultCellStyle.BackColor = Color.LightPink
                ElseIf country = "Russia" Then
                    row.DefaultCellStyle.BackColor = Color.LightYellow
                End If
            End If
        Next
    End Sub
    Private Sub RemoveDuplicates(dataGridView As DataGridView)
        Dim uniqueRows As New HashSet(Of String)
        For i As Integer = dataGridView.Rows.Count - 1 To 0 Step -1
            Dim rowKey As String = ""
            For Each cell As DataGridViewCell In dataGridView.Rows(i).Cells
                If cell.Value IsNot Nothing Then
                    rowKey &= cell.Value.ToString() & "|"
                Else
                    rowKey &= "null|"
                End If
            Next
            If uniqueRows.Contains(rowKey) Then
                dataGridView.Rows.RemoveAt(i)
            Else
                uniqueRows.Add(rowKey)
            End If
        Next
    End Sub
    Private Sub ExportToCSV()
        On Error Resume Next
        If DataGridView1.Rows.Count = 0 Then
            MessageBox.Show("No data to export", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Dim saveFileDialog As New SaveFileDialog()
        saveFileDialog.Filter = "CSV files (*.csv)|*.csv"
        saveFileDialog.Title = "Export Data to CSV"
        If saveFileDialog.ShowDialog() = DialogResult.OK Then
            Dim sb As New StringBuilder()
            Dim headers As New List(Of String)
            For Each col As DataGridViewColumn In DataGridView1.Columns
                headers.Add(col.HeaderText)
            Next
            sb.AppendLine(String.Join(",", headers))
            For Each row As DataGridViewRow In DataGridView1.Rows
                Dim values As New List(Of String)
                For Each cell As DataGridViewCell In row.Cells
                    Dim value As String = ""
                    If cell.Value IsNot Nothing Then
                        value = cell.Value.ToString()
                        If value.Contains(",") Or value.Contains("""") Or value.Contains(vbLf) Then
                            value = """" & value.Replace("""", """""") & """"
                        End If
                    End If
                    values.Add(value)
                Next
                sb.AppendLine(String.Join(",", values))
            Next
            System.IO.File.WriteAllText(saveFileDialog.FileName, sb.ToString())
            MessageBox.Show("Data exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub bfilelocation_Click(sender As Object, e As EventArgs) Handles bfilelocation.Click
        On Error Resume Next
        If String.IsNullOrEmpty(lastNumber) Then
            MessageBox.Show("No process selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Dim process As New Process()
        Dim startInfo As New ProcessStartInfo()
        startInfo.WindowStyle = ProcessWindowStyle.Hidden
        startInfo.FileName = "cmd.exe"
        startInfo.Arguments = "/C wmic process where ProcessId=" & lastNumber & " get ExecutablePath"
        startInfo.RedirectStandardOutput = True
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = True
        process.StartInfo = startInfo
        process.Start()
        Dim output As String = process.StandardOutput.ReadToEnd()
        Dim lines() As String = output.Split(vbCrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        If lines.Length > 1 Then
            Dim executablePath As String = lines(1).Trim()
            If Not String.IsNullOrEmpty(executablePath) Then
                Dim fileDirectory As String = System.IO.Path.GetDirectoryName(executablePath)
                If System.IO.Directory.Exists(fileDirectory) Then
                    Process.Start("explorer.exe", fileDirectory)
                Else
                    MessageBox.Show("Could not locate the file directory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                MessageBox.Show("Could not determine file location", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            MessageBox.Show("Could not retrieve process information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
End Class
