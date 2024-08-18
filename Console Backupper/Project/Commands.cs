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
            List<Backup> backups = Configuration.GetBackups();
            List<string> log = new List<string>(backups.Count);

            backups.ForEach(backup => log.Add(backup));

            Logger.Log(log);
        }
    }

    public class AddCommand : Command
    {
        private Backup backup;

        protected override byte ExpectedArgs => 2;

        protected override void Init(string[] args)
        {
            backup = new Backup(args[0], args[1]);
        }

        public override void Run()
        {
            Configuration.Add(backup);
        }
    }

    public class RemoveCommand : Command
    {
        private string source;

        protected override byte ExpectedArgs => 1;

        protected override void Init(string[] args)
        {
            source = args[0];
        }

        public override void Run()
        {
            Configuration.Remove(source);
        }
    }

    public class StartCommand : Command
    {
        public override void Run()
        {
            List<Backup> backups = Configuration.GetBackups();

            backups.ForEach(backup => backup.Execute());
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
