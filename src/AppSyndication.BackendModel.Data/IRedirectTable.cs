using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppSyndication.BackendModel.Data
{
    public interface IRedirectTable : ITableBase
    {
        IEnumerable<RedirectEntity> GetAllRedirects();

        RedirectEntity GetRedirect(string redirectKey);

        Task<RedirectEntity> GetRedirectAsync(string redirectKey);
    }
}