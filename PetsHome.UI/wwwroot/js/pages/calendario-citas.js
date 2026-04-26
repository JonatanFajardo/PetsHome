// ═══════════════════════════════════════════════════════
//  CONFIGURACIÓN
// ═══════════════════════════════════════════════════════

const TYPES = {
  'Emergencia':      { color: '#FF6B6B', bg: '#FFF0F0', icon: 'bi-exclamation-triangle-fill' },
  'Vacunacion':      { color: '#00B894', bg: '#E8FBF5', icon: 'bi-shield-plus-fill' },
  'Vacunación':      { color: '#00B894', bg: '#E8FBF5', icon: 'bi-shield-plus-fill' },
  'Control':         { color: '#0984E3', bg: '#EBF5FF', icon: 'bi-clipboard2-pulse-fill' },
  'Cirugia':         { color: '#E17055', bg: '#FFF3EE', icon: 'bi-scissors' },
  'Cirugía':         { color: '#E17055', bg: '#FFF3EE', icon: 'bi-scissors' },
  'Consulta General':{ color: '#A29BFE', bg: '#F0EEFF', icon: 'bi-chat-square-heart-fill' },
  'ConsultaGeneral': { color: '#A29BFE', bg: '#F0EEFF', icon: 'bi-chat-square-heart-fill' }
};

const DOW    = ['Dom','Lun','Mar','Mié','Jue','Vie','Sáb'];
const MONTHS = ['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre'];

// ═══════════════════════════════════════════════════════
//  DATOS (se llenan desde la API)
// ═══════════════════════════════════════════════════════
let appointments = [];
let fetchedStart = null;
let fetchedEnd   = null;

function fmt(d) {
  return `${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`;
}

/** Normaliza el tipo para coincidir con las clases CSS (sin tildes ni espacios). */
function typeClass(t) {
  if (!t) return 'ConsultaGeneral';
  return t.normalize('NFD').replace(/[\u0300-\u036f]/g,'').replace(/\s+/g,'');
}
function typeColor(t) { return (TYPES[t] || TYPES[typeClass(t)] || {}).color || '#A29BFE'; }
function typeBg(t)    { return (TYPES[t] || TYPES[typeClass(t)] || {}).bg    || '#F0EEFF'; }
function typeIcon(t)  { return (TYPES[t] || TYPES[typeClass(t)] || {}).icon  || 'bi-calendar'; }

function timeToMin(t) { const [h,m] = t.split(':').map(Number); return h*60+m; }

function fmtDateLong(d) {
  const dayNames = ['Domingo','Lunes','Martes','Miércoles','Jueves','Viernes','Sábado'];
  return `${dayNames[d.getDay()]}, ${d.getDate()} de ${MONTHS[d.getMonth()]} ${d.getFullYear()}`;
}

function getAppts(dateStr) {
  return appointments.filter(a => a.date === dateStr).sort((a,b) => a.time.localeCompare(b.time));
}

// ═══════════════════════════════════════════════════════
//  FETCH DE DATOS
// ═══════════════════════════════════════════════════════
async function ensureDataFetched(refDate) {
  const y = refDate.getFullYear(), m = refDate.getMonth();
  const start = new Date(y, m - 1, 1);
  const end   = new Date(y, m + 2,  0);

  // Ya tenemos el rango cubierto
  if (fetchedStart && fetchedEnd && start >= fetchedStart && end <= fetchedEnd) return;

  document.getElementById('cal-loading').style.display = 'flex';
  try {
    const resp = await fetch(`${URL_CALENDARIO}?inicio=${fmt(start)}&fin=${fmt(end)}`);
    if (!resp.ok) throw new Error('Error ' + resp.status);
    appointments = await resp.json();
    fetchedStart  = start;
    fetchedEnd    = end;
  } catch(e) {
    console.error('CalendarioData error:', e);
    appointments = [];
  } finally {
    document.getElementById('cal-loading').style.display = 'none';
  }
}

// ═══════════════════════════════════════════════════════
//  ESTADO
// ═══════════════════════════════════════════════════════
const today = new Date();
let currentView  = 'month';
let viewDate     = new Date(today.getFullYear(), today.getMonth(), 1);
let selectedDate = new Date(today);
let miniDate     = new Date(today.getFullYear(), today.getMonth(), 1);

