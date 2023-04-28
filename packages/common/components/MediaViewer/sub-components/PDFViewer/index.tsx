import React, { useEffect, useLayoutEffect, useState, useRef } from "react";
import styled from "styled-components";

import { loadScript, combineUrl } from "@docspace/common/utils";

import PDFViewerProps from "./PDFViewer.props";
import ViewerLoader from "../ViewerLoader";

import "./lib/AllFonts.js";

import "./lib/device_scale.js";
import "./lib/browser.js";
import "./lib/stringserialize.js";
import "./lib/skin.js";

import "./lib/font/loader.js";
import "./lib/font/map.js";
import "./lib/font/character.js";

import "./lib/SerializeCommonWordExcel.js";
import "./lib/Externals.js";
import "./lib/GlobalLoaders.js";
import "./lib/scroll.js";
import "./lib/WorkEvents.js";
import "./lib/Overlay.js";

// import "./lib/bookmarks.js";

import "./lib/file.js";
import "./lib/api.js";

const pdfViewerId = "pdf-viewer";

const PdfViewrWrapper = styled.div`
  position: fixed;
  z-index: 305;

  inset: 0;

  display: flex;
  justify-content: center;
  align-items: center;

  #mainPanel {
    width: 100%;
    height: 100%;

    position: relative;
  }

  .block_elem {
    position: absolute;
    padding: 0;
    margin: 0;
  }
`;

const ErrorMessage = styled.p`
  padding: 20px 30px;
  background-color: rgba(0, 0, 0, 0.6);
`;
function PDFViewer({ src }: PDFViewerProps) {
  const containerRef = useRef<HTMLDivElement>(null);

  const [file, setFile] = useState<ArrayBuffer | string | null>();

  const [isError, setIsError] = useState<boolean>(false);

  const [isLoadedViewerScript, setIsLoadedViewerScript] = useState<boolean>(
    () => {
      const result = document.getElementById(pdfViewerId);
      return result !== null;
    }
  );

  const [isLoadingScript, setIsLoadingScript] = useState<boolean>(false);
  const [isLoadingFile, setIsLoadingFile] = useState<boolean>(false);

  useLayoutEffect(() => {
    const origin = window.location.origin;
    const path = window.DocSpaceConfig.pdfViewerUrl;

    if (!isLoadedViewerScript) {
      setIsLoadingScript(true);
      loadScript(
        combineUrl(origin, path),
        pdfViewerId,
        () => {
          //@ts-ignore
          window.Viewer = new window.AscViewer.CViewer("mainPanel", {});
          setIsLoadedViewerScript(true);
          setIsLoadingScript(false);
        },
        (event) => {
          setIsLoadingScript(false);
          setIsError(true);
          console.error(event);
        }
      );
    }
  }, []);

  useEffect(() => {
    setIsLoadingFile(true);
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
          //@ts-ignore
          window.Viewer = new window.AscViewer.CViewer("mainPanel", {});
        }
        //@ts-ignore
        window.Viewer.open(file);
      } catch (error) {
        setIsError(true);
        console.log(error);
      }
    }
  }, [file, isLoadedViewerScript, isLoadingFile]);

  if (isError) {
    return (
      <PdfViewrWrapper>
        <ErrorMessage>Something went wrong</ErrorMessage>
      </PdfViewrWrapper>
    );
  }
  return (
    <>
      <PdfViewrWrapper>
        <ViewerLoader isLoading={isLoadingFile || isLoadingScript} />
        <div
          ref={containerRef}
          style={{
            visibility: isLoadingFile || isLoadingScript ? "hidden" : "visible",
          }}
          id="mainPanel"
        ></div>
      </PdfViewrWrapper>
    </>
  );
}

export default PDFViewer;
