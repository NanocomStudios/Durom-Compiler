Module PreProcessor

    Dim compilerPath As String = AppDomain.CurrentDomain.BaseDirectory()
    Dim filePath As String = IO.Directory.GetCurrentDirectory() & "\"

    Sub preProcess(ByVal path As String)
        Dim lines() As String = IO.File.ReadAllLines(path)

        If (IO.File.Exists(filePath + "prp.drm")) Then
            My.Computer.FileSystem.DeleteFile(filePath + "prp.drm")
        End If

        For Each line As String In lines
            If line.StartsWith("#include") Then
                Dim includePath As String = line.Replace("#include", "").Trim()
                If (includePath.StartsWith("<")) Then
                    includePath = compilerPath & "lib\" & includePath
                Else
                    includePath = filePath & includePath
                End If

                includePath = includePath.Replace("""", "")
                includePath = includePath.Replace("<", "")
                includePath = includePath.Replace(">", "")

                Console.WriteLine("Including file: " & includePath)

                If My.Computer.FileSystem.FileExists(includePath) Then
                    Dim includeLines() As String = IO.File.ReadAllLines(includePath)
                    For Each includeLine As String In includeLines
                        My.Computer.FileSystem.WriteAllText(filePath + "prp.drm", (includeLine + Chr(13)), True)
                    Next
                Else
                    Console.WriteLine("Include file not found: " & includePath)
                    End
                End If
            ElseIf line.StartsWith("//") Then
                ' Skip comment lines
            Else
                My.Computer.FileSystem.WriteAllText(filePath + "prp.drm", (line + Chr(13)), True)
            End If
        Next
    End Sub
End Module
