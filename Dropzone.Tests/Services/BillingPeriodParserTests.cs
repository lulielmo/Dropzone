using Dropzone.Services;
using FluentAssertions;

namespace Dropzone.Tests.Services;

public class BillingPeriodParserTests
{
    [Theory]
    [InlineData("Tillverkarens artikelnummer: AZURECONS\r\nPeriod 2026-06-01 -- 2026-06-30", "202606")]
    [InlineData("Period 2026-06-01 -- 2026-06-30", "202606")]
    [InlineData("period 2025-12-01 -- 2025-12-31", "202512")]
    [InlineData("Period 2026-06", "202606")]
    [InlineData("202606", "202606")]
    public void TryParse_ShouldReturnYearMonth(string text, string expected)
    {
        BillingPeriodParser.TryParse(text, out var period).Should().BeTrue();
        period.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("AZURECONS")]
    [InlineData("Period 2026-13-01 -- 2026-13-31")]
    public void TryParse_ShouldFailWhenPeriodMissingOrInvalid(string? text)
    {
        BillingPeriodParser.TryParse(text, out var period).Should().BeFalse();
        period.Should().BeEmpty();
    }
}
