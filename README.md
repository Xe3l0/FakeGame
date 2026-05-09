# 🎮 FakeExe
> Spoof your Discord Rich Presence — make it look like you're playing any game.

![Platform](https://img.shields.io/badge/platform-Windows-blue)
![Language](https://img.shields.io/badge/language-C%23-purple)
![License](https://img.shields.io/badge/license-MIT-green)

---

## ⚡ Installation

### Step 1 — Install .NET SDK

FakeExe requires .NET to run. Download it here:

👉 **[Download .NET SDK](https://aka.ms/dotnet/download)**

Choose **Windows x64** and install it. Takes about 2 minutes.

---

### Step 2 — Run the Installer

1. Download the project files
2. **Double-click `install.bat`**
3. FakeExe will build itself and appear on your Desktop automatically

> ✅ No CMD, no commands, no hassle.

---

## 🛠 How to Use

| Field | Description |
|-------|-------------|
| **Discord Application ID** | Your app ID from Discord Developer Portal — leave default for basic use |
| **Game Name** | The game name you want to show to your friends |
| **Details** | First line under the game name — e.g. `In the main menu` |
| **State** | Second line — e.g. `Level 12 \| Ranked Match` |
| **Large Image Key** | Image asset key from your Discord app (optional) |

Fill in the fields and hit **▶ Start Presence** — then check Discord! 🎮

---

## ⚠️ Important — Game Name Display

Discord shows your **application name** from the Developer Portal as the main title, not the name you type in the app.

To show the correct game name:

1. Go to 👉 [discord.com/developers/applications](https://discord.com/developers/applications)
2. Open your app or create a free one
3. Change the **App Name** to the game you want to show
4. Save and restart FakeExe

> Example: Want to appear playing `Minecraft`? Set your App Name to `Minecraft`.

---

## 💾 Settings

FakeExe automatically saves your settings to:
```
C:\Users\[YourName]\AppData\Roaming\FakeExe\settings.txt
```
Everything is restored next time you open the app ✅

---

## 🔧 Build Manually

If you prefer to build it yourself:

```bash
git clone https://github.com/YourUsername/FakeExe
cd FakeExe
dotnet publish -c Release
```

The `.exe` will be at:
```
bin\Release\net6.0-windows\win-x64\publish\FakeExe.exe
```

---

## 📋 Requirements

- Windows 10 or later
- [.NET 6 SDK](https://aka.ms/dotnet/download)
- Discord installed and running

---

## 📄 License

MIT — free to use, modify, and share.