// ═══════════════════════════════════════════════════════
//  VISTA MES
// ═══════════════════════════════════════════════════════
function renderMonth() {
  const dowRow = document.getElementById('dow-row');
  dowRow.innerHTML = DOW.map(d => `<div class="dow-cell">${d}</div>`).join('');

  const grid = document.getElementById('month-grid');
  grid.innerHTML = '';

  const firstDay = new Date(viewDate.getFullYear(), viewDate.getMonth(), 1);
  const lastDay  = new Date(viewDate.getFullYear(), viewDate.getMonth()+1, 0);
  const todayStr = fmt(today);
  const selStr   = fmt(selectedDate);

  let cells = [];
  for (let i = 0; i < firstDay.getDay(); i++) {
    const d = new Date(firstDay); d.setDate(d.getDate() - (firstDay.getDay() - i));
    cells.push({ date: d, other: true });
  }
  for (let d = 1; d <= lastDay.getDate(); d++)
    cells.push({ date: new Date(viewDate.getFullYear(), viewDate.getMonth(), d), other: false });
  while (cells.length % 7 !== 0) {
    const last = cells[cells.length-1].date;
    const nd = new Date(last); nd.setDate(nd.getDate()+1);
    cells.push({ date: nd, other: true });
  }

  cells.forEach(({ date, other }) => {
    const ds = fmt(date);
    const appts = getAppts(ds);
    const isToday = ds === todayStr;
    const isSel   = ds === selStr;

    const cell = document.createElement('div');
    cell.className = 'month-cell ripple' + (other?' other-month':'') + (isToday?' today':'') + (isSel&&!isToday?' selected':'');
    cell.onclick = () => selectDate(date);

    let pillsHtml = '';
    appts.slice(0, 3).forEach(a => {
      pillsHtml += `<div class="pill ${typeClass(a.type)}" onclick="event.stopPropagation();showEventTooltip(event,${a.id})" title="${a.pet} - ${a.type}">${a.pet}</div>`;
    });
    if (appts.length > 3) pillsHtml += `<div class="more-events">+${appts.length - 3} más</div>`;

    cell.innerHTML = `<div class="day-num">${date.getDate()}</div><div class="event-pills">${pillsHtml}</div>`;
    grid.appendChild(cell);
  });
}

// ═══════════════════════════════════════════════════════
//  VISTA SEMANA
// ═══════════════════════════════════════════════════════
function getWeekStart(d) {
  const s = new Date(d); s.setDate(s.getDate() - s.getDay()); return s;
}

function renderWeek() {
  const ws = getWeekStart(selectedDate);
  const days = Array.from({length:7}, (_,i) => { const d=new Date(ws); d.setDate(d.getDate()+i); return d; });
  const todayStr = fmt(today);

  const headerRow = document.getElementById('week-header-row');
  headerRow.style.gridTemplateColumns = `60px repeat(7,1fr)`;
  headerRow.innerHTML = `<div class="week-header-gutter"></div>` +
    days.map(d => `<div class="week-day-header ${fmt(d)===todayStr?'today':''}">
      <div class="week-day-name">${DOW[d.getDay()]}</div>
      <div class="week-day-num">${d.getDate()}</div>
    </div>`).join('');

  const body = document.getElementById('week-body');
  body.innerHTML = '';

  const START_H = 7, END_H = 21, SLOT_H = 48;

  const timeCol = document.createElement('div');
  timeCol.className = 'week-time-col';
  for (let h = START_H; h < END_H; h++) {
    const slot = document.createElement('div');
    slot.className = 'week-time-slot';
    slot.textContent = `${String(h).padStart(2,'0')}:00`;
    timeCol.appendChild(slot);
  }
  body.appendChild(timeCol);

  const grid = document.createElement('div');
  grid.className = 'week-grid';
  grid.style.gridTemplateColumns = `repeat(7,1fr)`;
  grid.style.position = 'relative';

  days.forEach(d => {
    const col = document.createElement('div');
    col.className = 'week-day-col';
    col.style.minHeight = `${(END_H-START_H)*SLOT_H}px`;

    for (let h = START_H; h < END_H; h++) {
      col.appendChild(Object.assign(document.createElement('div'), {className:'slot-line'}));
      col.appendChild(Object.assign(document.createElement('div'), {className:'slot-line half'}));
    }

    getAppts(fmt(d)).forEach(a => {
      const [ah, am] = a.time.split(':').map(Number);
      if (ah < START_H || ah >= END_H) return;
      const top    = ((ah - START_H) + am/60) * SLOT_H;
      const height = Math.max(28, (a.dur/60)*SLOT_H - 4);
      const ev = document.createElement('div');
      ev.className = `week-event ${typeClass(a.type)}`;
      ev.style.cssText = `top:${top}px;height:${height}px;`;
      ev.innerHTML = `<div class="week-event-time">${a.time}</div><div class="week-event-name">${a.pet}</div><div class="week-event-type">${a.type}</div>`;
      ev.onclick = e => { e.stopPropagation(); showEventTooltip(e, a.id); };
      col.appendChild(ev);
    });

    if (fmt(d) === fmt(today)) {
      const now = new Date();
      const nowMin = (now.getHours()-START_H)*60 + now.getMinutes();
      if (nowMin >= 0 && nowMin <= (END_H-START_H)*60) {
        const line = Object.assign(document.createElement('div'), {className:'time-now-line'});
        line.style.top = `${(nowMin/60)*SLOT_H}px`;
        col.appendChild(line);
      }
    }
    grid.appendChild(col);
  });
  body.appendChild(grid);
  setTimeout(() => { body.scrollTop = (8-START_H)*SLOT_H; }, 50);
}

