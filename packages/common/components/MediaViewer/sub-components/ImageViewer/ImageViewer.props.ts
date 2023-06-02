import { Dispatch, SetStateAction } from "react";
import { getCustomToolbar } from "../../helpers/getCustomToolbar";

interface ImageViewerProps {
  src?: string;
  thumbnailSrc?: string;
  isTiff?: boolean;
  imageId?: number;
  version?: number;

  isFistImage: boolean;
  isLastImage: boolean;
  panelVisible: boolean;
  mobileDetails: JSX.Element;
  toolbar: ReturnType<typeof getCustomToolbar>;

  onPrev: VoidFunction;
  onNext: VoidFunction;
  onMask: VoidFunction;

  resetToolbarVisibleTimer: VoidFunction;
  setIsOpenContextMenu: Dispatch<SetStateAction<boolean>>;
  generateContextMenu: (
    isOpen: boolean,
    right?: string,
    bottom?: string
  ) => JSX.Element;
}

export default ImageViewerProps;
