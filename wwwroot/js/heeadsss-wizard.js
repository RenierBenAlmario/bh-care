$(document).ready(function () {
    var $steps = $('.wizard-step');
    var $form = $('#heeadsssForm');
    var currentStep = 1;
    var totalSteps = $steps.length;

    function showStep(step) {
        $steps.hide();
        $steps.filter('[data-step="' + step + '"]').show();
        updateStepper(step);
    }

    function updateStepper(step) {
        var $stepper = $('#wizard-stepper');
        var html = '';
        for (var i = 1; i <= totalSteps; i++) {
            html += '<span class="wizard-dot' + (i === step ? ' active' : '') + '">' + i + '</span>';
        }
        $stepper.html(html);
    }

    function validateStep(step) {
        var $current = $steps.filter('[data-step="' + step + '"]');
        var valid = true;
        $current.find('[required]').each(function () {
            if (!$(this).val()) {
                $(this).addClass('is-invalid');
                valid = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        return valid;
    }

    $form.on('click', '.next-step', function () {
        if (!validateStep(currentStep)) return;
        if (currentStep < totalSteps) {
            currentStep++;
            showStep(currentStep);
        }
    });

    $form.on('click', '.prev-step', function () {
        if (currentStep > 1) {
            currentStep--;
            showStep(currentStep);
        }
    });

    $form.on('submit', function (e) {
        if (currentStep !== totalSteps) {
            e.preventDefault();
            if (validateStep(currentStep)) {
                currentStep++;
                showStep(currentStep);
            }
        }
    });

    // Style for stepper dots
    var style = document.createElement('style');
    style.innerHTML = '.wizard-dot { display: inline-block; width: 28px; height: 28px; line-height: 28px; border-radius: 50%; background: #e0e0e0; color: #333; text-align: center; margin: 0 4px; font-weight: bold; font-size: 16px; } .wizard-dot.active { background: #007bff; color: #fff; }';
    document.head.appendChild(style);

    showStep(currentStep);
});
