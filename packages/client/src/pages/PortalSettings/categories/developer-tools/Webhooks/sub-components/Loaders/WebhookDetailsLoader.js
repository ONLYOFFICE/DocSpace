import React from "react";

import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";

const LoaderWrapper = styled.div`
  width: 100%;

  .display-block {
    display: block;
  }

  .mb-4 {
    margin-bottom: 4px;
  }

  .mb-5 {
    margin-bottom: 5px;
  }

  .mb-16 {
    margin-bottom: 16px;
  }

  .mb-23 {
    margin-bottom: 23px;
  }

  .mb-24 {
    margin-bottom: 24px;
  }

  .mr-20 {
    margin-right: 20px;
  }
`;

const DetailsWrapperLoader = styled.div`
  margin-top: 20px;
  margin-bottom: 20px;
`;
const DetailsItemWrapper = styled.div`
  padding: 16px;
  margin-right: 40px;
  display: inline-block;
`;

export const WebhookDetailsLoader = () => {
  const DetailsItemLoader = () => (
    <DetailsItemWrapper>
      <Loaders.Rectangle width="37px" height="16px" className="mb-5 display-block" />
      <Loaders.Rectangle width="180px" height="20px" />
    </DetailsItemWrapper>
  );

  const MessageHeader = () => <Loaders.Rectangle width="130px" height="20px" className="mb-4" />;

  return (
    <LoaderWrapper>
      <DetailsWrapperLoader>
        <Loaders.Rectangle width="80px" height="20px" className="mb-24 display-block" />
        <DetailsItemLoader />
        <DetailsItemLoader />
        <DetailsItemLoader />
        <DetailsItemLoader />
      </DetailsWrapperLoader>
      <div className=" mb-23">
        <Loaders.Rectangle width="43px" height="32px" className="mr-20" />
        <Loaders.Rectangle width="64px" height="32px" />
      </div>

      <MessageHeader />
      <Loaders.Rectangle width="100%" height="212px" className="mb-16" />

      <MessageHeader />
      <Loaders.Rectangle width="100%" height="469px" />
    </LoaderWrapper>
  );
};
