using System.Collections.Generic;

namespace PipServices4.Commons.Reflect
{
    public interface IProperties
    {
        List<string> GetPropertyNames();
        object GetProperty(string name);
        void SetProperty(string name, object value);
    }
}
