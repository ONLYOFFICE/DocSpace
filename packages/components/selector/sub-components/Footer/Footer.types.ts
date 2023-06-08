import { AccessRight } from "../../Selector.types";

export type FooterProps = {
  isMultiSelect?: boolean;
  acceptButtonLabel: string;
  selectedItemsCount: number;
  withCancelButton?: boolean;
  cancelButtonLabel?: string;
  withAccessRights?: boolean;
  accessRights?: AccessRight[];
  selectedAccessRight?: AccessRight | null;
  onAccept?: () => void;
  onCancel?: () => void;
  onChangeAccessRights?: (access: string | number) => void;
};
