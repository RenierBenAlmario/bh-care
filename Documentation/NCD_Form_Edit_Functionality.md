# NCD Form Edit Functionality Documentation

## Overview
The NCD Form Management system now includes an **Edit** button functionality that allows administrators to process uploaded form images and either auto-populate editable forms with extracted data or provide manual entry forms when OCR is not successful.

## Features

### 1. Edit Button
- **Location**: Added to each uploaded image in the "Uploaded Images Management" table
- **Appearance**: Green "Edit" button with edit icon
- **Functionality**: Processes the uploaded form image for data extraction

### 2. OCR Processing Simulation
- **Automatic Detection**: System attempts to read form data from uploaded images
- **Success Rate**: Simulated 70% success rate for demonstration purposes
- **Processing Time**: Simulated 2-second delay to mimic real OCR processing

### 3. Dual Mode Operation

#### Mode 1: Auto-Population (Readable Images)
When the system successfully reads the form:
- **Status**: "Form data extracted successfully! Please review and edit as needed."
- **Action**: Form fields are automatically populated with extracted data
- **User Experience**: Review and modify the pre-filled data as needed

#### Mode 2: Manual Entry (Unreadable Images)
When the system cannot read the form:
- **Status**: "Image not readable by system. Please enter data manually."
- **Action**: Empty form is presented for manual data entry
- **User Experience**: Enter all data manually from the original image

## Technical Implementation

### Frontend Components

#### 1. Edit Button
```html
<button class="btn btn-sm btn-outline-success" onclick="editFormFromImage('@image.FileName', '@image.PageNumber')">
    <i class="fas fa-edit"></i> Edit
</button>
```

#### 2. Modal Dialog
- **Size**: Extra-large modal (90% screen width)
- **Layout**: Side-by-side view of original image and editable form
- **Responsive**: Scrollable content area with max height of 70vh

#### 3. Form Generation
- **Page 1 Form**: Primary information fields (demographics, contact info)
- **Page 2 Form**: Risk assessment fields (medical history, lifestyle factors, measurements)

### Backend Components

#### 1. OCR Processing Handler
```csharp
public async Task<IActionResult> OnPostProcessImageForEditAsync(string fileName, string pageNumber)
```

#### 2. Data Extraction Simulation
```csharp
private async Task<(bool IsReadable, object Data)> ProcessImageWithOCR(string filePath, string pageNumber)
```

#### 3. Form Data Saving
```csharp
public async Task<IActionResult> OnPostSaveFormDataAsync(string fileName, string pageNumber, IFormCollection formData)
```

## Form Fields

### Page 1 - Primary Information
- Health Facility
- Date of Assessment
- Family No.
- ID No.
- First Name
- Middle Name
- Last Name
- Contact Number
- Address
- Barangay
- Birthday
- Age
- Gender
- Religion
- Civil Status
- Occupation

### Page 2 - Risk Assessment
- **Medical History**: Diabetes, Hypertension, Cancer, Lung Disease, COPD, Eye Disease
- **Lifestyle Factors**: Salty Food, Sweet Food, Fatty Food, Alcohol, Smoking, Exercise
- **Risk Screening**: Weight, Height, BMI, Systolic BP, Diastolic BP

## User Workflow

### Step 1: Upload Form Image
1. Navigate to NCD Form Management
2. Upload form image with page number
3. Image appears in "Uploaded Images Management" table

### Step 2: Edit Form Data
1. Click the green "Edit" button for the desired image
2. System processes the image (2-second delay)
3. Modal opens with original image and processing status

### Step 3: Review/Enter Data
1. **If readable**: Review pre-populated data and make corrections
2. **If not readable**: Enter data manually from the original image
3. Modify any fields as needed

### Step 4: Save Data
1. Click "Save Form Data" button
2. System processes and saves the form data
3. Success message displayed
4. Modal closes automatically

## Real-World OCR Integration

### Current Implementation
- **Simulation**: Uses random data generation for demonstration
- **Success Rate**: 70% simulated success rate
- **Processing Time**: 2-second simulated delay

### Production Integration Options

#### 1. Tesseract.NET
```csharp
// Example integration
using Tesseract;

var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
var img = Pix.LoadFromFile(imagePath);
var page = engine.Process(img);
var text = page.GetText();
```

#### 2. Azure Computer Vision API
```csharp
// Example integration
var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(apiKey));
var result = await client.ReadAsync(imageUrl);
```

#### 3. Google Cloud Vision API
```csharp
// Example integration
var client = ImageAnnotatorClient.Create();
var image = Image.FromFile(imagePath);
var response = await client.DetectTextAsync(image);
```

## Configuration

### OCR Settings
- **Processing Timeout**: 30 seconds
- **Image Formats**: JPG, PNG
- **Max File Size**: 5MB
- **Language Support**: English, Filipino (Tagalog)

### Form Validation
- **Required Fields**: All demographic fields
- **Data Types**: Proper validation for numbers, dates, etc.
- **Range Validation**: BMI, blood pressure ranges

## Error Handling

### Common Scenarios
1. **File Not Found**: Clear error message with file path
2. **Processing Timeout**: Graceful fallback to manual entry
3. **Invalid Data**: Form validation with specific error messages
4. **Network Issues**: Retry mechanism with user feedback

### User Feedback
- **Success Messages**: Green alerts with checkmark icons
- **Error Messages**: Red alerts with exclamation icons
- **Info Messages**: Blue alerts with info icons
- **Loading States**: Spinner animations during processing

## Security Considerations

### Access Control
- **Admin Only**: Edit functionality restricted to admin users
- **CSRF Protection**: Anti-forgery tokens on all requests
- **File Validation**: Strict file type and size validation

### Data Privacy
- **Temporary Processing**: OCR data not permanently stored
- **Audit Logging**: All edit actions logged with user and timestamp
- **Data Encryption**: Sensitive data encrypted in transit

## Future Enhancements

### Planned Features
1. **Batch Processing**: Process multiple images simultaneously
2. **Template Matching**: Recognize specific form layouts
3. **Confidence Scoring**: Show OCR confidence levels
4. **Data Validation**: Cross-reference extracted data with database
5. **Export Options**: Export processed data to various formats

### Performance Optimizations
1. **Caching**: Cache OCR results for identical images
2. **Async Processing**: Background processing for large files
3. **Compression**: Optimize image processing for faster OCR
4. **CDN Integration**: Use content delivery networks for image storage

## Troubleshooting

### Common Issues

#### 1. Modal Not Opening
- **Cause**: JavaScript errors or Bootstrap not loaded
- **Solution**: Check browser console for errors, ensure Bootstrap CSS/JS loaded

#### 2. OCR Processing Fails
- **Cause**: Image quality or format issues
- **Solution**: Try different image format or improve image quality

#### 3. Form Data Not Saving
- **Cause**: Network issues or server errors
- **Solution**: Check network connection and server logs

#### 4. Pre-populated Data Incorrect
- **Cause**: OCR misinterpretation
- **Solution**: Manual correction in the form fields

### Debug Information
- **Browser Console**: Check for JavaScript errors
- **Network Tab**: Monitor API requests and responses
- **Server Logs**: Check application logs for backend errors

## Support

### Documentation
- **API Reference**: Detailed endpoint documentation
- **User Guide**: Step-by-step user instructions
- **Developer Guide**: Technical implementation details

### Contact
- **Technical Support**: admin@barangay161.ph
- **Bug Reports**: Use the system's issue tracking
- **Feature Requests**: Submit through admin dashboard

---

*Last Updated: December 2024*
*Version: 1.0*
*Author: Barangay Health Care System Development Team*