// ═══════════════════════════════════════════════════════
//  VISTA DÍA
// ═══════════════════════════════════════════════════════
function renderDay() {
  const ds = fmt(selectedDate);
  const appts = getAppts(ds);
  document.getElementById('day-view-title').textContent = fmtDateLong(selectedDate);
  document.getElementById('day-view-badge').textContent = appts.length;

  const body = document.getElementById('day-body');
  body.innerHTML = '';
  const START_H = 7, END_H = 21, SLOT_H = 60;

  const timeCol = document.createElement('div');
  timeCol.className = 'day-time-col';
  for (let h = START_H; h < END_H; h++) {
    const lbl = document.createElement('div');
    lbl.className = 'day-time-label';
    lbl.textContent = `${String(h).padStart(2,'0')}:00`;
    timeCol.appendChild(lbl);
  }
  body.appendChild(timeCol);

  const slotsCol = document.createElement('div');
  slotsCol.className = 'day-slots-col';
  slotsCol.style.cssText = `min-height:${(END_H-START_H)*SLOT_H}px;position:relative;`;
  for (let h = START_H; h < END_H; h++)
    slotsCol.appendChild(Object.assign(document.createElement('div'), {className:'day-slot'}));

  appts.forEach(a => {
    const [ah, am] = a.time.split(':').map(Number);
    if (ah < START_H || ah >= END_H) return;
    const top    = ((ah-START_H) + am/60)*SLOT_H + 4;
    const height = Math.max(44, (a.dur/60)*SLOT_H - 8);
    const ev = document.createElement('div');
    ev.className = `day-event ${typeClass(a.type)}`;
    ev.style.cssText = `top:${top}px;height:${height}px;`;
    ev.innerHTML = `
      <div class="day-event-time-row"><i class="bi ${typeIcon(a.type)}"></i>${a.time} · ${a.dur} min</div>
      <div class="day-event-name">${a.pet}</div>
      <div class="day-event-meta">${a.type}${a.owner ? ' · ' + a.owner : ''}</div>`;
    ev.onclick = e => { e.stopPropagation(); showEventTooltip(e, a.id); };
    slotsCol.appendChild(ev);
  });

  if (fmt(today) === ds) {
    const now = new Date();
    const nowMin = (now.getHours()-START_H)*60 + now.getMinutes();
    if (nowMin >= 0) {
      const line = Object.assign(document.createElement('div'), {className:'time-now-line'});
      line.style.top = `${(nowMin/60)*SLOT_H}px`;
      slotsCol.appendChild(line);
    }
  }
  body.appendChild(slotsCol);
  setTimeout(() => { body.scrollTop = (8-START_H)*SLOT_H; }, 50);
}

