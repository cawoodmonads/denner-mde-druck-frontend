Push-Location C:\projects\Monads\monadsag.github.io
if (-not(Test-Path C:\projects\Monads\monadsag.github.io\mde-druck)) {
    New-Item -ItemType Directory -Path C:\projects\Monads\monadsag.github.io\mde-druck
}
Remove-Item C:\projects\Monads\monadsag.github.io\mde-druck\* -Recurse -Force

Copy-Item C:\projects\denner\mde-druck\dist\* -Destination .\mde-druck -Recurse -Force
git add . && git commit -a -m "MDE Druck $(Get-Date -f yyyyMMddThhmmss)" && git push
Pop-Location