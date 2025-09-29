Module Compiler

    Dim blockStack As Stack = New Stack()
    Dim blockList As New List(Of String)

    Dim globalScope As String

    Sub compile(ByVal path As String)
        breakIntoBlocks(path)
        'compile separate blocks
        '   handle Variables &  Constants
        '   handle Operations
    End Sub

    Sub breakIntoBlocks(ByVal path As String)
        Dim currentBlock As String = ""

        For Each line As String In IO.File.ReadAllLines(path)

            For Each ch As Char In line
                If ch = "{"c Then
                    blockStack.Push(currentBlock)
                    currentBlock = ""
                ElseIf ch = "}"c Then
                    If blockStack.Count > 0 Then
                        blockList.Add(currentBlock)
                        currentBlock = blockStack.Pop() & " {" & blockList.Count - 1 & "} "
                    Else
                        Console.WriteLine("Unmatched closing brace")
                    End If
                Else
                    currentBlock &= ch
                End If

            Next
            currentBlock &= vbCrLf
        Next

        globalScope = currentBlock

        Console.WriteLine()
        Console.WriteLine("{global}:" & vbCrLf & globalScope)

        Dim i As Integer = 0
        For Each blk As String In blockList
            Console.WriteLine("{" & i & "}:" & vbCrLf & blk)
            i += 1
        Next


    End Sub
End Module

'if()
'While()
'keywords
'variables

'numbers
