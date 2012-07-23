using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Threading;
using CrumbLib;

namespace SnoopBreadcrumbs
{
    class MainVM : ViewModels.ViewModelBase, IMessageHandler
    {

        public MainVM()
        {
            this.RootFolder = @"C:\temp\Main2";
            DisplayText = "Select the root directory to apply breadcrumbs.\r\n***WARNING.  Xaml files will be overwritten, so using a copy of your project/solution is strongly recommended.";
        }

        string _displayText;

        public string DisplayText
        {
            get { return _displayText; }
            set
            {
                CheckPropertyChanged("DisplayText", ref _displayText, ref value);
            }
        }



        string _lastDisplayText;

        public string LastDisplayText
        {
            get { return _lastDisplayText; }
            set
            {
                CheckPropertyChanged("LastDisplayText", ref _lastDisplayText, ref value);
            }
        }



        int _totalFilesToProcess;

        public int TotalFilesToProcess
        {
            get { return _totalFilesToProcess; }
            set
            {
                CheckPropertyChanged("TotalFilesToProcess", ref _totalFilesToProcess, ref value);
            }
        }




        int _filesProcessed;

        public int FilesProcessed
        {
            get { return _filesProcessed; }
            set
            {
                CheckPropertyChanged("FilesProcessed", ref _filesProcessed, ref value);
            }
        }



        string _rootFolder;

        public string RootFolder
        {
            get { return _rootFolder; }
            set
            {
                CheckPropertyChanged("RootFolder", ref _rootFolder, ref value);
            }
        }



        bool _asyncProcessing;

        public bool AsyncProcessing
        {
            get { return _asyncProcessing; }
            set
            {
                CheckPropertyChanged("AsyncProcessing", ref _asyncProcessing, ref value);
            }
        }


        public void ProcessXamls()
        {

            this.AsyncProcessing = true;
            this.FilesProcessed = 0;

            int exceptionCount = 0;

            DisplayText = string.Empty;

            var task = Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        var processor = new XamlFilesProcessor();

                        var xamls = processor.FindFiles(this.RootFolder,
                            ShowErrMsgBox, this);

                        this.TotalFilesToProcess = xamls.Count;

                        exceptionCount += processor.ProcessXamls(xamls, 
                            ShowErrMsgBox, i => this.FilesProcessed = i, this);

                    }
                    catch (Exception ex)
                    {
                        exceptionCount++;
                        AddMessage("Exception: " + ex.ToString());
                    }
                }
                );



            task.ContinueWith(obj =>
            {

                AsyncProcessing = false;

                //TODO:  note skipped files count
                this.AddMessage(string.Format("Finished. Files Processed: {0}.  Excptions {1}.", this.FilesProcessed, exceptionCount));
            }
                );

        }

        void ShowErrMsgBox(string title, string msg)
        {
            MessageBox.Show(msg, title);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isStatus">updates 1 line status</param>
        /// <param name="isDetail">updates running log</param>
        public void AddMessage(string message, bool isStatus = true, bool isDetail = true)
        {
            if (isDetail)
            {
                DisplayText = string.Format("{0}:  {1}\r\n",
                    DateTime.Now.ToString("hh:mm:ss.fff"), message) + DisplayText;
            }

            if (isStatus)
                LastDisplayText = message;
        }
    }

}
