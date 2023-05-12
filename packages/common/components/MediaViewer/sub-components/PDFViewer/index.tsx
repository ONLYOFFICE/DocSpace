import React, { useEffect, useLayoutEffect, useState, useRef } from "react";
import { isDesktop, isMobile } from "react-device-detect";

import { loadScript, combineUrl } from "@docspace/common/utils";

import PDFViewerProps, { BookMark } from "./PDFViewer.props";
import ViewerLoader from "../ViewerLoader";
import MainPanel from "./ui/MainPanel";
import Sidebar from "./ui/Sidebar";

import {
  ErrorMessage,
  PDFViewerWrapper,
  DesktopTopBar,
  PDFToolbar,
  PDFViewerToolbarWrapper,
} from "./PDFViewer.styled";

import { ToolbarActionType } from "../../helpers";
import {
  ImperativeHandle,
  ToolbarItemType,
} from "../ImageViewerToolbar/ImageViewerToolbar.props";
import PageCount, { PageCountRef } from "./ui/PageCount";

// import { isDesktop } from "react-device-detect";?
const pdfViewerId = "pdf-viewer";
const MaxScale = 5;
const MinScale = 0.5;

function PDFViewer({
  src,
  title,
  toolbar,
  mobileDetails,
  isPDFSidebarOpen,
  onMask,
  generateContextMenu,
  setIsOpenContextMenu,
  setIsPDFSidebarOpen,
}: PDFViewerProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const pdfViewer = useRef<any>(null);
  const pdfThumbnail = useRef<any>(null);
  const toolbarRef = useRef<ImperativeHandle>(null);
  const pageCountRef = useRef<PageCountRef>(null);

  const [file, setFile] = useState<ArrayBuffer | string | null>();
  const [bookmarks, setBookmarks] = useState<BookMark[]>([]);

  const [isLoadedViewerScript, setIsLoadedViewerScript] = useState<boolean>(
    () => {
      const result = document.getElementById(pdfViewerId);
      return result !== null;
    }
  );
  const [isError, setIsError] = useState<boolean>(false);
  const [isLoadingScript, setIsLoadingScript] = useState<boolean>(false);
  const [isLoadingFile, setIsLoadingFile] = useState<boolean>(false);
  const [isFileOpened, setIsFileOpened] = useState<boolean>(false);

  useEffect(() => {
    window.addEventListener("resize", resize);

    return () => {
      window.removeEventListener("resize", resize);
      resetState();
    };
  }, []);

  useEffect(() => {
    resetState();
  }, [src]);

  useLayoutEffect(() => {
    const origin = window.location.origin;
    //@ts-ignore
    const path = window.DocSpaceConfig.pdfViewerUrl;

    if (!isLoadedViewerScript) {
      setIsLoadingScript(true);
      loadScript(
        combineUrl(origin, path),
        pdfViewerId,
        () => {
          initViewer();
          setIsLoadedViewerScript(true);
          setIsLoadingScript(false);
        },
        (event: any) => {
          setIsLoadingScript(false);
          setIsError(true);
          console.error(event);
        }
      );
    }
  }, []);

  useEffect(() => {
    setIsLoadingFile(true);
    setFile(undefined);
    fetch(src)
      .then((value) => {
        return value.blob();
      })
      .then((value) => {
        const reader = new FileReader();
        reader.onload = function (e) {
          setFile(e.target?.result);
        };
        reader.readAsArrayBuffer(value);
      })
      .catch((event) => {
        setIsError(true);
        console.error(event);
      })
      .finally(() => {
        setIsLoadingFile(false);
      });
  }, [src]);

  useEffect(() => {
    if (isLoadedViewerScript && !isLoadingFile && file) {
      try {
        if (!containerRef.current?.hasChildNodes()) {
          initViewer();
        }
        //@ts-ignore
        pdfViewer.current?.open(file);
        setIsFileOpened(true);
      } catch (error) {
        setIsError(true);
        console.log(error);
      }
    }
  }, [file, isLoadedViewerScript, isLoadingFile]);

  useEffect(() => {
    if (isLoadedViewerScript && containerRef.current?.hasChildNodes()) resize();
  }, [isPDFSidebarOpen, isLoadedViewerScript]);

  const initViewer = () => {
    console.log("init PDF Viewer");

    //@ts-ignore
    pdfViewer.current = new window.AscViewer.CViewer("mainPanel", {
      theme: { type: "dark" },
    });

    pdfThumbnail.current =
      //@ts-ignore
      pdfViewer.current.createThumbnails("viewer-thumbnail");

    pdfViewer.current.registerEvent(
      "onStructure",
      function (structure: BookMark[]) {
        setBookmarks(structure);
      }
    );
    pdfViewer.current.registerEvent("onZoom", function (currentZoom: number) {
      toolbarRef.current?.setPercentValue(currentZoom);
    });
    pdfViewer.current.registerEvent(
      "onCurrentPageChanged",
      function (pageNum: number) {
        pageCountRef.current?.setPageNumber(pageNum + 1);
      }
    );
    pdfViewer.current.registerEvent(
      "onPagesCount",
      function (pagesCount: number) {
        pageCountRef.current?.setPagesCount(pagesCount);
      }
    );

    if (isMobile) {
      pdfViewer.current.setZoomMode(2);
    }
  };

  const resize = () => {
    pdfViewer.current?.resize();
    pdfThumbnail.current?.resize();
  };

  const resetState = () => {
    setIsLoadingScript(false);
    setIsFileOpened(false);
    setIsLoadingFile(false);
    setIsPDFSidebarOpen(false);
  };

  function toolbarEvent(item: ToolbarItemType) {
    switch (item.actionType) {
      case ToolbarActionType.Panel:
        setIsPDFSidebarOpen((prev) => !prev);
        break;
      case ToolbarActionType.ZoomIn:
      case ToolbarActionType.ZoomOut:
        if (!pdfViewer.current) return;

        const currentZoom = pdfViewer.current.getZoom();

        const changeBy =
          ToolbarActionType.ZoomOut === item.actionType ? -10 : 10;

        const newZoom = Math.round(currentZoom + changeBy);

        if (newZoom < MinScale * 100 || newZoom > MaxScale * 100) return;

        pdfViewer.current.setZoom(newZoom);
        break;
      case ToolbarActionType.Reset:
        pdfViewer.current.setZoom(100);
        break;
      default:
        break;
    }
  }

  function navigate(page: number) {
    pdfViewer.current.navigate(page);
  }

  if (isError) {
    return (
      <PDFViewerWrapper>
        <ErrorMessage>Something went wrong</ErrorMessage>
      </PDFViewerWrapper>
    );
  }

  return (
    <>
      {isDesktop ? (
        <DesktopTopBar
          title={title}
          onMaskClick={onMask}
          isPanelOpen={isPDFSidebarOpen}
        />
      ) : (
        mobileDetails
      )}

      <PDFViewerWrapper>
        <ViewerLoader
          isLoading={isLoadingFile || isLoadingScript || !isFileOpened}
        />
        <Sidebar
          bookmarks={bookmarks}
          isPanelOpen={isPDFSidebarOpen}
          navigate={navigate}
          setIsPDFSidebarOpen={setIsPDFSidebarOpen}
        />
        <MainPanel
          ref={containerRef}
          isLoading={isLoadingFile || isLoadingScript || !isFileOpened}
        />
      </PDFViewerWrapper>
      <PDFViewerToolbarWrapper>
        <PageCount
          ref={pageCountRef}
          isPanelOpen={isPDFSidebarOpen}
          className="pdf-viewer_page-count"
          visible={!isLoadingFile && !isLoadingScript}
        />

        {isDesktop && !(isLoadingFile || isLoadingScript) && (
          <PDFToolbar
            ref={toolbarRef}
            percentValue={1}
            toolbar={toolbar}
            className="pdf-viewer_toolbar"
            toolbarEvent={toolbarEvent}
            isPanelOpen={isPDFSidebarOpen}
            generateContextMenu={generateContextMenu}
            setIsOpenContextMenu={setIsOpenContextMenu}
          />
        )}
      </PDFViewerToolbarWrapper>
    </>
  );
}

export default PDFViewer;
