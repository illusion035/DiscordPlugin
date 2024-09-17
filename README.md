# Discord Plugin for CS2

This plugin integrates Discord functionality into your CS2 server, allowing players to get information about the server's Discord community and server stats directly from the game.

## Features

- **Discord Invite Link**: Displays invite link to your Discord server.
- **Server Stats**: Optionally displays total and online member counts from your Discord server.
- **Customizable**: Configurable via a JSON file for easy adjustments.

## Installation

1. **Download**: The last release in this page

2. **Place the Plugin**: Copy the plugin files to your CS2 plugins directory.

    ```plaintext
    csgo/addons/counterstrikesharp/plugins/
    ```

3. **Configure**: Edit the configuration file to set your Discord server details.

    ```plaintext
    csgo/addons/counterstrikesharp/configs/plugins/DiscordPlugin/DiscordPlugin.json
    ```

## Configuration

Edit the `DiscordPlugin.json` file to configure the plugin. Here’s a breakdown of the configuration options:

```json
{
    "discord_link": "{orange}Join our Discord server: {lightblue}https://discord.gg/yourdiscordlink",
    "bot_token": "your_bot_token",
    "server_id": "your_server_id",
    "member_counts_message": "{purple}Total members: {total_members} {silver}| {green}Online members: {online_members}",
    "show_total_stats": true,
    "ConfigVersion": 1
}
```

## Image
![image](https://github.com/user-attachments/assets/63f3c0d9-8fdd-4a4c-96d2-739d2cffe7a1)
