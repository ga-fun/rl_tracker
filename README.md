# RlTracker

[![Latest release](https://img.shields.io/github/v/release/guillaumeast/rl_tracker?style=flat&label=version&color=orange)](https://github.com/guillaumeast/rl_tracker/releases/latest)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=guillaumeast_rl_tracker&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=guillaumeast_rl_tracker)
[![Platform](https://img.shields.io/badge/platform-Windows-0078D6?style=flat&logo=windows&logoColor=white)](#)
[![Language](https://img.shields.io/badge/language-C%23-512BD4?style=flat&logo=csharp&logoColor=white)](#)
[![.NET](https://img.shields.io/badge/-10.0-512BD4?style=flat&logo=dotnet&logoColor=white&labelColor=333333)](#)
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/guillaumeast/rl_tracker)

RlTracker is a small Windows desktop app that tracks your Rocket League session in real time.

It shows your wins, losses and current streak while you play.

## Features

- 📈 Live win/loss tracking
- 🚀 Current win/loss streak
- ⚔️ 1v1, 2v2 and 3v3 detection
- 📂 Steam and Epic Games install detection
- 🖼️ Simple Windows desktop UI
- 🗒️ Local settings and logs

## Download

1. Open the [latest release](https://github.com/guillaumeast/rl_tracker/releases/latest).
2. Scroll to **Assets**.
3. Download **RL_Tracker_0_0_0.exe**.
4. Put it anywhere you want, for example on your Desktop.
5. Double-click **RL_Tracker_0_0_0.exe** to launch it.

> ✌️ You do **not** need to install .NET manually.

## First launch

The Tracker may update your Rocket League config automatically.

If the app says that Rocket League needs to restart:

1. Close Rocket League.
2. Start Rocket League again.
3. Start or keep the Tracker open.

> If Rocket League is not detected, click **Open config file** and update the Rocket League install path manually.

## Windows / antivirus warning

🙈 This early version is not code-signed yet, so Windows or your antivirus may show a warning.

## Files location

📂 RlTracker stores its settings and logs here:

```text
%LOCALAPPDATA%\RlTracker
```

## Status

🧒 This is an early test version.

The goal for now is simple: check if the tracker works correctly on different Windows machines.
