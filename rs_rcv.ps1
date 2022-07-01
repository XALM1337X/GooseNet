
if (!$(Get-NetFirewallRule -DisplayName "rs1in" 2>$null)) {
	New-NetFirewallRule -DisplayName "rs1in" -Direction Inbound -LocalPort 1337 -Protocol TCP -Action Allow
} 

if (!$(Get-NetFirewallRule -DisplayName "rs1out" 2>$null)) {
	New-NetFirewallRule -DisplayName "rs1out" -Direction Outbound -LocalPort 1337 -Protocol TCP -Action Allow
} 
$exit = 0;
$enc = [system.Text.Encoding]::UTF8
# Set up endpoint and start listening	
$endpoint = new-object System.Net.IPEndPoint([ipaddress]::any, 1337) 
$server = new-object System.Net.Sockets.TcpListener $endpoint
[System.Net.Sockets.TcpClient[]]$clientList = $null;
[System.Net.Sockets.TcpClient]$MasterClient = $null;

Try { 	
	Write-Output "Booting server...";
	$server.Start();		
}
Catch {
	Write-Output $_;
}

Write-Output "Waiting for clients...";
while ($exit -eq 0) {
	#Accept new clients.
	if ($server.Pending()) {
		$client = $server.AcceptTcpClient();
		#$client.Client.RemoteEndPoint.Address.IPAddressToString; #HOW TO GET IP
		$clientList += $client;
		Write-Output "Client Connected...";
		$stream = $client.GetStream() 
		$reader = New-Object System.IO.StreamReader($stream);	
		if ($reader.Peek() -ge 0) {
			$in_buff = $reader.ReadLine();
			Write-Output $in_buff;
		}
		$writer = New-Object System.IO.StreamWriter($stream);		
		$writer.WriteLine("established");
		$writer.Flush();
	}
	
	for ($i=0; $i -lt $clientList.Length; $i++) {
		$client = $clientList[$i];		
		try {
			if($client.Client.Poll(500000,[System.Net.Sockets.SelectMode]::SelectRead) -eq $true) {
				if ($client.Client.Available -eq 0) {
					Write-Output "Connection to client lost.";
					$client.Close();
				} else {
					$stream = $client.GetStream() 
					$reader = New-Object System.IO.StreamReader($stream);
					while ($reader.Peek() -ge 0) {		
						try {
							$in_buff = $reader.ReadLine();
							Write-Output $in_buff;
						} catch {}
					}
				}
			}
		} catch [ObjectDisposedException] {
			$NewList = $null
			foreach($item in $clientList) {
				if ($item -ne $client) {
					$NewList += $item
				}
			}
			$clientList = $NewList;
		}
	}
	
	#iterate over existing clients and check for any messages received.
	
}