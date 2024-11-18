using shcome.loxone.sensor.Config;
using shcome.loxone.sensor.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Xml;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Net.Http;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;
using shcome.loxone.sensor.Importer;
using System.Xml.Serialization;

namespace shcome.loxone.sensor.Clients
{
    public class LoxoneStatsClient
    {
        private readonly SensorEndpoint _endpoint;
        private readonly ILogger _logger;

        public LoxoneStatsClient(SensorEndpoint endpoint, ILogger logger)
        {
            _endpoint = endpoint;
            _logger = logger;
        }

        public SensorEndpoint Endpoint => _endpoint;

        public async Task<List<StatsSensorData>> LoadSensorInformation(CancellationToken cancellationToken = default)
        {
            var ret = new List<StatsSensorData>();


            // add auth
            var url = _endpoint.EndpointUrl.WithBasicAuth(_endpoint.Username, _endpoint.Password);
            var content = await url.GetStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);

            var htmlSensorData = htmlDoc.DocumentNode.SelectNodes("//body/ul/li/a");
            var textPattern = @"(.+)\s?\((\w+)\s{1}(.+)\)\s?(\d{6})";
            var textRegEx = new Regex(textPattern);

            foreach (var sensorNode in htmlSensorData)
            {
                var LinkText = WebUtility.HtmlDecode(sensorNode.InnerText);
                var link = "";
                if (sensorNode.HasAttributes)
                {
                    link = sensorNode.GetAttributeValue("href", string.Empty);
                }

                //Writes to text file
                Debug.WriteLine(LinkText);
                Debug.WriteLine($"      {link}");

                var newData = new StatsSensorData();
                newData.Description = LinkText;
                newData.DownloadUri = $"{_endpoint.EndpointUrl}{link}";

                if (textRegEx.IsMatch(LinkText))
                {
                    var matches = textRegEx.Matches(LinkText);
                    if (matches.Count == 1 && matches[0].Groups.Count > 1)
                    {
                        var groupes = matches[0]?.Groups;

                        if (groupes != null)
                        {
                            newData.SensorName = groupes[1]?.Value?.Trim();
                            if (groupes.Count > 2)
                                newData.Category = groupes[2]?.Value;
                            if (groupes.Count > 3)
                            {
                                newData.Room = groupes[3]?.Value;
                                newData.Description = $"{newData.SensorName} - {newData.Room}";
                            }
                            if (groupes.Count > 4)
                                newData.ReadingTimeSpan = groupes[4]?.Value;//DateTime.ParseExact(groupes[4]?.Value ?? "", "yyyyMM", CultureInfo.InvariantCulture);
                        }
                    }
                }

                ret.Add(newData);
            }

            return ret;
        }

        public async Task<StatisticReadings> LoadSensorReadings(StatsSensorData sensorInfo, CancellationToken cancellationToken = default)
        {
            var ret = (StatisticReadings)null;

            if(sensorInfo != null && !string.IsNullOrEmpty(sensorInfo.DownloadUri))
            {
                var url = sensorInfo.DownloadUri.WithBasicAuth(_endpoint.Username, _endpoint.Password);

                try
                {
                    return await url.GetXmlAsync<StatisticReadings>(cancellationToken);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error downloading XML sensor data file. Try to download raw data.");
                    var rawText = await url.GetStringAsync(cancellationToken);

                    using (var stream = new MemoryStream())
                    {
                        var writer = new StreamWriter(stream);
                        writer.Write(rawText.Replace("<v.col>", "col"));
                        writer.Flush();
                        stream.Position = 0;


                        XmlSerializer serializer = new XmlSerializer(typeof(StatisticReadings));
                        return (StatisticReadings)serializer.Deserialize(stream);
                    }
                }
            }

            return null;
        }

        #region old code
        //private async Task HttpClientCode()
        //{
        //    var client = new HttpClient(new SocketsHttpHandler()
        //    {
        //        PlaintextStreamFilter = (context, token) =>
        //        {
        //            Console.WriteLine($"Request {context.InitialRequestMessage} --> negotiated version {context.NegotiatedHttpVersion}");
        //            //var memStream = new MemoryStream();
        //            //var buffer = new byte[4096];
        //            //var read = 0;
        //            //do
        //            //{
        //            //    //await context.PlaintextStream.CopyToAsync(memStream, 4096, cancellationToken);
        //            //    read = await context.PlaintextStream.ReadAsync(buffer, 0, 4096, cancellationToken);
        //            //    await memStream.WriteAsync(buffer, 0, read);
        //            //} while (read > 0);
        //            //memStream.Seek(0, SeekOrigin.Begin);
        //            //string decoded = Encoding.UTF8.GetString(memStream.ToArray());
        //            //memStream.Seek(0, SeekOrigin.Begin);
        //            ////return ValueTask.FromResult(context.PlaintextStream);
        //            ////return ValueTask.FromResult(memStream);
        //            //return memStream;

        //            return ValueTask.FromResult((Stream)new HttpStreamInterceptor(context.PlaintextStream as NetworkStream));
        //        }
        //    });


        //    var uri = new Uri(_endpoint.EndpointUrl);
        //    SetAuthorization(client);

        //    var content = await client.GetStringAsync(uri);
        //}
        //private void SetAuthorization(HttpClient client)
        //{
        //    var authenticationString = $"{_endpoint.Username}:{_endpoint.Password}";
        //    var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
        //    client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64EncodedAuthenticationString}");
        //}
        #endregion
    }
}
