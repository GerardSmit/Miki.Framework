﻿namespace Miki.Framework.Commands
{
    using Miki.Framework.Arguments;
    using Miki.Framework.Commands.Nodes;

	public class CommandTree
	{
		public NodeContainer Root { get; }

        public CommandTree()
		{
			Root = new NodeRoot();
		}

		public Node GetCommand(IArgumentPack pack)
		{
			return Root.FindCommand(pack);
		}

        public Node GetCommand(string name)
            => GetCommand(new ArgumentPack(name.Split(' ')));
    }
}
