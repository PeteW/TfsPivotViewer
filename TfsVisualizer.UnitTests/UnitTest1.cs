using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Client.Catalog.Objects;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectCollection = Microsoft.TeamFoundation.WorkItemTracking.Client.ProjectCollection;

namespace TfsVisualizer.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // URI of the team project collection
            var url = @"http://moy.corp.visionshareinc.com:8080/tfs/";
            var configurationServer = TfsConfigurationServerFactory.GetConfigurationServer(new Uri(url));
            var catalogNode = configurationServer.CatalogNode;
            var catalogNodes = catalogNode.QueryChildren(new Guid[] { CatalogResourceTypes.ProjectCollection },false, CatalogQueryOptions.None);

            // tpc = Team Project Collection
            foreach (var node in catalogNodes)
            {
                var collectionId = new Guid(node.Resource.Properties["InstanceId"]);
                var teamProjectCollection = configurationServer.GetTeamProjectCollection(collectionId);
                var workItemStore = (WorkItemStore)teamProjectCollection.GetService(typeof(WorkItemStore));
                WorkItemCollection workItemCollection = workItemStore.Query(@"
   Select [Id], [Title] 
   From WorkItems
   Where [Work Item Type] = 'Task'
   Order By [State] Asc, [Changed Date] Desc");
                //                var teamProject = workItemStore.Projects["DinnerNow"];
                // Get catalog of tp = 'Team Projects' for the tpc = 'Team Project Collection'
                var queryChildren = node.QueryChildren(new Guid[] { CatalogResourceTypes.TeamProject },false, CatalogQueryOptions.None);

                foreach (var p in queryChildren)
                {
                    Debug.Write(Environment.NewLine + " Team Project : " + p.Resource.DisplayName + " - " + p.Resource.Description + Environment.NewLine);
                }
                foreach (WorkItem workItem in workItemCollection)
                {
                    Console.WriteLine(workItem.Project.Name + ": "+workItem.Id + " - "+ workItem.Title);
                }
            }
        }
    }
}
