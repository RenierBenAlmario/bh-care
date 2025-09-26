(function () {
  // Console-safe logging
  var log = (typeof console !== 'undefined' && console.log) ? console.log.bind(console) : function () {};
  var warn = (typeof console !== 'undefined' && console.warn) ? console.warn.bind(console) : function () {};
  var error = (typeof console !== 'undefined' && console.error) ? console.error.bind(console) : function () {};

  // Minimal Bootstrap Modal polyfill if bootstrap.js is not present
  if (!window.bootstrap) window.bootstrap = {};
  if (!window.bootstrap.Modal) {
    window.bootstrap.Modal = class {
      constructor(el) { this._el = el; }
      show() {
        if (!this._el) return;
        this._el.classList.add('show');
        this._el.style.display = 'block';
        this._el.removeAttribute('aria-hidden');
      }
      hide() {
        if (!this._el) return;
        this._el.classList.remove('show');
        this._el.style.display = 'none';
        this._el.setAttribute('aria-hidden', 'true');
      }
    };
    warn('Archive fallback: Bootstrap Modal polyfill active');
  }

  function ensureModal(id) {
    var el = document.getElementById(id);
    if (!el) return null;
    try { return new window.bootstrap.Modal(el); }
    catch (e) {
      return {
        show: function () {
          el.classList.add('show');
          el.style.display = 'block';
          el.removeAttribute('aria-hidden');
        },
        hide: function () {
          el.classList.remove('show');
          el.style.display = 'none';
          el.setAttribute('aria-hidden', 'true');
        }
      };
    }
  }

  function setLoading() {
    var content = document.getElementById('familyDetailsContent');
    if (!content) return;
    content.innerHTML = (
      '<div class="text-center">' +
        '<div class="spinner-border" role="status">' +
          '<span class="visually-hidden">Loading...</span>' +
        '</div>' +
        '<p class="mt-2">Loading family details...</p>' +
      '</div>'
    );
  }

  function showError(message) {
    var content = document.getElementById('familyDetailsContent');
    if (!content) return;
    content.innerHTML = (
      '<div class="alert alert-danger">' +
        '<h6><i class="fas fa-exclamation-triangle me-2"></i>Error</h6>' +
        '<p class="mb-0">' + (message || 'Unexpected error') + '</p>' +
      '</div>'
    );
  }

  function renderBasicSummary(data, familyId) {
    var content = document.getElementById('familyDetailsContent');
    if (!content) return;
    var stats = (data && data.stats) || {};
    var d = (data && data.data) || {};
    var counts = {
      immunization: Array.isArray(d.immunization) ? d.immunization.length : 0,
      heeadsss: Array.isArray(d.heeadsss) ? d.heeadsss.length : 0,
      ncd: Array.isArray(d.ncd) ? d.ncd.length : 0,
      vitalSigns: Array.isArray(d.vitalSigns) ? d.vitalSigns.length : 0
    };
    var total = (typeof stats.total === 'number') ? stats.total : (counts.immunization + counts.heeadsss + counts.ncd + counts.vitalSigns);

    content.innerHTML = '' +
      '<div class="alert alert-info">' +
        '<h6><i class="fas fa-info-circle me-2"></i>Family Information</h6>' +
        '<div><strong>Family Number:</strong> ' + (familyId || '') + '</div>' +
        '<div><strong>Total Records:</strong> ' + total + '</div>' +
        '<div class="mt-2 small text-muted">This is a simplified view (fallback). The enhanced view will load when the main script parses successfully.</div>' +
      '</div>' +
      '<ul class="list-group">' +
        '<li class="list-group-item d-flex justify-content-between"><span>Immunization</span><span class="badge bg-success">' + counts.immunization + '</span></li>' +
        '<li class="list-group-item d-flex justify-content-between"><span>HEEADSSS</span><span class="badge bg-warning">' + counts.heeadsss + '</span></li>' +
        '<li class="list-group-item d-flex justify-content-between"><span>NCD</span><span class="badge bg-danger">' + counts.ncd + '</span></li>' +
        '<li class="list-group-item d-flex justify-content-between"><span>Vital Signs</span><span class="badge bg-info">' + counts.vitalSigns + '</span></li>' +
      '</ul>';
  }

  // Global: used by inline onclick
  window.viewFamilyDetails = function (familyId) {
    try { log('Fallback viewFamilyDetails invoked for:', familyId); } catch (_) {}

    var modal = ensureModal('familyDetailsModal');
    if (!modal) {
      alert('Family details modal not found on the page.');
      return;
    }
    modal.show();
    setLoading();

    // Fetch data
    var url = '/Admin/Archive?handler=FamilyDetails&familyId=' + encodeURIComponent(familyId || '');
    fetch(url, { credentials: 'same-origin', headers: { 'Accept': 'application/json' } })
      .then(function (res) {
        if (!res.ok) throw new Error('Request failed: ' + res.status + ' ' + res.statusText);
        return res.json();
      })
      .then(function (data) {
        if (!data || !data.success) {
          throw new Error((data && data.error) || 'Unknown error returned by server');
        }
        // If the main, richer renderer exists, use it
        if (typeof window.displayFamilyDataWithDecryptionOption === 'function') {
          try { return window.displayFamilyDataWithDecryptionOption(data, familyId); } catch (e) { warn('Enhanced renderer failed, using fallback:', e); }
        }
        // Otherwise, render a basic summary
        renderBasicSummary(data, familyId);
      })
      .catch(function (e) {
        error('viewFamilyDetails fallback error:', e);
        showError('Error loading family details: ' + e.message);
      });
  };
})();
