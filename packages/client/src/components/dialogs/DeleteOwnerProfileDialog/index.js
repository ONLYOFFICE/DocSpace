import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";

import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";

class DeleteOwnerProfileDialogComponent extends React.Component {
  constructor(props) {
    super(props);
  }
  onClick = () => {
    const { onClose, setChangeOwnerDialogVisible } = this.props;

    setChangeOwnerDialogVisible(true);
    onClose();
  };

  render() {
    console.log("DeleteOwnerProfileDialog render");
    const { t, tReady, visible, onClose } = this.props;

    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={onClose}
      >
        <ModalDialog.Header>{t("DeleteProfileTitle")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text fontSize="13px">{t("DeleteOwnerRestrictionText")}</Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="OwnerChangeBtn"
            label={t("Translations:OwnerChange")}
            size="normal"
            scale
            primary={true}
            onClick={this.onClick}
          />
          <Button
            key="CloseBtn"
            label={t("Common:CancelButton")}
            size="normal"
            scale
            onClick={onClose}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const DeleteOwnerProfileDialog = inject(({ peopleStore }) => ({
  setChangeOwnerDialogVisible:
    peopleStore.dialogStore.setChangeOwnerDialogVisible,
}))(
  observer(
    withTranslation(["DeleteSelfProfileDialog", "Translations", "Common"])(
      DeleteOwnerProfileDialogComponent
    )
  )
);

DeleteOwnerProfileDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default DeleteOwnerProfileDialog;
