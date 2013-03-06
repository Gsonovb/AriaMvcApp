Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports System.Web.Routing

Public Class RouteConfig
    Public Shared Sub RegisterRoutes(ByVal routes As RouteCollection)
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")

        routes.MapRoute( _
            name:="Download", _
            url:="Download/{action}/{*path}", _
            defaults:=New With {.controller = "Download", .action = "Index", .path = UrlParameter.Optional} _
        )
    End Sub
End Class