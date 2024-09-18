# Discord Plugin for CS2

This plugin integrates Discord functionality into your CS2 server, allowing players to get information about the server's Discord community and server stats directly from the game.

## Features

- **Discord Invite Link**: Displays an invite link to your Discord server.
- **Server Stats**: Optionally displays total and online member counts from your Discord server.
- **API Status Check**: Added functionality to check if the Discord API is offline. If the API is down, a custom message is displayed instead of showing the total and online members.
- **API Error Message Configuration**: Customizable error message when the Discord API is not working.
- **Show Link Only on API Error**: Option to show only the Discord invite link when the API is down, hiding member counts.
- **Caching**: Implements caching to reduce server lag and improve performance. Cached server stats and API status are updated every minute.
- **Customizable**: Configurable via a JSON file for easy adjustments.

## Installation

1. **Download**: The latest release from this page.

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
    "api_error_message": "{red}Discord API not working or Discord Status is down",
    "show_link_only_on_error": true,
    "cache_duration_seconds": 60, // Duration in seconds for caching server stats and API status
    "ConfigVersion": 1
}
```

## Image
![image](https://github.com/user-attachments/assets/63f3c0d9-8fdd-4a4c-96d2-739d2cffe7a1)
