using HolidayApproval.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayApprovalWFConsoleApp
{

    /// <summary>
    /// Toolbox that provides methods which are proxies for WF-activities
    /// </summary>
    public abstract class HolidayApprovalActivityToolbox
    {
        public abstract bool CreateHolidayApprovalTask(
            HolidayRequestEntity HolidayRequest, string ResponsibleUser);

        public abstract void BookHolidayRequest(
            HolidayRequestEntity HolidayRequest);

        public abstract void SendEmail(
            string Subject, string Body, string ToMailAddress);

        public abstract void UpdateHolidayRequestState(
            HolidayRequestEntity HolidayRequest, EHolidayRequestState NewState);

        internal void WriteLine(string v)
        {
            //DateTime now = DateTime.Now;

            //Console.Write(now);

DateTime now;
this.Assign(out now, DateTime.Now);
        }

        internal void Assign<T>(out T var, T value)
        {
            throw new Exception();
        }
    }


    class AbstractWorkflow<T>
    {
        public HolidayApprovalActivityToolbox Activities;
    }



}
