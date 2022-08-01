import React, { useState } from "react";
import styled, { css } from "styled-components";
import Button from "@docspace/components/button";
import TextInput from "@docspace/components/text-input";
import Text from "@docspace/components/text";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import CheckIcon from "../../public/images/check.react.svg";
import CrossIcon from "PUBLIC_DIR/images/cross.react.svg";
import { tablet } from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";

const StyledCheckIcon = styled(CheckIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.filesEditingWrapper.fill} !important;
  }
  :hover {
    fill: ${(props) => props.theme.filesEditingWrapper.hoverFill} !important;
  }
`;

StyledCheckIcon.defaultProps = { theme: Base };

const StyledCrossIcon = styled(CrossIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.filesEditingWrapper.fill} !important;
  }
  :hover {
    fill: ${(props) => props.theme.filesEditingWrapper.hoverFill} !important;
  }
`;

StyledCrossIcon.defaultProps = { theme: Base };

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

      border-bottom: ${(props) => props.theme.filesEditingWrapper.borderBottom};
      padding-bottom: 4px;
      margin-top: 4px;
    `}

  ${(props) =>
    props.viewAs === "tile" &&
    css`
      position: absolute;
      width: calc(100% - 18px);
      z-index: 1;
      gap: 4px;

      background-color: ${(props) =>
        props.theme.filesEditingWrapper.tile.background};

      border: ${(props) => props.theme.filesEditingWrapper.border};
      border-radius: ${(props) => (props.isFolder ? "6px" : "0 0 6px 6px")};

      height: 43px;
      bottom: 0;
      left: 0;
      right: 0;
      padding: 9px 8px 9px 8px;
    `}


  @media ${tablet} {
    height: ${(props) => (props.viewAs === "tile" ? "43px" : "56px")};
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
    color: ${(props) => props.theme.filesEditingWrapper.color};
    background: ${(props) =>
      props.theme.filesEditingWrapper.row.itemBackground} !important;

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
        background: none !important;
      `}

    ${(props) => props.viewAs === "table" && `padding-left: 12px`}

    ${(props) =>
      props.viewAs === "tile" &&
      !props.isUpdatingRowItem &&
      css`
        background: ${(props) =>
          props.theme.filesEditingWrapper.tile.itemBackground};
        border: ${(props) =>
          `1px solid ${props.theme.filesEditingWrapper.tile.itemBorder}`};

        &:focus {
          border: ${(props) =>
            `1px solid ${props.theme.filesEditingWrapper.tile.itemActiveBorder}`};
        }
      `};

    ${({ isDisabled }) => isDisabled && "background-color: #fff"}
  }

  .edit-button {
    margin-left: 8px;
    height: 32px;
    padding: 0px 7px 0px 7px;

    ${(props) =>
      props.viewAs === "tile" &&
      css`
        margin-left: 0px;
        background: ${(props) =>
          props.theme.filesEditingWrapper.tile.itemBackground};
        border: ${(props) =>
          `1px solid ${props.theme.filesEditingWrapper.tile.itemBorder}`};

        &:hover {
          border: ${(props) =>
            `1px solid ${props.theme.filesEditingWrapper.tile.itemActiveBorder}`};
        }
      `};

    ${(props) =>
      props.viewAs === "table" &&
      css`
        width: 24px;
        height: 24px;
        border: 1px transparent;
        padding: 0;
        display: flex;
        align-items: center;
        justify-content: center;

        &:hover {
          border: ${(props) => props.theme.filesEditingWrapper.border};
        }
      `}
  }

  .edit-ok-icon {
    width: 16px;
    height: 16px;
  }

  .edit-cancel-icon {
    width: 14px;
    height: 14px;
    padding: 1px;
  }
`;

EditingWrapper.defaultProps = { theme: Base };

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
    isFolder,
  } = props;

  const isTable = viewAs === "table";

  const [OkIconIsHovered, setIsHoveredOk] = useState(false);
  const [CancelIconIsHovered, setIsHoveredCancel] = useState(false);
  const [isTouchOK, setIsTouchOK] = useState(false);
  const [isTouchCancel, setIsTouchCancel] = useState(false);

  const [isLoading, setIsLoading] = useState(false);

  const inputRef = React.useRef(null);

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
    if (
      (e.relatedTarget && e.relatedTarget.classList.contains("edit-button")) ||
      OkIconIsHovered ||
      CancelIconIsHovered ||
      isTouchOK ||
      isTouchCancel
    )
      return false;

    if (!document.hasFocus() && inputRef.current === e.target) return false;

    !passwordEntryProcess && onClickUpdateItem(e, false);
  };

  return (
    <EditingWrapper
      viewAs={viewAs}
      isUpdatingRowItem={isUpdatingRowItem && !isTable}
      isFolder={isFolder}
      isDisabled={isLoading}
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
          forwardedRef={inputRef}
        />
      )}
      {!isUpdatingRowItem && (
        <>
          <Button
            className="edit-button not-selectable"
            size="small"
            isDisabled={isLoading}
            onClick={onClickUpdateItem}
            icon={okIcon}
            data-itemid={itemId}
            onMouseEnter={setIsHoveredOkHandler}
            onMouseLeave={setIsHoveredOkHandler}
            onTouchStart={() => setIsTouchOK(true)}
            isHovered={OkIconIsHovered}
            title=""
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
            onTouchStart={() => setIsTouchCancel(true)}
            isHovered={CancelIconIsHovered}
            title=""
          />
        </>
      )}
    </EditingWrapper>
  );
};

export default EditingWrapperComponent;
