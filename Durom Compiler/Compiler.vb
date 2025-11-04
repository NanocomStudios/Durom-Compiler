Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Module Compiler

    Dim blockStack As Stack = New Stack()
    Dim blockList As New List(Of String)

    Structure dataType
        Dim type As String
        Dim size As Integer
    End Structure

    Dim dataTypes As Dictionary(Of String, dataType) = New Dictionary(Of String, dataType) From {
        {"void", New dataType With {.type = "void", .size = 0}},
        {"bool", New dataType With {.type = "bool", .size = 1}},
        {"char", New dataType With {.type = "int", .size = 1}},
        {"short", New dataType With {.type = "int", .size = 2}},
        {"int", New dataType With {.type = "int", .size = 4}},
        {"long", New dataType With {.type = "int", .size = 8}},
        {"float", New dataType With {.type = "single", .size = 4}},
        {"double", New dataType With {.type = "double", .size = 8}}
    }

    Dim globalScope As Integer

    Sub compile(ByVal path As String)
        breakIntoBlocks(path)
        compileGlobalScope()
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

    Sub compileGlobalScope()
        Dim state As String = "start"
        For Each line As String In blockList(globalScope).Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            For Each l As String In line.CompilerSplit()
                Select Case state
                    Case "start"
                        Select Case l
                            Case "struct"
                                Console.WriteLine("struct detected")

                            Case "class"
                                Console.WriteLine("class detected")

                            Case "enum"
                                Console.WriteLine("enum detected")

                            Case "union"
                                Console.WriteLine("union detected")

                            Case Else
                                If dataTypes.ContainsKey(l) Then
                                    Console.WriteLine(l + " detected")
                                Else
                                End If


                        End Select
                End Select
            Next
        Next

    End Sub

End Module


'if()
'While()
'keywords
'variables

'numbers

Module StringExtensions
    <Extension()>
    Function CompilerSplit(ByVal str As String) As List(Of String)
        Dim tokens As New List(Of String)
        Dim tmp As String = ""

        For Each c As Char In str
            If Char.IsWhiteSpace(c) Then
                If Not String.IsNullOrEmpty(tmp) Then
                    tokens.Add(tmp)
                    tmp = ""
                End If
            ElseIf "();,{}[]".Contains(c) Then
                If Not String.IsNullOrEmpty(tmp) Then
                    tokens.Add(tmp)
                    tmp = ""
                End If
                tokens.Add(c)
            Else
                tmp &= c
            End If
        Next


        Return tokens
    End Function
End Module