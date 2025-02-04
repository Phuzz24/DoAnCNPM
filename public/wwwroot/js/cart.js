document.addEventListener("DOMContentLoaded", function () {
    // Lắng nghe sự kiện submit trên form cập nhật số lượng
    const quantityForms = document.querySelectorAll(".quantity-form");
    quantityForms.forEach(form => {
        form.addEventListener("submit", function (event) {
            event.preventDefault();

            const productId = form.querySelector("input[name='productId']").value;
            const quantity = form.querySelector("input[name='quantity']").value;

            updateQuantity(productId, quantity);
        });
    });

    // Hàm cập nhật số lượng sản phẩm trong giỏ hàng bằng Fetch API
    async function updateQuantity(productId, quantity) {
        try {
            const response = await fetch("/Cart/UpdateQuantity", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",  // Đảm bảo rằng Content-Type là application/json
                },
                body: JSON.stringify({ productId: parseInt(productId), quantity: parseInt(quantity) }) // Gửi dữ liệu dưới dạng JSON
            });

            const data = await response.json();
            if (data.success) {
                // Hiển thị thông báo thành công và làm mới trang
                Swal.fire("Thành công", data.message, "success").then(() => {
                    window.location.reload();
                });
            } else {
                Swal.fire("Lỗi", data.message, "error");
            }
        } catch (error) {
            console.error("Error updating quantity:", error);
            Swal.fire("Lỗi", "Đã xảy ra lỗi khi cập nhật số lượng.", "error");
        }
    }
});


    // Xóa sản phẩm khỏi giỏ hàng (AJAX)
    async function removeFromCart(productId) {
        try {
            const response = await fetch("/Cart/RemoveFromCart", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(productId) // Gửi productId dưới dạng JSON
            });

            const data = await response.json();
            if (data.success) {
                // Xóa thành công - làm mới trang hoặc cập nhật giao diện
                Swal.fire("Thành công", data.message, "success").then(() => {
                    window.location.reload();
                });
            } else {
                Swal.fire("Lỗi", data.message, "error");
            }
        } catch (error) {
            console.error("Error removing from cart:", error);
            Swal.fire("Lỗi", "Đã xảy ra lỗi khi xóa sản phẩm khỏi giỏ hàng.", "error");
        }
    }


    // Hiển thị thông báo từ server
    const successMessage = document.querySelector(".alert-success");
    const errorMessage = document.querySelector(".alert-danger");
    if (successMessage) {
        Swal.fire("Thành công", successMessage.textContent, "success");
        setTimeout(() => successMessage.style.display = "none", 3000);
    }
    if (errorMessage) {
        Swal.fire("Lỗi", errorMessage.textContent, "error");
        setTimeout(() => errorMessage.style.display = "none", 3000);
    }
});
