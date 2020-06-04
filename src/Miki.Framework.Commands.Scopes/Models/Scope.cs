namespace Miki.Framework.Commands.Scopes.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;

    [Table("Scopes")]
    [DataContract]
	public class Scope
	{
		[Column("UserId")]
        [DataMember(Name = "user_id", Order = 1)]
		public string UserId { get; set; }

        [Column("ScopeId")]
        [DataMember(Name = "scope_id", Order = 2)]
        public string ScopeId { get; set; }
        
        [Column("PlatformId")]
        [DataMember(Name = "platform_id", Order = 3)]
        public string PlatformId { get; set; }
	}
}
