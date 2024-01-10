function storeIdProductIntoCookie(id,quantity) {
    // check if cart has items?
    var myShoppingCart = JSON.parse(getCookie("shopping_Cart"));
    
    if (myShoppingCart.length == 0) {
        // if not, create new cart
        var myArray = {
            [id]: [quantity]
        }
        setCookie("shopping_Cart", myArray);
    }
    else {
        myShoppingCart[id] = quantity;
        setCookie("shopping_Cart", myShoppingCart);
    }
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
