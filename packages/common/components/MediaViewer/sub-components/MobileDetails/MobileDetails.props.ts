import { ContextMenuModel } from "../../types";

interface MobileDetailsProps {
  icon: string;
  title: string;
  isError: boolean;
  isPreviewFile: boolean;
  contextModel: () => ContextMenuModel[];

  onHide: VoidFunction;
  onMaskClick: VoidFunction;
  onContextMenu: (e: TouchEvent) => void;
}

export default MobileDetailsProps;
