
if (!$(Get-NetFirewallRule -DisplayName "rs1in" 2>$null)) {
	New-NetFirewallRule -DisplayName "rs1in" -Direction Inbound -LocalPort 1337 -Protocol TCP -Action Allow
} 

if (!$(Get-NetFirewallRule -DisplayName "rs1out" 2>$null)) {
	New-NetFirewallRule -DisplayName "rs1out" -Direction Outbound -LocalPort 1337 -Protocol TCP -Action Allow
} 
#$stream = $client.GetStream() 
#$reader = New-Object System.IO.StreamReader($stream);
#$writer = New-Object System.IO.StreamWriter($stream);

$exit = 0;
$enc = [system.Text.Encoding]::UTF8
# Set up endpoint and start listening
$clientIDTicker = 0;
$endpoint = new-object System.Net.IPEndPoint([ipaddress]::any, 1337) 
$server = new-object System.Net.Sockets.TcpListener $endpoint
[pscustomobject[]]$ClientList = $null;
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
			#Write-Output $in_buff;
			$join_buff = $in_buff -join "";
			if ($join_buff -eq "master_init") {
				$MasterClient = [pscustomobject]@{
								ID = $clientIDTicker
								ClientConn = $client;
								ClientRemoteIP = $client.Client.RemoteEndPoint.Address.IPAddressToString;

							  }				
				$writer.WriteLine("master established");
				$writer.Flush();
				Write-Output "Master Connected";
			} else {
				$ClientList += [pscustomobject]@{
								ID = $clientIDTicker;
								ClientConn = $client;
								ClientRemoteIP = $client.Client.RemoteEndPoint.Address.IPAddressToString;

							  }
				$writer.WriteLine("client established");
				$writer.Flush();
				Write-Output "Client Connected...";
			}
		}
		$clientIDTicker += 1;
	}
	if ($ClientList -ne $null -and $ClientList.Length -gt 0) {
		#Iterate over clients. Read content of messages and send results to master client.
		for ($i=0; $i -lt $ClientList.Length; $i++) {			
			try {
				if($ClientList[$i].ClientConn.Client.Poll(500000,[System.Net.Sockets.SelectMode]::SelectRead) -eq $true) {
					if ($ClientList[$i].ClientConn.Client.Available -eq 0) {
						Write-Output "Connection to client lost.";
						$ClientList[$i].ClientConn.Close();					
					} else {
						$client_stream = $ClientList[$i].ClientConn.GetStream();
						$client_reader = New-Object System.IO.StreamReader($client_stream);

						if ($MasterClient -ne $null) {
							$master_stream = $MasterClient.ClientConn.GetStream();
							$master_writer = New-Object System.IO.StreamWriter($master_stream);
							while ($client_reader.Peek() -ge 0) {
								try {
									$in_buff = $client_reader.ReadLine();
									$join = $in_buff -join "";
									$master_writer.WriteLine($join);
									$master_writer.Flush();								
								} catch {}
							}

						} else {
							while ($client_reader.Peek() -ge 0) {
								try {
									$in_buff = $client_reader.ReadLine();
									$join = $in_buff -join "";
									Write-Output $join;						
								} catch {}
							}
						}

					}
				}
			} catch [ObjectDisposedException] {
				$NewList = $null
				foreach($item in $ClientList) {
					if ($item.ClientConn -ne $ClientList[$i].ClientConn) {
						$NewList += $item
					}
				}
				$ClientList = $NewList;
			}
		}		
	}

	if ($MasterClient -ne $null) {
		try {
			if($MasterClient.ClientConn.Client.Poll(500000, [System.Net.Sockets.SelectMode]::SelectRead) -eq $true) {
				if ($MasterClient.ClientConn.Client.Available -eq 0) {
					Write-Output "Master Connection lost.";
					$MasterClient.ClientConn.Close();
				} else {
						$master_stream = $MasterClient.ClientConn.GetStream();
						$master_reader = New-Object System.IO.StreamReader($master_stream);
						$master_writer = New-Object System.IO.StreamWriter($master_stream);

					while ($master_reader.Peek() -ge 0) {
						try {
							
							$in_buff = $master_reader.ReadLine();
							$join = $in_buff -join "";
							#Read commands send by master client, and relay to appropriate endpoint.
							
							#Dump connected client info to master client.
							
							#client_dump####################################################
							if ($join -match ".*--client_dump") {
								Write-Output "Dumping client list to master client.";
								if ($ClientList.Length -gt 0) {
									for ($i=0; $i -lt $ClientList.Length; $i++) {
										try {
											$master_writer.WriteLine($ClientList[$i]);
												
										} catch {
											Write-Output $_;
										}
										
									}									
								} else {
									$master_writer.WriteLine("No clients connected");
								}

								$master_writer.Flush();
							} elseif ($join -match ".*--[i|I][d|D]=([0-9]+)\s+--command=(.*)") {
								
								for ($i=0; $i -lt $ClientList.Length; $i++) {
									if ($Matches[1] -eq $ClientList[$i].ID) {
										$client_stream = $ClientList[$i].ClientConn.GetStream();
										$client_writer = New-Object System.IO.StreamWriter($client_stream)
										$client_writer.WriteLine($Matches[2]);
										$client_writer.Flush();
									}
								}

							} else {
								$master_writer.WriteLine("improper format detected:\n");
								$master_writer.WriteLine("Command: '--id=<int> --command=<string>'")
								$master_writer.WriteLine("ClientList: '--client_dump'")
								$master_writer.Flush();
							}
							#################################################################
						} catch{
							Write-Output $_;
						}
					}
				}
			}
		} catch [ObjectDisposedException] {
			Write-Output "disposed exception thrown on master connection.";
			$MasterClient = $null;
		}		
	}	
}