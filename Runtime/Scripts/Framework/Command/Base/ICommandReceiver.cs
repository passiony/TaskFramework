using System.Collections.Generic;
using System;

namespace TF.Runtime
{

    public class ICommandReceiver
    {
        private Dictionary<ECommand, Delegate> mCommands = new Dictionary<ECommand, Delegate>();

        public void AddCommand<T>(ECommand cmd, CommandHandler<T> handler) where T : ICommand
        {
            if (!this.mCommands.ContainsKey(cmd))
            {
                this.mCommands.Add(cmd, handler);
            }
        }

        public ECommandReply Command<T>(T cmd, bool undo) where T : ICommand
        {
            Delegate del = null;
            mCommands.TryGetValue(cmd.Command, out del);
            if (del == null)
            {
                return ECommandReply.N;
            }

            return (ECommandReply)del.DynamicInvoke(cmd, undo);
        }
    }
}