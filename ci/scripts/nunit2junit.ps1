
Param(
    [Parameter(Mandatory)]
    [System.IO.FileInfo]
    $inputXml,

    [Parameter(Mandatory)]
    [System.IO.FileInfo]
    $outputXml,

    [System.IO.FileInfo]
    $transformer = "/usr/local/lib/nunit3-junit.xslt"
)
Begin{
    if(-Not ($inputXml | Test-Path) ){
        throw "Arg 1 (inputXml): File or folder ""$inputXml"" does not exist"
    }
    if(-Not ($inputXml | Test-Path -PathType Leaf) ){
        throw "Arg 1 (inputXml): The argument must be a file. Folder paths are not allowed."
    }
    if($inputXml -notmatch "(\.xml)$"){
        throw "Arg 1 (inputXml): The file specified in the path argument must be filetype .xml"
    }
    $inputXml = (Resolve-Path $inputXml).ToString()
    
    if($outputXml | Test-Path){
        throw "Arg 2 (outputXml): File or folder ""$outputXml"" already exists"
    }
    if (-not [System.IO.Path]::IsPathRooted($outputXml)) {
        $outputXml = Join-Path ($pwd) $outputXml
    }

    if(-Not ($transformer | Test-Path) ){
        throw "Arg 3 (transformer): File or folder ""$transformer"" does not exist"
    }
    if(-Not ($transformer | Test-Path -PathType Leaf) ){
        throw "Arg 3 (transformer): The argument must be a file. Folder paths are not allowed."
    }
    if($transformer -notmatch "(\.xslt)$"){
        throw "Arg 3 (transformer): The file specified in the path argument must be filetype .xslt"
    }
}
Process {
    $xslt = New-Object System.Xml.Xsl.XslCompiledTransform;
    $xslt.Load($transformer);
    $xslt.Transform($inputXml, $outputXml);
    Write-Output "Output: $outputXml using $transformer"
}
