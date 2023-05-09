import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import Toast from "@docspace/components/toast";
import toastr from "@docspace/components/toast/toastr";

import { useNavigate } from "react-router-dom";

import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";
import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";

import Headline from "@docspace/common/components/Headline";
import IconButton from "@docspace/components/icon-button";

import { tablet } from "@docspace/components/utils/device";

const HeaderContainer = styled.div`
  position: relative;
  display: flex;
  align-items: center;
  max-width: calc(100vw - 32px);
  min-height: 69px;

  .arrow-button {
    margin-right: 18.5px;

    @media ${tablet} {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }

  .headline {
    font-size: 18px;
    margin-right: 16px;
  }
`;

const DetailsNavigationHeader = (props) => {
  const { eventId, retryWebhookEvent } = props;
  const navigate = useNavigate();
  const onBack = () => {
    navigate(-1);
  };
  const handleRetryEvent = async () => {
    await retryWebhookEvent(eventId);
    toastr.success("Webhook retry again", <b>Done</b>);
  };

  return (
    <HeaderContainer>
      <IconButton
        iconName={ArrowPathReactSvgUrl}
        size="17"
        isFill={true}
        onClick={onBack}
        className="arrow-button"
      />
      <Headline type="content" truncate={true} className="headline">
        Webhook details
      </Headline>
      <IconButton iconName={RetryIcon} size="17" isFill={true} onClick={handleRetryEvent} />

      <Toast />
    </HeaderContainer>
  );
};

export default inject(({ webhooksStore }) => {
  const { retryWebhookEvent } = webhooksStore;

  return { retryWebhookEvent };
})(observer(DetailsNavigationHeader));
