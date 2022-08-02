import React from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";

const StyledLoader = styled.div`
  .title-long {
    width: 283px;
    padding-bottom: 8px;
  }

  .width {
    width: 100%;
    padding-bottom: 8px;
  }

  .link {
    width: 57px;
    padding-bottom: 20px;
  }

  .title {
    display: block;
    width: 132px;
    padding-bottom: 8px;
  }
`;

const LoaderCustomizationNavbar = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle height="22px" className="title-long" />
      <Loaders.Rectangle height="100px" className="width" />
      <Loaders.Rectangle height="20px" className="link" />

      <Loaders.Rectangle height="22px" className="title" />
      <Loaders.Rectangle height="80px" className="width" />

      <Loaders.Rectangle height="22px" className="title" />
      <Loaders.Rectangle height="20px" className="width" />
    </StyledLoader>
  );
};

export default LoaderCustomizationNavbar;
