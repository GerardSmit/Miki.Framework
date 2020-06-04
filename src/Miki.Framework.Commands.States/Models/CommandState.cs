namespace Miki.Framework.Commands.States
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CommandState
	{
		[DataMember(Order = 1)]
		public string Name { get; set; }

        [DataMember(Order = 2)]
        public string ChannelId { get; set; }

        [DataMember(Order = 3)]
        public bool State { get; set; }

        [DataMember(Order = 4)]
        public string PlatformId { get; set; }
	}
}