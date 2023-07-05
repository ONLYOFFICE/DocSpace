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
  disableAcceptButton?: boolean;
  onAccept?: () => void;
  onCancel?: () => void;
  onChangeAccessRights?: (access: AccessRight) => void;

  withFooterInput?: boolean;
  withFooterCheckbox?: boolean;
  footerInputHeader?: string;
  currentFooterInputValue?: string;
  footerCheckboxLabel?: string;
  setNewFooterInputValue?: (value: string) => void;
  isFooterCheckboxChecked?: boolean;
  setIsFooterCheckboxChecked?: any;

  acceptButtonId?: string;
  cancelButtonId?: string;
};
