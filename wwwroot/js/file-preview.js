/**
 * File Preview Handler
 * Provides functionality for previewing PDF and image files
 */

document.addEventListener('DOMContentLoaded', function() {
    // Initialize file preview modal
    initFilePreview();
});

/**
 * Initialize the file preview functionality
 */
function initFilePreview() {
    // Find all file preview buttons
    const previewButtons = document.querySelectorAll('.document-preview-btn');
    
    // Check if we're using Bootstrap modal or need to create our own
    let useBootstrapModal = document.getElementById('filePreviewModal') && typeof bootstrap !== 'undefined';
    let previewModal;
    
    if (useBootstrapModal) {
        console.log('Using existing Bootstrap modal');
        previewModal = document.getElementById('filePreviewModal');
    } else {
        console.log('Creating custom modal');
        previewModal = createFilePreviewModal();
        document.body.appendChild(previewModal);
    }
    
    // Add click event to all preview buttons
    previewButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const filename = button.getAttribute('data-filename');
            let filepath = button.getAttribute('data-filepath') || filename;
            const fileType = button.getAttribute('data-type') || determineFileType(filename);
            
            // Ensure filepath is correct for residency proofs
            if (filepath && !filepath.startsWith('http')) {
                // Make sure the path starts with a slash
                if (!filepath.startsWith('/')) {
                    filepath = '/' + filepath;
                }
                
                // Ensure we're loading from the residency_proofs directory
                if (!filepath.includes('/uploads/residency_proofs/') && !filepath.includes('/uploads/guardian_proofs/')) {
                    console.warn('Filepath does not point to a valid residency proof directory:', filepath);
                    // Try to correct the path if it contains a valid filename
                    if (filename) {
                        filepath = '/uploads/residency_proofs/' + filename;
                    }
                }
            }
            
            console.log('Opening file preview:', { filename, filepath, fileType });
            
            if (useBootstrapModal) {
                openBootstrapFilePreview(filename, filepath, fileType);
            } else {
                openFilePreview(filename, filepath, fileType);
            }
        });
    });
    
    // Handle close events for custom modal
    if (!useBootstrapModal) {
        // Close modal when clicking the close button
        const closeButton = document.querySelector('.file-preview-close');
        if (closeButton) {
            closeButton.addEventListener('click', closeFilePreview);
        }
        
        // Close modal when clicking outside the content
        previewModal.addEventListener('click', function(e) {
            if (e.target === previewModal) {
                closeFilePreview();
            }
        });
        
        // Close modal with escape key
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape' && previewModal.classList.contains('show')) {
                closeFilePreview();
            }
        });
    }
}

/**
 * Create the file preview modal DOM structure
 */
function createFilePreviewModal() {
    const modal = document.createElement('div');
    modal.id = 'filePreviewModal';
    modal.className = 'file-preview-modal';
    
    modal.innerHTML = `
        <div class="file-preview-container">
            <div class="file-preview-header">
                <h3 class="file-preview-title">Document Preview</h3>
                <button type="button" class="file-preview-close" aria-label="Close">×</button>
            </div>
            <div class="file-preview-body">
                <div class="file-preview-content"></div>
                <div class="file-preview-loading">Loading document...</div>
                <div class="file-preview-error" style="display: none;">
                    <p>Failed to load document</p>
                    <button class="btn btn-primary reload-preview">Retry</button>
                </div>
            </div>
            <div class="file-preview-footer">
                <div class="file-info"></div>
                <div class="file-actions">
                    <a href="#" class="btn btn-primary download-file" target="_blank">Download</a>
                    <a href="#" class="btn btn-secondary open-file" target="_blank">Open in New Tab</a>
                </div>
            </div>
        </div>
    `;
    
    return modal;
}

/**
 * Open file preview modal and load the file
 */
function openFilePreview(filename, filepath, fileType) {
    const modal = document.getElementById('filePreviewModal');
    const contentDiv = modal.querySelector('.file-preview-content');
    const loadingDiv = modal.querySelector('.file-preview-loading');
    const errorDiv = modal.querySelector('.file-preview-error');
    const titleElement = modal.querySelector('.file-preview-title');
    const fileInfoDiv = modal.querySelector('.file-info');
    const downloadLink = modal.querySelector('.download-file');
    const openLink = modal.querySelector('.open-file');
    
    // Update title and reset content
    titleElement.textContent = filename;
    contentDiv.innerHTML = '';
    errorDiv.style.display = 'none';
    loadingDiv.style.display = 'block';
    
    // Show the modal
    modal.classList.add('show');
    
    // Set download and open links
    downloadLink.href = filepath;
    openLink.href = filepath;
    
    // Get file information
    fileInfoDiv.innerHTML = `<p>File Name: ${filename}</p>`;
    
    // Load the file content
    loadFileContent(filepath, fileType, contentDiv, loadingDiv, errorDiv);
    
    // Add event listener to the retry button
    const retryButton = modal.querySelector('.reload-preview');
    retryButton.addEventListener('click', function() {
        errorDiv.style.display = 'none';
        loadingDiv.style.display = 'block';
        loadFileContent(filepath, fileType, contentDiv, loadingDiv, errorDiv);
    });
}

/**
 * Load file content based on its type
 */
