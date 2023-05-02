import React, { useState, memo, useRef, useEffect, useCallback } from "react";
import { Document, Page, pdfjs } from "react-pdf";
import { FixedSizeList } from "react-window";

import PDFViewerProps, {
  LoadSuccessType,
  PDFDocumentProxy,
  PDFPageProxy,
} from "./PDFViewer.props";

import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";

pdfjs.GlobalWorkerOptions.workerSrc = `//cdnjs.cloudflare.com/ajax/libs/pdf.js/${pdfjs.version}/pdf.worker.js`;

import "react-pdf/dist/esm/Page/AnnotationLayer.css";
import "react-pdf/dist/esm/Page/TextLayer.css";

const PDFViewer = ({ src, handleChangeVersion }: PDFViewerProps) => {
  const listRef = useRef<FixedSizeList>(null);
  const pdfRef = useRef<PDFDocumentProxy>();
  const pageRef = useRef<PDFPageProxy>();

  const [scale, setScale] = useState(1);
  const [numPages, setNumPages] = useState<number>(0);

  const [viewport, setViewport] = useState({
    width: 0,
    height: 0,
  });

  useEffect(() => {
    window.addEventListener("resize", handleResize);

    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, [viewport]);

  const handleResize = () => {
    if (!pdfRef.current) return;
    pdfRef.current.getPage(1).then((page) => {
      const { width: widthViewport, height: heightViewport } = page.getViewport(
        { scale: 1.5 }
      );

      let width = Math.min(widthViewport, window.innerWidth);
      let height = (width / widthViewport) * heightViewport;

      setViewport({ width, height });
    });
  };

  const setSize = (scale: number) => {
    if (!pageRef.current) return;

    const { width, height } = pageRef.current.getViewport({ scale });
    setViewport({ width, height });
  };

  const handleLoadSuccess: LoadSuccessType = (pdf) => {
    setNumPages(pdf.numPages);
    pdfRef.current = pdf;

    pdf.getPage(1).then((page) => {
      pageRef.current = page;
      const { width: widthViewport, height: heightViewport } = page.getViewport(
        { scale: 1.5 }
      );

      let width = Math.min(widthViewport, window.innerWidth);
      let height = (width / widthViewport) * heightViewport;

      setViewport({ width, height });
    });
    // loadThumbnailPages(getPage, numPages);
  };

  const renderPage = useCallback(
    ({ index, style }) => {
      return (
        <div
          style={{
            display: "grid",
            justifyItems: "center",
            ...style,
          }}
        >
          <Page
            pageNumber={index + 1}
            width={viewport.width}
            // height={viewport.height}
            // viewport={viewport}
          />
        </div>
      );
    },
    [viewport.width]
  );

  const onZoomIn = () => {
    setSize(scale + 0.1);
    setScale(scale + 0.1);
  };

  const onZoomOut = () => {
    setSize(scale - 0.1);
    setScale(scale - 0.1);
  };

  return (
    <>
      <div
        style={{
          position: "fixed",
          inset: 0,
          zIndex: 300,

          paddingTop: "85px",
          background: "rgba(55, 55, 55, 0.6)",
        }}
      >
        <div>
          <Document
            file={src}
            onLoadSuccess={handleLoadSuccess}
            onItemClick={(e) => {
              listRef.current.scrollToItem(e.pageNumber);
            }}
          >
            <FixedSizeList
              ref={listRef}
              height={window.innerHeight}
              itemCount={numPages}
              itemSize={viewport.height}
              outerElementType={CustomScrollbarsVirtualList}
            >
              {renderPage}
            </FixedSizeList>
          </Document>
        </div>
      </div>

      <div
        style={{
          position: "fixed",
          bottom: 50,
          left: "50%",
          zIndex: 300,
        }}
      >
        <button onClick={onZoomIn}>Zoom In</button>
        <button onClick={onZoomOut}>Zoom Out</button>
      </div>

      <div
        style={{
          position: "fixed",
          zIndex: 310,
          left: "50px",
          top: "50px",
          cursor: "pointer",
        }}
      >
        <button onClick={() => handleChangeVersion("1")}>Viewer V1</button>
      </div>
    </>
  );
};

export default memo(PDFViewer);
