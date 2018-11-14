using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayApproval.Entities
{
    // Represents a holiday request entity
    public class HolidayRequestEntity
    {
        // User who origins the holiday request
        public UserEntity Originator;

        // Start date of requested holiday
        public DateTime HolidayFrom;

        // End date of requested holiday
        public DateTime HolidayTo;

        // State of holidayrequest [New,Approved,Refused,Booked]
        public EHolidayRequestState State;

        public HolidayRequestEntity()
        {
        }

        public HolidayRequestEntity(DateTime holidayFrom, DateTime holidayTo, UserEntity originator)
        {
            this.HolidayFrom = holidayFrom;
            this.HolidayTo = holidayTo;
            this.Originator = originator;
        }

        public static HolidayRequestEntity ForTesting = new HolidayApproval.Entities.HolidayRequestEntity(DateTime.Today, DateTime.Today, new HolidayApproval.Entities.UserEntity()
        {
            EmailAddress = "test@test.at",
            UserName = "ma"
        });
    }
}
