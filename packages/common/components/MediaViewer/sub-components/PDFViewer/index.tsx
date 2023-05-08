import React, { useEffect, useLayoutEffect, useState, useRef } from "react";

import { loadScript, combineUrl } from "@docspace/common/utils";

import PDFViewerProps, { BookMark } from "./PDFViewer.props";
import ViewerLoader from "../ViewerLoader";
import MainPanel from "./ui/MainPanel";
import Sidebar from "./ui/Sidebar";

import {
  ErrorMessage,
  PdfViewrWrapper,
  DesktopTopBar,
  PDFToolbar,
} from "./PDFViewer.styled";

import { ToolbarActionType } from "../../helpers";
import { ToolbarItemType } from "../ImageViewerToolbar/ImageViewerToolbar.props";

// import "./lib";

// import { isDesktop } from "react-device-detect";?
const pdfViewerId = "pdf-viewer";

function PDFViewer({
  src,
  title,
  toolbar,
  isPDFSidebarOpen,
  onMask,
  generateContextMenu,
  setIsOpenContextMenu,
  setIsPDFSidebarOpen,
}: PDFViewerProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const pdfViewer = useRef<any>(null);
  const pdfThumbnail = useRef<any>(null);

  const [file, setFile] = useState<ArrayBuffer | string | null>();
  const [bookmarks, setBookmarks] = useState<BookMark[]>([]);

  const [isError, setIsError] = useState<boolean>(false);

  const [isLoadedViewerScript, setIsLoadedViewerScript] = useState<boolean>(
    () => {
      const result = document.getElementById(pdfViewerId);
      return result !== null;
    }
  );

  const [isLoadingScript, setIsLoadingScript] = useState<boolean>(false);
  const [isLoadingFile, setIsLoadingFile] = useState<boolean>(false);

  const resize = () => {
    pdfViewer.current?.resize();
    pdfThumbnail.current?.resize();
  };

  useEffect(() => {
    window.addEventListener("resize", resize);

    return () => {
      window.removeEventListener("resize", resize);
    };
  }, []);

  const initViewer = () => {
    console.log("init PDF Viewer");

    //@ts-ignore
    pdfViewer.current = new window.AscViewer.CViewer("mainPanel", {
      theme: { type: "dark" },
    });
    //@ts-ignore
    pdfThumbnail.current =
      //@ts-ignore
      pdfViewer.current.createThumbnails("viewer-thumbnail");
    //@ts-ignore
    pdfViewer.current.registerEvent(
      "onStructure",
      function (structure: BookMark[]) {
        setBookmarks(structure);
      }
    );

    console.log(pdfThumbnail.current);
    console.log(pdfViewer.current);
    // console.log(pdfViewer.current.navigate());
    console.log(pdfViewer.current.navigateToPage);
  };

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
          console.log("");
          initViewer();
        }
        console.log("open");
        //@ts-ignore
        pdfViewer.current?.open(file);
        resize();
      } catch (error) {
        setIsError(true);
        console.log(error);
      }
    }
  }, [file, isLoadedViewerScript, isLoadingFile]);

  useEffect(() => {
    if (isLoadedViewerScript && containerRef.current?.hasChildNodes()) resize();
  }, [isPDFSidebarOpen, isLoadedViewerScript]);

  function toolbarEvent(item: ToolbarItemType) {
    switch (item.actionType) {
      case ToolbarActionType.Panel:
        setIsPDFSidebarOpen((prev) => !prev);
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
      <PdfViewrWrapper>
        <ErrorMessage>Something went wrong</ErrorMessage>
      </PdfViewrWrapper>
    );
  }

  return (
    <>
      <DesktopTopBar
        title={title}
        onMaskClick={onMask}
        isPanelOpen={isPDFSidebarOpen}
      />

      <PdfViewrWrapper>
        <ViewerLoader isLoading={isLoadingFile || isLoadingScript} />
        <Sidebar
          bookmarks={bookmarks}
          isPanelOpen={isPDFSidebarOpen}
          navigate={navigate}
          setIsPDFSidebarOpen={setIsPDFSidebarOpen}
        />
        <MainPanel
          ref={containerRef}
          isLoading={isLoadingFile || isLoadingScript}
        />
      </PdfViewrWrapper>

      <PDFToolbar
        toolbar={toolbar}
        percentValue={1}
        isPanelOpen={isPDFSidebarOpen}
        toolbarEvent={toolbarEvent}
        generateContextMenu={generateContextMenu}
        setIsOpenContextMenu={setIsOpenContextMenu}
      />
    </>
  );
}

export default PDFViewer;
