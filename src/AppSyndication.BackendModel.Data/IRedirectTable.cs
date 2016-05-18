using System.Collections.Generic;

namespace AppSyndication.BackendModel.Data
{
    public interface IRedirectTable : ITableBase
    {
        IEnumerable<RedirectEntity> GetAllRedirects();

        RedirectEntity GetRedirect(string redirectKey);
    }
}