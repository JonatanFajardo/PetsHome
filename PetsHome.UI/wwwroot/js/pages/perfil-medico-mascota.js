
function switchTab(id, btn) {
  document.querySelectorAll('.tab-panel').forEach(p => p.classList.remove('active'));
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  document.getElementById('tab-' + id).classList.add('active');
  btn.classList.add('active');
  animateBars();
}

function animateBars() {
  setTimeout(() => {
    document.querySelectorAll('.prog-fill, .trat-prog-fill').forEach(bar => {
      const pct = bar.dataset.pct || 0;
      bar.style.width = pct + '%';
    });
  }, 80);
}

// Animate on load
window.addEventListener('load', () => {
  // Stagger sidebar stats
  document.querySelectorAll('.mini-stat').forEach((el, i) => {
    el.style.animationDelay = (i * 0.08 + 0.2) + 's';
  });
  animateBars();

  // Close modal on backdrop click
  document.getElementById('modalConsulta').addEventListener('click', function(e) {
    if (e.target === this) this.style.display = 'none';
  });
});