// ═══════════════════════════════════════════════════════
//  SIDEBAR
// ═══════════════════════════════════════════════════════
function renderSidebar() {
  const ds = fmt(selectedDate);
  const appts = getAppts(ds);
  document.getElementById('sidebar-badge').textContent = appts.length;
  document.getElementById('sidebar-date-label').textContent = fmtDateLong(selectedDate);

  const list = document.getElementById('appt-list');
  list.innerHTML = '';

  if (appts.length === 0) {
    list.innerHTML = `<div class="empty-state"><i class="bi bi-calendar2-x"></i><p>Sin citas para este día</p></div>`;
    return;
  }
  appts.forEach((a, idx) => {
    const item = document.createElement('div');
    item.className = 'appt-item';
    item.style.animationDelay = `${idx*60}ms`;
    item.innerHTML = `
      <div class="appt-color-bar" style="background:${typeColor(a.type)}"></div>
      <div class="appt-time">${a.time}</div>
      <div class="appt-info">
        <div class="appt-pet">${a.pet}</div>
        <div class="appt-type">${a.type} · ${a.dur} min</div>
      </div>
      <div class="appt-icon" style="color:${typeColor(a.type)}"><i class="bi ${typeIcon(a.type)}"></i></div>`;
    item.onclick = e => showEventTooltip(e, a.id);
    list.appendChild(item);
  });
}

// ═══════════════════════════════════════════════════════
//  MINI CALENDARIO
// ═══════════════════════════════════════════════════════
function renderMini() {
  document.getElementById('mini-title').textContent = `${MONTHS[miniDate.getMonth()].substring(0,3)} ${miniDate.getFullYear()}`;
  document.getElementById('mini-dow').innerHTML = ['D','L','M','M','J','V','S'].map(d=>`<div class="mini-dow">${d}</div>`).join('');

  const days = document.getElementById('mini-days');
  days.innerHTML = '';
  const first = new Date(miniDate.getFullYear(), miniDate.getMonth(), 1);
  const last  = new Date(miniDate.getFullYear(), miniDate.getMonth()+1, 0);
  const todayStr = fmt(today), selStr = fmt(selectedDate);

  let cells = [];
  for (let i=0;i<first.getDay();i++){const d=new Date(first);d.setDate(d.getDate()-(first.getDay()-i));cells.push({date:d,other:true});}
  for (let d=1;d<=last.getDate();d++) cells.push({date:new Date(miniDate.getFullYear(),miniDate.getMonth(),d),other:false});
  while (cells.length%7!==0){const l=cells[cells.length-1].date;const nd=new Date(l);nd.setDate(nd.getDate()+1);cells.push({date:nd,other:true});}

  cells.forEach(({date,other}) => {
    const ds = fmt(date);
    const hasEv = appointments.some(a => a.date === ds);
    const el = document.createElement('div');
    el.className = 'mini-day'+(other?' other':'')+(ds===todayStr?' today':'')+(ds===selStr&&ds!==todayStr?' selected':'')+(hasEv&&!other?' has-events':'');
    el.textContent = date.getDate();
    el.onclick = () => selectDate(date);
    days.appendChild(el);
  });
}

// ═══════════════════════════════════════════════════════
//  TOOLTIP
// ═══════════════════════════════════════════════════════
function showEventTooltip(e, id) {
  const a = appointments.find(x => x.id === id);
  if (!a) return;
  const tt = document.getElementById('event-tooltip');
  document.getElementById('tt-bar').style.background = typeColor(a.type);
  document.getElementById('tt-pet').textContent  = a.pet;
  document.getElementById('tt-time').textContent = `${a.time} · ${a.dur} min`;
  document.getElementById('tt-type').textContent = a.type;
  document.getElementById('tt-owner').textContent = a.owner || '—';
  document.getElementById('tt-dur').textContent  = `Duración: ${a.dur} minutos`;
  document.getElementById('tt-detail-link').href  = `${URL_DETALLE}/${a.id}`;

  const rect = e.target.getBoundingClientRect();
  let left = rect.right + 10, top = rect.top;
  if (left + 240 > window.innerWidth) left = rect.left - 250;
  if (top + 200 > window.innerHeight) top = window.innerHeight - 210;
  tt.style.left = `${left}px`;
  tt.style.top  = `${Math.max(8,top)}px`;
  tt.classList.add('show');
  e.stopPropagation();
}
function hideTooltip() { document.getElementById('event-tooltip').classList.remove('show'); }
document.addEventListener('click', hideTooltip);

