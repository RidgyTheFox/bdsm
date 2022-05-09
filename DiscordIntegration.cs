using System;
using UnityEngine;
using Discord;

namespace BDSM
{
    /// <summary>
    /// This enum contains all available activities for displaying in Discord Rich Presence.
    /// </summary>
    public enum AvailbaleActivities : ushort
    {
        IN_MAIN_MENU = 0,
        IN_SINGLEPLAYER = 1,
        IN_MULTIPLAYER = 2
    }

    /// <summary>
    /// This class taking control over Discord Rich Presence integration.
    /// </summary>
    public class DiscordIntegration : MonoBehaviour
    {
        private Discord.Discord _discord;
        private ActivityManager _activityManager;

        // These activityes can be displayed in user profile in Discord.
        private Activity _inMainMenuActivity;
        private Activity _inSinglePlayerActivity;
        private Activity _inMultiplayerActivity;

        public string currentMap = "";
        public int playersAmount = 1;
        public int playersLimit = 10;

        /// <summary>
        /// This method initializing everything with some basic data for next usage.
        /// </summary>
        private void Awake()
        {
            Debug.Log("DISCORD_INTEGRATION: Initializing...");
            StaticData.discordIntegrationInstance = this;
            _discord = new Discord.Discord(952984020109623398, (System.UInt64)Discord.CreateFlags.Default);
            _activityManager = _discord.GetActivityManager();

            Debug.Log("DISCORD_INTEGRATION: Creating activities...");
            _inMainMenuActivity = new Activity {
                State = "In main menu...",
                Assets = {
                    // TODO: Replace temp logo with new one. (_inMainMenuActivity)
                    LargeImage = "bds_temp_logo",
                    LargeText = "Bus Driver Simulator"
                }
            };

            _inSinglePlayerActivity = new Activity {
                State = "Playing in singleplayer",
                Details = $"Driving Vector Next in Serpukhov...",
                Assets = {
                    // TODO: Replace temp logo with new one. (_inSingleplayerActivity)
                    LargeImage = "bds_temp_logo",
                    LargeText = "Bus Driver Simulator"
                }
            };

            _inMultiplayerActivity = new Activity {
                State = "Playing in multiplayer!",
                Details = "Driving Vector Next in Serpukhov...",
                Assets = {
                    // TODO: Replace temp logo with new one. (_inMultiplayerActivity)
                    LargeImage = "bds_temp_logo",
                    LargeText = "Bus Driver Simulator"
                }
            };

            UpdateActivity(AvailbaleActivities.IN_MAIN_MENU);
        }

        private void FixedUpdate()
        {
            _discord.RunCallbacks();
        }

        /// <summary>
        /// To display players count in activity, you should provide secret and public keys for lobby in DRP. So, use this function to get random symbol-numeric key with 32 letters length.
        /// </summary>
        /// <returns></returns>
        private string GenerateRandomSecretKey()
        {
            const uint keyLength = 32;
            string output = "";
            string availableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            DateTime foo = DateTime.Now;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            var r = new System.Random((int)unixTime);

            for (int i = 0; i <= keyLength; i++)
            {
                int selection = r.Next(0, 2);

                switch(selection)
                {
                    case 0: // Add number.
                        output += r.Next(0, 10);
                        break;
                    case 1: // Add letter.
                        output += availableChars[r.Next(0, availableChars.Length)];
                        break;
                }
            }

            Debug.LogWarning($"DISCORD_INTEGRATION: New secret key for party was created. Key is: {output}");
            return output;
        }

        /// <summary>
        /// This function returns full bus name by him short name.
        /// </summary>
        /// <param name="l_shortname">bus short name.</param>
        /// <returns></returns>
        private string GetFullBusNameByShortname(string l_shortname)
        {
            switch (l_shortname)
            {
                case "VN":
                    return "Vector Next";
                case "CT":
                    return "Citaro";
                case "BIGCT":
                    return "Citaro L";
                case "IC":
                    return "Icarus";
                case "LA":
                    return "LAZ695";
                case "TOURIST":
                    return "LAZ 699";
                case "LZ5292":
                    return "LIAZ 5292";
                case "LZ":
                    return "LIAZ 677";
                case "BIGMN":
                    return "MAN 15";
                case "MN":
                    return "MAN";
                case "PZ":
                    return "PAZ 3205";
                case "OLDPZ672":
                    return "PAZ 672";
                case "SPR":
                    return "Sprinter";
                default:
                    return "Vector Next";
            }
        }

        /// <summary>
        /// This function will update internal activities classes and will diplay it in user profile in Discord.
        /// </summary>
        /// <param name="l_activityForUpdate"></param>
        public void UpdateActivity(AvailbaleActivities l_activityForUpdate)
        {
            DateTime foo = DateTime.Now;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            string currentBus;

            // This try-catch block was used to check for a saved game.
            // If the player has never entered the campaign mode and has not bought a bus, then we will not be able to read the data and will get an exception.
            try
            {
                currentBus = FreeMode.PlayerData.GetCurrentData().SelectedBusData.ShortName;
            }
            catch (ArgumentOutOfRangeException e)
            {
                currentBus = "Vector Next";
                Debug.LogError($"DISCORD_INTEGRATION: Exception while try to get bus name. Cannot load save data! Falling back to default...");
            }

            // This switch fills up selected activity and displaying it.
            switch (l_activityForUpdate)
            {
                case AvailbaleActivities.IN_MAIN_MENU:
                    _activityManager.UpdateActivity(_inMainMenuActivity, (res) => {
                        if (res == Discord.Result.Ok)
                            Debug.Log("DISCORD_INTEGRATION: Activity \"_inMainMenu\" was set!");
                        else
                            Debug.LogError("DISCORD_INTEGRATION: Cannot set \"_inMainMenu\" activity!");
                    });
                    break;
                case AvailbaleActivities.IN_SINGLEPLAYER:
                    _inSinglePlayerActivity.State = "Playing in singleplayer.";
                    _inSinglePlayerActivity.Details = $"Driving {GetFullBusNameByShortname(currentBus)} in {currentMap}";
                    _inSinglePlayerActivity.Timestamps.Start = unixTime;

                    _activityManager.UpdateActivity(_inSinglePlayerActivity, (res) => {
                        if (res == Discord.Result.Ok)
                            Debug.Log("DISCORD_INTEGRATION: Activity \"_inSingleplayerActivity\" was set!");
                        else
                            Debug.LogError("DISCORD_INTEGRATION: Cannot set \"_inSinglePlayerAcitivty\" activity!");
                    });
                    break;
                case AvailbaleActivities.IN_MULTIPLAYER:
                    _inMultiplayerActivity.State = "Playing in multiplayer!";
                    _inMultiplayerActivity.Details = $"Driving {GetFullBusNameByShortname(currentBus)} in {currentMap}";
                    _inMultiplayerActivity.Timestamps.Start = unixTime;
                    _inMultiplayerActivity.Party.Size.MaxSize = playersLimit;
                    _inMultiplayerActivity.Party.Size.CurrentSize = playersAmount;
                    _inMultiplayerActivity.Secrets.Join = GenerateRandomSecretKey();

                    _activityManager.UpdateActivity(_inMultiplayerActivity, (res) => {
                        if (res == Discord.Result.Ok)
                            Debug.Log("DISCORD_INTEGRATION: Activity \"_inMultiplayerActivity\" was set!");
                        else
                            Debug.LogError("DISCORD_INTEGRATION: Cannot set \"_inMultiplayerActivity\" activity!");
                    });
                    break;
                default: return;
            }
        }
    }
}
