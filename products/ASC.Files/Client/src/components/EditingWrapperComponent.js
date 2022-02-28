import React, { useState } from "react";
import styled, { css } from "styled-components";
import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import Text from "@appserver/components/text";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";

import CheckIcon from "../../public/images/check.react.svg";
import CrossIcon from "../../../../../public/images/cross.react.svg";
import { tablet } from "@appserver/components/utils/device";

const StyledCheckIcon = styled(CheckIcon)`
  ${commonIconsStyles}
  path {
    fill: #a3a9ae;
  }
  :hover {
    fill: #657077;
  }
`;

const StyledCrossIcon = styled(CrossIcon)`
  ${commonIconsStyles}
  path {
    fill: #a3a9ae;
  }
  :hover {
    fill: #657077;
  }
`;

export const okIcon = <StyledCheckIcon className="edit-ok-icon" size="scale" />;
export const cancelIcon = (
  <StyledCrossIcon className="edit-cancel-icon" size="scale" />
);

const EditingWrapper = styled.div`
  width: 100%;
  display: inline-flex;
  align-items: center;

  ${(props) =>
    props.viewAs === "table" &&
    css`
      grid-column-start: 1;
      grid-column-end: -1;

      border-bottom: 1px solid #eceef1;
      padding-bottom: 4px;
      margin-top: 4px;

      /* margin-left: -4px; */
    `}

  ${(props) =>
    props.viewAs === "tile" &&
    `margin-right: 10px !important; margin-left: 8px;`}
  
  
  @media ${tablet} {
    height: 56px;
  }

  .edit-text {
    height: 32px;
    font-size: ${(props) =>
      props.viewAs === "table"
        ? "13px"
        : props.viewAs === "tile"
        ? "14px"
        : "15px"};
    outline: 0 !important;
    font-weight: 600;
    margin: 0;
    font-family: "Open Sans", sans-serif, Arial;
    text-align: left;
    color: #333333;
    ${(props) =>
      props.viewAs === "tile" &&
      css`
        margin-right: 2px;
        border: none;
        background: none;
      `};
    ${(props) =>
      props.isUpdatingRowItem &&
      css`
        margin-left: 0;
        display: flex;
        align-items: center;
      `}

    ${(props) => props.viewAs === "table" && `padding-left: 12px`}
  }

  .edit-button {
    margin-left: 8px;
    height: 32px;
    padding: 8px 7px 7px 7px;

    ${(props) =>
      props.viewAs === "tile" &&
      css`
        margin-left: 0px;
        background: none;
        border: 1px solid transparent;

        :hover {
          border-color: #d0d5da;
        }

        &:last-child {
          margin-left: 2px;
        }
      `};

    ${(props) =>
      props.viewAs === "table" &&
      css`
        width: 24px;
        height: 24px;
        border: 1px transparent;
        padding: 4px 0 0 0;

        :hover {
          border: 1px solid #d0d5da;
        }
      `}
  }

  .edit-ok-icon {
    margin-top: -6px;
    width: 16px;
    height: 16px;
  }

  .edit-cancel-icon {
    margin-top: -6px;
    width: 14px;
    height: 14px;
    padding: 1px;
  }

  .is-edit {
    /* margin-top: 4px; */
    ${(props) => props.viewAs === "table" && `padding-left: 4px;`}
  }
`;

const EditingWrapperComponent = (props) => {
  const {
    itemTitle,
    itemId,
    renameTitle,
    onClickUpdateItem,
    cancelUpdateItem,
    //isLoading,
    viewAs,
    elementIcon,
    isUpdatingRowItem,
    passwordEntryProcess,
  } = props;

  const isTable = viewAs === "table";

  const [OkIconIsHovered, setIsHoveredOk] = useState(false);
  const [CancelIconIsHovered, setIsHoveredCancel] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const onKeyUpUpdateItem = (e) => {
    if (isLoading) return;

    var code = e.keyCode || e.which;
    if (code === 13) {
      if (!isLoading) setIsLoading(true);
      return onClickUpdateItem(e);
    }
  };
  const onEscapeKeyPress = (e) => {
    if (e.keyCode === 27) return cancelUpdateItem(e);
  };

  const setIsHoveredOkHandler = () => {
    setIsHoveredOk(!OkIconIsHovered);
  };

  const setIsHoveredCancelHandler = () => {
    setIsHoveredCancel(!CancelIconIsHovered);
  };

  const onFocus = (e) => e.target.select();
  const onBlur = (e) => {
    if (e.relatedTarget && e.relatedTarget.classList.contains("edit-button"))
      return false;

    !passwordEntryProcess && onClickUpdateItem(e, false);
  };

  return (
    <EditingWrapper
      viewAs={viewAs}
      isUpdatingRowItem={isUpdatingRowItem && !isTable}
    >
      {isTable && elementIcon}
      {isUpdatingRowItem && !isTable ? (
        <Text className="edit-text">{itemTitle}</Text>
      ) : (
        <TextInput
          className="edit-text"
          name="title"
          scale={true}
          value={itemTitle}
          tabIndex={1}
          isAutoFocussed={true}
          onChange={renameTitle}
          onKeyPress={onKeyUpUpdateItem}
          onKeyDown={onEscapeKeyPress}
          onFocus={onFocus}
          onBlur={onBlur}
          isDisabled={isLoading}
          data-itemid={itemId}
          withBorder={!isTable}
        />
      )}
      {!isUpdatingRowItem && (
        <>
          <Button
            className="edit-button not-selectable"
            size="medium"
            isDisabled={isLoading}
            onClick={onClickUpdateItem}
            icon={okIcon}
            data-itemid={itemId}
            onMouseEnter={setIsHoveredOkHandler}
            onMouseLeave={setIsHoveredOkHandler}
            isHovered={OkIconIsHovered}
          />
          <Button
            className="edit-button not-selectable"
            size="medium"
            isDisabled={isLoading}
            onClick={cancelUpdateItem}
            icon={cancelIcon}
            data-itemid={itemId}
            data-action="cancel"
            onMouseEnter={setIsHoveredCancelHandler}
            onMouseLeave={setIsHoveredCancelHandler}
            isHovered={CancelIconIsHovered}
          />
        </>
      )}
    </EditingWrapper>
  );
};

export default EditingWrapperComponent;