// ═══════════════════════════════════════════════════════
//  NAVEGACIÓN
// ═══════════════════════════════════════════════════════
function updateTitle() {
  const el = document.getElementById('cal-title');
  if (currentView === 'month') {
    el.textContent = `${MONTHS[viewDate.getMonth()]} ${viewDate.getFullYear()}`;
  } else if (currentView === 'week') {
    const ws = getWeekStart(selectedDate);
    const we = new Date(ws); we.setDate(we.getDate()+6);
    el.textContent = `${ws.getDate()} – ${we.getDate()} de ${MONTHS[ws.getMonth()]} ${ws.getFullYear()}`;
  } else {
    el.textContent = fmtDateLong(selectedDate);
  }
}

document.getElementById('btn-prev').onclick = () => {
  if (currentView==='month')      { viewDate.setMonth(viewDate.getMonth()-1); miniDate.setMonth(miniDate.getMonth()-1); }
  else if (currentView==='week')  { selectedDate.setDate(selectedDate.getDate()-7); viewDate = new Date(selectedDate); }
  else                            { selectedDate.setDate(selectedDate.getDate()-1); }
  refresh();
};
document.getElementById('btn-next').onclick = () => {
  if (currentView==='month')      { viewDate.setMonth(viewDate.getMonth()+1); miniDate.setMonth(miniDate.getMonth()+1); }
  else if (currentView==='week')  { selectedDate.setDate(selectedDate.getDate()+7); viewDate = new Date(selectedDate); }
  else                            { selectedDate.setDate(selectedDate.getDate()+1); }
  refresh();
};
document.getElementById('mini-prev').onclick = () => { miniDate.setMonth(miniDate.getMonth()-1); renderMini(); };
document.getElementById('mini-next').onclick = () => { miniDate.setMonth(miniDate.getMonth()+1); renderMini(); };

function goToday() {
  selectedDate = new Date();
  viewDate  = new Date(today.getFullYear(), today.getMonth(), 1);
  miniDate  = new Date(today.getFullYear(), today.getMonth(), 1);
  refresh();
}
function selectDate(d) {
  selectedDate = new Date(d);
  viewDate = new Date(d.getFullYear(), d.getMonth(), 1);
  miniDate = new Date(d.getFullYear(), d.getMonth(), 1);
  if (currentView !== 'month') currentView = 'day';
  refresh();
  updateViewBtns();
  setActiveView(currentView);
}

// ═══════════════════════════════════════════════════════
//  CAMBIO DE VISTA
// ═══════════════════════════════════════════════════════
document.querySelectorAll('.view-btn').forEach(btn => {
  btn.addEventListener('click', () => {
    currentView = btn.dataset.view;
    updateViewBtns();
    setActiveView(currentView);
    refresh();
  });
});
function updateViewBtns() {
  document.querySelectorAll('.view-btn').forEach(b => b.classList.toggle('active', b.dataset.view === currentView));
}
function setActiveView(v) {
  ['month','week','day'].forEach(id => {
    const el = document.getElementById(`view-${id}`);
    el.style.display = id === v ? 'flex' : 'none';
    if (id === v) el.style.flexDirection = 'column';
  });
}

// ═══════════════════════════════════════════════════════
//  REFRESH (async)
// ═══════════════════════════════════════════════════════
async function refresh() {
  const refDate = currentView === 'month' ? viewDate : selectedDate;
  await ensureDataFetched(refDate);
  updateTitle();
  renderMini();
  renderSidebar();
  if (currentView === 'month')     renderMonth();
  else if (currentView === 'week') renderWeek();
  else                             renderDay();
}

// ═══════════════════════════════════════════════════════
//  INIT
// ═══════════════════════════════════════════════════════
setActiveView('month');
refresh();

// Actualizar línea de tiempo actual cada minuto
setInterval(() => {
  if (currentView === 'week') renderWeek();
  if (currentView === 'day')  renderDay();
}, 60000);