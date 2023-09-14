import React from "react";
import Loaders from "@docspace/common/components/Loaders";
import styled from "styled-components";

const StyledLoader = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0px;

  .description_loader {
    max-width: 700px;
    margin-bottom: 16px;
  }

  .header_loader {
    margin-bottom: 8px;
  }

  .text-input_loader {
    margin: 12px 0;
  }
`;

const LoaderWhiteLabel = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle height="20px" className="description_loader" />
      <Loaders.Rectangle height="22px" width="95px" className="header_loader" />
      <Loaders.Rectangle height="16px" className="description_loader" />
      <Loaders.Rectangle height="20px" width="291px" />
      <Loaders.Rectangle
        height="32px"
        width="350px"
        className="text-input_loader"
      />
    </StyledLoader>
  );
};

export default LoaderWhiteLabel;
