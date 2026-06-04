# TODO

- `RlNotFound` + `RlNeedRestart` => move to `RocketLeague.Config`
- UI => Refactor `MainTracker`
- UI v2:
	- polish `MainTracker`
	- add `settings`

---

# Build command for releases

```bash
dotnet publish ".\src\8_RlTracker.Ui\RlTracker.Ui.csproj" `
	-c Release `
	-r win-x64 `
	--self-contained true `
	-o ".\publish" `
	-p:PublishSingleFile=true `
	-p:IncludeNativeLibrariesForSelfExtract=true `
	-p:EnableCompressionInSingleFile=true `
	-p:DebugType=None `
	-p:DebugSymbols=false
```
