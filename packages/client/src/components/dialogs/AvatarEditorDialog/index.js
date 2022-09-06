import React from "react";
import styled from "styled-components";

import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";

const StyledModalDialog = styled(ModalDialog)``;

const AvatarEditorDialog = (props) => {
  const { t, visible, onClose } = props;

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
      <ModalDialog.Footer></ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default AvatarEditorDialog;
