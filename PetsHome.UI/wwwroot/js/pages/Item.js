
var Item = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'itm_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'itm_Codigo',
                    Size: 100,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-family:monospace;font-weight:600;letter-spacing:0.5px;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'itm_Descripcion',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<div style="display:flex;align-items:center;gap:8px;">'
                                + '<span style="background:#ede9fe;border-radius:6px;padding:4px 7px;">'
                                + '<i class="fas fa-box" style="color:#7c3aed;font-size:13px;"></i></span>'
                                + '<span style="font-weight:500;color:#111827;">' + data + '</span>'
                                + '</div>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cat_Descripcion',
                    Size: 140,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'itm_Precio',
                    Size: 100,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (data === null || data === undefined || data === '') return '<span style="color:#9ca3af;">—</span>';
                            var val = parseFloat(data);
                            if (isNaN(val)) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-weight:500;color:#111827;">$' + val.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'itm_StockActual',
                    Size: 90,
                    render: function (data, type) {
                        if (type !== 'display') return data;
                        return '<span style="font-weight:600;color:#111827;">' + (data || 0) + '</span>';
                    }
                },
                {
                    FieldName: 'itm_StockMinimo',
                    Size: 90,
                    render: function (data, type) {
                        if (type !== 'display') return data;
                        return '<span style="color:#6b7280;">' + (data || 0) + '</span>';
                    }
                },
                {
                    FieldName: 'itm_StockActual',
                    Size: 100,
                    render: function (data, type, row) {
                        if (type !== 'display') return data;
                        var stock  = parseFloat(row.itm_StockActual  || 0);
                        var minimo = parseFloat(row.itm_StockMinimo || 0);
                        var label, bg, color;
                        if (stock === 0)          { label = 'Crítico'; bg = '#fee2e2'; color = '#b91c1c'; }
                        else if (stock <= minimo)  { label = 'Bajo';    bg = '#fef3c7'; color = '#b45309'; }
                        else                      { label = 'OK';      bg = '#dcfce7'; color = '#15803d'; }
                        return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;'
                             + 'font-size:12px;font-weight:600;background:' + bg + ';color:' + color + ';">'
                             + label + '</span>';
                    }
                }
            ];
            datatable.init(Direction, header);
        })
    }

    obj.initPorVencer = function (urlPorVencer) {
        if ($.fn.DataTable.isDataTable('#datatable-vencer')) return;
        $('#datatable-vencer').DataTable({
            ajax: { url: urlPorVencer, dataSrc: 'data' },
            columns: [
                { data: 'itm_Codigo',              title: 'Código',    width: '90px' },
                { data: 'itm_Descripcion',          title: 'Item' },
                { data: 'cat_Descripcion',          title: 'Categoría', width: '130px' },
                { data: 'recdet_NumeroLote',        title: 'Lote',      width: '100px',
                  render: function (d) { return d || '<span style="color:#9ca3af;">—</span>'; } },
                { data: 'recdet_Cantidad',          title: 'Cant.',     width: '70px', className: 'text-center' },
                { data: 'recdet_FechaVencimiento',  title: 'Vence',     width: '110px',
                  render: function (d, t) {
                      if (t !== 'display') return d;
                      return d ? new Date(d).toLocaleDateString('es-HN') : '—';
                  }
                },
                { data: 'DiasRestantes', title: 'Días', width: '80px', className: 'text-center',
                  render: function (d, t) {
                      if (t !== 'display') return d;
                      var bg = d < 7 ? '#fee2e2' : '#fef3c7';
                      var co = d < 7 ? '#b91c1c' : '#b45309';
                      return '<span style="display:inline-block;padding:2px 8px;border-radius:9999px;'
                           + 'font-size:12px;font-weight:600;background:' + bg + ';color:' + co + ';">'
                           + d + ' días</span>';
                  }
                }
            ],
            order: [[5, 'asc']],
            language: {
                zeroRecords: 'Sin items próximos a vencer',
                emptyTable: 'Sin items próximos a vencer',
                processing: 'Cargando...',
                info: 'Mostrando _START_ de _TOTAL_ registros',
                paginate: { next: 'Siguiente', previous: 'Anterior' }
            },
            responsive: true
        });
    };

    obj.initValidation = function (validarUrl) {
        if (!$.validator.methods.noSpaceAtStart) {
            $.validator.addMethod("noSpaceAtStart", function (value) {
                return value.length === 0 || value[0] !== ' ';
            }, "No puede comenzar con espacios.");
        }
        $(function () {
            var $form = $("form");
            $form.validate({
                rules: {
                    itm_Codigo: {
                        required: true,
                        noSpaceAtStart: true,
                        remote: {
                            url: validarUrl,
                            type: "GET",
                            data: {
                                itm_Codigo: function () { return $("#itm_Codigo").val(); },
                                itm_Id: function () { return $("#itm_Id").val(); }
                            }
                        }
                    }
                },
                messages: {
                    itm_Codigo: {
                        required: "El código es requerido.",
                        remote: "Ya existe un item con ese código."
                    }
                },
                errorElement: "span",
                errorClass: "text-danger",
                highlight: function (el) { $(el).addClass("is-invalid"); },
                unhighlight: function (el) { $(el).removeClass("is-invalid"); }
            });
        });
    };

    return obj;

}());
