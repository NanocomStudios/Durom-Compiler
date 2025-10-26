Imports System.Runtime.CompilerServices
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

    Dim globalScope As Integer

    Sub compile(ByVal path As String)
        breakIntoBlocks(path)
        decodeLineOperations()
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

        globalScope = blockList.Count
        blockList.Add(currentBlock)

        Console.WriteLine()

        Dim i As Integer = 0
        For Each blk As String In blockList
            If (i = globalScope) Then
                Console.WriteLine("{global}:" & vbCrLf & blk)
            Else
                Console.WriteLine("{" & i & "}:" & vbCrLf & blk)
            End If
            i += 1
        Next


    End Sub

    Sub decodeLineOperations()

        Dim i As Integer = 0
        For Each blk As String In blockList
            Console.WriteLine()
            Console.WriteLine(i & " : ")

            For Each line As String In blk.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
                line.TrimStart()
                If line.StartsWith("void") Then
                    Console.WriteLine("void")
                ElseIf line.StartsWith("const") Then
                    Console.WriteLine("const")
                ElseIf line.StartsWith("char") Then
                    Console.WriteLine("char")
                ElseIf line.StartsWith("short") Then
                    Console.WriteLine("short")
                ElseIf line.StartsWith("int") Then
                    Console.WriteLine("int")
                ElseIf line.StartsWith("long") Then
                    Console.WriteLine("long")
                ElseIf line.StartsWith("float") Then
                    Console.WriteLine("float")
                ElseIf line.StartsWith("double") Then
                    Console.WriteLine("double")
                ElseIf line.StartsWith("if") Then
                    Console.WriteLine("if")
                ElseIf line.StartsWith("for") Then
                    Console.WriteLine("for")
                ElseIf line.StartsWith("while") Then
                    Console.WriteLine("while")
                ElseIf line.StartsWith("switch") Then
                    Console.WriteLine("switch")
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
