// Admin Utilities for BHCARE
$(document).ready(function() {
    // Handle NCD form image uploads if the upload control exists
    if ($('#ncdFormImageUploader').length) {
        setupNCDFormImageUploader();
    }
    
    // Handle HEEADSSS form image uploads if the upload control exists
    if ($('#heeadsssFormImageUploader').length) {
        setupHEEADSSSFormImageUploader();
    }
});

/**
 * Sets up the NCD Form Image uploader functionality
 */
function setupNCDFormImageUploader() {
    const fileInput = $('#ncdFormImageUploader');
    const pageSelector = $('#ncdFormPageSelector');
    const previewArea = $('#ncdFormImagePreview');
    const uploadButton = $('#uploadNcdFormImage');
    const statusMessage = $('#ncdFormUploadStatus');
    
    // Handle file selection for preview
    fileInput.on('change', function() {
        const file = this.files[0];
        if (file) {
            // Validate file type
            if (!file.type.match('image/jpeg') && !file.type.match('image/png')) {
                statusMessage.html('<div class="alert alert-danger">Please select a JPG or PNG image file.</div>');
                previewArea.html('');
                uploadButton.prop('disabled', true);
                return;
            }
            
            // Validate file size (max 5MB)
            if (file.size > 5 * 1024 * 1024) {
                statusMessage.html('<div class="alert alert-danger">File is too large. Maximum size is 5MB.</div>');
                previewArea.html('');
                uploadButton.prop('disabled', true);
                return;
            }
            
            // Preview the image
            const reader = new FileReader();
            reader.onload = function(e) {
                previewArea.html(`<img src="${e.target.result}" class="img-fluid border rounded">`);
                uploadButton.prop('disabled', false);
                // Also enable OCR button if it exists
                $('#extractTextFromImage').prop('disabled', false);
                statusMessage.html('');
            }
            reader.readAsDataURL(file);
        }
    });
    
    // Handle form upload
    uploadButton.on('click', function() {
        const file = fileInput[0].files[0];
        const page = pageSelector.val();
        if (!file || !page) {
            statusMessage.html('<div class="alert alert-warning">Please select both a file and page number.</div>');
            return;
        }
        
        // Create form data for upload
        const formData = new FormData();
        formData.append('file', file);
        formData.append('page', page);
        
        // Show loading state
        uploadButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Uploading...');
        
        // Send to server
        $.ajax({
            url: '/Admin/UploadNCDFormImage',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function(response) {
                if (response.success) {
                    statusMessage.html(`<div class="alert alert-success">
                        <i class="fas fa-check-circle me-2"></i> Successfully uploaded image for page ${page}.
                        <a href="${response.fileUrl}" target="_blank" class="ms-2">View File</a>
                    </div>`);
                    
                    // Reset form after successful upload
                    fileInput.val('');
                    previewArea.html('');
                } else {
                    statusMessage.html(`<div class="alert alert-danger">
                        <i class="fas fa-exclamation-circle me-2"></i> ${response.message}
                    </div>`);
                }
            },
            error: function() {
                statusMessage.html(`<div class="alert alert-danger">
                    <i class="fas fa-exclamation-circle me-2"></i> Server error occurred. Please try again.
                </div>`);
            },
            complete: function() {
                uploadButton.prop('disabled', false).html('<i class="fas fa-upload me-1"></i> Upload');
            }
        });
    });
}

/**
 * Sets up the HEEADSSS Form Image uploader functionality
 */
function setupHEEADSSSFormImageUploader() {
    const fileInput = $('#heeadsssFormImageUploader');
    const pageSelector = $('#heeadsssFormPageSelector');
    const previewArea = $('#heeadsssFormImagePreview');
    const uploadButton = $('#uploadHeeadsssFormImage');
    const statusMessage = $('#heeadsssFormUploadStatus');
    
    // Handle file selection for preview
    fileInput.on('change', function() {
        const file = this.files[0];
        if (file) {
            // Validate file type
            if (!file.type.match('image/jpeg') && !file.type.match('image/png')) {
                statusMessage.html('<div class="alert alert-danger">Please select a JPG or PNG image file.</div>');
                previewArea.html('');
                uploadButton.prop('disabled', true);
                return;
            }
            
            // Validate file size (max 5MB)
            if (file.size > 5 * 1024 * 1024) {
                statusMessage.html('<div class="alert alert-danger">File is too large. Maximum size is 5MB.</div>');
                previewArea.html('');
                uploadButton.prop('disabled', true);
                return;
            }
            
            // Preview the image
            const reader = new FileReader();
            reader.onload = function(e) {
                previewArea.html(`<img src="${e.target.result}" class="img-fluid border rounded">`);
                uploadButton.prop('disabled', false);
                // Also enable OCR button if it exists
                $('#extractTextFromHeeadsssImage').prop('disabled', false);
                statusMessage.html('');
            }
            reader.readAsDataURL(file);
        }
    });
    
    // Handle form upload
    uploadButton.on('click', function() {
        const file = fileInput[0].files[0];
        const page = pageSelector.val();
        if (!file || !page) {
            statusMessage.html('<div class="alert alert-warning">Please select both a file and page number.</div>');
            return;
        }
        
        // Create form data for upload
        const formData = new FormData();
        formData.append('file', file);
        formData.append('page', page);
        
        // Show loading state
        uploadButton.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Uploading...');
        
        // Send to server
        $.ajax({
            url: '/Admin/UploadHEEADSSSFormImage',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function(response) {
                if (response.success) {
                    statusMessage.html(`<div class="alert alert-success">
                        <i class="fas fa-check-circle me-2"></i> Successfully uploaded image for page ${page}.
                        <a href="${response.fileUrl}" target="_blank" class="ms-2">View File</a>
                    </div>`);
                    
                    // Reset form after successful upload
                    fileInput.val('');
                    previewArea.html('');
                } else {
                    statusMessage.html(`<div class="alert alert-danger">
                        <i class="fas fa-exclamation-circle me-2"></i> ${response.message}
                    </div>`);
                }
            },
            error: function() {
                statusMessage.html(`<div class="alert alert-danger">
                    <i class="fas fa-exclamation-circle me-2"></i> Server error occurred. Please try again.
                </div>`);
            },
            complete: function() {
                uploadButton.prop('disabled', false).html('<i class="fas fa-upload me-1"></i> Upload');
            }
        });
    });
} 