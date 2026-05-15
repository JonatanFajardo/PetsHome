
  // ── Sticky navbar ──
  const nav = document.getElementById('main-nav');
  window.addEventListener('scroll', () => nav.classList.toggle('scrolled', scrollY > 24), { passive: true });

  // ── Scroll reveal + stats + timeline ──
  const revObs = new IntersectionObserver(entries => {
    entries.forEach(e => {
      if (!e.isIntersecting) return;
      e.target.classList.add('in');

      // Counters
      if (e.target.hasAttribute('data-stat') && !e.target.dataset.counted) {
        e.target.dataset.counted = '1';
        e.target.querySelectorAll('.stat-num').forEach(el => {
          const target = +el.dataset.target;
          let cur = 0;
          const step = Math.ceil(target / 55);
          const t = setInterval(() => {
            cur = Math.min(cur + step, target);
            el.textContent = cur + '+';
            if (cur >= target) clearInterval(t);
          }, 24);
        });
      }

      // Timeline
      if (e.target.id === 'step-connector') {
        document.getElementById('step-fill').classList.add('animate');
      }
    });
  }, { threshold: 0.15 });

  document.querySelectorAll('.reveal').forEach(el => revObs.observe(el));
  const sc = document.getElementById('step-connector');
  if (sc) revObs.observe(sc);

  // ── Filter chips ──
  document.querySelectorAll('.filter-chip').forEach(chip => {
    chip.addEventListener('click', () => {
      document.querySelectorAll('.filter-chip').forEach(c => c.classList.remove('active'));
      chip.classList.add('active');
      const f = chip.dataset.filter;
      document.querySelectorAll('.animal-col').forEach(col => {
        col.style.display = (f === 'all' || (col.dataset.type || '').includes(f)) ? '' : 'none';
      });
    });
  });
