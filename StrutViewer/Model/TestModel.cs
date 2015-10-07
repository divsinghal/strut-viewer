using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DivyaSinghal.StrutViewer.Annotations;

namespace DivyaSinghal.StrutViewer
{
    public class TestModel : BaseTestModel, INotifyPropertyChanged
    {
        private IList<UnitTestModel> _tests;
        public string Description { get; set; }

        public IList<UnitTestModel> Tests
        {
            get { return _tests; }
            set
            {
                OnPropertyChanged();
                _tests = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
