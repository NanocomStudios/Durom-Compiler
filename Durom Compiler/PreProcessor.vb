Module PreProcessor

    Dim compilerPath As String = AppDomain.CurrentDomain.BaseDirectory()
    Dim filePath As String = IO.Directory.GetCurrentDirectory() & "\"


    Dim definers As New List(Of String)
    Dim definees As New List(Of String)

    Sub preProcess(ByVal path As String)
        preProcessIncludes(path)
        preProcessDefines()
    End Sub

    Sub preProcessIncludes(ByVal path As String)
        Dim preProcessLoop As Boolean = False
        Dim round As Byte = 1
        Do
            preProcessLoop = False
            If (round = 0) Then
                path = filePath + "prp.drm"
            End If

            ' Optimise this later
            If round = 1 Then
                round = 0
            End If

            Dim lines() As String = IO.File.ReadAllLines(path)

            If (IO.File.Exists(filePath + "prp.drm")) Then
                My.Computer.FileSystem.DeleteFile(filePath + "prp.drm")
            End If

            For Each line As String In lines
                If line.StartsWith("#include") Then
                    preProcessLoop = True
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
                ElseIf line.StartsWith("#define") Then
                    Dim defineMacro As String = line.Replace("#define", "").Trim()

                    Dim definer As String = defineMacro.Substring(0, defineMacro.IndexOf(" "))

                    defineMacro = defineMacro.Replace(definer, "").Trim()

                    definers.Add(definer)
                    definees.Add(defineMacro)

                    Console.WriteLine("Defining macro: " & definer & " as " & defineMacro)

                ElseIf line.StartsWith("//") Then
                    ' Skip comment lines
                Else
                    My.Computer.FileSystem.WriteAllText(filePath + "prp.drm", (line + Chr(13)), True)
                End If
            Next

        Loop While (preProcessLoop = True)
    End Sub

    Sub preProcessDefines()
        For Each definer As String In definers
            ' Replace all instances of definer with definee in prp.drm
            Dim definee As String = definees(definers.IndexOf(definer))
            Dim text As String = My.Computer.FileSystem.ReadAllText(filePath + "prp.drm")
            text = text.Replace(definer, definee)
            My.Computer.FileSystem.WriteAllText(filePath + "prp.drm", text, False)
        Next
    End Sub
End Module
