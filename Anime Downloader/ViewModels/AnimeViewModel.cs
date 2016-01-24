using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Anime_Downloader.ViewModels {
    public class AnimeViewModel : INotifyPropertyChanged {
        private DateTime _addedInternal;
        private string _nameInternal;
        private string _statusInternal;
        private string _tagInternal;

        public string Name {
            get { return _nameInternal; }
            set {
                if (_nameInternal == value) return;
                _nameInternal = value;
                OnPropertyChanged();
            }
        }

        public string Status {
            get { return _statusInternal; }
            set {
                if (_statusInternal == value) return;
                _statusInternal = value;
                OnPropertyChanged();
            }
        }

        public string Tag {
            get { return _tagInternal; }
            set {
                if (_tagInternal == value) return;
                _tagInternal = value;
                OnPropertyChanged();
            }
        }

        public DateTime Added {
            get { return _addedInternal; }
            set {
                if (_addedInternal == value) return;
                _addedInternal = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}