import { BookMark } from "../../PDFViewer.props";

interface BookmarksProps {
  bookmarks: BookMark[];
  navigate: (page: number) => void;
}

export default BookmarksProps;
