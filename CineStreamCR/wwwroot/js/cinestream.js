(() => {
  'use strict';

  const state = {
    user: null,
    genres: [],
    years: [],
    watchlists: [],
    catalog: null,
    featured: null,
    currentView: 'home',
    filters: { search: '', genreId: '', year: '', sort: 'title', direction: 'asc', page: 1, pageSize: 8 },
    queue: [],
    currentMovie: null,
    currentIndex: -1,
    requestCount: 0,
    lastProgressSave: 0,
    searchTimer: null,
    catalogRequestId: 0
  };

  const dom = {};

  document.addEventListener('DOMContentLoaded', initialize);
  window.addEventListener('unhandledrejection', event => {
    event.preventDefault();
    console.error(event.reason);
    if (dom.toastContainer) toast(event.reason?.message || 'Ocurrió un error inesperado.', 'error');
  });

  async function initialize() {
    cacheDom();
    bindStaticEvents();

    try {
      const user = await api('/api/auth/me', {}, { showSpinner: false, redirectOn401: false });
      if (user) await enterApplication(user);
      else showLogin();
    } catch {
      showLogin();
    }
  }

  function cacheDom() {
    dom.loginView = document.querySelector('#login-view');
    dom.loginForm = document.querySelector('#login-form');
    dom.loginError = document.querySelector('#login-error');
    dom.appShell = document.querySelector('#app-shell');
    dom.appMain = document.querySelector('#app-main');
    dom.headerSearch = document.querySelector('#header-search');
    dom.userMenuButton = document.querySelector('#user-menu-button');
    dom.userDropdown = document.querySelector('#user-dropdown');
    dom.logoutButton = document.querySelector('#logout-button');
    dom.userName = document.querySelector('#user-name');
    dom.userEmail = document.querySelector('#user-email');
    dom.userInitial = document.querySelector('#user-initial');
    dom.modalRoot = document.querySelector('#modal-root');
    dom.toastContainer = document.querySelector('#toast-container');
    dom.spinner = document.querySelector('#spinner');
    dom.playerDock = document.querySelector('#player-dock');
    dom.dockPoster = document.querySelector('#dock-poster');
    dom.dockTitle = document.querySelector('#dock-title');
    dom.dockTime = document.querySelector('#dock-time');
    dom.dockProgressFill = document.querySelector('#dock-progress-fill');
    dom.dockPrev = document.querySelector('#dock-prev');
    dom.dockPlay = document.querySelector('#dock-play');
    dom.dockNext = document.querySelector('#dock-next');
    dom.dockExpand = document.querySelector('#dock-expand');
    dom.dockClose = document.querySelector('#dock-close');
    dom.playerOverlay = document.querySelector('#player-overlay');
    dom.video = document.querySelector('#video-player');
    dom.playerTitle = document.querySelector('#player-title');
    dom.playerSubtitle = document.querySelector('#player-subtitle');
    dom.playerCollapse = document.querySelector('#player-collapse');
    dom.playerPrev = document.querySelector('#player-prev');
    dom.playerBack = document.querySelector('#player-back');
    dom.playerPlay = document.querySelector('#player-play');
    dom.playerForward = document.querySelector('#player-forward');
    dom.playerNext = document.querySelector('#player-next');
    dom.playerCurrentTime = document.querySelector('#player-current-time');
    dom.playerProgress = document.querySelector('#player-progress');
    dom.playerDuration = document.querySelector('#player-duration');
    dom.playerVolume = document.querySelector('#player-volume');
    dom.playerFullscreen = document.querySelector('#player-fullscreen');
  }

  function bindStaticEvents() {
    dom.loginForm.addEventListener('submit', handleLogin);
    document.querySelectorAll('[data-nav]').forEach(button => {
      button.addEventListener('click', () => runUiAction(() => navigate(button.dataset.nav)));
    });

    dom.headerSearch.addEventListener('input', () => {
      clearTimeout(state.searchTimer);
      state.searchTimer = setTimeout(() => runUiAction(async () => {
        state.filters.search = dom.headerSearch.value.trim();
        state.filters.page = 1;
        if (state.currentView !== 'home') await renderHome();
        else await refreshCatalog();
      }), 350);
    });

    dom.userMenuButton.addEventListener('click', event => {
      event.stopPropagation();
      const isHidden = dom.userDropdown.classList.toggle('hidden');
      dom.userMenuButton.setAttribute('aria-expanded', String(!isHidden));
    });
    document.addEventListener('click', () => {
      dom.userDropdown.classList.add('hidden');
      dom.userMenuButton.setAttribute('aria-expanded', 'false');
    });
    dom.userDropdown.addEventListener('click', event => event.stopPropagation());
    dom.logoutButton.addEventListener('click', () => runUiAction(logout));

    document.addEventListener('keydown', event => {
      if (event.key !== 'Escape') return;
      if (!dom.playerOverlay.classList.contains('hidden')) dom.playerOverlay.classList.add('hidden');
      else if (dom.modalRoot.innerHTML) closeModal();
      else {
        dom.userDropdown.classList.add('hidden');
        dom.userMenuButton.setAttribute('aria-expanded', 'false');
      }
    });

    dom.dockPlay.addEventListener('click', () => runUiAction(togglePlayback));
    dom.playerPlay.addEventListener('click', () => runUiAction(togglePlayback));
    dom.dockPrev.addEventListener('click', () => runUiAction(playPrevious));
    dom.playerPrev.addEventListener('click', () => runUiAction(playPrevious));
    dom.dockNext.addEventListener('click', () => runUiAction(playNext));
    dom.playerNext.addEventListener('click', () => runUiAction(playNext));
    dom.dockExpand.addEventListener('click', () => dom.playerOverlay.classList.remove('hidden'));
    dom.playerCollapse.addEventListener('click', () => dom.playerOverlay.classList.add('hidden'));
    dom.dockClose.addEventListener('click', closePlayer);
    dom.playerBack.addEventListener('click', () => { dom.video.currentTime = Math.max(0, dom.video.currentTime - 10); });
    dom.playerForward.addEventListener('click', () => { dom.video.currentTime = Math.min(dom.video.duration || 0, dom.video.currentTime + 10); });
    dom.playerProgress.addEventListener('input', () => {
      if (Number.isFinite(dom.video.duration)) dom.video.currentTime = (Number(dom.playerProgress.value) / 100) * dom.video.duration;
    });
    dom.playerVolume.addEventListener('input', () => { dom.video.volume = Number(dom.playerVolume.value); });
    dom.playerFullscreen.addEventListener('click', async () => {
      try { await dom.video.requestFullscreen(); } catch { toast('No fue posible activar pantalla completa.', 'error'); }
    });
    dom.video.addEventListener('loadedmetadata', onVideoMetadata);
    dom.video.addEventListener('timeupdate', onVideoTimeUpdate);
    dom.video.addEventListener('play', updatePlayButtons);
    dom.video.addEventListener('pause', () => { updatePlayButtons(); savePlaybackProgress(); });
    dom.video.addEventListener('ended', () => runUiAction(async () => {
      await savePlaybackProgress(true);
      if (state.currentIndex < state.queue.length - 1) await playNext();
    }));
    dom.video.addEventListener('error', () => toast('No se encontró el video. Agrega el archivo MP4 correspondiente en wwwroot/videos.', 'error'));
    window.addEventListener('beforeunload', () => savePlaybackProgress(false, true));
  }

  async function handleLogin(event) {
    event.preventDefault();
    dom.loginError.textContent = '';
    const identifier = document.querySelector('#login-identifier').value.trim();
    const password = document.querySelector('#login-password').value;
    if (!identifier || !password) {
      dom.loginError.textContent = 'Complete ambos campos.';
      return;
    }

    try {
      const user = await api('/api/auth/login', {
        method: 'POST',
        body: JSON.stringify({ identifier, password })
      }, { redirectOn401: false });
      await enterApplication(user);
    } catch (error) {
      dom.loginError.textContent = error.message || 'No fue posible iniciar sesión.';
    }
  }

  async function enterApplication(user) {
    state.user = user;
    dom.loginView.classList.add('hidden');
    dom.appShell.classList.remove('hidden');
    dom.userName.textContent = user.displayName;
    dom.userEmail.textContent = user.email;
    dom.userInitial.textContent = (user.displayName || 'U').charAt(0).toUpperCase();

    const [genres, years, watchlists] = await Promise.all([
      api('/api/genres'),
      api('/api/years'),
      api('/api/watchlists')
    ]);
    state.genres = genres;
    state.years = years;
    state.watchlists = watchlists;
    await renderHome();
  }

  function showLogin() {
    state.user = null;
    state.requestCount = 0;
    dom.spinner.classList.add('hidden');
    dom.appShell.classList.add('hidden');
    dom.loginView.classList.remove('hidden');
    dom.modalRoot.innerHTML = '';
    document.body.classList.remove('modal-open');
    dom.userDropdown.classList.add('hidden');
    dom.userMenuButton.setAttribute('aria-expanded', 'false');
  }

  async function logout() {
    try { await savePlaybackProgress(); } catch { /* ignore */ }
    try { await api('/api/auth/logout', { method: 'POST' }, { showSpinner: false }); } catch { /* ignore */ }
    closePlayer();
    showLogin();
  }

  async function navigate(destination) {
    if (destination === 'home') await renderHome();
    if (destination === 'watchlists') await renderWatchLists();
  }

  function setActiveNavigation(name) {
    document.querySelectorAll('.nav-link').forEach(button => {
      button.classList.toggle('active', button.dataset.nav === name);
    });
  }

  async function renderHome() {
    state.currentView = 'home';
    setActiveNavigation('home');
    dom.headerSearch.value = state.filters.search;

    if (!state.featured) {
      const featuredCard = await api('/api/movies/featured');
      state.featured = await api(`/api/movies/${featuredCard.id}`);
    }

    const movie = state.featured;
    dom.appMain.innerHTML = `
      <section class="hero">
        <div class="hero-bg" style="background-image:url('${escapeAttr(movie.backdropUrl)}')"></div>
        <div class="hero-content">
          <span class="hero-badge">Estreno destacado</span>
          <h1>${escapeHtml(movie.title)}</h1>
          <div class="hero-meta">
            <span>${movie.releaseYear}</span>
            <span>${movie.durationMinutes} min</span>
            <span>★ ${formatRating(movie.rating)}</span>
            <span>${movie.genres.map(escapeHtml).join(' · ')}</span>
          </div>
          <p class="hero-description">${escapeHtml(movie.synopsis)}</p>
          <div class="hero-actions">
            <button id="hero-play" class="btn btn-primary">▶ Reproducir</button>
            <button id="hero-details" class="btn btn-secondary">ⓘ Más información</button>
            <button id="hero-list" class="btn btn-ghost">＋ Agregar a lista</button>
          </div>
        </div>
      </section>
      <section class="catalog-section">
        <div class="section-title">
          <div><h2>Catálogo de películas</h2><span id="catalog-count">Explora el catálogo completo</span></div>
        </div>
        <div class="filters">
          <label>Buscar por título
            <input id="catalog-search" type="search" value="${escapeAttr(state.filters.search)}" placeholder="Ejemplo: Chaplin" />
          </label>
          <label>Género
            <select id="genre-filter"><option value="">Todos</option>${state.genres.map(g => `<option value="${g.id}" ${String(g.id) === String(state.filters.genreId) ? 'selected' : ''}>${escapeHtml(g.name)}</option>`).join('')}</select>
          </label>
          <label>Año
            <select id="year-filter"><option value="">Todos</option>${state.years.map(y => `<option value="${y}" ${String(y) === String(state.filters.year) ? 'selected' : ''}>${y}</option>`).join('')}</select>
          </label>
          <label>Ordenar por
            <select id="sort-filter">
              <option value="title" ${state.filters.sort === 'title' ? 'selected' : ''}>Título</option>
              <option value="year" ${state.filters.sort === 'year' ? 'selected' : ''}>Año</option>
              <option value="rating" ${state.filters.sort === 'rating' ? 'selected' : ''}>Calificación</option>
            </select>
          </label>
          <label>Dirección
            <select id="direction-filter">
              <option value="asc" ${state.filters.direction === 'asc' ? 'selected' : ''}>Ascendente</option>
              <option value="desc" ${state.filters.direction === 'desc' ? 'selected' : ''}>Descendente</option>
            </select>
          </label>
        </div>
        <div id="movie-grid" class="movie-grid"></div>
        <div id="pagination" class="pagination"></div>
      </section>`;

    document.querySelector('#hero-play').addEventListener('click', () => runUiAction(() => playMovieById(movie.id, [movie.id])));
    document.querySelector('#hero-details').addEventListener('click', () => runUiAction(() => openMovieDetail(movie.id)));
    document.querySelector('#hero-list').addEventListener('click', () => runUiAction(() => openWatchListPicker(movie.id)));

    const catalogSearch = document.querySelector('#catalog-search');
    catalogSearch.addEventListener('input', () => {
      clearTimeout(state.searchTimer);
      state.searchTimer = setTimeout(() => runUiAction(async () => {
        state.filters.search = catalogSearch.value.trim();
        dom.headerSearch.value = state.filters.search;
        state.filters.page = 1;
        await refreshCatalog();
      }), 350);
    });
    ['genre-filter', 'year-filter', 'sort-filter', 'direction-filter'].forEach(id => {
      document.querySelector(`#${id}`).addEventListener('change', event => runUiAction(async () => {
        const key = id.replace('-filter', '').replace('genre', 'genreId');
        state.filters[key] = event.target.value;
        state.filters.page = 1;
        await refreshCatalog();
      }));
    });

    await refreshCatalog();
    dom.appMain.focus();
  }

  async function refreshCatalog() {
    if (state.currentView !== 'home') return;
    const requestId = ++state.catalogRequestId;
    const grid = document.querySelector('#movie-grid');
    const count = document.querySelector('#catalog-count');

    if (grid) {
      grid.setAttribute('aria-busy', 'true');
      grid.innerHTML = catalogSkeletonHtml();
    }
    if (count) count.textContent = 'Actualizando catálogo…';

    const params = new URLSearchParams();
    Object.entries(state.filters).forEach(([key, value]) => {
      if (value !== '' && value !== null && value !== undefined) params.set(key, value);
    });

    try {
      const result = await api(`/api/movies?${params.toString()}`, {}, { showSpinner: false });
      if (requestId !== state.catalogRequestId || state.currentView !== 'home') return;
      state.catalog = result;
      renderCatalogResults();
    } catch (error) {
      if (requestId !== state.catalogRequestId) return;
      if (grid) grid.innerHTML = `<div class="empty-state error-state"><strong>No fue posible cargar el catálogo</strong>${escapeHtml(error.message)}</div>`;
      if (count) count.textContent = 'Error al cargar';
      toast(error.message, 'error');
    } finally {
      if (requestId === state.catalogRequestId) grid?.removeAttribute('aria-busy');
    }
  }

  function catalogSkeletonHtml() {
    return Array.from({ length: 4 }, () => `
      <article class="movie-card skeleton-card" aria-hidden="true">
        <div class="skeleton skeleton-poster"></div>
        <div class="movie-card-body">
          <div class="skeleton skeleton-line skeleton-title"></div>
          <div class="skeleton skeleton-line"></div>
          <div class="skeleton skeleton-button"></div>
        </div>
      </article>`).join('');
  }

  function renderCatalogResults() {
    const grid = document.querySelector('#movie-grid');
    const count = document.querySelector('#catalog-count');
    const pagination = document.querySelector('#pagination');
    if (!grid || !state.catalog) return;

    count.textContent = `${state.catalog.totalCount} película${state.catalog.totalCount === 1 ? '' : 's'} encontrada${state.catalog.totalCount === 1 ? '' : 's'}`;
    if (!state.catalog.items.length) {
      grid.innerHTML = `<div class="empty-state"><strong>No encontramos películas</strong>Prueba con otros filtros o términos de búsqueda.</div>`;
      pagination.innerHTML = '';
      return;
    }

    grid.innerHTML = state.catalog.items.map(movie => movieCard(movie)).join('');
    bindMovieCardEvents(grid, state.catalog.items.map(x => x.id));
    pagination.innerHTML = paginationHtml(state.catalog.page, state.catalog.totalPages);
    pagination.querySelectorAll('button[data-page]').forEach(button => {
      button.addEventListener('click', () => runUiAction(async () => {
        state.filters.page = Number(button.dataset.page);
        await refreshCatalog();
        document.querySelector('.catalog-section')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }));
    });
  }

  function movieCard(movie, options = {}) {
    return `
      <article class="movie-card" data-movie-id="${movie.id}">
        <div class="movie-poster-wrap" data-action="details">
          <img class="movie-poster" src="${escapeAttr(movie.posterUrl)}" alt="Póster de ${escapeAttr(movie.title)}" loading="lazy" />
          <div class="movie-overlay">
            <button class="card-play" data-action="play" aria-label="Reproducir ${escapeAttr(movie.title)}">▶</button>
            ${movie.inAnyWatchList ? '<span class="watch-indicator" title="Está en una de tus listas">✓</span>' : ''}
          </div>
        </div>
        <div class="movie-card-body">
          <h3>${escapeHtml(movie.title)}</h3>
          <div class="movie-card-meta">
            <span>${movie.releaseYear}</span><span>${movie.durationMinutes} min</span><span class="rating-pill">★ ${formatRating(movie.rating)}</span>
          </div>
          <div class="card-actions">
            <button class="btn btn-secondary" data-action="details">Detalles</button>
            ${options.removeFromListId
              ? `<button class="btn btn-danger" data-action="remove" data-list-id="${options.removeFromListId}">Quitar</button>`
              : '<button class="btn btn-ghost" data-action="list">＋ Lista</button>'}
          </div>
        </div>
      </article>`;
  }

  function bindMovieCardEvents(container, queueIds, afterRemove) {
    container.querySelectorAll('.movie-card').forEach(card => {
      const id = Number(card.dataset.movieId);
      card.querySelectorAll('[data-action="details"]').forEach(el => el.addEventListener('click', event => {
        if (event.target.closest('[data-action="play"]')) return;
        runUiAction(() => openMovieDetail(id));
      }));
      card.querySelector('[data-action="play"]')?.addEventListener('click', event => {
        event.stopPropagation();
        runUiAction(() => playMovieById(id, queueIds));
      });
      card.querySelector('[data-action="list"]')?.addEventListener('click', () => runUiAction(() => openWatchListPicker(id)));
      card.querySelector('[data-action="remove"]')?.addEventListener('click', () => {
        const listId = Number(card.querySelector('[data-action="remove"]').dataset.listId);
        confirmAction('Quitar película', '¿Deseas quitar esta película de la lista?', async () => {
          await api(`/api/watchlists/${listId}/movies/${id}`, { method: 'DELETE' });
          toast('Película eliminada de la lista.', 'success');
          await refreshWatchListsCache();
          if (afterRemove) await afterRemove();
        });
      });
    });
  }

  function paginationHtml(current, total) {
    if (total <= 1) return '';
    const pages = [];
    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);
    pages.push(`<button data-page="${current - 1}" ${current === 1 ? 'disabled' : ''}>‹</button>`);
    for (let page = start; page <= end; page++) pages.push(`<button data-page="${page}" class="${page === current ? 'active' : ''}">${page}</button>`);
    pages.push(`<button data-page="${current + 1}" ${current === total ? 'disabled' : ''}>›</button>`);
    return pages.join('');
  }

  async function openMovieDetail(movieId) {
    const movie = await api(`/api/movies/${movieId}`);
    const score = movie.userReview?.score ?? 0;
    openModal(`
      <div class="modal modal-large">
        <div class="modal-header"><h2>Detalle de película</h2><button class="modal-close" data-close-modal aria-label="Cerrar">✕</button></div>
        <div class="modal-scroll">
          <div class="detail-hero">
            <div class="detail-hero-bg" style="background-image:url('${escapeAttr(movie.backdropUrl)}')"></div>
            <div class="detail-content">
              <span class="hero-badge">CineStream CR</span>
              <h2>${escapeHtml(movie.title)}</h2>
              <div class="detail-meta"><span>${movie.releaseYear}</span><span>${movie.durationMinutes} min</span><span>★ ${formatRating(movie.rating)}</span></div>
              <div class="genre-list">${movie.genres.map(g => `<span class="genre-chip">${escapeHtml(g)}</span>`).join('')}</div>
              <p class="detail-description">${escapeHtml(movie.synopsis)}</p>
              <div class="source-links">
                ${movie.informationSourceUrl ? `<a href="${escapeAttr(movie.informationSourceUrl)}" target="_blank" rel="noopener noreferrer">Fuente de información</a>` : ''}
                ${movie.imageSourceUrl ? `<a href="${escapeAttr(movie.imageSourceUrl)}" target="_blank" rel="noopener noreferrer">Fuente de imagen</a>` : ''}
              </div>
              <div class="detail-actions">
                <button id="detail-play" class="btn btn-primary">▶ Reproducir</button>
                <button id="detail-list" class="btn btn-secondary">＋ Administrar listas</button>
              </div>
            </div>
          </div>
          <div class="detail-sections">
            <section class="detail-section">
              <h3>Dirección</h3>
              ${movie.directors?.length ? `<div class="person-row">${movie.directors.map(p => personButton(p, 'Dirección')).join('')}</div>` : '<p class="empty-state">No se registró dirección.</p>'}
            </section>
            <section class="detail-section">
              <h3>Elenco principal</h3>
              <div class="person-row">${movie.cast.map(p => personButton(p, p.characterName ? `Como ${p.characterName}` : 'Actor/Actriz')).join('')}</div>
            </section>
            <section class="detail-section">
              <h3>Tu calificación y reseña</h3>
              <div class="review-box">
                <div class="score-picker">
                  <span>Selecciona de 1 a 10</span>
                  <div class="score-buttons">${Array.from({ length: 10 }, (_, i) => `<button type="button" data-score="${i + 1}" class="${score === i + 1 ? 'selected' : ''}">${i + 1}</button>`).join('')}</div>
                </div>
                <div class="form-grid">
                  <label>Reseña opcional<textarea id="review-comment" maxlength="1000" placeholder="Comparte una opinión breve...">${escapeHtml(movie.userReview?.comment ?? '')}</textarea></label>
                  <button id="save-review" class="btn btn-primary">Guardar calificación</button>
                </div>
              </div>
            </section>
          </div>
        </div>
      </div>`);

    let selectedScore = score;
    dom.modalRoot.querySelectorAll('[data-score]').forEach(button => {
      button.addEventListener('click', () => {
        selectedScore = Number(button.dataset.score);
        dom.modalRoot.querySelectorAll('[data-score]').forEach(x => x.classList.toggle('selected', x === button));
      });
    });
    dom.modalRoot.querySelector('#detail-play').addEventListener('click', () => {
      const queue = state.catalog?.items?.map(x => x.id) ?? [movieId];
      closeModal();
      runUiAction(() => playMovieById(movieId, queue));
    });
    dom.modalRoot.querySelector('#detail-list').addEventListener('click', () => runUiAction(() => openWatchListPicker(movieId, true)));
    dom.modalRoot.querySelectorAll('[data-person-id]').forEach(button => button.addEventListener('click', () => runUiAction(() => openPersonProfile(Number(button.dataset.personId)))));
    dom.modalRoot.querySelector('#save-review').addEventListener('click', () => runUiAction(async () => {
      if (!selectedScore) { toast('Selecciona una calificación entre 1 y 10.', 'error'); return; }
      const comment = dom.modalRoot.querySelector('#review-comment').value.trim();
      const saveButton = dom.modalRoot.querySelector('#save-review');
      saveButton.disabled = true;
      saveButton.textContent = 'Guardando…';
      try {
        await api(`/api/movies/${movieId}/review`, { method: 'PUT', body: JSON.stringify({ score: selectedScore, comment }) });
        toast('Calificación guardada.', 'success');
        if (state.featured?.id === movieId) state.featured = await api(`/api/movies/${movieId}`, {}, { showSpinner: false });
        if (state.currentView === 'home') await renderHome();
        await openMovieDetail(movieId);
      } catch (error) {
        toast(error.message, 'error');
        saveButton.disabled = false;
        saveButton.textContent = 'Guardar calificación';
      }
    }));
  }

  function personButton(person, subtitle) {
    return `<button class="person-card" data-person-id="${person.id}">
      <img src="${escapeAttr(person.photoUrl)}" alt="Foto de ${escapeAttr(person.fullName)}" onerror="this.onerror=null;this.src='/images/people/person-fallback.jpg'" />
      <strong>${escapeHtml(person.fullName)}</strong>
      <span>${escapeHtml(subtitle)}</span>
    </button>`;
  }

  async function openPersonProfile(personId) {
    const person = await api(`/api/people/${personId}`);
    openModal(`
      <div class="modal modal-large">
        <div class="modal-header"><h2>Perfil artístico</h2><button class="modal-close" data-close-modal aria-label="Cerrar">✕</button></div>
        <div class="modal-scroll modal-body person-profile">
          <img src="${escapeAttr(person.photoUrl)}" alt="Foto de ${escapeAttr(person.fullName)}" onerror="this.onerror=null;this.src='/images/people/person-fallback.jpg'" />
          <div>
            <h2>${escapeHtml(person.fullName)}</h2>
            <p><strong>Nacionalidad:</strong> ${escapeHtml(person.nationality)}</p>
            <p><strong>Fecha de nacimiento:</strong> ${formatDate(person.birthDate)}</p>
            <p class="bio">${escapeHtml(person.biography)}</p>
            <div class="source-links">
              ${person.informationSourceUrl ? `<a href="${escapeAttr(person.informationSourceUrl)}" target="_blank" rel="noopener noreferrer">Fuente biográfica</a>` : ''}
              ${person.imageSourceUrl ? `<a href="${escapeAttr(person.imageSourceUrl)}" target="_blank" rel="noopener noreferrer">Fuente de fotografía</a>` : ''}
            </div>
            <h3>Filmografía</h3>
            <div class="film-list">
              ${person.films.map(film => `<button class="film-item" data-film-id="${film.movieId}">
                <img src="${escapeAttr(film.posterUrl)}" alt="Póster de ${escapeAttr(film.title)}" />
                <span><strong>${escapeHtml(film.title)}</strong><span>${film.releaseYear} · ${escapeHtml(film.role)}</span>${film.characterName ? `<span>Personaje: ${escapeHtml(film.characterName)}</span>` : ''}</span>
              </button>`).join('')}
            </div>
          </div>
        </div>
      </div>`);
    dom.modalRoot.querySelectorAll('[data-film-id]').forEach(button => button.addEventListener('click', () => runUiAction(() => openMovieDetail(Number(button.dataset.filmId)))));
  }

  async function renderWatchLists() {
    state.currentView = 'watchlists';
    setActiveNavigation('watchlists');
    await refreshWatchListsCache();
    const totalMovies = state.watchlists.reduce((sum, list) => sum + list.movieCount, 0);
    dom.appMain.innerHTML = `
      <section class="page watchlists-page">
        <div class="page-header">
          <div><span class="eyebrow">Tu biblioteca personal</span><h1>Mis WatchLists</h1><p>${state.watchlists.length} listas · ${totalMovies} películas organizadas</p></div>
          <button id="new-watchlist" class="btn btn-primary">＋ Nueva lista</button>
        </div>
        <div class="watchlist-grid">
          ${state.watchlists.length ? state.watchlists.map((list, index) => `
            <article class="watchlist-card" data-list-id="${list.id}" style="--list-index:${index}">
              <div class="watchlist-visual"><span>${list.movieCount}</span><small>película${list.movieCount === 1 ? '' : 's'}</small></div>
              <div class="watchlist-card-content">
                <h3>${escapeHtml(list.name)}</h3>
                <p>${escapeHtml(list.description || 'Sin descripción.')}</p>
                <div class="meta"><span>Actualizada ${formatDateTime(list.updatedAt)}</span></div>
                <div class="watchlist-actions">
                  <button class="btn btn-primary" data-list-action="open">Abrir lista</button>
                  <button class="btn btn-secondary" data-list-action="edit">Editar</button>
                  <button class="btn btn-danger" data-list-action="delete">Eliminar</button>
                </div>
              </div>
            </article>`).join('') : '<div class="empty-state"><strong>Aún no tienes listas</strong>Crea una WatchList para empezar a organizar tu catálogo.</div>'}
        </div>
      </section>`;
    document.querySelector('#new-watchlist').addEventListener('click', () => openWatchListForm());
    dom.appMain.querySelectorAll('.watchlist-card').forEach(card => {
      const id = Number(card.dataset.listId);
      card.querySelector('[data-list-action="open"]').addEventListener('click', () => runUiAction(() => renderWatchListDetail(id)));
      card.querySelector('[data-list-action="edit"]').addEventListener('click', () => openWatchListForm(state.watchlists.find(x => x.id === id)));
      card.querySelector('[data-list-action="delete"]').addEventListener('click', () => deleteWatchList(id));
    });
    dom.appMain.focus();
  }

  async function renderWatchListDetail(listId) {
    setActiveNavigation('watchlists');
    const list = await api(`/api/watchlists/${listId}`);
    state.currentView = 'watchlist-detail';
    const queue = list.movies.map(x => x.id);
    dom.appMain.innerHTML = `
      <section class="page">
        <div class="watchlist-detail-head">
          <div><button id="back-lists" class="btn btn-ghost">← Volver a mis listas</button><span class="eyebrow">WatchList</span><h1>${escapeHtml(list.name)}</h1><p>${escapeHtml(list.description || 'Sin descripción.')}</p></div>
          <div class="watchlist-actions"><button id="edit-current-list" class="btn btn-secondary">Editar lista</button><button id="delete-current-list" class="btn btn-danger">Eliminar lista</button></div>
        </div>
        <div class="section-title"><div><h2>Películas guardadas</h2><span>${list.movies.length} elemento${list.movies.length === 1 ? '' : 's'}</span></div></div>
        <div id="watchlist-movies" class="movie-grid">
          ${list.movies.length ? list.movies.map(movie => movieCard(movie, { removeFromListId: list.id })).join('') : '<div class="empty-state"><strong>Esta lista está vacía</strong>Agrega películas desde el catálogo.</div>'}
        </div>
      </section>`;
    document.querySelector('#back-lists').addEventListener('click', () => runUiAction(renderWatchLists));
    document.querySelector('#edit-current-list').addEventListener('click', () => openWatchListForm(list));
    document.querySelector('#delete-current-list').addEventListener('click', () => deleteWatchList(list.id));
    bindMovieCardEvents(document.querySelector('#watchlist-movies'), queue, () => renderWatchListDetail(list.id));
    dom.appMain.focus();
  }

  function openWatchListForm(list = null, afterSave = null) {
    const editing = Boolean(list?.id);
    openModal(`
      <div class="modal">
        <div class="modal-header"><h2>${editing ? 'Editar WatchList' : 'Crear WatchList'}</h2><button class="modal-close" data-close-modal>✕</button></div>
        <form id="watchlist-form">
          <div class="modal-body form-grid">
            <label>Nombre<input id="watchlist-name" maxlength="100" required value="${escapeAttr(list?.name ?? '')}" /></label>
            <label>Descripción<textarea id="watchlist-description" maxlength="500">${escapeHtml(list?.description ?? '')}</textarea></label>
            <p id="watchlist-error" class="form-error"></p>
          </div>
          <div class="modal-footer"><button type="button" class="btn btn-ghost" data-close-modal>Cancelar</button><button class="btn btn-primary" type="submit">Guardar</button></div>
        </form>
      </div>`);
    dom.modalRoot.querySelector('#watchlist-form').addEventListener('submit', async event => {
      event.preventDefault();
      const form = event.currentTarget;
      const submitButton = form.querySelector('button[type="submit"]');
      const errorBox = form.querySelector('#watchlist-error');
      const name = form.querySelector('#watchlist-name').value.trim();
      const description = form.querySelector('#watchlist-description').value.trim();
      errorBox.textContent = '';
      if (name.length < 2) { errorBox.textContent = 'El nombre debe tener al menos 2 caracteres.'; return; }
      submitButton.disabled = true;
      submitButton.textContent = editing ? 'Actualizando…' : 'Creando…';
      try {
        if (editing) await api(`/api/watchlists/${list.id}`, { method: 'PUT', body: JSON.stringify({ name, description }) });
        else await api('/api/watchlists', { method: 'POST', body: JSON.stringify({ name, description }) });
        toast(editing ? 'Lista actualizada.' : 'Lista creada.', 'success');
        closeModal();
        await refreshWatchListsCache();
        if (afterSave) await afterSave();
        else if (state.currentView === 'watchlist-detail' && editing) await renderWatchListDetail(list.id);
        else if (state.currentView === 'watchlists') await renderWatchLists();
      } catch (error) {
        errorBox.textContent = error.message;
        submitButton.disabled = false;
        submitButton.textContent = 'Guardar';
      }
    });
  }

  function deleteWatchList(listId) {
    const list = state.watchlists.find(x => x.id === listId);
    confirmAction('Eliminar WatchList', `Se eliminará “${list?.name ?? 'esta lista'}”. Las películas permanecerán en el catálogo.`, async () => {
      await api(`/api/watchlists/${listId}`, { method: 'DELETE' });
      toast('Lista eliminada.', 'success');
      await refreshWatchListsCache();
      await renderWatchLists();
    });
  }

  async function openWatchListPicker(movieId, returnToDetail = false) {
    await refreshWatchListsCache();
    const movie = await api(`/api/movies/${movieId}`);
    const selected = new Set(movie.inWatchListIds);
    openModal(`
      <div class="modal">
        <div class="modal-header"><h2>Agregar “${escapeHtml(movie.title)}”</h2><button class="modal-close" data-close-modal aria-label="Cerrar">✕</button></div>
        <div class="modal-body modal-scroll">
          ${state.watchlists.length ? `<div class="check-list">${state.watchlists.map(list => `
            <div class="check-row"><input id="list-check-${list.id}" type="checkbox" value="${list.id}" ${selected.has(list.id) ? 'checked' : ''} /><label for="list-check-${list.id}"><strong>${escapeHtml(list.name)}</strong><br><small>${escapeHtml(list.description || `${list.movieCount} películas`)}</small></label></div>`).join('')}</div>` : '<div class="empty-state"><strong>No hay listas disponibles</strong>Crea una lista para agregar esta película.</div>'}
        </div>
        <div class="modal-footer"><button id="create-list-from-picker" class="btn btn-ghost">＋ Nueva lista</button>${state.watchlists.length ? '<button id="save-list-selection" class="btn btn-primary">Guardar selección</button>' : ''}</div>
      </div>`);
    dom.modalRoot.querySelector('#create-list-from-picker').addEventListener('click', () => openWatchListForm(null, () => openWatchListPicker(movieId, returnToDetail)));
    dom.modalRoot.querySelector('#save-list-selection')?.addEventListener('click', () => runUiAction(async () => {
      const saveButton = dom.modalRoot.querySelector('#save-list-selection');
      saveButton.disabled = true;
      saveButton.textContent = 'Guardando…';
      try {
        const chosen = new Set([...dom.modalRoot.querySelectorAll('.check-row input:checked')].map(x => Number(x.value)));
        const operations = [];
        for (const list of state.watchlists) {
          if (chosen.has(list.id) && !selected.has(list.id)) operations.push(api(`/api/watchlists/${list.id}/movies/${movieId}`, { method: 'POST' }, { showSpinner: false }));
          if (!chosen.has(list.id) && selected.has(list.id)) operations.push(api(`/api/watchlists/${list.id}/movies/${movieId}`, { method: 'DELETE' }, { showSpinner: false }));
        }
        await Promise.all(operations);
        toast('Listas actualizadas.', 'success');
        await refreshWatchListsCache();
        if (state.currentView === 'home') await refreshCatalog();
        if (returnToDetail) await openMovieDetail(movieId);
        else closeModal();
      } catch (error) {
        saveButton.disabled = false;
        saveButton.textContent = 'Guardar selección';
        throw error;
      }
    }));
  }

  async function refreshWatchListsCache() {
    state.watchlists = await api('/api/watchlists', {}, { showSpinner: false });
  }

  async function playMovieById(movieId, queueIds = null) {
    const movie = await api(`/api/movies/${movieId}`);
    if (queueIds?.length) state.queue = [...new Set(queueIds)];
    else if (!state.queue.includes(movieId)) state.queue = [movieId];
    state.currentIndex = state.queue.indexOf(movieId);
    if (state.currentIndex < 0) { state.queue = [movieId]; state.currentIndex = 0; }

    await savePlaybackProgress();
    state.currentMovie = movie;
    state.lastProgressSave = 0;
    dom.dockPoster.src = movie.posterUrl;
    dom.dockPoster.alt = `Póster de ${movie.title}`;
    dom.dockTitle.textContent = movie.title;
    dom.playerTitle.textContent = movie.title;
    dom.playerSubtitle.textContent = `${movie.releaseYear} · ${movie.durationMinutes} min · ${movie.genres.join(' · ')}`;
    dom.playerDock.classList.remove('hidden');
    dom.playerOverlay.classList.remove('hidden');
    updateQueueButtons();

    const sourceChanged = !dom.video.src.endsWith(movie.videoUrl);
    if (sourceChanged) {
      dom.video.src = movie.videoUrl;
      dom.video.load();
    } else {
      try { await dom.video.play(); } catch { /* browser may require another click */ }
    }
  }

  async function onVideoMetadata() {
    dom.video.volume = Number(dom.playerVolume.value);
    if (!state.currentMovie) return;
    try {
      const progress = await api(`/api/playback/${state.currentMovie.id}`, {}, { showSpinner: false });
      if (progress && !progress.isCompleted && progress.currentSecond > 0 && progress.currentSecond < dom.video.duration - 2) {
        dom.video.currentTime = progress.currentSecond;
      }
    } catch { /* progress is optional */ }
    dom.playerDuration.textContent = formatTime(dom.video.duration);
    try { await dom.video.play(); } catch { updatePlayButtons(); }
  }

  function onVideoTimeUpdate() {
    const current = dom.video.currentTime || 0;
    const duration = dom.video.duration || 0;
    const percent = duration ? (current / duration) * 100 : 0;
    dom.playerProgress.value = String(percent);
    dom.playerCurrentTime.textContent = formatTime(current);
    dom.playerDuration.textContent = formatTime(duration);
    dom.dockTime.textContent = `${formatTime(current)} / ${formatTime(duration)}`;
    dom.dockProgressFill.style.width = `${percent}%`;

    if (current - state.lastProgressSave >= 5) {
      state.lastProgressSave = current;
      savePlaybackProgress();
    }
  }

  async function savePlaybackProgress(completed = false, useBeacon = false) {
    if (!state.currentMovie || !Number.isFinite(dom.video.currentTime)) return;
    const payload = {
      movieId: state.currentMovie.id,
      currentSecond: dom.video.currentTime || 0,
      totalSeconds: Number.isFinite(dom.video.duration) ? dom.video.duration : 0,
      isCompleted: completed || (dom.video.duration > 0 && dom.video.currentTime >= dom.video.duration - 1)
    };
    if (useBeacon) {
      try {
        fetch('/api/playback', {
          method: 'PUT',
          credentials: 'same-origin',
          headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
          },
          body: JSON.stringify(payload),
          keepalive: true
        });
      } catch { /* non-critical */ }
      return;
    }
    try {
      await api('/api/playback', { method: 'PUT', body: JSON.stringify(payload) }, { showSpinner: false });
    } catch { /* non-critical */ }
  }

  async function togglePlayback() {
    if (!state.currentMovie) return;
    if (dom.video.paused) {
      try { await dom.video.play(); } catch { toast('Pulsa nuevamente para iniciar la reproducción.', 'info'); }
    } else dom.video.pause();
  }

  function updatePlayButtons() {
    const symbol = dom.video.paused ? '▶' : '❚❚';
    dom.dockPlay.textContent = symbol;
    dom.playerPlay.textContent = symbol;
  }

  function updateQueueButtons() {
    const hasPrevious = state.currentIndex > 0;
    const hasNext = state.currentIndex >= 0 && state.currentIndex < state.queue.length - 1;
    [dom.dockPrev, dom.playerPrev].forEach(x => x.disabled = !hasPrevious);
    [dom.dockNext, dom.playerNext].forEach(x => x.disabled = !hasNext);
  }

  async function playPrevious() {
    if (state.currentIndex <= 0) return;
    await playMovieById(state.queue[state.currentIndex - 1], state.queue);
  }

  async function playNext() {
    if (state.currentIndex < 0 || state.currentIndex >= state.queue.length - 1) return;
    await playMovieById(state.queue[state.currentIndex + 1], state.queue);
  }

  function closePlayer() {
    savePlaybackProgress();
    dom.video.pause();
    dom.video.removeAttribute('src');
    dom.video.load();
    dom.playerDock.classList.add('hidden');
    dom.playerOverlay.classList.add('hidden');
    state.currentMovie = null;
    state.queue = [];
    state.currentIndex = -1;
  }

  function openModal(html) {
    dom.modalRoot.innerHTML = `<div class="modal-backdrop" role="presentation">${html}</div>`;
    document.body.classList.add('modal-open');
    const backdrop = dom.modalRoot.querySelector('.modal-backdrop');
    const modal = dom.modalRoot.querySelector('.modal');
    backdrop.scrollTop = 0;
    modal?.setAttribute('role', 'dialog');
    modal?.setAttribute('aria-modal', 'true');
    dom.modalRoot.querySelectorAll('[data-close-modal]').forEach(button => button.addEventListener('click', closeModal));
    backdrop.addEventListener('click', event => {
      if (event.target === backdrop) closeModal();
    });
    requestAnimationFrame(() => dom.modalRoot.querySelector('.modal-close, input, button')?.focus({ preventScroll: true }));
  }

  function closeModal() {
    dom.modalRoot.innerHTML = '';
    document.body.classList.remove('modal-open');
  }

  function confirmAction(title, message, onConfirm) {
    openModal(`
      <div class="modal">
        <div class="modal-header"><h2>${escapeHtml(title)}</h2><button class="modal-close" data-close-modal>✕</button></div>
        <div class="modal-body"><p>${escapeHtml(message)}</p></div>
        <div class="modal-footer"><button class="btn btn-ghost" data-close-modal>Cancelar</button><button id="confirm-action" class="btn btn-danger">Confirmar</button></div>
      </div>`);
    dom.modalRoot.querySelector('#confirm-action').addEventListener('click', async () => {
      try { await onConfirm(); closeModal(); } catch (error) { toast(error.message, 'error'); }
    });
  }

  async function runUiAction(action) {
    try {
      return await action();
    } catch (error) {
      console.error(error);
      toast(error?.message || 'Ocurrió un error inesperado.', 'error');
      return null;
    }
  }

  function toast(message, type = 'info') {
    const element = document.createElement('div');
    element.className = `toast ${type}`;
    element.textContent = message;
    dom.toastContainer.appendChild(element);
    setTimeout(() => element.remove(), 3600);
  }

  async function api(url, options = {}, settings = {}) {
    const { showSpinner = true, redirectOn401 = true } = settings;
    if (showSpinner) setLoading(true);
    try {
      const headers = new Headers(options.headers || {});
      if (options.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json');
      const method = String(options.method || 'GET').toUpperCase();
      if (!['GET', 'HEAD', 'OPTIONS'].includes(method)) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        if (token) headers.set('RequestVerificationToken', token);
      }
      const response = await fetch(url, { credentials: 'same-origin', ...options, headers });
      if (response.status === 401 && !url.includes('/auth/login')) {
        if (redirectOn401) showLogin();
        throw createHttpError('La sesión expiró. Inicia sesión nuevamente.', 401);
      }
      if (response.status === 204) return null;
      const contentType = response.headers.get('content-type') || '';
      const data = contentType.includes('application/json') ? await response.json() : await response.text();
      if (data && typeof data === 'object' && Object.prototype.hasOwnProperty.call(data, 'esCorrecto')) {
        if (!data.esCorrecto || !response.ok) {
          throw createHttpError(data.mensaje || 'Ocurrió un error.', data.codigo || response.status);
        }
        return data.dato ?? null;
      }
      if (!response.ok) {
        const message = typeof data === 'object' ? (data.message || data.title || 'Ocurrió un error.') : (data || 'Ocurrió un error.');
        throw createHttpError(message, response.status);
      }
      return data;
    } catch (error) {
      if (error.name === 'TypeError') throw createHttpError('No se pudo conectar con el servidor.', 0);
      throw error;
    } finally {
      if (showSpinner) setLoading(false);
    }
  }

  function setLoading(active) {
    state.requestCount += active ? 1 : -1;
    state.requestCount = Math.max(0, state.requestCount);
    dom.spinner.classList.toggle('hidden', state.requestCount === 0);
  }

  function createHttpError(message, status) {
    const error = new Error(message);
    error.status = status;
    return error;
  }

  function formatRating(value) { return Number(value || 0).toFixed(1); }
  function formatTime(seconds) {
    if (!Number.isFinite(seconds)) return '00:00';
    const total = Math.max(0, Math.floor(seconds));
    const minutes = Math.floor(total / 60);
    const secs = total % 60;
    return `${String(minutes).padStart(2, '0')}:${String(secs).padStart(2, '0')}`;
  }
  function formatDate(value) {
    if (!value) return 'No disponible';
    return new Intl.DateTimeFormat('es-CR', { year: 'numeric', month: 'long', day: 'numeric', timeZone: 'UTC' }).format(new Date(`${value}T00:00:00Z`));
  }
  function formatDateTime(value) {
    if (!value) return '';
    return new Intl.DateTimeFormat('es-CR', { year: 'numeric', month: 'short', day: 'numeric' }).format(new Date(value));
  }
  function escapeHtml(value) {
    return String(value ?? '').replace(/[&<>'"]/g, char => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', "'": '&#39;', '"': '&quot;' })[char]);
  }
  function escapeAttr(value) { return escapeHtml(value); }
})();
