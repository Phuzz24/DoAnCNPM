function searchByName() {
    let input = document.getElementById('search').value.toLowerCase();
    let items = document.getElementsByClassName('product-item');

    Array.from(items).forEach(item => {
        let name = item.querySelector('h3').textContent.toLowerCase();
        item.style.display = name.includes(input) ? 'block' : 'none';
    });
}

function filterProducts() {
    let brandFilter = document.getElementById('brandFilter').value;
    let categoryFilter = document.getElementById('categoryFilter').value;
    let priceFilter = document.getElementById('priceFilter').value;
    let items = document.getElementsByClassName('product-item');

    Array.from(items).forEach(item => {
        let brand = item.getAttribute('data-brand');
        let category = item.getAttribute('data-category');
        let price = parseFloat(item.getAttribute('data-price'));

        let [minPrice, maxPrice] = priceFilter.split('-').map(Number);

        let matchesBrand = !brandFilter || brand === brandFilter;
        let matchesCategory = !categoryFilter || category === categoryFilter;
        let matchesPrice = !priceFilter ||
            (price >= minPrice && (!maxPrice || price <= maxPrice));

        item.style.display = matchesBrand && matchesCategory && matchesPrice ? 'block' : 'none';
    });
}
const url = `/Product/SanPham?searchTerm=${searchTerm}&brandId=${brandFilter}&categoryId=${categoryFilter}&priceRange=${priceFilter}`;
window.location.href = url;

function addToCart(productId) {
    alert('Đã thêm sản phẩm vào giỏ hàng: ' + productId);
}
