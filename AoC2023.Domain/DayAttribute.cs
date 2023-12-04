namespace AoC23.Domain;

[AttributeUsage(AttributeTargets.Class)]
public class DayAttribute : Attribute
{
    public int Day { get; }
    public DayAttribute(int day) => Day = day;
}

