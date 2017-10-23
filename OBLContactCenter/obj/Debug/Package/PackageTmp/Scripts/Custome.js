
$(document).ready(function () {

    //$('.navMenu').click(function (data) {
    //    $(this).parent().find('ul').addClass('start active open');
    //});
    $(document).on("click", ".editItem", function () {
        var url = $(this).attr("data-url");
        //var url = "../UserSetting/EditUser";
        var id = $(this).attr('data-id'); // the id that's given to each button in the list
        $.get(url + '/' + id, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", "#createBtn", function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr('data-id'); // the id that's given to each button in the list

        $.get(url + '/' + id, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", ".editTicker", function () {
        var url = $(this).attr("data-url");
        //var url = "../UserSetting/EditUser";
        var id = $(this).attr('data-id'); // the id that's given to each button in the list
        $.get(url + '/' + id, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", "#createTicker", function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr('data-id'); // the id that's given to each button in the list

        $.get(url + '/' + id, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", ".editGroup", function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr('data-id'); // the id that's given to each button in the list
        
        $.get(url + '/' + id, function (data) {
            $('.modal-body').html(data);
            $('#GroupInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", "#createGroup", function () {
        var url = $(this).attr("data-url");
        //var url = '../GroupSetting/CreateGroup';
        $.get(url, function (data) {
            $('.modal-body').html(data);
            $('#GroupInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", ".editRole", function () {
        var url = $(this).attr("data-url");
        //var url = '../RoleSetting/CreateEditRole';
        var id = $(this).attr('data-id'); // the id that's given to each button in the list
        //alert(url2 + '/' + id);
        $.get(url + '/' + id, function (data) {
            $('.modal-body').html(data);
            $('#RoleInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", "#createRole", function () {
        var url = $(this).attr("data-url");
        var url = '../RoleSetting/CreateEditRole';
        var id = 0; // the id that's given to each button in the list
        $.get(url + '/' + id, function (data) {
            $('.modal-body').html(data);
            $('#RoleInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", ".editSource", function () {
        var url = $(this).attr("data-url");
        //var url = '../SourceProductSetting/EditSource';
        var id = $(this).attr('data-id'); // the id that's given to each button in the list
        //alert(url2 + '/' + id);
        $.get(url + '/' + id, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", "#createSource", function () {
        var url = $(this).attr("data-url");
        //var url = '../SourceProductSetting/CreateSource';
        $.get(url, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });
    $(document).on("click", ".editProd", function () {
        var url = $(this).attr("data-url");
        //var url = '../SourceProductSetting/EditProduct';
        var id = $(this).attr('data-id'); // the id that's given to each button in the list
        //alert(url2 + '/' + id);
        $.get(url + '/' + id, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", "#createProd", function () {
        var url = $(this).attr("data-url");
        //var url = '../SourceProductSetting/CreateProduct';
        $.get(url, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });
    

    $(document).on("click", ".editCat", function () {
        var url = $(this).attr("data-url");
        //var url = '../CallCatTypeSetting/EditCatagory';
        var id = $(this).attr('data-id'); // the id that's given to each button in the list
        //alert(url2 + '/' + id);
        $.get(url + '/' + id, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", "#createCat", function () {
        var url = $(this).attr("data-url");
        //var url = '../CallCatTypeSetting/CreateCatagory';
        $.get(url, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", ".editType", function () {
        var url = $(this).attr("data-url");
        //var url = '../CallCatTypeSetting/EditType';
        var id = $(this).attr('data-id'); // the id that's given to each button in the list
        //alert(url2 + '/' + id);
        $.get(url + '/' + id, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    $(document).on("click", "#createType", function () {
        var url = $(this).attr("data-url");
        //var url = '../CallCatTypeSetting/CreateType';
        $.get(url, function (data) {
            $('.modal-body').html(data);
            $('#DetailInfo').modal('show');
            $.validator.unobtrusive.parse($('.modal-body'));
        });
    });

    



});
