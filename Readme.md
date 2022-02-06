## AppCommand:
### Concept:

Sometimes we need to use our web application for some other purposes. Such as running our seeds, applying our pending migrations or dropping the database and etc.   
We don't want to build our application from scratch or even we don't have access to source code. We also need to access our services and create instance from them.   
At this situation AppCommand package can help us.   
You can easily create a new CLI command and pass you parameters to it and run your application for what you want.

#### Commands that created with [AppCommand](https://www.nuget.org/packages/AppCommand/) can access to DI tree, so you can inject your services to this commands.

### How to enable:

#### Dotnet 5 or below:

Put following lines to `Main` function in `Program` class:

```c#
CommandManager.SearchForCommands();
var host = CreateHostBuilder(args).Build();

using (var scope = host.Services.CreateScope()){
    CommandManager.SetServiceProvider(scope.ServiceProvider);
    await CommandManager.InvokeCommand(args);
}

host.Run();
```

##### Note that you change `Main` method to an `async` method.

#### Dotnet 6 or later:

Put following lines after `var app = builder.Build();` line `Program.cs` in file:

```c#
CommandManager.SearchForCommands();
CommandManager.SetServiceProvider(app.Services);
CommandManager.InvokeCommand(args);
```

### How add new command:

1) Create a `TestCommand` class.
2) Add `[Command('test')]` attribute to `TestCommand` class.
3) Inherit `TestCommand` from `AbstractCommand`.
4) Implement what you want in `Run` method.

Your class should looks like this:

```c#
[Command("test")]
public class TestCommand : AbstractCommand
{
    public ILogger<TestCommand> Logger { get; set; }

    public TestCommand(ILogger<TestCommand> logger)
    {
        /// All services you want to inject.
        Logger = logger;
    }

    public override Task Run(string[] args, CancellationToken cancellationToken = default)
    {
        // Implement what you want.
        Logger.LogInformation("Hi there!!!");
        var applicationArgs = args.Aggregate((x, y) => x += $" {y}");
        Logger.LogInformation("ApplicationArgs = {applicationArgs}", applicationArgs);
        Thread.Sleep(100);
        return Task.CompletedTask;
    }
}
```

### How to run specific command:

You have 2 ways to run your commands:

1) If you have access ro source code or you want to compile it:   
   run this command in terminal `dotnet run YOUR_COMMAND ARGS`
2) If you don't have access to source code or you don't want to compile it:   
   run this command in terminal `./YOUR_APP YOUR_COMMAND ARGS`

### Available commands:

#### These commands will be updated over time, you can add a command and create an RP to this repo.

| Command | Description                   | Args |
|---------|-------------------------------|------|
| `help`  | Prints all available commands |      |