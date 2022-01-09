using EstimatorX.Client.Repositories;
using EstimatorX.Shared.Definitions;
using EstimatorX.Shared.Models;

namespace EstimatorX.Client.Stores;

public class TemplateStore : StoreEditBase<Template, TemplateRepository>, IScopedService
{
    public TemplateStore(ILoggerFactory loggerFactory, TemplateRepository repository) : base(loggerFactory, repository)
    {
    }
}
