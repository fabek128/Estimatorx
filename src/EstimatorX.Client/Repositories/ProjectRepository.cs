
using EstimatorX.Client.Services;
using EstimatorX.Shared.Models;

namespace EstimatorX.Client.Repositories;

[RegisterScoped]
public class ProjectRepository : RepositorySearchBase<Project, ProjectSummary>
{
    public ProjectRepository(GatewayClient gateway) : base(gateway)
    {
    }

    protected override string GetBasePath()
    {
        return "/api/project";
    }
}
