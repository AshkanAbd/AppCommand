using System.Reflection;
using AppCommand.Abstracts;
using AppCommand.Attributes;

namespace AppCommand;

/// <summary>
/// The static class that manages all commands in the application.
/// </summary>
public static class CommandManager
{
    /// <summary>
    /// Service provider that helps to create instance from commands.
    /// </summary>
    private static IServiceProvider _serviceProvider = null!;

    /// <summary>
    /// Stores all commands by there names.
    /// </summary>
    internal static IDictionary<string, Type> CommandRipository = new Dictionary<string, Type>();

    /// <summary>
    /// Sets <see cref="_serviceProvider"/> in <see cref="CommandManager"/>.    
    /// You MUST pass a not disposed service provider to use command manager
    /// </summary>
    /// <param name="serviceProvider">A not disposed service provider</param>
    public static void SetServiceProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    /// <summary>
    /// Searches <see cref="AppDomain"/> for all commands and add them to <see cref="CommandRipository"/>
    /// </summary>
    public static void SearchForCommands()
    {
        foreach (var commandType in AppDomain.CurrentDomain
                     .GetAssemblies()
                     .SelectMany(x => x.GetTypes())
                     .Where(x => x.IsClass && x.IsSubclassOf(typeof(AbstractCommand)) &&
                                 x.GetCustomAttribute<CommandAttribute>() != null
                     )) {
            AddCommand(commandType.GetCustomAttribute<CommandAttribute>()!.Name, commandType);
        }
    }

    /// <summary>
    /// Adds a new command to <see cref="CommandRipository"/>
    /// </summary>
    /// <param name="name">Name of the command</param>
    /// <param name="command">Type of the command</param>
    /// <typeparam name="T">Type of the command</typeparam>
    public static void AddCommand<T>(string name, T command) where T : AbstractCommand
    {
        AddCommand(name, command.GetType());
    }

    /// <summary>
    /// Adds a new command to <see cref="CommandRipository"/>
    /// </summary>
    /// <param name="name">Name of the command</param>
    /// <param name="commandType">Type of the command</param>
    public static void AddCommand(string name, Type commandType)
    {
        if (!commandType.IsSubclassOf(typeof(AbstractCommand)) ||
            commandType.GetCustomAttribute<CommandAttribute>() == null) {
            throw new Exception($"{commandType} is not a valid command.");
        }

        CommandRipository[name.ToLower()] = commandType;
    }

    /// <summary>
    /// Removes a command with given name from <see cref="CommandRipository"/>.
    /// </summary>
    /// <param name="name">Name of the command that should remove</param>
    public static void RemoveCommand(string name) => CommandRipository.Remove(name.ToLower());

    /// <summary>
    /// Determines that a command is available in <see cref="CommandRipository"/> 
    /// </summary>
    /// <param name="name">Name of the command that should check</param>
    /// <returns>Returns true oif command exists in command repository</returns>
    public static bool HasCommand(string name) => CommandRipository.ContainsKey(name.ToLower());

    /// <summary>
    /// Invokes a command with given name in <see cref="args"/> if its exists in <see cref="CommandRipository"/>
    /// </summary>
    /// <param name="args">Arguments that application runs with them. First argument is command name</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public static async Task InvokeCommand(string[] args, CancellationToken cancellationToken = default)
    {
        if (args.Length >= 1 && HasCommand(args[0])) {
            if (await InvokeCommandForResult(args[0].ToLower(), args, cancellationToken)) {
                Environment.Exit(1);
            }
        }
    }

    /// <summary>
    /// Invokes a command with given name, if its exists in <see cref="CommandRipository"/>
    /// </summary>
    /// <param name="name">Name of the command</param>
    /// <param name="args">Arguments that application runs with them. First argument is command name</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Returns that application should run after the command or not.<seealso cref="CommandAttribute"/></returns>
    /// <exception cref="Exception">Throw an exception if can't create an instance from the command</exception>
    private static async Task<bool> InvokeCommandForResult(string name, string[] args,
        CancellationToken cancellationToken = default)
    {
        var commandType = CommandRipository[name];
        var objectInstance = InstanceCreator.GetInstance(commandType, _serviceProvider);
        if (objectInstance is not AbstractCommand commandInstance) {
            throw new Exception("Can't find suitable constructor for command");
        }

        await commandInstance.Run(args, cancellationToken);

        return commandType.GetCustomAttribute<CommandAttribute>()!.ExitAtEnd;
    }
}