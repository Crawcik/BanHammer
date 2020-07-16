using Smod2;
using Smod2.API;
using Smod2.Commands;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;
using System.Linq;

namespace BanHammer
{
    [Smod2.Attributes.PluginDetails(
        author = "Crawcik",
        description = "Ban Hammer",
        id = "banhammer",
        name = "Ban Hammer",
        SmodMajor = 3,
        SmodMinor = 7,
        SmodRevision = 0,
        version = "1.1"
        )]
    public class BanHammer : Plugin, ICommandHandler, IEventHandlerPlayerHurt, IEventHandlerRoundRestart
    {
        private string[] acceptableRanks;
        private readonly List<int> enabledPlayers = new List<int>();

        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            if (enabledPlayers.Contains(ev.Attacker.PlayerId))
                ev.Player.Disconnect("Banhammered");
        }
        public void OnRoundRestart(RoundRestartEvent ev)
        {
            enabledPlayers.Clear();
        }

        #region Setup
        public override void OnDisable()
        {
            this.EventManager.RemoveEventHandlers(this);
        }

        public override void OnEnable()
        {
            acceptableRanks = this.GetConfigList("banhammer_allowed_ranks");
            this.AddEventHandler(typeof(IEventHandlerPlayerHurt), this, Priority.Low);
        }

        public override void Register()
        {
            this.AddCommand("banhammer", this);
        }
        #endregion

        #region CommandHandler
        public string[] OnCall(ICommandSender sender, string[] args)
        {
            Player player = sender as Player;
            if (!acceptableRanks.Contains(player.GetRankName()))
                return new string[] {"You're not allowed to use this command!" };
            if(args.Length != 1)
                return new string[] { GetCommandDescription(), GetUsage() };
            if (args[0] == "on")
            {
                if (!enabledPlayers.Contains(player.PlayerId))
                    enabledPlayers.Add(player.PlayerId);
                return new string[] { "Banhammer is ON! Use it carefully" };
            }
            else if (args[0] == "off")
            {
                if (enabledPlayers.Contains(player.PlayerId))
                    enabledPlayers.Remove(player.PlayerId);
                return new string[] { "Banhammer is OFF! You can now shot anyone safely" };
            }
            return new string[] { "You did something wrong!", GetUsage() };

        }

        public string GetUsage() => "banhammer [on/off]";

        public string GetCommandDescription() => "Ban Hammer give you ability to kick people you hurt";
        #endregion
    }
}
