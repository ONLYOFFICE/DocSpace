import React from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";

const StyledLoader = styled.div`
  width: 100%;

  .block {
    display: block;
  }

  .padding-bottom {
    padding-bottom: 16px;
  }

  .flex {
    display: flex;
  }

  .padding-right {
    padding-right: 12px;
  }

  .preview-title {
    padding-bottom: 8px;
  }

  .preview {
    width: 100%;
    max-width: 575px;
    padding-top: 12px;
    padding-bottom: 32px;
  }
`;

const Loader = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle
        height="24px"
        width="93px"
        className="block padding-bottom"
      />
      <Loaders.Rectangle
        height="16px"
        width="118px"
        className="block padding-bottom"
      />
      <div className="flex padding-bottom">
        <Loaders.Rectangle
          height="46px"
          width="46px"
          className="padding-right"
        />
        <Loaders.Rectangle
          height="46px"
          width="46px"
          className="padding-right"
        />
        <Loaders.Rectangle
          height="46px"
          width="46px"
          className="padding-right"
        />
        <Loaders.Rectangle
          height="46px"
          width="46px"
          className="padding-right"
        />
        <Loaders.Rectangle
          height="46px"
          width="46px"
          className="padding-right"
        />
        <Loaders.Rectangle
          height="46px"
          width="46px"
          className="padding-right"
        />
        <Loaders.Rectangle
          height="46px"
          width="46px"
          className="padding-right"
        />
      </div>
      <Loaders.Rectangle
        height="16px"
        width="118px"
        className="block padding-bottom"
      />
      <Loaders.Rectangle
        height="46px"
        width="46px"
        className="block padding-bottom"
      />
      <Loaders.Rectangle
        height="24px"
        width="93px"
        className="block preview-title"
      />
      <Loaders.Rectangle height="32px" width="211px" className="block" />
      <Loaders.Rectangle height="325px" className="block preview" />
      <Loaders.Rectangle height="32px" width="447px" className="block" />
    </StyledLoader>
  );
};

export default Loader;
