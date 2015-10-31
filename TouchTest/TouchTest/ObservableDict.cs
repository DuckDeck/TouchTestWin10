using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchTest
{
    public class ObservableDict<Tkey, TValue> : Dictionary<Tkey, TValue>,INotifyPropertyChanged
     {

        public int Count
        {
            get { return base.Count; }
        }

        public void AddValue(Tkey key, TValue value)
        {
            base.Add(key, value);
            OnPropertyChanged("Count");
        }

        public bool RemoveValue(Tkey key)
        {
            bool result = base.Remove(key);
            OnPropertyChanged("Count");
            return result;
        }

        public void ClearValue()
        {
            base.Clear();
            OnPropertyChanged("Count");
        }

        public void OnPropertyChanged(string propertyName)
         {
             if (PropertyChanged != null)
             {
                 PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
             }
         }

        public event PropertyChangedEventHandler PropertyChanged;
     }
}
