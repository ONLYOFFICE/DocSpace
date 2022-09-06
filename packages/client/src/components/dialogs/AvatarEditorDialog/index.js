import React from "react";
import styled from "styled-components";

import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";

const StyledModalDialog = styled(ModalDialog)``;

const AvatarEditorDialog = (props) => {
  const { t, visible, onClose, onSave } = props;

  return (
    <StyledModalDialog
      displayType="aside"
      withBodyScroll
      visible={visible}
      onClose={onClose}
      withFooterBorder
    >
      <ModalDialog.Header>
        <Text fontSize="21px" fontWeight={700}>
          {t("EditPhoto")}
        </Text>
      </ModalDialog.Header>
      <ModalDialog.Body></ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="AvatarEditorSaveBtn"
          label={t("Common:SaveButton")}
          size="normal"
          scale
          primary={true}
          onClick={onSave}
        />
        <Button
          key="AvatarEditorCloseBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default AvatarEditorDialog;
