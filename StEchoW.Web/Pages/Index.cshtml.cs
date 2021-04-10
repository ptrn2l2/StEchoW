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

        public string ContainerId {get;set;}
        public HostString RequestHost { get; set; }
        public string XForwardedHost { get; set; }


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

            RequestHeaders = request.Headers;
            // Hostname...
            ContainerId = Dns.GetHostName(); // get container id
            // ...but, if behind a Proxy, the client requested:
            XForwardedHost = Request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? string.Empty;
            
            ServerAddressList = (await Dns.GetHostEntryAsync(ContainerId)).AddressList;
            
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                // ReSharper disable once ConstantNullCoalescingCondition
                RequestBodyStr = await reader.ReadToEndAsync() ?? string.Empty;
            }

            RequestHost = request.Host;
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
                IPAddress? remoteIpAddress = request.HttpContext.Connection.RemoteIpAddress;
                ClientAddresses.Add(remoteIpAddress?.ToString() ?? string.Empty);
            }


            RequestScheme = request.Scheme;
            // request.Cookies will not be echoed for security reasons
        }

    }
}