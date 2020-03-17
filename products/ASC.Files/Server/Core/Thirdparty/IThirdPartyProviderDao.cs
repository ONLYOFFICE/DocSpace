using System;

using ASC.Files.Core;
using ASC.Files.Core.Thirdparty;

namespace ASC.Files.Thirdparty
{
    internal interface IThirdPartyProviderDao<T> : IDisposable where T : class, IProviderInfo
    {
        void Init(BaseProviderInfo<T> t1, RegexDaoSelectorBase<T> selectorBase);
    }
}
