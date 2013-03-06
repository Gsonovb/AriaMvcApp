Imports System.IO


Public Class DownloadController
    Inherits System.Web.Mvc.Controller


    Dim _Downloadpath As DirectoryInfo


    ''' <summary>
    ''' 下载目录
    ''' </summary>
    ''' <remarks></remarks>
    Private ReadOnly Property Downloadpath As DirectoryInfo
        Get
            If Me._Downloadpath Is Nothing Then
                Dim temp = ConfigurationManager.AppSettings("downloadpath")

                _Downloadpath = New DirectoryInfo(If(String.IsNullOrWhiteSpace(temp), Server.MapPath("~/Downloads"), temp))

            End If
            Return _Downloadpath
        End Get
    End Property




    Dim _isDeleteToRecycleBin As Nullable(Of Boolean)

    Private ReadOnly Property IsDeleteToRecycleBin As Boolean
        Get
            If Not Me._isDeleteToRecycleBin.HasValue Then
                Dim temp = ConfigurationManager.AppSettings("DeleteToRecycleBin")

                Dim value As Boolean
                If Boolean.TryParse(temp, value) Then
                    Me._isDeleteToRecycleBin = value
                Else
                    Me._isDeleteToRecycleBin = False
                End If
            End If
            Return Me._isDeleteToRecycleBin.Value
        End Get
    End Property





    '
    ' GET: /Download

    Function Index(path As String) As ActionResult

        ViewData("Path") = path

        If Not Me.Downloadpath.Exists() Then
            ViewData("Message") = "指定下载目录不存在，请检查配置文件。"
            Return View(New List(Of FDInfo))
        End If


        Dim cpath = GetCurrentPath(path)

        If String.IsNullOrWhiteSpace(cpath) Then

            ViewData("Message") = "浏览的路径偶问题，自动返回到根目录。"
            Return RedirectToAction("Index")
        End If

        If IO.File.Exists(cpath) Then
            '路径是文件 》》下载文件

            Return File(cpath, GetContentType(IO.Path.GetExtension(cpath)), GetRelativePath(cpath).Substring(1))

        End If


        Dim dir As New DirectoryInfo(cpath)


        Dim list As New List(Of FDInfo)

        If Not dir.FullName.Equals(Me.Downloadpath.FullName, StringComparison.InvariantCultureIgnoreCase) Then

            Dim ppath = GetRelativePath(dir.Parent.FullName)

            list.Add(New FDInfo() With {.Name = "..",
                                        .Size = "",
                                        .Path = ppath,
                                        .CreationTime = dir.Parent.CreationTime})


        End If


        Dim qdirs = dir.EnumerateDirectories.Select(Function(item)

                                                        Dim size = ""
                                                        Dim tpath = GetRelativePath(item.FullName)


                                                        Return New FDInfo With {.Name = item.Name,
                                                                                .Path = tpath,
                                                                                .Size = size,
                                                                                .CreationTime = item.CreationTime
                                                                               }
                                                    End Function).OrderBy(Function(item) item.Name)

        Dim qfiles = dir.EnumerateFiles.Select(Function(item)

                                                   Dim size = GetSize(item)
                                                   Dim tpath = GetRelativePath(item.FullName)


                                                   Return New FDInfo With {.Name = item.Name,
                                                                           .Path = tpath,
                                                                           .Size = size,
                                                                           .CreationTime = item.CreationTime
                                                                          }
                                               End Function).OrderBy(Function(item) item.Name)


        list.AddRange(qdirs)
        list.AddRange(qfiles)

        Return View(list)
    End Function








    Public Function Delete(ByVal path As String) As String

        Try
            Dim cpath = GetCurrentPath(path)

            If String.IsNullOrWhiteSpace(cpath) Then

                Return String.Format(" {0} ERROR!  ", path)
            End If

            If IO.File.Exists(cpath) Then
                '路径是文件 》》下载文件


                If Me.IsDeleteToRecycleBin Then
                    My.Computer.FileSystem.DeleteFile(cpath, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                Else
                    IO.File.Delete(cpath)
                End If


            ElseIf IO.Directory.Exists(cpath) AndAlso Not cpath.Equals(Me.Downloadpath.FullName, StringComparison.InvariantCultureIgnoreCase) Then


                If Me.IsDeleteToRecycleBin Then
                    My.Computer.FileSystem.DeleteDirectory(cpath, FileIO.DeleteDirectoryOption.DeleteAllContents, FileIO.RecycleOption.SendToRecycleBin)

                Else

                    IO.Directory.Delete(cpath, True)
                End If

            Else
                Return String.Format(" {0} ERROR! ", path)

            End If


            Return "OK"


        Catch ex As Exception
            Return "Exception: " & ex.ToString

        End Try


        Return "(?)"
    End Function

    ''
    '' POST: /Download/Delete/5

    '<HttpPost()> _
    'Function Delete(ByVal path As String, ByVal collection As FormCollection) As ActionResult
    '    Try
    '        ' TODO: Add delete logic here

    '        Return RedirectToAction("Index")
    '    Catch
    '        Return View()
    '    End Try
    'End Function



    Private Function GetContentType(ext As String) As String

        Dim tag = ext.ToLower

        Select Case tag

            Case ".atom"
                Return "application/atom+xml"
            Case ".x"
                Return "application/directx"
            Case ".evy"
                Return "application/envoy"
            Case ".fif"
                Return "application/fractals"
            Case ".spl"
                Return "application/futuresplash"
            Case ".hta"
                Return "application/hta"
            Case ".acx"
                Return "application/internet-property-stream"
            Case ".jar"
                Return "application/java-archive"
            Case ".jck", ".jcz"
                Return "application/liquidmotion"
            Case ".hqx"
                Return "application/mac-binhex40"
            Case ".accdb", ".accde", ".accdt"
                Return "application/msaccess"
            Case ".doc", ".dot"
                Return "application/msword"
            Case ".aaf", ".aca", ".afm", ".asd", ".asi", ".bin", ".cab", ".chm", ".csv", ".cur", ".deploy", ".dsp", ".dwp", ".emz", ".eot", ".exe", ".fla", ".hhk", ".hhp", ".ics", ".inf", ".java", ".jpb", ".lpk", ".lzh", ".mdp", ".mix", ".msi", ".mso", ".ocx", ".pcx", ".pcz", ".pfb", ".pfm", ".prm", ".prx", ".psd", ".psm", ".psp", ".qxd", ".rar", ".sea", ".smi", ".snp", ".thn", ".toc", ".ttf", ".u32", ".xsn", ".xtp"
                Return "application/octet-stream"
            Case ".oda"
                Return "application/oda"
            Case ".ods"
                Return "application/oleobject"
            Case ".axs"
                Return "application/olescript"
            Case ".one", ".onea", ".onetoc", ".onetoc2", ".onetmp", ".onepkg"
                Return "application/onenote"
            Case ".osdx"
                Return "application/opensearchdescription+xml"
            Case ".pdf"
                Return "application/pdf"
            Case ".prf"
                Return "application/pics-rules"
            Case ".p10"
                Return "application/pkcs10"
            Case ".p7c", ".p7m"
                Return "application/pkcs7-mime"
            Case ".p7s"
                Return "application/pkcs7-signature"
            Case ".crl"
                Return "application/pkix-crl"
            Case ".ai", ".eps", ".ps"
                Return "application/postscript"
            Case ".rtf"
                Return "application/rtf"
            Case ".setpay"
                Return "application/set-payment-initiation"
            Case ".setreg"
                Return "application/set-registration-initiation"
            Case ".ssm"
                Return "application/streamingmedia"
            Case ".fdf"
                Return "application/vnd.fdf"
            Case ".xla", ".xlc", ".xlm", ".xls", ".xlt", ".xlw"
                Return "application/vnd.ms-excel"
            Case ".xlam"
                Return "application/vnd.ms-excel.addin.macroEnabled.12"
            Case ".xlsb"
                Return "application/vnd.ms-excel.sheet.binary.macroEnabled.12"
            Case ".xlsm"
                Return "application/vnd.ms-excel.sheet.macroEnabled.12"
            Case ".xltm"
                Return "application/vnd.ms-excel.template.macroEnabled.12"
            Case ".calx"
                Return "application/vnd.ms-office.calx"
            Case ".thmx"
                Return "application/vnd.ms-officetheme"
            Case ".sst"
                Return "application/vnd.ms-pki.certstore"
            Case ".pko"
                Return "application/vnd.ms-pki.pko"
            Case ".cat"
                Return "application/vnd.ms-pki.seccat"
            Case ".stl"
                Return "application/vnd.ms-pki.stl"
            Case ".pot", ".pps", ".ppt"
                Return "application/vnd.ms-powerpoint"
            Case ".ppam"
                Return "application/vnd.ms-powerpoint.addin.macroEnabled.12"
            Case ".pptm"
                Return "application/vnd.ms-powerpoint.presentation.macroEnabled.12"
            Case ".sldm"
                Return "application/vnd.ms-powerpoint.slide.macroEnabled.12"
            Case ".ppsm"
                Return "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"
            Case ".potm"
                Return "application/vnd.ms-powerpoint.template.macroEnabled.12"
            Case ".mpp"
                Return "application/vnd.ms-project"
            Case ".vdx"
                Return "application/vnd.ms-visio.viewer"
            Case ".docm"
                Return "application/vnd.ms-word.document.macroEnabled.12"
            Case ".dotm"
                Return "application/vnd.ms-word.template.macroEnabled.12"
            Case ".wcm", ".wdb", ".wks", ".wps"
                Return "application/vnd.ms-works"
            Case ".xps"
                Return "application/vnd.ms-xpsdocument"
            Case ".pptx"
                Return "application/vnd.openxmlformats-officedocument.presentationml.presentation"
            Case ".sldx"
                Return "application/vnd.openxmlformats-officedocument.presentationml.slide"
            Case ".ppsx"
                Return "application/vnd.openxmlformats-officedocument.presentationml.slideshow"
            Case ".potx"
                Return "application/vnd.openxmlformats-officedocument.presentationml.template"
            Case ".xlsx"
                Return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Case ".xltx"
                Return "application/vnd.openxmlformats-officedocument.spreadsheetml.template"
            Case ".docx"
                Return "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Case ".dotx"
                Return "application/vnd.openxmlformats-officedocument.wordprocessingml.template"
            Case ".rm"
                Return "application/vnd.rn-realmedia"
            Case ".rmvb"
                Return "application/vnd.rn-realmedia-vbr"
            Case ".vsd", ".vss", ".vst", ".vsw", ".vsx", ".vtx"
                Return "application/vnd.visio"
            Case ".wmlc"
                Return "application/vnd.wap.wmlc"
            Case ".wmlsc"
                Return "application/vnd.wap.wmlscriptc"
            Case ".hlp"
                Return "application/winhlp"
            Case ".xaml"
                Return "application/xaml+xml"
            Case ".bcpio"
                Return "application/x-bcpio"
            Case ".cdf"
                Return "application/x-cdf"
            Case ".z"
                Return "application/x-compress"
            Case ".tgz"
                Return "application/x-compressed"
            Case ".cpio"
                Return "application/x-cpio"
            Case ".csh"
                Return "application/x-csh"
            Case ".dcr", ".dir", ".dxr"
                Return "application/x-director"
            Case ".dvi"
                Return "application/x-dvi"
            Case ".gtar"
                Return "application/x-gtar"
            Case ".gz"
                Return "application/x-gzip"
            Case ".hdf"
                Return "application/x-hdf"
            Case ".ins", ".isp"
                Return "application/x-internet-signup"
            Case ".iii"
                Return "application/x-iphone"
            Case ".class"
                Return "application/x-java-applet"
            Case ".js"
                Return "application/x-javascript"
            Case ".latex"
                Return "application/x-latex"
            Case ".mvc"
                Return "application/x-miva-compiled"
            Case ".mdb"
                Return "application/x-msaccess"
            Case ".application"
                Return "application/x-ms-application"
            Case ".crd"
                Return "application/x-mscardfile"
            Case ".clp"
                Return "application/x-msclip"
            Case ".dll"
                Return "application/x-msdownload"
            Case ".manifest"
                Return "application/x-ms-manifest"
            Case ".m13", ".m14", ".mvb"
                Return "application/x-msmediaview"
            Case ".wmf"
                Return "application/x-msmetafile"
            Case ".mny"
                Return "application/x-msmoney"
            Case ".pub"
                Return "application/x-mspublisher"
            Case ".lit"
                Return "application/x-ms-reader"
            Case ".scd"
                Return "application/x-msschedule"
            Case ".trm"
                Return "application/x-msterminal"
            Case ".vsto"
                Return "application/x-ms-vsto"
            Case ".wmd"
                Return "application/x-ms-wmd"
            Case ".wmz"
                Return "application/x-ms-wmz"
            Case ".wri"
                Return "application/x-mswrite"
            Case ".xbap"
                Return "application/x-ms-xbap"
            Case ".nc"
                Return "application/x-netcdf"
            Case ".hhc"
                Return "application/x-oleobject"
            Case ".pma", ".pmc", ".pml", ".pmr", ".pmw"
                Return "application/x-perfmon"
            Case ".p12", ".pfx"
                Return "application/x-pkcs12"
            Case ".p7b", ".spc"
                Return "application/x-pkcs7-certificates"
            Case ".p7r"
                Return "application/x-pkcs7-certreqresp"
            Case ".qtl"
                Return "application/x-quicktimeplayer"
            Case ".sh"
                Return "application/x-sh"
            Case ".shar"
                Return "application/x-shar"
            Case ".swf"
                Return "application/x-shockwave-flash"
            Case ".xap"
                Return "application/x-silverlight-app"
            Case ".mmf"
                Return "application/x-smaf"
            Case ".sit"
                Return "application/x-stuffit"
            Case ".sv4cpio"
                Return "application/x-sv4cpio"
            Case ".sv4crc"
                Return "application/x-sv4crc"
            Case ".tar"
                Return "application/x-tar"
            Case ".tcl"
                Return "application/x-tcl"
            Case ".tex"
                Return "application/x-tex"
            Case ".texi", ".texinfo"
                Return "application/x-texinfo"
            Case ".roff", ".t", ".tr"
                Return "application/x-troff"
            Case ".man"
                Return "application/x-troff-man"
            Case ".me"
                Return "application/x-troff-me"
            Case ".ms"
                Return "application/x-troff-ms"
            Case ".ustar"
                Return "application/x-ustar"
            Case ".src"
                Return "application/x-wais-source"
            Case ".crt", ".der"
                Return "application/x-x509-ca-cert"
            Case ".zip"
                Return "application/x-zip-compressed"
            Case ".aifc", ".aiff"
                Return "audio/aiff"
            Case ".au", ".snd"
                Return "audio/basic"
            Case ".mid", ".midi", ".rmi"
                Return "audio/mid"
            Case ".mp3"
                Return "audio/mpeg"
            Case ".wav"
                Return "audio/wav"
            Case ".aif"
                Return "audio/x-aiff"
            Case ".m3u"
                Return "audio/x-mpegurl"
            Case ".wax"
                Return "audio/x-ms-wax"
            Case ".wma"
                Return "audio/x-ms-wma"
            Case ".ra", ".ram"
                Return "audio/x-pn-realaudio"
            Case ".rpm"
                Return "audio/x-pn-realaudio-plugin"
            Case ".smd", ".smx", ".smz"
                Return "audio/x-smd"
            Case ".dwf"
                Return "drawing/x-dwf"
            Case ".bmp", ".dib"
                Return "image/bmp"
            Case ".cod"
                Return "image/cis-cod"
            Case ".gif"
                Return "image/gif"
            Case ".ief"
                Return "image/ief"
            Case ".jpe", ".jpeg", ".jpg"
                Return "image/jpeg"
            Case ".jfif"
                Return "image/pjpeg"
            Case ".png", ".pnz"
                Return "image/png"
            Case ".tif", ".tiff"
                Return "image/tiff"
            Case ".rf"
                Return "image/vnd.rn-realflash"
            Case ".wbmp"
                Return "image/vnd.wap.wbmp"
            Case ".ras"
                Return "image/x-cmu-raster"
            Case ".cmx"
                Return "image/x-cmx"
            Case ".ico"
                Return "image/x-icon"
            Case ".art"
                Return "image/x-jg"
            Case ".pnm"
                Return "image/x-portable-anymap"
            Case ".pbm"
                Return "image/x-portable-bitmap"
            Case ".pgm"
                Return "image/x-portable-graymap"
            Case ".ppm"
                Return "image/x-portable-pixmap"
            Case ".rgb"
                Return "image/x-rgb"
            Case ".xbm"
                Return "image/x-xbitmap"
            Case ".xpm"
                Return "image/x-xpixmap"
            Case ".xwd"
                Return "image/x-xwindowdump"
            Case ".eml", ".mht", ".mhtml", ".nws"
                Return "message/rfc822"
            Case ".css"
                Return "text/css"
            Case ".dlm"
                Return "text/dlm"
            Case ".323"
                Return "text/h323"
            Case ".htm", ".html", ".hxt"
                Return "text/html"
            Case ".uls"
                Return "text/iuls"
            Case ".jsx"
                Return "text/jscript"
            Case ".asm", ".bas", ".c", ".cnf", ".cpp", ".h", ".map", ".txt", ".vcs", ".xdr"
                Return "text/plain"
            Case ".rtx"
                Return "text/richtext"
            Case ".sct"
                Return "text/scriptlet"
            Case ".sgml"
                Return "text/sgml"
            Case ".tsv"
                Return "text/tab-separated-values"
            Case ".vbs"
                Return "text/vbscript"
            Case ".wml"
                Return "text/vnd.wap.wml"
            Case ".wmls"
                Return "text/vnd.wap.wmlscript"
            Case ".htt"
                Return "text/webviewhtml"
            Case ".htc"
                Return "text/x-component"
            Case ".hdml"
                Return "text/x-hdml"
            Case ".disco", ".dll.config", ".dtd", ".exe.config", ".mno", ".vml", ".wsdl", ".xml", ".xsd", ".xsf", ".xsl", ".xslt"
                Return "text/xml"
            Case ".odc"
                Return "text/x-ms-odc"
            Case ".etx"
                Return "text/x-setext"
            Case ".vcf"
                Return "text/x-vcard"
            Case ".mp4"
                Return "video/mp4"
            Case ".m1v", ".mp2", ".mpa", ".mpe", ".mpeg", ".mpg", ".mpv2"
                Return "video/mpeg"
            Case ".mov", ".qt"
                Return "video/quicktime"
            Case ".flv"
                Return "video/x-flv"
            Case ".IVF"
                Return "video/x-ivf"
            Case ".lsf", ".lsx"
                Return "video/x-la-asf"
            Case ".m4v"
                Return "video/x-m4v"
            Case ".asf", ".asr", ".asx", ".nsc"
                Return "video/x-ms-asf"
            Case ".avi"
                Return "video/x-msvideo"
            Case ".wm"
                Return "video/x-ms-wm"
            Case ".wmp"
                Return "video/x-ms-wmp"
            Case ".wmv"
                Return "video/x-ms-wmv"
            Case ".wmx"
                Return "video/x-ms-wmx"
            Case ".wvx"
                Return "video/x-ms-wvx"
            Case ".movie"
                Return "video/x-sgi-movie"
            Case ".flr", ".wrl", ".wrz", ".xaf", ".xof"
                Return "x-world/x-vrml"
        End Select


        Return "application/octet-stream"
    End Function

    ''' <summary>
    ''' 获取文件大小
    ''' </summary>
    ''' <param name="item"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSize(item As FileInfo) As String
        If item Is Nothing Then Return ""

        Dim size = item.Length

        Dim unit = "Byte"

        Select Case size

            Case Is > 2 ^ 50
                unit = "PB"
                size /= 2 ^ 50

            Case Is > 2 ^ 40
                size /= 2 ^ 40
                unit = "TB"
            Case Is > 2 ^ 30
                size /= 2 ^ 30
                unit = "GB"
            Case Is > 2 ^ 20
                size /= 2 ^ 20
                unit = "MB"
            Case Is > 2 ^ 10
                size /= 2 ^ 10
                unit = "KB"

        End Select


        Return String.Format("{0} {1}", size, unit)
    End Function


    ''' <summary>
    ''' 获取相对路径
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetRelativePath(path As String) As String
        If String.IsNullOrWhiteSpace(path) Then Return String.Empty


        Return path.Replace(Me.Downloadpath.FullName, "")
    End Function


    ''' <summary>
    ''' 获取当前路径
    ''' </summary>
    ''' <param name="path">当前路径参数</param>
    ''' <returns>返回指定参数的物理路径。如果有错误，则返回Nothing。</returns>
    ''' <remarks></remarks>
    Private Function GetCurrentPath(path As String) As String
        Dim p = IO.Path.Combine(Me.Downloadpath.FullName, If(String.IsNullOrEmpty(path), "", path)).Replace("/"c, "\"c)

        If Not p.StartsWith(Me.Downloadpath.FullName, StringComparison.InvariantCultureIgnoreCase) Then
            '如果路径不在下载目录下，则返回空

            Return Nothing

        End If

        Return p
    End Function


End Class