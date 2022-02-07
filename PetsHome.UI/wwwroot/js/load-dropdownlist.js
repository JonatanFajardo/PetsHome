

//$('#load-dropdown-sexo').on(function, onclick)






var dropdown = (function () {
    var obj = {};

    function addOptions(domElement, array) {
        var select = document.getElementsByName(domElement)[0];

        for (value in array) {
            var option = document.createElement("option");
            option.text = array[value];
            select.add(option);
        }
    }

    function sexo() {
        var array = ["Masculino", "Femenino"];
        array.sort();
        addOptions("load-dropdown-sexo", array);
    }


    obj.load = function () {
        sexo();

    }

    return obj;
}());



$(function () {
    dropdown.load();
})