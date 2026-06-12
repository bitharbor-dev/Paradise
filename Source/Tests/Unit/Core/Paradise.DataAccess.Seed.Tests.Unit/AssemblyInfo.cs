using Paradise.DataAccess.Seed.Models.ApplicationLogic;
using Paradise.DataAccess.Seed.Models.Domain;
using Paradise.Tests.Serialization;
using Xunit.Sdk;

[assembly: Trait("Type", "Unit")]

[assembly: RegisterXunitSerializer(typeof(XunitJsonSerializer), typeof(DomainDataSeedModel))]
[assembly: RegisterXunitSerializer(typeof(XunitJsonSerializer), typeof(InfrastructureDataSeedModel))]