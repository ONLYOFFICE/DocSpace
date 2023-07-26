import { Dispatch, SetStateAction } from "react";
import { BookMark } from "../../PDFViewer.props";

interface SidebarProps {
  isPanelOpen: boolean;
  setIsPDFSidebarOpen: Dispatch<SetStateAction<boolean>>;
  bookmarks: BookMark[];
  navigate: (page: number) => void;
}

export default SidebarProps;
