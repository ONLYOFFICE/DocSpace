import React from "react";
import styled from "styled-components";
import { withRouter } from "react-router";

import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";

import Headline from "@docspace/common/components/Headline";
import IconButton from "@docspace/components/icon-button";
import { Hint } from "../../styled-components";

import { tablet } from "@docspace/components/utils/device";

const HeaderContainer = styled.div`
  position: relative;
  display: flex;
  align-items: center;
  max-width: calc(100vw - 32px);

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

const HistoryHeader = (props) => {
  const { history } = props;
  const onBack = () => {
    history.goBack();
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
        History
      </Headline>
      <Hint backgroundColor="#F8F9F9" color="#555F65">
        Deliveries are automatically deleted after 15 days
      </Hint>
    </HeaderContainer>
  );
};

export default withRouter(HistoryHeader);
