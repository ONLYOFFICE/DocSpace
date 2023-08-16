import { Dispatch, SetStateAction } from "react";
import { getCustomToolbar } from "../../helpers/getCustomToolbar";
import { ContextMenuModel } from "../../types";

interface ImageViewerProps {
  src?: string;
  errorTitle: string;
  isFistImage: boolean;
  isLastImage: boolean;
  panelVisible: boolean;
  mobileDetails: JSX.Element;
  toolbar: ReturnType<typeof getCustomToolbar>;

  onPrev: VoidFunction;
  onNext: VoidFunction;
  onMask: VoidFunction;
  contextModel: () => ContextMenuModel[];
  resetToolbarVisibleTimer: VoidFunction;
  setIsOpenContextMenu: Dispatch<SetStateAction<boolean>>;
  generateContextMenu: (
    isOpen: boolean,
    right?: string,
    bottom?: string
  ) => JSX.Element;
}

export default ImageViewerProps;
