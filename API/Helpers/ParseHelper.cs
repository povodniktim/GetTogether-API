namespace API.Helpers
{
    public static class ParseHelper
    {
        public static Dictionary<string, string> ParseObjToDictionary<T>(T obj)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(obj) as string;
                    if (value != null)
                    {
                        dictionary[property.Name] = value;
                    }
                }
            }

            return dictionary;
        }
    }
}
