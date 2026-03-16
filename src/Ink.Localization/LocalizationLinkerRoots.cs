using System.Diagnostics.CodeAnalysis;
using System.Resources;

namespace Ink.Localization;

/// <summary>
/// Prevents the IL trimmer / WASM linker from removing ResX infrastructure.
/// ResourceManager uses reflection internally to locate and read satellite assemblies.
/// </summary>
internal static class LocalizationLinkerRoots
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ResourceManager))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ResourceReader))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ResourceSet))]
    internal static void Preserve()
    {
        // Never called - exists solely to carry [DynamicDependency] annotations
        // so the trimmer roots these types regardless of call-graph analysis.
    }
}
