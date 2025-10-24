Imports System.Text.RegularExpressions

Module Compiler

    Dim blockStack As Stack = New Stack()
    Dim blockList As New List(Of String)

    Dim bracketStack As Stack = New Stack()
    Dim bracketList As New List(Of String)

    Structure Variable
        Dim name As String
        Dim size As Byte
    End Structure

    Structure Constant
        Dim name As String
        Dim size As Int16
        Dim value As List(Of Byte)
    End Structure

    Dim variableList As New List(Of Variable)
    Dim constList As New List(Of Constant)

    Dim globalScope As String

    Sub compile(ByVal path As String)
        breakIntoBlocks(path)
        breakIntoBracketBlocks(path)
        identifyVariables()
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
                        Console.WriteLine("Unmatched closing brace '}'")
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

    Sub breakIntoBracketBlocks(ByVal path As String)
        Dim currentBlock As String = ""

        For Each line As String In IO.File.ReadAllLines(path)

            For Each ch As Char In line
                If ch = "("c Then
                    bracketStack.Push(currentBlock)
                    currentBlock = ""
                ElseIf ch = ")"c Then
                    If bracketStack.Count > 0 Then
                        bracketList.Add(currentBlock)
                        currentBlock = bracketStack.Pop() & " (" & bracketList.Count - 1 & ") "
                    Else
                        Console.WriteLine("Unmatched closing brace ')'")
                    End If
                Else
                    currentBlock &= ch
                End If

            Next
            currentBlock &= vbCrLf
        Next


        Dim i As Integer = 0
        For Each blk As String In bracketList
            Console.WriteLine("(" & i & "):" & vbCrLf & blk)
            i += 1
        Next


    End Sub

    Sub identifyVariables()
        Dim varPattern As String = "\b(char|int|short|long|float|double)\s+([a-zA-Z_][a-zA-Z0-9_]*(\s*=\s*[^,;]+)?(\s*,\s*[a-zA-Z_][a-zA-Z0-9_]*(\s*=\s*[^,;]+)?)*\s*);"
        Dim varRegex As New Regex(varPattern)
        Dim i As Integer = 0
        For Each blk As String In blockList
            Console.WriteLine()
            Console.WriteLine(i & " : ")

            For Each line As String In blk.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
                Dim m As Match = Regex.Match(line, varPattern)
                If m.Success Then
                    Dim varType As String = m.Groups(1).Value
                    Dim varsSection As String = m.Groups(2).Value

                    Dim vars() As String = varsSection.Split(","c)
                    For Each v As String In vars
                        Dim part As String = v.Trim()
                        Dim name As String
                        Dim value As String = Nothing

                        If part.Contains("=") Then
                            Dim nv() As String = part.Split("="c)
                            name = nv(0).Trim()
                            value = nv(1).Trim()
                        Else
                            name = part
                        End If

                        Console.WriteLine($"Type: {varType}, Name: {name}, Value: {If(value, "uninitialized")}")
                    Next
                End If
            Next
            i += 1
        Next
    End Sub

End Module


'if()
'While()
'keywords
'variables

'numbers
