namespace E621TelegramBot
{
    public interface ICommand
    {
        // So what do we need for this?
        // Ideally it would have some descriptive information on the command for both a help function
        //     and to register the command list on startup.
        // It should also have some ability to set up access control for the different commands.
        // It should have a validation method that is executed before executing the function body.
    }
}