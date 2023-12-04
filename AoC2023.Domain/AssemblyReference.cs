using System.Reflection;

namespace AoC23.Domain;

public class AssemblyReference
{
    public readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}

