# Change log

## 3.5.0

### New features

#### General changes

* Switching from yarn to pnpm in the client
* Updated the nodejs version to v22
* Updated the SDK JS local script version to v2.1
* Switching the server from the Polly library to Resilience (a library
  for ensuring stability and handling temporary failures, which allows
  implementing strategies such as repeated request attempts, timeouts, etc.)
* Added a two-factor authentication advertising banner to the Portal Settings
  -> Login History and Profile -> Login sections
* Completely rewritten the Info panel. Added new logic for displaying content
  loading inside it (now old content is always shown with blur until
  new content is received, after 500 ms animation appears in the tab)
* Added new loading animation when switching through a directory and profile
  (now the skeleton appears only on the first page load or after switching
  from the portal settings). Also rewritten the logic of tabs in the profile
* Added the ability to copy page content
* Added new keyboard hotkeys: Disable the selection area - Ctrl + Alt, Copy
  the selected items text - Ctrl + Shift + C
* Updated the Pomelo.EntityFramework.Core connector for MySql to version 9.0
* Switching to new ESLINT v9
* Enabled linter for shared components
* Added a service for receiving notifications via Telegram. It's possible
  to configure it on the Settings -> Integration -> Third-party services page
  and enable notifications in the profile
* Added buttons for connecting and disconnecting the WeChat login provider
* Updated outdated logos for some login providers
* Changing the language in the profile occurs without refreshing the page
* Updated EmptyScreens in selectors and the info panel
* Added additional fonts (apple-system, "segoe ui") to the basic set to improve
  the display of Arabic characters on macOS
* Removed the key parameter (access key) from the link. Now the user cannot
  access the portal by copying the link from the browser address bar

#### Settings

* Added a checkbox to hide/show the "About this program" window
* Added placeholders for lists in the OAuth, Webhooks and ApiKeys sections
* Changed Backup:
  * Rewrote the AutoBackup, RestoreBackup, and ManualBackup pages
    from JavaScript to TypeScript
  * Along with the main pages, redesigned all related components, including
    auxiliary modules, dialogs and controls
  * Replaced class components with functional ones using React hooks.
  * Removed dependencies on the global store, simplified logic and improved
    architecture
  * Moved pages to the Shared directory for reuse
  * Fixed bugs, optimized code, improved readability
  * In Space Management, pages are no longer connected via Module Federation
* Changed Portal Encryption. Rewrote component styles from styled-components
  to scss. When decrypting the portal, changed the translation from
  "The encryption process in progress" to "The decryption process in progress"
* In the final Audit Events report, replaced the Products/Module columns with Location
* Added the ability to buy the Backup service via the Wallet
* Updated the report in the Wallet
* Added filter reset in the Wallet
* Added dialogs when disabling the backup service
* Added a warning for DocSpace admins that backups are not free if the free
  backup quota is exceeded
* Added the ability to upload plugins to the SaaS version
* Split the Action column in the Login History report: added
  the LoginSuccessViaOAuth, LoginSuccessViaPassword actions, added the name
  of a specific provider into LoginSuccessViaApiSocialAccount, etc.
* Added the WeChat login provider to the Integrations section
* Updated outdated logos for some login providers in the Integrations section
* Added a new loading animation when switching between settings sections
  and tabs in all sections
* Added a dialog window with user statistics for the Enterprise license
* Brought plugins description to a unified form, added the “Learn more” link
  to the Plugins SDK documentation. Added plugin versioning: incompatible
  versions are highlighted in red, a tooltip appears when hovering over them,
  and a toast appears when uploading asking for an update

#### Login page

* Redesigned the two-factor authentication page
* An open email address is no longer used in links on confirmation pages
  Instead, the `encemail` parameter is now passed — an encrypted version of the email
* Changed the logic on the EmailChange page: if the user is not authorized
  in a browser, he will be redirected to the login page with a request
  to enter the previous credentials to log in to the portal
* Added additional user information to the ProfileRemove page: name, email, and avatar
* Added buttons for logging in via WeChat
* Updated outdated logos for some login providers
* Updated the login captcha design
* Added a captcha to the password recovery request window and the access
  recovery request window. Now the methods require a captcha key
  and it is always called (in hidden or explicit form)

#### Documents

* Now opensearch contains only document content without sources
* Added the "Everyone" system group in the invitation panel
* Improved the drag-drop functionality. A floating button now appears
  with information about where the selected element will be placed
