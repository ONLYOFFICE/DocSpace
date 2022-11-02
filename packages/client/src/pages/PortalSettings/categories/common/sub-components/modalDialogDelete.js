import React from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import styled from "styled-components";

const StyledModalDialogDelete = styled(ModalDialog)`
  .button-modal {
    width: 50%;
  }
`;

const ModalDialogDelete = (props) => {
  const { visible, onClose, onClickDelete } = props;

  return (
    <StyledModalDialogDelete
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>Delete theme forever?</ModalDialog.Header>
      <ModalDialog.Body>
        The theme will be deleted permanently. You will not be able to undo this
        action.
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="button-modal"
          label="Delete"
          onClick={onClickDelete}
          primary
          size="normal"
        />
        <Button
          className="button-modal"
          label="Cancel"
          size="normal"
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </StyledModalDialogDelete>
  );
};

export default ModalDialogDelete;
