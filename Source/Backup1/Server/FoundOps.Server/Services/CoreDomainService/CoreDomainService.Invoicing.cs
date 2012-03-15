using System.Linq;
using System.Data;
using FoundOps.Core.Models.CoreEntities;
using System.ServiceModel.DomainServices.EntityFramework;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any invoice entities:
    /// Invoice and SalesTerms
    /// </summary>
    public partial class CoreDomainService
    {
        #region Invoice

        public IQueryable<Invoice> GetInvoices()
        {
            return this.ObjectContext.Invoices;
        }

        public void InsertInvoice(Invoice invoice)
        {
            if ((invoice.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(invoice, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Invoices.AddObject(invoice);
            }
        }

        public void UpdateInvoice(Invoice currentInvoice)
        {
            this.ObjectContext.Invoices.AttachAsModified(currentInvoice);
        }

        public void DeleteInvoice(Invoice invoice)
        {
            if ((invoice.EntityState == EntityState.Detached))
                this.ObjectContext.Invoices.Attach(invoice);

            this.ObjectContext.Invoices.DeleteObject(invoice);
        }

        #endregion

        #region SalesTerm

        public IQueryable<SalesTerm> GetSalesTerms()
        {
            return this.ObjectContext.SalesTerms;
        }

        public void InsertSalesTerm(SalesTerm salesTerm)
        {
            if ((salesTerm.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(salesTerm, EntityState.Added);
            }
            else
            {
                this.ObjectContext.SalesTerms.AddObject(salesTerm);
            }
        }

        public void UpdateSalesTerm(SalesTerm currentSalesTerm)
        {
            this.ObjectContext.SalesTerms.AttachAsModified(currentSalesTerm);
        }

        public void DeleteSalesTerm(SalesTerm salesTerm)
        {
            if ((salesTerm.EntityState == EntityState.Detached))
                this.ObjectContext.SalesTerms.Attach(salesTerm);

            this.ObjectContext.SalesTerms.DeleteObject(salesTerm);
        }

        #endregion
    }
}
