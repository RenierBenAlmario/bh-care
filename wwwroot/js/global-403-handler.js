/* Global handler for 401/403 responses from AJAX/fetch requests.
   Shows a non-disruptive toast instead of navigating to AccessDenied/Login pages. */
(function () {
  'use strict';

  var DEFAULT_MSG_401 = 'Your session has expired or you are not logged in. Please sign in.';
  var DEFAULT_MSG_403 = 'Access denied. Your account is not verified yet or you lack permission.';
  var lastToastAt = 0; // simple de-dupe to prevent toast spam

  function preferToast(message, type) {
    // De-duplication window (1.5s)
    var now = Date.now();
    if (now - lastToastAt < 1500) return;
    lastToastAt = now;

    // Prefer an existing global showToast if present
    if (typeof window.showToast === 'function') {
      try {
        // Try signature: (type, message, duration)
        window.showToast(type || 'error', message, 5000);
        return;
      } catch (e1) {
        try {
          // Try signature: (message, type)
          window.showToast(message, type || 'error');
          return;
        } catch (e2) { /* fall through */ }
      }
    }

    // Try toastr if available
    if (typeof window.toastr !== 'undefined') {
      var t = type || 'error';
      if (t === 'success') toastr.success(message);
      else if (t === 'warning') toastr.warning(message);
      else if (t === 'info') toastr.info(message);
      else toastr.error(message);
      return;
    }

    // Bootstrap 5 toast fallback
    try {
      if (typeof window.bootstrap !== 'undefined') {
        var toastContainer = document.querySelector('.toast-container');
        if (!toastContainer) {
          toastContainer = document.createElement('div');
          toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
          toastContainer.style.zIndex = '1080';
          document.body.appendChild(toastContainer);
        }

        var color = 'info';
        if (type === 'error') color = 'danger';
        else if (type === 'warning') color = 'warning';
        else if (type === 'success') color = 'success';

        var toastEl = document.createElement('div');
        toastEl.className = 'toast align-items-center text-white bg-' + color + ' border-0';
        toastEl.setAttribute('role', 'alert');
        toastEl.setAttribute('aria-live', 'assertive');
        toastEl.setAttribute('aria-atomic', 'true');
        toastEl.innerHTML = '\n          <div class="d-flex">\n            <div class="toast-body">' + message + '</div>\n            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>\n          </div>\n        ';

        toastContainer.appendChild(toastEl);
        var toast = new bootstrap.Toast(toastEl, { autohide: true, delay: 5000 });
        toast.show();
        toastEl.addEventListener('hidden.bs.toast', function () { toastEl.remove(); });
        return;
      }
    } catch (_) { /* ignore and fallback */ }

    // Basic fallback
    alert(message);
  }

  function parseJsonSafely(text) {
    try { return JSON.parse(text); } catch (_) { return null; }
  }

  function extractMessageFromXhr(jqXHR, fallback) {
    var msg = fallback;
    if (!jqXHR) return msg;

    // Prefer JSON payload message if present
    if (jqXHR.responseJSON && (jqXHR.responseJSON.message || jqXHR.responseJSON.error)) {
      return jqXHR.responseJSON.message || jqXHR.responseJSON.error;
    }

    if (typeof jqXHR.responseText === 'string' && jqXHR.responseText.trim().length) {
      var data = parseJsonSafely(jqXHR.responseText);
      if (data && (data.message || data.error)) {
        return data.message || data.error;
      }
    }

    // Custom header support (if backend sends it)
    try {
      var headerMsg = jqXHR.getResponseHeader && jqXHR.getResponseHeader('X-Error-Message');
      if (headerMsg) return headerMsg;
    } catch (_) { /* ignore */ }

    return msg;
  }

  // jQuery global error handler (covers $.ajax, $.get, $.post, etc.)
  if (window.jQuery) {
    jQuery(document).ajaxError(function (_event, jqXHR /*, settings, thrownError */) {
      if (!jqXHR) return;
      if (jqXHR.status === 401) {
        var m401 = extractMessageFromXhr(jqXHR, DEFAULT_MSG_401);
        preferToast(m401, 'error');
      } else if (jqXHR.status === 403) {
        var m403 = extractMessageFromXhr(jqXHR, DEFAULT_MSG_403);
        preferToast(m403, 'error');
      }
    });
  }

  // fetch() wrapper
  if (window.fetch) {
    var originalFetch = window.fetch;
    window.fetch = function (input, init) {
      return originalFetch(input, init).then(function (resp) {
        try {
          if (resp && (resp.status === 401 || resp.status === 403)) {
            var defaultMsg = resp.status === 401 ? DEFAULT_MSG_401 : DEFAULT_MSG_403;
            var contentType = (resp.headers && resp.headers.get && resp.headers.get('content-type')) || '';

            if (contentType.indexOf('application/json') !== -1) {
              // Clone to avoid consuming body for callers
              return resp.clone().json().then(function (data) {
                var msg = (data && (data.message || data.error)) ? (data.message || data.error) : defaultMsg;
                preferToast(msg, 'error');
                return resp; // return the original response unchanged
              }).catch(function () {
                preferToast(defaultMsg, 'error');
                return resp;
              });
            } else {
              preferToast(defaultMsg, 'error');
            }
          }
        } catch (_) { /* ignore toast errors */ }
        return resp;
      });
    };
  }
})();
