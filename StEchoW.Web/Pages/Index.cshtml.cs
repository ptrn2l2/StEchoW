using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace StEchoW.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string ContainerId { get; set; }
        public HostString RequestHost { get; set; }
        public string XForwardedHost { get; set; }

        public string OsDescription { get; set; }

        public IPAddress[] ServerAddressList { get; set; }
        public IList<string> ClientAddresses { get; set; }

        public string RequestProtocol { get; set; }
        public string RequestMethod { get; set; }
        public string RequestScheme { get; set; }
        public PathString RequestPath { get; set; }

        public IHeaderDictionary RequestHeaders {get;set;}

        public string RequestBodyStr {get;set;}

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            var request = Request;

            OsDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            RequestHeaders = request.Headers;
            // Hostname...
            ContainerId = Dns.GetHostName(); // get container id
            // ...but, if behind a Proxy, the client requested:
            XForwardedHost = Request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? string.Empty;
            
            // ServerAddressList will not be echoed
            ServerAddressList = (await Dns.GetHostEntryAsync(ContainerId)).AddressList;
            
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                // ReSharper disable once ConstantNullCoalescingCondition
                RequestBodyStr = await reader.ReadToEndAsync() ?? string.Empty;
            }

            RequestHost = request.Host;
            // var requestHostLocationUri = request.Host.ToUriComponent();
            RequestMethod = request.Method;
            RequestProtocol = request.Protocol;
            var forwardedPath = request.Headers["X-Forwarded-Path"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedPath))
            {
                RequestPath = $"{forwardedPath}{request.Path}";
            }
            else
            {
                RequestPath = request.Path;
            }

            
            StringValues xForwardedForList = Request.Headers["X-Forwarded-For"];
            ClientAddresses = new List<string>();
            if (xForwardedForList.Count>0)
            {
                foreach (string xff in xForwardedForList)
                {
                    ClientAddresses.Add(xff);
                }
            }
            else
            {
                IPAddress remoteIpAddress = request.HttpContext.Connection.RemoteIpAddress;
                ClientAddresses.Add(remoteIpAddress?.ToString() ?? string.Empty);
            }


            RequestScheme = request.Scheme;
        }
        
        public string BuildAllinfos(bool bEchoAll = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Server id: {ContainerId}");
            sb.AppendLine();
            if (XForwardedHost != string.Empty)
            {
                sb.AppendLine($"Host: {XForwardedHost} ({RequestHost.ToString()})");
            }
            else
            {
                sb.AppendLine($"Host: {RequestHost.ToString()}");
            }

            if (bEchoAll)
            {
                int iMax = ServerAddressList.Length;
                if (iMax > 0)
                {
                    sb.AppendLine("Host IP(s):");
                    sb.AppendLine($"    {ServerAddressList[0]}");
                    for (int i = 1; i < iMax; ++i)
                    {
                        sb.AppendLine($"    {ServerAddressList[i]}");
                    }
                }
            }

            if (ClientAddresses.Count > 1)
            {
                sb.AppendLine("Client:");
                foreach (var cAddr in ClientAddresses)
                {
                    sb.AppendLine($"    {cAddr}");
                }
            }
            else if (ClientAddresses.Count == 1)
            {
                sb.AppendLine($"Client: {ClientAddresses[0]}");
            }

            sb.AppendLine();
            sb.AppendLine($"{RequestScheme}");
            sb.AppendLine($"{RequestProtocol} {RequestMethod} {RequestPath}");

            foreach (var hdr in RequestHeaders)
            {
                if (hdr.Key.StartsWith("Cookie", StringComparison.OrdinalIgnoreCase))
                {
                    if (!bEchoAll)
                    {
                        sb.AppendLine("Cookie=(...)");
                        // request.Cookies should not be echoed for security reasons
                        continue;
                    }
                }

                if (hdr.Value.Count == 0)
                {
                    sb.AppendLine($"{hdr.Key}=");
                }
                else if (hdr.Value.Count == 1)
                {
                    sb.AppendLine($"{hdr.Key}={hdr.Value[0]}");
                }
                else
                {
                    sb.AppendLine(string.Join('&', $"{hdr.Key}={hdr.Value}"));
                }
            }

            sb.AppendLine();
            sb.AppendLine($"{RequestBodyStr}");
            sb.AppendLine();
            return sb.ToString();
        }
    } // class
}
