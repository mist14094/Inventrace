<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BigOrdersChart.ascx.cs"
    Inherits="Admin.UserControls.Dashboard.BigOrdersChart" %>
<article class="chart-block">
    <div class="clearfix">
        <h2 class="chart-orders-title">
            <%= Resources.Resource.Admin_Default_Orders %></h2>
        <div class="chart-orders-period">
            <div data-plugin="radiolist" class="radiolist">
                <label>
                    <input type="radio" checked="checked" id="gr-chart0" name="gr-chart" value="#chartWeek" />
                    <%= Resources.Resource.Admin_Charts_Week %></label>
                <label>
                    <input type="radio" id="gr-chart1" name="gr-chart" value="#chartMounth" />
                    <%= Resources.Resource.Admin_Charts_Mounth %></label>
            </div>
        </div>
    </div>
    <div id="chartWeek" style="width: 99%; height: 190px;" data-plugin="chart" data-chart="<%= RenderData(7) %>"
        data-chart-options="{xaxis : { 
            mode: 'time', 
            timeformat: '%d %b', 
            min: <%= GetTimestamp(DateTime.Now.AddDays(-7).Date) %> ,    
            max: <%= GetTimestamp(DateTime.Now.AddDays(1).Date) %>
            }
        }">
    </div>
    <div id="chartMounth" style="display: none; width: 99%; height: 190px;" data-plugin="chart" data-chart="<%= RenderData(30) %>"
        data-chart-options="{xaxis : { 
                mode: 'time', 
                timeformat: '%d %b', 
                min: <%= GetTimestamp(DateTime.Now.AddDays(-30).Date) %> ,    
                max: <%= GetTimestamp(DateTime.Now.AddDays(1).Date) %>
            }
        }">
    </div>
</article>
