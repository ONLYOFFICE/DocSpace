/**
 * Array for generation current settings tree.
param title is unused
param link also used as translation key
 */
export const settingsTree = [
 {
     title: 'Common',
     key: '0',
     icon: 'SettingsIcon',
     link: 'common',
     children: [
         {
             title: 'Customization',
             key: '0-0',
             icon: '',
             link: 'customization',
             children: [
              {
                  title: 'Customization1',
                  key: '0-0-0',
                  icon: '',
                  link: 'customization1',
                  children: [
                   {
                       title: 'Customization2',
                       key: '0-0-0-0',
                       icon: '',
                       link: 'customization2',
                       
                   },
               ]
                  
              },
          ]
         },
     ]
 },
 {
     title: 'Security',
     key: '1',
     icon: 'SettingsIcon',
     link: 'security',
     children: [
         {
             title: 'Access Rights',
             key: '1-0',
             icon: '',
             link: 'access-rights',
         },
     ]
 },

];

/**
* Array for generation full settings tree.
param title is unused
param link also used as translation key
*/
export const settingsTreeFull = [
   {
       title: 'Common',
       key: '0',
       icon: 'SettingsIcon',
       link: 'common',
       children: [
           {
               title: 'Customization',
               key: '0-0',
               icon: '',
               link: 'customization',
           },
           {
               title: 'Modules & tools',
               key: '0-1',
               icon: '',
               link: 'modules-and-tools',
           },
           {
               title: 'White label',
               key: '0-2',
               icon: '',
               link: 'white-label',
           },
       ]
   },
   {
       title: 'Security',
       key: '1',
       icon: 'SettingsIcon',
       link: 'security',
       children: [
           {
               title: 'Portal Access',
               key: '1-0',
               icon: '',
               link: 'portal-access',
           },
           {
               title: 'Access Rights',
               key: '1-1',
               icon: '',
               link: 'access-rights',
           },
           {
               title: 'Login History',
               key: '1-2',
               icon: '',
               link: 'login-history',
           },
           {
               title: 'Audit Trail',
               key: '1-3',
               icon: '',
               link: 'audit-trail',
           },
       ]
   },
   {
       title: 'Data Management',
       key: '2',
       icon: 'SettingsIcon',
       link: 'data-management',
       children: [
           {
               title: 'Migration',
               key: '2-0',
               icon: '',
               link: 'migration',
           },
           {
               title: 'Backup',
               key: '2-1',
               icon: '',
               link: 'backup',
           },
           {
               title: 'Portal Deactivation/Deletion',
               key: '2-2',
               icon: '',
               link: 'portal-deactivation-deletion',
           },
       ]
   },
   {
       title: 'Integration',
       key: '3',
       icon: 'SettingsIcon',
       link: 'integration',
       children: [
           {
               title: 'Third-Party Services',
               key: '3-0',
               icon: '',
               link: 'third-party-services',
           },
           {
               title: 'SMTP Settings',
               key: '3-1',
               icon: '',
               link: 'smtp-settings',
           }
       ]
   },
   {
       title: 'Statistics',
       key: '4',
       icon: 'SettingsIcon',
       link: 'statistics',
   },
 ];
