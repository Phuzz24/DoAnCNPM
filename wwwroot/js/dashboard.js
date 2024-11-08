document.addEventListener('DOMContentLoaded', () => {
    const toggleBtn = document.querySelector('.toggle-btn');
    const sidebar = document.querySelector('.sidebar');

    toggleBtn.addEventListener('click', () => {
        sidebar.classList.toggle('closed');
    });

    const userInfo = document.querySelector('.user-info');
    const dropdownMenu = document.querySelector('.dropdown-menu');

    userInfo.addEventListener('mouseover', () => {
        dropdownMenu.style.display = 'block';
    });

    userInfo.addEventListener('mouseout', () => {
        dropdownMenu.style.display = 'none';
    });
});
