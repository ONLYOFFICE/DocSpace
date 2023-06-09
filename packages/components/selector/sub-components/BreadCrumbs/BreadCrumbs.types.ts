export type BreadCrumb = {
  id: string | number;
  label: string;
  onClick?: (e: any, open: any, item: BreadCrumb) => void;
};

export type DisplayedItem = {
  id: string | number;
  label: string;
  isArrow: boolean;
  isList: boolean;
  listItems?: BreadCrumb[];
};

export type BreadCrumbsProps = {
  breadCrumbs?: BreadCrumb[];
  onSelectBreadCrumb?: (item: BreadCrumb) => void;
};
