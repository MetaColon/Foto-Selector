using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using LAS_Interface.Stuff;

namespace Foto_Selector
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _source, _destination;
        private List<string> _fileNames;
        private int _pos;
        private MainWindow MainWindow;

        private string FileName
        {
            get { return Pos >= 0 && Pos < _fileNames.Count ? _fileNames[Pos] : ""; }
            set
            {
                if (Pos >= 0 && Pos < _fileNames.Count)
                    _fileNames[Pos] = value;
            }
        }

        public MainViewModel (MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            _fileNames = new List<string> ();
            Pos = 0;

            ButtonRightCommand = new DelegateCommand (ButtonRight);
            ButtonLeftCommand = new DelegateCommand (ButtonLeft);
            SelectButtonCommand = new DelegateCommand(SelectButton);
            Source = @"C:\Users\Timo\Dropbox\Kroatienfreizeit 2016 Bilder";
            Destination = @"C:\Users\Timo\Pictures\Kroatien Vorselektiert";
        }

        public void KeyUp(object sender, KeyEventArgs args)
        {
            switch (args.Key)
            {
                case Key.F1:
                    CopyCurrentFileToDestination();
                    break;
                case Key.F2:
                    ButtonLeft(null);
                    break;
                case Key.F3:
                    ButtonRight(null);
                    break;
            }
        }

        public void CopyCurrentFileToDestination() => File.Copy(Source + "\\" + FileName, Destination + "\\" + FileName);

        public void ButtonRight (object param)
        {
            if (Pos < _fileNames.Count - 1)
                Pos++;
        }

        public void ButtonLeft (object param)
        {
            if (Pos > 0)
                Pos--;
        }

        public void SelectButton(object param)
        {
            CopyCurrentFileToDestination();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
                if (_source == null || !Directory.Exists (Source))
                    return;
                _fileNames = new DirectoryInfo (value).GetFiles ().Select (file => file.Name).ToList ();
                OnPropertyChanged (nameof (Source));
                OnPropertyChanged (nameof (CurrentPicture));
                Pos = 0;
            }
        }

        public string Destination
        {
            get { return _destination; }
            set
            {
                _destination = value;
                OnPropertyChanged (nameof (Destination));
            }
        }

        public string CurrentPicture
        {
            get
            {
                return Source + "\\" + FileName;
            }
            set
            {
                _source = value;
                OnPropertyChanged (nameof (CurrentPicture));
            }
        }

        private int Pos
        {
            get { return _pos; }
            set
            {
                _pos = value;
                OnPropertyChanged (nameof (Pos));
                OnPropertyChanged (nameof(CurrentPicture));
                MainWindow.Title = String.IsNullOrEmpty(FileName) ? "Foto Selector" : "Foto Selector - " + FileName;
            }
        }

        public ICommand ButtonRightCommand { get; set; }
        public ICommand ButtonLeftCommand { get; set; }
        public ICommand SelectButtonCommand { get; set; }

        protected void OnPropertyChanged (string name)
            => PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (name));
    }
}