* Changed design of tiles for folders and files
* Added the `api/2.0/files/folder/{folderId}/log/report` method to generate
  a report on the room
* Added a hint if a file is locked by another user and there are no rights
  to unlock it
* Updated file and folder sharing: added new features for working with links
  and improved the interface
  * Added the ability to share folders without creating rooms
  * Implemented separate links for files and folders both inside and outside
    of rooms
  * Added the ability to share links for internal and external users (for rooms
    and their contents, including the Documents section)
  * In the Documents section, links now have a name (title) and access rights
    for links can be configured in the same way as for rooms
  * Brought the interface for working with links to a unified style
  * Added new notifications (toasts)
  * Added new items to context menus for managing links
  * Added pages for entering a password when accessing files and folders
* Added the Recent and Favorites sections
* Added the Shared with me section
  * The 'Shared with me' section displays files and folders shared by other
    users from the 'My Documents' section, as well as files and folders
 	accessible via a link
  * Folders shared via a link open in the 'Shared with me' section
    for authorized users
  * Added access levels for files and folders
  * Added the 'Sharing with DocSpace users' section for PDF forms in the
    'Share to fill out' panel, as well as sharing functionality for other files
 	in the 'Share' panel
* Redesigned the left menu. Renamed the Documents section to My Documents.
* Added the AI agents item to the left menu
* Renamed the Room column in Trash to Location
* Added the ability to hide the "Download applications" section via the config
  on the server
* Added a mechanism for preloading editor resources to speed up the first opening
* Added the `onFileManagerClick` event for SDK JS when clicking on a file in a list
* Added the `onEditorOpen` event for SDK JS for events related to opening
  an editor (creation, editing, filling) from context menus, modal windows, panels, or hotkeys
* Added grouping of context menu items
* Added the ability to repeat the upload or conversion in case of an error
* Added display of the Room Manager role with a corresponding hint in the role
  change list for a user or guest
* Added the ability to delete rooms
* Redesigned file/folder deletion dialogs
* Removed the 'New' badge from the room creation dialog and the context menu
  for Templates
* Disabled saving the selected filter in rooms (only sorting remains)

#### Management

* Completely rewrote Space Management in next.js
* Fixed behavior when encrypting a portal: if another tab with
  a portal is open, it will also switch to the /encryption-portal page
  
### Fixes

#### General changes

* Fixed the issue with missing cache control headers for static resources
  of next clients (login, editor, management, sdk) in NGINX settings
* Fixed issues with client dependency audit; updated outdated packages
  (webpack, next, axios, react, next, typescript, etc.)
* Fixed issues with eslint and typescript

#### Login page

* Fixed login page flickering if SSO is configured and the hideAuthPage
  setting is enabled
* Fixed usage of externalResources in the wizard and login

#### Documents

* Fixed a bug when a table header was broken in Safari

## 3.2.1

### New features

#### General changes

* The ability to rename the portal is now only available on Business plans

#### Documents

* Added support for .pages, .numbers, .key, .hwp, .hwpx, .odg, .md formats
* Added support for opening Diagram-type documents: .vsdx, .vssx, .vstx,
  .vsdm, .vssm, .vstm
* Favicons updated and improved
* Starter documents updated in some languages

#### Settings

* Added the ability to hide the advertising block in Ad Management
  (for commercial builds of DocSpace Server only)
* The Payer is now determining via API only through portal/payment/customerinfo,
  and information about the payer is returned immediately
* Purchasing administrators during the Grace Period now requires adding
  a payment method
* The recommended payment has been removed from the top-up dialog
  when downgrading a plan
* Added Apply and Clear buttons to the Wallet filter in the mobile version

### Fixes

#### Settings

* Fixed the ability to perform Backup when the data limit is exceeded
 on the portal
* Fixed an infinite process of creating a Backup file in Amazon AWS S3
  if the Region or Bucket is specified incorrectly
* Fixed an error when accessing Developer Tools under Room Admin if the section
  is hidden from User-type users
* Fixed the operation of the `GET /api/2.0/keys/@self` method if Permissions
  other than All are selected when creating the key
* Fixed the ability to generate an Audit Trail report in a neighboring tab
* Improved the description of the Developer Tools access configuration

#### Documents

* Fixed an error when attempting to restore a session after a forcesave
  and connection loss while working on a document
