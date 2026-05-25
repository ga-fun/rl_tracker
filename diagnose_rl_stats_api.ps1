$ErrorActionPreference = "Continue"

function Show-Config($path) {
	if (Test-Path $path) {
		Write-Host "FOUND: $path"
		Get-Content $path | ForEach-Object { Write-Host "  $_" }
	} else {
		Write-Host "MISSING: $path"
	}
}

$paths = @(
	"$env:USERPROFILE\Documents\My Games\Rocket League\TAGame\Config\TAStatsAPI.ini",
	"$env:USERPROFILE\Documents\My Games\Rocket League\TAGame\Config\DefaultStatsAPI.ini",
	"C:\Program Files (x86)\Steam\steamapps\common\rocketleague\TAGame\Config\DefaultStatsAPI.ini",
	"C:\Program Files\Epic Games\rocketleague\TAGame\Config\DefaultStatsAPI.ini"
)

Write-Host "== Config files =="
foreach ($path in $paths) {
	Show-Config $path
}

Write-Host ""
Write-Host "== TCP port test =="
Write-Host "This only works while Rocket League is in a match."
try {
	$client = New-Object System.Net.Sockets.TcpClient
	$client.Connect("127.0.0.1", 49123)
	Write-Host "OK: connected to 127.0.0.1:49123"
	$stream = $client.GetStream()
	$buffer = New-Object byte[] 8192
	$deadline = (Get-Date).AddSeconds(5)
	while ((Get-Date) -lt $deadline) {
		if ($stream.DataAvailable) {
			$count = $stream.Read($buffer, 0, $buffer.Length)
			$text = [System.Text.Encoding]::UTF8.GetString($buffer, 0, $count)
			Write-Host "Received bytes: $count"
			Write-Host $text.Substring(0, [Math]::Min(700, $text.Length))
			break
		}
		Start-Sleep -Milliseconds 100
	}
	$client.Close()
} catch {
	Write-Host "FAIL: could not connect to 127.0.0.1:49123"
	Write-Host $_.Exception.Message
	Write-Host "Make sure Rocket League was restarted after config update and is currently in a match."
}
