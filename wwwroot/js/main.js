document.addEventListener("DOMContentLoaded", function () {
    // JavaScript cho Slider Chữ
    let currentTextSlideIndex = 0;
    const textSlides = document.querySelectorAll('.text-slide');
    const textDots = document.querySelectorAll('.dot');
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

    // Gọi slide đầu tiên khi trang tải
    showImageSlide(currentImageSlideIndex);

    // Nút điều hướng cho banner ảnh
    document.querySelector('.prev').addEventListener('click', () => moveImageSlide(-1));
    document.querySelector('.next').addEventListener('click', () => moveImageSlide(1));

    // Auto chuyển slide ảnh mỗi 5 giây
    setInterval(() => {
        moveImageSlide(1);
    }, 5000);
});

//slide iPhone
const prevBtn = document.querySelector('.prev-btn');
const nextBtn = document.querySelector('.next-btn');
const productList = document.querySelector('.product-list');
const productListWrapper = document.querySelector('.product-list-wrapper');

let scrollAmount = 0; // Lượng đã cuộn
const productItemWidth = document.querySelector('.product-item').offsetWidth + 30; // Chiều rộng của sản phẩm (cộng với margin)
const maxScroll = productList.scrollWidth - productListWrapper.clientWidth; // Độ dài có thể cuộn tối đa

// Nút "next"
nextBtn.addEventListener('click', () => {
    if (scrollAmount < maxScroll) {
        scrollAmount += productItemWidth * 4; // Cuộn theo nhóm 4 sản phẩm
        if (scrollAmount > maxScroll) {
            scrollAmount = maxScroll; // Đảm bảo không cuộn quá xa
        }
    }
    productListWrapper.scrollTo({
        top: 0,
        left: scrollAmount,
        behavior: 'smooth' // Hiệu ứng cuộn mượt mà
    });
});

// Nút "prev"
prevBtn.addEventListener('click', () => {
    if (scrollAmount > 0) {
        scrollAmount -= productItemWidth * 4; // Cuộn lại nhóm 4 sản phẩm
        if (scrollAmount < 0) {
            scrollAmount = 0; // Đảm bảo không cuộn quá xa về bên trái
        }
    }
    productListWrapper.scrollTo({
        top: 0,
        left: scrollAmount,
        behavior: 'smooth' // Hiệu ứng cuộn mượt mà
    });
});



function showThankYouMessage() {
    // Hiển thị thông báo "Cảm ơn phản hồi của bạn"
    document.getElementById('thankYouMessage').style.display = 'block';

    // Ẩn phần câu hỏi phản hồi
    document.querySelector('.feedback-box').style.display = 'none';
}

