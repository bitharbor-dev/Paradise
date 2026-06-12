using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;
using Paradise.Tests.Serialization;
using System.Globalization;
using Xunit.Sdk;

[assembly: Trait("Type", "Unit")]

[assembly: RegisterXunitSerializer(typeof(XunitJsonSerializer), typeof(ValueWrapper))]
[assembly: RegisterXunitSerializer(typeof(XunitJsonSerializer), typeof(BaseEmailModel))]
[assembly: RegisterXunitSerializer(typeof(CultureInfoSerializer), typeof(CultureInfo))]