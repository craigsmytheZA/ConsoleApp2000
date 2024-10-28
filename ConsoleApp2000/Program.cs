using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.CSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class Program
{
    //"https://localhost:44397/api/onestreamapi"
    private static void Main(string[] args)
    {
        DataTable Snowflakedt = new DataTable();
        List<SnowflakeData> SnowflakeList = new List<SnowflakeData>();

        string BToken = "9999#r3onr39u8yhuhtdyD#K20fg7TlmW9Oq31sA";
        Console.WriteLine("Start Operation.");
        Console.ReadKey();

        try
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var client = new HttpClient(handler);
            var request1 = new HttpRequestMessage(HttpMethod.Post, "https://www.cl3.co.za/Snowflake/api/onestreamapi");
            request1.Headers.Add("Authorization", "Bearer " + BToken);
            var response1 = client.Send(request1);
            response1.EnsureSuccessStatusCode();

            var task = Task.Run(() => response1.Content.ReadAsStringAsync());
            task.Wait();
            var responseBody = task.Result;

            //Parse the JSON response from the response body 
            var jsonObject = JObject.Parse(responseBody);
            var contents = jsonObject["content"]["headers"][0]["value"].ToString();
            contents = contents.Substring(6, contents.Length - 10);
            contents = contents.Replace("\\", "");
            var SjSon = JsonConvert.DeserializeObject(contents);
            var FinalJson = JsonConvert.DeserializeObject(SjSon.ToString()).ToString();

            //Create the DataTable from the final Json string
            Snowflakedt = (DataTable)JsonConvert.DeserializeObject(FinalJson, (typeof(DataTable)));

            for (int i = 0; i < Snowflakedt.Rows.Count; i++)
            {
                SnowflakeData data = new SnowflakeData();
                data.LOCATIONNAME = Snowflakedt.Rows[i]["LOCATIONNAME"].ToString();
                data.ACCOUNTNO = Snowflakedt.Rows[i]["ACCOUNTNO"].ToString();
                data.TITLE = Snowflakedt.Rows[i]["TITLE"].ToString();
                data.BALANCEYTD = Snowflakedt.Rows[i]["BALANCEYTD"].ToString();
                data.LEVEL11_NAME = Snowflakedt.Rows[i]["LEVEL11_NAME"].ToString();
                data.LEVEL11_DESCRIPTION = Snowflakedt.Rows[i]["LEVEL11_DESCRIPTION"].ToString();
                data.CATEGORY = Snowflakedt.Rows[i]["CATEGORY"].ToString();
                SnowflakeList.Add(data);
            }


            Console.WriteLine(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Application Exception: " + ex.Message);
            //throw ErrorHandler.LogWrite(si, new XFException(si, ex));
        }
        Console.WriteLine("Operation Complete.");
        Console.ReadKey();
    }  

    public class SnowflakeData
    {
        public string LOCATIONNAME { get; set; }
        public string ACCOUNTNO { get; set; }
        public string TITLE { get; set; }
        public string BALANCEYTD { get; set; }
        public string LEVEL11_NAME { get; set; }
        public string LEVEL11_DESCRIPTION { get; set; }
        public string CATEGORY { get; set; }
    }    
}