# File Collector

Windows Forms application for automated file collection from multiple folders with fully dynamic configuration.

Built with .NET Framework 4.8 + WinForms, Persian RTL UI.

## Quick Start

1. Open `FileCollector.sln` in Visual Studio 2019/2022
2. Restore NuGet packages:
   - `Newtonsoft.Json`
   - `System.Data.SQLite.Core`
3. Build and run

## Features

- **Multi-folder watch** with subfolder recursion and per-folder pause/resume/stop
- **11 action types**: Copy, Move, Rename, Delete, Recycle, Zip, ZipAndMove, Extract, CustomCommand, TextProcessing, DatabaseStore
- **Chain actions** (up to 5 steps)
- **Text processing**: Find/Replace (with Regex), Header/Footer, Append/Prepend
- **Remote SQL Server storage** with 3 modes: BLOB Direct / Hybrid / FILESTREAM
- **4 progress bars**: Overall / Per Folder / Per File / Queue
- **22 template variables**: `{date}`, `{md5}`, `{size}`, `{filename}`, ...
- **Deduplication** via MD5 hash
- **System tray** with minimize-to-tray
- **JSON config** with hot-reload
- **Local SQLite** for history/log
- **Import/Export** settings

See [README.md](src/FileCollector/README.md) for full documentation.

## License

MIT
