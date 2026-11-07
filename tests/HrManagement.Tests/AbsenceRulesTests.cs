using HrManagement.Core;

namespace HrManagement.Tests;

public sealed class AbsenceRulesTests
{
    [Fact]
    public void OneDayAbsenceContainsOneCalendarDay()
    {
        var days = AbsenceRules.CalculateCalendarDays(
            new DateOnly(2026, 11, 2),
            new DateOnly(2026, 11, 2));

        Assert.Equal(1, days);
    }

    [Fact]
    public void TwoWeekVacationContainsFourteenDays()
    {
        var days = AbsenceRules.CalculateCalendarDays(
            new DateOnly(2026, 11, 9),
            new DateOnly(2026, 11, 22));

        Assert.Equal(14, days);
    }

    [Fact]
    public void ReversedPeriodIsRejected()
    {
        Assert.Throws<ArgumentException>(() => AbsenceRules.CalculateCalendarDays(
            new DateOnly(2026, 11, 3),
            new DateOnly(2026, 11, 2)));
    }

    [Fact]
    public void IntersectingPeriodsOverlap()
    {
        var result = AbsenceRules.Overlaps(
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 10),
            new DateOnly(2026, 11, 10),
            new DateOnly(2026, 11, 15));

        Assert.True(result);
    }

    [Fact]
    public void SeparatePeriodsDoNotOverlap()
    {
        var result = AbsenceRules.Overlaps(
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 5),
            new DateOnly(2026, 11, 6),
            new DateOnly(2026, 11, 8));

        Assert.False(result);
    }
}
