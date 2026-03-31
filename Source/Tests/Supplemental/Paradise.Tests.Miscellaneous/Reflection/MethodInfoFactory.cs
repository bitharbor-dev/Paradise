using System.Reflection;
using System.Reflection.Emit;

namespace Paradise.Tests.Miscellaneous.Reflection;

/// <summary>
/// <see cref="MethodInfo"/> factory class.
/// </summary>
public static class MethodInfoFactory
{
    #region Constants
    private const string MainModuleName = "Main";
    #endregion

    #region Fields
    private static readonly ModuleBuilder _mainModule
        = AssemblyBuilder
        .DefineDynamicAssembly(new(nameof(MethodInfoFactory)), AssemblyBuilderAccess.Run)
        .DefineDynamicModule(MainModuleName);
    #endregion

    #region Public methods
    /// <summary>
    /// Creates a runtime-generated instance method
    /// with a <see langword="void"/> return type and no parameters.
    /// </summary>
    /// <param name="typeName">
    /// The logical name of the dynamic type.
    /// <para>
    /// Caller must ensure uniqueness.
    /// </para>
    /// </param>
    /// <param name="name">
    /// The logical name of the method.
    /// </param>
    /// <param name="typeAttributes">
    /// The collection of <see cref="CustomAttributeBuilder"/> instances to apply to the generated type.
    /// </param>
    /// <param name="methodAttributes">
    /// The collection of <see cref="CustomAttributeBuilder"/> instances to apply to the generated method.
    /// </param>
    /// <returns>
    /// A <see cref="MethodInfo"/> representing the emitted method with applied attributes.
    /// </returns>
    /// <remarks>
    /// The generated method contains a minimal valid IL body consisting of a single <c>ret</c> instruction.
    /// This is required by the runtime in order to materialize the method.
    /// </remarks>
    public static MethodInfo CreateVoid(string typeName, string name,
                                        IEnumerable<CustomAttributeBuilder>? typeAttributes = null,
                                        IEnumerable<CustomAttributeBuilder>? methodAttributes = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var typeBuilder = _mainModule.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);

        if (typeAttributes is not null)
        {
            foreach (var attribute in typeAttributes)
                typeBuilder.SetCustomAttribute(attribute);
        }

        var methodBuilder = typeBuilder.DefineMethod(name, MethodAttributes.Public, typeof(void), Type.EmptyTypes);

        if (methodAttributes is not null)
        {
            foreach (var attribute in methodAttributes)
                methodBuilder.SetCustomAttribute(attribute);
        }

        var generator = methodBuilder.GetILGenerator();
        generator.Emit(OpCodes.Ret);

        var type = typeBuilder.CreateType();
        return type.GetMethod(name)!;
    }
    #endregion
}