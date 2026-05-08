
/* ── DATA ──────────────────────────────────────────── */
const DATA = {
  mes: {
    bar: { labels:['Nov','Dic','Ene','Feb','Mar','Abr'], values:[28,35,22,41,38,48] },
    donut: { aprobadas:48, pendientes:14, revision:11, rechazadas:9 },
    kpi: { total:48, pending:14, rate:76, time:8 },
    breeds:[
      { emoji:'🐕', name:'Labrador Retriever', count:12 },
      { emoji:'🐈', name:'Gato Doméstico',     count:9 },
      { emoji:'🐩', name:'Poodle',              count:7 },
      { emoji:'🦮', name:'Golden Retriever',    count:6 },
      { emoji:'🐱', name:'Siamés',              count:5 },
    ],
    recent:[
      { pet:'Luna', breed:'Labrador', emoji:'🐕', adopter:'Ana García',      date:'26 abr', status:'aprobada',  days:6  },
      { pet:'Max',  breed:'Poodle',   emoji:'🐩', adopter:'Carlos Mejía',    date:'25 abr', status:'aprobada',  days:4  },
      { pet:'Mimi', breed:'Siamés',   emoji:'🐱', adopter:'Laura Sosa',      date:'24 abr', status:'pendiente', days:2  },
      { pet:'Toby', breed:'Golden',   emoji:'🦮', adopter:'Roberto Núñez',   date:'23 abr', status:'revision',  days:5  },
      { pet:'Cleo', breed:'Gato Dom.',emoji:'🐈', adopter:'María Fernández', date:'22 abr', status:'aprobada',  days:7  },
      { pet:'Rex',  breed:'Pastor A.',emoji:'🐕', adopter:'Josué Molina',    date:'20 abr', status:'rechazada', days:10 },
    ]
  },
  hoy: {
    bar: { labels:['Nov','Dic','Ene','Feb','Mar','Hoy'], values:[28,35,22,41,38,3] },
    donut: { aprobadas:2, pendientes:3, revision:1, rechazadas:0 },
    kpi: { total:3, pending:3, rate:67, time:6 },
    breeds:[
      { emoji:'🐕', name:'Labrador Retriever', count:1 },
      { emoji:'🐩', name:'Poodle',              count:1 },
      { emoji:'🐈', name:'Gato Doméstico',     count:1 },
    ],
    recent:[
      { pet:'Lola', breed:'Labrador', emoji:'🐕', adopter:'Ana García',   date:'Hoy', status:'aprobada',  days:6 },
      { pet:'Tom',  breed:'Poodle',   emoji:'🐩', adopter:'Carlos Mejía', date:'Hoy', status:'pendiente', days:1 },
      { pet:'Nina', breed:'Gato Dom.',emoji:'🐈', adopter:'Laura Sosa',   date:'Hoy', status:'revision',  days:3 },
    ]
  }
};

let currentPeriod = 'mes';
let barChartInst, donutChartInst;

/* ── INTERSECTION OBSERVER (appear on scroll) ──────── */
const io = new IntersectionObserver((entries) => {
  entries.forEach(e => {
    if (e.isIntersecting) {
      const el = e.target;
      const delay = parseInt(el.dataset.delay || 0);
      setTimeout(() => el.classList.add('visible'), delay);
      io.unobserve(el);
    }
  });
}, { threshold: 0.1 });

document.querySelectorAll('.kpi-card, .chart-card, .list-card').forEach(el => io.observe(el));

/* ── COUNT-UP ANIMATION ────────────────────────────── */
function animateCount(el, target, suffix='', duration=1400) {
  const start = performance.now();
  const update = (now) => {
    const p = Math.min((now - start) / duration, 1);
    const ease = 1 - Math.pow(1-p, 4);
    el.textContent = Math.round(ease * target) + suffix;
    if (p < 1) requestAnimationFrame(update);
    else el.textContent = target + suffix;
  };
  requestAnimationFrame(update);
}

