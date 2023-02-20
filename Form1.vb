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
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        On Error Resume Next
        ListBox1.Items.Clear()
        ListBox1.Items.Add("Loading...")
        Timer1.Start()
    End Sub
    Private Sub ListBox1_OneClick(sender As Object, e As EventArgs) Handles ListBox1.Click
        On Error Resume Next
        If CheckBox4.Checked = True Then
            Dim line As String = ListBox1.SelectedItem.ToString()
            Dim parts() As String = line.Split("|")
            Dim ipAddress As String = parts(0).Trim().Substring("IP Address : ".Length)
            Dim NoSpaceIP As String = ipAddress.Replace(" ", "")
            Dim IP As String = NoSpaceIP
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
        Button4_Click(sender, e)
    End Sub
    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        On Error Resume Next
        If CheckBox1.Checked = True Then
            If ListBox1.SelectedIndex >= 0 Then
                Dim line As String = ListBox1.SelectedItem.ToString()
                Dim parts() As String = line.Split("|")
                Dim ipAddress As String = parts(0).Trim().Substring("IP Address : ".Length)
                Dim port As String = parts(1).Trim().Substring("Port : ".Length)
                Dim NoSpaceIP As String = ipAddress.Replace(" ", "")
                Dim NoSpacePort As String = port.Replace(" ", "")
                Dim Result = NoSpaceIP & ":" & NoSpacePort
                Clipboard.SetText(Result.Split("Country : ")(0))
            End If
        Else
            If ListBox1.SelectedIndex >= 0 Then
                Dim line As String = ListBox1.SelectedItem.ToString()
                Dim parts() As String = line.Split("|")
                Dim ipAddress As String = parts(0).Trim().Substring("IP Address : ".Length)
                Dim NoSpaceIP As String = ipAddress.Replace(" ", "")
                Clipboard.SetText(NoSpaceIP)
            End If
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        On Error Resume Next
        ListBox1.Items.Clear()
        ListBox1.Items.Add("Loading...")
        ListBox1.Enabled = False
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
        On Error Resume Next
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
        ListBox1.Items.Clear()
        Dim counter As Integer = 1
        For Each line As String In filteredLines
            If line.Length > 0 AndAlso Regex.IsMatch(line, "\d+\.\d+\.\d+\.\d+:\d+") AndAlso line.Contains("TCP") AndAlso Not line.Contains("127.0.0.1") AndAlso Not line.Contains(":443 ") AndAlso Not line.Contains(":80 ") Then
                Dim parts() As String = line.Split(" ")
                For Each part As String In parts
                    If Regex.IsMatch(part, "\d+\.\d+\.\d+\.\d+:\d+") Then
                        Dim foreignAddress As String = part
                        Dim parts2() As String = foreignAddress.Split(":")
                        Dim ipAddress As String = parts2(0)
                        Dim port As String = parts2(1)
                        If counter Mod 2 = 0 Then
                            If CheckBox3.Checked = True Then
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                Dim request As HttpWebRequest = DirectCast(WebRequest.Create("https://ipinfo.io/" & ipAddress & "?token=" & TextBox1.Text), HttpWebRequest)
                                Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                                Dim reader As New System.IO.StreamReader(response.GetResponseStream(), UTF8Encoding.UTF8)
                                Dim responseText = reader.ReadToEnd()
                                Dim responseJson = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(responseText)
                                Dim CountryTag = DirectCast(responseJson("country"), String)
                                Dim countryName As String = CountryTagConverter.ConvertTag(CountryTag)
                                Dim Organization = DirectCast(responseJson("org"), String)
                                Dim maxIPLength As Integer = 12
                                Dim maxPortLength As Integer = 5
                                Dim spacesIP As String
                                If maxIPLength >= ipAddress.Length Then
                                    spacesIP = New String(" "c, maxIPLength - ipAddress.Length)
                                Else
                                    spacesIP = "   "
                                End If
                                Dim spacesPort As String
                                If maxPortLength >= port.Length Then
                                    spacesPort = New String(" "c, maxPortLength - port.Length)
                                Else
                                    spacesPort = "   "
                                End If
                                ListBox1.BeginUpdate()
                                ListBox1.Items.Add("IP Address : " & ipAddress & spacesIP & "| Port : " & port & spacesPort & "| Country : " & countryName & " | Organization : " & Organization)
                                ListBox1.EndUpdate()
                            Else
                                Dim maxIPLength As Integer = 12
                                Dim maxPortLength As Integer = 5
                                Dim spacesIP As String
                                If maxIPLength >= ipAddress.Length Then
                                    spacesIP = New String(" "c, maxIPLength - ipAddress.Length)
                                Else
                                    spacesIP = "   "
                                End If
                                Dim spacesPort As String
                                If maxPortLength >= port.Length Then
                                    spacesPort = New String(" "c, maxPortLength - port.Length)
                                Else
                                    spacesPort = "   "
                                End If
                                ListBox1.BeginUpdate()
                                ListBox1.Items.Add("IP Address : " & ipAddress & spacesIP & "| Port : " & port & spacesPort)
                                ListBox1.EndUpdate()

                            End If
                            Dim pause As New TimeSpan(0, 0, 0.5)
                            For i As Integer = 0 To ListBox1.Items.Count - 1
                                Application.DoEvents()
                                Threading.Thread.Sleep(pause)
                            Next
                        End If
                        counter = counter + 1
                    End If
                Next
            End If
        Next
        ListBox1.Enabled = True
    End Sub
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        On Error Resume Next
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
        ListBox1.Items.Clear()
        Dim counter As Integer = 1
        For Each line As String In filteredLines
            If line.Length > 0 AndAlso Regex.IsMatch(line, "\d+\.\d+\.\d+\.\d+:\d+") AndAlso line.Contains("TCP") AndAlso Not line.Contains("127.0.0.1") Then
                Dim parts() As String = line.Split(" ")
                For Each part As String In parts
                    If Regex.IsMatch(part, "\d+\.\d+\.\d+\.\d+:\d+") Then
                        Dim foreignAddress As String = part
                        Dim parts2() As String = foreignAddress.Split(":")
                        Dim ipAddress As String = parts2(0)
                        Dim port As String = parts2(1)
                        If counter Mod 2 = 0 Then
                            If CheckBox3.Checked = True Then
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                Dim request As HttpWebRequest = DirectCast(WebRequest.Create("https://ipinfo.io/" & ipAddress & "?token=" & TextBox1.Text), HttpWebRequest)
                                Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                                Dim reader As New System.IO.StreamReader(response.GetResponseStream(), UTF8Encoding.UTF8)
                                Dim responseText = reader.ReadToEnd()
                                Dim responseJson = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(responseText)
                                Dim CountryTag = DirectCast(responseJson("country"), String)
                                Dim countryName As String = CountryTagConverter.ConvertTag(CountryTag)
                                Dim Organization = DirectCast(responseJson("org"), String)
                                Dim maxIPLength As Integer = 12
                                Dim maxPortLength As Integer = 5
                                Dim spacesIP As String
                                If maxIPLength >= ipAddress.Length Then
                                    spacesIP = New String(" "c, maxIPLength - ipAddress.Length)
                                Else
                                    spacesIP = "   "
                                End If
                                Dim spacesPort As String
                                If maxPortLength >= port.Length Then
                                    spacesPort = New String(" "c, maxPortLength - port.Length)
                                Else
                                    spacesPort = "   "
                                End If
                                ListBox1.BeginUpdate()
                                ListBox1.Items.Add("IP Address : " & ipAddress & spacesIP & "| Port : " & port & spacesPort & "| Country : " & countryName & " | Organization : " & Organization)
                                ListBox1.EndUpdate()
                            Else
                                Dim maxIPLength As Integer = 12
                                Dim maxPortLength As Integer = 5
                                Dim spacesIP As String
                                If maxIPLength >= ipAddress.Length Then
                                    spacesIP = New String(" "c, maxIPLength - ipAddress.Length)
                                Else
                                    spacesIP = "   "
                                End If
                                Dim spacesPort As String
                                If maxPortLength >= port.Length Then
                                    spacesPort = New String(" "c, maxPortLength - port.Length)
                                Else
                                    spacesPort = "   "
                                End If
                                ListBox1.BeginUpdate()
                                ListBox1.Items.Add("IP Address : " & ipAddress & spacesIP & "| Port : " & port & spacesPort)
                                ListBox1.EndUpdate()

                            End If
                            Dim pause As New TimeSpan(0, 0, 0.5)
                            For i As Integer = 0 To ListBox1.Items.Count - 1
                                Application.DoEvents()
                                Threading.Thread.Sleep(pause)
                            Next
                        End If
                        counter = counter + 1
                    End If
                Next
            End If
        Next
        ListBox1.Enabled = True
    End Sub
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        On Error Resume Next
        Process.Start("https://jadadev.com")
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.EnabledChanged
        On Error Resume Next
        Dim unique As New HashSet(Of String)
        For i As Integer = ListBox1.Items.Count - 1 To 0 Step -1
            If unique.Contains(ListBox1.Items(i).ToString()) Then
                ListBox1.Items.RemoveAt(i)
            Else
                unique.Add(ListBox1.Items(i).ToString())
            End If
        Next
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        On Error Resume Next
        Process.Start("https://ipinfo.io/account/home")
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs)
        On Error Resume Next
        ListBox2.Items.Clear()
        If ListBox1.SelectedIndex >= 0 Then
            Dim line As String = ListBox1.SelectedItem.ToString()
            Dim parts() As String = line.Split("|")
            Dim port As String = parts(1).Trim().Substring("Port : ".Length)
            Dim NoSpacePort As String = port.Replace(" ", "")
            Dim Result = NoSpacePort
            portnum = (Result.Split("Country : ")(0))
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
        Dim selectedItem As String = ListBox1.SelectedItem.ToString()
        Dim selectedLine As String = ""
        Dim ipAddress As String = Regex.Match(selectedItem, "\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b\s*\|").Value.TrimEnd("|"c, " "c)
        For Each line As String In outputa.Split(vbCrLf)
            If line.Contains(ipAddress) Then
                lastNumber = line.Substring(line.LastIndexOf(" ") + 1)
                selectedLine = line
            End If
        Next
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
        ListBox2.Items.Add(selectedLine)
        For Each line As String In lines
            ListBox2.Items.Add(line)
        Next
    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        On Error Resume Next
        Dim startInfo As New ProcessStartInfo()
        startInfo.FileName = "cmd.exe"
        startInfo.Arguments = "/C taskkill /PID " & lastNumber & " /F"
        startInfo.WindowStyle = ProcessWindowStyle.Hidden
        Process.Start(startInfo)
        ListBox2.Items.Clear()
    End Sub
End Class
