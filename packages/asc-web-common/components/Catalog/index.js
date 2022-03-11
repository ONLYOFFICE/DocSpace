import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import { isMobile, isMobileOnly } from "react-device-detect";
import { Resizable } from "re-resizable";

import {
  isDesktop as isDesktopUtils,
  isTablet as isTabletUtils,
  isMobile as isMobileUtils,
} from "@appserver/components/utils/device";

import SubCatalogBackdrop from "./sub-components/catalog-backdrop";
import SubCatalogHeader from "./sub-components/catalog-header";
import SubCatalogMainButton from "./sub-components/catalog-main-button";
import SubCatalogBody from "./sub-components/catalog-body";

import { StyledCatalog } from "./styled-catalog";

const enable = {
  top: false,
  right: !isMobile,
  bottom: false,
  left: false,
};

const Catalog = ({
  showText,
  setShowText,
  catalogOpen,
  toggleShowText,
  toggleCatalogOpen,
  children,
  ...rest
}) => {
  const [catalogHeaderContent, setCatalogHeaderContent] = React.useState(null);
  const [
    catalogMainButtonContent,
    setCatalogMainButtonContent,
  ] = React.useState(null);
  const [catalogBodyContent, setCatalogBodyContent] = React.useState(null);

  const refTimer = React.useRef(null);

  React.useEffect(() => {
    if (isMobileOnly) {
      window.addEventListener("popstate", hideText);
      return () => window.removeEventListener("popstate", hideText);
    }
  }, [hideText]);

  React.useEffect(() => {
    window.addEventListener("resize", sizeChangeHandler);
    return () => window.removeEventListener("resize", sizeChangeHandler);
  }, []);

  React.useEffect(() => {
    sizeChangeHandler();
  }, []);

  React.useEffect(() => {
    React.Children.forEach(children, (child) => {
      const childType =
        child && child.type && (child.type.displayName || child.type.name);

      switch (childType) {
        case Catalog.Header.displayName:
          setCatalogHeaderContent(child);
          break;
        case Catalog.MainButton.displayName:
          setCatalogMainButtonContent(child);
          break;
        case Catalog.Body.displayName:
          setCatalogBodyContent(child);
          break;
        default:
          break;
      }
    });
  }, [children]);

  const sizeChangeHandler = React.useCallback(() => {
    clearTimeout(refTimer.current);

    refTimer.current = setTimeout(() => {
      if (isMobileOnly || isMobileUtils() || window.innerWidth === 375)
        setShowText(true);
      if (
        ((isTabletUtils() && window.innerWidth !== 375) || isMobile) &&
        !isMobileOnly
      )
        setShowText(false);
      if (isDesktopUtils() && !isMobile) setShowText(true);
    }, 100);
  }, [refTimer.current, setShowText]);

  const hideText = React.useCallback((event) => {
    event.preventDefault;
    setShowText(false);
  }, []);

  return (
    <>
      <StyledCatalog showText={showText} catalogOpen={catalogOpen} {...rest}>
        <Resizable
          defaultSize={{
            width: 256,
          }}
          enable={enable}
          className="resizable-block"
          handleWrapperClass="resizable-border not-selectable"
        >
          <SubCatalogHeader showText={showText} onClick={toggleShowText}>
            {catalogHeaderContent ? catalogHeaderContent.props.children : null}
          </SubCatalogHeader>
          <SubCatalogMainButton showText={showText}>
            {catalogMainButtonContent
              ? catalogMainButtonContent.props.children
              : null}
          </SubCatalogMainButton>
          <SubCatalogBody showText={showText}>
            {catalogBodyContent ? catalogBodyContent.props.children : null}
          </SubCatalogBody>
        </Resizable>
      </StyledCatalog>
      {catalogOpen && (isMobileOnly || window.innerWidth <= 375) && (
        <>
          <SubCatalogBackdrop onClick={toggleCatalogOpen} />
        </>
      )}
    </>
  );
};

Catalog.propTypes = {
  showText: PropTypes.bool,
  setShowText: PropTypes.func,
  catalogOpen: PropTypes.bool,
  toggleCatalogOpen: PropTypes.func,
  children: PropTypes.any,
};

Catalog.Header = () => {
  return null;
};
Catalog.Header.displayName = "Header";

Catalog.MainButton = () => {
  return null;
};
Catalog.MainButton.displayName = "MainButton";

Catalog.Body = () => {
  return null;
};
Catalog.Body.displayName = "Body";

export default inject(({ auth }) => {
  const { isLoaded, settingsStore } = auth;

  const {
    showText,
    setShowText,
    catalogOpen,
    toggleShowText,
    toggleCatalogOpen,
  } = settingsStore;

  return {
    showText,
    setShowText,
    catalogOpen,
    toggleShowText,
    toggleCatalogOpen,
  };
})(observer(Catalog));
