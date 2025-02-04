//Mở và đóng slidebar
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


document.addEventListener("DOMContentLoaded", () => {
    const notificationButton = document.getElementById("notificationButton");
    const notificationDropdown = document.getElementById("notificationDropdown");
    const notificationContent = document.getElementById("notificationContent");
    const notificationCount = document.getElementById("notificationCount");

    // Dữ liệu thông báo giả lập
    const notifications = [
        { id: 1, admin: "Admin A", activity: "Đã thêm sản phẩm iPhone 15", timestamp: "2024-11-14 09:00:00" },
        { id: 2, admin: "Admin B", activity: "Đã xóa đơn hàng #123", timestamp: "2024-11-14 08:30:00" },
        { id: 3, admin: "Admin C", activity: "Đã cập nhật thông tin sản phẩm Galaxy S23", timestamp: "2024-11-14 07:45:00" }
    ];

    // Hiển thị số lượng thông báo
    notificationCount.textContent = notifications.length > 0 ? notifications.length : "";

    // Tạo nội dung thông báo
    if (notifications.length === 0) {
        notificationContent.innerHTML = '<p class="notification-empty">Không có thông báo nào.</p>';
    } else {
        notifications.forEach(notification => {
            const item = document.createElement("div");
            item.className = "notification-item";
            item.innerHTML = `
                <p><strong>${notification.admin}</strong>: ${notification.activity}</p>
                <small>${new Date(notification.timestamp).toLocaleString()}</small>
            `;
            notificationContent.appendChild(item);
        });
    }

    // Hiển thị hoặc ẩn dropdown
    notificationButton.addEventListener("click", (event) => {
        event.stopPropagation();
        notificationDropdown.classList.toggle("active");
    });

    // Ẩn dropdown khi click bên ngoài
    document.addEventListener("click", () => {
        notificationDropdown.classList.remove("active");
    });
});


