
  lucide.createIcons();

  function updateClock() {
    const now = new Date();
    const h = String(now.getHours()).padStart(2,'0');
    const m = String(now.getMinutes()).padStart(2,'0');
    const s = String(now.getSeconds()).padStart(2,'0');
    document.getElementById('clock').textContent = h+':'+m+':'+s;

    const dias = ['Domingo','Lunes','Martes','Miércoles','Jueves','Viernes','Sábado'];
    const meses = ['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre'];
    document.getElementById('date-display').textContent =
      dias[now.getDay()]+', '+now.getDate()+' de '+meses[now.getMonth()]+' '+now.getFullYear();
  }
  updateClock();
  setInterval(updateClock, 1000);

  // Animate KPI numbers counting up
  document.querySelectorAll('.kpi-number').forEach(el => {
    const target = parseInt(el.textContent);
    if (isNaN(target)) return;
    let start = 0; const dur = 1200; const step = 16;
    const inc = target / (dur / step);
    const timer = setInterval(() => {
      start = Math.min(start + inc, target);
      el.textContent = Math.round(start);
      if (start >= target) clearInterval(timer);
    }, step);
  });
