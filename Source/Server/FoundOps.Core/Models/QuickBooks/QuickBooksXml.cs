using System;
using System.Text;
using System.Xml;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Models.QuickBooks
{
    public enum Operation
    {
        Create,
        Update,
        Delete
    }

    public static class QuickBooksXml
    {
        /// <summary>
        /// Creates the quickbooks XML writer and string builder.
        /// </summary>
        /// <param name="type">The type of QuickBooks element.</param>
        /// <param name="writeElementSpecificXml">A method to write the specific element's xml.</param>
        /// <returns></returns>
        public static string QuickBooksElementXml(string type, Action<XmlWriter, StringBuilder> writeElementSpecificXml)
        {
            var settings = new XmlWriterSettings();
            var builder = new StringBuilder();

            const string nameSpaceOne = "http://www.intuit.com/sb/cdm/v2";
            const string nameSpaceTwo = "http://www.intuit.com/sb/cdm/qbo";

            using (var writer = XmlWriter.Create(builder, settings))
            {
                writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");

                writer.WriteStartElement(type, nameSpaceOne);
                writer.WriteAttributeString("xmlns", "ns2", null, nameSpaceTwo);

                //Write the element specific xml
                writeElementSpecificXml(writer, builder);

                writer.WriteEndElement(); //Close element
            }

            return builder.ToString();
        }

        /// <summary>
        /// Invoices the XML.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="operation">The enum representation of the operation.</param>
        /// <returns></returns>
        public static string InvoiceXml(Invoice invoice, string clientId, Operation operation)
        {
            return QuickBooksElementXml("Invoice", (writer, builder) =>
            {
                #region Additions for Update & Delete

                if (operation == Operation.Update || operation == Operation.Delete)
                {
                    //Customer Id and SyncToken are required for Updates and Deletes

                    writer.CreateElement("Id", invoice.CustomerId, "idDomain", "QBO");
                    writer.CreateElement("SyncToken", invoice.SyncToken);

                    //No more is necessary, return the Delete XML
                    if (operation == Operation.Delete)
                        return;

                    writer.CreateElement("CreateTime", invoice.CreateTime);
                    writer.CreateElement("LastUpdatedTime", invoice.LastUpdatedTime);
                }

                #endregion

                #region Header

                writer.WriteElement("Header", () =>
                {
                    writer.CreateElement("Msg", invoice.Memo);

                    writer.CreateElement("CustomerId", clientId, "idDomain", "QBO");

                    writer.WriteElement("BillAddr", () =>
                    {
                        writer.CreateElement("Line1", invoice.BillToLocation.AddressLineOne);

                        writer.CreateElement("Line2", invoice.BillToLocation.AddressLineTwo);

                        writer.CreateElement("City", invoice.BillToLocation.AdminDistrictTwo);

                        writer.CreateElement("CountrySubDivisionCode", invoice.BillToLocation.AdminDistrictOne);

                        writer.CreateElement("ZipCode", invoice.BillToLocation.PostalCode);
                    });

                    //writer.CreateElement("SalesTermId", invoiceToCreate.BillToLocation.AdminDistrictTwo);
                    //writer.CreateElement("DueDate", invoiceToCreate.DueDate.ToString());
                });

                #endregion

                //Add the line items
                #region Line Items

                foreach (var item in invoice.LineItems)
                {
                    writer.WriteElement("Line", () =>
                    {
                        writer.CreateElement("Desc", item.Description);
                        writer.CreateElement("Amount", item.Amount);
                    });
                }

                #endregion
            });
        }

        /// <summary>
        /// Writes an element and it's body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="createBody">The action to create the element's body.</param>
        public static void WriteElement(this XmlWriter writer, string elementName, Action createBody)
        {
            writer.WriteStartElement(elementName);
            createBody();
            writer.WriteEndElement();
        }

        /// <summary>
        /// Creates the element.
        /// </summary>
        /// <param name="xmlWriter">The XML writer.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="value">The value.</param>
        /// <param name="attributeName">Name of the attribute local.</param>
        /// <param name="attributeValue">The attribute value.</param>
        public static void CreateElement(this XmlWriter xmlWriter, string elementName, string value, string attributeName = null, string attributeValue = null)
        {
            xmlWriter.WriteStartElement(elementName);
            if (attributeName != null && attributeValue != null)
                xmlWriter.WriteAttributeString(attributeName, attributeValue);
            xmlWriter.WriteString(value);
            xmlWriter.WriteEndElement();
        }
    }
}