$ErrorActionPreference = "Stop"

$section = "[TAGame.MatchStatsExporter_TA]"
$content = @"
[TAGame.MatchStatsExporter_TA]
Port=49123
PacketSendRate=10
"@

$paths = @(
	"$env:USERPROFILE\Documents\My Games\Rocket League\TAGame\Config\TAStatsAPI.ini",
	"$env:USERPROFILE\Documents\My Games\Rocket League\TAGame\Config\DefaultStatsAPI.ini",
	"C:\Program Files (x86)\Steam\steamapps\common\rocketleague\TAGame\Config\DefaultStatsAPI.ini",
	"C:\Program Files\Epic Games\rocketleague\TAGame\Config\DefaultStatsAPI.ini"
)

foreach ($path in $paths) {
	try {
		$directory = Split-Path -Parent $path
		if (-not (Test-Path $directory)) {
			New-Item -ItemType Directory -Force -Path $directory | Out-Null
		}
		Set-Content -Path $path -Value $content -Encoding ASCII
		Write-Host "OK: $path"
	} catch {
		Write-Host "FAIL: $path"
		Write-Host $_.Exception.Message
	}
}

Write-Host ""
Write-Host "Close and restart Rocket League before testing."
