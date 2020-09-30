namespace SCS.HomePhotos.Service.Workers
{
    public class TaskCompleteInfo
    {
        public TaskCompleteInfo(TaskType type, string contextUserName)
        {
            Type = type;
            ContextUserName = contextUserName;
        }
        public TaskCompleteInfo(TaskType type, string contextUserName, bool success) : this(type, contextUserName)
        {            
            Success = success;
        }
        public TaskCompleteInfo(TaskType name, string contextUserName, bool success, object data) : this(name, contextUserName, success)
        {
            Data = data;      
        }

        public TaskType Type { get; set; }
        public bool? Success { get; set; }
        public object Data { get; set; }
        public string ContextUserName { get; set; }
    }
}