* Fixed the reset of the Days remaining value in the Erasure column after
  restoring and re-deleting a file
* Fixed a file search error when selecting the Members - Other filter
  for a user of type User
* Fixed an error when resetting assigned roles in a VDR Room and starting
  to fill it out from beginning
* Corrected the tooltip wording when copying a link to a file or room
* Fixed the presence of a download button for a locked file in a Public Room
  opened via a link and in thumbnail mode
* Fixed the display of the file lock label for users with the Editor role
* Fixed skeleton display when quickly switching from a room to the archive
  with a slow connection
* Fixed the operation of the Enter key in the confirmation window when moving
  a Room to the Archive
* Fixed clipboard functionality in Firefox
* Fixed downloading of a document or folder if its name contains the "#" symbol
* Fixed the appearance of an extra window when trying to open a document
  in the mobile version on iOS if the app is not installed
* Improved user interaction when opening a document in the mobile version
  with the "Open in app only" setting selected
* Fixed navigation using the browser Back button while viewing media
* Fixed the shift in room button highlighting when launching Take a short tour

#### Accounts

* Fixed an error when retrieving the list of Contacts if the sort value
  in Local Storage was incorrect
* Fixed the accessibility of Owner and data transfer initiator in the
  "Choose from list" window of the Data reassignment panel
* Fixed the display of the account list for a user whose type was changed
  from Room Admin to DocSpace Admin and vice versa
* Fixed the dependency of the language for resend invitation emails
  on the portal language instead of the set Invitation language
* Fixed the appearance of the New Rooms button in the menu when switching
  between Rooms and Contacts with a slow connection
* The order of the First Name and Last Name fields in the Change name
  window now depends on the region
* Improved the display of Social Media connect buttons in the user profile

#### Server

* Fixed display of the domain instead of localhost in notifications
  about changes on the portal
* Fixed the operation of the POST /apisystem/portal/register method
  if a negative time zone is set on the server
* Fixed an error applying Back up when restoring a backup of version 2.6.0
  on version 3.1.1
  
#### Security

* Fixed a vulnerability that allowed users with User or Guest roles to view
  the Owner's id using the `GET /api/2.0/settings/security/administrator/:productid` method
* Fixed a vulnerability that allowed viewing the Owner's profile
  using the `GET /api/2.0/portal/users/:userID` method.
* Fixed the accessibility of the PUT /api/2.0/people/:userid/contacts method.

## 3.2.0

### New features

#### General changes

* Updated the react-virtualized library to the latest version
  used for rendering lists of files/rooms/contacts
* Updated the react-route library to the latest version (7.5.2)
  used for navigation
* Added new language: sq-AL (Albanian)
* Added caching for requests to web plugins
* Added a link to UserVoice in the menu and a new banner

#### Settings

* Added settings for inviting users to the portal
* Added the Wallet section (for SaaS only)
* Added the Services section (for SaaS only)
* Changed the editor logo scheme in the Branding section in connection
  with new editor themes

#### Login page

* Added the ability to share guests. When selecting the "Share guest" option
  in the Contacts/Guests section, the user receives a link to a new confirm
  page called "GuestInviteForm"
* The account is now automatically linked to the DocSpace account with the same
  email address when logging in via social media

#### Documents

* Added support for onRequestRefreshFile. Now, if you lose Internet connection
  or wait for a long time, the document will be updated itself,
  without reloading the page
* Added empty file templates for creating in the following languages: ca-ES, cs-CZ, da-DK, hu-HU, id-ID, ro-RO, sq-AL, ur-PK
* Added scroll control from the keyboard in the new files panel
* The logic for generating the URL for the Open file location button has been moved to the server

### Fixes

#### Documents

* Optimized work with filters
* Fixed errors when calculating activated filters
* Fixed sending duplicate requests to the server when filtering a file list
  by author or a room list by participant

## 3.1.1

### Fixes

#### Settings

* Fixed client crash when authorizing via Auth link without specifying scope
* Fixed an error when displaying the application of the removed user
  in the Developer Tools
* Fixed an issue with repeatedly resetting the 2FA application
* Fixed an error after connecting Google Drive as Third-Party resource
  in server version
* Fixed the visual freeze of the Back up process when multiple Spaces
  are created
* Implemented the resetting of the 2FA application after a long period
  of inactivity
