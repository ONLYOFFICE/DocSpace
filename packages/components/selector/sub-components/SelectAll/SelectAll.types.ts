export type SelectAllProps = {
  label?: string;
  icon?: string;
  onSelectAll?: () => void;
  isChecked?: boolean;
  isIndeterminate?: boolean;
  isLoading?: boolean;
  rowLoader: any;
};
