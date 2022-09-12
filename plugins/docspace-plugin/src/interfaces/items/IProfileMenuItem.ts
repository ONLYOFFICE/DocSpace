export interface IProfileMenuItem {
  key: string;
  position: number;
  label: string;
  icon: string;
  onClick: () => void;
}
