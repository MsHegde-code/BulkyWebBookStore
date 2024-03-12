$(document).ready(function () {
    var url = window.location.search;
    //filtering the dataTable based on status
    if (url.includes("inprocess"))
        loadDataTable("inprocess");

    else if (url.includes("pending"))
        loadDataTable("pending");

    else if (url.includes("completed"))
        loadDataTable("completed");

    else if (url.includes("approved"))
        loadDataTable("approved");

    else
        loadDataTable("all");


    //loadDataTable();
});

var dataTable;

function loadDataTable(status) {
    dataTable = $('#tblData').dataTable({
        "ajax": { "url": "/Admin/Order/getall?status=" + status },
        "columns": [
            { data: "id", "width":"10%" },
            { data: "name", "width": "20%" },
            { data: "phoneNumber", "width": "15%" },
            { data: "applicationUser.email", "width": "20%" },
            { data: "orderStatus", "width": "10%" },
            { data: "orderTotal", "width": "10%" },
            {
                data: "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                            <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-primary mx-2">Details</a>

                </div>`
                },"width":"15%"
            }
        ]
    });
}