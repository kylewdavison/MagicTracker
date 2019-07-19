$('a[rel=popover]').popover({
    html: true,
    trigger: 'hover',
    content: function () {
        return '<img src="' + $(this).data('img') + '" />';
    }
});