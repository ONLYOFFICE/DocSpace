import React from "react";

import { Item } from "./sub-components/Item/Item.types";
import { BreadCrumb } from "./sub-components/BreadCrumbs/BreadCrumbs.types";

export type AccessRight = {
  string: {
    key: string;
    label: string;
    description: string;

    access: string | number;
  };
};

export type SelectorProps = {
  id?: string;
  className?: string;
  style?: React.CSSProperties;
  headerLabel: string;
  withoutBackButton?: boolean;
  onBackClick?: () => void;
  hideSearch?: boolean;
  searchPlaceholder?: string;
  searchValue?: string;
  onSearch?: (value: string) => void;
  onClearSearch?: () => void;
  items: Item[];
  onSelect: (item: Item) => void;
  isMultiSelect?: boolean;
  selectedItems?: Item[];
  acceptButtonLabel: string;
  onAccept: (
    selectedItems: Item[],
    access: AccessRight | null,
    fileName: string,
    isFooterCheckboxChecked: boolean
  ) => void;
  withSelectAll?: boolean;
  selectAllLabel?: string;
  selectAllIcon?: string;
  onSelectAll?: () => void;
  withAccessRights?: boolean;
  accessRights?: AccessRight[];
  selectedAccessRight?: AccessRight;
  onAccessRightsChange?: (access: AccessRight) => void;
  withCancelButton?: boolean;
  cancelButtonLabel?: string;
  onCancel?: () => void;
  emptyScreenImage?: string;
  emptyScreenHeader?: string;
  emptyScreenDescription?: string;
  searchEmptyScreenImage?: string;
  searchEmptyScreenHeader?: string;
  searchEmptyScreenDescription?: string;
  hasNextPage?: boolean;
  isNextPageLoading?: boolean;
  loadNextPage?: (startIndex: number) => void;
  totalItems: number;
  isLoading?: boolean;
  searchLoader?: any;
  rowLoader?: any;
  withBreadCrumbs?: boolean;
  breadCrumbs?: BreadCrumb[];
  onSelectBreadCrumb?: (item: BreadCrumb) => void;
  breadCrumbsLoader?: any;
  isBreadCrumbsLoading?: boolean;

  withFooterInput?: boolean;
  footerInputHeader?: string;
  currentFooterInputValue?: string;
  footerCheckboxLabel?: string;
};
