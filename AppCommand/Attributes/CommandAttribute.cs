namespace AppCommand.Attributes;

/// <summary>
/// Identifies a command with its name.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class CommandAttribute : Attribute
{
    /// <summary>
    /// Creates a command
    /// </summary>
    /// <param name="name">Name of the command</param>
    /// <param name="exitAtEnd">Determines that application should run after the command or not</param>
    public CommandAttribute(string name, bool exitAtEnd = true)
    {
        Name = name;
        ExitAtEnd = exitAtEnd;
    }

    /// <summary>
    /// Name of the command
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Determines that application should run after the command or not
    /// </summary>
    public bool ExitAtEnd { get; }

    /// <summary>
    /// Description for the command. Only for documentation purposes in help command.  
    /// </summary>
    public string? Description { get; set; }
}