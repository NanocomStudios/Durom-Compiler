Module Compiler

    Sub compile(ByVal path As String)
        breakIntoBlocks(path)
    End Sub

    Sub breakIntoBlocks(ByVal path As String)
        For Each line As String In IO.File.ReadAllLines(path)



        Next
    End Sub
End Module

'if()
'While()
'keywords
'variables

'numbers
