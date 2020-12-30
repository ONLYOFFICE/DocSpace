import React from "react";
import styled from "styled-components";
import { Headline } from "asc-web-common";
import { IconButton, utils } from "asc-web-components";

const { desktop } = utils.device;

const StyledContainer = styled.div`
  display: grid;
  grid-template-columns: auto 1fr auto auto;
  align-items: center;

  .arrow-button {
    margin-left: -8px;
    margin-right: 15px;
    min-width: 17px;

    @media (max-width: 1024px) {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }

  .headline-header {
    @media ${desktop} {
      margin-left: -9px;
    }
  }
`;

const SectionHeaderContent = (props) => {
  const { title, onClickBack } = props;

  return (
    <StyledContainer>
      <IconButton
        iconName="ArrowPathIcon"
        size="17"
        color="#A3A9AE"
        hoverColor="#657077"
        isFill={true}
        onClick={onClickBack}
        className="arrow-button"
      />

      <Headline className="headline-header" type="content" truncate={true}>
        {title}
      </Headline>
    </StyledContainer>
  );
};

export default SectionHeaderContent;
