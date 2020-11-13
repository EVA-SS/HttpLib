namespace HttpLib
{
    public class Val
    {
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
    }
}
