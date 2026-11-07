namespace HrManagement.Core;

public static class AbsenceRules
{
    public static int CalculateCalendarDays(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
        {
            throw new ArgumentException("Дата окончания не может быть раньше даты начала.");
        }

        return endDate.DayNumber - startDate.DayNumber + 1;
    }

    public static bool Overlaps(
        DateOnly firstStart,
        DateOnly firstEnd,
        DateOnly secondStart,
        DateOnly secondEnd)
    {
        return firstStart <= secondEnd && secondStart <= firstEnd;
    }
}
