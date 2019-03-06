jQuery(function ($) {
    function fix_size() {
        var images = $('.img-container img');
        image(setsize);

        function setsize() {
            var img = $(this),
                img_dom = img.get(0),
                container = img.parents('.img-container');
            if (img_dom.complete) {
                resize();
            } else img.one('load', resize);

            function resize() {
                if ((container.width() / container.height()) > (img_dom.width / img_dom.height)) {
                    img.width('100%');
                    img.height('auto');
                } else {
                    img.height('100%');
                    img.width('auto');
                }
                var marginx=(img.width()-container.width())/-2,
                    marginy=(img.height()-container.height())/-2;
               console.log(marginx);
               img.css({'margin-left': marginx, 'margin-top': marginy});
                    
            }
        }
    }
    $(window).on('resize', fix_size);
    fix_size();
});