﻿function setEditDate(id) {
    $(id).datepicker({ dateFormat: "yy-mm-dd" });
    var text = $(id).val();
    text = text.substring(0, 10);
    $(id).val(text);

    var hour = $('#Hour').val();
    if (hour.toString().length == 1)
        hour = '0' + hour.toString();
    var minutes = $('#Minutes').val();
    if (minutes.toString().length == 1)
        minutes = '0' + minutes.toString();
    $('#Hour').val(hour);
    $('#Minutes').val(minutes);

    $('#time-now').click(function () {
        var date = new Date();
        var month = date.getMonth() + 1;
        if (month.length == 1)
            month = '0' + month;
        var day = date.getDate();
        if (day.length == 1)
            day = '0' + day;
        $('#News_Date').val(date.getFullYear() + '-' + month + '-' + day);
        var hour = date.getHours();
        if (hour.toString().length == 1)
            hour = '0' + hour.toString();
        var minutes = date.getMinutes();
        if (minutes.toString().length == 1)
            minutes = '0' + minutes.toString();
        $('#Hour').val(hour);
        $('#Minutes').val(minutes);
    });
}