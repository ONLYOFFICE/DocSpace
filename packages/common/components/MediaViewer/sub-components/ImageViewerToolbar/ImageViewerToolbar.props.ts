import { Dispatch, SetStateAction } from "react";
import { getCustomToolbar } from "../../helpers/getCustomToolbar";

interface ImageViewerToolbarProps {
  toolbar: ReturnType<typeof getCustomToolbar>;
  generateContextMenu: (
    isOpen: boolean,
    right?: string,
    bottom?: string
  ) => JSX.Element;
  percent: number;
  setIsOpenContextMenu: Dispatch<SetStateAction<boolean>>;
  ToolbarEvent: (item: ToolbarItemType) => void;
}

export type ToolbarItemType = ReturnType<typeof getCustomToolbar>[number];

export default ImageViewerToolbarProps;
