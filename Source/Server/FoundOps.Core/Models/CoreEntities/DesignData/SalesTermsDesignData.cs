using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class SalesTermsDesignData
    {
        public SalesTerm DesignSalesTerm { get; private set; }
        public SalesTerm DesignSalesTermTwo { get; private set; }
        public SalesTerm DesignSalesTermThree { get; private set; }

        public IEnumerable<SalesTerm> DesignSalesTerms { get; private set; }

        public SalesTermsDesignData()
        {
            InitializeSalesTerms();
        }

        private void InitializeSalesTerms()
        {
            DesignSalesTerm = new SalesTerm
            {
                Name = "Net 15",
                DueDays = 15
            };

            DesignSalesTermTwo = new SalesTerm
            {
                Name = "Net 30",
                DueDays = 30
            };

            DesignSalesTermThree = new SalesTerm
            {
                Name = "Net 60",
                DueDays = 60
            };

            DesignSalesTerms = new List<SalesTerm> { DesignSalesTerm, DesignSalesTermTwo, DesignSalesTermThree };
        }
    }
}
