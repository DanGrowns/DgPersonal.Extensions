using DgPersonal.Extensions.General.Interfaces;

namespace DgPersonal.Extensions.General.Classes
{
    public class BindingData : IBindingData
    {
        public BindingData(string key, string value)
        {
            Key = key;
            Value = value;
        }
        
        public string Key { get; }
        public string Value { get; }

        public string OptionText() => Key;
        public string OptionValue() => Value;
    }
}