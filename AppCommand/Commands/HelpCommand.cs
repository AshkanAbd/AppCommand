using System.Reflection;
using System.Text;
using AppCommand.Abstracts;
using AppCommand.Attributes;

namespace AppCommand.Commands;

/// <summary>
/// Help command that will print all available commands in application
/// </summary>
[Command("Help", Description = "Shows available commands in application.")]
public class HelpCommand : AbstractCommand
{
    public override Task Run(string[] args, CancellationToken cancellationToken = default)
    {
        var longestCommandLenght = GetLongestCommandLength();

        foreach (var (commandName, commandType) in CommandManager.CommandRipository) {
            var builder = new StringBuilder("  ");

            builder.Append(commandName);
            for (var i = longestCommandLenght + 5; i > commandName.Length; i--) {
                builder.Append(' ');
            }

            builder.Append(GetCommandAttribute(commandType).Description);

            Console.WriteLine(builder.ToString());
        }

        return Task.CompletedTask;
    }

    private CommandAttribute GetCommandAttribute(Type commandType)
    {
        var attribute = commandType.GetCustomAttribute(typeof(CommandAttribute));
        return (attribute as CommandAttribute)!;
    }

    private long GetLongestCommandLength()
    {
        return CommandManager.CommandRipository
            .Keys.Max(x => x.Length);
    }
}