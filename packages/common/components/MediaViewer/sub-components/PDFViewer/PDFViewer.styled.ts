import styled, { css } from "styled-components";
import { isDesktop } from "react-device-detect";

import DesktopDetails from "../DesktopDetails";
import ImageViewerToolbar from "../ImageViewerToolbar";

type Panel = { isPanelOpen?: boolean };

export const PdfViewrWrapper = styled.div`
  position: fixed;
  z-index: 305;
  inset: 0;

  display: flex;

  background: rgba(55, 55, 55, 0.6);

  #mainPanel {
    width: 100%;
    height: 100%;

    position: relative;
  }
  #id_viewer {
    ${isDesktop &&
    css`
      background: none !important;
    `}
  }
  .block_elem {
    position: absolute;
    padding: 0;
    margin: 0;
  }
`;

export const ErrorMessage = styled.p`
  padding: 20px 30px;
  background-color: rgba(0, 0, 0, 0.6);
`;

export const DesktopTopBar = styled(DesktopDetails)<Panel>`
  display: flex;

  left: ${(props) => (props.isPanelOpen ? "306px" : 0)};
  width: ${(props) => (props.isPanelOpen ? "calc(100%  - 306px)" : "100%")};

  .mediaPlayerClose {
    margin-top: -3px;
    margin-left: -7px;
    width: 28px;
  }

  .title {
    padding-right: 16px;
  }
`;

export const PDFToolbar = styled(ImageViewerToolbar)<Panel>`
  left: ${({ isPanelOpen }) => `calc(50% + ${isPanelOpen ? 306 / 2 : 0}px)`};

  transition: background 0.26s ease-out 0s;
`;
