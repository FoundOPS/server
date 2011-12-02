using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.Blocks
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class ExportPageAttribute : ExportAttribute
    {
        public ExportPageAttribute() : base(typeof(Page)) { }
        public ExportPageAttribute(string navigateUri)
            : this()
        {
            this.NavigateUri = navigateUri;
        }
        public string NavigateUri { get; set; }
    }
}
