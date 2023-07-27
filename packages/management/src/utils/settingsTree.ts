import SpacesSvgUrl from "PUBLIC_DIR/images/spaces.react.svg?url";
import BrandingSvgUrl from "PUBLIC_DIR/images/branding.react.svg?url";
import DataManagementIconUrl from "PUBLIC_DIR/images/data-management.react.svg?url";
import RestoreIconUrl from "PUBLIC_DIR/images/restore.react.svg?url";
import PaymentIconUrl from "PUBLIC_DIR/images/payment.react.svg?url";

export const settingsTree = [
    {
        id: "management-settings_catalog-spaces",
        key: "0",
        icon: SpacesSvgUrl,
        link: "spaces",
        tKey: "Spaces",
        isHeader: true,
    },
    {
        id: "management-settings_catalog-branding",
        key: "1",
        icon: BrandingSvgUrl,
        link: "branding",
        tKey: "Branding",
        isHeader: true,
    },
    {
        id: "management-settings_catalog-backup",
        key: "2",
        icon: DataManagementIconUrl,
        link: "backup",
        tKey: "Backup",
        isHeader: true,
    },
    {
        id: "management-settings_catalog-restore",
        key: "3",
        icon: RestoreIconUrl,
        link: "restore",
        tKey: "RestoreBackup",
        isHeader: true,
    },
    {
        id: "management-settings_catalog-payments",
        key: "4",
        icon: PaymentIconUrl,
        link: "payments",
        tKey: "Common:PaymentsTitle",
        isHeader: true,
    },
];

