using Microsoft.Identity.Web;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using SiseveebRaptortid.Models;
using SiseveebRaptortid.ViewModels;

namespace SiseveebRaptortid.Services
{
    public class ApiService
    {
        private IConfiguration _configuration;
        private ITokenAcquisition _tokenAcquisition;
        private Uri _powerBiServiceApiRootUrl;
        private Guid _workspaceId;

        public const string PowerBiDefaultScope = "https://analysis.windows.net/powerbi/api/.default";

        public ApiService(IConfiguration configuration, ITokenAcquisition tokenAcquisition)
        {
            _configuration = configuration;
            _powerBiServiceApiRootUrl = new Uri(configuration["PowerBi:ServiceRootUrl"]);
            _workspaceId = new Guid(configuration["PowerBi:WorkspaceId"]);
            _tokenAcquisition = tokenAcquisition;
        }

        public string GetAccessToken()
        {
            //Authenticate as service principal
             return _tokenAcquisition.GetAccessTokenForAppAsync(PowerBiDefaultScope).Result;
        }

        public PowerBIClient GetPowerBiClient()
        {
            var tokenCredentials = new TokenCredentials(GetAccessToken(), "Bearer");
            return new PowerBIClient(_powerBiServiceApiRootUrl, tokenCredentials);
        }

