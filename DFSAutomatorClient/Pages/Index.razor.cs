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

        #region Client Events

        async void ImportSpreads()
        { 
            var files = await FileReaderService.CreateReference(_vegasSpreadsUpload).EnumerateFilesAsync();
            await using var memoryStream = await files.First().CreateMemoryStreamAsync();
            VegasSpreads = _csvLoader.Load<TeamVegasSpread>(memoryStream).OrderByDescending(s=>s.RankValue).ToArray();
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

        #endregion

        #region Private Methods

        private void CalculateTeamMaxPcts()
        {
            var teamCount = VegasSpreads.Length;
            var maxPct = 40;
            TeamMaxPcts = new Dictionary<string, decimal>();
            for (int rank = 0; rank < teamCount; rank++)
            {
                var spread = VegasSpreads[rank];
                var rankPct = (decimal)rank / teamCount;
                var teamMaxPct = Math.Round(maxPct - (maxPct * rankPct), 2);
                TeamMaxPcts.Add(spread.Team, teamMaxPct);
            }
        }

        private void CalculatePlayerMaxPct()
        {
            foreach (var player in Players)
            {
                var teamMaxPct = TeamMaxPcts[player.team];
                var playerOwnership = player.proj_own;
                player.fpts = playerOwnership >= 1 ? player.fpts : 0;
                player.max_exposure = playerOwnership >= 1 ? teamMaxPct : 0;
            }
        }

        #endregion
    }
}
