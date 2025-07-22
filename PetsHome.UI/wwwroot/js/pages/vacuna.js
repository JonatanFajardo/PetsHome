var Vacuna = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
 
            var header = [
                { FieldName: "vac_Id", Size: "200px" },
                { FieldName: "vac_Descripcion" }
            ];


            // Verificar que la URL sea válida
            if (!Direction.listUrl) {
                console.error('ERROR: listUrl no está definida');
                return;
            }

            // Inicializar el DataTable
        datatableCatalogs.init(Direction.listUrl, header);
        
    };

    return obj;
}());