AOS.init({
    once: true
});
function HomeJs() {
    $(".banner").slick({
        dots: false,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 1500,
        autoplay: true,
        autoplaySpeed: 3000,
        arrows: false,
    });
    $(".banner-mb").slick({
        dots: false,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 1500,
        autoplay: true,
        autoplaySpeed: 3000,
        arrows: false,
    });
    $(".list-logo").slick({
        dots: false,
        infinite: true,
        slidesToShow: 6,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: true,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><i class="fal fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fal fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 3,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                }
            }
        ]
    });
    $('.filter-item').click(function () {
        $('.filter-item').removeClass('active');
        $(this).addClass('active');
        var categoryId = $(this).data('id');
        $.post("/Home/GetProductHome", { catId: categoryId }, function (data) {
            $('#filter-home').empty();
            $('#filter-home').html(data);
        });
    });
    $('.list-feedback').slick({
        dots: true,
        infinite: true,
        speed: 300,
        slidesToShow: 3,
        arrows: false,
        speed: 1500,
        autoplay: true,
        autoplaySpeed: 3000,
        slidesToScroll: 1,
        prevArrow: '<button class="slick-prev"><i class="fal fa-chevron-left"></i></button>',
        nextArrow: '<button class="slick-next"><i class="fal fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: false,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2,
                    dots: false,

                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    dots: false,

                }
            }
        ]
    });
}
function productDetail() {
    $('.slider-for').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        fade: true,
        asNavFor: '.slider-nav',
        arrows: false,
    });
    $('.slider-nav').slick({
        slidesToShow: 5,
        slidesToScroll: 1,
        asNavFor: '.slider-for',
        dots: false,
        centerMode: false,
        focusOnSelect: true,
        prevArrow: '<button class="chevron-prev"><i class="fal fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fal fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 830,
                settings: {
                    slidesToShow: 5,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 4,
                }
            },
        ]
    });

    $(".nice-number").niceNumber();

    $("#formBookProduct").on("submit", function (e) {
        e.preventDefault();
        if ($("#formBookProduct").valid()) {
            $.post("/gio-hang/them-vao-gio-hang", $(this).serialize(), function (data) {
                if (data.result === 1) {
                    $.toast({
                        text: "Thêm vào giỏ hàng thành công",
                        icon: "success",
                        position: "bottom-right"
                    });
                    $(".itemshop").text(data.count);
                } else {
                    $.toast("Quá trình thực hiện không thành công");
                }
            });
        }
    });

    $(".btn-buy").on("click", function () {
        const proId = $(this).parents("#formBookProduct").find("input[name=ProductId]").val();
        const quantity = $(this).parents("#formBookProduct").find("input[name=quantity]").val();

        $.post("/gio-hang/them-vao-gio-hang", { quantity: quantity, productId: proId }, function (data) {
            if (data.result === 1) {
                alert("Thêm vào giỏ hàng thành công");
                $(".itemshop").text(data.count);
                window.location.href = "/gio-hang/thong-tin";
            } else {
                Toast('success', '#2abfa5', data.msg)
            }
        });
        return false;
    });
}
function articleDetil() {
    $(".related-list").slick({
        dots: false,
        infinite: true,
        slidesToShow: 3,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><i class="far fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="far fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1
                }
            }
        ]
    });
}

