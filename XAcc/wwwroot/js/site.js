// Write your JavaScript code.

var confirmLogout = function () {
    if (confirm("ยืนยันการออกจากระบบ") === true) {
        window.location = "/Home/Logout";
    }
}

$(document).ready(function () {
    
    //$(".scrollpane").addSlimScroll();

    $('.ui.accordion').accordion('refresh');
    $('.ui.sidebar').sidebar('toggle');
    $('.ui.icon.button').popup();
});

$.fn.extend({
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
            return target_elem.modal({
                "show": true,
                "backdrop": true,
                "keyboard": false
            });
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