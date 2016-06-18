using System.ComponentModel;

namespace UnitFlags
{
    class FlagDisplay : INotifyPropertyChanged
    {
        string name;
        public string Name {
            get
            {
                return name;
            }
            set
            {
                if(name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        private bool enabled;
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if(enabled != value)
                {
                    enabled = value;
                    NotifyPropertyChanged("Enabled");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
