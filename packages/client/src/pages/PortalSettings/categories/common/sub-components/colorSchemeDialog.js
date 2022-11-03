import React from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import styled from "styled-components";
import Button from "@docspace/components/button";

const StyledComponent = styled(ModalDialog)`
  .modal-dialog-aside-footer {
    width: 100%;
    bottom: 0 !important;
    left: 0;
    padding: 16px;
    box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
  }

  .flex {
    display: flex;
    justify-content: space-between;

    :not(:last-child) {
      padding-bottom: 20px;
    }
  }

  .text {
    font-weight: 700;
    font-size: 18px;
    line-height: 24px;
  }

  .relative {
    position: relative;
  }

  .box {
    width: 46px;
    height: 46px;
    border-radius: 8px;
    cursor: pointer;
  }

  .accent-box {
    background: ${(props) =>
      props.currentColorAccent
        ? props.currentColorAccent
        : `#eceef1 url("/static/images/plus.theme.svg") no-repeat center`};
  }

  .buttons-box {
    background: ${(props) =>
      props.currentColorButtons
        ? props.currentColorButtons
        : `#eceef1 url("/static/images/plus.theme.svg") no-repeat center`};
  }

  .modal-add-theme {
    width: 46px;
    height: 46px;
    border-radius: 8px;
    cursor: pointer;
  }
`;

const ColorSchemeDialog = (props) => {
  const {
    visible,
    onClose,
    header,
    nodeHexColorPickerAccent,
    nodeHexColorPickerButtons,
    viewMobile,
    showSaveButtonDialog,
    onSaveColorSchemeDialog,
    t,
    onClickColor,
    currentColorAccent,
    currentColorButtons,
  } = props;

  return (
    <StyledComponent
      visible={visible}
      onClose={onClose}
      displayType="aside"
      currentColorAccent={currentColorAccent}
      currentColorButtons={currentColorButtons}
    >
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <div>
          <div className="flex relative">
            <div className="text">Accent</div>
            <div
              id="accent"
              className="modal-add-theme accent-box"
              onClick={onClickColor}
            />

            {!viewMobile && nodeHexColorPickerAccent}
          </div>

          <div className="flex relative">
            <div className="text">Buttons</div>
            <div
              id="buttons"
              className="modal-add-theme buttons-box"
              onClick={onClickColor}
            />

            {!viewMobile && nodeHexColorPickerButtons}
          </div>
        </div>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        {showSaveButtonDialog && (
          <>
            <Button
              label="Save"
              size="normal"
              primary
              scale
              onClick={onSaveColorSchemeDialog}
            />
            <Button label="Cancel" size="normal" scale onClick={onClose} />
          </>
        )}
      </ModalDialog.Footer>
    </StyledComponent>
  );
};

export default ColorSchemeDialog;
