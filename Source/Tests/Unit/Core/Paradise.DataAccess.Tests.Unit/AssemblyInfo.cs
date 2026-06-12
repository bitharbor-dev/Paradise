using Paradise.Tests.Serialization;
using System.Globalization;
using Xunit.Sdk;

[assembly: Trait("Type", "Unit")]

[assembly: RegisterXunitSerializer(typeof(CultureInfoSerializer), typeof(CultureInfo))]