using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServer.DB
{
    public enum ProviderType
    {
        Facebook = 1,
        Google = 2,
        Email = 3,
    }

    [Table("Account")]
    public class AccountDb
    {
        public int AccountDbId { get; set; }

        [Required] [MaxLength(20)] public string AccountName { get; set; }

        [Required] public string Password { get; set; }
        [Required] public ProviderType LoginProviderType { get; set; }
    }
}