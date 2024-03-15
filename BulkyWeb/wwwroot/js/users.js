$(document).ready(function () {
    loadUserData();
});

var dataTable;
function loadUserData() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: "/admin/user/getall" },
        "columns": [
            { data: "name", "width": "20%" },
            { data: "email", "width": "20%" },
            { data: "phoneNumber", "width": "15%" },
            { data: "company.name", "width": "15%" },
            { data: "role", "width": "10%" },
            
            {
                data: { id:'id', lockoutEnd:"lockoutEnd"},
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();//getting the lockout date from db

                    if (lockout > today) {
                        return `<div class="test-center">
                        <a onclick="lockunclock('${data.id}')" class="btn btn-danger text-white" style="cursor:pointer">Lock</a>
                        <a href="/Admin/User/RoleManagement?userId=${data.id}" class="btn btn-primary text-white" style="cursor:pointer">Permission</a>
                        </div>`
                    }
                    else {
                        return `<div class="test-center">
                        <a onclick="lockunclock('${data.id}')" class="btn btn-success text-white" style="cursor:pointer">Unlock</a>

                        <a href="/Admin/User/RoleManagement?userId=${data.id}" class="btn btn-primary text-white" style="cursor:pointer">Permission</a>
                        </div>`
                    }
                },
                "width": "20%"
            }
        ]
    });
}

function lockunclock(id) {
    $.ajax({
        type: "POST",
        url: "/admin/user/LockUnlock",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }

        }
    });
}
