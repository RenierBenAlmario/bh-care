# Admin-Only NCD Form Images Directory

This directory contains NCD Risk Assessment Form images that are managed exclusively by system administrators.

## Purpose

- **Admin-Only Access**: Only users with admin privileges can upload, manage, and delete images in this directory
- **Version Control**: Multiple versions of form images can be stored with timestamps
- **Active Image Management**: Administrators can set which image version is currently active for users
- **Complete Separation**: This directory is completely separate from user-accessible form images

## File Naming Convention

- `ncd-form-page1-YYYYMMDDHHMMSS.jpg` - Page 1 images with timestamp
- `ncd-form-page2-YYYYMMDDHHMMSS.jpg` - Page 2 images with timestamp

## Security

- All operations require `RequireAdminRole` policy authorization
- Files are stored separately from user-accessible images
- Admin actions are logged for audit purposes

## Usage

1. Upload new form images through the Admin Dashboard → NCD Form Management
2. Set active images that will be displayed to users
3. Manage multiple versions of form images
4. Delete outdated or incorrect images

## Related Files

- Active images (displayed to users): `/images/forms/ncd-form-page1.jpg`, `/images/forms/ncd-form-page2.jpg`
- Admin management page: `/Admin/NCDFormManagement`
- Admin authorization: `RequireAdminRole` policy
