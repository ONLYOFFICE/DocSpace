import { Dispatch, SetStateAction } from "react";
import { getCustomToolbar } from "../../helpers/getCustomToolbar";

interface ImageViewerToolbarProps {
  toolbar: ReturnType<typeof getCustomToolbar>;
  generateContextMenu: (
    isOpen: boolean,
    right?: string,
    bottom?: string
  ) => JSX.Element;
  setIsOpenContextMenu: Dispatch<SetStateAction<boolean>>;
  ToolbarEvent: (item: ToolbarItemType) => void;
}

export type ToolbarItemType = ReturnType<typeof getCustomToolbar>[number];

export type ImperativeHandle = {
  setPercentValue: (percent: number) => void;
};

export default ImageViewerToolbarProps;
