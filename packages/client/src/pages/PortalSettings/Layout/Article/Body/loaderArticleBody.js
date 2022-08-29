import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { isTablet as isTabletUtils } from "@docspace/components/utils/device";
import { isTablet } from "react-device-detect";

const StyledLoader = styled.div`
  padding-left: 8px;

  .loader {
    padding-bottom: 12px;
  }

  .section-name-container {
    padding: 1px 3px 29px;
    display: flex;
    justify-content: flex-start;
  }

  .section-name {
    width: 20px;
    height: 1px;
    line-height: 1px;
    background: #d0d5da;
    margin: 0px;
  }

  @media (min-width: 1024px) {
    padding-left: 10px;
    padding-top: 20px;
    display: flex;
    flex-direction: column;

    .section-name-loader {
      margin-bottom: 12px;
    }

    .loader {
      padding-bottom: 16px;
    }
  }
`;

const LoaderArticleBody = () => {
  const [isTabletView, setIsTabletView] = useState(false);

  const checkInnerWidth = () => {
    const isTabletView = window.innerWidth <= 1024 || isTablet;

    if (isTabletView) {
      setIsTabletView(true);
    } else {
      setIsTabletView(false);
    }
  };

  useEffect(() => {
    checkInnerWidth();
    window.addEventListener("resize", checkInnerWidth);

    return () => window.removeEventListener("resize", checkInnerWidth);
  });

  const height = isTabletView ? "28px" : "20px";
  const width = isTabletView ? "28px" : "187px";

  return (
    <StyledLoader>
      {isTabletView ? (
        <div className="section-name-container">
          <p className="section-name"></p>
        </div>
      ) : (
        <Loaders.Rectangle
          width={"42px"}
          height={"12px"}
          className="section-name-loader"
        />
      )}
      <Loaders.Rectangle width={width} height={height} className="loader" />
      <Loaders.Rectangle width={width} height={height} className="loader" />
      <Loaders.Rectangle width={width} height={height} className="loader" />
      <Loaders.Rectangle width={width} height={height} className="loader" />
      <Loaders.Rectangle width={width} height={height} className="loader" />
      <Loaders.Rectangle width={width} height={height} className="loader" />
      <Loaders.Rectangle width={width} height={height} className="loader" />
    </StyledLoader>
  );
};

export default LoaderArticleBody;
