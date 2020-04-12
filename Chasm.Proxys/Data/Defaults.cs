namespace Chasm.Proxys.Data
{
    public static class Defaults
    {
        //TODO: Find a better regex if exsist
        public const string PROXY_PARSER_REGEX = @"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\:((5[0-9]{1,4})|(6[0-5]{1,2}[0-3][0-5])|([0-9]{1,4}))";
    }
}
