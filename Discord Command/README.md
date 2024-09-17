# DiscordPlugin for CS2

## Overview

The `DiscordPlugin` is a plugin for Counter-Strike 2 that allows players to receive an invite link to your Discord server through a chat command. This plugin is designed to be easy to configure and use, providing a straightforward way to promote your community server.

## Features

- **Discord Command**: Type `!discord` in the chat to receive a Discord invite link.
- **Customizable Link**: Configure the Discord invite link and message formatting in the plugin's configuration file.
- **Color Formatting**: Use customizable color codes to style your message.

## Installation

1. **Download the Plugin**: Clone or download the repository.
2. **Place the Plugin**: Move the plugin file to your CS2 server's plugin directory.
3. **Configure the Plugin**: Edit the configuration file to include your Discord invite link and customize the message.

## Usage

To use the plugin, simply type `!discord` in the chat. The plugin will send the configured Discord invite link formatted with the specified colors.

## Configuration

The configuration file (`DiscordPlugin.json`) is automatically generated and can be customized as needed. Below is an example of the configuration file:

```json
{
  "discord_link": "{orange}Join our Discord server: {lightblue}https://discord.gg/yourdiscordlink",
  "ConfigVersion": 1
}
