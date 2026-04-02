using Paradise.WebApi.Startup;
using Paradise.WebApi.Startup.Steps.PostBuild;
using Paradise.WebApi.Startup.Steps.PreBuild;

var preBuildSteps = new IPreBuildStep[]
{
    new ConfigurationBootstrap(),
    new ServiceRegistrationBootstrap()
};

var postBuildSteps = new IPostBuildStep[]
{
    new PipelineBootstrap(),
    new RoutingBootstrap(),
    new DataBootstrap()
};

var bootstrapper = new ApplicationBootstrapper(preBuildSteps, postBuildSteps);
var app = await bootstrapper.BootstrapAsync(args);

await app.RunAsync()
    .ConfigureAwait(false);