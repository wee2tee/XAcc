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
    $('.ui.dropdown.modules').dropdown();
    $('.ui.radio.checkbox').checkbox();
    $('.ui.accordion').accordion('refresh');
    $('.ui.sidebar').sidebar('toggle');
    $('.ui.icon.button').popup();
    $('#top-menu .item').tab();

    // show top menu at first
    //$('#show-top-menu').click();


    //$("body").mousewheel(function (event, delta) {
    //    event.preventDefault();
    //    console.log(delta);
    //    console.log(event);

    //    this.scrollLeft -= (delta * 30);


    //});
    //$('#glacc-table .item').tab({
    //    cache: false,
    //    context: 'parent',
    //    auto: true,
    //    path: '/Glacc/',
        
    //});
    //$('.main-menu').on('click', function (event) {
    //    event.preventDefault();
    //    var modul = $(this).attr('aria-module');

    //    $.ajax({
    //        url: '/Home/GetSubMenuDisplay',
    //        method: 'GET',
    //        data: { modul_name: modul },
    //        beforeSend: function () {
    //            $('#sub-menu-display').html('');
    //        },
    //        success: function (data) {
    //            $('#sub-menu-display').html(data);
    //            $('.sub-menu-display-panel').fadeIn(200);
    //        }
    //    });

    //    //$('#sub-menu-display').html('');
    //    //$('#sub-menu-display').jstree({
    //    //    "core": {
    //    //        "data": {
    //    //            "url": "/Home/GetSubMenuDisplay",
    //    //            "method": "GET",
    //    //            "dataType": "json",
    //    //            "data": { modul_name: modul }
    //    //        },
    //    //        "multiple": false,
    //    //    },
    //    //    //"plugins": ["wholerow"]
    //    //});
    //    //$('.sub-menu-display-panel').fadeIn(200);
    //});
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
        //var sub_menu_display_panel = $(".sub-menu-display-panel");

        if ($(this).parents(".nav-sidebar").offset().left >= 0) {
            $(sidebar).animate({
                left: "-=160"
            }, 200, function () {
                $('#nav-container').addClass('minimize');
            });
            $(sidebar_menu).animate({
                left: "-=160"
            }, 200, function () {

                });
            //$(sub_menu_display_panel).fadeOut('fast');
        }
        else {
            $(sidebar).animate({
                left: "+=160"
            }, 200, function () {
                $('#nav-container').removeClass('minimize');
            });
            $(sidebar_menu).animate({
                left: "+=160"
            }, 200, function () {

                });
            //$(sub_menu_display_panel).fadeIn('fast');
        }
    },
    addSlimScroll: function (event) {
        var height = $(this).innerHeight();
        $(this.slimScroll({
            height: '"' + height + 'px"'
        }))
    },

    showSubMenu: function (event) {
        var modul = $(this).attr('aria-module');

        $.ajax({
            url: '/Home/GetSubMenuDisplay',
            method: 'GET',
            data: { modul_name: modul },
            success: function (data) {
                $('.sub-menu-display-panel').fadeOut(300, function () {
                    $('#sub-menu-display').html(data);
                    $('.sub-menu-display-panel').fadeIn(300);
                });
            }
        });
    },

    closeSubMenu: function (event) {
        $(this).parent(".sub-menu-display-panel").fadeOut(200);
        $(this).parent(".sub-menu-display-panel").find('#sub-menu-display').html('');
    },

    toggleShowTopMenu: function (event) {
        var menu_panel = $(this).parents('#top-menu-panel');
        var toggle_btn = $(this);

        if (!($(menu_panel).hasClass('min'))) {
            $('.dim-overlay').fadeOut(200);
            $(menu_panel).animate({
                top: "-307px"
            }, 300, function () {
                $(menu_panel).addClass('min');
                $(toggle_btn).html('<i class="fa fa-angle-down"></i>Show Menu');
            });
        }
        else {
            $('.dim-overlay').fadeIn(200);
            $(menu_panel).animate({
                top: "0px"
            }, 300, function () {
                $(menu_panel).removeClass('min');
                $(toggle_btn).html('<i class="fa fa-angle-up"></i>Hide Menu');
            });
        }
        
    },

    scrollHorizontal: function (event) {
        event.preventDefault();
        var top_menu = $(this);
        var segment = $(this).find('.segment:visible')[0];
        var elem = $(this).find('.sub-menu-wrapper');
        var wrapper_width = $(this).innerWidth();
        var curr_pos = $(segment).scrollLeft();

        if (event.deltaY > 0) {
            //console.log('move right');
            $(segment).scrollLeft(curr_pos + 255);
        }
        else {
            //console.log('move left');
            $(segment).scrollLeft(curr_pos - 255);
        }
    }

});