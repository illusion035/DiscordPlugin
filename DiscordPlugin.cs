using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace DiscordPlugin
{
    public class DiscordPluginConfig : BasePluginConfig
    {
        [JsonPropertyName("discord_link")]
        public string DiscordLink { get; set; } = "{orange}Join our Discord server: {lightblue}https://discord.gg/yourdiscordlink";

        [JsonPropertyName("bot_token")]
        public string BotToken { get; set; } = "your_bot_token";

        [JsonPropertyName("server_id")]
        public string ServerId { get; set; } = "your_server_id";

        [JsonPropertyName("member_counts_message")]
        public string MemberCountsMessage { get; set; } = "{purple}Total members: {total_members} {silver}| {green}Online members: {online_members}";

        [JsonPropertyName("show_total_stats")]
        public bool ShowTotalStats { get; set; } = true;

        [JsonPropertyName("cache_duration_seconds")]
        public int CacheDurationSeconds { get; set; } = 60;

        [JsonPropertyName("ConfigVersion")]
        public override int Version { get; set; } = 1;
    }

    public class DiscordPlugin : BasePlugin, IPluginConfig<DiscordPluginConfig>
    {
        public override string ModuleName => "DiscordPlugin";
        public override string ModuleVersion => "1.0";
        public override string ModuleAuthor => "illusion";
        public override string ModuleDescription => "Sends a Discord invite link and server stats to players upon command.";

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

        private static readonly string AssemblyName = typeof(DiscordPlugin).Assembly.GetName().Name ?? "";
        private static readonly string CfgPath = $"{Server.GameDirectory}/csgo/addons/counterstrikesharp/configs/plugins/{AssemblyName}/{AssemblyName}.json";

        private (int totalMembers, int onlineMembers) _cachedCounts = (0, 0);
        private DateTime _lastCacheUpdate = DateTime.MinValue;

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

        private async Task<(int totalMembers, int onlineMembers)> GetDiscordMemberCountsAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", Config.BotToken);

            var response = await client.GetAsync($"https://discord.com/api/v10/guilds/{Config.ServerId}?with_counts=true");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch Discord server info: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var json = JsonDocument.Parse(content);
                var root = json.RootElement;
                
                if (root.TryGetProperty("approximate_member_count", out var memberCountProp) &&
                    root.TryGetProperty("approximate_presence_count", out var onlineCountProp))
                {
                    var totalMembers = memberCountProp.GetInt32();
                    var onlineMembers = onlineCountProp.GetInt32();
                    return (totalMembers, onlineMembers);
                }
                else
                {
                    throw new Exception("Could not find member count data in the response.");
                }
            }
            catch (JsonException ex)
            {
                throw new Exception($"Error parsing response: {ex.Message}");
            }
        }

        private async Task<(int totalMembers, int onlineMembers)> GetCachedDiscordMemberCountsAsync()
        {
            if ((DateTime.Now - _lastCacheUpdate).TotalSeconds > Config.CacheDurationSeconds)
            {
                var counts = await GetDiscordMemberCountsAsync();
                _cachedCounts = counts;
                _lastCacheUpdate = DateTime.Now;
            }
            return _cachedCounts;
        }

        private HookResult OnDiscordCommand(CCSPlayerController? player, CommandInfo message)
        {
            if (player == null || !player.IsValid || player.IsBot)
            {
                return HookResult.Handled;
            }

            string commandArgument = message.GetArg(0);

            if (string.IsNullOrEmpty(commandArgument))
            {
                return HookResult.Handled;
            }

            string discordMessage = ReplaceColorPlaceholders(Config.DiscordLink);

            var (totalMembers, onlineMembers) = Task.Run(async () => await GetCachedDiscordMemberCountsAsync()).Result;

            player.PrintToChat(discordMessage);

            if (Config.ShowTotalStats)
            {
                string memberCountsMessage = Config.MemberCountsMessage
                    .Replace("{total_members}", totalMembers.ToString())
                    .Replace("{online_members}", onlineMembers.ToString());

                memberCountsMessage = ReplaceColorPlaceholders(memberCountsMessage);

                player.PrintToChat(memberCountsMessage);
            }

            return HookResult.Handled;
        }
    }
}
