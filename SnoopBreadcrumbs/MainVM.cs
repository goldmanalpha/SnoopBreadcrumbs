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
    class MainVM : ViewModels.ViewModelBase
    {

        public MainVM()
        {
            this.RootFolder = @"C:\temp\Main2";
            DisplayText = "Select the root directory to apply breadcrumbs.\r\n***WARNING.  Xaml files will be overwritten, so using a copy of your project/solution is strongly recommended.";
        }




        XmlHelper _xmlHelper = new XmlHelper();
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

            var task = Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        exceptionCount += ProcessXamls2();
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

                this.AddMessage(string.Format("Finished. Files Processed: {0}.  Excptions {1}.", this.FilesProcessed, exceptionCount));
            }
                );

        }

        public int ProcessXamls2()
        {
            AddMessage("Finding Xamls");

            var root = this.RootFolder;
            if (!System.IO.Directory.Exists(root))
            {
                var msg = "Can't process.  Pick a root directory for your *copied* project.";
                AddMessage(msg);
                MessageBox.Show(msg, "Invalid Folder");
                return 0;
            }

            DisplayText = string.Empty;

            AddMessage("Looking for Xamls in " + root);

            List<string> xamls = new List<string>();

            ScanDir(root, xamls);

            this.TotalFilesToProcess = xamls.Count;

            AddMessage(string.Format("Found {0} xaml files.", xamls.Count));

            AddMessage("Inserting Xaml Tags");

            int count = 0;
            int exceptionCount = 0;
            foreach (var file in xamls)
            {
                try
                {
                    AddMessage("Processing " + file);

                    this.FilesProcessed = count++;

                    var xaml = System.IO.File.ReadAllText(file);

                    var fileName = Path.GetFileName(file);
                    
                    var newXaml = this._xmlHelper.TagXmlElements(xaml,
                        s => this.AddMessage(s, false), 
                        fileName + ": " + XmlHelper.LineNumberFormatTag + " " + file
                        );

                    AddMessage("Writing: " + file);

                    // remove read only:
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                    System.IO.File.WriteAllText(file, newXaml);
                }
                catch (Exception ex)
                {
                    exceptionCount++;
                    AddMessage("Exception: " + ex.ToString());
                }

            }

            return exceptionCount;
        }

        void ScanDir(string path, List<string> files)
        {

            var xamls = System.IO.Directory.GetFiles(path)
                .Where(f => f.EndsWith(".xaml",
                    StringComparison.InvariantCultureIgnoreCase));

            foreach (var file in xamls)
            {
                files.Add(file);
                AddMessage(file);
            }

            var dirs = System.IO.Directory.GetDirectories(path);

            foreach (var dir in dirs)
            {
                ScanDir(dir, files);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isStatus">updates 1 line status</param>
        /// <param name="isDetail">updates running log</param>
        private void AddMessage(string message, bool isStatus = true, bool isDetail = true)
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
