using System;
using System.Collections.Generic;

namespace ConsoleBackupper
{

    #region Base

    public abstract class Command
    {
        protected virtual byte ExpectedArgs => 0;

        #region Initialization

        /// <summary> Validates a command and returns it if it's positive. </summary>
        public static Command GetCommand<C>(string[] args)
            where C : Command, new()
        {
            C command = new C();

            if (command.ValidateArgs(args))
            {
                command.Init(args);

                return command;
            }

            return null;
        }

        protected virtual bool ValidateArgs(string[] args) => CheckExpectedArgs(args);

        protected bool CheckExpectedArgs(string[] args)
        {
            if (args.Length != ExpectedArgs)
            {
                Logger.LogError($"Expected {ExpectedArgs} arguments");

                return false;
            }

            return true;
        }

        protected virtual void Init(string[] args) { }

        #endregion

        public abstract void Run();
    }

    #endregion

    // ----------------------------------------------------------------

    #region Commands

    public class QueryCommand : Command
    {
        public override void Run()
        {
            List<Location> locations = Configuration.GetLocations();
            List<string> log = new List<string>(locations.Count);

            locations.ForEach(location => log.Add(location));

            Logger.Log(log);
        }
    }

    public class AddCommand : Command
    {
        private Location location;

        protected override byte ExpectedArgs => 3;

        protected override void Init(string[] args)
        {
            location = new Location(args[0], args[1], args[2]);
        }

        public override void Run()
        {
            Configuration.Add(location);
        }
    }

    public class RemoveCommand : Command
    {
        private string name;

        protected override byte ExpectedArgs => 1;

        protected override void Init(string[] args)
        {
            name = args[0];
        }

        public override void Run()
        {
            Configuration.Remove(name);
        }
    }

    public class RemoveAllCommand : Command
    {
        public override void Run()
        {
            Configuration.RemoveAll();
        }
    }

    public class StartCommand : Command
    {
        public override void Run()
        {
            List<Location> locations = Configuration.GetLocations();

            locations.ForEach(backup => backup.Execute());
        }
    }

    public class ClearCommand : Command
    {
        public override void Run()
        {
            Console.Clear(); 
        }
    }

    public class ExitCommand : Command
    {
        public override void Run()
        {
            Environment.Exit(0);
        }
    }

    #endregion

}
