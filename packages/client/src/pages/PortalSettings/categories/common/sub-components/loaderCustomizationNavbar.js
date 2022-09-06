import React from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";

const StyledLoader = styled.div`
  .section {
    padding-bottom: 16px;
  }
  .title-long {
    width: 283px;
    padding-bottom: 4px;
  }

  .width {
    width: 100%;
  }

  .padding-bottom {
    padding-bottom: 4px;
  }

  .padding-top {
    padding-top: 5px;
  }

  .display {
    display: block;
  }
`;

const LoaderCustomizationNavbar = () => {
  return (
    <StyledLoader>
      <div className="section">
        <Loaders.Rectangle height="22px" className="title-long" />
        <Loaders.Rectangle height="80px" className="width padding-bottom" />
        <Loaders.Rectangle height="20px" width="73px" />
      </div>

      <div className="section">
        <Loaders.Rectangle
          height="22px"
          width="201px"
          className="title padding-bottom display"
        />
        <Loaders.Rectangle height="80px" className="width" />
      </div>

      <div className="section">
        <Loaders.Rectangle
          height="22px"
          width="119px"
          className="padding-top"
        />
        <Loaders.Rectangle height="40px" className="width" />
        <Loaders.Rectangle
          height="20px"
          width="73px"
          className="width padding-top"
        />
      </div>

      <div className="section">
        <Loaders.Rectangle
          height="22px"
          width="150px"
          className="title padding-bottom display"
        />
        <Loaders.Rectangle
          height="20px"
          width="253px"
          className="padding-top"
        />
      </div>
    </StyledLoader>
  );
};

export default LoaderCustomizationNavbar;
