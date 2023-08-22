export type SearchProps = {
  placeholder?: string;
  value?: string;
  onSearch: (value: string) => void;
  onClearSearch: () => void;
};
