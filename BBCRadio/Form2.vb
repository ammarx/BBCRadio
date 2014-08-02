Imports System.Text.RegularExpressions

Public Class Form2

    Dim datetime As String
    Dim artist As String
    Dim record_id As String
    Dim pid As String
    Dim snippet_url As String

    Dim sourcecode As String

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("http://open.live.bbc.co.uk/aps/programmes/" + pid + "/segments.json" + "?t=" + datetime) 'DateTime.Now.ToLocalTime())
        Dim response As System.Net.HttpWebResponse = request.GetResponse()
        artist = ""
        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())

        sourcecode = sr.ReadToEnd()



    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        snippet_urlx()

        Rartistx()
        record_idx()
        'track_titlex()
        
    End Sub

    Public Sub Rartistx()
        Dim reg = New Regex("""artist"":""(.*?)"",""track_title"":""(.*?)""")
        Dim s As String
        Dim matches = reg.Matches(sourcecode)
        For Each mat As Match In matches
            s = mat.Value

            artist = s
            artist = artist.Replace(Chr(34), "")
            artist = artist.Replace("artist:", "")
            artist = artist.Replace(",track_title:", " - ")
            ListBox1.Items.Add(artist)

        Next mat

    End Sub

    Public Sub record_idx()
        Dim reg = New Regex("""record_id"":""(.*?)""")
        Dim s As String
        Dim matches = reg.Matches(sourcecode)
        For Each mat As Match In matches
            s = mat.Value

            record_id = s
            record_id = record_id.Replace(Chr(34), "")
            record_id = record_id.Replace("record_id:", "")
            ListBox3.Items.Add(record_id)

        Next mat

    End Sub

    Public Sub snippet_urlx()
        Dim reg = New Regex("""snippet_url"":""(.*?)""")
        Dim s As String
        Dim matches = reg.Matches(sourcecode)
        For Each mat As Match In matches
            s = mat.Value

            snippet_url = s
            ListBox2.Items.Add(snippet_url)

        Next mat

    End Sub
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Dim index As Integer
        index = ListBox1.SelectedIndex
        PictureBox1.ImageLocation = "http://www.bbc.co.uk/music/images/records/96x96/" + ListBox3.Items(index)

        Dim mp3url As String
        mp3url = ListBox2.Items(index)
        mp3url = mp3url.Replace(Chr(34), "")
        mp3url = mp3url.Replace("snippet_url:", "")
        ' MsgBox(mp3url)

        AxWindowsMediaPlayer1.URL = mp3url

        'http://www.bbc.co.uk/music/images/records/96x96/nzqpxx

    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        AxWindowsMediaPlayer1.enableContextMenu = False
        pid = Form1.AxWindowsMediaPlayer1.URL
        pid = pid.Replace("http://open.live.bbc.co.uk/mediaselector/4/asx/", "")
        pid = pid.Replace("/stream-nonuk-audio_streaming_wma_low_nonuk", "")
        pid = pid.Trim()

        'MsgBox(pid)
        BackgroundWorker1.RunWorkerAsync()

    End Sub

End Class