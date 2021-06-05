Peeb
====

![Peeb](peeb.png)

A Discord bot for Final Fantasy XIV servers, find out more at [peeb.gg](https://peeb.gg).

Setup
-----

1. Install [.NET].
2. Create `appsettings.Development.json`.

### macOS

Run the following zsh commands in the project's directory, [Homebrew] will also be installed:

```zsh
% /usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
% brew install --cask dotnet-sdk
% cp src/Peeb.Bot/appsettings.Json src/Peeb.Bot/appsettings.Development.json
```

### Windows

Run the following PowerShell commands in the project's directory, [Chocolatey] will also be installed:

```powershell
> Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
> choco install dotnet-sdk
> copy src/Peeb.Bot/appsettings.Json src/Peeb.Bot/appsettings.Development.json
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

Deploy
------

Pushing to the `develop` or `master` branch will trigger an automated deployment via [GitHub Actions].

[.NET]: https://dotnet.microsoft.com
[Chocolatey]: https://chocolatey.org
[Github Actions]: https://github.com/peeb-bot/peeb-bot/actions
[Homebrew]: https://brew.sh
