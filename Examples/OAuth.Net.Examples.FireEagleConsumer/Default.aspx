<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OAuth.Net.Examples.FireEagleConsumer.DefaultPage" %>
<%@ Import Namespace="System.Configuration" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!--

Copyright (c) 2008 Madgex

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

OAuth.net uses the Windsor Container from the Castle Project. See "Castle 
Project License.txt" in the Licenses folder.

Authors: Bruce Boughton, Chris Adams
Website: http://lab.madgex.com/oauth-net/
Email:   oauth-dot-net@madgex.com

-->
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Where am I?</title>
    <style type="text/css">
    
    html,
    body
    {
    	padding: 0;
    	margin: 0;
    }
    
    #map
    {
        background: white;
        position: absolute;
        left: 0;
        top: 0;
        width: 100%;
        height: 100%;
        padding: 0;
        margin: 0;
    }
    
    </style>
</head>
<body>
<% 
    if (UserLocation == null)
    {
        %>
            <p>Could not find your location.</p>
        <%
    } 
    else {
        %>
            <script type="text/javascript" src="http://yui.yahooapis.com/2.5.2/build/yahoo/yahoo-min.js"></script> 
            <script type="text/javascript" src="http://yui.yahooapis.com/2.5.2/build/json/json-min.js"></script>
            <script type="text/javascript" src="http://yui.yahooapis.com/2.5.2/build/event/event-min.js"></script> 
            <script type="text/javascript" src="http://yui.yahooapis.com/2.5.2/build/connection/connection-min.js"></script>
            <script type="text/javascript" src="http://www.google.com/jsapi?key=<%= ConfigurationManager.AppSettings["GoogleMapsApiKey"] %>"></script>
            <script type="text/javascript">              
              google.load("maps", "2.x");
               
              FireEagleDemo = {                
                map: null,  
                            
                latlng: null,
                
                zoom: null,  
                
                marker: null, 
                
                location: null,
                
                interruptable: true,                             
              
                initialize: function(initialLoc) {
                    this.map = new google.maps.Map2(document.getElementById("map"));
                    this.map.addControl(new google.maps.LargeMapControl());
                    this.map.addControl(new google.maps.OverviewMapControl());
                    this.map.enableScrollWheelZoom();
                    
                    this.updateMap(initialLoc.location, true);
                },
                    
                updateMap: function(loc, showMarkerInfo) {
                
                    var newCenter;
                    var newZoom;
                    var changed = false;
                
                    if (loc.point)
                    {
                        newCenter = new google.maps.LatLng(loc.point.latitude, loc.point.longitude);
                        newZoom = 15;                 
                    }
                    else if (loc.boundingbox)
                    {
                        var sw = new google.maps.LatLng(loc.boundingbox.southwest.latitude, loc.boundingbox.southwest.longitude);
                        var ne = new google.maps.LatLng(loc.boundingbox.northeast.latitude, loc.boundingbox.northeast.longitude);
                        var bounds = new google.maps.LatLngBounds(sw, ne);
                        
                        newCenter = bounds.getCenter();              
                        newZoom = this.map.getBoundsZoomLevel(bounds);
                    }
                    
                    if (!newCenter.equals(this.latlng) || newZoom != this.zoom)
                    {
                        if (newZoom == this.zoom)
                            this.map.panTo(newCenter);
                        else
                            this.map.setCenter(newCenter, newZoom);
                            
                        this.latlng = newCenter;
                        this.zoom = newZoom;
                        changed = true;
                    }
                    
                    this.location = loc;                    
                                                         
                    if (this.marker == null)
                        this.createMarker();
                    
                    if (changed || !this.latlng.equals(this.marker.getLatLng()))
                        this.marker.setLatLng(this.latlng);
                    
                    if (showMarkerInfo)
                        this.showRetrievedLocation();    
                  },
                  
                  createMarker: function()
                  {
                    this.marker = new google.maps.Marker(this.latlng, { draggable: true, bouncy: true });
                    
                    google.maps.Event.addListener(FireEagleDemo.marker, "dragstart", function() {
                        FireEagleDemo.interruptable = false;
                        FireEagleDemo.marker.closeInfoWindow();
                    });
                    
                    google.maps.Event.addListener(FireEagleDemo.marker, "dragend", function() {
                        FireEagleDemo.interruptable = true;                            
                        FireEagleDemo.updateLocation();
                    });
                    
                    this.map.addOverlay(FireEagleDemo.marker);                        
                  },
                  
                  showRetrievedLocation: function() {
                    this.updateMarkerInfo("<p>Last spotted at <b>" + this.location.name + "</b></p>", true);
                  },
                  
                  showUpdatedLocation: function() {
                    this.updateMarkerInfo("<p>Updated location to: <b>" + this.location.name + "</b></p>", true);
                  },
                  
                  updateMarkerInfo: function(html, show)
                  {                  
                    if (this.marker == null)
                        this.createMarker();
                        
                    if (show)
                        this.marker.openInfoWindowHtml(html);   
                    else
                        this.marker.bindInfoWindowHtml(html);                           
                  },
                  
                  unload: function() {
                    google.maps.Unload();
                  },
                  
                  getLocation: function(showMarkerInfo, action) {                  
                    var url = "ajax/get-location?r=" + new Date().getTime();
                    
                    var callback = {
                        success: function(o) {                            
                            var response = YAHOO.lang.JSON.parse(o.responseText);
                                                
                            if (response.authrequired)
                                window.location = response.authrequired;
                            else if (response.location)
                            {
                                FireEagleDemo.updateMap(response.location, showMarkerInfo);
                                
                                if (action != null)
                                    action.apply(this, []);
                            }
                            else if (response.error)
                                console.error("FireEagleDemo.getLocation: " + response.error);
                            else
                            {
                                console.error("FireEagleDemo.getLocation: Unexpected response");
                                console.dir(o);
                            }
                        },
                        
                        failure: function(o) {                          
                            console.error("FireEagleDemo.getLocation: No response");
                            console.dir(o);
                        }
                    }
                    
                    YAHOO.util.Connect.asyncRequest('GET', url, callback, null);
                  },
                  
                  updateLocation: function() {                  
                    var ll = FireEagleDemo.marker.getLatLng();
                    var url = "ajax/update-location?r=" + new Date().getTime();
                    
                    var callback = {
                        success: function(o) {                            
                            var response = YAHOO.lang.JSON.parse(o.responseText);
                                                
                            if (response.authrequired)
                                window.location = response.authrequired;
                            else if (response.ok)
                                FireEagleDemo.getLocation(false, 
                                    function() { FireEagleDemo.showUpdatedLocation(); } );
                            else if (response.error)
                                console.error("FireEagleDemo.updateLocation: " + response.error);
                            else
                            {
                                console.error("FireEagleDemo.updateLocation: Unexpected response");
                                console.dir(o);
                            }
                        },
                        
                        failure: function(o) {                          
                            console.error("FireEagleDemo.updateLocation: No response");
                            console.dir(o);
                        }
                    }
                    
                    YAHOO.util.Connect.asyncRequest('POST', url, callback, "lat=" + ll.lat() + "&lng=" + ll.lng());
                  }
              }
              
              YAHOO.util.Event.addListener(window, "load", function() { FireEagleDemo.initialize(<%= UserLocationJson %>); } );
              YAHOO.util.Event.addListener(window, "unload", function() { FireEagleDemo.unload(); } );
              
              setInterval(function() { if (FireEagleDemo.interruptable) FireEagleDemo.getLocation(true); }, 30 * 1000);
            </script>
            <div id="map" />
        <%
    }
%>
</body>
</html>
