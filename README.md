Peeb
====

![Peeb](peeb.png)

A Discord bot for Final Fantasy XIV servers, find out more at [peeb.gg](https://peeb.gg).

Setup
-----

1. Install [.NET].
2. Create a `PEEB_DISCORD_BOT_TOKEN` environment variable.

### macOS

Run the following zsh commands in the project's directory, [Homebrew] will also be installed:

```zsh
% /usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
% brew install --cask dotnet-sdk
% echo 'export PEEB_DISCORD_BOT_TOKEN=Secret' >> ~/.zshrc
```

### Windows

Run the following PowerShell commands in the project's directory, [Chocolatey] will also be installed:

```powershell
> Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
> choco install dotnet-sdk
> [Environment]::SetEnvironmentVariable("PEEB_DISCORD_BOT_TOKEN", "Secret", "User")
```

Build
-----

```zsh
% dotnet build
```

Run
---

```zsh
% dotnet run
```

Test
----

```zsh
% dotnet test
```

[.NET]: https://dotnet.microsoft.com
[Chocolatey]: https://chocolatey.org
[Homebrew]: https://brew.sh
