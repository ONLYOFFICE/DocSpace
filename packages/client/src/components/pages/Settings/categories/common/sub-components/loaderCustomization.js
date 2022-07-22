import React from "react";
import styled, { css } from "styled-components";
import Loaders from "@docspace/common/components/Loaders";

import { isTablet } from "react-device-detect";

const tabletStyles = css`
  .header {
    display: block;
    width: ${(props) =>
      props.lngTZSettings
        ? "283px"
        : props.welcomePage
        ? "201px"
        : props.portalRenaming
        ? "150px"
        : 0};
    padding-bottom: 16px;
  }

  .description {
    display: none;
  }

  .title {
    display: block;
    width: ${(props) =>
      props.lngTZSettings
        ? "65px"
        : props.welcomePage
        ? "31px"
        : props.portalRenaming
        ? "113px"
        : 0};
  }

  .combo-box {
    display: block;
    width: 350px;
  }

  .field-container {
    display: block;
    width: 350px;
  }

  .save-cancel-buttons {
    display: block;
    position: static;
    width: 350px;
    padding: 0;
  }
`;

const StyledLoader = styled.div`
  .header {
    display: none;
  }

  .description {
    width: 100%;
    padding-bottom: 12px;
  }

  .title {
    width: ${(props) => (props.portalRenaming ? "49px" : "63.7px")};
    padding-bottom: 4px;
  }

  .title-long {
    display: block;
    width: 68px;
    padding-bottom: 4px;
  }

  .combo-box {
    display: block;
    width: 100%;
    padding-bottom: 24px;
  }

  .field-container {
    width: 100%;
    padding-bottom: 12px;
  }

  .save-cancel-buttons {
    display: block;
    position: absolute;
    bottom: 0;
    left: 0;
    width: calc(100% - 32px);
    padding: 0 0 16px 16px;
  }

  @media (min-width: 600px) {
    ${tabletStyles}
  }

  ${isTablet &&
  `
    ${tabletStyles}
  `}
`;

const LoaderCustomization = ({
  lngTZSettings,
  portalRenaming,
  welcomePage,
}) => {
  const heightSaveCancelButtons = window.innerWidth < 600 ? "40px" : "32px";

  return (
    <StyledLoader
      lngTZSettings={lngTZSettings}
      portalRenaming={portalRenaming}
      welcomePage={welcomePage}
      className="category-item-wrapper"
    >
      <Loaders.Rectangle height="22px" className="header" />

      {portalRenaming && (
        <Loaders.Rectangle height="80px" className="description" />
      )}

      <Loaders.Rectangle height="20px" className="title" />
      <Loaders.Rectangle height="32px" className="combo-box" />
      {lngTZSettings && (
        <>
          <Loaders.Rectangle height="20px" className="title-long" />
          <Loaders.Rectangle height="32px" className="combo-box" />
        </>
      )}
      <Loaders.Rectangle
        height={heightSaveCancelButtons}
        className="save-cancel-buttons"
      />
    </StyledLoader>
  );
};

export default LoaderCustomization;
