$serverAddress = "pi@washingiot"

$path = (Get-Item "$PWD").FullName 
$folderName = (Get-Item $path).Name

Write-Host $path

Remove-Item -Recurse -Path bin/ -ErrorAction Ignore

dotnet publish -o out

ssh $serverAddress "mkdir -p /home/pi/${folderName}"
scp -r $path/out/** "${serverAddress}:/home/pi/${folderName}"