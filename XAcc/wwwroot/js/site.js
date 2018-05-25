// Write your JavaScript code.

var confirmLogout = function () {
    if (confirm("ยืนยันการออกจากระบบ") === true) {
        window.location = "/Home/Logout";
    }
}

var clearConfirmDialog = function () {
    $('#confirm-header').html('');
    $('#confirm-message').html('');
    $('#confirm-dialog').modal('hide');
}

function preventEnterSubmit(e) {
    if (e.which == 13) {
        var $targ = $(e.target);

        if (!$targ.is("textarea") && !$targ.is(":button,:submit")) {
            var focusNext = false;
            $(this).find(":input:visible:not([disabled],[readonly]), a").each(function () {
                if (this === e.target) {
                    focusNext = true;
                }
                else if (focusNext) {
                    $(this).focus();
                    return false;
                }
            });

            return false;
        }
    }
}

$(document).ready(function () {
    $('input').keypress(function (event) {
        if (event.keyCode === 10 || event.keyCode === 13)
            event.preventDefault();
    });

    //$(".scrollpane").addSlimScroll();
    $('.popup').popup({
        on: 'click'
    });
    $('.ui.dropdown').dropdown();
    $('.ui.accordion').accordion('refresh');
    $('.ui.sidebar').sidebar('toggle');
    $('.ui.icon.button').popup();
    $('#glacc-table .item').tab({
        cache: false,
        context: 'parent',
        auto: true,
        path: '/Glacc/',
        
    });
});

$.fn.extend({
    showConfirmDialog: function (caption, msg, buttons) {
        if (buttons.toLowerCase().indexOf('ok') > -1) {
            $('#confirm-btn-ok').show();
        }
        else {
            $('#confirm-btn-ok').hide();
        }

        if (buttons.toLowerCase().indexOf('cancel') > -1) {
            $('#confirm-btn-cancel').show();
        }
        else {
            $('#confirm-btn-cancel').hide();
        }

        if (buttons.toLowerCase().indexOf('close') > -1) {
            $('#confirm-btn-close').show();
        }
        else {
            $('#confirm-btn-cancel').hide();
        }

        var form_control = $(this).attr("data-form-control");
        var form_action = $(this).attr("data-form-action");
        var data_id = $(this).attr("data-form-action-id");
        $("#confirm-dialog-form").attr({ action: "/" + form_control + "/" + form_action });
        $("#confirm-dialog-form [name='form_action_id']").val(data_id);

        $("#confirm-header").html(caption);
        $("#confirm-message").html(msg);
        $("#confirm-dialog").modal('show');
    },

    getDataList: function (sernum, phrase, username, pass) {
        var target_elem = $(this);
        var post_data = new Object();
        post_data.sernum = sernum;
        post_data.phrase = phrase;
        post_data.username = username;
        post_data.pass = pass;

        $.ajax({
            type: "POST",
            url: "/Home/GetDataList",
            data: JSON.stringify(post_data),
            contentType: "application/json; charset=utf-8",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            error: function (err) {
                if (err.status === 400) {
                    window.location.reload(true);
                }
            }
        }).done(function (data) {
            target_elem.html(data);
            $(target_elem).modal('show');
            //return target_elem.modal({
            //    "show": true,
            //    "backdrop": true,
            //    "keyboard": false
            //});
        });

    },
    getSelectDataList: function (sernum, username) {
        var target_elem = $(this);
        var post_data = new Object();
        post_data.sernum = sernum;
        post_data.phrase = '';
        post_data.username = username;
        post_data.pass = '';

        $.ajax({
            type: "POST",
            url: "/Home/GetSelectDataList",
            data: JSON.stringify(post_data),
            contentType: "application/json; charset=utf-8",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            error: function (err) {
                if (err.status === 400) {
                    window.location.reload(true);
                }
            }
        }).done(function (data) {
            target_elem.html(data);
            $(target_elem).modal('show');
        });

    },
    toggleShowSidebar: function (event) {
        var sidebar = $(this).parents(".nav-sidebar");
        var sidebar_menu = $(sidebar).find(".nav-sidebar-menu");
        var sub_menu_display_panel = $(".sub-menu-display-panel");

        if ($(this).parents(".nav-sidebar").offset().left >= 0) {
            $(sidebar).animate({
                left: "-=160"
            }, 200, function () {

            });
            $(sidebar_menu).animate({
                left: "-=160"
            }, 200, function () {

                });
            $(sub_menu_display_panel).fadeOut('fast');
        }
        else {
            $(sidebar).animate({
                left: "+=160"
            }, 200, function () {

            });
            $(sidebar_menu).animate({
                left: "+=160"
            }, 200, function () {

                });
            $(sub_menu_display_panel).fadeIn('fast');
        }
    },
    addSlimScroll: function (event) {
        var height = $(this).innerHeight();
        $(this.slimScroll({
            height: '"' + height + 'px"'
        }))
    }
});