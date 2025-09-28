Module Compiler

    Sub compile(ByVal path As String)

        For Each line As String In IO.File.ReadAllLines(path)
            Console.WriteLine(line)
        Next

    End Sub
End Module
