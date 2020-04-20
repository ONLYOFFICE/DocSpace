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
  font-weight: bold;
  margin: 0;
  font-family: 'Open Sans',sans-serif,Arial;
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

const EditingWrapperComponent = props => {
    const { isLoading, itemTitle, okIcon, cancelIcon, renameTitle, onKeyUpUpdateItem, onClickUpdateItem, cancelUpdateItem } = props;

    return(
      <EditingWrapper>
        <TextInput
          className='edit-text'
          name='title'
          scale={true}
          value={itemTitle}
          tabIndex={1}
          isAutoFocussed={true}
          onChange={renameTitle}
          onKeyUp={onKeyUpUpdateItem}
          isDisabled={isLoading}
        />
        <Button
          className='edit-button'
          size='medium'
          isDisabled={isLoading}
          onClick={onClickUpdateItem}
          icon={okIcon}
        />
        <Button
          className='edit-button'
          size='medium'
          isDisabled={isLoading}
          onClick={cancelUpdateItem}
          icon={cancelIcon}
        />
      </EditingWrapper>
    )
}

export default memo(EditingWrapperComponent);