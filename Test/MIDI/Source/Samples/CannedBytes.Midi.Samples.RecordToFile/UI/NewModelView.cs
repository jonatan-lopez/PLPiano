using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;
using System.Windows;

namespace CannedBytes.Midi.Samples.RecordToFile.UI
{
    public  class NewModelView : ViewModelBase, ICommand
    {
       

        public static NewModelView Singleton { get => singleton; set => singleton = value; }

        private static NewModelView singleton;



        public NewModelView()
        {
            model = new Model();
            model.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
        }

        private readonly Model model;

        public string LastKey
        {
            get => model.LastKey;
            set { model.LastKey = value; }
        }

        public void Execute(object? parameter) => model.Start();
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => true;

    }

  

    public class Model : INotifyPropertyChanged
    {
        private string lastKey;

        public string LastKey { get { return lastKey; } set { lastKey = value; OnPropertyChanged(nameof(LastKey)); } }

        internal void Start() => Task.Run(() =>
        {
            //Thread.Sleep(1000);
            //IsLevel = !IsLevel;
            //OnPropertyChanged(nameof(IsLevel));
        });


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