* Fixed the display of the portal logo in Developer Tools after page refresh
* Fixed the mention of the platform in the tooltip for Authentication
  when configuring the LDAP server
* Fixed opening a page in a new tab using the right mouse button

#### Login page

* Fixed a security issue that allowed authorization after changing the password

#### Documents

* Fixed adding an entry to reports in Filling Form Room if the user
  is not authorized
* Fixed the application of settings for the VDR room when it is created from
  a template
* Fixed missing values for tag and quota in the template when creating a Room
  from it in the root of Rooms
* Fixed the presence of the Start filling/Filling status/Stop filling/Reset
  and start filling PDF form menu buttons in rooms other than VDR
* Fixed the presence of the Quick sharing button in the PDF form editor
  for certain Room types
* Fixed the application of the template icon using Customize cover
  in the info panel
* Fixed the possibility of re-invoking the internal template menu after changing
  the Name/Tags settings
* Fixed the presence of the Complete & Submit button in the form labeled My Draft
  in the Filling Form Room, restored from Archive
* Fixed the notifications to the room owner about file upload/edit
  by another participant
* Fixed the possibility of deleting a Public Room used as storage
* Fixed the change of type from PDF Form to PDF Documents in the Type column
  after changing the index in VDR Room
* Fixed the ability to edit a PDF form on external access
  with fill-in permissions
* Fixed the designation of a Copy/Duplicate file as editing if the original
  was opened in the editor
* Fixed the appearance of a deleted value in the SearchInput field
  on the Select panel
* Fixed opening of PDF forms in the desktop version of editors when opening
  a file in VDR Room and mobile version
* Fixed the display of Your turn/In Progress statuses in the VDR Room
  and mobile version
* Fixed highlighting of elements during the Short tour for the Form Filling Room
  on iPad
* Fixed the ability to scroll content in the Create room dialog
  at a screen resolution of 1024x650
* Fixed the ability to perform operations with templates using drag-and-drop
* Fixed the description in the Empty screen of the Upload from DocSpace module

#### Accounts

* Fixed email confirmation for DocSpace and Room Admin
* Fixed the ability to delete a guest
* Fixed adding more than one Authorized app in the user profile

#### SDK

* Fixed an error when attempting to authorize a system user in WordPress
* Fixed the display of inactive folders Complete/In progress
  in the file selector from Filling Form Room in WordPress
* Fixed the display of the editor close button in the corresponding
  WordPress frame
* Fixed the display of the selector in the DocSpace frame embedded in Wordpress
 after logging out
* Fixed the logo offset on the Empty screen during the DocSpace configuration
  process in WordPress
* Fixed the presence of the "Back to room" button for unauthorized users
  filling out the form via WordPress
* Fixed the presence of the sidebar call button for a non-Public room embedded
  in WordPress
* Fixed the presence of Share and Embed buttons in the menu of a file embedded
  in WordPress in embedded mode
* Fixed the ability to go to the Trash inside a frame with a room embedded
  in WordPress
* Fixed the ability for a User role to create a new room in WordPress
* Fixed the display of extra rows in the room skeleton embedded in WordPress
  for unauthorized users
* The format of the type parameter of the createRoom method has been brought
  into compliance

## 3.1.0

### New features

#### General changes

* Fixed a security issue in NextJS (Authorization Bypass in Next.js Middleware)
* A new SSR client (/sdk) has been added for working with the JS-SDK,
  which includes basic modules for working with DocSpace (file-selector,
  room-selector, public-room)
* Implemented basic functionality of the public room for the SDK client
* Expanded methods for connecting the JS-SDK on the settings page
* Added a key parameter to the URL of Filling Forms Room, Custom Room,
  and Public Room. Now a link to a room can be copied directly from
  the browser's address bar
* Added a page with an error message for opening a file of an unsupported
  format via an external link that is no longer available

#### Settings

* Updated button colors for adding and changing the avatar in the profile
* Added an error message to the main settings methods in the sections Backup
  and Restore
* The "Company Name" field in the Branding section has been replaced with
  "Brand name" and "Generate logo from text"
* Added a data storage region in Storage management for the cloud version
* Added a setting for deep link
* Added a setting to disable email activation for LDAP/SSO portal users
* The DeveloperTools section is now available to all users except guests
* Added triggers in the Developer tools – Webhooks section
* Added a section for API keys in the Developer Tools
* The form for configuring the document editing server service
  in the Integration - Document Service section has been modified to allow
  specifying a secret key and Authorization header

