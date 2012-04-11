using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class InvoicesDesignData
    {
        public Invoice DesignInvoice { get; private set; }
        public Invoice DesignInvoiceTwo { get; private set; }
        public Invoice DesignInvoiceThree { get; private set; }

        public IEnumerable<Invoice> DesignInvoices { get; private set; }

        public InvoicesDesignData()
        {
            InitializeInvoices();

            var salesTermsDesignData = new SalesTermsDesignData();
            var i = 0;
            foreach (var invoice in DesignInvoices)
            {
                //account for more invoices than Sales terms
                if (i >= salesTermsDesignData.DesignSalesTerms.Count()) 
                    i = 0;
                //Add a sales term
                invoice.SalesTerm = salesTermsDesignData.DesignSalesTerms.ElementAt(i);
                i++;
            }
        }

        private void InitializeInvoices()
        {
            DesignInvoice = new Invoice
            {
                DueDate = DateTime.UtcNow,
                FixedScheduleOptionInt = (Int32)FixedScheduleOption.FirstOfMonth,
                Memo = "Cleaning Grease Trap",
                RelativeScheduleDays = 5,
                ScheduleModeInt = (Int32)ScheduleMode.FixedDate
            };

            DesignInvoiceTwo = new Invoice
            {
                DueDate = DateTime.UtcNow.AddDays(5),
                FixedScheduleOptionInt = (Int32)FixedScheduleOption.MiddleOfMonth,
                Memo = "Food Delivery",
                RelativeScheduleDays = 10,
                ScheduleModeInt = (Int32)ScheduleMode.Relative
            };

            DesignInvoiceThree = new Invoice
            {
                DueDate = DateTime.UtcNow.AddDays(40),
                FixedScheduleOptionInt = (Int32)FixedScheduleOption.LastOfMonth,
                Memo = "Deliver Freight",
                RelativeScheduleDays = 34,
                ScheduleModeInt = (Int32)ScheduleMode.FixedDate
            };

            DesignInvoices = new List<Invoice> { DesignInvoice, DesignInvoiceTwo, DesignInvoiceThree };
        }
    }
}
