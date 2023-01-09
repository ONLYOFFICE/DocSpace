import React from "react";
import Loaders from "@docspace/common/components/Loaders";
import styled from "styled-components";

const StyledLoader = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0px;
  .description {
    max-width: 700px;
  }
  .header {
    margin-bottom: 8px;
  }
  .text-input {
    margin: 12px 0;
  }
`;

const LoaderWhiteLabel = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle height="20px" className="description" />
      <Loaders.Rectangle height="22px" width="95px" className="header" />
      <Loaders.Rectangle height="16px" className="description" />
      <Loaders.Rectangle height="20px" width="291px" />
      <Loaders.Rectangle height="32px" width="350px" className="text-input" />
    </StyledLoader>
  );
};

export default LoaderWhiteLabel;
