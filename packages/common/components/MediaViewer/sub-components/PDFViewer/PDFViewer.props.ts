import { Dispatch, SetStateAction } from "react";
import { getPDFToolbar } from "./../../helpers/getCustomToolbar";
interface PDFViewerProps {
  src: string;
  title: string;
  toolbar: ReturnType<typeof getPDFToolbar>;
  isPDFSidebarOpen: boolean;
  mobileDetails: JSX.Element;

  isLastImage: boolean;
  isFistImage: boolean;

  onMask: VoidFunction;
  generateContextMenu: (
    isOpen: boolean,
    right?: string,
    bottom?: string
  ) => JSX.Element;
  setIsOpenContextMenu: Dispatch<SetStateAction<boolean>>;
  setIsPDFSidebarOpen: Dispatch<SetStateAction<boolean>>;

  onPrev: VoidFunction;
  onNext: VoidFunction;
}

export type BookMark = {
  description: string;
  level: number;
  page: number;
  y: number;
};

export default PDFViewerProps;
