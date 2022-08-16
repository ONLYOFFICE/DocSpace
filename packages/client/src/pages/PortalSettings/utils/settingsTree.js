/**
 * Array for generation current settings tree.
 */

export const settingsTree = [
  {
    key: "0",
    icon: "/static/images/settings.react.svg",
    link: "common",
    tKey: "Common:Common",
    isHeader: true,
    children: [
      {
        key: "0-0",
        icon: "",
        link: "customization",
        tKey: "Customization",
        isCategory: true,
        children: [
          {
            key: "0-0-0",
            icon: "",
            link: "language-and-time-zone",
            tKey: "StudioTimeLanguageSettings",
          },
          {
            key: "0-0-1",
            icon: "",
            link: "welcome-page-settings",
            tKey: "CustomTitlesWelcome",
          },
          {
            key: "0-0-2",
            icon: "",
            link: "portal-renaming",
            tKey: "PortalRenaming",
          },
        ],
      },
      {
        key: "0-2",
        icon: "",
        link: "whitelabel",
        tKey: "WhiteLabel",
      },
    ],
  },
  {
    key: "1",
    icon: "/images/security.react.svg",
    link: "security",
    tKey: "ManagementCategorySecurity",
    isHeader: true,
    children: [
      {
        key: "1-0",
        icon: "",
        link: "access-portal",
        tKey: "PortalAccess",
        isCategory: true,
        children: [
          {
            key: "1-0-0",
            icon: "",
            link: "password",
            tKey: "SettingPasswordStrength",
          },
          {
            key: "1-0-1",
            icon: "",
            link: "tfa",
            tKey: "TwoFactorAuth",
          },
          {
            key: "1-0-2",
            icon: "",
            link: "trusted-mail",
            tKey: "TrustedMail",
          },
          {
            key: "1-0-3",
            icon: "",
            link: "ip",
            tKey: "IPSecurity",
          },
          {
            key: "1-0-4",
            icon: "",
            link: "admin-message",
            tKey: "AdminsMessage",
          },
          {
            key: "1-0-5",
            icon: "",
            link: "lifetime",
            tKey: "SessionLifetime",
          },
        ],
      },
      {
        key: "1-1",
        icon: "",
        link: "access-rights",
        tKey: "AccessRights",
        isCategory: true,
        children: [
          {
            key: "1-1-0",
            icon: "",
            link: "admins",
            tKey: "Admins",
          },
        ],
      },
    ],
  },
  {
    key: "3",
    icon: "/images/integration.react.svg",
    link: "integration",
    tKey: "ManagementCategoryIntegration",
    isHeader: true,
    children: [
      {
        key: "3-0",
        icon: "",
        link: "single-sign-on",
        tKey: "SingleSignOn",
        isCategory: true,
      },
      {
        key: "3-1",
        icon: "",
        link: "third-party-services",
        tKey: "ThirdPartyAuthorization",
        isCategory: true,
      },
      {
        key: "3-1",
        icon: "",
        link: "portal-integration",
        tKey: "PortalIntegration",
        isCategory: true,
      },
    ],
  },
  {
    key: "4",
    icon: "/images/payment.react.svg",
    link: "payments",
    tKey: "Payments",
    isHeader: true,
    children: [
      {
        key: "4-0",
        icon: "",
        link: "portal-payments",
        tKey: "Payments",
        isCategory: true,
      },
    ],
  },
  {
    key: "5",
    icon: "/images/data-management.react.svg",
    link: "datamanagement",
    tKey: "ManagementCategoryDataManagement",
    isHeader: true,
    children: [
      {
        key: "5-0",
        icon: "",
        link: "backup",
        tKey: "Backup",
        isCategory: true,
      },
    ],
  },
];

/**
* Array for generation full settings tree, old structure.
param title is unused
param link also used as translation key
*/
export const settingsTreeFull = [
  {
    title: "Common",
    key: "0",
    icon: "/static/images/settings.react.svg",
    link: "common",
    children: [
      {
        title: "Customization",
        key: "0-0",
        icon: "",
        link: "customization",
      },
      {
        title: "Modules & tools",
        key: "0-1",
        icon: "",
        link: "modules-and-tools",
      },
      {
        title: "White label",
        key: "0-2",
        icon: "",
        link: "white-label",
      },
    ],
  },
  {
    title: "Security",
    key: "1",
    icon: "/static/images/settings.react.svg",
    link: "security",
    children: [
      {
        title: "Portal Access",
        key: "1-0",
        icon: "",
        link: "portal-access",
      },
      {
        title: "Access Rights",
        key: "1-1",
        icon: "",
        link: "access-rights",
      },
      {
        title: "Login History",
        key: "1-2",
        icon: "",
        link: "login-history",
      },
      {
        title: "Audit Trail",
        key: "1-3",
        icon: "",
        link: "audit-trail",
      },
    ],
  },
  {
    title: "Data Management",
    key: "2",
    icon: "/static/images/settings.react.svg",
    link: "data-management",
    children: [
      {
        title: "Migration",
        key: "2-0",
        icon: "",
        link: "migration",
      },
      {
        title: "Backup",
        key: "2-1",
        icon: "",
        link: "backup",
      },
      {
        title: "Portal Deactivation/Deletion",
        key: "2-2",
        icon: "",
        link: "portal-deactivation-deletion",
      },
    ],
  },
  {
    title: "Integration",
    key: "3",
    icon: "/static/images/settings.react.svg",
    link: "integration",
    children: [
      {
        title: "Third-Party Services",
        key: "3-0",
        icon: "",
        link: "third-party-services",
      },
      {
        title: "SMTP Settings",
        key: "3-1",
        icon: "",
        link: "smtp-settings",
      },
    ],
  },
  {
    title: "Statistics",
    key: "4",
    icon: "/static/images/settings.react.svg",
    link: "statistics",
  },
];
