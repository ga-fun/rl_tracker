# RL Session Tracker MVP v9

Small Windows MVP for Rocket League Stats API.

## How to run

1. Close Rocket League.
2. Run `enable_rl_stats_api.bat` once.
3. Launch Rocket League.
4. Run `run_tracker.bat`.
5. Click `Start listening`.
6. Select your player once per account. The tracker saves players by `PrimaryId`.

## v9 behavior

- Saves every manually selected player by `PrimaryId`.
- Auto-selects the most recently manually selected saved player among current match players.
- Logs only useful tracker events. Player detection is logged immediately whenever the player set changes:
  - `Players detected: <count>`
  - `Player selected: <manual/auto>, <player>`
  - `Goal scored: <win/loss/tie>, score <own>-<opponent>`
  - `Mode detected: <1v1/2v2/3v3>`
  - `Tracker updated: <manual/auto/guessed>, <win/loss>`
- `auto` means the result came from a real `MatchEnded` / winner event.
- `guessed` means the result was inferred from premature match destruction / leaving.
- If a premature exit happens and current state is not `win`, it counts as `loss`.
- Tracks separate counters for `1v1`, `2v2`, `3v3`, and `other`.
- The mode is inferred from the highest number of players detected during the match:
  - 2 players => `1v1`
  - 4 players => `2v2`
  - 6 players => `3v3`

## Diagnostic

Run `run_diagnostic.bat` while Rocket League is in a match if the tracker does not connect.
