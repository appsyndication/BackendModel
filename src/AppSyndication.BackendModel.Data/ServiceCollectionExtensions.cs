using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AppSyndication.BackendModel.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTagStorage(this IServiceCollection services, string storageConnectionString)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAdd(ServiceDescriptor.Singleton<ITagStorageConnection>(s => new Connection(storageConnectionString)));

            services.TryAddSingleton<IDownloadTable, DownloadTable>();
            services.TryAddSingleton<IRedirectTable, RedirectTable>();
            services.TryAddSingleton<ITagTable, TagTable>();
            services.TryAddSingleton<ITagBlobContainer, TagBlobContainer>();
            services.TryAddSingleton<ITagTransactionBlobContainer, TagTransactionBlobContainer>();
            services.TryAddSingleton<ITagTransactionTable, TransactionTable>();
            services.TryAddSingleton<ITagQueue, TagQueue>();

            return services;
        }
    }
}
