using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace processpubsubapi
{
    public static class processpubsub
    {
        [FunctionName("processpubsub")]
        public static async Task Run([ServiceBusTrigger("khsvrlessohsbtopic", "khsvrlessohsbtopicsub", Connection = "PUBSUBConnection")]string mySbMsg,
            [Blob("receipts-high-value/{rand-guid}.json", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream receipt,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
            log.LogInformation($"Total Cost is greater than or equal to $100.00 starting...");
            dynamic data = JObject.Parse(mySbMsg);

            try
            {
                // Get Receipt in PDF Format
                log.LogInformation($"PDF Receipt starting...");
                HttpClient httpClient = HttpClientFactory.Create();
                string responseMessage = String.Empty;
                HttpResponseMessage pdfResponse = await httpClient.GetAsync((string)data.receiptUrl);

                byte[] pdfData = null;
                pdfData = await pdfResponse.Content.ReadAsByteArrayAsync();
                string base64PDF = Convert.ToBase64String(pdfData);
                log.LogInformation($"PDF Receipt completed.");

                Greater100 greater100 = new Greater100();
                greater100.Store = data.storeLocation;
                greater100.SalesNumber = data.salesNumber;
                greater100.TotalCost = (double)data.totalCost;
                greater100.Items = (long)data.totalItems;
                greater100.SalesDate = data.salesDate;
                greater100.ReceiptImage = base64PDF;

                string g100Content = JsonConvert.SerializeObject(greater100);
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(g100Content)))
                {
                    await stream.CopyToAsync(receipt);
                }
            }
            catch (Exception exc)
            {
                log.LogError($"Total Cost is greater than $100.00 failed with exception type {exc.GetType().ToString()} and error message: {exc.Message}");
            }
            log.LogInformation($"Total Cost is greater than $100.00 completed.");
        }
    }

    public static class processpubsub2
    {
        [FunctionName("processpubsub2")]
        public static async Task Run([ServiceBusTrigger("khsvrlessohsbtopic", "khsvrlessohsbtopicsub2", Connection = "PUBSUBConnection")]string mySbMsg,
            [Blob("receipts/{rand-guid}.json", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream receipt,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
            log.LogInformation($"Total Cost is less than $100.00 starting...");
            dynamic data = JObject.Parse(mySbMsg);

            try
            {
                Less100 less100 = new Less100();
                less100.Store = data.storeLocation;
                less100.SalesNumber = data.salesNumber;
                less100.TotalCost = (double)data.totalCost;
                less100.Items = (long)data.totalItems;
                less100.SalesDate = data.salesDate;
                string l100Content = JsonConvert.SerializeObject(less100);
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(l100Content)))
                {
                    await stream.CopyToAsync(receipt);
                }
            }
            catch (Exception exc)
            {
                log.LogError($"Total Cost is less than $100.00 failed with exception type {exc.GetType().ToString()} and error message: {exc.Message}");
            }
            log.LogInformation($"Total Cost is less than $100.00 completed.");
        }
    }
}