function loadFileContent(filepath, fileType, contentDiv, loadingDiv, errorDiv) {
    console.log('Loading file content:', { filepath, fileType });
    
    // Validate the file path
    if (!filepath || typeof filepath !== 'string') {
        console.error('Invalid file path:', filepath);
        errorDiv.style.display = 'block';
        loadingDiv.style.display = 'none';
        return;
    }
    
    // For images and PDFs that can be directly loaded
    if (fileType === 'pdf') {
        // Create PDF viewer
        const pdfViewer = document.createElement('iframe');
        pdfViewer.className = 'pdf-viewer';
        pdfViewer.src = filepath;
        contentDiv.appendChild(pdfViewer);
        
        // Add event listener for iframe load events
        pdfViewer.addEventListener('load', () => {
            console.log('PDF loaded successfully:', filepath);
            loadingDiv.style.display = 'none';
        });
        
        pdfViewer.addEventListener('error', (e) => {
            console.error('Error loading PDF:', e);
            errorDiv.style.display = 'block';
            loadingDiv.style.display = 'none';
        });
    } else if (fileType === 'image') {
        // Create image viewer
        const imgViewer = document.createElement('img');
        imgViewer.className = 'img-viewer';
        imgViewer.src = filepath;
        contentDiv.appendChild(imgViewer);
        
        // Add event listener for image load events
        imgViewer.addEventListener('load', () => {
            console.log('Image loaded successfully:', filepath);
            loadingDiv.style.display = 'none';
        });
        
        imgViewer.addEventListener('error', (e) => {
            console.error('Error loading image:', e);
            errorDiv.style.display = 'block';
            loadingDiv.style.display = 'none';
        });
    } else {
        // For other files, show download link
        contentDiv.innerHTML = `
            <div class="unsupported-file">
                <i class="fas fa-file fa-5x"></i>
                <p>This file type cannot be previewed</p>
                <p>Please download the file to view it</p>
            </div>
        `;
        loadingDiv.style.display = 'none';
    }
}

/**
 * Close the file preview modal
 */
function closeFilePreview() {
    const modal = document.getElementById('filePreviewModal');
    modal.classList.remove('show');
    
    // Clean up object URLs to prevent memory leaks
    const contentDiv = modal.querySelector('.file-preview-content');
    const iframes = contentDiv.querySelectorAll('iframe');
    const images = contentDiv.querySelectorAll('img');
    
    iframes.forEach(iframe => {
        const src = iframe.src;
        if (src.startsWith('blob:')) {
            URL.revokeObjectURL(src);
        }
    });
    
    images.forEach(img => {
        const src = img.src;
        if (src.startsWith('blob:')) {
            URL.revokeObjectURL(src);
        }
    });
}

/**
 * Determine file type from filename
 */
function determineFileType(filename) {
    const extension = filename.split('.').pop().toLowerCase();
    
    if (['pdf'].includes(extension)) {
        return 'pdf';
    } else if (['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'].includes(extension)) {
        return 'image';
    } else {
        return 'other';
    }
}

/**
 * Create file preview buttons for all document links on the page
 */
function createFilePreviewButtons() {
    // Find all links to documents and convert them to preview buttons
    document.querySelectorAll('a[href$=".pdf"], a[href$=".jpg"], a[href$=".jpeg"], a[href$=".png"]').forEach(link => {
        // Skip if already processed
        if (link.classList.contains('document-preview-btn')) {
            return;
        }
        
        // Get filename from href
        const href = link.getAttribute('href');
        const filename = href.split('/').pop();
        
        // Add preview class and attributes
        link.classList.add('document-preview-btn');
        link.setAttribute('data-filename', filename);
        link.setAttribute('data-type', determineFileType(filename));
        
        // Replace click event
        link.addEventListener('click', function(e) {
            e.preventDefault();
            openFilePreview(filename, determineFileType(filename));
        });
    });
}

/**
 * Open Bootstrap file preview modal and load the file
 */
function openBootstrapFilePreview(filename, filepath, fileType) {
    const modal = document.getElementById('filePreviewModal');
    const contentDiv = modal.querySelector('#filePreviewContent');
    const downloadBtn = modal.querySelector('#downloadDocumentBtn');
    
    // Set download link
    if (downloadBtn) {
        downloadBtn.href = filepath;
        downloadBtn.setAttribute('download', filename);
    }
    
    // Clear previous content and show loading indicator
    contentDiv.innerHTML = `
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
    `;
    
    // Show the modal using Bootstrap
    const bsModal = new bootstrap.Modal(modal);
    bsModal.show();
    
    // Load content based on file type
    setTimeout(() => {
        if (fileType === 'pdf') {
            contentDiv.innerHTML = `
                <div class="pdf-container">
                    <iframe src="${filepath}" width="100%" height="500" frameborder="0"></iframe>
                </div>
            `;
        } else if (fileType === 'image') {
            contentDiv.innerHTML = `
                <div class="image-container">
                    <img src="${filepath}" alt="${filename}" class="img-fluid" />
                </div>
            `;
        } else {
            contentDiv.innerHTML = `
                <div class="unsupported-file">
                    <i class="fas fa-file fa-5x"></i>
                    <p>This file type cannot be previewed</p>
                    <p>Please download the file to view it</p>
                </div>
            `;
        }
    }, 500);
} 