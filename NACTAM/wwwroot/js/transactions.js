/// <summary>
/// This is needed for filter and search through transactions
/// Authornames: Marco Lembert
/// </summary>
$(document).ready(function () {
    $('#transaction-table').DataTable({
        "paging": false,
        "scrollY": "500px",
        "scrollCollapse": true,
        initComplete: function () {

        }
    });
});