        public async Task<WorkspaceViewModel> GetReportsEmbeddingData()
        {
            // Connect to Power BI
            var client = GetPowerBiClient();
            // Get reports in the workspace
            var reports = (await client.Reports.GetReportsInGroupAsync(_workspaceId)).Value;
            var reportList = new List<EmbeddedReport>();
            var reportTokenRequests = new List<GenerateTokenRequestV2Report>();

                foreach (var report in reports)
                {
                    {
                        reportList.Add(new EmbeddedReport
                        {
                            Id = report.Id.ToString(),
                            Name = report.Name,
                            EmbedUrl = report.EmbedUrl
                        });
                        reportTokenRequests.Add(new GenerateTokenRequestV2Report(report.Id, allowEdit: true));
                    }   
                 }

            // Get datasets in the workspace
            var datasets = (await client.Datasets.GetDatasetsInGroupAsync(_workspaceId)).Value;
            var datasetList = new List<EmbeddedDataset>();
            var datasetTokenRequests = new List<GenerateTokenRequestV2Dataset>();


            foreach (var dataset in datasets)
            {
                datasetList.Add(new EmbeddedDataset
                {
                    Id = dataset.Id.ToString(),
                    Name = dataset.Name,
                });
                datasetTokenRequests.Add(new GenerateTokenRequestV2Dataset(dataset.Id));
             } 

            // Bundle token requests for reports, datasets, and the workspace
            var tokenRequest = new GenerateTokenRequestV2(
                reports: reportTokenRequests,
                datasets: datasetTokenRequests
            );
           
            // Generate the embed token
              string embedToken = (await client.EmbedToken.GenerateTokenAsync(tokenRequest)).Token;
       
            //string embedToken =   "H4sIAAAAAAAEACWUxcr1CAJE3-XbZiBuA72Iu9tNdrEbd08z7z7f370vKDicqr9_7PTpp7T4-e8PieAD4ox6ZKGtrtv5GDyM2g_yaUnIzolVrc7-rcyg7m4XGDjMY9YNz8W3nvabBe3k3H35qi2Ae5xB35iMIIA6HcNM9lpGn8oqapZEKjY4fxnMue37CxNeSCW61GcjMRqtoce8G0GinYdAV76nMu53GBlK1EX434IrYXbum5u1OM3DPDI9Rd9UUZPifnK6HhHT61Fw3tPwLukBlzx6qI0nTlOuOBkL3ODgqz-Dsl7ukRkeVSlFQ6L6g3pORsCvfIVnc28Qubml2SmzL6zJEcUXrFcBgXrhFZODlXuLo-m0tKxL-fJqAlzWhEsq_RkxByLB5RMLIiH1hDj4eoJ5cxDtKoacVZlcWUepRkAIeZMlVZ2JpvAVnUYjZe8Fyc46gMnbbJKswhYuSWv-7Is95JwXSauYC4SOFvbATV_GHi67HPdtloEzpZyL_752SIa7yrzqMUw1f9TPSybSPDqv0HIsnyyJzctxJtwQbnWsQ15xYA2qfQBqSLfnlCtSVIlozD5nBgOoRdz2yEMISkt1ZnMaEEU7WLaqGoKW14mZv6JEVyiWZzxtRGcA57hyBILVR7IoTGQTYUz3I3mvj4_fwdfyebDrkFrkubxv5Lit7RruRm7FR9FRyAvGdyUWOCvMe81hl0bFX0BT-4NmLKVFl8KSC4KOpaz9FvWsBRvO1O3aAjUERFUw2ai_hxkXWEkf3B206wqJQodp81iN0Oax9cW8BaD_60r9hGBvHkRTLJgtt97q2WjMGHdTAJrP9QwY_BJbFdbUmU5OTWseCneA6GdMrtkMY1gLQ-KgnKhVMU_2j6Dua7nu0dfRN2hk-NbFhw-mGkY8XYKlOnUpJ3FvjRRI-IRVuG1uegyg9wWBocu5cifN5oiwkmDH4EN080WxelhuklIR9Pq6ImquJJHUMz75ebkocGFchOh0sCLSR5jW7XnI-evnPz_c-sz7pJXP73TXLDF8IESK5MYwmlAYtokhvg4_yHKDWCLRCjGq_baZpEok2iLdEiWdA8vWimU06Sd07h2Fc27okF4wqXM3xVdtcrk14rPRMktyM88f5yqxUtdKe92TAsPu4NPcOZb-8rkm8wmWlwm_-Nw3IDszHwhGCKVqfwwh_vrKMeqYix-MRdcOg-bpQ3F2QX1YtZkqSvXF1KcB8iXulqZwNUgCaEoy1oj2MvRtd0vt5KZBKJmztuAz2GpNrdG0Wb0YCsRIXc6Ybt5duL5Z4kqWcgihfoyVLGVWrmUCCb2RvUJTTsmCNqbfxhLwlQ-1EzPRNSzYgKfnKJSGyOdYbpdpgnoRJFkF8frrH8zPXJerEv45SPX9cDFUWsS2zic4au9a6P-mvKb6o_5a_sZEdMoATHZo_Dpl0kjvOvbKnsS_IKyd33XLqWzVkvruYUV3P4J_POZS0erKx9_BE6IgdQNKfkI82wiFUxl5qKdFhFvdebGwOA8LjoDpNHiFg0Hq6LAPjzAEPS_0Lg_OoxKRsWmGWUtn7XZfDNMoclFxFu_yl-8qAQT6NStoO22XiybNhdWwGjItX1yzrXkuSchj8xQKeLYZhSejPhFa66IGIXO2yiXxghnboj4DCzl_zerSRy6LcthLOIvzylgAsklT9E7so3dxXoXBmYVSBtTaO93fryRUdM2FaBJBYxK-mBd0F7n73R2Ku0drg3InXDf0-8Oqqq0YTwAfTxJeX_377n8w_-__NpA5764GAAA=.eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVdFU1QtVVMtRS1QUklNQVJZLXJlZGlyZWN0LmFuYWx5c2lzLndpbmRvd3MubmV0IiwiZXhwIjoxNjgzNTMzNjg0LCJhbGxvd0FjY2Vzc092ZXJQdWJsaWNJbnRlcm5ldCI6dHJ1ZX0=";
             
                return new WorkspaceViewModel
            {
                ReportsJson = JsonConvert.SerializeObject(reportList),
                EmbedToken = embedToken
            };
        }
    }
}