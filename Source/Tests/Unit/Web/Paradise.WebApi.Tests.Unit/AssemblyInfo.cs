using Paradise.Models.WebApi.Services.Authentication;
using Paradise.Tests.Serialization;
using Xunit.Sdk;

[assembly: Trait("Type", "Unit")]

[assembly: RegisterXunitSerializer(typeof(XunitJsonSerializer), typeof(LoginModel))]