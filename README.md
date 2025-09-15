Desktop App with Control Buttons in C# for Raspberry Pi 64‐bit

Welcome to the vspi desktop application project! This is a cross-platform C# desktop app built using Avalonia UI for deployment on Raspberry Pi 64-bit systems.

🚀 Getting Started

✅ Prerequisites Visual Studio 2022 (VS22) installed on Windows

.NET SDK installed

Raspberry Pi with 64-bit Linux OS

sudo apt install playerctl mpv

🛠️ Setup Instructions

Open Developer Command Prompt for Visual Studio

Go to: Tools → Command Line → Developer Command Prompt

Install Avalonia UI Templates

dotnet new install Avalonia.Templates

dotnet workload restore

Publish for Raspberry Pi (ARM64)

dotnet publish -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false

Deploy to Raspberry Pi (in pi terminal)

mkdir vspi

copy content from folder publish (on windows pc) into the vspi folder of your pi (C:\Users\dell\source\repos\vspi\bin\Release\net9.0\linux-arm64\publish)

cd ~/vspi
chmod +x vspi
./vspi

🧩 Features Simple GUI with control buttons

Built with Avalonia UI (cross-platform support)

Runs natively on ARM64 Raspberry Pi devices

📦 Technologies Used C#

.NET 9

Avalonia UI

Raspberry Pi 64-bit Linux

android version

https://play.google.com/store/apps/details?id=vspi.vspi

windows version in folder vspi-app_x.x.x.x_windows
