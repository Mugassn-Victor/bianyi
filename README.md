# H2 Linux MultiTerminal

Windows WPF prototype for opening several independent `https://cli.h2.nexus` Linux terminal sessions and broadcasting commands through WebView2/CDP input events.

## Build

1. Install .NET 8 SDK on Windows 10/11.
2. Run `publish-win-x64.bat`.
3. Find the EXE under `bin\Release\net8.0-windows\win-x64\publish\`.
4. Microsoft Edge WebView2 Runtime must be installed (normally present on Windows 10/11).

## Use

1. Create 1-8 instances.
2. In every page, manually start Linux and wait for its terminal to become ready.
3. Click **定位终端**, then click the prompt/input area inside that terminal.
4. Check desired instances, type commands, and send.
5. `Ctrl+C` and `Ctrl+L` are broadcast to selected instances.

## Notes

- Each instance has an independent WebView2 user-data directory under `%LocalAppData%\H2LinuxMultiTerminal\Profiles`.
- Saved target positions are proportional coordinates, so ordinary resizing is tolerated.
- This version does not bypass site limits, timers, login, or CAPTCHA and does not scrape terminal output.
- If site layout or zoom changes, locate the terminal again.
