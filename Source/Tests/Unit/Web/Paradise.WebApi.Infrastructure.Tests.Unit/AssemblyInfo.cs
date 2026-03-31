using Paradise.Tests.Miscellaneous.XunitSerialization;
using System.Globalization;
using Xunit.Sdk;

[assembly: Trait("Type", "Unit")]

[assembly: RegisterXunitSerializer(typeof(CultureInfoSerializer), typeof(CultureInfo))]