@echo off
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0rl_session_tracker.ps1"
if errorlevel 1 pause
