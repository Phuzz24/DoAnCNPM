document.addEventListener("DOMContentLoaded", function () {
    // JavaScript cho Slider Chữ
    let currentTextSlideIndex = 0;
    const textSlides = document.querySelectorAll('.text-slide');
    const textDots = document.querySelectorAll('.dot');
    const fadeElements = document.querySelectorAll('.fade-in');

    const totalTextSlides = textSlides.length;

    function showTextSlide(index) {
        // Ẩn tất cả các slide chữ
        textSlides.forEach(slide => slide.style.display = 'none');
        textDots.forEach(dot => dot.classList.remove('active'));

        // Hiển thị slide chữ tương ứng
        textSlides[index].style.display = 'flex';
        textDots[index].classList.add('active');
    }


    function moveTextSlide(step) {
        currentTextSlideIndex = (currentTextSlideIndex + step + totalTextSlides) % totalTextSlides;
        showTextSlide(currentTextSlideIndex);
    }

    // Kiểm tra nếu phần tử slide và dot tồn tại
    if (textSlides.length && textDots.length) {
        // Điều khiển slide chữ bằng dot
        textDots.forEach((dot, index) => {
            dot.addEventListener('click', () => {
                currentTextSlideIndex = index;
                showTextSlide(currentTextSlideIndex);
            });
        });

        // Gọi slide đầu tiên khi trang tải
        showTextSlide(currentTextSlideIndex);

        // Tự động chuyển slide chữ sau 3 giây
        setInterval(() => {
            moveTextSlide(1);
        }, 3000);
    }

    // JavaScript cho Slider Ảnh
    let currentImageSlideIndex = 0;
    const imageSlides = document.querySelectorAll('.image-slide');
    const totalImageSlides = imageSlides.length;

    function showImageSlide(index) {
        // Ẩn tất cả các slide ảnh
        imageSlides.forEach(slide => slide.style.display = 'none');

        // Hiển thị slide ảnh tương ứng
        imageSlides[index].style.display = 'block';
    }

    function moveImageSlide(step) {
        currentImageSlideIndex = (currentImageSlideIndex + step + totalImageSlides) % totalImageSlides;
        showImageSlide(currentImageSlideIndex);
    }

    // Kiểm tra nếu slide ảnh tồn tại
    if (imageSlides.length) {
        // Gọi slide đầu tiên khi trang tải
        showImageSlide(currentImageSlideIndex);

        // Kiểm tra và thêm sự kiện cho nút điều hướng
        const prevButton = document.querySelector('.prev');
        const nextButton = document.querySelector('.next');
        if (prevButton && nextButton) {
            prevButton.addEventListener('click', () => moveImageSlide(-1));
            nextButton.addEventListener('click', () => moveImageSlide(1));
        }

        // Auto chuyển slide ảnh mỗi 5 giây
        setInterval(() => {
            moveImageSlide(1);
        }, 5000);
    }
});

document.addEventListener("DOMContentLoaded", function () {
    // Lấy phần tử product-list và các sản phẩm bên trong
    const productList = document.querySelector('.product-list');
    const productItems = document.querySelectorAll('.product-item');

    // Kiểm tra sự tồn tại của productList và productItems
    if (productList && productItems.length > 0) {
        let scrollAmount = 0;
        const productItemWidth = productItems[0].offsetWidth + 20; // Độ rộng sản phẩm + khoảng cách
        const maxScroll = productList.scrollWidth - productList.clientWidth;

        function nextPage() {
            if (scrollAmount < maxScroll) {
                scrollAmount += productItemWidth * 4; // Cuộn theo 4 sản phẩm mỗi lần
                if (scrollAmount > maxScroll) scrollAmount = maxScroll; // Đảm bảo không cuộn quá phải
                productList.scrollTo({
                    top: 0,
                    left: scrollAmount,
                    behavior: 'smooth' // Hiệu ứng cuộn mượt mà
                });
            }
        }

        function prevPage() {
            if (scrollAmount > 0) {
                scrollAmount -= productItemWidth * 4; // Cuộn lại 4 sản phẩm
                if (scrollAmount < 0) scrollAmount = 0; // Đảm bảo không cuộn quá trái
                productList.scrollTo({
                    top: 0,
                    left: scrollAmount,
                    behavior: 'smooth' // Hiệu ứng cuộn mượt mà
                });
            }
        }

        // Gán sự kiện cho các nút next và prev
        const nextButton = document.querySelector('.next-btn');
        const prevButton = document.querySelector('.prev-btn');
        if (nextButton && prevButton) {
            nextButton.addEventListener('click', nextPage);
            prevButton.addEventListener('click', prevPage);
        }
    } else {
        console.warn("Không tìm thấy phần tử product-list hoặc product-item.");
    }
});





function showThankYouMessage() {
    // Hiển thị thông báo "Cảm ơn phản hồi của bạn"
    const thankYouMessage = document.getElementById('thankYouMessage');
    const feedbackBox = document.querySelector('.feedback-box');

    if (thankYouMessage && feedbackBox) {
        thankYouMessage.style.display = 'block';
        feedbackBox.style.display = 'none';
    }
}

// Hiển thị nút khi người dùng cuộn xuống 100px và thêm hiệu ứng nhấp nháy
window.onscroll = function () { scrollFunction() };

function scrollFunction() {
    const backToTopBtn = document.getElementById("backToTopBtn");
    if (document.body.scrollTop > 100 || document.documentElement.scrollTop > 100) {
        backToTopBtn.style.display = "block";
        backToTopBtn.classList.add("active"); // Thêm lớp nhấp nháy
    } else {
        backToTopBtn.style.display = "none";
        backToTopBtn.classList.remove("active"); // Loại bỏ lớp nhấp nháy khi nút ẩn đi
    }
}

// Hàm để cuộn về đầu trang khi nhấn nút
function scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
}
