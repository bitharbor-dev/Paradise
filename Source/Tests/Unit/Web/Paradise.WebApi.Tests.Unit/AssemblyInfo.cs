using Paradise.Models.WebApi.Services.Authentication;
using Paradise.Tests.Miscellaneous.XunitSerialization;
using Xunit.Sdk;

[assembly: Trait("Type", "Unit")]

[assembly: RegisterXunitSerializer(typeof(XunitJsonSerializer), typeof(LoginModel))]