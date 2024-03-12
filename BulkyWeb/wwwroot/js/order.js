$(document).ready(function () {
    loadDataTable();
});

var dataTable;

function loadDataTable() {
    dataTable = $('#tblData').dataTable({
        "ajax": { "url": "/Admin/Order/getall" },
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
                            <a href="/Admin/order/detail?orderId=${data}" class="btn btn-primary mx-2">Details</a>

                </div>`
                },"width":"15%"
            }
        ]
    });
}