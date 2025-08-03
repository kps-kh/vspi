Desktop App with Control Buttons in C# for Raspberry Pi 64‐bit

Welcome to the vspi desktop application project! This is a cross-platform C# desktop app built using Avalonia UI for deployment on Raspberry Pi 64-bit systems.

🚀 Getting Started

✅ Prerequisites Visual Studio 2022 (VS22) installed on Windows

.NET SDK installed

Raspberry Pi with 64-bit Linux OS sudo apt install playerctl

🛠️ Setup Instructions Open Developer Command Prompt for Visual Studio

Go to: Tools → Command Line → Developer Command Prompt

Install Avalonia UI Templates

dotnet new install Avalonia.Templates

Publish for Raspberry Pi (ARM64)

dotnet publish -r linux-arm64 -c Release --self-contained true

Deploy to Raspberry Pi

mkdir vspi

copy content from folder public (on windows pc) into the vspi folder of your pi

cd ~/vspi chmod +x vspi ./vspi

🧩 Features Simple GUI with control buttons

Built with Avalonia UI (cross-platform support)

Runs natively on ARM64 Raspberry Pi devices

📦 Technologies Used C#

.NET 9

Avalonia UI

Raspberry Pi 64-bit Linux
