const animItems = document.querySelectorAll('._anim-items');
if (animItems.length > 0) {
    window.addEventListener('scroll', animOnScroll);
    function animOnScroll() {
        for (let index = 0; index < animItems.length; index++) {
            const animItem = animItems[index];
            const animItemHeight = animItem.offsetHeight;
            const animItemOffSet = offset(animItem).top;
            const animStart = 4;

            let animItemPoint = window.innerHeight - animItemHeight / animStart;

            if (animItemHeight > window.innerHeight) {
                animItemPoint = window.innerHeight - window.innerHeight / animStart;

            }

            if ((pageYOffset > animItemOffSet - animItemPoint) && pageYOffset < (animItemOffSet + animItemHeight)) {
                animItem.classList.add('_active');
            }
            else {
                if (!animItem.classList.contains('_anim-no-hide')) {
                    animItem.classList.remove('_active');
                }
            }
        }
    }

    function offset(el) {
        const rect = el.getBoundingClientRect(),
            scrollLeft = window.pageXOffset || document.documentElement.scrollLeft,
            scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        return { top: rect.top + scrollTop, left: rect.left + scrollLeft }
    }

    setTimeout(() => {
        animOnScroll();
    }, 300);
}

const animItemsCategory = document.querySelectorAll('._anim-items-category');
if (animItemsCategory.length > 0) {
    window.addEventListener('scroll', animOnScrollCategory);
    function animOnScrollCategory() {
        for (let index = 0; index < animItemsCategory.length; index++) {
            const animItemCategory = animItemsCategory[index];
            const animItemCategoryHeight = animItemCategory.offsetHeight;
            const animItemCategoryOffSet = offset(animItemCategory).top;
            const animStart = 10;

            let animItemCategoryPoint = window.innerHeight - animItemCategoryHeight / animStart;

            if (animItemCategoryHeight > window.innerHeight) {
                animItemCategoryPoint = window.innerHeight - window.innerHeight / animStart;

            }

            if ((pageYOffset > animItemCategoryOffSet - animItemCategoryPoint) && pageYOffset < (animItemCategoryOffSet + animItemCategoryHeight)) {
                animItemCategory.classList.add('_active');
            }
            else {
                if (!animItemCategory.classList.contains('_anim-no-hide')) {
                    animItemCategory.classList.remove('_active');
                }
            }
        }
    }

    function offset(el) {
        const rect = el.getBoundingClientRect(),
            scrollLeft = window.pageXOffset || document.documentElement.scrollLeft,
            scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        return { top: rect.top + scrollTop, left: rect.left + scrollLeft }
    }

    setTimeout(() => {
        animOnScrollCategory();
    }, 300);
}

const animItemsOnload = document.querySelectorAll('._anim-load-window-items');
if (animItemsOnload.length > 0) {
    window.addEventListener('load', animOnload);
    function animOnload() {
        for (let index = 0; index < animItemsOnload .length; index++) {
            const animItemOnload = animItemsOnload[index];
            animItemOnload .classList.add('_active');
        }
    }
    setTimeout(() => {
        animOnload();
    }, 300);
}