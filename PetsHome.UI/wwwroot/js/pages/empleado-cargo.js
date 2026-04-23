header = [
    { FieldName: "cargo_Id", Size: 80, Visibility: false },
    {
        FieldName: "cargo_Descripcion",
        render: function (data) {
            return `<div style="display:flex;align-items:center;gap:10px;">
                      <div style="background:#ede9fe;border-radius:8px;padding:7px 9px;">
                        <i class="fas fa-briefcase" style="color:#7c3aed;font-size:13px;"></i>
                      </div>
                      <span>${data ?? ''}</span>
                    </div>`;
        }
    },
    {
        FieldName: "cargo_Salario",
        Size: 150,
        render: function (data, type) {
            if (type === 'display') {
                return `$${Number(data).toLocaleString('en-US')}`;
            }
            return data; // para sort/filter usa el número puro
        }
    },
    {
        FieldName: "cargo_EsActivo",
        Size: 140,
        render: function (data, type) {
            if (type === 'display') {
                var activo = data === true || data === 1 || data === "Activo";
                return activo
                    ? `<span class="status-badge status-activo">Activo</span>`
                    : `<span class="status-badge status-inactivo">Inactivo</span>`;
            }
            return data;
        }
    }
];