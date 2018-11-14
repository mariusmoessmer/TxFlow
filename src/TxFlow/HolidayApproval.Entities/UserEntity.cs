using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayApproval.Entities
{
    // Represents a user
    public class UserEntity
    {
        public UserEntity(string userName, string emailAddress)
        {
            this.UserName = userName;
            this.EmailAddress = emailAddress;
        }

        public UserEntity()
        {
        }

        // username which identifies the user
        public string UserName;

        // email-address of the user
        public string EmailAddress;
    }
}
