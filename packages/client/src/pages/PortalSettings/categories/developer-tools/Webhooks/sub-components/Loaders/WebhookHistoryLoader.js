import React from "react";

import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";

const LoaderWrapper = styled.div`
  width: 100%;
`;

const NavContainerLoader = styled.nav`
  display: flex;
  justify-content: space-between;
  margin-top: 5px;
  margin-bottom: 17px;
`;

const HistoryHeaderLoader = styled.header`
  display: flex;
  justify-content: space-between;
  margin-bottom: 27px;
`;

const HistoryRowWrapper = styled.div`
  margin-bottom: 27px;

  .historyIconLoader {
    display: inline-block;
    margin-inline-end: 16px;
  }

  .historyContentLoader {
    display: inline-block;
    width: calc(100% - 36px);
  }
`;

export const WebhookHistoryLoader = () => {
  const HistoryRowLoader = () => (
    <HistoryRowWrapper>
      <Loaders.Rectangle
        width="20px"
        height="20px"
        className="historyIconLoader"
      />
      <Loaders.Rectangle height="20px" className="historyContentLoader" />
    </HistoryRowWrapper>
  );

  return (
    <LoaderWrapper>
      <NavContainerLoader>
        <Loaders.Rectangle width="118px" height="22px" />
        <Loaders.Rectangle width="32px" height="22px" />
      </NavContainerLoader>

      <HistoryHeaderLoader>
        <Loaders.Rectangle width="51px" height="16px" />
        <Loaders.Rectangle width="60px" height="16px" />
        <Loaders.Rectangle width="60px" height="16px" />
        <Loaders.Rectangle width="62px" height="16px" />
        <Loaders.Rectangle width="16px" height="16px" />
      </HistoryHeaderLoader>

      <HistoryRowLoader />
      <HistoryRowLoader />
      <HistoryRowLoader />
      <HistoryRowLoader />
      <HistoryRowLoader />
      <HistoryRowLoader />
    </LoaderWrapper>
  );
};
