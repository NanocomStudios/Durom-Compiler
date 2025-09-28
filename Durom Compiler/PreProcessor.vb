Imports System.IO

Module PreProcessor

    Dim compilerPath As String = AppDomain.CurrentDomain.BaseDirectory()
    Dim filePath As String = IO.Directory.GetCurrentDirectory() & "\"


    Dim definers As New List(Of String)
    Dim definees As New List(Of String)

    Sub preProcess(ByVal path As String)
        preProcessIncludes(path)
        preProcessComments()
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

            Dim ifDefBlock As Boolean = False
            Dim skipLine As Boolean = False
            Dim commentBlock As Boolean = False

            For Each line As String In lines
                line = line.Trim()
                If ifDefBlock = True Then
                    If skipLine Then
                        If line.StartsWith("#endif") Then
                            ifDefBlock = False
                            skipLine = False
                        End If
                    Else
                        ' Currently in an ifdef block
                        If line.StartsWith("#endif") Then
                            ifDefBlock = False
                        Else
                            My.Computer.FileSystem.WriteAllText(filePath + "prp.drm", (line + Chr(13)), True)
                        End If
                    End If

                ElseIf commentBlock = True Then
                    ' Currently in a comment block
                    If line.Contains("*/") Then
                        commentBlock = False
                        line = line.Substring(line.IndexOf("*/") + 2).Trim()
                        If line <> "" Then
                            My.Computer.FileSystem.WriteAllText(filePath + "prp.drm", (line + Chr(13)), True)
                        End If
                    End If
                Else

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

                        Dim index As Integer = defineMacro.IndexOf(" ")

                        If index = -1 Then
                            definers.Add(defineMacro)
                            definees.Add("")
                            Console.WriteLine("Defining macro: " & defineMacro)
                        Else
                            Dim definer As String = defineMacro.Substring(0, defineMacro.IndexOf(" "))

                            defineMacro = defineMacro.Replace(definer, "").Trim()

                            definers.Add(definer)
                            definees.Add(defineMacro)

                            Console.WriteLine("Defining macro: " & definer & " as " & defineMacro)
                        End If

                    ElseIf line.StartsWith("#ifdef") Then
                        preProcessLoop = True
                        Dim defineMacro As String = line.Replace("#ifdef", "").Trim()
                        If (definers.IndexOf(defineMacro) <> -1) Then
                            ifDefBlock = True
                            skipLine = False
                        Else
                            ifDefBlock = True
                            skipLine = True
                        End If

                    ElseIf line.StartsWith("#ifndef") Then
                        preProcessLoop = True
                        Dim defineMacro As String = line.Replace("#ifndef", "").Trim()
                        If (definers.IndexOf(defineMacro) = -1) Then
                            ifDefBlock = True
                            skipLine = False
                        Else
                            ifDefBlock = True
                            skipLine = True
                        End If

                    ElseIf line.StartsWith("//") Then
                        ' Skip comment lines
                    Else

                        Dim index As Integer = line.IndexOf("/*")
                        If index <> -1 Then
                            commentBlock = True
                            line = line.Substring(0, index).Trim()
                        End If

                        If line <> "" Then
                            My.Computer.FileSystem.WriteAllText(filePath + "prp.drm", (line + Chr(13)), True)
                        End If
                    End If
                End If

            Next

        Loop While (preProcessLoop = True)
    End Sub

    Sub preProcessComments()
        Dim lines() As String = IO.File.ReadAllLines(filePath + "prp.drm")

        My.Computer.FileSystem.DeleteFile(filePath + "prp.drm")

        For Each line As String In lines
            Dim index As Integer = line.IndexOf("//")
            If index <> -1 Then
                line = line.Substring(0, index)
            End If
            My.Computer.FileSystem.WriteAllText(filePath + "prp.drm", (line + Chr(13)), True)
        Next

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
