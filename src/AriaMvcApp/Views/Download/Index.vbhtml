@ModelType IEnumerable(Of AriaMvcApp.FDInfo)

@Code
    
    
    Dim grid = New WebGrid(source:=Model, defaultSort:="Genre")
    
End Code


<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />

    <title>Download Dir - @ViewData("Path")</title>
    <script src="~/Scripts/jquery-1.9.1.js"></script>
    <script src="~/Scripts/jquery-ui-1.10.1.js"></script>
    <script src="~/Scripts/jquery.validate.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.js"></script>
    <script src="~/Scripts/jquery.unobtrusive-ajax.js"></script>
    <script type="text/javascript">




        function ShowToDo() {
            $("#todo").show();
            $("#lnkReflish").attr("href", document.URL);
        }





</script>
</head>
<body>



    <h2>View</h2>

    <div>
        <a href="~/index.html">Return Back </a>
    </div>

    <div>

        @ViewData("Path")

    </div>

    <div>
        @If ViewData.ContainsKey("Message") Then
            @<strong>@ViewData("Message")</strong>
    
        End If
    </div>

    <div id="list">



        @grid.GetHtml(
            tableStyle:="grid",
            headerStyle:="head",
            alternatingRowStyle:="alt",
            columns:=grid.Columns(
            grid.Column("Name", canSort:=True, format:=@@<span>@Html.ActionLink(item.Name, "Index", New With {.path = item.Path})</span>),
            grid.Column("Size", canSort:=True, format:=@@<span>@item.Size</span>),
            grid.Column("Creation Time", canSort:=True, format:=@@<span>@item.CreationTime</span>),
            grid.Column("Delete", canSort:=False, format:=@@<strong>@If Not item.Name=".." Then
                                                                        @Ajax.ActionLink("Delete", "Delete", New With {.path = item.Path}, New AjaxOptions With {.Confirm = "Are You Sure To Delete This?", .UpdateTargetId = "txtresult", .OnComplete = "ShowToDo()"})
                                                                    End If </strong>)))

       
    </div>

    <div id="todo" style="background-color: orange ;display:none;">
        <div id="txtresult"></div>
        Please <a id="lnkReflish" href="" >Reflash</a> !
    </div>


</body>
</html>

