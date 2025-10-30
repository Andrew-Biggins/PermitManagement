namespace PermitManagement.Core.UnitTests;

internal sealed class Gwt : FactAttribute
{
    public Gwt(string given, string when, string then) => DisplayName = $"{given}, {when}, {then}.";
}

internal sealed class GwtTheory : TheoryAttribute
{
    public GwtTheory(string given, string when, string then) => DisplayName = $"{given}, {when}, {then}.";
}