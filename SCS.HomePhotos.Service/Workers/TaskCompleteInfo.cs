namespace SCS.HomePhotos.Service.Workers
{
    /// <summary>
    /// Completed task info.
    /// </summary>
    public class TaskCompleteInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCompleteInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="contextUserName">Name of the context user.</param>
        public TaskCompleteInfo(TaskType type, string contextUserName)
        {
            Type = type;
            ContextUserName = contextUserName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCompleteInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="contextUserName">Name of the context user.</param>
        /// <param name="success">if set to <c>true</c> task was successful.</param>
        public TaskCompleteInfo(TaskType type, string contextUserName, bool success) : this(type, contextUserName)
        {            
            Success = success;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCompleteInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="contextUserName">Name of the context user.</param>
        /// <param name="success">if set to <c>true</c> task was successful.</param>
        /// <param name="data">The data.</param>
        public TaskCompleteInfo(TaskType name, string contextUserName, bool success, object data) : this(name, contextUserName, success)
        {
            Data = data;      
        }


        /// <summary>
        /// Gets or sets the task type.
        /// </summary>
        /// <value>
        /// The task type.
        /// </value>
        public TaskType Type { get; set; }

        /// <summary>
        /// Gets or sets the success.
        /// </summary>
        /// <value>
        /// The success.
        /// </value>
        public bool? Success { get; set; }

        /// <summary>
        /// Gets or sets the task data.
        /// </summary>
        /// <value>
        /// The task data.
        /// </value>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the name of the context user.
        /// </summary>
        /// <value>
        /// The name of the context user.
        /// </value>
        public string ContextUserName { get; set; }
    }
}
