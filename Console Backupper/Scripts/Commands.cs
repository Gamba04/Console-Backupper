using System;
using System.Collections.Generic;

namespace ConsoleBackupper
{

    #region Base

    public abstract class Command
    {
        protected virtual byte ExpectedArgs => 0;

        public virtual bool ValidateArgs(string[] args)
        {
            bool valid = true;

            valid &= Validate(args.Length == ExpectedArgs, $"Expected {ExpectedArgs} arguments");

            return valid;

            static bool Validate(bool condition, string error = null)
            {
                if (condition) return true;

                if (error != null) Logger.LogError(error);

                return false;
            }
        }

        public virtual void Init(string[] args) { }

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

        public override void Init(string[] args)
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

        public override void Init(string[] args)
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

    public class BackupCommand : Command
    {
        private string name;

        protected override byte ExpectedArgs => 1;

        public override void Init(string[] args)
        {
            name = args[0];
        }

        public override void Run()
        {
            Location location = Configuration.GetLocation(name);

            location.Backup();
        }
    }

    public class BackupAllCommand : Command
    {
        public override void Run()
        {
            List<Location> locations = Configuration.GetLocations();

            locations.ForEach(location => location.Backup());
        }
    }

    public class ClearCommand : Command
    {
        public override void Run()
        {
            Console.Clear(); 
        }
    }

    public class HelpCommand : Command
    {
        public override void Run()
        {
            List<string> commands = new List<string>(Input.commandsLibrary.Keys);

            Logger.Log(commands);
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
