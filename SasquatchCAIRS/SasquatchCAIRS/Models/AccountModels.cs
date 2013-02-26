using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace SasquatchCAIRS.Models {
    public class UsersContext : DbContext {
        public UsersContext()
            : base("sasquatchConnectionString") {
        }

        public DbSet<UserProfile> UserProfiles {
            get;
            set;
        }
    }

    [Table("UserProfile")]
    public class UserProfile {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId {
            get;
            set;
        }
        public string UserName {
            get;
            set;
        }
        [Display(Name = "Name")]
        public string UserFullName {
            get;
            set;
        }
        [Display(Name = "Email")]
        public string UserEmail {
            get;
            set;
        }
        [Display(Name = "Status")]
        public bool UserStatus {
            get;
            set;
        }
    }
}
