# Change log

## 2.6.0

### Fixes

* Fixed display of images when flipping
* The clickable area size for the copy links button has been increased.
* Changed scrollbar color for dark theme
* Fixed display of history from the previous room
* Fixed authorization via ADFS server
* Fixed display of info panel after changing user type
* Fixed display of the document creation menu in the horizontal position of mobile devices
* Fixed the description of the role in the room for the owner
* Fixed account selection when clicking on name
* Fixed browser backspace navigation
* MySQL Server updated to version 8.0.37
* Added user type description to profile
* Fixed file selection in horizontal position on mobile devices
* Fixed deletion of files on the server after deleting a user from the portal
* Fixed opening PDF on preview. A copy of the file is no longer created
* Added automatic installation of docker on RedHat
* Fixed display of error about insufficient rights to view the room
* Fixed changing user type on the invite panel
* Hidden status line in user info for server version
* The elasticsearch service has been removed from startup
* Fixed following links from notifications under an authorized user
* Added a record to the room history when deleting an external link
* Added a link to the portal in the topic of notifications What's New
* Added field names for brute force protection settings
* Fixed transition inside a group by Enter
* Fixed low logo resolution for "use as logo" setting
* Fixed filter when viewing user's room list
* Fixed rewinding of media files on the server version
* Fixed restore process for server version
* Fixed sorting when navigating through folders.
* Fixed embedding of password protected documents
* Fixed "rate limit" for simultaneous upload of multiple large files
* Fixed modification date when editing documents
* Added a record to the history of the destination folder about moving/copying a file to it
* Fixed page scrolling when selecting elements and drag'n'drop
* Fixed room history entry about adding existing tags
* Added a record to the audit trail about changing quotas
* Added a button to return to the general list of presets JS SDK
* Fixed saving empty company name in branding
* Fixed the appearance of authorization buttons via third-party services on the login page
* Added confirmation window for restoring LDAP settings
* Fixed default port changing when switching to ssl for LDAP settings
* Fixed authorization via ldap server
* Fixed disabling of "Feedback & Support" and "Show link to Help Center" links in branding
* Fixed CSP error for plugins on the server version
* Fixed quota change on mobile devices
* Fixed work with document server when using spaces
* Fixed license reset when restoring one of the space portals
* Fixed access to the room after filling out the embedded form
* Fixed license reset when restoring one of the spaces portals
* Fixed lock of file opened for editing in inactive browser tab
* Fixed connection of Sharepoint via webdav to public room
* Fixed installation of docker version on custom port
* Fixed installation of docker version without documentserver
* Fixed sorting set in compact view mode
* Fixed element selection when clicking on a type
* Fixed restore from backup list
* Fixed account activation when registering via social networks
* Fixed embedding of PDF form (filling was not available)

## 2.5.0

### Fixes

* Fixed downloading files from the public room
* Added content search for PDF files
* Fixed saving SSO settings from drop-down lists
* Fixed blocking of document editing when opening the save/share panel, and closing them by esc
* Fixed adding an existing tag for rooms
* Fixed search in Chinese (by content too)
* Fixed display of the Branding section when mobile devices are in a vertical position
* Fixed closing Copy/Move panels by esc
* Fixed display of content when logging in via social networks
* Fixed folder counting in the Info/Details panel
* Fixed sending a letter "Forgot password" for users with non-activated mail
* Fixed display of the user invitation panel on smartphones
* Fixed display of links in notifications for Chinese language
* Fixed display of the context menu for folders and rooms on smartphones
* Fixed display of content on iPad when changing position multiple times
* Fixed generation of thumbnails for media files
* Corrected access rights for the Reviewer/Commenter role in the room
* Fixed display of a removed room in a frame
* Fixed loading files in a room in a frame
* Fixed an error when converting password protected files
* Fixed display of plugin keys after deleting them
* Fixed opening the payment page from the portal settings for community edition(Standalone)
* Add installation on Fedora by oneclick scripts(Standalone)
* Fixed the ability to specify an incorrect document server address in the portal settings(Standalone)
* Fixed the use of self-signed certificates(Standalone)
* Fixed indication of custom port in notification links(Standalone)
* Fixed updating of Letsencrypt certificates for Windows(Standalone)