import React, { useState, useEffect } from "react";
import styled, { css } from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { isTablet } from "react-device-detect";

const tabletStyles = css`
  .header {
    display: ${(props) => !props.dnsSettings && "block"};
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
        ? "61px"
        : props.welcomePage
        ? "28px"
        : props.portalRenaming
        ? "109px"
        : 0};
    padding-bottom: 4px;
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
    width: ${(props) => (props.welcomePage ? "274px" : "197px")};
    padding: 8px 0 0;
  }

  .dns-description {
    width: 122px;
    padding-bottom: 12px;
  }

  .dns-field {
    width: 350px;
    padding-bottom: 12px;
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
    width: ${(props) => (props.portalRenaming ? "109px" : "61px")};
  }

  .title-long {
    display: block;
    width: 64px;
    padding-bottom: 4px;
  }

  .combo-box {
    display: block;
    width: 100%;
    padding-bottom: 16px;
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

  .flex {
    display: flex;
    align-items: center;
    padding-bottom: 8px;
  }

  .dns-description {
    padding-bottom: 8px;
  }

  .padding-right {
    padding-right: 8px;
  }

  .dns-field {
    height: 32px;
  }

  @media (min-width: 600px) {
    ${tabletStyles}
  }

  ${isTablet &&
  `
    ${tabletStyles}
  `}

  @media (min-width: 1024px) {
    .save-cancel-buttons {
      width: ${(props) => (props.welcomePage ? "264px" : "192px")};
    }
  }
`;

const LoaderCustomization = ({
  lngTZSettings,
  portalRenaming,
  welcomePage,
  dnsSettings,
}) => {
  const [isMobileView, setIsMobileView] = useState(false);
  const [isDesktopView, setIsDesktopView] = useState(false);

  const checkInnerWidth = () => {
    const isMobileView = window.innerWidth < 600;
    const isDesktopView = window.innerWidth <= 1024;

    if (isMobileView) {
      setIsMobileView(true);
    } else {
      setIsMobileView(false);
    }

    if (isDesktopView) {
      setIsDesktopView(true);
    } else {
      setIsDesktopView(false);
    }
  };

  useEffect(() => {
    checkInnerWidth();
    window.addEventListener("resize", checkInnerWidth);

    return () => window.removeEventListener("resize", checkInnerWidth);
  });

  const heightSaveCancelButtons = isDesktopView ? "40px" : "32px";
  const heightDnsDescription = isMobileView ? "40px" : "22px";

  return (
    <StyledLoader
      lngTZSettings={lngTZSettings}
      portalRenaming={portalRenaming}
      welcomePage={welcomePage}
      dnsSettings={dnsSettings}
      className="category-item-wrapper"
    >
      <Loaders.Rectangle height="22px" className="header" />

      {portalRenaming && (
        <Loaders.Rectangle height="80px" className="description" />
      )}

      {dnsSettings ? (
        <>
          <Loaders.Rectangle
            className="dns-description"
            height={heightDnsDescription}
          />
          <div className="flex">
            <Loaders.Rectangle
              height="16px"
              width="16px"
              className="padding-right"
            />
            <Loaders.Rectangle height="20px" width="135px" />
          </div>
          <Loaders.Rectangle className="dns-field" />
        </>
      ) : (
        <>
          <Loaders.Rectangle height="20px" className="title" />
          <Loaders.Rectangle height="32px" className="combo-box" />
        </>
      )}

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
