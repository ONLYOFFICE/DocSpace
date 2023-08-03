import React, { useEffect } from "react";
import styled from "styled-components";
import ModalDialogContainer from "@docspace/client/src/components/dialogs/ModalDialogContainer";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import { useTranslation } from "react-i18next";
import { observer } from "mobx-react";
import { TextInput, Checkbox } from "@docspace/components";
import { useStore } from "SRC_DIR/store";

const StyledModal = styled(ModalDialogContainer)`
  #modal-dialog {
    min-height: 326px;
  }

  .create-docspace-input-block {
    padding: 16px 0;
  }

  .cancel-btn {
    display: inline-block;
    margin-left: 8px;
  }

  .create-docspace-checkbox {
    margin-bottom: 10px;
  }

  .create-docspace-input {
    width: 100%;
  }
`;

const CreatePortalDialog = () => {
  const [visit, setVisit] = React.useState<boolean>(false);
  const [restrictAccess, setRestrictAccess] = React.useState<boolean>(false);

  const { spacesStore, authStore } = useStore();

  const {
    createPortalDialogVisible: visible,
    setCreatePortalDialogVisible,
    createNewPortal,
    getAllPortals,
  } = spacesStore;

  const { t } = useTranslation(["Management", "Common"]);

  const [name, setName] = React.useState<string>("");

  const onHandleName = (e) => {
    setName(e.target.value);
  };

  const onHandleClick = async () => {
    const { firstName, lastName, email } = authStore.userStore.user;

    const data = {
      firstName,
      lastName,
      email,
      portalName: name,
      limitedAccessSpace: false,
    };

    await createNewPortal(data);
    await getAllPortals();
    onClose();
  };

  const onClose = () => {
    setCreatePortalDialogVisible(false);
  };

  const isNameError = name.length > 0 && (name.length > 100 || name.length < 6);
  return (
    <StyledModal
      isLarge
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>{t("CreatingDocSpace")}</ModalDialog.Header>
      <ModalDialog.Body className="create-docspace-body">
        <Text noSelect={true}>{t("CreateSpaceDescription")}</Text>
        <div className="create-docspace-input-block">
          <Text
            color="#333"
            fontSize="13px"
            fontWeight="600"
            style={{ paddingBottom: "5px" }}
          >
            {t("DocSpaceName")}
          </Text>
          <TextInput
            //     mask={[/[1-9]/, ".", "onlyoffice"]}
            // placeholderChar={[/[1-9]/, ".", "onlyoffice"]}
            onChange={onHandleName}
            hasError={isNameError}
            value={name}
            placeholder={t("addTranslationAndMask")}
            className="create-docspace-input"
          />
        </div>
        <div>
          <Checkbox
            className="create-docspace-checkbox"
            label={t("VisitSpace")}
            onChange={() => setVisit((visit) => !visit)}
            isChecked={visit}
          />

          <Checkbox
            label={t("RestrictAccess")}
            onChange={() => setRestrictAccess((access) => !access)}
            isChecked={restrictAccess}
          />
        </div>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="CreateButton"
          label={t("Common:Create")}
          size="normal"
          scale
          isDisabled={isNameError || name.length === 0}
          primary
          onClick={onHandleClick}
        />
        <Button
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onClose}
          scale
        />
      </ModalDialog.Footer>
    </StyledModal>
  );
};

export default observer(CreatePortalDialog);
