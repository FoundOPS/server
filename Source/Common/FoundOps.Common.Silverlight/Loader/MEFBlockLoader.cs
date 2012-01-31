using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Collections.Generic;
using FoundOps.Core.Navigator.Loader;
using System.ComponentModel.Composition;
using FoundOps.Common.Silverlight.Blocks;
using FoundOps.Common.Tools.ExtensionMethods;
using System.ComponentModel.Composition.Hosting;

namespace FoundOps.Common.Silverlight.Loader
{
    public class MEFBlockLoader : ContentLoaderBase, INotifyPropertyChanged
    {
        private static Dictionary<Uri, Uri> _uriMap = new Dictionary<Uri, Uri>();
        private static List<Uri> _loadedXaps = new List<Uri>();
        private bool _isBusy;
        private int _progressPercentage;

        //Let's the content loader know NavigateTo "Clients" = NavigateTo "Clients", XapLocation = "www.abc.com/XAPFILE.xap"
        public static void MapUri(Uri pageNavigateUri, Uri xapLocation)
        {
            _uriMap[pageNavigateUri] = xapLocation;
        }

        protected override LoaderBase CreateLoader()
        {
            return new Loader(this);
        }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public IAggregateCatalogService AggregateCatalogService { get; set; }

        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<Page> Pages { get; set; }

        private bool _initialized;
        private void Initialize()
        {
            //Import aggregate catalog and pages
            CompositionInitializer.SatisfyImports(this);
            _initialized = true;
        }

        public int ProgressPercentage
        {
            get
            {
                return _progressPercentage;
            }
            private set
            {
                _progressPercentage = value;
                RaisePropertyChanged("ProgressPercentage");
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            private set
            {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        private class Loader : LoaderBase
        {
            private MEFBlockLoader parent;
            public Loader(MEFBlockLoader parent)
            {
                this.parent = parent;
            }

            public override void Load(Uri targetUri, Uri currentUri)
            {
                try
                {
                    //Remove any parameters, Ex. /HomePage?Id=342442 to /HomePage
                    if (targetUri.ToString().IndexOf("?") > 0)
                        targetUri = new Uri(targetUri.ToString().Substring(0, targetUri.ToString().IndexOf("?")),
                                            UriKind.RelativeOrAbsolute);

                    if (!parent._initialized)
                        parent.Initialize();
                    var savedTargetUri = targetUri;

                    Uri navigateUri = null;

                    if (_uriMap.Count <= 0) //If there are no uri mappings return
                    {
                        return;
                    }

                    //Hardcode LogOff
                    if (targetUri.ToString() == @"Account/LogOff")
                    {
                        HtmlPage.Window.Navigate(new Uri(String.Format(@"{0}/{1}", HtmlPage.Document.DocumentUri.RootUrl(), targetUri), UriKind.RelativeOrAbsolute));
                        return;
                    }

                    //Harcode User Feedback and Support
                    if (targetUri.ToString() == @"FeedbackSupport")
                    {
                        return;
                    }

                    if (_uriMap.TryGetValue(savedTargetUri, out navigateUri) && navigateUri != null && !_loadedXaps.Contains(navigateUri) && navigateUri.ToString().Count(c => c == '/') > 1)
                    {
                        var navigateUriForCurrentUri = new Uri(String.Format("{0}{1}", HtmlPage.Document.DocumentUri.RootUrl(), navigateUri), UriKind.RelativeOrAbsolute);

                        //Download the XAP
                        var dc = new DeploymentCatalog(navigateUriForCurrentUri);
                        dc.DownloadProgressChanged += (dcs, dscpe) => Dispatcher.BeginInvoke(() => parent.ProgressPercentage = dscpe.ProgressPercentage);
                        dc.DownloadCompleted += (dcSender, ayncCompletedEventArgs) =>
                        {
                            if (ayncCompletedEventArgs.Error != null)
                                throw ayncCompletedEventArgs.Error;
                            if (ayncCompletedEventArgs.Error != null)
                            {
                                this.Error(ayncCompletedEventArgs.Error);
                                return;
                            }
                            this.parent.IsBusy = false;
                            parent.AggregateCatalogService.AddCatalog(dc);

                            _loadedXaps.Add(navigateUri);
                            NavigateToPage(targetUri);
                        };
                        this.parent.ProgressPercentage = 0;
                        this.parent.IsBusy = true;
                        dc.DownloadAsync();
                    }
                    else
                        NavigateToPage(targetUri);
                }
                catch (Exception e)
                {
                    Error(e);
                }
            }

            private void NavigateToPage(Uri targetUri)
            {
                //Load pages
                CompositionInitializer.SatisfyImports(this);
                //Find matching page
                var pageToNavigateTo = this.parent.Pages.FirstOrDefault(p =>
                                                              {
                                                                  System.Attribute[] attrs = System.Attribute.GetCustomAttributes(p.GetType(), true);
                                                                  //Comparing to the page's ExportPage NavigateUri metadata property
                                                                  ExportPageAttribute exportPageAtrribute =
                                                                     (ExportPageAttribute)attrs.FirstOrDefault(a => a is ExportPageAttribute);
                                                                  return
                                                                      CompareUri(exportPageAtrribute.NavigateUri, targetUri);
                                                              });
                Complete(pageToNavigateTo);
                this.parent.IsBusy = false;
            }

            public static bool CompareUri(string currentUri, Uri targetUri)
            {
                Uri uri = GetTrimmedUri(currentUri);
                return targetUri.Equals(uri);
            }

            public static Uri GetTrimmedUri(string uriString)
            {
                //Get rid of additional parameters
                if (uriString.Contains('?'))
                    uriString = uriString.Substring(0, uriString.IndexOf('?') + 1);
                Uri result = new Uri(uriString, UriKind.RelativeOrAbsolute);
                return result;
            }

            public override void Cancel()
            {
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                RaisePropertyChanged(name);
            }
        }
        #endregion
    }
}