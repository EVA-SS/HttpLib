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
            Key = key;
            Value = value;
        }

        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
        public void SetValue(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Key + "=" + Value;
        }

        public string ToStringEscape()
        {
            if (Value != null) return Key + "=" + Uri.EscapeDataString(Value);
            else return Key + "=";
        }
    }
}