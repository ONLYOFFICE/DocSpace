import { BreadCrumb } from "../BreadCrumbs/BreadCrumbs.types";
import { Item } from "./../Item/Item.types";

export type BodyProps = {
  footerVisible: boolean;
  withHeader?: boolean;
  isSearch: boolean;
  isAllIndeterminate?: boolean;
  isAllChecked?: boolean;
  placeholder?: string;
  value?: string;
  withSearch?: boolean;
  onSearch: (value: string) => void;
  onClearSearch: () => void;
  items: Item[];
  onSelect: (item: Item) => void;
  isMultiSelect?: boolean;
  withSelectAll?: boolean;
  selectAllLabel?: string;
  selectAllIcon?: string;
  onSelectAll?: () => void;
  emptyScreenImage?: string;
  emptyScreenHeader?: string;
  emptyScreenDescription?: string;
  searchEmptyScreenImage?: string;
  searchEmptyScreenHeader?: string;
  searchEmptyScreenDescription?: string;
  loadMoreItems: (startIndex: number) => void;
  hasNextPage?: boolean;
  isNextPageLoading?: boolean;
  totalItems: number;
  isLoading?: boolean;
  searchLoader: any;
  rowLoader: any;
  withBreadCrumbs?: boolean;
  breadCrumbs?: BreadCrumb[];
  onSelectBreadCrumb?: (item: BreadCrumb) => void;
  breadCrumbsLoader?: any;
  isBreadCrumbsLoading?: boolean;

  withFooterInput?: boolean;
  withFooterCheckbox?: boolean;

  descriptionText?: string;
};
