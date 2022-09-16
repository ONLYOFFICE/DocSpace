import React from "react";
import Loaders from "@docspace/common/components/Loaders";
import styled from "styled-components";

const StyledLoader = styled.div`
  margin-top: 40px;

  .item {
    padding-bottom: 15px;
  }

  .loader-header {
    display: block;
  }

  .flex {
    display: flex;
    align-items: center;
  }

  .checkbox {
    padding-right: 8px;
  }

  .button {
    padding-top: 10px;
  }

  .save {
    padding-right: 8px;
  }
`;

const LoaderAdditionalResources = () => {
  return (
    <StyledLoader>
      <div className="item">
        <Loaders.Rectangle
          width="166px"
          height="22px"
          className="loader-header"
        />
      </div>

      <div className="item">
        <Loaders.Rectangle
          width="700px"
          height="20px"
          className="loader-description"
        />
      </div>

      <div className="item">
        <div className="flex">
          <Loaders.Rectangle width="16px" height="16px" className="checkbox" />
          <Loaders.Rectangle width="166px" height="20px" />
        </div>
      </div>

      <div className="item">
        <div className="flex">
          <Loaders.Rectangle width="16px" height="16px" className="checkbox" />
          <Loaders.Rectangle width="150px" height="20px" />
        </div>
      </div>

      <div className="item">
        <div className="flex">
          <Loaders.Rectangle width="16px" height="16px" className="checkbox" />
          <Loaders.Rectangle width="157px" height="20px" />
        </div>
      </div>

      <div className="button">
        <Loaders.Rectangle width="86px" height="32px" className="save" />
        <Loaders.Rectangle width="170px" height="32px" />
      </div>
    </StyledLoader>
  );
};

export default LoaderAdditionalResources;
