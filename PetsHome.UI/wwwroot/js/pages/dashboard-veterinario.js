
  /* === Live Clock === */
  function updateClock() {
    const now = new Date();
    let h = now.getHours();
    const m = String(now.getMinutes()).padStart(2,'0');
    const ampm = h >= 12 ? 'PM' : 'AM';
    h = h % 12 || 12;
    document.getElementById('liveclock').textContent = `${String(h).padStart(2,'0')}:${m} ${ampm}`;
  }
  updateClock();
  setInterval(updateClock, 10000);

  /* === Countdown timer (demo: 13 min from now) === */
  let totalSeconds = 13 * 60;
  function updateCountdown() {
    if (totalSeconds <= 0) {
      document.getElementById('countdown').textContent = '00:00';
      document.getElementById('countdown2').textContent = '0';
      return;
    }
    totalSeconds--;
    const m = Math.floor(totalSeconds / 60);
    const s = totalSeconds % 60;
    document.getElementById('countdown').textContent = `${String(m).padStart(2,'0')}:${String(s).padStart(2,'0')}`;
    document.getElementById('countdown2').textContent = String(m);
  }
  setInterval(updateCountdown, 1000);

  /* === Calendar nav === */
  function goToDay(day) {
    console.log('Navegando al día', day, 'de Abril 2026');
  }

  /* === Stagger animations for items === */
  document.querySelectorAll('.timeline-item').forEach((el, i) => {
    el.style.animationDelay = (0.08 * i + 0.1) + 's';
  });
  document.querySelectorAll('.patient-card').forEach((el, i) => {
    el.style.animationDelay = (0.1 * i + 0.1) + 's';
  });
  document.querySelectorAll('.alert-item').forEach((el, i) => {
    el.style.animationDelay = (0.08 * i + 0.08) + 's';
  });
  document.querySelectorAll('.activity-item').forEach((el, i) => {
    el.style.animationDelay = (0.08 * i + 0.08) + 's';
  });

  /* === Progress bars animated on load === */
  document.querySelectorAll('.progress-bar-custom').forEach(bar => {
    const target = bar.style.width;
    bar.style.width = '0';
    setTimeout(() => {
      bar.style.transition = 'width 1.2s cubic-bezier(0.4,0,0.2,1)';
      bar.style.width = target;
    }, 500);
  });
