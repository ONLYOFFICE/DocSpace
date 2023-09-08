import Scrollbar from "@docspace/components/scrollbar";
import { tablet } from "@docspace/components/utils/device";
import React from "react";
import styled, { css } from "styled-components";

const StyledScrollbar = styled(Scrollbar)`
  ${({ $isScrollLocked }) =>
    $isScrollLocked &&
    css`
      box-sizing: border-box;
      & .scroll-wrapper > .scroller > .scroll-body {
        overflow: hidden !important;
        ${props =>
          props.theme.interfaceDirection === "rtl"
            ? css`
                padding-left: 17px !important;
                margin-left: 0 !important;
              `
            : css`
                padding-right: 17px !important;
                margin-right: 0 !important;
              `}

        padding-bottom: 20px !important;
        margin-bottom: 0 !important;
      }
    `}
`;

const SubInfoPanelBody = ({ children, isInfoPanelScrollLocked }) => {
  const content = children?.props?.children;

  return (
    <StyledScrollbar
      $isScrollLocked={isInfoPanelScrollLocked}
      scrollclass="section-scroll"
      stype="mediumBlack">
      {content}
    </StyledScrollbar>
  );
};

SubInfoPanelBody.displayName = "SubInfoPanelBody";

export default SubInfoPanelBody;