function animateKPIs(data) {
  const kpis = [
    { id:'kpi-total',   val:data.total,   suf:'' },
    { id:'kpi-pending', val:data.pending, suf:'' },
    { id:'kpi-rate',    val:data.rate,    suf:'%' },
    { id:'kpi-time',    val:data.time,    suf:' d' },
  ];
  kpis.forEach(k => {
    const el = document.getElementById(k.id);
    if (el) animateCount(el, k.val, k.suf);
  });
}

/* ── BAR CHART ─────────────────────────────────────── */
function buildBarChart(d) {
  const ctx = document.getElementById('barChart').getContext('2d');
  if (barChartInst) barChartInst.destroy();
  barChartInst = new Chart(ctx, {
    type: 'bar',
    data: {
      labels: d.labels,
      datasets: [{
        label: 'Adopciones',
        data: d.values,
        backgroundColor: (ctx2) => {
          const g = ctx2.chart.ctx.createLinearGradient(0,0,0,260);
          g.addColorStop(0,'rgba(108,92,231,.85)');
          g.addColorStop(1,'rgba(108,92,231,.25)');
          return g;
        },
        borderRadius: 8,
        borderSkipped: false,
        hoverBackgroundColor: '#6C5CE7',
      }]
    },
    options: {
      responsive: true,
      plugins: {
        legend: { display: false },
        tooltip: {
          backgroundColor: '#2d3436',
          titleFont: { family:'Plus Jakarta Sans', weight:'700', size:12 },
          bodyFont:  { family:'Plus Jakarta Sans', size:12 },
          padding: 10,
          cornerRadius: 8,
          callbacks: {
            label: ctx2 => `  ${ctx2.parsed.y} adopciones`
          }
        }
      },
      scales: {
        x: { grid:{ display:false }, ticks:{ font:{ family:'Plus Jakarta Sans', size:12, weight:'600'}, color:'#8a92a6' } },
        y: { grid:{ color:'#e8eaf2', drawBorder:false }, ticks:{ font:{ family:'Plus Jakarta Sans', size:11 }, color:'#8a92a6' }, beginAtZero:true }
      },
      animation: { duration:900, easing:'easeOutQuart' }
    }
  });
}

/* ── DONUT CHART ───────────────────────────────────── */
function buildDonutChart(d) {
  const ctx = document.getElementById('donutChart').getContext('2d');
  if (donutChartInst) donutChartInst.destroy();
  const total = d.aprobadas + d.pendientes + d.revision + d.rechazadas;
  const vals  = [d.aprobadas, d.pendientes, d.revision, d.rechazadas];
  const cols  = ['#00b894','#e17055','#0984e3','#d63031'];
  const names = ['Aprobadas','Pendientes','En Revisión','Rechazadas'];

  donutChartInst = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: names,
      datasets: [{ data: vals, backgroundColor: cols, borderWidth:2, borderColor:'#fff', hoverOffset:6 }]
    },
    options: {
      cutout: '70%',
      plugins: {
        legend: { display:false },
        tooltip: {
          backgroundColor:'#2d3436',
          titleFont:{ family:'Plus Jakarta Sans', weight:'700', size:12 },
          bodyFont: { family:'Plus Jakarta Sans', size:12 },
          padding:10, cornerRadius:8,
          callbacks: { label: c => `  ${c.parsed} (${Math.round(c.parsed/total*100)}%)` }
        }
      },
      animation: { duration:900, easing:'easeOutQuart' }
    }
  });

  // Legend
  const leg = document.getElementById('donaLegend');
  leg.innerHTML = names.map((n,i) => `
    <div class="legend-item">
      <span class="legend-dot" style="background:${cols[i]}"></span>
      ${n}
      <span class="legend-pct">${Math.round(vals[i]/total*100)}%</span>
    </div>`).join('');
}

