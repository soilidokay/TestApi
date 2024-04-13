using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;
namespace TestApi.Common
{
    public static class ConfigurationExtenstion
    {
        public static IApplicationBuilder UseMicroserviceV2(this IApplicationBuilder app, string microUrl)
        {
            app.Use(async (context, next) =>
            {
                var uri = new UriBuilder(microUrl);
                string targetServerUrl = $"{uri.Scheme}://{uri.Host}:{uri.Port}"; // Thay đổi thành máy chủ thật của bạn

                if (context.Request.Path.StartsWithSegments(uri.Path))
                {
                    // Tạo một đối tượng HttpClient để thực hiện yêu cầu tới máy chủ khác.
                    var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
                    {
                        UseProxy = false,
                        AllowAutoRedirect = false,
                        AutomaticDecompression = DecompressionMethods.None,
                        UseCookies = false,
                        ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
                        ConnectTimeout = TimeSpan.FromSeconds(15),
                        SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                        {
                            RemoteCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                        }
                    });
                    var requestConfig = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };
                    var forwarder = context.RequestServices.GetService<IHttpForwarder>();
                    var error = await forwarder.SendAsync(context, targetServerUrl, httpClient, requestConfig);
                }
                else
                {
                    await next();
                }
            });
            return app;
        }
    }
}
