using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Denner.MDEDruck;
using System.Security.Cryptography.X509Certificates;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddOptions<ServiceOptions>().Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("ServiceOptions").Bind(settings);
        }).ValidateDataAnnotations();
        services.AddSingleton<PrintAPIAdapter, PrintAPIAdapter>();

        X509Certificate2Collection rootCerts = new();
        // Need to add certificates from Root and My stores since Azure Function supports only adding certs to My while locally the are in Root 
        foreach (StoreName name in (new[] { StoreName.My, StoreName.Root }))
        {
            var certStore = new X509Store(name, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            rootCerts.AddRange(certStore.Certificates);
            certStore.Close();
        }

        services.AddHttpClient("HttpClientWithSSLCustom").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, _) =>
            {
                if (chain == null) throw new ArgumentNullException(nameof(chain));
                if (cert == null) throw new ArgumentNullException(nameof(cert));
                chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                chain.ChainPolicy.CustomTrustStore.AddRange(rootCerts);
                return chain.Build(cert);
            }
        });

    })
    .Build();

host.Run();