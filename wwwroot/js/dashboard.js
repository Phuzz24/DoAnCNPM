document.addEventListener('DOMContentLoaded', () => {
    const toggleBtn = document.querySelector('.toggle-btn');
    const sidebar = document.querySelector('.sidebar');

    if (toggleBtn && sidebar) { // Kiểm tra nếu cả hai phần tử tồn tại
        toggleBtn.addEventListener('click', () => {
            sidebar.classList.toggle('closed');
        });
    }

    const userInfo = document.querySelector('.user-info');
    const dropdownMenu = document.querySelector('.dropdown-menu');

    if (userInfo && dropdownMenu) { // Kiểm tra nếu cả hai phần tử tồn tại
        userInfo.addEventListener('mouseover', () => {
            dropdownMenu.style.display = 'block';
        });

        userInfo.addEventListener('mouseout', () => {
            dropdownMenu.style.display = 'none';
        });
    }
});
