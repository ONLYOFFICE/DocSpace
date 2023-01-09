import React from "react";
import Loaders from "@docspace/common/components/Loaders";
import styled from "styled-components";

const StyledLoader = styled.div`
  .item {
    padding-bottom: 12px;
  }

  .loader-header {
    padding-bottom: 5px;
    display: block;
  }

  .loader-label {
    padding-bottom: 4px;
    display: block;
  }

  .button {
    padding-top: 8px;
  }

  .save {
    padding-right: 8px;
  }
`;

const LoaderCompanyInfoSettings = () => {
  return (
    <StyledLoader>
      <div className="item">
        <Loaders.Rectangle
          width="179px"
          height="22px"
          className="loader-header"
        />
        <Loaders.Rectangle width="419px" height="22px" />
      </div>

      <div className="item">
        <Loaders.Rectangle
          width="99px"
          height="20px"
          className="loader-label"
        />
        <Loaders.Rectangle width="433px" height="32px" />
      </div>

      <div className="item">
        <Loaders.Rectangle
          width="35px"
          height="20px"
          className="loader-label"
        />
        <Loaders.Rectangle width="433px" height="32px" />
      </div>

      <div className="item">
        <Loaders.Rectangle
          width="40px"
          height="20px"
          className="loader-label"
        />
        <Loaders.Rectangle width="433px" height="32px" />
      </div>

      <div className="item">
        <Loaders.Rectangle
          width="51px"
          height="20px"
          className="loader-label"
        />
        <Loaders.Rectangle width="433px" height="32px" />
      </div>

      <div className="item">
        <Loaders.Rectangle
          width="51px"
          height="20px"
          className="loader-label"
        />
        <Loaders.Rectangle width="433px" height="32px" />
      </div>

      <div className="button">
        <Loaders.Rectangle width="86px" height="32px" className="save" />
        <Loaders.Rectangle width="170px" height="32px" />
      </div>
    </StyledLoader>
  );
};

export default LoaderCompanyInfoSettings;
