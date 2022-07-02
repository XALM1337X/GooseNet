
if (!$(Get-NetFirewallRule -DisplayName "rs1in" 2>$null)) {
	New-NetFirewallRule -DisplayName "rs1in" -Direction Inbound -LocalPort 1337 -Protocol TCP -Action Allow
} 

if (!$(Get-NetFirewallRule -DisplayName "rs1out" 2>$null)) {
	New-NetFirewallRule -DisplayName "rs1out" -Direction Outbound -LocalPort 1337 -Protocol TCP -Action Allow
} 
$exit = 0;
$enc = [system.Text.Encoding]::UTF8
# Set up endpoint and start listening
$clientIDTicker = 0;
$endpoint = new-object System.Net.IPEndPoint([ipaddress]::any, 1337) 
$server = new-object System.Net.Sockets.TcpListener $endpoint
[pscustomobject[]]$clientList = $null;
[pscustomobject]$MasterClient = $null;

Try { 	
	Write-Output "Booting server...";
	$server.Start();		
}
Catch {
	Write-Output $_;
}

Write-Output "Waiting for clients...";
while ($exit -eq 0) {
	#Client Initialization.
	if ($server.Pending()) {
		$client = $server.AcceptTcpClient();
		$stream = $client.GetStream() 
		$reader = New-Object System.IO.StreamReader($stream);
		$writer = New-Object System.IO.StreamWriter($stream);			
		if ($reader.Peek() -ge 0) {
			$in_buff = $reader.ReadLine();
			Write-Output $in_buff;
			$join_buff = $in_buff -join "";
			if ($join_buff -eq "master_init") {
				$MasterClient = [pscustomobject]@{
								ID = $clientIDTicker
								ClientConn = $client;
								ClientRemoteIP = $client.Client.RemoteEndPoint.Address.IPAddressToString;
								StreamReader = $reader;
								StreamWriter = $writer;
							  }				
				$writer.WriteLine("master established");
				$writer.Flush();
				Write-Output "Master Connected";
			} else {
				$clientList += [pscustomobject]@{
								ID = $clientIDTicker;
								ClientConn = $client;
								ClientRemoteIP = $client.Client.RemoteEndPoint.Address.IPAddressToString;
								StreamReader = $reader;
								StreamWriter = $writer;
							  }
				$writer.WriteLine("client established");
				$writer.Flush();
				Write-Output "Client Connected...";
			}
		}
		$clientIDTicker += 1;
	}
	
	#Iterate over clients. Read content of messages and send results to master client.
	for ($i=0; $i -lt $clientList.Length; $i++) {			
		try {
			if($clientList[$i].ClientConn.Client.Poll(500000,[System.Net.Sockets.SelectMode]::SelectRead) -eq $true) {
				if ($clientList[$i].ClientConn.Client.Available -eq 0) {
					Write-Output "Connection to client lost.";
					$clientList[$i].ClientConn.Close();					
				} else {
					while ($clientList[$i].StreamReader.Peek() -ge 0) {
						try {
							$in_buff = $clientList[$i].StreamReader.ReadLine();
							$join = $in_buff -join "";
							$MasterClient.StreamWriter.WriteLine($join);
							$MasterClient.StreamWriter.Flush();
							
						} catch {}
					}
				}
			}
		} catch [ObjectDisposedException] {
			$NewList = $null
			foreach($item in $clientList) {
				if ($item.ClientConn -ne $clientList[$i].ClientConn) {
					$NewList += $item
				}
			}
			$clientList = $NewList;
		}
	}

	#TODO: Check MasterClient StreamReader for commands
	
}