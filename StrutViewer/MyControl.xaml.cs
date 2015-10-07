using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DivyaSinghal.StrutViewer.Annotations;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace DivyaSinghal.StrutViewer
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl, INotifyPropertyChanged
    {
        private string _testResult;
        private string _strutConfigFile;
        string _solutionPath;
        public MyControl()
        {
            InitializeComponent();
            Tests = new ObservableCollection<TestModel>();
            Tests.CollectionChanged += delegate(object o, NotifyCollectionChangedEventArgs args) { };
            DataContext = this;
            YamlFIlePath = @"C:\BitBucket\webapi-accounts\README.yaml";
            Setup();
        }

        private void Setup()
        {
            if (CheckIfSolutionAlreadyOpen().Result)
            {
                _strutConfigFile = GetStrutYamlFile().Result;
                if (string.IsNullOrWhiteSpace(_strutConfigFile))
                {

                }
            };
        }

        public ObservableCollection<TestModel> Tests { get; set; }

        public string YamlFIlePath
        {
            get;
            set;
        }

        public string TestResult
        {
            get { return _testResult; }
            set
            {
                _testResult = value;
                OnPropertyChanged();
            }
        }


        private void LoadTests(object sender, RoutedEventArgs e)
        {

            DTE dte;
            dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
            var solutionPath = dte.Solution.FileName;

            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                this.lblFilePath.Content = "No project / solution open";
                this.lblFilePath.Background = new SolidColorBrush(Colors.Red);
                return;

            }
            // find the yaml file
            YamlFIlePath = Directory.GetFiles(Path.GetDirectoryName(solutionPath), "*.yaml").FirstOrDefault();

            if (string.IsNullOrEmpty(YamlFIlePath))
            {
                this.lblFilePath.Content = "No file found";
                this.lblFilePath.Background = new SolidColorBrush(Colors.Red);
                return;
            }
             
            this.lblFilePath.Content = Path.GetFileName(YamlFIlePath);
            this.lblFilePath.Background = new SolidColorBrush(Colors.Transparent);

            var yamlParser = new YamlFileParser(YamlFIlePath);

            foreach (var testModel in yamlParser.Parse())
            {
                Tests.Add(testModel);
            }
            yamlParser.Parse();
        }

        private void RunTests(object sender, RoutedEventArgs e)
        {
           PowerShell ps = PowerShell.Create();
           ps.AddScript(@"cd \ ;");;
           var solutionPath = Path.GetDirectoryName(YamlFIlePath);
           ps.AddScript(string.Format(@"cd {0} ;",solutionPath ));
           ps.AddScript("strut");

           foreach (PSObject pso in ps.Invoke())
           {
               TestResult += pso;
           } 

        }

        private async Task<string> GetTestsYamlFile()
        {
            return await Task.FromResult(Directory.GetFiles(_solutionPath, "*.yaml").FirstOrDefault());
        }

        private async Task<string> GetStrutYamlFile()
        {
            return await Task.FromResult(Directory.GetFiles(_solutionPath, ".strut.yml").FirstOrDefault());
        }

        private string GetSolutionFilePath()
        {
            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
            string solutionFilePath = dte.Solution.FileName;
            return string.IsNullOrWhiteSpace(solutionFilePath) ? string.Empty : Path.GetDirectoryName(solutionFilePath);
        }

        private async Task<bool> CheckIfSolutionAlreadyOpen()
        {
            _solutionPath = GetSolutionFilePath();
            if (string.IsNullOrWhiteSpace(_solutionPath))
            {
                this.lblFilePath.Content = "No project / solution open";
                this.lblFilePath.Background = new SolidColorBrush(Colors.Red);
                return false;
            }

            YamlFIlePath = await GetTestsYamlFile();

            if (string.IsNullOrEmpty(YamlFIlePath))
            {
                this.lblFilePath.Content = "No file found";
                this.lblFilePath.Background = new SolidColorBrush(Colors.Red);
                return false;
            }

            this.lblFilePath.Content = Path.GetFileName(YamlFIlePath);
            this.lblFilePath.Background = new SolidColorBrush(Colors.Transparent);
            return true;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}