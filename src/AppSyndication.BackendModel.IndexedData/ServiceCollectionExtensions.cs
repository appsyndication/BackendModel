using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AppSyndication.BackendModel.IndexedData
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTagIndex(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton<ITagIndex, TagIndex>();

            return services;
        }
    }
}
