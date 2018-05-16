nuget restore "$PsScriptRoot\dnspy.sln"
msbuild /m /nologo /v:q /clp:ErrorsOnly "$PsScriptRoot\dnspy.sln" /p:Configuration=Release
# $files = dir "$PsScriptRoot\dnspy\dnspy\bin\release" -Recurse | ? { $_.Name -notmatch '\.(pdb|xml)$' } | Select -Expand FullName
# Compress-Archive -Destination "$PsScriptRoot\dnspy.release.zip" -LiteralPath $files -Force
$zipfile = "$PsScriptRoot\dnspy.release.zip"
if (Test-Path $zipfile) { Remove-Item $zipfile }
7z a -tzip '-xr!*.pdb' '-xr!*.xml' -mx9 $zipfile "$PsScriptRoot\dnspy\dnspy\bin\release\*"
