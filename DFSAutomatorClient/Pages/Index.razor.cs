using DFSAutomatorClient.Models;
using DFSAutomatorClient.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Tewr.Blazor.FileReader;

namespace DFSAutomatorClient.Pages
{
    public partial class Index
    {
        [Inject] ICSVLoader _csvLoader { get; set; }
        [Inject] public IFileReaderService FileReaderService { get; set; }

        protected ElementReference _vegasSpreadsUpload;

        public TeamVegasSpread[] VegasSpreads { get; private set; }

        async void ImportSpreads()
        {
            var files = await FileReaderService.CreateReference(_vegasSpreadsUpload).EnumerateFilesAsync();
            await using var memoryStream = await files.First().CreateMemoryStreamAsync();
            VegasSpreads = _csvLoader.Load<TeamVegasSpread>(memoryStream).ToArray();
            StateHasChanged();
        }
    }
}
