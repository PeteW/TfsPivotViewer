using System;

namespace TfsVisualizer.Silverlight
{
    public class TfsWorkItem
    {
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string Project { get; set; }
        public string Type { get; set; }
        public string AssignedTo { get; set; }
        public string AreaPath { get; set; }
        public string Iteration { get; set; }
        public float Estimate { get; set; }
        public string Activity { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string History { get; set; }
        public string Description { get; set; }
    }
}