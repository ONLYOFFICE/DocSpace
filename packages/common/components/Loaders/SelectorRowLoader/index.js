import React from "react";
import styled, { css } from "styled-components";

import RectangleLoader from "../RectangleLoader/RectangleLoader";

const StyledContainer = styled.div`
  width: 100%;
  height: 100%;

  overflow: hidden;

  display: flex;
  flex-direction: column;
`;

const StyledItem = styled.div`
  width: 100%;
  height: 48px;
  min-height: 48px;

  padding: 0 16px;

  box-sizing: border-box;

  display: flex;
  align-items: center;

  .avatar {
    margin-right: 8px;

    ${(props) =>
      props.isUser &&
      css`
        border-radius: 50px;
      `}
  }

  .checkbox {
    margin-left: auto;
  }
`;

const SelectorRowLoader = ({
  id,
  className,
  style,
  isMultiSelect,
  isContainer,
  isUser,
  ...rest
}) => {
  const getRowItem = (key) => {
    return (
      <StyledItem
        id={id}
        className={className}
        style={style}
        isMultiSelect={isMultiSelect}
        isUser={isUser}
        key={key}
        {...rest}
      >
        <RectangleLoader className={"avatar"} width={"32px"} height={"32px"} />
        <RectangleLoader width={"212px"} height={"16px"} />
        {isMultiSelect && (
          <RectangleLoader
            className={"checkbox"}
            width={"16px"}
            height={"16px"}
          />
        )}
      </StyledItem>
    );
  };

  const getRowItems = () => {
    const rows = [];
    for (let i = 0; i < 5; i++) {
      rows.push(getRowItem(i));
    }

    return rows;
  };

  return isContainer ? (
    <StyledContainer id={id} className={className} style={style} {...rest}>
      {getRowItems()}
    </StyledContainer>
  ) : (
    getRowItem()
  );
};

export default SelectorRowLoader;
