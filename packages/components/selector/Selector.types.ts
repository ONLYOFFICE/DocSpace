import React from "react";

import { Item } from "./sub-components/Item/Item.types";

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
  onBackClick: () => void;
  searchPlaceholder?: string;
  searchValue?: string;
  onSearch?: (value: string) => void;
  onClearSearch?: () => void;
  items: Item[];
  onSelect: (item: Item) => void;
  isMultiSelect?: boolean;
  selectedItems?: Item[];
  acceptButtonLabel: string;
  onAccept: (selectedItems: Item[], access: AccessRight | null) => void;
  withSelectAll?: boolean;
  selectAllLabel?: string;
  selectAllIcon?: string;
  onSelectAll?: () => void;
  withAccessRights?: boolean;
  accessRights?: AccessRight[];
  selectedAccessRight?: AccessRight;
  onAccessRightsChange?: (access: string | number) => void;
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
};
