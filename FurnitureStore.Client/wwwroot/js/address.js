function getProvinceListAJAX() {
    $.ajax({
        url: "https://provinces.open-api.vn/api/p/",
        type: "get",
        dataType: "json",
        success: function (res) {
            handleProvinceData(res);
        },
    });
}
function handleProvinceData(data) {
    // Xử lý dữ liệu trả về từ API ở đây
    // load danh sách tỉnh lên thẻ select
    var selectElement = document.getElementById("city-select");

    data.forEach(function (item) {
        var option = document.createElement("option");
        option.value = item.code; 
        option.text = item.name;
        selectElement.appendChild(option);
    });
}
function getDistrictListByProvinceCodeAJAX(code) {
   
        $.ajax({
            url: "https://provinces.open-api.vn/api/p/" + code + "?depth=2",
            type: "get",
            dataType: "json",
            success: function (res) {
                handleDistrictData(res);
            },
            error: function (xhr, status, error) {
                handleDistrictError();
            }
        });
    
}
function initiateDistrictData() {
    var selectElement = document.getElementById("district-select");
    selectElement.innerHTML = "";
    // tạo option chọn khu vực
    var option = document.createElement("option");
    option.value = "-1";
    option.text = "Chọn khu vực";
    selectElement.appendChild(option);
    return selectElement;
}
function handleDistrictError() {
    initiateDistrictData();
}

function handleDistrictData(data) {
    var selectElement = initiateDistrictData();
    if (data != null) {
        data.districts.forEach(function (item) {
            var option = document.createElement("option");
            option.value = item.code;
            option.text = item.name;
            selectElement.appendChild(option);
        });
    }
}
function getWardListByDistrictCodeAJAX(code) {

    $.ajax({
        url: "https://provinces.open-api.vn/api/d/" + code + "?depth=2",
        type: "get",
        dataType: "json",
        success: function (res) {
            handleWardsData(res);
        },
        error: function (xhr, status, error) {
            handleWardError();
        }
       
    });

}
function handleWardError() {
    initiateWardData();
}
function initiateWardData() {
    var selectElement = document.getElementById("ward-select");
    selectElement.innerHTML = "";
    // tạo option chọn khu vực
    var option = document.createElement("option");
    option.value = "-1";
    option.text = "Chọn khu vực";
    selectElement.appendChild(option);
    return selectElement;
}
function handleWardsData(data) {
    var selectElement = initiateWardData();

    data.wards.forEach(function (item) {
        var option = document.createElement("option");
        option.value = item.name;
        option.text = item.name;
        selectElement.appendChild(option);
    });
}
