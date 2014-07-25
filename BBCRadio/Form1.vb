﻿Imports System.Media
Imports System.IO
Imports System.Xml
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports System.Net

'important urls:
'http://www.bbc.co.uk/radio/aod/availability/radio1.xml
'http://atlas.metabroadcast.com/#apiExplorer
'http://www.bbc.co.uk/mediaselector/4/asx/b03xzz0v/iplayer_intl_stream_wma_lo_concrete
'http://www.bbc.co.uk/mediaselector/4/mtis/stream/b03xzz0v


Public Class Form1
    Dim final As String

    Dim image As String

    Dim finalx As String

    Dim datetime As String

    Dim s As String

    Dim posupos As Boolean = True


    Dim URL As String
    Dim result As String
    Dim urlpath As String = ""

    Dim urlpathx As String = ""


    Dim showX As String
    Dim artistX As String
    Dim titleX As String

    Dim counter As Integer = 0
    Dim counterx As Integer = 0
    Dim counterxx As Integer = 0

    Dim valueend As Integer



    '--------------------------


    Public Sub getinfo()

        lv()


        Dim document As XmlReader = New XmlTextReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/BBC/live.xml")

        showX = vbNullString


        artistX = vbNullString



        While (document.Read())

            Dim type = document.NodeType

            If (type = XmlNodeType.Element) Then

                If (document.Name = "artist") Then

                    If counter = 0 Then
                        showX = document.ReadInnerXml.ToString()
                        counter = 1
                    Else
                        'don't do anything!
                    End If

                End If

                If (document.Name = "title") Then
                    artistX = document.ReadInnerXml.ToString()
                End If

                If (document.Name = "brand_pid") Then
                    titleX = document.ReadInnerXml.ToString()

                End If

            End If

        End While
        counter = 0
        counterx = 0
        counterxx = 0
        document.Close()

        'convert brand_pid into text
        convbrand_pid()
        showX = showX.Replace("&amp;", "&")

        artistX = artistX.Replace("&amp;", "&")

        titleX = titleX.Replace("&amp;", "&")

        'MsgBox(showX)
        'MsgBox(artistX)
        'MsgBox(titleX)

        '        randomimage()
    End Sub

    Public Sub convbrand_pid()

        Dim document As XmlReader = New XmlTextReader("http://www.bbc.co.uk/programmes/" + titleX + ".xml" + "?t=" + datetime)

        While (document.Read())

            Dim type = document.NodeType

            If (type = XmlNodeType.Element) Then

                If (document.Name = "title") Then

                    If counterxx = 0 Then
                        titleX = document.ReadInnerXml.ToString()
                        counterxx = 1
                    Else
                        'don't do anything!
                    End If

                End If

            End If

        End While

    End Sub

    Public Sub ConvAndSave()
        Dim json As String
        json = result

        Dim node As XNode = JsonConvert.DeserializeXNode(json, "Root")

        Dim objReader As New StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/BBC/live.xml")
        objReader.Write(node.ToString())
        objReader.Close()
    End Sub



    Public Sub lv()
        Dim client As WebClient = New WebClient()
        Try
            ' URL = "http://open.bbci.co.uk/rio/poll/service/bbc_radio_one/10.json"
            URL = "http://polling.bbc.co.uk/radio/realtime/bbc_radio_one.json" + "?t=" + datetime 'DateTime.Now.ToLocalTime()
            result = client.DownloadString(URL)
            'MsgBox(result)
            ConvAndSave()

        Catch ex As Exception

        End Try

    End Sub



    Public Sub randomimage()

        Dim RandomClass As New Random()
        Dim RandomNumber As Integer
        ' For counter As Integer = 0 To 5
        RandomNumber = RandomClass.Next(0, 7)
        '  MsgBox(RandomNumber)

        PictureBox1.ImageLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/BBC/Images/" + CType(RandomNumber, String) + "_large.png"

        'Next
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            BackgroundWorker3.CancelAsync()

            BackgroundWorker3.RunWorkerAsync()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub BackgroundWorker3_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker3.DoWork
        '"<span class=""title-container"">Phil Taggart and Alice Levine</span>"
        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("http://open.bbci.co.uk/rio/poll/service/bbc_radio_one/10.json" + "?t=" + datetime) 'DateTime.Now.ToLocalTime())
        Dim response As System.Net.HttpWebResponse = request.GetResponse()
        urlpath = ""
        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())

        Dim sourcecode As String = sr.ReadToEnd()

        Dim reg = New Regex("""type"":""IMAGE"",""uri"":""(.*?)""")
        Dim s As String
        Dim matches = reg.Matches(sourcecode)
        If sourcecode.Contains("512x512") Then
            For Each mat As Match In matches
                s = mat.Value

                If s.Contains("512x512") Then

                    urlpath = s
                    urlpath = urlpath.Replace(Chr(34), "")
                    urlpath = urlpath.Replace("type:IMAGE,uri:", "")

                    '------------------------------------------ new stuff here

                    valueend = 1


                    '-----------------------------------------------------------
                    '     PictureBox1.ImageLocation = urlpath

                End If
            Next mat

        Else
            valueend = 2


        End If

    End Sub

    Private Sub BackgroundWorker3_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker3.RunWorkerCompleted

        If valueend = 1 Then
            PictureBox1.ImageLocation = urlpath
            Try

                BackgroundWorker4.RunWorkerAsync()

            Catch ex As Exception

            End Try
        End If
        If valueend = 2 Then
            randomimage()
            Try
                BackgroundWorker4.RunWorkerAsync()

            Catch ex As Exception

            End Try

        End If
        '       PictureBox1.ImageLocation = urlpath

        'getinfo()
    End Sub

    Private Sub BackgroundWorker4_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker4.DoWork
        Try

            getinfo()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub BackgroundWorker4_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker4.RunWorkerCompleted
        ToolStripStatusLabel9.Text = artistX
        ToolStripStatusLabel7.Text = showX


        ToolStripStatusLabel2.Text = titleX

        ' Label1.Text = showX
        ' Label2.Text = artistX

        ' Label3.Text = titleX

    End Sub
    '--------------------------




    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ToolStripStatusLabel2.Text = ListBox1.SelectedItem.ToString
        Dim index As Integer
        index = ListBox1.SelectedIndex

        ToolStripStatusLabel4.Text = ListBox3.Items(index)
        '        http://open.live.bbc.co.uk/mediaselector/4/asx/b049b7g7/stream-nonuk-audio_streaming_wma_low_nonuk
        AxWindowsMediaPlayer1.URL = "http://open.live.bbc.co.uk/mediaselector/4/asx/" + ToolStripStatusLabel6.Text + "/stream-nonuk-audio_streaming_wma_low_nonuk"
        BackgroundWorker5.RunWorkerAsync()

    End Sub


    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Dim index As Integer
        index = ListBox1.SelectedIndex
        ToolStripStatusLabel6.Text = ListBox2.Items(index)
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork

        Dim document As XmlReader = New XmlTextReader("http://www.bbc.co.uk/radio/aod/availability/radio1.xml")

        final = vbNullString


        finalx = vbNullString



        While (document.Read())

            Dim type = document.NodeType

            If (type = XmlNodeType.Element) Then

                If (document.Name = "title") Then

                    'TextBox1.Text = TextBox1.Text + document.ReadInnerXml.ToString() + Environment.NewLine
                    final = final + document.ReadInnerXml.ToString() + Environment.NewLine

                End If

                If (document.Name = "links") Then
                    'TextBox1.Text = TextBox1.Text + document.ReadInnerXml.ToString() + Environment.NewLine
                    finalx = finalx + document.ReadInnerXml.ToString() + Environment.NewLine

                End If

                
            End If

        End While
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        ListBox1.Items.Clear()
        ListBox2.Items.Clear()

        finalx = finalx.Replace(Chr(34), "")

        finalx = finalx.Replace("      <link type=mediaselector>", "")
        final = final.Replace("BBC Audio On Demand Availability Schedule" + Environment.NewLine, "")
        final = final.Replace("&amp;", "&")
        finalx = finalx.Replace("    ", "")
        finalx = finalx.Replace("</link>", "")
        finalx = finalx.Replace("http://www.bbc.co.uk/mediaselector/4/mtis/stream/", "")
        ListBox1.Items.AddRange(Split(final, vbCrLf))

        ListBox1.Items.Remove("")

        ListBox2.Items.AddRange(Split(finalx, vbCrLf))
        ListBox2.Items.Remove("")

        
        RefreshListToolStripMenuItem.Enabled = True


    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

        RefreshListToolStripMenuItem.Enabled = False

        BackgroundWorker1.RunWorkerAsync()
        BackgroundWorker2.RunWorkerAsync()
    End Sub

  
    Private Sub Form1_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        End

    End Sub

    Dim td As String


    Dim tdx As String

    Public Sub golive()

    End Sub


    Public Sub gooffline()

    End Sub


    Private Sub LiveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LiveToolStripMenuItem.Click
        AxWindowsMediaPlayer1.URL = "http://www.bbc.co.uk/radio/listen/live/r1.asx"
        ToolStripStatusLabel4.Text = "LIVE"
        BackgroundWorker3.RunWorkerAsync()

    End Sub

    Private Sub RefreshListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshListToolStripMenuItem.Click
        RefreshListToolStripMenuItem.Enabled = False

        BackgroundWorker1.RunWorkerAsync()
        BackgroundWorker2.RunWorkerAsync()
    End Sub

    Private Sub BackgroundWorker2_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork

        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("http://www.bbc.co.uk/radio/aod/availability/radio1.xml" + "?t=" + TimeOfDay.ToString())
        Dim response As System.Net.HttpWebResponse = request.GetResponse()

        Dim urlpath As String = ""
        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())

        Dim sourcecode As String = sr.ReadToEnd()

        Dim reg = New Regex("<availability start=""(.*?)"" end=""(.*?)"" />")

        Dim s As String
        Dim matches = reg.Matches(sourcecode)

        For Each mat As Match In matches
            s = mat.Value


            urlpath = s
            s = s.Replace("""", "")

            s = s.Replace("<availability start=", "")
            Dim subString As String = Microsoft.VisualBasic.Left(s, 10)
            td = td + subString + Environment.NewLine

        Next mat


   
    End Sub

    Private Sub BackgroundWorker2_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        ListBox3.Items.Clear()
        ListBox3.Items.AddRange(Split(td, vbCrLf))
        ListBox3.Items.Remove("")
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub


    Private Sub BackgroundWorker5_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker5.DoWork
      
        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("http://www.bbc.co.uk/radio/aod/availability/radio1.xml" + "?t=" + TimeOfDay.ToString())
        Dim response As System.Net.HttpWebResponse = request.GetResponse()

        Dim urlpath As String = ""
        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())

        Dim sourcecode As String = sr.ReadToEnd()

        Dim reg = New Regex("<parent pid=""(.*?)""")
        '<parent pid="b0080x5m" 
        Dim s As String
        Dim matches = reg.Matches(sourcecode)

        For Each mat As Match In matches
            s = mat.Value


            urlpath = s
            s = s.Replace("""", "")

            s = s.Replace("<parent pid=", "")
            Dim subString As String = Microsoft.VisualBasic.Left(s, 10)
            tdx = tdx + subString + Environment.NewLine

        Next mat


    End Sub

    Private Sub BackgroundWorker5_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker5.RunWorkerCompleted
        ListBox4.Items.Clear()
        ListBox4.Items.AddRange(Split(tdx, vbCrLf))
        ListBox4.Items.Remove("")
        'image location comes here.
        Dim imgname As String

        Dim index As Integer
        index = ListBox1.SelectedIndex
        imgname = ListBox4.Items(index)
        PictureBox1.ImageLocation = "http://www.bbc.co.uk/iplayer/images/progbrand/" + imgname + "_512_288.jpg"
        ' PictureBox1.ImageLocation = "http://ichef.bbci.co.uk/images/ic/272x272/" + imgname + ".jpg"
    End Sub
End Class

'http://www.bbc.co.uk/programmes/b037wxgj.rdf
'http://www.bbc.co.uk/programmes/b037wxgj
'get the rdf and use it to get: http://ichef.bbci.co.uk/images/ic/272x272/p01tbfcn.jpg
'  <po:image_pid>p01tbfcn</po:image_pid>
'http://ichef.bbci.co.uk/images/ic/640x360/p01m23mw.jpg
'http://ichef.bbci.co.uk/images/ic/272x272/p01m23mw.jpg


'<parent pid="b00mbxsh" type="Brand">Fearne Cotton</parent>
'http://www.bbc.co.uk/iplayer/images/progbrand/b00mbxsh_512_288.jpg
'<parent pid="b006wkry" type="Brand">Newsbeat</parent>
'http://www.bbc.co.uk/iplayer/images/progbrand/b006wkry_512_288.jpg
'http://www.bbc.co.uk/radio/aod/availability/radio1.xml
'http://www.bbc.co.uk/iplayer/images/progbrand/p022gdqk_512_288.jpg