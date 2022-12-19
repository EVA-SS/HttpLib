namespace TestService
{
    public class Model
    {
        public Model(HttpRequest http)
        {
            header = new List<string>();
            query = new List<string>();
            foreach (var item in http.Headers)
                header.Add(item.Key + "=" + item.Value);
            foreach (var item in http.Query)
                query.Add(item.Key + "=" + item.Value);
            bool noForm = true;
            try
            {
                if (http.Form != null)
                {
                    if (http.Form.Count > 0)
                    {
                        noForm = true;
                        var form = new List<string>();
                        foreach (var item in http.Form)
                            form.Add(item.Key + "=" + item.Value);
                        data = form;
                    }
                    if (http.Form.Files != null && http.Form.Files.Count > 0)
                    {
                        file = new List<string>();
                        foreach (var item in http.Form.Files)
                            file.Add("name:" + item.FileName + ",type:" + item.ContentType);
                    }
                }
            }
            catch { }
            if (noForm)
            {
                try
                {
                    string bodystr = "";
                    using (var stream = new StreamReader(http.Body))
                    {
                        bodystr = stream.ReadToEndAsync().GetAwaiter().GetResult();
                    }
                    if (!string.IsNullOrEmpty(bodystr)) { data = bodystr; }
                }
                catch { }
            }
        }
        public List<string> query { get; set; }
        public object data { get; set; }
        public List<string> file { get; set; }
        public List<string> header { get; set; }
    }
}