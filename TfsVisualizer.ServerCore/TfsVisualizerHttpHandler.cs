using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsVisualizer.ServerCore.Configuration;

namespace TfsVisualizer.ServerCore
{
    public class TfsVisualizerHttpHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            //gather criteria from url
            var criteria = DeepZoomHandlerCriteria.Read(context);
            //query tfs
            var tfsWorkItems = GetTfsWorkItems(criteria);
            //serialize to json and return response
            var serializer = new DataContractJsonSerializer(tfsWorkItems.GetType());
            serializer.WriteObject(context.Response.OutputStream, tfsWorkItems);
            context.Response.End();
        }

        public bool IsReusable
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Gets the TFS work items.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public List<TfsWorkItem> GetTfsWorkItems(DeepZoomHandlerCriteria criteria)
        {
            var result = new List<TfsWorkItem>();

            var configurationServer = TfsConfigurationServerFactory.GetConfigurationServer(new Uri(SettingsManager.TfsUrl));
            var catalogNode = configurationServer.CatalogNode;
            var catalogNodes = catalogNode.QueryChildren(new Guid[] { CatalogResourceTypes.ProjectCollection }, false, CatalogQueryOptions.None);

            foreach (var node in catalogNodes)
            {
                var collectionId = new Guid(node.Resource.Properties["InstanceId"]);
                var teamProjectCollection = configurationServer.GetTeamProjectCollection(collectionId);
                var workItemStore = (WorkItemStore)teamProjectCollection.GetService(typeof(WorkItemStore));
                var workItemCollection = GetWorkItemCollection(workItemStore, criteria);
                workItemCollection.PageSize = 200;
                foreach (WorkItem workItem in workItemCollection)
                {
                    var estimate = 0f;
                    var url = SettingsManager.TfsUrl + "web/wi.aspx?pcguid=" + collectionId + "&id=" + workItem.Id;
                    try
                    {
                        if (workItem.Type.Name == "Product Backlog Item")
                            estimate = float.Parse(workItem.Fields["Effort"].Value.ToString());
                        else
                            estimate = float.Parse(workItem.Fields["Remaining Work"].Value.ToString());
                    }
                    catch
                    {
                    }
                    var team = string.Empty;
                    try
                    {
                        team = workItem.Fields["Team"].Value.ToString();
                    }
                    catch{}

                    var tfsWorkItem = new TfsWorkItem();
                    tfsWorkItem.Url = url;
                    tfsWorkItem.DateCreated = workItem.CreatedDate;
                    tfsWorkItem.CreatedBy = workItem.CreatedBy;
                    tfsWorkItem.Team = team;
                    tfsWorkItem.Status = workItem.State;
                    tfsWorkItem.Project = workItem.Project.Name;
                    tfsWorkItem.Type = workItem.Type.Name;
                    tfsWorkItem.AssignedTo = workItem.Fields["Assigned To"].Value.ToString();
                    tfsWorkItem.AreaPath = workItem.AreaPath;
                    tfsWorkItem.Iteration = workItem.IterationPath;
                    tfsWorkItem.Estimate = estimate;
                    tfsWorkItem.Activity = workItem.GetActionsHistory().GetLength(0).ToString();
                    tfsWorkItem.Id = workItem.Id.ToString();
                    tfsWorkItem.Title = workItem.Title;
                    tfsWorkItem.History = Regex.Replace(workItem.History, @"<[^>]*>", String.Empty);
                    tfsWorkItem.Description = Regex.Replace(workItem.Description, @"<[^>]*>", String.Empty);
                    result.Add(tfsWorkItem);
                }
            }
            return result;
        }

        private static WorkItemCollection GetWorkItemCollection(WorkItemStore workItemStore, DeepZoomHandlerCriteria criteria)
        {
            //http://msdn.microsoft.com/en-us/library/ms194971.aspx
            var wiql = @"
                   Select 
                        [Id]
                        ,[Title]
                        ,[Assigned To]
                        ,[Original Estimate]
                        ,[Remaining Work]
                        
                        ,[Team]
                   From 
                        WorkItems";
            if (!criteria.IsEmpty)
            {
                var whereClause = new List<string>();
                if (!string.IsNullOrEmpty(criteria.ProjectName))
                    whereClause.Add(string.Format("[Team Project] = '{0}'", criteria.ProjectName));
                if (!string.IsNullOrEmpty(criteria.IterationPath))
                    whereClause.Add(string.Format("[Iteration Path] = '{0}'", criteria.IterationPath));
                if (!string.IsNullOrEmpty(criteria.WorkItemType))
                    whereClause.Add(string.Format("[Work Item Type] = '{0}'", criteria.WorkItemType));
                wiql += " WHERE " + string.Join(" AND ", whereClause.ToArray());
            }
            var workItemCollection = workItemStore.Query(wiql);
            return workItemCollection;
        }
    }

    public class DeepZoomHandlerCriteria
    {
        public static DeepZoomHandlerCriteria Read(HttpContext context)
        {
            var result = new DeepZoomHandlerCriteria();
            result.ProjectName = context.Request["ProjectName"];
            result.IterationPath = context.Request["IterationPath"];
            return result;
        }
        public bool IsEmpty { get { return string.IsNullOrEmpty(ProjectName) && string.IsNullOrEmpty(IterationPath); } }
        public string ProjectName { get; set; }
        public string IterationPath { get; set; }
        public string WorkItemType { get; set; }
    }
}