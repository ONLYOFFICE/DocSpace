import { Dispatch, SetStateAction } from "react";
import { getCustomToolbar } from "../../helpers/getCustomToolbar";

interface ImageViewerToolbarProps {
  toolbar: ReturnType<typeof getCustomToolbar>;
  generateContextMenu: (
    isOpen: boolean,
    right?: string,
    bottom?: string
  ) => JSX.Element;
  percentValue: number;
  setIsOpenContextMenu: Dispatch<SetStateAction<boolean>>;
  toolbarEvent: (item: ToolbarItemType) => void;
  className?: string;
}

export type ToolbarItemType = ReturnType<typeof getCustomToolbar>[number];

export type ImperativeHandle = {
  setPercentValue: (percent: number) => void;
};

export default ImageViewerToolbarProps;
