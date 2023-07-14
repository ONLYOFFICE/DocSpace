import Heading from "@docspace/components/heading";
import React from "react";
import styled from "styled-components";

const StyledBlock = styled.div`
  width: 100%;
  height: auto;

  margin-bottom: 32px;

  .block-header {
    margin-bottom: 32px;
  }

  .block-body {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }
`;

const Block = ({ headerText, headerTitle, children }) => {
  return (
    <StyledBlock>
      <Heading
        className={"block-header"}
        title={headerTitle}
        size={"medium"}
        truncate
      >
        {headerText}
      </Heading>
      <div className="block-body">{children}</div>
    </StyledBlock>
  );
};

export default Block;
