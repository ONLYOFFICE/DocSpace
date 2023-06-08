import { Dispatch, SetStateAction } from "react";
import { BookMark } from "../../PDFViewer.props";

interface MobileDrawerProps {
  bookmarks: BookMark[];
  isOpenMobileDrawer: boolean;
  navigate: (page: number) => void;
  setIsOpenMobileDrawer: Dispatch<SetStateAction<boolean>>;
  resizePDFThumbnail: VoidFunction;
}

export default MobileDrawerProps;
