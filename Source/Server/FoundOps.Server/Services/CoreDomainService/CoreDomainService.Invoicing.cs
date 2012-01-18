using System;
using System.Linq;
using System.Data;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using System.ServiceModel.DomainServices.EntityFramework;
using FoundOps.Core.Models.QuickBooks;
using FoundOps.Server.Authentication;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any invoice entities:
    /// Invoice, SalesTerms and LineItems
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
            QuickBooksTools.AddUpdateDeleteToTable(currentInvoice, Operation.Update);

            this.ObjectContext.Invoices.AttachAsModified(currentInvoice);
        }

        public void DeleteInvoice(Invoice invoice)
        {
            QuickBooksTools.AddUpdateDeleteToTable(invoice, Operation.Delete);

            if ((invoice.EntityState == EntityState.Detached))
                this.ObjectContext.Invoices.Attach(invoice);

            this.ObjectContext.Invoices.DeleteObject(invoice);
        }

        #endregion

        #region SalesTerm

        public IQueryable<SalesTerm> GetSalesTerms(Guid roleId)
        {
            var businessAccountForRole = ObjectContext.BusinessAccountForRole(roleId);

            var baseUrl = QuickBooksTools.GetBaseUrl(businessAccountForRole);

            var salesTermsXml = QuickBooksTools.GetEntityList(businessAccountForRole, "sales-terms", baseUrl);

            var quickBooksSalesTerms = QuickBooksTools.CreateSalesTermsFromQuickBooksResponse(salesTermsXml);

            foreach (var salesTerm in quickBooksSalesTerms)
            {
                var exists = businessAccountForRole.SalesTerms.FirstOrDefault(st => st.QuickBooksId == salesTerm.QuickBooksId);
                
                if (exists != null)
                {
                    businessAccountForRole.SalesTerms.Remove(exists);
                }
                //Always add the new sales term because it either never existed or the old one was removed from the business account above.
                businessAccountForRole.SalesTerms.Add(salesTerm);
            }

            this.ObjectContext.SaveChanges();

            var salesTerms =  this.ObjectContext.SalesTerms.Where(st => st.BusinessAccountId == businessAccountForRole.Id);
            return salesTerms;
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

        #region LineItem

        public IQueryable<LineItem> GetLineItem()
        {
            return this.ObjectContext.LineItems;
        }

        public void InsertLineItem(LineItem lineItem)
        {
            if ((lineItem.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(lineItem, EntityState.Added);
            }
            else
            {
                this.ObjectContext.LineItems.AddObject(lineItem);
            }
        }

        public void UpdateLineItem(LineItem lineItem)
        {
            this.ObjectContext.LineItems.AttachAsModified(lineItem);
        }

        public void DeleteLineItem(LineItem lineItem)
        {
            if ((lineItem.EntityState == EntityState.Detached))
                this.ObjectContext.LineItems.Attach(lineItem);

            this.ObjectContext.LineItems.DeleteObject(lineItem);
        }

        #endregion
    }
}
