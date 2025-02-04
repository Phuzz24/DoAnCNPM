@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
document.addEventListener("DOMContentLoaded", function () {
    const formGroups = document.querySelectorAll('.form-group');
    let delay = 0;

    // Thêm hiệu ứng trượt vào cho các form-group với độ trễ tăng dần
    formGroups.forEach(group => {
        group.style.animationDelay = `${delay}s`;
        delay += 0.1;
    });

    // Animation trên nút submit khi hover
    const submitButton = document.querySelector('button[type="submit"]');
    submitButton.addEventListener('mouseenter', () => {
        submitButton.style.transform = 'scale(1.05)';
    });

    submitButton.addEventListener('mouseleave', () => {
        submitButton.style.transform = 'scale(1)';
    });

    // Animation trên nút secondary khi hover
    const secondaryButton = document.querySelector('.btn-secondary');
    secondaryButton.addEventListener('mouseenter', () => {
        secondaryButton.style.transform = 'scale(1.05)';
    });

    secondaryButton.addEventListener('mouseleave', () => {
        secondaryButton.style.transform = 'scale(1)';
    });

    document.addEventListener("DOMContentLoaded", function () {
        // Tính năng xem trước hình ảnh khi người dùng chọn ảnh
        document.getElementById("ImageFile").addEventListener("change", function (event) {
            const file = event.target.files[0];
            const preview = document.getElementById("image-preview");

            if (file) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    preview.src = e.target.result;
                    preview.style.display = "block"; // Hiển thị ảnh khi có ảnh được chọn
                };
                reader.readAsDataURL(file);
            } else {
                preview.style.display = "none"; // Ẩn ảnh nếu không có file được chọn
                preview.src = "";
            }
        });
    });