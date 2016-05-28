using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppSyndication.BackendModel.Data
{
    public interface IRedirectTable : ITableBase
    {
        Task<IEnumerable<RedirectEntity>> GetAllRedirectsAsync();

        Task<RedirectEntity> GetRedirectAsync(string redirectKey);
    }
}