#### Login page

* Added the ability to share guests. When selecting the "Share guest" option
  in the Contacts/Guests section, the user receives a link to a new confirm
  page called "GuestInviteForm"
* The account is now automatically linked to the DocSpace account with the same
  email address when logging in via social media

#### Documents

* Added the ability to delete a file version in the version panel
* Added the ability to create templates from rooms and rooms from templates
* Implemented the ability to fill out the form based on user roles
  in the Virtual Data Room
* Added the ability to share PDF forms directly without rooms
* Added a password input dialog when downloading a protected file in a format
  different from the original
* Updated the library for thumbnail generation
* Links created in the Documents section are now created with no expiration
  date and with Read only access by default
* The search bar on mobile devices is now hiding when scrolling down
* Settings for the "SearchInContent" and "WithSubfolders" document filters
  have been removed and are now enabled by default
* The report format in Filling Form Room has been changed to XLSX
* Improved user interaction during operation progress
* The appearance of the tiles near the rooms has been changed
* Added a training system for the Form Filling Room
* Added the ability to enable and disable custom filters for table
* Added new document types (PDF Form and PDF Document) and filters
  (PDF-forms and PDF-documents)
* The design of the "Fill in as" and "Share to fill out" panels has been
  unified in style
* The "Share" button has been removed from the editor for PDF forms
* The logic of the link creation in the editor has been moved from
  "Share panel" to "Share to fill out"
* The list of push notifications in the mobile app for events on the portal
  has been expanded

#### Management

* Added an ability to encrypt data at rest in the server version

#### Accounts

* Added the ability to downgrade a type to User and to Guest

## 3.0.4

### Fixes
* Fixed adding users with a dot in the email
* Fixed context menu for creating files with screen width 1000px
* Fixed update external links in spreadsheet for anonymus user
* Fixed performance and timeout errors

## 3.0.3

* No public release.

## 3.0.2

* No public release.

## 3.0.1

### Fixes
* Fixed the work of OAuth2 in the US region
* Fixed viewing document versions in someone else's room for the administrator
* Fixed opening editors for docker with self-signed certificates(Standalone)
* Fixed opening editors after update when using self-signed certificates(Standalone)
* Fixed default value in user type field on ldap settings page after update
* Removed description of user type on payments page
* Fixed dialog for choosing the method of opening a document on mobile devices under an unauthorized user
* Fixed registration on the portal for guests via social networks
* Fixed closing of Action required window when copying file again
* Fixed uploading/downloading files without extension
* Fixed the ability to insert a document from a form room into WordPress
* Fixed authorization on the portal after an update if an ldap server with the user type "power user" was
* Added guest user type to ldap server connection settings
* Fixed access rights in public rooms for dockspace administrators
* Fixed email activation for ldap users

## 3.0.0

### New features

#### General changes

* Restored the mobile app advertising banner.
* Changed the licensing scheme. The Branding functionality is blocked for the enterprise_license without the customization parameter. Branding is available for the developer_license with the customization parameter. The About window is hidden for the developer_plus_license. There is no the Branding tab in the free Community version.
* Fixed loading of only the first 1000 records in the Info -> History panel, now the remaining records are also loaded when scrolling.
* Changed file format icons.

#### Settings

* Added a new OAuth2.0 section: Settings -> Developer Tools -> OAuth2. Added new login and profile pages.
* Added a warning before migrating if the quota is enabled.
* Fixed logo sizes in Branding.

#### Login page

* Added OAuth support for Login.
* Added the ability to select the necessary portal if a user has more than 1 portal according to the entered data.
* Added the 'instance ID' field to the setup wizard.
* Added the checkbox about the newsletter subscription to the registration page.

#### Documents