/* ── BREEDS LIST ───────────────────────────────────── */
function buildBreeds(breeds) {
  const max = breeds[0].count;
  const cont = document.getElementById('breedsList');
  cont.innerHTML = breeds.map((b,i) => `
    <div class="breed-item" style="animation-delay:${i*60}ms">
      <span class="breed-rank">#${i+1}</span>
      <span class="breed-emoji">${b.emoji}</span>
      <div class="breed-info">
        <div class="breed-name">${b.name}</div>
        <div class="breed-bar-wrap">
          <div class="breed-bar" data-w="${Math.round(b.count/max*100)}" style="width:0%"></div>
        </div>
      </div>
      <span class="breed-count">${b.count}</span>
    </div>`).join('');

  // animate bars
  requestAnimationFrame(() => {
    document.querySelectorAll('.breed-bar').forEach(el => {
      setTimeout(() => { el.style.width = el.dataset.w + '%'; }, 300);
    });
  });
}

/* ── RECENT TABLE ──────────────────────────────────── */
const statusMap = {
  aprobada:  { cls:'aprobada',  label:'Aprobada'  },
  pendiente: { cls:'pendiente', label:'Pendiente' },
  revision:  { cls:'revision',  label:'En Revisión'},
  rechazada: { cls:'rechazada', label:'Rechazada' },
};
const avatarBg = { aprobada:'#d4f5ee', pendiente:'#fff3d4', revision:'#d6ecff', rechazada:'#ffe5e5' };

function buildRecentTable(rows) {
  const tbody = document.getElementById('recentTableBody');
  tbody.innerHTML = rows.map((r,i) => `
    <tr style="animation-delay:${i*70}ms">
      <td>
        <div class="pet-cell">
          <div class="pet-avatar" style="background:${avatarBg[r.status]}">${r.emoji}</div>
          <div>
            <div class="pet-name">${r.pet}</div>
            <div class="pet-breed">${r.breed}</div>
          </div>
        </div>
      </td>
      <td style="font-size:12px;font-weight:600;">${r.adopter}</td>
      <td style="font-size:12px;color:var(--muted);">${r.date}</td>
      <td><span class="badge-status ${r.status}">${statusMap[r.status].label}</span></td>
      <td style="text-align:center"><span class="days-pill">${r.days}d</span></td>
    </tr>`).join('');
}

/* ── RENDER ALL ────────────────────────────────────── */
function render(period) {
  const d = DATA[period] || DATA['mes'];
  animateKPIs(d.kpi);
  buildBarChart(d.bar);
  buildDonutChart(d.donut);
  buildBreeds(d.breeds);
  buildRecentTable(d.recent);
}

function updatePeriod() {
  currentPeriod = document.getElementById('periodoSelect').value;
  if (currentPeriod === 'custom') {
    alert('Funcionalidad de rango personalizado próximamente.');
    document.getElementById('periodoSelect').value = 'mes';
    currentPeriod = 'mes';
  }
  render(currentPeriod);
}

function updateRefugio() {
  // Simula refresh
  const cards = document.querySelectorAll('.kpi-card');
  cards.forEach(c => { c.style.opacity='.5'; });
  setTimeout(() => {
    cards.forEach(c => { c.style.opacity='1'; });
    render(currentPeriod);
  }, 400);
}

function exportPDF() {
  const btn = document.querySelector('.btn-export');
  btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Generando...';
  btn.style.pointerEvents = 'none';
  setTimeout(() => {
    btn.innerHTML = '<i class="fas fa-check"></i> Listo';
    btn.style.background = '#d4f5ee';
    btn.style.color = '#00b894';
    setTimeout(() => {
      btn.innerHTML = '<i class="fas fa-file-pdf"></i> Exportar PDF';
      btn.style.background = '';
      btn.style.color = '';
      btn.style.pointerEvents = '';
    }, 2000);
  }, 1800);
}

/* ── INIT ──────────────────────────────────────────── */
window.addEventListener('DOMContentLoaded', () => {
  // Trigger kpi animations when they become visible
  const kpiObserver = new IntersectionObserver(entries => {
    entries.forEach(e => {
      if (e.isIntersecting) {
        render('mes');
        kpiObserver.disconnect();
      }
    });
  }, { threshold: 0.2 });
  kpiObserver.observe(document.getElementById('kpiGrid'));
});
