// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Initialize dark mode on page load
(function() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark' || (savedTheme === null && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
        document.documentElement.classList.add('dark-mode');
        document.body.classList.add('dark-mode');
    }
})();

$(document).ready(function () {
    // Initialize dark mode toggle
    initializeDarkMode();
    
    // Sidebar toggle
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
        $('#content').toggleClass('active');
    });

    // Close sidebar on mobile when clicking outside
    $(document).on('click', function (e) {
        if ($(window).width() <= 768) {
            if (!$(e.target).closest('#sidebar').length && !$(e.target).closest('#sidebarCollapse').length) {
                $('#sidebar').addClass('active');
                $('#content').addClass('active');
            }
        }
    });

    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Initialize popovers
    $('[data-toggle="popover"]').popover();

    // Handle appointment date change
    $('#date').on('change', function() {
        const selectedDate = $(this).val();
        if (selectedDate) {
            // Clear existing options
            $('#doctorId').empty().append('<option value="">Loading doctors...</option>');
            
            // Fetch available doctors
            fetch(`/api/doctors/available?date=${selectedDate}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Failed to load doctors');
                    }
                    return response.json();
                })
                .then(data => {
                    // Clear loading option
                    $('#doctorId').empty().append('<option value="">Select doctor</option>');
                    
                    // Add doctor options
                    if (data.doctors && data.doctors.length > 0) {
                        data.doctors.forEach(doctor => {
                            $('#doctorId').append(`<option value="${doctor.id}">${doctor.name} (${doctor.availableSlots} slots available)</option>`);
                        });
                        // Enable the select element
                        $('#doctorId').prop('disabled', false);
                    } else {
                        $('#doctorId').append('<option value="">No doctors available for this date</option>');
                        $('#doctorId').prop('disabled', true);
                    }
                })
                .catch(error => {
                    console.error('Error loading doctors:', error);
                    $('#doctorId').empty().append('<option value="">Failed to load available doctors</option>');
                    $('#doctorId').prop('disabled', true);
                });
        } else {
            // Clear doctor options if no date selected
            $('#doctorId').empty().append('<option value="">Select doctor</option>');
            $('#doctorId').prop('disabled', true);
        }
    });

    // Handle form validation
    $('form').on('submit', function () {
        $(this).find(':input').filter(function () {
            return !this.value;
        }).closest('.form-group').addClass('has-error');
    });

    // Remove error class on input
    $('input').on('change', function () {
        $(this).closest('.form-group').removeClass('has-error');
    });

    // Handle table row hover
    $('.table tbody tr').hover(
        function () {
            $(this).addClass('table-hover');
        },
        function () {
            $(this).removeClass('table-hover');
        }
    );

    // Handle alert dismissal
    $('.alert-dismissible .close').on('click', function () {
        $(this).closest('.alert').fadeOut();
    });

    // Handle dropdown hover
    $('.dropdown').hover(
        function () {
            $(this).find('.dropdown-menu').first().stop(true, true).delay(250).slideDown();
        },
        function () {
            $(this).find('.dropdown-menu').first().stop(true, true).delay(100).slideUp();
        }
    );

    // Handle card hover
    $('.card').hover(
        function () {
            $(this).addClass('shadow-lg');
        },
        function () {
            $(this).removeClass('shadow-lg');
        }
    );

    // Handle scroll to top
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('#scrollToTop').fadeIn();
        } else {
            $('#scrollToTop').fadeOut();
        }
    });

    // Handle scroll to top button click
    $('#scrollToTop').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 800);
        return false;
    });

    // Handle active navigation
    var currentPage = window.location.pathname;
    $('.nav-link').each(function () {
        if ($(this).attr('href') === currentPage) {
            $(this).addClass('active');
        }
    });

    // Handle responsive tables
    $('.table-responsive').each(function () {
        $(this).wrap('<div class="table-responsive-wrapper"></div>');
    });

    // Handle form autosave
    var autosaveTimeout;
    $('form[data-autosave]').on('change', 'input, select, textarea', function () {
        clearTimeout(autosaveTimeout);
        autosaveTimeout = setTimeout(function () {
            $('form[data-autosave]').submit();
        }, 1000);
    });

    // Handle loading states
    $('form').on('submit', function () {
        $(this).find('button[type="submit"]').prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Loading...');
    });

    // Handle doctor availability toggle
    $('#availabilityToggle').on('change', function() {
        const isAvailable = $(this).prop('checked');
        
        fetch('/Doctor/DoctorDashboard?handler=UpdateAvailability', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(isAvailable)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showToast('Status updated successfully', 'success');
            } else {
                showToast('Failed to update status', 'error');
                // Revert toggle if update failed
                $(this).prop('checked', !isAvailable);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Failed to update status', 'error');
            // Revert toggle if update failed
            $(this).prop('checked', !isAvailable);
        });
    });

    // Handle working days update
    $('input[name="workingDays"]').on('change', function() {
        const selectedDays = $('input[name="workingDays"]:checked')
            .map(function() { return this.value; })
            .get()
            .join(',');
        
        fetch('/Doctor/DoctorDashboard?handler=UpdateWorkingDays', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(selectedDays)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showToast('Working days updated successfully', 'success');
            } else {
                showToast('Failed to update working days', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Failed to update working days', 'error');
        });
    });

    // Handle working hours update
    function updateWorkingHours() {
        const start = $('#workingHoursStart').val();
        const end = $('#workingHoursEnd').val();
        
        if (!start || !end) return;
        
        fetch('/Doctor/DoctorDashboard?handler=UpdateWorkingHours', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify({
                workingHoursStart: start,
                workingHoursEnd: end
            })
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showToast('Working hours updated successfully', 'success');
            } else {
                showToast(data.message || 'Failed to update working hours', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Failed to update working hours', 'error');
        });
    }

    $('#workingHoursStart, #workingHoursEnd').on('change', updateWorkingHours);

    // Handle max patients update
    $('#maxDailyPatients').on('change', function() {
        const maxPatients = parseInt($(this).val());
        if (isNaN(maxPatients) || maxPatients < 0) return;
        
        fetch('/Doctor/DoctorDashboard?handler=UpdateMaxPatients', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(maxPatients)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showToast('Maximum daily patients updated successfully', 'success');
            } else {
                showToast('Failed to update maximum daily patients', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Failed to update maximum daily patients', 'error');
        });
    });

    // Add toast notification functions
    function showToast(message, type = 'info') {
        // Check if the toastr library is available (preferred)
        if (typeof toastr !== 'undefined') {
            switch(type) {
                case 'success':
                    toastr.success(message);
                    break;
                case 'error':
                    toastr.error(message);
                    break;
                case 'warning':
                    toastr.warning(message);
                    break;
                default:
                    toastr.info(message);
            }
            return;
        }
        
        // Fallback to bootstrap toast if available
        if (typeof bootstrap !== 'undefined') {
            // Check if toast container exists, if not create it
            let toastContainer = document.querySelector('.toast-container');
            if (!toastContainer) {
                toastContainer = document.createElement('div');
                toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
                document.body.appendChild(toastContainer);
            }
            
            // Create toast element
            const toastEl = document.createElement('div');
            toastEl.className = `toast align-items-center text-white bg-${type === 'error' ? 'danger' : 
                                                                        type === 'success' ? 'success' : 
                                                                        type === 'warning' ? 'warning' : 'info'}`;
            toastEl.setAttribute('role', 'alert');
            toastEl.setAttribute('aria-live', 'assertive');
            toastEl.setAttribute('aria-atomic', 'true');
            
            toastEl.innerHTML = `
                <div class="d-flex">
                    <div class="toast-body">
                        ${message}
                    </div>
                    <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            `;
            
            toastContainer.appendChild(toastEl);
            
            // Initialize and show the toast
            const toast = new bootstrap.Toast(toastEl, { autohide: true, delay: 5000 });
            toast.show();
            
            // Remove toast from DOM after it's hidden
            toastEl.addEventListener('hidden.bs.toast', () => {
                toastEl.remove();
            });
        } else {
            // Basic fallback
            alert(message);
        }
    }

    // Helper function to format vital signs for display
    function formatVitalSigns(vitalSigns) {
        let result = '';
        
        if (vitalSigns.bloodPressure) 
            result += `Blood Pressure: ${vitalSigns.bloodPressure}\n`;
        
        if (vitalSigns.heartRate) 
            result += `Heart Rate: ${vitalSigns.heartRate} bpm\n`;
        
        if (vitalSigns.temperature) 
            result += `Temperature: ${vitalSigns.temperature}°C\n`;
        
        if (vitalSigns.respiratoryRate) 
            result += `Respiratory Rate: ${vitalSigns.respiratoryRate}/min\n`;
        
        if (vitalSigns.spo2) 
            result += `SpO2: ${vitalSigns.spo2}%\n`;
        
        if (vitalSigns.weight) 
            result += `Weight: ${vitalSigns.weight} kg\n`;
        
        if (vitalSigns.height) 
            result += `Height: ${vitalSigns.height} cm\n`;
        
        return result || 'No vital signs recorded';
    }

    // Initialize on document ready
    document.addEventListener('DOMContentLoaded', function() {
        // Handle active navigation
        const currentPage = window.location.pathname;
        document.querySelectorAll('.nav-link').forEach(function(navLink) {
            if (navLink.getAttribute('href') === currentPage) {
                navLink.classList.add('active');
            }
        });
        
        // Toggle password visibility
        const togglePasswordBtns = document.querySelectorAll('.toggle-password');
        togglePasswordBtns.forEach(function(btn) {
            btn.addEventListener('click', function() {
                const targetId = this.getAttribute('data-target') || 'Password';
                const passwordInput = document.getElementById(targetId);
                
                if (passwordInput) {
                    const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
                    passwordInput.setAttribute('type', type);
                    
                    const icon = this.querySelector('i');
                    if (icon) {
                        icon.classList.toggle('fa-eye');
                        icon.classList.toggle('fa-eye-slash');
                    }
                    
                    this.classList.toggle('active');
                }
            });
        });
        
        // Handle navbar collapse on mobile
        const navbarToggler = document.querySelector('.navbar-toggler');
        if (navbarToggler) {
            navbarToggler.addEventListener('click', function() {
                const target = document.querySelector(this.getAttribute('data-bs-target'));
                if (target) {
                    target.classList.toggle('show');
                }
            });
            
            // Close navbar when clicking outside on mobile
            document.addEventListener('click', function(event) {
                const navbar = document.querySelector('.navbar-collapse.show');
                if (navbar && !navbar.contains(event.target) && !navbarToggler.contains(event.target)) {
                    navbar.classList.remove('show');
                }
            });
        }
        
        // Handle form loading states
        document.querySelectorAll('form').forEach(function(form) {
            form.addEventListener('submit', function() {
                const submitBtn = this.querySelector('button[type="submit"]');
                if (submitBtn) {
                    submitBtn.disabled = true;
                    if (!submitBtn.innerHTML.includes('spinner')) {
                        const originalText = submitBtn.innerHTML;
                        submitBtn.setAttribute('data-original-text', originalText);
                        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span> Loading...';
                    }
                    
                    // Reset button after timeout (in case form submission fails)
                    setTimeout(function() {
                        if (submitBtn.disabled && submitBtn.getAttribute('data-original-text')) {
                            submitBtn.disabled = false;
                            submitBtn.innerHTML = submitBtn.getAttribute('data-original-text');
                        }
                    }, 10000);
                }
            });
        });

        // Initialize dropdowns
        var dropdownElementList = [].slice.call(document.querySelectorAll('.dropdown-toggle'));
        var dropdownList = dropdownElementList.map(function(dropdownToggleEl) {
            return new bootstrap.Dropdown(dropdownToggleEl);
        });

        // Make dropdowns work on click and hover
        var dropdownItems = document.querySelectorAll('.dropdown');
        dropdownItems.forEach(function(dropdown) {
            dropdown.addEventListener('click', function(e) {
                var dropdownMenu = this.querySelector('.dropdown-menu');
                if (dropdownMenu) {
                    if (dropdownMenu.classList.contains('show')) {
                        dropdownMenu.classList.remove('show');
                    } else {
                        // Close other open dropdowns
                        document.querySelectorAll('.dropdown-menu.show').forEach(function(menu) {
                            menu.classList.remove('show');
                        });
                        dropdownMenu.classList.add('show');
                    }
                }
            });
            
            // Close dropdown when clicking outside
            document.addEventListener('click', function(e) {
                if (!dropdown.contains(e.target)) {
                    var dropdownMenu = dropdown.querySelector('.dropdown-menu');
                    if (dropdownMenu && dropdownMenu.classList.contains('show')) {
                        dropdownMenu.classList.remove('show');
                    }
                }
            });
        });
    });

    // Global dark mode function for all pages
    function initializeDarkMode() {
        // Check for existing toggle or create one if on a page without it
        let themeToggle = document.getElementById('theme-toggle');
        
        // If we're not on a page with a theme-toggle already (like admin dashboard)
        if (!themeToggle && !document.querySelector('.theme-toggle')) {
            // Create a floating theme toggle button for all pages
            const toggleBtn = document.createElement('button');
            toggleBtn.id = 'global-theme-toggle';
            toggleBtn.className = 'global-theme-toggle';
            toggleBtn.innerHTML = '<i class="fas fa-moon"></i>';
            toggleBtn.setAttribute('aria-label', 'Toggle dark mode');
            toggleBtn.setAttribute('title', 'Toggle dark mode');
            document.body.appendChild(toggleBtn);
            
            themeToggle = toggleBtn;
        }
        
        // Use either the existing toggle or our new global one
        const toggleElement = themeToggle || document.querySelector('.theme-toggle');
        
        if (toggleElement) {
            // Get icon element within the toggle
            const icon = toggleElement.querySelector('i');
            
            // Update the icon based on current theme
            const isDarkMode = document.documentElement.classList.contains('dark-mode');
            if (icon) {
                if (isDarkMode) {
                    icon.className = 'fas fa-sun';
                } else {
                    icon.className = 'fas fa-moon';
                }
            }
            
            // Add click event listener
            toggleElement.addEventListener('click', function(e) {
                e.preventDefault();
                
                // Toggle dark mode class on both document element and body
                document.documentElement.classList.toggle('dark-mode');
                document.body.classList.toggle('dark-mode');
                
                // Get current state
                const isDarkMode = document.documentElement.classList.contains('dark-mode');
                
                // Update toggle button icon
                if (icon) {
                    if (isDarkMode) {
                        icon.className = 'fas fa-sun';
                    } else {
                        icon.className = 'fas fa-moon';
                    }
                }
                
                // Save preference
                localStorage.setItem('theme', isDarkMode ? 'dark' : 'light');
                
                // Update charts if they exist (for dashboard pages)
                if (window.updateChartsTheme) {
                    window.updateChartsTheme(isDarkMode);
                }
            });
        }
        
        // Listen for system theme changes
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', event => {
            // Only apply system preference if user hasn't manually chosen a theme
            if (localStorage.getItem('theme') === null) {
                const isDarkMode = event.matches;
                document.documentElement.classList.toggle('dark-mode', isDarkMode);
                document.body.classList.toggle('dark-mode', isDarkMode);
                
                // Update toggle button icon if it exists
                const icon = toggleElement ? toggleElement.querySelector('i') : null;
                if (icon) {
                    icon.className = isDarkMode ? 'fas fa-sun' : 'fas fa-moon';
                }
                
                // Update charts if they exist
                if (window.updateChartsTheme) {
                    window.updateChartsTheme(isDarkMode);
                }
            }
        });
    }
});
