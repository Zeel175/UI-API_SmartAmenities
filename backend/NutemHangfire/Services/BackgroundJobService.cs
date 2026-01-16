using Application.Helper;
using Domain.ViewModels;

namespace NutemHangfire.Services
{
    public class BackgroundJobService
    {
        private readonly IRestClient _restClient;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        // private readonly ICARPARSummaryRestService _CARPARSummaryRestService;
        public BackgroundJobService(
            IRestClient restClient,
            IHttpClientFactory clientFactory,
            IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _restClient = restClient;
        }
        public async Task<RestReponse<ResponseModel>> GetProductionOrders()
        {
            TraceService("===============================================================================");
            TraceService("Start GetProductionOrders  service : " + DateTime.Now);
            string baseApiPath = _configuration.GetValue<string>("BaseApiPath");
            RestRequest reqeust = new RestRequest($"{baseApiPath}ProductionOrder/GetProductionOrdersByHangfire", RestMethodType.Get);
            return await _restClient.ExecuteGetAsync<ResponseModel>(reqeust);

        }

        //////////////////////////////////
        private void TraceService(string content)
        {
            string FilePath = _configuration.GetSection("LogFilePath").GetSection("FilePath").Value;
            try
            {
                FileStream fs = new FileStream(@"" + FilePath + "", FileMode.OpenOrCreate, FileAccess.Write);

                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(content);
                sw.Flush();
                sw.Close();
            }
            catch { }
        }
    }
}
