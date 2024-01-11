function storeIdProductIntoCookie(id,quantity) {
    // check if cart has items?
    var myShoppingCart = getCookie("shopping_Cart");
 
    if (myShoppingCart == "") {
        
        var myArray = {
            [id]: quantity,
        };
        setCookie("shopping_Cart", myArray);
    }
    else {
        myShoppingCart = JSON.parse(myShoppingCart);
        // kiểm tra xem idProduct đã tồn tại trong cart -> tăng số lượng
        // Sử dụng Object.keys()
        Object.keys(myShoppingCart).forEach(function (key) {
            if (id === key) {
                quantity = myShoppingCart[key] + quantity;
            }
        });
        myShoppingCart[id] = quantity;
        setCookie("shopping_Cart", myShoppingCart);
    }
}
function updateIdProductIntoCookie(id, quantity) {
    // check if cart has items?
    var myShoppingCart = getCookie("shopping_Cart");

  
        myShoppingCart = JSON.parse(myShoppingCart);
        myShoppingCart[id] = quantity;
        setCookie("shopping_Cart", myShoppingCart);
    
}
function removeIdProductFromCookie(id) {
    // check if cart has items?
    var myShoppingCart = getCookie("shopping_Cart");
    myShoppingCart = JSON.parse(myShoppingCart);

    Object.keys(myShoppingCart).forEach(function (key) {
        if (id === key) {
            delete myShoppingCart[key];
        }
    });

    setCookie("shopping_Cart", myShoppingCart);
}



function getProductsFromCookie() {
    return JSON.parse(getCookie("shopping_Cart"));

}

function setCookie(name, json) {
    let cookieValue = "";

    //Specify the cookie name and value
    cookieValue = name + "=" + JSON.stringify(json) + ";";

    //Set cookie
    document.cookie = cookieValue;
}
function getCookie(cookieName) {
    const cookies = document.cookie.split("; ");
    for (let i = 0; i < cookies.length; i++) {
        const cookie = cookies[i].split("=");
        if (cookie[0] === cookieName) {
            return cookie[1];
        }
    }
    return "";
}
