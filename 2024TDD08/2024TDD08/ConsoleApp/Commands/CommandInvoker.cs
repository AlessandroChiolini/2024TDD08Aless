﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConsoleApp.Commands
{
    public class CommandInvoker
    {
        private readonly List<ICommand> _commands = new List<ICommand>();

        public void AddCommand(ICommand command)
        {
            _commands.Add(command);
        }

        public async Task ExecuteCommandsAsync()
        {
            foreach (var command in _commands)
            {
                await command.ExecuteAsync();
            }
        }

        public void ClearCommands()
        {
            _commands.Clear();
        }
    }
}