* Added a dialog window for converting XML files. Added the ability to convert XML files to the docx or xlsx format.
* Rewrote the logic for applying Right-to-Left styles to logical CSS properties.
* Added the shardkey parameter to all editor requests (api.js loading, conversion, builder, command execution) to work with the cluster version that supports sharding.
* Added the ability to share a Public Room for editing.
* Changed the context menu. Added nesting in the mobile view.
* Added new empty screens.
* Added an option to display file extensions to the Profile -> File management section.
* Added requirements for the public link password.
* Added an hourglass icon for unverified users.
* Changed the Share feature in the Documents section: moved the Remove link option, added a new toast about copying the link, fixed minor bugs.
* Added the ability to set a custom logo for a room.
* Changed the design for the profile and room avatar editor.
* Added a new room type: Virtual Data Room. Implemented new capabilities for working with data in this type of room: indexing of data with the ability to change the index and generate a summary report on the file structure of the room, setting the lifetime of files, prohibition on copying and downloading files, setting watermarks.
* Added a toast about the view-only mode and an authorization button in the public room.
* Increased the original size of room avatars by 2 times to support 4k displays.
* Changed the panel with information about latest new files in rooms.
* When creating a new document for the en-US language, a template with the Letter page format is used. If there are no templates for a language, a document with the A4 page format is used.
* Added empty files templates for creation in the following languages: fi-FI, he-IL, nb-NO, sl-SI.
* Starting documents have titles in the corresponding language.
* Added the ability to display public rooms shared via link in the room list.
* Optimized queries when saving changes in the room editing panel and when creating a room: all parameters are passed in one method.
* Added new types of records to the Info -> History panel.

#### Management

* Redesigned the Management section.
* Added the ability to quickly delete a portal (if the wizard is not completed, the portal will be deleted without confirmation).

#### Accounts

* Renamed the Accounts section to Contacts. Added a new Guests tab to it. Renamed the People tab to Members.
* The DocSpace Owner/Admin has read-only access to rooms he was not invited to, but he can change the owner for such a room or archive it.
* Any role can be assigned to the DocSpace Owner/Admin and he will work in the room according to the role.
* The Room Admin role is renamed to Room Manager. He can invite only Users and Guests to the portal (via rooms) and cannot promote the type of other contacts higher than the User.
* The Power User type is a free type, which is renamed to User.
* The Power User role is renamed to Content Creator. The Content Creator role allows creating/uploading/deleting files/folders even for Guests.
* The User type renamed to Guest. Guests can only be invited via rooms.
* Changed the Info -> History panel, now the focus is on actions, not the user name.
* Changed the appearance of the element for insertion when entering an email in the invitation.

### Fixes
* Added the ability to use keyboard actions in modal windows
* Fixed display of support button (live chat)
* Added notification about enabled quota when migration starts
* Added the ability to select the user type when connecting to the SSO server
* Added notification when trying to copy a document other than PDF into the form room
* Added reset to default settings for brute force protection
* Fixed extra browser window when copying a link on Safari
* Fixed message when adding users over the tariff limit
* Added the ability to edit files in a public room without authorization
* Fixed an error in the browser console for the user when creating a room by the administrator
* Fixed setting the filter when clicking on the portal logo
* Fixed double click on room in copy window
* Fixed an error if a folder/room for auto backup was deleted
* Fixed switcher color in portal settings for dark theme
* Fixed errors in logs after installing the standalone version
* Fixed storage of files without extension
* Fixed creation of new documents according to the selected language
* Fixed creation of sample documents according to the selected language
* Fixed focus on search field after results are displayed
* Fixed connection of public room in frame for server version
* Fixed automatic update of LetsEncrypt certificates in the server version
* Fixed emails when registering via social networks
* Added sand clock icon for accounts that have not accepted an invitation in the group selector
* Added a message about exceeding the quota limit before the file upload starts
* Fixed adding users with underscore in email
* Fixed favicon on tab after form submission
* Added a message when trying to edit PDF forms on mobile devices
* Twitter has been replaced by X
* Fixed creation of a new CSV report after deleting it in the form filling room
* Fixed the appearance of the new badge for the room owner when filling out the form
* Fixed display of diacritic characters on the groups tab
* Removed the role of filler forms in the custom room
* Added the ability to connect Egnyte as a third-party storage via WebDav

## 2.6.3

### Fixes
* Fixed redirection to mobile app when opening documents via deeplink
* Added duplication function for the form filling room
* Fixed display of history for users without rights in the form filling room
* Fixed the ability to see other people's forms in the form filling room
* Fixed display of more than 100 users in a group on mobile devices
* Fixed copy link option for non-editable formats
* Fixed display of PDF and forms when opened on mobile devices
* Fixed display of secret codes for 2FA
* Fixed opening of the form via a link with a password for an authorized user

## 2.6.0

### New features

#### General changes

