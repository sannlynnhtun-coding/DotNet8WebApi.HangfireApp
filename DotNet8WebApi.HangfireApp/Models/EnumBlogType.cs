namespace DotNet8WebApi.HangfireApp.Models
{
    public enum EnumBlogType
    {
        None,
        List,
        Create,
        Update,
        Delete,
    }

    public class CronRequestModel
    {
        public string ServiceName { get; set; }
        public string CronExpression { get; set; }
    }
}
