$(function(){
    'use strict';
    

    $(".single_post").slice(0, 4).show();
    $(".load").on("click", function(){
        $(".single_post:hidden").slice(0, 4).slideDown("slow");
        if ($(".single_post:hidden").length == 0) {
            $(".load").fadeOut("slow");
        }
    });
});