* Added banners about reaching the disk space limit for Room Admins/Power users.
* Added hiding/appearing behavior for the scrollbar.
* In the content part, the scroll is set from the beginning of the page. In the room members panel, the scroll is set from the beginning of the block.
* Fixed the direction of the sorting arrow.
* Added the ability to create new folders in the Move to / Copy to / Restore / Save copy as dialog windows. When opening an input field, the following hotkeys are supported: Enter for creating the folder, Esc for closing the input field.
* Fixed user avatars in the version history and co-editing.
* Backup now has smoother progress and uses less memory.
* Written a new component for tabs. Added the ability to automatically scroll when clicking on a tab that is not fully visible. Added blur to the container edges.
* Changed email messages, added new ones, reworked texts and links.
* Localized logos and product names for China.
* Renamed Guest to Anonymous in the Audit Trail settings.

#### Settings

* Changed translations and page styles on the Bonus page in the Community edition. Added the Bonus page in the Space Management section in the Community edition.
* Added the LDAP Settings section to the Integration settings. This setting allows importing users and groups via the LDAP protocol from a third-party LDAP Server.
* Removed the BETA badge from the Plugins section.
* Removed the 'Storage period' field in the Audit Trail, Login History settings.
* Added the button to return back when entering a specific preset in Javascript SDK.

#### Login page

* Implemented changing the language on the login and confirmation pages by adding a combo box in the upper right corner. Now users will be able to sign in or sign up using the selected language.
* Added reCaptcha on the login page. It appears if the Brute Force Protection Settings are configured and a certain number of incorrect logins are made, and a public key and type are added to appsettings.json.
* Changed work of invitation links.

#### Documents

* Viewing images will be done through the 3840x2160 (4K) thumbnails to speed up loading and flipping through large images.
* Reworked thumbnail mechanism: one image of the single 1280x720 size will be shown instead of 12 different images to reduce the number of requests and reduce the load on the thumbnail generation service.
* Added links for connecting non-activated third-party storages in a public room. When clicking on a non-activated storage, the Integration page with a modal window for activating a third-party service will open.
* Added a new room type: Form filling. In these rooms, users can create forms on the base of PDF templates and invite other users to fill out these forms. Once the forms are completed, the data from them is automatically analyzed and compiled into a spreadsheet.
* Changed room icons in the room creation panel. Added effects when hovering a cursor over a button, the button border now matches the portal theme.
* Added the ability to open a media file / image via an external link.
* Added a context menu for creating new items instead of the standard browser menu.
* Added the ability to copy a link to a folder for external users in Public rooms.
* Fixed a bug with the incorrect table in Rooms.
* Added the ability to open a document in the same tab.
* Renamed the My Documents section to Documents, updated its icon.
* In a mobile browser, the ONLYOFFICE logo will not be displayed when opening the editor if the license allows (Docs v8.1 and later required).
* Added support for new entries in the file and room history (for files: converting files; for files and folders: renaming, moving, copying files and folders; for rooms: renaming rooms, changing a logo, adding/removing tags, adding/renaming/removing links, changing roles/removing users, changing roles/adding/removing groups).
* Added the ability to embed Public rooms and documents from them.
* Implemented duplication for rooms and folders. Changed the duplication method for files. When duplicating rooms, the icon and tags are copied, users are deleted. Duplicated items have an index at the end of the name.
* Added a limit on the number of pinned rooms - 10.
* Added a hotkey for renaming files/folders - F2.
* Added scrolling of the file list when selecting with the mouse while holding the left mouse button.
* It's no longer possible to convert from docx to docxf.
* Changed colors of badges and third-party services icons for the dark theme.

#### Accounts

* Added support for 'ArrowUp' and 'ArrowDown' hotkeys for the list. 'Enter', 'Backspace' for groups.
* Changed the work of the filter by user status.
* Added a country and city to IP in the Location column in the profile for the SaaS version.

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

### New features

#### General changes

* Switching to .net 8 and yarn v4.
* Added distributed locking based on Redis/ZooKeeper.
* The confirmation window for the selected Google account always appears: when logging in, connecting and on the backup page.
* Changed scrolling of pages, panels, etc.
* Added a mechanism for working with Google Tag Manager, it works automatically if there is an corresponding setting in the server configuration.
* Styled the NGINX standard errors.

#### Settings

