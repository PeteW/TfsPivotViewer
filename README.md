TfsPivotViewer
==============

Navigate your population of TFS work items with the power of silverlight pivot viewer and deep zoom

###Navigate TFS in turbo mode
* Minimal installation: install/register an http handler, add the XAP to a page and thats it
* Works in SharePoint, MVC, ASPX sites or any other IIS site

This solution contains a silverlight application which reads query string filters (project name, iteration, area path) and calls an HTTP handler. The HttpHandler which will query TFS and return work items in JSON which the silverlight application binds to a pivot viewer. Silverlight 5 required. 

![Screenshot 1](https://github.com/PeteW/TfsPivotViewer/raw/master/screenshot1.png)
![Screenshot 1](https://github.com/PeteW/TfsPivotViewer/raw/master/screenshot2.png)


### Installation
* Copy the TfsVisualizer dll to the bin of your web site
* Register the http handler like below:


```xml
<handlers>
      <add name="DeepZoomHandler" path="*TfsWorkItems.json" verb="GET" type="TfsVisualizer.Util.DeepZoomHttpHandler" />
</handlers>
```
* Copy the xap to an accessible location within the same site
* Edit a page to reference the xap:


```html
    <object id="4" data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
        <param name="source" value='@Url.Content("~/ClientBin/TfsVisualizer.Silverlight.xap")'/>
        <param name="onError" value="onSilverlightError" />
        <param name="background" value="white" />
        <param name="minRuntimeVersion" value="4.0.50401.0" />
        <param name="autoUpgrade" value="true" />
        <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50401.0" style="text-decoration: none">
            <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style: none" />
        </a>
    </object>
    <iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px; border: 0px"></iframe>
```
* use Silverlight.js if desired

### Usage
Browse to the page. To improve performance, filter by project name, iteration or area path such as:

http://path.tosite/page.html/?ProjectName=MyProjec&IterationPath=myIterationPath