function CartJS() {
    $("[data-item=city]").on("change", function (data) {
        const id = $(this).val();
        var items = [];
        items.push("<option value>Chọn quận huyện</option>");

        if (id !== "") {
            $.getJSON("/Base/GetDistrict", { cityId: id }, function (data) {
                $.each(data, function (key, val) {
                    items.push("<option value='" + val.Id + "'>" + val.Name + "</option>");
                });
                $("[data-item=district]").html(items.join(""));
            });
            $.getJSON("/Base/GetShipFee", { cityId: id }, function (data) {
                const ct = $("#cartTotal").val();
                if (data.shipFee !== "null") {
                    const sf = data.shipFee.toLocaleString();
                    $(".ship-fee").text(sf + " đ");
                    const rct = ct.replace(/[^0-9]+/g, "");
                    const final = parseInt(data.shipFee) + parseInt(rct);
                    const rf = final.toLocaleString();
                    $(".total-price").text(rf + " đ");
                }
                else {
                    $(".ship-fee").text("0 đ");
                    $(".total-price").text(ct + " đ");
                }
            });
        } else {
            $("[data-item=district]").html(items.join(""));
        }
    });
    $(".remove-product").click(function () {
        const recordToDelete = $(this).attr("data-id");
        if (recordToDelete !== "") {
            Swal.fire({
                title: 'Bạn có muốn xóa sản phẩm?',
                text: "Sản phẩm sẽ bị xóa khỏi giỏ hàng",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Xóa'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.post("/ShoppingCart/RemoveFromCart", { "id": recordToDelete }, function (data) {
                        if (data.ItemCount === 1) {
                            alert("Quá trình thực hiện không thành công");

                        } else {
                            $("tr[data-cart='" + recordToDelete + "']").fadeOut();
                            function reload() {
                                location.reload();
                            }
                            setTimeout(reload, 500);
                            Swal.fire(
                                'Xóa thành công!',
                                'Sản phẩm đã bị xóa khỏi giỏ hàng',
                                'success'
                            )
                        }
                    });


                }
            })

        }
    });
    $('input[name="Order.TypePay"]').change(function () {
        $('.qr-code').slideUp();
        if ($(this).val() === '2') {
            $('.qr-code').slideDown();
        }
    });
    $('input[name="Order.TypePay"]:checked').change();
}
window.addEventListener("scroll", function () {
    if ($(this).scrollTop() > 40) {
        $(".backtop").fadeIn(200);
    } else {
        $(".backtop").fadeOut(70);
    }
});


function alertBox() {
    $("#AlertBox").fadeOut(1000);
}


function addToCart(n) {
    $.post("/gio-hang/them-vao-gio-hang", { productId: n}, function (data) {
        if (data.result === 1) {
            $.toast({
                text: "Thêm vào giỏ hàng thành công",
                icon: "success",
                position: "bottom-right"
            });
            $(".itemshop").text(data.count);
        } else {
            $.toast("Quá trình thực hiện không thành công");
        }
    });

} $("#form-contact").on("submit", function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        $.post("/Home/Contact", $(this).serialize(), function (data) {
            if (data.status) {
                $.toast({
                    heading: 'Liên hệ thành công',
                    text: data.msg,
                    icon: 'success'
                });
                $("#form-contact").trigger("reset");
            } else {
                $.toast({
                    heading: 'Liên hệ không thành công',
                    text: data.msg,
                    icon: 'error'
                });
            }
        });
    }
});
$("#formSubscribe").on("submit", function (e) {
    e.preventDefault();
    $.post("/Home/SubcribeForm", $(this).serialize(), function (data) {
        $("#formSubscribe input").val("");
        if (data.status) {
            $.toast({
                heading: "Liên hệ thành công",
                text: data.msg,
                icon: "success"
            });
            $("#formSubscribe").trigger("reset");
        } else {
            $.toast({
                heading: "Liên hệ không thành công",
                text: data.msg,
                icon: "error"
            });
        }
    });
});
function UpdateToCard(id, changeValue) {
    $.ajax({
        type: "Post",
        url: "/ShoppingCart/UpdateCartV2", data: { productId: id, changeValue},
        success: function (res) {
            if (res) {
                $("[data-cart-item=" + id + "]").text(res.total + "đ");
                $(".total-price").html(res.totalMoneyString + "đ");
                $.toast({
                    heading: res.Msg,
                    position: "bottom-right",
                    icon: "success"
                });
            } else {
                $.toast({
                    heading: res.Msg,
                    position: "bottom-right",
                    icon: "error"
                });
            }
        }
    })
}
$(".backtop").click(function () {
    $("html, body").animate({ scrollTop: 0 }, "slow");
});
$(".hamburger").click(function () {
    $(this).toggleClass("active");
    $(".menu-mb").toggleClass("active");
    $(".overlay").toggleClass("active");
});
$(".expand-bar").click(function () {
    $(this).toggleClass('open');
    $(this).siblings('.sub-nav-mb').slideToggle();
});
$(".overlay").click(function () {
    $(this).removeClass("active");
    $(".menu-mb").removeClass("active");
    $(".hamburger").removeClass("active");
});
function openSearch() {

}
$(".search").click(function () {
    $(".search-box").addClass("active")
})
$(".remove-box").click(function () {
    $(".search-box").removeClass("active")
})