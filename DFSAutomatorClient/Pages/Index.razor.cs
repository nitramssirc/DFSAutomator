using DFSAutomatorClient.Models;
using DFSAutomatorClient.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Tewr.Blazor.FileReader;
using BlazorDownloadFile;

namespace DFSAutomatorClient.Pages
{
    public partial class Index
    {
        [Inject] ICSVLoader _csvLoader { get; set; }
        [Inject] public IFileReaderService FileReaderService { get; set; }
        [Inject] IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

        protected ElementReference _vegasSpreadsUpload;
        protected ElementReference _playerUpload;

        public TeamVegasSpread[] VegasSpreads { get; private set; }
        public Player[] Players { get; private set; }
        public Dictionary<string, decimal> TeamMaxPcts = new Dictionary<string, decimal>();
        public List<SalaryTier> SalaryTiers = new List<SalaryTier>();

        private string salaryTierPos;
        private int? salaryTierMin;
        private int? salaryTierMax;
        private decimal? salaryTierPct;

        #region Client Events

        async void ImportSpreads()
        {
            var files = await FileReaderService.CreateReference(_vegasSpreadsUpload).EnumerateFilesAsync();
            await using var memoryStream = await files.First().CreateMemoryStreamAsync();
            VegasSpreads = _csvLoader.Load<TeamVegasSpread>(memoryStream).OrderByDescending(s => s.RankValue).ToArray();
            CalculateTeamMaxPcts();

            StateHasChanged();
        }

        async void ImportPlayers()
        {
            var files = await FileReaderService.CreateReference(_playerUpload).EnumerateFilesAsync();
            await using var memoryStream = await files.First().CreateMemoryStreamAsync();
            Players = _csvLoader.Load<Player>(memoryStream).OrderByDescending(p => p.proj_own).ToArray();
            CalculatePlayerMaxPct();

            StateHasChanged();
        }

        async Task SavePlayers()
        {
            var csvString = _csvLoader.Generate(Players);
            if (csvString != null)
            {
                await BlazorDownloadFileService.DownloadFileFromText("Players_MaxExposure.csv", csvString,
                    "application/octet-stream");
            }
        }

        private void ClearSpreads()
        {
            VegasSpreads = new TeamVegasSpread[0];
        }

        private void ClearPlayers()
        {
            Players = new Player[0];
        }

        private void AddSalaryTier()
        {
            if (salaryTierPos == null || !salaryTierMin.HasValue || !salaryTierMax.HasValue || !salaryTierPct.HasValue) return;
            
            SalaryTiers.Add(new SalaryTier(salaryTierPos, salaryTierMin.Value, salaryTierMax.Value, salaryTierPct.Value));
            salaryTierPos = null;
            salaryTierMin = null;
            salaryTierMax = null;
            salaryTierPct = null;

            CalculatePlayerMaxPct();
        }

        #endregion

        #region Private Methods

        private void CalculateTeamMaxPcts()
        {
            var teamCount = VegasSpreads.Length;
            var maxPct = 25;
            var maxValue = VegasSpreads.Max(vs => vs.RankValue);
            TeamMaxPcts = new Dictionary<string, decimal>();
            foreach (var spread in VegasSpreads)
            {
                spread.Team = MapSpreadTeamToPlayerTeam(spread.Team);
                var rankPct = spread.RankValue / maxValue;
                var teamMaxPct = Math.Round(maxPct * rankPct, 2);
                TeamMaxPcts.Add(spread.Team, teamMaxPct);
            }
        }

        private void CalculatePlayerMaxPct()
        {
            foreach (var player in Players)
            {
                var teamMaxPct = TeamMaxPcts[player.team];
                var playerOwnership = player.proj_own;
                var playerSalaryTierPct = GetSalaryTierForPlayer(player);

                player.fpts = playerOwnership >= 1 ? player.fpts : 0;
                player.custom = CalculatePlayerWeightedFantasyPts(player);
                player.max_exposure = playerOwnership >= 1 ? CalculateWeightedMaxExposure(teamMaxPct, playerSalaryTierPct) : 0;
            }
        }

        private decimal CalculateWeightedMaxExposure(decimal teamMaxPct, decimal? playerSalaryTierPct)
        {
            if (!playerSalaryTierPct.HasValue) return teamMaxPct;
            return teamMaxPct * 0.75M +
                playerSalaryTierPct.Value * 0.25M;
        }

        private decimal CalculatePlayerWeightedFantasyPts(Player player)
        {
            return (player.floor ?? 0) * .2M +
                    (player.fpts ?? 0) * .5M +
                    (player.ceil ?? 0) * .3M;
        }

        private string MapSpreadTeamToPlayerTeam(string spreadTeam)
        {
            switch (spreadTeam)
            {
                case "NOS": return "NO";
                case "TBB": return "TB";
                case "JAC": return "JAX";
                case "SFO": return "SF";
                case "LVR": return "LV";
                case "KCC": return "KC";
                case "GBP": return "GB";
                case "NEP": return "NE";
                default: return spreadTeam;
            }
        }

        private decimal? GetSalaryTierForPlayer(Player player)
        {
            var salaryTier = SalaryTiers.FirstOrDefault(st => st.Pos == player.pos && player.salary >= st.Min && player.salary <= st.Max);
            return salaryTier?.Pct;
        }

        #endregion
    }
}
