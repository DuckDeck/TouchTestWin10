using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Windows.Storage;

namespace TouchTest
{
   public class Setting<T>
    {
        string name;
        T value;
        T defaultvalue;
        bool hasvalue;
        private ApplicationDataContainer localSetting;
        public Setting(string name, T defaultvalue)
        {
            this.name = name;
            this.defaultvalue = defaultvalue;
            localSetting = ApplicationData.Current.LocalSettings;
        }

        public T Value
        {
            get
            {
                if (!hasvalue)
                {
                    Type type = typeof (T);
                    if (type.HasElementType)
                    {
                        //使用文件处理
                        throw  new Exception("this is a composite emelent handler later!");
                    }
                    else
                    {
                        if (!localSetting.Values.ContainsKey(name))
                        {
                            value = this.defaultvalue;
                            localSetting.Values[name] = value;
                        }
                        value = (T)localSetting.Values[name];
                        hasvalue = true;
                    }
                }
                return value;
            }
            set
            {
               // IsolatedStorageSettings.ApplicationSettings[name] = value;
                Type type = typeof(T);
                if (type.HasElementType)
                {
                    //使用文件处理
                    throw new Exception("this is a composite emelent handler later!");
                }
                else
                {
                    Debug.WriteLine(string.Format("will save the value from {0} to {1}",this.value,value));
                    localSetting.Values[name] = value; 
                    this.value = value;
                    hasvalue = true;
                }
               
            }

        }

        public T DefaultValue
        {
            get { return defaultvalue; }
        }

        public void ForceRefresh()
        {
            this.hasvalue = false;
           
        }

    }
}
