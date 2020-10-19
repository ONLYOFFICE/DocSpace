import React, { memo } from "react";
import styled from "styled-components";
import { TextInput, Button } from "asc-web-components";

const EditingWrapper = styled.div`
  width: 100%;
  display: inline-flex;
  align-items: center;

  @media (max-width: 1024px) {
    height: 56px;
  }
  .edit-text {
    height: 30px;
    font-size: 15px;
    outline: 0 !important;
    font-weight: 600;
    margin: 0;
    font-family: "Open Sans", sans-serif, Arial;
    text-align: left;
    color: #333333;
  }
  .edit-button {
    margin-left: 8px;
    height: 30px;
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
  }
`;

const EditingWrapperComponent = (props) => {
  const {
    itemTitle,
    itemId,
    okIcon,
    cancelIcon,
    renameTitle,
    onClickUpdateItem,
    cancelUpdateItem,
    isLoading,
  } = props;

  const onUpdate = (e) => {
    onClickUpdateItem(e);
  };

  const onCancel = (e) => {
    cancelUpdateItem(e);
  };

  const onKeyUpUpdateItem = (e) => {
    if (e.keyCode === 13) {
      onClickUpdateItem(e);
    }

    if (e.keyCode === 27) return cancelUpdateItem(e);
  };

  return (
    <EditingWrapper>
      <TextInput
        className="edit-text"
        name="title"
        scale={true}
        value={itemTitle}
        tabIndex={1}
        isAutoFocussed={true}
        onChange={renameTitle}
        onKeyUp={onKeyUpUpdateItem}
        isDisabled={isLoading}
        data-itemid={itemId}
      />
      <Button
        className="edit-button"
        size="medium"
        isDisabled={isLoading}
        onClick={onUpdate}
        icon={okIcon}
        data-itemid={itemId}
      />
      <Button
        className="edit-button"
        size="medium"
        isDisabled={isLoading}
        onClick={onCancel}
        icon={cancelIcon}
        data-itemid={itemId}
      />
    </EditingWrapper>
  );
};

export default memo(EditingWrapperComponent);
