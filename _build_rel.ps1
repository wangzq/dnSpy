nuget restore "$PsScriptRoot\dnspy.sln"
msbuild /m /nologo /v:q /clp:ErrorsOnly "$PsScriptRoot\dnspy.sln" /p:Configuration=Release
$zipfile = "$PsScriptRoot\dnspy.release.zip"
if (Test-Path $zipfile) { Remove-Item $zipfile }
$dir = "$PsScriptRoot\dnspy\dnspy\bin\release\net471"
# delete all xml documentation files; not using '-xr!*.xml' because there are other xml files such as debug\DotNet.Ex.xml we still need
dir $dir -file *.dll | % {
	$xml = [IO.Path]::ChangeExtension($_.FullName, '.xml')
	if (Test-Path $xml) {
		Remove-Item $xml
	}
}
7z a -tzip '-xr!*.pdb' -mx9 $zipfile "$dir\*"
