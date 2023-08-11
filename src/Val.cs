namespace HttpLib
{
    public class Val
    {
        public Val(string key, int? value) : this(key, value?.ToString()) { }
        public Val(string key, long? value) : this(key, value?.ToString()) { }
        public Val(string key, double? value) : this(key, value?.ToString()) { }
        public Val(string key, string? value) { Key = key; Value = value; }

        public string Key { get; private set; }
        public string? Value { get; private set; }
        public void SetValue(string value)
        {
            Value = value;
        }
        public override string ToString()
        {
            return Key + "=" + Value;
        }
    }
}
