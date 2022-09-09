import React, { useEffect, useState } from "react";
import styled, { css } from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { isTablet, isMobileOnly } from "react-device-detect";

const StyledLoader = styled.div`
  display: flex;
  align-items: center;

  .arrow {
    padding-right: 12px;
  }

  padding: ${(props) =>
    props.isTabletView
      ? "16px 0 17px"
      : props.isDesktopView
      ? "21px 0 23px"
      : "14px 0 0"};
`;

const LoaderSectionHeader = () => {
  const [isTabletView, setIsTabletView] = useState(false);
  const [isDesktopView, setIsDesktopView] = useState(false);

  const height = isTabletView ? "28" : "24";
  const width = isTabletView ? "163" : "140";

  const levelSettings = location.pathname.split("/").length - 1;

  const checkInnerWidth = () => {
    const isTabletView =
      (window.innerWidth >= 600 && window.innerWidth <= 1024) ||
      (isTablet && !isMobileOnly);

    const isDesktopView = window.innerWidth > 1024;

    if (isTabletView) {
      setIsTabletView(true);
    } else {
      setIsTabletView(false);
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

  return (
    <StyledLoader isTabletView={isTabletView} isDesktopView={isDesktopView}>
      {levelSettings === 4 && (
        <Loaders.Rectangle width="17" height="17" className="arrow" />
      )}

      <Loaders.Rectangle width={width} height={height} className="loader" />
    </StyledLoader>
  );
};

export default LoaderSectionHeader;
