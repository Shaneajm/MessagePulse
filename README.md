<div align="center">
  <img src="https://i.imgur.com/hgBU4O5.png"
      style="margin-bottom: -20px;" />
  <div>
    <p style="margin-bottom: -10px;">
      <span style="font-size: 28px;">MESSAGE - ANNOUNCEMENT SYSTEM</span>
      </br>
      <span style="font-size: 16px;">BUILT FOR CS2 USING SWIFTLY FRAMEWORK</span>
      <hr>
    </p>
  </div>
</div>

<p align="center">
  <img src="https://img.shields.io/github/downloads/itsAudioo/MessagePulse/total?style=for-the-badge&logo=dotnet" alt="Downloads">
  <img src="https://img.shields.io/github/stars/itsAudioo/MessagePulse?style=for-the-badge&logo=macys&logoColor=yellow" alt="Stars">
</p>

<div align="center">
  <table>
    <tr>
      <td align="center" style="vertical-align: middle; padding-right: 32px;">
        <p style="font-size: 18px;"><strong>If you like this project consider leaving a star ⭐</strong></p>
      </td>
      <td align="center" style="vertical-align: middle;">
        <a href="https://github.com/itsAudioo/MessagePulse/stargazers">
          <img src="https://i.imgur.com/D9GJFM8.png" height="40px" alt="Star on GitHub"/>
        </a>
      </td>
    </tr>
  </table>
</div>

## 📖 Description
## NOTE: OUT OF DATE WITH RECENT SWIFTLY CHANGES
**MessagePulse** is a feature-rich plugin for SwiftlyS2 that enhances server communication and player engagement. It provides a centralized system for handling various message types and interactions.

**Key Features:**

- **Event-Based Messages**: Automatically send messages based on game events (e.g., player connect, round end, player death).
- **Translation Support**: Each player gets messages in their preferred language.
- **Custom Commands**: Create custom chat commands with ease.
- **Scheduled Broadcasts**: Rotate advertisements and announcements to keep players informed.
- **Dead Show Image**: Display images to players when they are spectating.

## 📦 Requirements

To ensure MessagePulse works correctly, please make sure you have the following:

- **Counter-Strike 2 Server** (Working installation)
- **[SwiftlyS2](https://github.com/swiftly-solution/swiftlys2)** (Latest Release)
- **[PlaceholderAPI](https://github.com/SwiftlyS2-Plugins/PlaceholderAPI)**
  - _Required for dynamic placeholders in messages._
- **[MenuFlickeringFix](https://github.com/SwiftlyS2-Plugins/MenuFlickeringFix)**
  - _Required **only** if using the **Dead Show Image** feature to prevent UI flickering._

## 🚀 Installation

1.  **Download**: Go to the [Releases Page](https://github.com/itsAudioo/MessagePulse/releases) and download the latest release.
2.  **Install**: Extract the contents of the downloaded zip file into your server's `addons/swiftlys2/plugins` folder.
3.  **Initialize**: Start or restart your server.
    - Example configuration files will be automatically generated in `{swRoot}/configs/plugins/MessagePulse` on the first load.

## ⚙️ Configuration

You can fully customize MessagePulse by editing the configuration files located in `{swRoot}/configs/plugins/MessagePulse`.

### Placeholders

MessagePulse utilizes **PlaceholderAPI** to support dynamic values in your messages (e.g., `{PLAYERNAME}`, `{NEXTMAP}`, `{TIME}`).

> 🔗 **For a complete list of available placeholders, please visit the [PlaceholderAPI Repository](https://github.com/SwiftlyS2-Plugins/PlaceholderAPI).**

## 💻 Credits

[K4ryuu](https://github.com/K4ryuu) - Dynamic event hooking system
