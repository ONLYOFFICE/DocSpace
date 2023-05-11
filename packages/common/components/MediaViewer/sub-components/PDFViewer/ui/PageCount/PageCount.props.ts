interface PageCountProps {
  visible: boolean;
  isPanelOpen: boolean;
}

export default PageCountProps;

export type PageCountRef = {
  setPageNumber: (pageNumber: number) => void;
  setPagesCount: (pagesCount: number) => void;
};
