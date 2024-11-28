# Rusty Hearts Server Manager
[![License](https://img.shields.io/github/license/JuniorDark/RustyHearts-Server-Manager?color=green)](LICENSE)
[![Build](https://github.com/JuniorDark/RustyHearts-Server-Manager/actions/workflows/build.yml/badge.svg)](https://github.com/JuniorDark/RustyHearts-Server-Manager/actions/workflows/build.yml)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/JuniorDark/RustyHearts-Server-Manager)](https://github.com/JuniorDark/RustyHearts-Server-Manager/releases/latest) <a href="https://github.com/JuniorDark/RustyHearts-Server-Manager/releases">

A C# Windows Forms application designed to simplify the configuration and management of Rusty Hearts game servers. This tool provides an intuitive way to modify and manage various server/api parameters and start the servers with ease.

## Getting Started

To get started with this tool, simply download the latest release from the GitHub repository.

## Features

- **Server Configuration:** Easily configure various server settings, player limits, level cap, and more.
- **API Configuration:** Configure the [Rusty Hearts API](https://github.com/JuniorDark/RustyHearts-API) settings, including authentication credentials, endpoint URLs, and settings.
- **Server Startup:** Start the servers/API with a single button, ensuring that the configured settings are applied correctly.
- **Backup and Restore:** Create backups of the databases and restore them when needed.

## Preview
![image](preview/preview-01.png)
![image](preview/preview-02.png)
![image](preview/preview-03.png)
![image](preview/preview-04.png)
![image](preview/preview-05.png)

# Prerequisites for Development
* Visual Studio 2022 (Any Edition - 17.12 or later)
* Windows 10 SDK or Windows 11 SDK via Visual Studio Installer
* .NET Core 9 SDK (9.0.100 or later)

## Building

If you wish to build the project yourself, follow these steps:

### Step 1

Install the [.NET 9.0 (or higher) SDK](https://dotnet.microsoft.com/download/dotnet/9.0).
Make sure your SDK version is higher or equal to the required version specified. 

### Step 2

Either use `git clone https://github.com/JuniorDark/RustyHearts-Server-Manager` on the command line to clone the repository or use Code --> Download zip button to get the files.

### Step 3

To build the project, open a command prompt inside the project directory.
You can quickly access it on Windows by holding shift in File Explorer, then right clicking and selecting `Open command window here`.
Then type the following command: `dotnet build -c Release`.
 
The built files will be found in the newly created `bin` build directory.

## License
This project is licensed under the terms found in [`LICENSE-0BSD`](LICENSE).

## Credits
The following third-party libraries are used in this project:
* [System.Data.SqlClient](https://www.nuget.org/packages/System.Data.SqlClient)
