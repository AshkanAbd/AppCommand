using System.Reflection;

namespace AppCommand;

/// <summary>
/// Internal static class that used for creating instance from commands.
/// </summary>
internal static class InstanceCreator
{
    /// <summary>
    /// Creates and instance from given command type.
    /// </summary>
    /// <param name="commandType">The command that should create an instance from it</param>
    /// <param name="serviceProvider">A service provider that will helps to create an instance</param>
    /// <returns>Returns an instance from <see cref="commandType"/></returns>
    /// <exception cref="Exception">Throw an exception if can't create an instance from the command</exception>
    internal static object? GetInstance(Type commandType, IServiceProvider serviceProvider)
    {
        var constructorParameters = GetConstructorInfo(commandType);
        if (constructorParameters == null) {
            throw new Exception("Can't find suitable constructor for command");
        }

        var constructorArgs = CreateConstructorParameters(constructorParameters, serviceProvider);
        return Activator.CreateInstance(commandType, constructorArgs);
    }

    /// <summary>
    /// Gets the command constructor parameters.
    /// </summary>
    /// <param name="commandType">Command type that constructor method is needed.</param>
    /// <returns>Returns constructor method of <see cref="commandType"/></returns>
    private static ConstructorInfo? GetConstructorInfo(Type commandType)
    {
        return commandType.GetConstructors().Length < 1 ? null : commandType.GetConstructors()[0];
    }

    /// <summary>
    /// Creates an instance from given constructor.
    /// </summary>
    /// <param name="constructor">Constructor of the command</param>
    /// <param name="serviceProvider">A service provider that will helps to create an instance</param>
    /// <returns>Returns an instance from the class of given <see cref="constructor"/></returns>
    private static object?[] CreateConstructorParameters(MethodBase constructor, IServiceProvider serviceProvider)
    {
        return constructor.GetParameters()
            .Select(parameter => serviceProvider.GetService(parameter.ParameterType))
            .ToArray();
    }
}