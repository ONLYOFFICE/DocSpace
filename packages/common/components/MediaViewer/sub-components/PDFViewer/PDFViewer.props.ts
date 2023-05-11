import { Dispatch, SetStateAction } from "react";
import { getPDFToolbar } from "./../../helpers/getCustomToolbar";
interface PDFViewerProps {
  src: string;
  title: string;
  toolbar: ReturnType<typeof getPDFToolbar>;
  isPDFSidebarOpen: boolean;
  mobileDetails: JSX.Element;

  onMask: VoidFunction;
  generateContextMenu: (
    isOpen: boolean,
    right?: string,
    bottom?: string
  ) => JSX.Element;
  setIsOpenContextMenu: Dispatch<SetStateAction<boolean>>;
  setIsPDFSidebarOpen: Dispatch<SetStateAction<boolean>>;
  handleChangeVersion: (arg: string) => void; // temp
}

export type BookMark = {
  description: string;
  level: number;
  page: number;
  y: number;
};

export default PDFViewerProps;
