using System.Collections.Generic;
using DgPersonal.Extensions.General.Classes;
using FluentAssertions;
using Xunit;

namespace DgPersonal.Extensions.XUnitTests;

public class UnitTests
{
    [Theory]
    [InlineData("Id", "0", 0)]
    [InlineData("UnexpectedKey", "RandomValue", 0)]
    public void GetValueOrDefault_ChangingType_Ok(string key, string value, int expectedValue)
    {
        var pairs = new List<KeyValuePair<string, string>> { new(key, value) };

        var id = pairs.GetValueOrDefault<int>("Id");
        id.Should().Be(expectedValue);
    }

    [Fact]
    public void GetValueOrDefault_StringReturn_Ok()
    {
        const string key = "Name";
        const string value = "This value";
            
        var pairs = new List<KeyValuePair<string, string>> { new(key, value) };

        var name = pairs.GetValueOrDefault<string>(key);
        name.Should().Be(value);
    }
}