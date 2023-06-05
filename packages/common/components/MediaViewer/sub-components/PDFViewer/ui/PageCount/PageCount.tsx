import React, {
  ForwardedRef,
  forwardRef,
  useCallback,
  useImperativeHandle,
  useState,
} from "react";
import { isMobile } from "react-device-detect";
import PageCountProps, { PageCountRef } from "./PageCount.props";
import { PageCountWrapper } from "./PageCount.styled";

import PanelReactSvg from "PUBLIC_DIR/images/panel.react.svg";

function PageCount(
  { isPanelOpen, visible, className, setIsOpenMobileDrawer }: PageCountProps,
  ref: ForwardedRef<PageCountRef>
) {
  const [pagesCount, setPagesCount] = useState<number>(0);
  const [pageNumber, setPageNumber] = useState<number>(0);

  useImperativeHandle(ref, () => ({
    setPagesCount(pagesCount: number) {
      setPagesCount(pagesCount);
    },
    setPageNumber: (pageNumber: number) => {
      setPageNumber(pageNumber);
    },
  }));

  const openMobileDrawer = useCallback(() => {
    setIsOpenMobileDrawer(true);
  }, []);

  if (!visible) return <></>;

  return (
    <PageCountWrapper isPanelOpen={isPanelOpen} className={className}>
      {isMobile && <PanelReactSvg onClick={openMobileDrawer} />}
      <div>
        <span>{pageNumber}</span> / <span>{pagesCount}</span>
      </div>
    </PageCountWrapper>
  );
}

export default forwardRef<PageCountRef, PageCountProps>(PageCount);