* Added presets for Javascript SDK: DocSpace, Public room, Editor, Viewer, Room selector, File selector, Custom.
* Added a mechanism for forwarding the editor customization config.
* Adjusted the behavior of internal checks for the CSP mechanism.
* Changed the loader for the SDK.
* Fixed the mechanism of synchronizing the color scheme transmitted to the SDK for the editor.
* Added work with the type of opened editor (restored the mechanism of the embedded display).
* Added a new view for the selector mode - a button that opens the selector window.
* Added the ability to upload files using drag and drop in the Plugins list.
* Added validation and notifications about errors when changing the portal name in the DNS settings.
* Implemented migration from ONLYOFFICE Workspace, Google Workspace and Nextcloud: added the ability to migrate users with files, groups and shared files/folders.
* Changed the display of buttons in the portal settings on mobile devices.
* Added users and rooms quotas (a default quota, which can be set in the portal settings, and a custom quota, which can be set in the info panel, in the corresponding column of the table, through the menu and in the editing/creating room dialog window).
* Added the new Storage Management page which displays statistics on storage space used.

#### Accounts

* Changed the page for inviting users to a portal/room: redesigned the form, divided into steps. The first step is to enter the email. If the user with such email exists on the portal, the second step is to go to the modified login page. If the user does not exist, the second step is to enter a name and create a password. It's also possible to return to the previous step using the Back button and enter a different email.
* Changed social network buttons.
* Added tabs to the Accounts page.
* Added the new Group column to the group table.
* Added the new way to sort/filter users – by groups.
* Added a table to display groups.
* Changed HeaderMenu and MainButton in the Accounts section.
* Added the new panel for creating/editing groups.
* Added group filtering by members.
* Added the new Info panel which displays information about the group.
* Added new data about groups when displaying user information in the Info panel.
* Added the ability to enter a group as if it were a folder, the table for users in the group, and navigation back to accounts.
* Removed the ability to edit an already uploaded photo (change zoom and aspect) in the Avatar Editor.
* A larger image is now uploaded to the server to improve quality when zooming in on large screens.

#### Documents

* Moved long operations to ASC.Files.Service.
* Optimized conversion.
* Changed width calculation when adding new table columns.
* Changed the URL to view media files: /media/view is used instead of /products/files/#preview.
* Redesigned the info panel: fixed the context menu, actions with the selected element, display of the element in the panel.
* Added asynchronous file upload.
* The Overwrite Confirmation dialog window is now shown every time without a separate setting.
* Added the ability to share files from My Documents.
* Added the Share tab in the Info panel to assign permissions to links.
* Added empty file templates for ar-SA and sr-Latn-RS cultures.
* Accelerated loading of large files (chunks are now loaded in several threads).
* Restored the ability to create rooms based on third-party storages (for Public rooms only).
* Added additional information about users in the Info panel.
* Changed the tariff banner and moved it to the header.
* Added the “Share room” button to the navigation for public rooms (copies the link to the public room and shows it).
* Removed the “Additional links” header for public rooms now additional links are included in one list).
* Changed “Copy general link” to “Copy shared link” for public rooms (“General link” to “Shared links”).
* Changed the “Megaphone” icon to “Planet” for public rooms.
* Replaced the ONLYOFFICE Sample Form.oform file with ONLYOFFICE Sample Form.pdf.
* Changed work of the InviteInput field.
* Added the ability to invite groups to rooms for free roles.
* Added the ability to jointly search for users and/or groups within a room in the info panel.
* Added the safe area for mobile devices with “bangs”.
* Now the last set room filter is not reset when switching to other sections (in Rooms/Archive).
* Added the new advertising banner.
* Added new icons for the xlsb format.
* Added the "Create a new folder in the storage" checkbox when creating a room with the connection of a third-party resource.
* Rewrote Document Editor to the NextJS library. Accelerated opening documents.
* Rewrote all selectors (files, room and people) to a more optimal working scheme.
* Changed drag for the Tiles view (now dragging files/folders occurs over the entire tile area).
* Added a new state in all selectors with users. If a user is blocked, he is always displayed in the list, but it's not possible to take actions with him. If a user is already added, he is displayed with the 'Invited' label in the selector for inviting users to a room.

#### Management

* Added a new Management client (Standalone only) for managing DocSpace spaces: it allows you to configure the current portal and manage other portals, set up branding for all portals, perform backup/restore of all portals, go to the payments page.
* Added a new modal window to Management: after configuring the portal, a window appears where you are asked to go to the domain or stay in the settings.

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