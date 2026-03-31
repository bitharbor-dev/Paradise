using Paradise.DataAccess.Seed.Models.ApplicationLogic;
using Paradise.DataAccess.Seed.Models.Domain;
using Paradise.Tests.Miscellaneous.XunitSerialization;
using Xunit.Sdk;

[assembly: Trait("Type", "Unit")]

[assembly: RegisterXunitSerializer(typeof(XunitJsonSerializer), typeof(ApplicationDataSeedModel))]
[assembly: RegisterXunitSerializer(typeof(XunitJsonSerializer), typeof(DomainDataSeedModel))]