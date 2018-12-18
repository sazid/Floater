# Floater
A floating web browser built with .NET and Chromium (Chromium Embed Framework) for Windows.

![Screenshot1](screenshots/1.jpg)
![Screenshot2](screenshots/2.png)
![Screenshot3](screenshots/3.png)

### Download
1. Download & install [Visual Studio](https://visualstudio.microsoft.com/)
2. Install the data sources plugin for Visual Studio if not already installed (you'll be prompted for the first time if you haven't used data sources before).
3. (Download)[https://github.com/sazid/Floater/archive/master.zip] or clone the repository using `git`
4. Open the Floater.sln file in Visual Studio.
5. Build & run the project using `Ctrl-F5` (If the application hangs, copy `floaterdb.mdf` and `floaterdb_log.ldf` files to the respsective build directory i.e `Floater/bin/debug/x64` for a debug build of 64-bit architecture) and then run again.

### Features
1. Float above all other windows (perfect for watching video or chat when doing something else e.g following a tutorial)
2. Transparent Window
3. Hide UI elements & dedicated YouTube(r) video mode to watch videos without distraction
4. Clean and Minimal UI

### Keyboard Shortcuts
* `Ctrl-B` - Go back
* `Ctrl-F` - Go forward
* `Ctrl-H` - Show history
* `Ctrl-R` - Reload
* `Ctrl-L` - Focus address bar
* `Ctrl-P` - Print
* `ESC` - Move focus from address bar to browser

### Developer
Mohammed Sazid Al Rashid
* [LinkedIn](https://linkedin.com/in/sazidz)
* [GitHub](https://github.com/sazid)
* [Stackoverflow](https://stackoverflow.com/users/1941132/sazid)

### License
[MIT License](LICENSE)

This project uses the open source [Captura](https://github.com/MathewSachin/Captura) screen recording software. Check out the project for its own license.
