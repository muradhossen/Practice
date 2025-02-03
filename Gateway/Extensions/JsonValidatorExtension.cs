using System.Text.Json;

namespace Gateway.Extensions
{
    public static class JsonValidatorExtension
    {

        public static bool TryValidateJson(this String responseBodyContent, out object value)
        {
            try
            {
                value = JsonSerializer.Deserialize<object>(responseBodyContent);
                return true;
            }
            catch (JsonException)
            {
                value = responseBodyContent;
                return false;
            }
        }
    }
}
