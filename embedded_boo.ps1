﻿$args = [object[]]@(,[string[]]@())

$EncodedCompressedFile = @'
'@
$DeflatedStream = New-Object IO.Compression.DeflateStream([IO.MemoryStream][Convert]::FromBase64String($EncodedCompressedFile),[IO.Compression.CompressionMode]::Decompress)
$UncompressedFileBytes = New-Object Byte[](2168832)
$DeflatedStream.Read($UncompressedFileBytes, 0, 2168832) | Out-Null

$asm = [Reflection.Assembly]::Load($UncompressedFileBytes)
$type = $asm.GetType("Program")
$main = $type.GetMethod("Main")
$main.Invoke($null, $args)