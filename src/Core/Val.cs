using System;

namespace HttpLib
{
    public class Val
    {
        public Val(string key, int value) : this(key, value.ToString()) { }
        public Val(string key, long value) : this(key, value.ToString()) { }
        public Val(string key, double value) : this(key, value.ToString()) { }
        public Val(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }


        public string Key { get; private set; }
        public string Value { get; private set; }
        public void SetValue(string value)
        {
            this.Value = value;
        }
        public override string ToString()
        {
            return Key + "=" + Value;
        }
        public string ToStringEscape()
        {
            if (Value != null)
            {
                return Key + "=" + Uri.EscapeDataString(Value);
            }
            else { return Key + "="; }
        }
    }
}
