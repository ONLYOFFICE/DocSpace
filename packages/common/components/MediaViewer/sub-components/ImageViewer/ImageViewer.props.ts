import { Dispatch, SetStateAction } from "react";
import { getCustomToolbar } from "../../helpers/getCustomToolbar";

interface ImageViewerProps {
  src: string;

  isFistImage: boolean;
  isLastImage: boolean;
  panelVisible: boolean;
  mobileDetails: JSX.Element;
  toolbar: ReturnType<typeof getCustomToolbar>;

  onPrev: VoidFunction;
  onNext: VoidFunction;
  onMask: VoidFunction;

  resetToolbarVisibleTimer: VoidFunction;
  setPanelVisible: Dispatch<SetStateAction<boolean>>;
  setIsOpenContextMenu: Dispatch<SetStateAction<boolean>>;
  generateContextMenu: (
    isOpen: boolean,
    right?: string,
    bottom?: string
  ) => JSX.Element;
}

export default ImageViewerProps;
