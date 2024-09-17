using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Text.Json;

namespace DiscordPlugin
{
    public class DiscordPluginConfig : BasePluginConfig
    {
        [JsonPropertyName("discord_link")]
        public string DiscordLink { get; set; } = "{orange}Join our Discord server: {lightblue}https://discord.gg/yourdiscordlink";

        [JsonPropertyName("ConfigVersion")]
        public override int Version { get; set; } = 1;
    }

    public class DiscordPlugin : BasePlugin, IPluginConfig<DiscordPluginConfig>
    {
        public override string ModuleName => "DiscordPlugin";
        public override string ModuleVersion => "1.0";
        public override string ModuleAuthor => "illusion";
        public override string ModuleDescription => "Sends a Discord invite link to players upon command.";

        public DiscordPluginConfig Config { get; set; } = null!;

        private static readonly Dictionary<string, char> ColorMap = new Dictionary<string, char>
        {
            { "{default}", ChatColors.Default },
            { "{white}", ChatColors.White },
            { "{darkred}", ChatColors.DarkRed },
            { "{green}", ChatColors.Green },
            { "{lightyellow}", ChatColors.LightYellow },
            { "{lightblue}", ChatColors.LightBlue },
            { "{olive}", ChatColors.Olive },
            { "{lime}", ChatColors.Lime },
            { "{red}", ChatColors.Red },
            { "{lightpurple}", ChatColors.LightPurple },
            { "{purple}", ChatColors.Purple },
            { "{grey}", ChatColors.Grey },
            { "{yellow}", ChatColors.Yellow },
            { "{gold}", ChatColors.Gold },
            { "{silver}", ChatColors.Silver },
            { "{blue}", ChatColors.Blue },
            { "{darkblue}", ChatColors.DarkBlue },
            { "{bluegrey}", ChatColors.BlueGrey },
            { "{magenta}", ChatColors.Magenta },
            { "{lightred}", ChatColors.LightRed },
            { "{orange}", ChatColors.Orange }
        };

        public void OnConfigParsed(DiscordPluginConfig config)
        {
            Config = config;
            UpdateConfig(config);
        }

        private static readonly string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "";
        private static readonly string CfgPath = $"{Server.GameDirectory}/csgo/addons/counterstrikesharp/configs/plugins/{AssemblyName}/{AssemblyName}.json";

        private void UpdateConfig<T>(T config) where T : BasePluginConfig, new()
        {
            var newCfgVersion = new T().Version;

            if (config.Version == newCfgVersion)
                return;

            config.Version = newCfgVersion;

            var updatedJsonContent = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(CfgPath, updatedJsonContent);

            Console.WriteLine($"Config updated for V{newCfgVersion}.");
        }

        public override void Load(bool hotReload)
        {
            AddCommandListener("discord", OnDiscordCommand);
            Console.WriteLine("DiscordPlugin loaded and command listener added.");
        }

        private string ReplaceColorPlaceholders(string message)
        {
            if (!string.IsNullOrEmpty(message) && message[0] != ' ')
            {
                message = " " + message;
            }

            foreach (var colorPlaceholder in ColorMap)
            {
                message = message.Replace(colorPlaceholder.Key, colorPlaceholder.Value.ToString());
            }
            return message;
        }

        private HookResult OnDiscordCommand(CCSPlayerController? player, CommandInfo message)
        {
            if (player == null)
            {
                return HookResult.Handled;
            }

            if (!player.IsValid)
            {
                return HookResult.Handled;
            }

            if (player.IsBot)
            {
                return HookResult.Handled;
            }

            string commandArgument = message.GetArg(0);

            if (string.IsNullOrEmpty(commandArgument))
            {
                return HookResult.Handled;
            }

            string discordMessage = ReplaceColorPlaceholders(Config.DiscordLink);
            player.PrintToChat(discordMessage);

            return HookResult.Handled;
        }
    }
}
