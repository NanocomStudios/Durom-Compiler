Module Main

    Dim compilerPath As String = AppDomain.CurrentDomain.BaseDirectory()
    Dim filePath As String = IO.Directory.GetCurrentDirectory() & "\"

    Dim inputFile(0) As String
    Dim inputFileCount As Integer = 0

    Sub Main()
        Console.WriteLine(Environment.CurrentDirectory)
        Dim argCount As Integer = My.Application.CommandLineArgs.Count
        Console.WriteLine(argCount)
        If argCount > 0 Then
            For cnt As Integer = 0 To argCount - 1
                Select Case My.Application.CommandLineArgs.Item(cnt)
                    Case "-o"
                        Console.WriteLine("Output file")
                    Case "-O"
                        Console.WriteLine("Output File")
                    Case Else
                        Dim path As String = getFilePath(My.Application.CommandLineArgs.Item(cnt))
                        If My.Computer.FileSystem.FileExists(path) = False Then
                            Console.WriteLine("Input file not exists!")
                            End
                        Else
                            ReDim Preserve inputFile(inputFile.Count)
                            inputFile(inputFileCount) = path
                            inputFileCount += 1

                        End If
                End Select
            Next

        Else

        End If

        If (My.Computer.FileSystem.FileExists(filePath + "tmp.drm")) Then
            My.Computer.FileSystem.DeleteFile(filePath + "tmp.drm")
        End If

        For Each path As String In inputFile
            If path <> "" Then
                My.Computer.FileSystem.WriteAllText(filePath + "tmp.drm", (My.Computer.FileSystem.ReadAllText(path) + Chr(13)), True)
            End If
        Next

        preProcess(filePath + "tmp.drm")
        compile(filePath + "prp.drm")


    End Sub

    Function getFilePath(ByVal path As String) As String
        If path.Length > 3 Then
            If path.Chars(1) = ":" And path.Chars(2) = "\" Then
                Return path
            Else
                Return Environment.CurrentDirectory + "\" + path
            End If
        Else
            Return Environment.CurrentDirectory + "\" + path
        End If
    End Function

End Module
