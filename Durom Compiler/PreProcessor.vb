Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Module PreProcessor

    Dim compilerPath As String = AppDomain.CurrentDomain.BaseDirectory()
    Dim filePath As String = IO.Directory.GetCurrentDirectory() & "\"

    Dim lines As New List(Of String)
    Dim newLines As New List(Of String)

    Dim definers As New List(Of String)
    Dim definees As New List(Of String)

    Sub preProcess(ByVal path As String)
        preProcessIncludes(path)
        Console.WriteLine()
        preProcessComments()
        preProcessDefines()
        Console.WriteLine()
        My.Computer.FileSystem.WriteAllText(filePath + "prp.drm", String.Join(vbCrLf, lines), False)
    End Sub

    Sub preProcessIncludes(ByVal path As String)
        Dim preProcessLoop As Boolean = False
        Dim initialRound As Boolean = True
        Do
            preProcessLoop = False
            If (initialRound) Then
                lines = IO.File.ReadAllLines(path).ToList
                initialRound = False
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
                            newLines.Add(line)
                        End If
                    End If

                ElseIf commentBlock = True Then
                    ' Currently in a comment block
                    If line.Contains("*/") Then
                        commentBlock = False
                        line = line.Substring(line.IndexOf("*/") + 2).Trim()
                        If line <> "" Then
                            newLines.Add(line)
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
                                newLines.Add(includeLine)
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
                            newLines.Add(line)
                        End If
                    End If
                End If
            Next

            lines.Clear()
            For Each line As String In newLines
                lines.Add(line)
            Next

            newLines.Clear()
        Loop While (preProcessLoop = True)
    End Sub

    Sub preProcessComments()
        newLines.Clear()
        For Each line As String In lines
            Dim index As Integer = line.IndexOf("//")
            If index <> -1 Then
                line = line.Substring(0, index)
            End If
            newLines.Add(line)
        Next
        lines.Clear()
        For Each line As String In newLines
            lines.Add(line)
        Next
        newLines.Clear()

    End Sub

    Sub preProcessDefines()
        Dim text As String = String.Join(vbCr, lines)

        For Each definer As String In definers
            ' Replace all instances of definer with definee in prp.drm
            Dim definee As String = definees(definers.IndexOf(definer))
            text = replaceWord(text, definer, definee)
        Next

        lines = text.Split(vbCr).ToList()
    End Sub


    Function replaceWord(ByVal original As String, ByVal wordToReplace As String, ByVal replacementWord As String) As String
        Dim pattern As String = "\b" & Regex.Escape(wordToReplace) & "\b"
        Dim result As String = Regex.Replace(original, pattern, replacementWord)
        Return result
    End Function

End Module
